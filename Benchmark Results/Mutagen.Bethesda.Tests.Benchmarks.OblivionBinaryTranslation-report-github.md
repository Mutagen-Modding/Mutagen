``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.800-preview-009696
  [Host]     : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT


```
|                      Method |     Mean |    Error |   StdDev |        Gen 0 |       Gen 1 |     Gen 2 |  Allocated |
|---------------------------- |---------:|---------:|---------:|-------------:|------------:|----------:|-----------:|
|                CreateBinary | 17.325 s | 0.3441 s | 0.4710 s | 1855000.0000 | 401000.0000 | 1000.0000 |  264.91 MB |
|        CreateAndWriteBinary | 19.681 s | 0.2970 s | 0.2633 s | 2027000.0000 | 474000.0000 | 1000.0000 |  264.91 MB |
| CreateAndWriteBinaryWrapper |  8.051 s | 0.0639 s | 0.0598 s | 1243000.0000 |  77000.0000 |         - | 7263.36 MB |
|                 WriteBinary |  3.485 s | 0.0176 s | 0.0165 s |  191000.0000 |  46000.0000 |         - |  1147.1 MB |
