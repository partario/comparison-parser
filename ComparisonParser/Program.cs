using System;
using System.Collections.Generic;

namespace ComparisonParser
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var given in new List<string>
            {
                "colName = 1",
                "colName == 1",
                "colName = \"1\"",
                "colName < 1",
                "colName <= 1",
                "colName > 1",
                "colName >= 1",
                "colName != 1",
                "colName <> 1",
                "colName == \"this\"",
                "colName = \"this\"",
                "colName <> \"this\"",
                "    colName=99.9   ",
                "    colName=9",
                @"  colName = ""this \""is\"" a test""",
                "",
                "   fleeble lt 9.99 ",
                "   fleeble le 9.99 ",
                "   fleeble eq 9.99 ",
                "   fleeble EQ 9.99 ",
                "   fleeble ge 9.99 ",
                "   fleeble gt 9.99 ",
                "   blork != \"ooh\" ",
                "   blork <> \"ooh\" ",
                "   blork != \"ooh\" "
            }) {
                Console.WriteLine($"[{given}]:\n    {Parser.FromString(given)}");
            }
        }
    }
}

