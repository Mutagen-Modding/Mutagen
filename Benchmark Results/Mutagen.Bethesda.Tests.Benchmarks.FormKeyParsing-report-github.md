``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT


```
|        Method |     Mean |   Error |  StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |---------:|--------:|--------:|-------:|------:|------:|----------:|
|  ParseFormKey | 299.4 ns | 5.61 ns | 5.77 ns | 0.0834 |     - |     - |     352 B |
|   ParseModKey | 147.9 ns | 0.37 ns | 0.32 ns | 0.0112 |     - |     - |      48 B |
|   ParseFormID | 106.6 ns | 0.41 ns | 0.38 ns |      - |     - |     - |         - |
| ParseFormID0x | 118.3 ns | 0.32 ns | 0.28 ns | 0.0112 |     - |     - |      48 B |
