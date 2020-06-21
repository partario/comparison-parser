using System;
using System.Collections.Generic;

namespace ComparisonParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var fromUser = "CountByMarket > 3479";
            var parsed = Comparison.FromString(fromUser);
            if (!parsed.ValidComparison) {
                throw new ArgumentException($"MatchExpression \"{fromUser}\" is invalid.");
            }
            Console.WriteLine($"Expression \"{fromUser}\" is valid.");
            Console.WriteLine(parsed);
        }
    }
}

