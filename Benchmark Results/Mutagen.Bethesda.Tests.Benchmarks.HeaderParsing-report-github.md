``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.101
  [Host]     : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT
  DefaultJob : .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT


```
|                     Method |     Mean |    Error |   StdDev |
|--------------------------- |---------:|---------:|---------:|
|      MajorRecordHeaderSpan | 23.75 ns | 0.145 ns | 0.128 ns |
| MajorRecordHeaderGetStream | 28.30 ns | 0.147 ns | 0.137 ns |
|           MajorRecordFrame | 33.72 ns | 0.208 ns | 0.185 ns |
|     MajorRecordMemoryFrame | 35.98 ns | 0.266 ns | 0.249 ns |
