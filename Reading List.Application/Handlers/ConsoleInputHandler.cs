using Reading_List.Domain.Models;

namespace Reading_List.Application.ErrorHandlers
{
    internal static class ConsoleInputHandler
    {
        public static async Task<Result<int>> ReadIntAsync(string prompt, Func<int, bool>? validate = null, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (int.TryParse(raw, out var value))
                {
                    if (validate is null || validate(value))
                        return Result<int>.Success(value);
                    Console.WriteLine("Invalid value.");
                }
                else
                {
                    Console.WriteLine("Please enter a valid integer.");
                }
                await Task.Yield();
            }
            return Result<int>.Failure("Operation cancelled.");
        }

        public static async Task<Result<decimal>> ReadDecimalAsync(string prompt, Func<decimal, bool>? validate = null, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (decimal.TryParse(raw, out var value))
                {
                    if (validate is null || validate(value))
                        return Result<decimal>.Success(value);
                    Console.WriteLine("Invalid value.");
                }
                else
                {
                    Console.WriteLine("Please enter a valid number.");
                }
                await Task.Yield();
            }
            return Result<decimal>.Failure("Operation cancelled.");
        }

        public static async Task<Result<string>> ReadNonEmptyStringAsync(string prompt, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(raw))
                    return Result<string>.Success(raw.Trim());
                Console.WriteLine("Value cannot be empty.");
                await Task.Yield();
            }
            return Result<string>.Failure("Operation cancelled.");
        }
    }
}