``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.100
  [Host]     : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT
  DefaultJob : .NET Core 2.1.14 (CoreCLR 4.6.28207.04, CoreFX 4.6.28208.01), X64 RyuJIT


```
|                                      Method |    Mean |    Error |   StdDev |  Median |        Gen 0 |       Gen 1 |     Gen 2 |  Allocated |
|-------------------------------------------- |--------:|---------:|---------:|--------:|-------------:|------------:|----------:|-----------:|
|                                CreateBinary | 4.266 s | 0.0831 s | 0.1137 s | 4.252 s |  350000.0000 | 123000.0000 | 1000.0000 |  424.35 MB |
|           CreateAndWriteBinaryOverlayToDisk | 2.831 s | 0.0173 s | 0.0153 s | 2.824 s |  382000.0000 |  57000.0000 |         - | 2711.29 MB |
|         CreateAndWriteBinaryOverlayToMemory | 2.201 s | 0.0118 s | 0.0104 s | 2.198 s |  464000.0000 |           - |         - |  2286.7 MB |
|   CreateAndWriteBinaryOverlayParallelToDisk | 2.695 s | 0.0530 s | 0.1119 s | 2.729 s |  779000.0000 | 237000.0000 | 1000.0000 |  425.57 MB |
| CreateAndWriteBinaryOverlayParallelToMemory | 2.530 s | 0.0917 s | 0.2703 s | 2.450 s |  739000.0000 | 242000.0000 | 1000.0000 |  426.36 MB |
|      CreateAndWriteBinaryOverlayAsyncToDisk | 2.836 s | 0.0864 s | 0.2547 s | 2.781 s |  987000.0000 | 327000.0000 | 1000.0000 |  463.04 MB |
|    CreateAndWriteBinaryOverlayAsyncToMemory | 2.704 s | 0.0750 s | 0.2211 s | 2.720 s | 1018000.0000 | 331000.0000 | 1000.0000 |  463.03 MB |
|                           WriteBinaryToDisk | 1.896 s | 0.0061 s | 0.0054 s | 1.896 s |  185000.0000 |  45000.0000 |         - |  1100.7 MB |
|                         WriteBinaryToMemory | 1.268 s | 0.0022 s | 0.0021 s | 1.269 s |  168000.0000 |           - |         - |  676.11 MB |
|                   WriteBinaryParallelToDisk | 1.598 s | 0.0317 s | 0.0602 s | 1.609 s |  540000.0000 | 184000.0000 | 1000.0000 |    16.8 MB |
|                 WriteBinaryParallelToMemory | 1.823 s | 0.1037 s | 0.3059 s | 1.713 s |  543000.0000 | 185000.0000 | 1000.0000 |    1.85 MB |
|                      WriteBinaryAsyncToDisk | 1.982 s | 0.0740 s | 0.2160 s | 1.933 s |  751000.0000 | 263000.0000 | 1000.0000 |     6.2 MB |
|                    WriteBinaryAsyncToMemory | 1.762 s | 0.0363 s | 0.1031 s | 1.766 s |  763000.0000 | 269000.0000 | 1000.0000 |    6.19 MB |
