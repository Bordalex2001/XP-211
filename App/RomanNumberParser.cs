using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace App
{
    public class RomanNumberParser
    {
        
        public static RomanNumber Parse(string input)
        {
            CheckSymbols(input);
            CheckPairs(input);
            CheckZeros(input);
            String argValue = input;
            input = input.Trim();
            CheckFormat(input);

            int prevValue = 0;
            int result = 0;
            foreach (char c in input) {
                int currentValue = DigitValue(c);
                result += currentValue - (currentValue > prevValue ? (prevValue << 1) : 0);
                prevValue = currentValue;
            }
            return new RomanNumber(result);
        }

        private static void CheckPairs(string input) {
            String argValue = input;
            input = input.Trim();
            int prevValue = 0;
            int position = 0;

            foreach (char c in input) {
                int currentValue = DigitValue(c);

                if (prevValue != 0 && prevValue < currentValue) {
                    // не всі комбінації "віднімання" припустимі
                    if (currentValue / prevValue > 10 || prevValue.ToString()[0] == '5') {
                        throw new FormatException($"RomanNumber.Parse('{argValue}') error: '{input[position - 1]}' before '{c}' in position {position - 1}");
                    }
                }
                prevValue = currentValue;
                position++;
            }
        }

        private static void CheckZeros(string input)
        {
            int position = input.IndexOf("N");
            if (input.Length > 1 && position != -1) {
                throw new FormatException($"RomanNumber.Parse('{input}') error: digit N must not be a numer in position {position}");
            }
        }

        private static void CheckFormat(String input)
        {
            int maxDigit = 0;
            bool wasMaxRepeat = false;
            for (int i = input.Length - 1; i >= 0; i -= 1)
            {
                char c = input[i];
                int digit = DigitValue(c);
                if (digit > maxDigit)
                {
                    maxDigit = digit;
                    wasMaxRepeat = false;
                }
                else if (digit == maxDigit)
                {
                    wasMaxRepeat = true;
                }
                else if (wasMaxRepeat)
                {
                    throw new FormatException(
                        $"RomanNumber.Parse('{input}') error invalid format: '{c}' misplaced at position {i}"
                    );
                }

            }
        }

        private static void CheckSymbols(String input)
        {
            String argValue = input;
            input = input.Trim();
            for (int i = 0; i < input.Length; i++)
            {
                try
                {
                    DigitValue(input[i]);
                }
                catch
                {
                    String dtl = Regex.IsMatch(input[i].ToString(), @"\s|\0")
                    ? "space-symbol could not be"
                    : $"illegal char '{input[i]}'";

                    throw new FormatException(
                        $"RomanNumber.Parse('{argValue}') error: {dtl} in position {i}"
                    );
                }
            }

        }

        public static int DigitValue(char digit) => digit switch {
            'N' => 0,
            'I' => 1,
            'V' => 5,
            'X' => 10,
            'L' => 50,
            'C' => 100,
            'D' => 500,
            'M' => 1000,
            _ => throw new ArgumentException($"RomanNumberParser.DigitValue('{digit}') error: invalid value '{digit}' for argument 'digit'"),
        };
    }
}