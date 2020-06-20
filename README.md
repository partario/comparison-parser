# ComparisonParser

Parses a simple comparison expression.

All accepted expressions consist of three parts, in this order:
1. A column name
1. A comparison operator
1. A value to compare against.

## Column name part
The column name should be a contiguous group of alphabetic, numeric and underscore characters.
Upper and lower case are preserved.

## Value part
Recognized value types:
-  number: integer or floating point, signed or unsigned. No commas are allowed, and no leading zeroes.
- a string. All strings start and end with a double-quote (") character. Double-quote characters inside the string are escaped with a backslash; backslashes inside the string are also escaped with a backslash. Whitespace inside the string value is preserved.
- null: the literal text "null" (without quotes) is used for the value.

## Comparison operator part
The comparison operators recognized vary by the value type:
- number:
    - `<` -- Less than. May also be written as `lt`.
    - `<=` -- Less than or equal. May also be written `le`.
    - `=` -- Equal. May also be written `==` or `eq`.
    - `>=` -- Greater than or equal. May also be written `ge`.
    - `>` -- Greater than. May also be written `gt`.
    - `!=` -- Not equals. May also be written `<>` or `ne`.
- string:
    - `=` -- Equals. May also be written `==` or `eq`.
    - `!=` -- Not equals. May also be written `<>` or `ne`.
- null:
    - `is` -- True when null.
    - `is not` -- True when not null.

## Optional whitespace
Any amount of whitespace may be placed at the beginning or end of the string, or between the three parts, and will be ignored.
Whitespace inside a string value is preserved.

## Notes
Most of this should be what you expect.

The `ComparisonParser.Parser()` method accepts an optional parameter that disallows the null tests, for use where no value can ever be null.