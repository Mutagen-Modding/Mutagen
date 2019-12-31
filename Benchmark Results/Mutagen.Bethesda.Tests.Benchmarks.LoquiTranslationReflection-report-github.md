``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT


```
|  Method |     Mean |   Error |  StdDev |
|-------- |---------:|--------:|--------:|
|  Direct | 170.1 ns | 0.99 ns | 0.88 ns |
| Wrapped | 172.8 ns | 1.70 ns | 1.59 ns |
