using App;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class RomanNumberTest
    {
        [TestMethod]
        public void ParseTest() {            
            TestCase[] validCases = [
                new("I",      1    ),
                new("II",     2    ),
                new("III",    3    ),
                new("IV",     4    ),
                new("IIII",   4    ),  
                new("IX",     9    ),
                new("XX",     20   ),
                new("X",      10   ),
                new("N",      0    ),
                new("XV",     15   ),
                new("XIX",    19   ),
                new("XXVI",   26   ),
                new("CIX",    109  ),
                new("CD",     400  ),
                new("MMXXIV", 2024 ),
                new("MMMM",   4000 ),

                new(" XX",    20   ),
                new("CC ",    200  ),
                new("  XX",   20   ),
                new("  CC  ", 200  ),
                new("\tIV",   4    ),
                new("CD\n",   400  ),
                new("XI\t\t", 11   ),
            ];
            foreach (TestCase validCase in validCases)  {
                Assert.AreEqual(
                    validCase.Value, 
                    RomanNumber.Parse(validCase.Source.ToString()!).Value, 
                    $"Valid Parser Test: '{validCase.Source}' => {validCase.Value}"
                );
            }

            var Assert_ThrowsException_Method =
                typeof(Assert)
                .GetMethods()
                .Where(m =>
                    m.Name == "ThrowsException" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters().First().ParameterType.Name == "Func`1")
                .FirstOrDefault();

            Assert.IsNotNull(
                Assert_ThrowsException_Method,
                "Reflection: 'ThrowsException' not found"
            );

            String[][] testCases8 = [
                [ "IXIX",  "I", "0" ],  
                [ "CXCXC", "X", "1" ],   
                [ "IXX",   "I", "0" ],  
                [ "IXXX",  "I", "0" ],
            ];
            foreach (var testCase in testCases8) {
                var ex = Assert.ThrowsException<FormatException>(
                    () => RomanNumber.Parse(testCase[0]),
                    $"RomanNumber.Parse('{testCase[0]}') must throw FormatException"
                );
                Assert.IsTrue(
                    ex.Message.Contains($"'{testCase[1]}'") &&
                    ex.Message.Contains($"position {testCase[2]}"),
                    $"ex.Message must contain error char '{testCase[1]}' and its position {testCase[2]}: " +
                    $"testCase='{testCase[0]}',ex.Message='{ex.Message}'"
                );
                Assert.IsTrue(
                    ex.Message.Contains("RomanNumber.Parse"),
                    $"ex.Message must contain origin (class and method): " +
                    $"testCase='{testCase[0]}',ex.Message='{ex.Message}'"
                );
                Assert.IsTrue(
                    ex.Message.Contains($"'{testCase[0]}'"),
                    $"ex.Message must contain input value '{testCase[0]}': " +
                    $"testCase='{testCase[0]}',ex.Message='{ex.Message}'"
                );
            }
        }

        [TestMethod]
        public void ToStringTest() {
            RomanNumber rn = new(1);
            Assert.IsNotNull ( rn.ToString() );
            Dictionary<int, string> testCases = new() 
            {
                {1, "I" },
                {2, "II" },
                {3, "III" },
                {4, "IV" },
                {5, "V" },
                {6, "VI" },
                {7, "VII" },
                {8, "VIII" },
                {9, "IX" },
                {10, "X" },
                {11, "XI" },
                {12, "XII" },
            };
            foreach(var pair in testCases) {
                Assert.AreEqual(
                    pair.Value, 
                    (rn with { Value = pair.Key }).ToString(), 
                    $"RomanNumber.ToString {pair.Key} --> {pair.Value}"
                );
            }
        }

        [TestMethod]
        public void CrossTest_Parse_ToString() { 
            for(int i = 0; i < 2000; i++) {
                Assert.AreEqual(
                    i,
                    RomanNumber.Parse(new RomanNumber(i).ToString()).Value,
                    $"Cross test for {i}"
                );
            }
        }

        // Завдання: скласти тест для майбутнього методу, який додаватиме два римські числа
        [TestMethod]
        public void PlusTest()
        {
            RomanNumber rn1 = new(1), rn2 = new(2);
            RomanNumber res = rn1.Plus(rn2);
            
            // Завдання: скласти твердження, що Sum має повертати об'єкт типу RomanNumber
            Assert.IsInstanceOfType<RomanNumber>(res);
            
            // Завдання: скласти твердження, що Sum має повертати об'єкт, який не є псевдонімом ані з rn2, ані з rn1 (Immutable)
            Assert.AreNotEqual(res, rn2);
            Assert.AreNotEqual(res, rn1);

            // Завдання: скласти 10 випадкових тестів, що підтверджують правильність алгоритму додавання (передбачити можливість додавання 0)
            for(int i = 0; i < 10; i++)
            {
                int n1 = Random.Shared.Next(0, 2000);
                int n2 = Random.Shared.Next(0, 2000);

                Assert.AreEqual(
                    n1 + n2,
                    new RomanNumber(n1).Plus(new(n2)).Value,
                    $"Test {n1} + {n2}"
                );
            }
        }
    }

    public class TestCase
    {
        public Object Source { get; set; }
        public Int32? Value { get; set; }
        public String? ExceptionMessage { get; set; }
        public Type? ExceptionType { get; set; }

        public TestCase()
        {
            Source = null!;
        }

        public TestCase(Object Source, Int32? Value)
        {
            this.Source = Source;
            this.Value = Value;
        }
        public TestCase(Object Source, String? ExceptionMessage, Type? ExceptionType = null)
        {
            this.Source = Source;
            this.ExceptionMessage = ExceptionMessage;
            this.ExceptionType = ExceptionType;
        }
    }

    public static class StringExtension
    {
        public static String F(this String str, params Object[] pars)
        {
            return String.Format(str, pars);
        }
    }

    public static class AssertExtension
    {
        private static MethodInfo Assert_ThrowsException_Method = 
            typeof(Assert)
                .GetMethods()
                .Where(m =>
                    m.Name == "ThrowsException" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2 &&
                    m.GetParameters().First().ParameterType.Name == "Func`1")
                .FirstOrDefault()!;

        public static void CheckTestCase(Func<Object?> action, TestCase testCase)
        {
            Type exType = testCase.ExceptionType ?? typeof(Exception);

            var Assert_ThrowsException_Generic =
                Assert_ThrowsException_Method
                .MakeGenericMethod(exType);

            dynamic? ex = Assert_ThrowsException_Generic.Invoke(null, [
                action,
                $"action('{testCase.Source}') must throw {exType.Name}"
            ]);

            Assert.AreEqual(
                testCase.ExceptionMessage,
                ex!.Message,
                $"ex.Message must contain part '{testCase.ExceptionMessage}': " +
                $"testCase='{testCase.Source}',ex.Message='{ex.Message}'"
            );
        }
    }
}