using System.Net.NetworkInformation;
using System.Text.RegularExpressions;


namespace Utils
{
    public enum Alignment : int
    {
        LEFT,
        RIGHT,
        CENTER
    }

    public class Output
    {
        public static string Color(string text, string color)
        {
            return $"{color}{text}";
        }
        public static string Bold(string text)
        {
            return $"{ANSICodes.Effects.Bold}{text}";
        }
        public static string Reset(string text)
        {
            return $"{text}{ANSICodes.Reset}";
        }
        public static string Write(String text, bool newLine = false)
        {
            if (newLine)
            {
                Console.WriteLine(text);
            }
            else
            {
                Console.Write(text);
            }
            return text;
        }
        public static string WriteInRed(string text, bool newLine = false)
        {
            return Write(Color(text, ANSICodes.Colors.Red), newLine);
        }
        public static string WriteInGreen(string text, bool newLine = false)
        {
            return Write(Color(text, ANSICodes.Colors.Green), newLine);
        }
        public static string WriteInBlue(string text, bool newLine = false)
        {
            return Write(Color(text, ANSICodes.Colors.Blue), newLine);
        }
        public static string WriteInGray(string text, bool newLine = false)
        {
            return Write(Color(text, ANSICodes.Colors.Gray), newLine);
        }
        public static string WriteInYellow(string text, bool newLine = false)
        {
            return Write(Color(text, ANSICodes.Colors.Yellow), newLine);
        }
        public static string Align(string text, Alignment alignment = Alignment.LEFT, bool newLine = false)
        {
            if (text.Split("\n").Length > 1)
            {
                string[] lines = text.Split("\n");
                List<string> lineSegments = new List<string>();
                foreach (string line in lines)
                {
                    lineSegments.Add(Align($"{line}", alignment));
                }
                return string.Join("", lineSegments);
            }

            if (alignment != Alignment.LEFT)
            {
                int width = System.Console.WindowWidth;
                int textWidth = text.Length;
                int paddingLeft = 0;
                int paddingRight = 0;

                if (alignment == Alignment.CENTER)
                {
                    paddingRight = paddingLeft = (int)Math.Floor((width - textWidth) / 2.0);
                }
                else if (alignment == Alignment.RIGHT)
                {
                    paddingLeft = width - textWidth;
                }

                paddingLeft = Math.Clamp(paddingLeft, 0, int.MaxValue);
                paddingRight = Math.Clamp(paddingRight, 0, int.MaxValue);
                text = $"{new string(' ', paddingLeft)}{text}{new string(' ', paddingRight)}";
            }

            if (newLine) { text += "\n"; }

            return text;
        }
        public static string ColorizeWords(string text, string primaryColor, string secondaryColor)
        {
            string modifiedInput = secondaryColor + text + ANSICodes.Reset;

            return Regex.Replace(
                modifiedInput,
                @"\*(.*?)\*",
                match => ANSICodes.Reset + primaryColor + match.Groups[1].Value + ANSICodes.Reset + secondaryColor
            );
        }

    }

}
