namespace Reading_List.Application.Abstractions
{
    public interface ILoggerService
    {
        Task LogInfoAsync(string message, CancellationToken ct = default);
        Task LogWarningAsync(string message, CancellationToken ct = default);
        Task LogErrorAsync(string message, Exception? ex = null, CancellationToken ct = default);
    }
}
