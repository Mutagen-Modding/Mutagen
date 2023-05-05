# BinaryReadStream
`IBinaryReadStream` is an interface that exposes binary extraction from a stream, with `BinaryReadStream` being a basic implementation.  The interface offers calls to read `int`, `short`, `uint`, `double`, `byte[]`, and even newer concepts such as `ReadOnlySpan<byte>` and `ReadOnlyMemorySlice<byte>`.

# MutagenBinaryReadStream
This is just a further extension on BinaryReadStream, offering additionally:
- A [[HeaderConstants]] object for reference when alignment is important
- An offset member, to help calculate position relative to a source file, if the MutagenBinaryReadStream happens to be a substream on only a slice of data.
