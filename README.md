
# ComparisonParser

Parses a simple comparison expression.

## Summary

```C#
var fromUser = "CountByMarket > 3479";
var parsed = Comparison.FromString(fromUser);
if (!parsed.ValidComparison) {
    throw new ArgumentException($"MatchExpression \"{fromUser}\" is invalid.");
}
Console.WriteLine($"Expression \"{fromUser}\" is valid.");
Console.WriteLine(parsed);
```
gives the output:
```
Expression "CountByMarket > 3479" is valid.
Comparison(UserString="CountByMarket > 3479", ValidComparison=true, ColumnName="CountByMarket", ComparisonType=">", Value="3479", ValueType="number")
```

## Expressions: the details

All accepted expressions consist of three parts, in this order:
1. A column name
1. A comparison operator
1. A value to compare against.

### Column name part
The column name should be a contiguous group of alphabetic, numeric and underscore characters.
Upper and lower case are preserved.

### Value part
Recognized value types:
- number: integer or floating point, signed or unsigned. No commas are allowed, and no leading zeroes.
- string: All strings start and end with a double-quote (") character. Double-quote characters inside the string are escaped with a backslash; backslashes inside the string are also escaped with a backslash. Whitespace inside the string value is preserved.
- null: the literal text "null" (without quotes) is used for the value.

### Comparison operator part
The comparison operators recognized vary by the value type:
- number:
    - `<` -- Less than. May also be written as `lt`.
    - `<=` -- Less than or equal. May also be written `le`.
    - `=` -- Equal. May also be written `==` or `eq`.
    - `>=` -- Greater than or equal. May also be written `ge`.
    - `>` -- Greater than. May also be written `gt`.
    - `!=` -- Not equal. May also be written `<>` or `ne`.
- string:
    - `=` -- Equal. May also be written `==` or `eq`.
    - `!=` -- Not equal. May also be written `<>` or `ne`.
- null:
    - `is` -- True when null.
    - `is not` -- True when not null.

### Optional whitespace
Any amount of whitespace may be placed at the beginning or end of the string, or between the three parts, and will be ignored.
Whitespace inside a string value is preserved.

### Notes
The expressions should be unsurprising, mostly.

The `Comparison.FromString()` method accepts an optional parameter that disallows the null tests, for use where no value can ever be null.
