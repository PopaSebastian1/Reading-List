using Reading_List.Domain.Exceptions;

namespace Reading_List.Application.Handlers
{
    public static class ConsoleInputHandler
    {
        public static int ReadInt(string prompt, Func<int, bool>? validate = null, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (int.TryParse(raw, out var value))
                {
                    if (validate == null || validate(value))
                        return value;
                    throw new InvalidInputException($"Invalid integer '{value}'.");
                }
                Console.WriteLine("Please enter a valid integer.");
            }
            throw new OperationCanceledException();
        }

        public static decimal ReadDecimal(string prompt, Func<decimal, bool>? validate = null, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine()?.Replace(',', '.');
                if (decimal.TryParse(raw,
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var value))
                {
                    if (validate == null || validate(value))
                        return value;
                    throw new InvalidInputException($"Invalid decimal '{value}'.");
                }
                Console.WriteLine("Please enter a valid number.");
            }

            throw new OperationCanceledException();
        }

        public static string ReadNonEmptyString(string prompt, CancellationToken ct = default)
        {
            while (!ct.IsCancellationRequested)
            {
                Console.Write(prompt);
                var raw = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(raw))
                    return raw.Trim();
                throw new InvalidInputException("Value cannot be empty.");
            }

            throw new OperationCanceledException();
        }
    }
}
