``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT


```
|          Method |     Mean |    Error |   StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |---------:|---------:|---------:|-------:|------:|------:|----------:|
|    ArrayRenting | 60.08 ns | 0.129 ns | 0.115 ns | 0.0132 |     - |     - |      56 B |
| ArrayAllocation | 31.97 ns | 0.153 ns | 0.143 ns | 0.0267 |     - |     - |     112 B |
|     UnsafeAlloc | 26.66 ns | 0.104 ns | 0.097 ns | 0.0133 |     - |     - |      56 B |
