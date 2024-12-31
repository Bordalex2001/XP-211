using App;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class RomanNumberParserTest
    {
        [TestMethod]
        public void ParseTest()
        {
            foreach (TestCase validCase in validCases.Concat(validCasesSpaced)) {
                Assert.AreEqual(
                    validCase.Value,
                    RomanNumber.Parse(validCase.Source.ToString()!).Value,
                    $"Valid Parser Test: '{validCase.Source}' => {validCase.Value}"
                );
            }
        }

        [TestMethod]
        public void CheckPairsTest()
        {
            String methodName = "CheckPairs";
            var method = typeof(RomanNumberParser).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, $"{methodName} method is inaccessible");

            foreach (TestCase validCase in validCases) {
                method!.Invoke(null, [validCase.Source]);
            }

            foreach (var testCase in invalidPairsCases) {
                var ex = Assert.ThrowsException<TargetInvocationException>(
                    () => method!.Invoke(null, [testCase.Source]),
                    $"TargetInvocationException expected from Reflect-Invoke"
                );

                Assert.IsInstanceOfType<FormatException>(
                    ex.InnerException,
                    $"RomanNumber.Parse('{testCase.Source}') must throw FormatException"
                );
            }
        }

        [TestMethod]
        public void CheckZerosTest()
        {
            String methodName = "CheckZeros";
            var method = typeof(RomanNumberParser).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, $"{methodName} method is inaccessible");

            foreach (TestCase validCase in validCases) {
                method!.Invoke(null, [validCase.Source]);
            }

            foreach (var testCase in invalidZerosCases) {
                var ex = Assert.ThrowsException<TargetInvocationException>(
                    () => method!.Invoke(null, [testCase.Source]),
                    $"TargetInvocationException expected from Reflect-Invoke"
                );

                Assert.IsInstanceOfType<FormatException>(
                    ex.InnerException,
                    $"RomanNumber.Parse('{testCase.Source}') must throw FormatException"
                );
            }
        }

        [TestMethod]
        public void DigitValueTest()
        {
            TestCase[] testCases = [
                new('N', 0    ), 
                new('I', 1    ),
                new('V', 5    ),
                new('X', 10   ),
                new('L', 50   ),
                new('C', 100  ),
                new('D', 500  ),
                new('M', 1000 ),
            ];
            foreach (var testCase in testCases)
            {
                Assert.AreEqual(
                    testCase.Value,
                    RomanNumberParser.DigitValue((char)testCase.Source),
                    $"DigitValue('{testCase.Source}') -> {testCase.Value}"
                );
            }
            String tpl = "RomanNumberParser.DigitValue('{0}') error: invalid value '{0}' for argument 'digit'";
            Type exType = typeof(ArgumentException);
            TestCase[] exCases =
            [
                new('1', tpl.F('1'), exType),
                new('q', tpl.F('q'), exType),
                new('c', tpl.F('c'), exType),
                new('=', tpl.F('='), exType),
                new('/', tpl.F('/'), exType),
            ];
            foreach (var exCase in exCases)
            {
                AssertExtension.CheckTestCase(
                    () => RomanNumberParser.DigitValue((char)exCase.Source),
                    exCase
                );
            }
        }

        [TestMethod]
        public void CheckFormatTest()
        {
            String methodName = "CheckFormat";
            var method = typeof(RomanNumberParser).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, $"{methodName} method is inaccessible");

            foreach(TestCase validCase in validCases) {
                method!.Invoke(null, [validCase.Source]);
            }
            
        }

        [TestMethod]
        public void CheckSymbolsTest()
        {
            var method = typeof(RomanNumberParser).GetMethod("CheckSymbols", BindingFlags.NonPublic | BindingFlags.Static);
            Assert.IsNotNull(method, "CheckSymbols method is inaccessible");

            foreach (TestCase validCase in validCases.Concat(validCasesSpaced)) {
                method!.Invoke(null, [validCase.Source]);
            }

            foreach (var testCase in invalidSymbolCases.Concat(invalidSpaceCases)) {
                var ex = Assert.ThrowsException<TargetInvocationException>(
                    () => method!.Invoke(null, [testCase.Source]),
                    $"TargetInvocationException expected from Reflect-Invoke"
                );

                Assert.IsInstanceOfType<FormatException>(
                    ex.InnerException,
                    $"RomanNumber.Parse('{testCase.Source}') must throw FormatException"
                );
            }
        }

        private TestCase[] validCases = [
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
        ];

        private TestCase[] validCasesSpaced = [
            new(" XX",    20   ),
            new("CC ",    200  ),
            new("  XX",   20   ),
            new("  CC  ", 200  ),
            new("\tIV",   4    ),
            new("CD\n",   400  ),
            new("XI\t\t", 11   ),
        ];

        static String src = "RomanNumber.Parse('{0}') error: ";
        static String tpl = src + "illegal char '{1}' in position {2}";
        static Type exType = typeof(FormatException);
        TestCase[] invalidSymbolCases = [
            new("IW",  tpl.F("IW",   'W', 1), exType),
            new("SI",  tpl.F("SI",   'S', 0), exType),
            new("IXW", tpl.F("IXW",  'W', 2), exType),
            new("IEX", tpl.F("IEX",  'E', 1), exType),
            new("CDX1",tpl.F("CDX1", '1', 3), exType),
            new("IQ",  tpl.F("IQ",   "Q", "1"), exType),
            new("AI",  tpl.F("AI",   "A", "0"), exType),
            new("IXF", tpl.F("IXF",  "F", "2"), exType),
            new("IGX", tpl.F("IGX",  "G", "1"), exType),
            new("CDX2",tpl.F("CDX2", "2", "3"), exType),
        ];

        static String tpl2 = src + "'{1}' before '{2}' in position {3}";
        TestCase[] invalidPairsCases = [
            new("IL", tpl2.F("IL", 'I', 'L', 0), exType),
            new("IC", tpl2.F("IC", 'I', 'C', 0), exType),
            new("ID", tpl2.F("ID", 'I', 'D', 0), exType),
            new("IM", tpl2.F("IM", 'I', 'M', 0), exType),
            new("XD", tpl2.F("XD", 'X', 'D', 0), exType),
            new("XM", tpl2.F("XM", 'X', 'M', 0), exType),
            new("VX", tpl2.F("VX", 'V', 'X', 0), exType),
            new("VM", tpl2.F("VM", 'V', 'M', 0), exType),
        ];

        static String tpl3 = src + "digit 'N' must not be in number in position {1}";
        TestCase[] invalidZerosCases = [
            new("VN", tpl3.F("VN", 1), exType),
            new("NV", tpl3.F("NV", 0), exType),
        ];

        static String tpl4 = src + "space-symbol could not be in position {1}";
        TestCase[] invalidSpaceCases = [
            new("V I",  tpl4.F("V I",  1), exType),
            new("X\tX", tpl4.F("X\tX", 1), exType),
            new("X\nX", tpl4.F("X\nX", 1), exType),
            new("X\0X", tpl4.F("X\0X", 1), exType),
        ];

    }
}