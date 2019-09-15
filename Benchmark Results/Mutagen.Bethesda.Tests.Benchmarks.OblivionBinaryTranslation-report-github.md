``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview9-014004
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT


```
|                      Method |    Mean |    Error |   StdDev |       Gen 0 |       Gen 1 |     Gen 2 | Allocated |
|---------------------------- |--------:|---------:|---------:|------------:|------------:|----------:|----------:|
| CreateAndWriteBinaryWrapper | 3.057 s | 0.1670 s | 0.4710 s | 786000.0000 | 231000.0000 | 1000.0000 | 432.08 MB |
|                 WriteBinary | 2.091 s | 0.1078 s | 0.3178 s | 540000.0000 | 185000.0000 | 1000.0000 |   3.77 MB |
