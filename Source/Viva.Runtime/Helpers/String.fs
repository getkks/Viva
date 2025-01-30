namespace Viva.Runtime.Helpers

open System

[<AutoOpen>]
module String =
    let contains (left: char ReadOnlySpan) (right: _ ReadOnlySpan) = left.IndexOf right > -1
    let equals left (right: _ ReadOnlySpan) = right.SequenceEqual left

    let equalsIgnoreCase left (right: _ ReadOnlySpan) =
        right.Equals(left, StringComparison.OrdinalIgnoreCase)

    let isNullOrEmpty str = String.IsNullOrEmpty str
    let isNullOrWhiteSpace str = String.IsNullOrWhiteSpace str
    let startsWith (left: _ ReadOnlySpan) (right: _ ReadOnlySpan) = left.StartsWith right

    let startsWithIgnoreCase left (right: _ ReadOnlySpan) =
        right.StartsWith(left, StringComparison.OrdinalIgnoreCase)

    let trim(str: _ ReadOnlySpan) = str.Trim()
    let (|Contains|_|) left right = contains left right
    let (|Equals|_|) left right = equals left right
    let (|EqualsIgnoreCase|_|) left right = equalsIgnoreCase left right
    let (|IsNullOrEmpty|_|) str = isNullOrEmpty str
    let (|IsNullOrWhiteSpace|_|) str = isNullOrWhiteSpace str
    let (|StartsWithIgnoreCase|_|) left right = startsWithIgnoreCase left right
    let (|StartsWith|_|) left right = startsWith left right