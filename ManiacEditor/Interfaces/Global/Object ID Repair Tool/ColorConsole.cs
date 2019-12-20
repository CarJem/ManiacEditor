using System;

namespace ManiacEditor
{
    /// <summary>
    /// Provides methods for easily writing to the system console in color.
    /// The color will be reset to system defaults on every call.
    /// </summary>
    internal static class ColorConsole
    {

        internal static void Write(string text, ConsoleColor foreColour)
        {
            Console.ForegroundColor = foreColour;
            Console.Write(text);
            Console.ResetColor();
        }

        internal static void Write(string textFormat, ConsoleColor foreColour, params object[] args)
        {
            Write(string.Format(textFormat, args), foreColour);
        }

        internal static void WriteLine(string text, ConsoleColor foreColour)
        {
            Console.ForegroundColor = foreColour;
            Console.WriteLine(text);
            Console.ResetColor();
        }

        internal static void WriteLine(string textFormat, ConsoleColor foreColour, params object[] args)
        {
            WriteLine(string.Format(textFormat, args), foreColour);
        }
    }
}
