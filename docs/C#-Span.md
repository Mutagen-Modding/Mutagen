`Span`s are not a Mutagen concept, but rather a general C# concept.  However, since they are used extensively by Mutagen's parsing systems and they are a newer concept just recently added to C#, it will be covered to some extent here.

If you are more interested in Mutagen-specific concepts, you can skip this section.

# Spans are Sub-Sections of Arrays
A `Span<T>` is very similar to a `T[]`.  It points to a spot in memory where T objects reside, like an array.  Consider using a typical array, however, where you wanted some logic to process just a subsection of it.  You would either have to:
1) Pass the array to a function, with `start` and `end` indices of where you wanted to process.
2) Make a new smaller array, and copy the data over, using that array with just the interesting data to represent a "subsection" of the original array.

`Span<T>` would be an alternate way of handling the above problem.  It lets you "scope" to a small subsection of an array, without actually creating a new array.
```cs
byte[] originalArray = new byte[150];

// This scopes to only "contain" 5 bytes, framing indices 10-14 of the original array
Span<byte> subSection = originalArray.AsSpan(10, 5);

// This scopes in even farther, pointing to indices 2-3 of subSection,
// which are also the indices 12-13 of the original array.
Span<byte> evenSmallerSection = subSection.Slice(2, 2);

// Modify a byte in the smaller sub section, affecting the single common array at index 13
evenSmallerSection[1] = 123;
```

Changing the value in `evenSmallerSection` will affect all others; They point to the same underlying array and space in memory.

# Faster Substring Alternative
Trimming off or grabbing a few characters on a `string` using Substring() means allocating a whole 2nd string with mostly the same data, just some characters trimmed off.  This is a fairly wasteful operation.

`Span` concepts are great for Substring logic, as the original `string` memory can be reused while Span<char> just points to small substrings of the original string without copies or new allocations.

```cs
string str = "Junk Good Stuff Junk";
Span<char> origSpan = str.AsSpan();

// Equivalent to .Substring(5) to get rid of first Junk
Span<char> result = origSpan.Slice(4);

// Get rid of second junk by using TrimEnd of string "Junk"
result = result.TrimEnd("Junk".AsSpan());

// Trim whitespace
result = result.Trim();

// Print "Good Stuff"
System.Console.WriteLine(result.ToString());

// Note that the above call still has to call ToString(), which does allocate a new string with "Good Stuff".
// But at least we avoided several new transition strings while we were processing to our end result.

// Additionally, the WriteLine() API might be upgraded in C# to eventually take ReadOnlySpan<char> as input, too
```

# Interpreting Data as Another Type
Another cool trick `Span`s can do is overlay on top of a `byte[]` a `Span` of a different type:
```cs
byte[] someBytes = new byte[16];
// Fill with some data

// "Overlay" a uint Span on our bytes
Span<uint> uintSpan = someBytes.AsUInt32Span();

// Retrieve the uint contained in bytes 4-7
uint secondNumber = uintSpan[1];

// Will loop 4 times
for (int i = 0 ; i < uintSpan.Length ; i++) 
{
   // Print all 4 uints
   System.Console.WriteLine(uintSpan[i]);

   // Can also set original data.  Setting bytes to FFFF
   uintSpan[i] = uint.MaxValue;
}

// All bytes in someBytes now contain 255 / 0xFF
```

# Parsing Data from Span
Numeric primitives can be extracted from a `Span<byte>` fairly easily:
```cs
Span<byte> span = ...;

// Read int from span's bytes starting at index 0
int i = BinaryPrimitives.ReadInt32LittleEndian(span);

// Read short from span's bytes starting at index 7 (going to index 8)
short s = BinaryPrimitives.ReadInt16LittleEndian(span.Slice(7));
```

Strings (at least in the realm of Mutagen) are not as easily extracted, as `char` is 2 bytes in C#, while Bethesda binary has 1 byte chars and have null termination concepts.  There are utility functions provided by Mutagen for this, though.
```cs
Span<byte> span = ...;

// Find next null termination, convert bytes to standard C# string
string str = BinaryStringUtility.ProcessWholeToZString(span);

// Assumes entire span is relevant string information, and will turn
// all contained bytes into a resulting string
str = BinaryStringUtility.ToZString(span.Slice(11, 23));
```

# MemorySlice Alternative for Non-Stack Usage
One of the major downsides of `Span` is that it is a `ref struct` which can only "live" on the stack.  This means it cannot be a member of a class, or even be associated with async/await concepts, among other things.

In this case, `MemorySlice` is an alternative concept (subsection of an array) that can live outside of the stack.
