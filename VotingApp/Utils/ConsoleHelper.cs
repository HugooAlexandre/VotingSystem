namespace VotingApp.Utils
{
    public static class ConsoleHelper
    {
        public static void WriteSuccess(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ForegroundColor = originalColor;
        }
        
        public static void WriteError(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ForegroundColor = originalColor;
        }
        
        public static void WriteWarning(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ForegroundColor = originalColor;
        }
        
        public static void WriteInfo(string message)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ForegroundColor = originalColor;
        }
        
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine(CenterText(title, 60));
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
        }
        
        public static void PrintSeparator()
        {
            Console.WriteLine(new string('-', 60));
        }
        
        public static bool ConfirmAction(string message)
        {
            Console.Write($"{message} (S/N): ");
            var response = Console.ReadLine()?.Trim().ToUpper();
            return response == "S" || response == "SIM";
        }
        
        public static void PressToContinue()
        {
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
        
        private static string CenterText(string text, int width)
        {
            if (text.Length >= width) return text;
            int padding = (width - text.Length) / 2;
            return text.PadLeft(padding + text.Length).PadRight(width);
        }
    }
}