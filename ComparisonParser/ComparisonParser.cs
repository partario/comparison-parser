using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ComparisonParser
{
    public class Parser {
        private static bool setup = false;
        private static string comparisonExpressionRx;
        private static Dictionary<string, string> normalComparisonNames;

        const string columnNameRx = @"(?<columnName> [A-Za-z0-9_]+ )";
        const string numberComparisonRx = @"(?i)(?<numberComparison> [<=>]=? | [lg][te] | eq | ne | <> | != )";
        const string numberRx = @"(?<number> [-+]? (?: [1-9]\d*(?:\.\d*)? | 0?\.\d+ ))";
        const string stringComparisonRx = @"(?i)(?<stringComparison> ==? | eq | ne | <> | != )";
        const string stringRx = @" "" (?<string> (?: \\[""\\] | [^""] )* ) "" ";  // \" is a ",  \\ is a \, \[otherwise] is \[otherwise]
        const string nullRx = @"(?i)(?<null> is \s* (?: (?<not> not ) \s* )? null )";
        const string optionalWhiteSpaceRx = @"\s*";

        static void Setup() {
            if (! setup) {
                comparisonExpressionRx = $@"
                    ^
                    {optionalWhiteSpaceRx}
                    {columnNameRx}
                    {optionalWhiteSpaceRx}
                    (?:
                        {stringComparisonRx}
                        {optionalWhiteSpaceRx}
                        {stringRx}
                        |
                        {numberComparisonRx}
                        {optionalWhiteSpaceRx}
                        {numberRx}
                        |
                        {nullRx}
                    )
                    {optionalWhiteSpaceRx}
                    $
                ";

                normalComparisonNames = new Dictionary<string,string>()
                {
                    { "<>", "!=" },
                    { "ne", "!=" },
                    { "lt", "<" },
                    { "le", "<=" },
                    { "==", "=" },
                    { "eq", "=" },
                    { "gt", ">" },
                    { "ge", ">=" }
                };

                setup = true;
            }
        }
        public static string Escape(string escaped) {
            var unescaped = Regex.Replace(escaped, @"([\\""])", "\\$1");
            return unescaped;
        }
        public static string Unescape(string escaped) {
            var unescaped = Regex.Replace(escaped, @"\\([\\""])", "$1");
            return unescaped;
        }
        static string NormalizeTest(string testop) {
            return normalComparisonNames.GetValueOrDefault(testop.ToLower(), testop);
        }

        public static Comparison FromString(string userString, bool nullTestsAllowed = true) {
            Setup();

            Match m = Regex.Match(userString, comparisonExpressionRx, RegexOptions.IgnorePatternWhitespace);
            if (m.Groups["number"].Value != "") {
                return new Comparison()
                {
                    UserString = userString,
                    ValidComparison = true,
                    ColumnName = m.Groups["columnName"].Value,
                    ComparisonType = NormalizeTest(m.Groups["numberComparison"].Value),
                    Value = m.Groups["number"].Value,
                    ValueType = "number",
                };
            }
            else if (m.Groups["string"].Value != "") {
                return new Comparison()
                {
                    UserString = userString,
                    ValidComparison = true,
                    ColumnName = m.Groups["columnName"].Value,
                    ComparisonType = NormalizeTest(m.Groups["stringComparison"].Value),
                    Value = Unescape(m.Groups["string"].Value),
                    ValueType = "string",
                };
            }
            else if (m.Groups["null"].Value != "" && nullTestsAllowed) {
                return new Comparison()
                {
                    UserString = userString,
                    ValidComparison = true,
                    ColumnName = m.Groups["columnName"].Value,
                    ComparisonType = (m.Groups["not"].Value != "") ? "!=" : "=",
                    Value = "null",
                    ValueType = "null"

                };
            }
            else {
                return new Comparison() { UserString = userString, ValidComparison = false };
            }
        }
    }

    public class Comparison {
        public bool ValidComparison { get; set; }
        public string ColumnName { get; set; }
        public string ComparisonType { get; set; }
        public string Value { get; set; }
        public string ValueType { get; set; }
        public string UserString { get; set; }

        public override string ToString() {
            return "Comparison(" +
                $"UserString=\"{Parser.Escape(UserString)}\", " +
                $"ValidComparison={ValidComparison.ToString().ToLower()}, " +
                $"ColumnName=\"{ColumnName}\", " +
                $"ComparisonType=\"{ComparisonType}\", " +
                $"Value=\"{Value}\", " +
                $"ValueType=\"{ValueType}\"" +
            ")";
        }
    }
}
