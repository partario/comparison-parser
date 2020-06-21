using NUnit.Framework;

namespace ComparisonParser.Tests
{
    [TestFixture]
    class Tests
    {
        [TestCase("")]   // empty string
        [TestCase("= 1")]  // missing ColumnName
        [TestCase("colName 1")]  // missing comparison operator
        [TestCase("colName =")]  // missing value
        [TestCase("*oops* < 5¢")]  // bad column name, bad number
        [TestCase("colName = 01")]  // integers can't start with leading zeroes
        [TestCase("colName = 01.01")]  // floats can't start with leading zeroes (unless that's the only digit before the decimal point)
        [TestCase("colName is 5")]  // "is" is only valid for "is null" or "is not null"
        [TestCase("colName is not \"this\"")]  // "is not" is only valid for "is not null"
        public void FromString_InvalidStrings_ReturnNotValid(string userProvided)
        {
            var result = ComparisonParser.FromString(userProvided);

            Assert.IsFalse(result.ValidComparison, $"\"{ComparisonParser.Escape(userProvided)}\" should not be accepted as a valid comparison");
        }

        // all the number comparison types
        [TestCase("colName < 12",  "number", "colName", "<",  "12")]
        [TestCase("colName lt 12", "number", "colName", "<",  "12")]
        [TestCase("colName <= 12", "number", "colName", "<=", "12")]
        [TestCase("colName le 12", "number", "colName", "<=", "12")]
        [TestCase("colName = 12",  "number", "colName", "=",  "12")]
        [TestCase("colName == 12", "number", "colName", "=",  "12")]
        [TestCase("colName eq 12", "number", "colName", "=",  "12")]
        [TestCase("colName >= 12", "number", "colName", ">=", "12")]
        [TestCase("colName ge 12", "number", "colName", ">=", "12")]
        [TestCase("colName > 12",  "number", "colName", ">",  "12")]
        [TestCase("colName gt 12", "number", "colName", ">",  "12")]
        [TestCase("colName != 12", "number", "colName", "!=", "12")]
        [TestCase("colName <> 12", "number", "colName", "!=", "12")]
        [TestCase("colName ne 12", "number", "colName", "!=", "12")]
        // all the string comparison types
        [TestCase("colName = \"this\"",  "string", "colName", "=",  "this")]
        [TestCase("colName == \"this\"", "string", "colName", "=",  "this")]
        [TestCase("colName eq \"this\"", "string", "colName", "=",  "this")]
        [TestCase("colName != \"this\"", "string", "colName", "!=", "this")]
        [TestCase("colName <> \"this\"", "string", "colName", "!=", "this")]
        [TestCase("colName ne \"this\"", "string", "colName", "!=", "this")]
        // check case-insensitivity of comparison operator
        [TestCase("colName lt 12", "number", "colName", "<", "12")]
        [TestCase("colName lT 12", "number", "colName", "<", "12")]
        [TestCase("colName Lt 12", "number", "colName", "<", "12")]
        [TestCase("colName LT 12", "number", "colName", "<", "12")]
        // null / not-null tests
        [TestCase("colName is null",     "null", "colName", "=",  "null")]
        [TestCase("colName is not null", "null", "colName", "!=", "null")]
        // optional whitespace
        [TestCase("colName=1", "number", "colName", "=", "1")]
        [TestCase("colName = 1", "number", "colName", "=", "1")]
        [TestCase("    colName    =    1  ", "number", "colName", "=", "1")]
        // number values: integers, floats < 1, floats < 1 with leading zero, floats > 1
        [TestCase("colName = 877",      "number", "colName", "=", "877")]
        [TestCase("colName = +877",     "number", "colName", "=", "+877")]
        [TestCase("colName = -877",     "number", "colName", "=", "-877")]
        [TestCase("colName = .71",      "number", "colName", "=", ".71")]
        [TestCase("colName = +.71",     "number", "colName", "=", "+.71")]
        [TestCase("colName = -.71",     "number", "colName", "=", "-.71")]
        [TestCase("colName = 0.313",    "number", "colName", "=", "0.313")]
        [TestCase("colName = +0.313",   "number", "colName", "=", "+0.313")]
        [TestCase("colName = -0.313",   "number", "colName", "=", "-0.313")]
        [TestCase("colName = .314",     "number", "colName", "=", ".314")]
        [TestCase("colName = +.314",    "number", "colName", "=", "+.314")]
        [TestCase("colName = -.314",    "number", "colName", "=", "-.314")]
        [TestCase("colName = 552.",     "number", "colName", "=", "552.")]
        [TestCase("colName = +552.",    "number", "colName", "=", "+552.")]
        [TestCase("colName = -552.",    "number", "colName", "=", "-552.")]
        [TestCase("colName = 26.0101",  "number", "colName", "=", "26.0101")]
        [TestCase("colName = +26.0101", "number", "colName", "=", "+26.0101")]
        [TestCase("colName = -26.0101", "number", "colName", "=", "-26.0101")]
        // string value with embedded quotes
        [TestCase("colName = \"\\\"\"", "string", "colName", "=", "\"")]
        // multiline input doesn't affect numbers
        [TestCase("\n\n  colName\n    =\n    33\n\n\r\n", "number", "colName", "=", "33")]
        // multiline string values retain their internal whitespace, including newlines
        [TestCase("colName  = \n\n   \"this\n  that   \n    the other\"    \n    ", "string", "colName", "=", "this\n  that   \n    the other")]
        // column names
        [TestCase("a = 1", "number", "a", "=", "1")]
        [TestCase("aaaaa = 1", "number", "aaaaa", "=", "1")]
        [TestCase("a0 = 1", "number", "a0", "=", "1")]
        [TestCase("_a = 1", "number", "_a", "=", "1")]
        [TestCase("_0a = 1", "number", "_0a", "=", "1")]
        [TestCase("a0_ = 1", "number", "a0_", "=", "1")]
        [TestCase("a0_000_3 = 1", "number", "a0_000_3", "=", "1")]
        public void FromString_ValidExpression_GivesCorrectValues(
            string userProvided,
            string valueType,
            string columnName,
            string comparisonType,
            string value
        ) {
           var result = ComparisonParser.FromString(userProvided);

           Assert.IsTrue(result.ValidComparison, $"\"{ComparisonParser.Escape(userProvided)}\" should be accepted as a valid comparison");
           Assert.That(result.ValueType == valueType, $"\"{ComparisonParser.Escape(userProvided)}\" should have value type \"{valueType}\" but had \"{result.ValueType}\"");
           Assert.That(result.ColumnName == columnName, $"\"{ComparisonParser.Escape(userProvided)}\" should have column name \"{columnName}\" but had \"{result.ColumnName}\"");
           Assert.That(result.ComparisonType == comparisonType, $"\"{ComparisonParser.Escape(userProvided)}\" should have comparison type \"{comparisonType}\" but had \"{result.ComparisonType}\"");
           Assert.That(result.Value == value, $"\"{ComparisonParser.Escape(userProvided)}\" should have value \"{value}\" but had \"{result.Value}\"");
        }

        [TestCase("", "")]
        [TestCase("simple", "simple")]
        [TestCase(@"\", @"\\")]
        [TestCase(@"C:\Windows\System32\drivers\etc\hosts.txt", @"C:\\Windows\\System32\\drivers\\etc\\hosts.txt")]
        [TestCase(@"""", @"\""")]
        [TestCase(@"""Yawn,"" he yawned. ""Sigh.""", @"\""Yawn,\"" he yawned. \""Sigh.\""")]
        public void Escape_GivenAString_ReturnWithQuotesAndBackslashesEscaped(string input, string expected) {
            var got = ComparisonParser.Escape(input);

            Assert.That(got == expected, $"Escaped version of [{input}] should be [{expected}], not [{got}]");
        }
        [TestCase("", "")]
        [TestCase(@"\", @"\")]
        [TestCase(@"This is \x1b[1mbold\x1b[0m", @"This is \x1b[1mbold\x1b[0m")]
        [TestCase(@"Text with \""escaped\"" quotes", @"Text with ""escaped"" quotes")]
        [TestCase(@"Text with a backslash: \\", @"Text with a backslash: \")]
        public void Unescape_GivenAString_ReturnWithBackslashesRemoved(string input, string expected) {
            var got = ComparisonParser.Unescape(input);

            Assert.That(got == expected, $"Unescaped version of [{input}] should be [{expected}], not [{got}]");
        }
    }
}
