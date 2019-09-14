``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-preview9-014004
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT


```
|                      Method |     Mean |    Error |   StdDev |        Gen 0 |       Gen 1 |     Gen 2 | Allocated |
|---------------------------- |---------:|---------:|---------:|-------------:|------------:|----------:|----------:|
|                CreateBinary | 15.380 s | 0.2985 s | 0.4468 s | 1050000.0000 | 271000.0000 | 1000.0000 | 424.52 MB |
|        CreateAndWriteBinary | 17.435 s | 0.3382 s | 0.3895 s | 1245000.0000 | 410000.0000 | 1000.0000 | 424.52 MB |
| CreateAndWriteBinaryWrapper |  2.806 s | 0.0564 s | 0.1628 s |  994000.0000 | 333000.0000 | 1000.0000 | 464.44 MB |
|                 WriteBinary |  2.204 s | 0.0668 s | 0.1969 s |  763000.0000 | 269000.0000 | 1000.0000 |   8.82 MB |
