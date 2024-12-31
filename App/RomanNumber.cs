using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App
{
    public record RomanNumber(int Value)
    {
        public static RomanNumber Parse(string input) => RomanNumberParser.Parse(input);

        public override string ToString() {
            Dictionary<int, string> ranges = new() 
            {
                {1, "I" },
                {4, "IV" },
                {5, "V" },
                {9, "IX" },
                {10, "X" },
            };
            int n = Value;
            StringBuilder sb = new();
            foreach(int range  in ranges.Keys.OrderByDescending(k => k)) {
                while (n >= range) {
                    n -= range;
                    sb.Append(ranges[range]);
                }
            }
            return sb.ToString();
        }

        // Завдання: створити метод додавання, що проходить тест
        public RomanNumber Plus(RomanNumber second) => this with { Value = Value + second.Value };
    }
}