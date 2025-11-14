using Reading_List.Application.Abstractions;

namespace Reading_List.Tests.TestsHelpers
{
    public sealed class TestLogger : ILoggerService
    {
        public readonly List<string> Infos = new();
        public readonly List<string> Warnings = new();
        public readonly List<string> Errors = new();

        public Task LogInfoAsync(string message, CancellationToken ct = default)
        { Infos.Add(message); return Task.CompletedTask; }

        public Task LogWarningAsync(string message, CancellationToken ct = default)
        { Warnings.Add(message); return Task.CompletedTask; }

        public Task LogErrorAsync(string message, Exception? ex = null, CancellationToken ct = default)
        {
            var msg = ex is null ? message : $"{message} :: {ex.GetType().Name}: {ex.Message}";
            Errors.Add(msg);
            return Task.CompletedTask;
        }
    }

}
