using Reading_List.Application.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reading_List.Application.Services
{
    public class LoggerService : ILoggerService, IDisposable
    {
        private readonly string logFilePath;
        private readonly StreamWriter logStreamWriter;

        public LoggerService()
        { 
            var solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\.."));
            var dataDir = Path.Combine(solutionRoot, "Reading List.Infrastructure", "Data");
            var directory = Path.Combine(dataDir, "Logs");
            Directory.CreateDirectory(directory);

            var path = Path.Combine(directory, $"logs.txt");
            logStreamWriter = new StreamWriter(path, append: false)
            {
                AutoFlush = true
            };
        }
        public async Task LogInfoAsync(string message, CancellationToken ct = default)
            => await WriteAsync("INFO", message, ct);

        public async Task LogWarningAsync(string message, CancellationToken ct = default)
            => await WriteAsync("WARN", message, ct);

        public async Task LogErrorAsync(string message, Exception? ex = null, CancellationToken ct = default)
            => await WriteAsync("ERROR", message + (ex != null ? $"\n{ex}" : ""), ct);

        private async Task WriteAsync(string level, string message, CancellationToken ct)
        {
            var line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            await logStreamWriter.WriteLineAsync(line.AsMemory(), ct);
        }

        public void Dispose()
        {
            logStreamWriter?.Dispose();
        }
    }
}
