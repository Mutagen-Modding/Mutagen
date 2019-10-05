``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-rc1-014190
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT


```
|                                      Method |     Mean |    Error |   StdDev |   Median |        Gen 0 |       Gen 1 |     Gen 2 |  Allocated |
|-------------------------------------------- |---------:|---------:|---------:|---------:|-------------:|------------:|----------:|-----------:|
|                                CreateBinary | 14.568 s | 0.3146 s | 0.5342 s | 14.520 s | 1021000.0000 | 261000.0000 | 1000.0000 |  424.52 MB |
|           CreateAndWriteBinaryWrapperToDisk |  2.997 s | 0.0044 s | 0.0039 s |  2.996 s |  397000.0000 |  57000.0000 |         - | 2758.77 MB |
|         CreateAndWriteBinaryWrapperToMemory |  2.368 s | 0.0070 s | 0.0058 s |  2.368 s |  476000.0000 |   1000.0000 |         - |  2334.1 MB |
|   CreateAndWriteBinaryWrapperParallelToDisk |  2.978 s | 0.1904 s | 0.5432 s |  2.858 s |  780000.0000 | 236000.0000 | 1000.0000 |  433.09 MB |
| CreateAndWriteBinaryWrapperParallelToMemory |  2.944 s | 0.1904 s | 0.5554 s |  2.756 s |  743000.0000 | 234000.0000 | 1000.0000 |  443.02 MB |
|      CreateAndWriteBinaryWrapperAsyncToDisk |  2.876 s | 0.0621 s | 0.1831 s |  2.886 s |  971000.0000 | 332000.0000 | 1000.0000 |  464.44 MB |
|    CreateAndWriteBinaryWrapperAsyncToMemory |  2.639 s | 0.0545 s | 0.1599 s |  2.634 s |  981000.0000 | 320000.0000 | 1000.0000 |  464.43 MB |
|                           WriteBinaryToDisk |  2.243 s | 0.0095 s | 0.0084 s |  2.243 s |  192000.0000 |  46000.0000 |         - | 1147.66 MB |
|                         WriteBinaryToMemory |  1.588 s | 0.0051 s | 0.0042 s |  1.587 s |  180000.0000 |           - |         - |  723.07 MB |
|                   WriteBinaryParallelToDisk |  2.031 s | 0.0997 s | 0.2939 s |  1.972 s |  544000.0000 | 188000.0000 | 1000.0000 |   81.55 MB |
|                 WriteBinaryParallelToMemory |  2.235 s | 0.1424 s | 0.4198 s |  2.405 s |  548000.0000 | 189000.0000 | 1000.0000 |   17.39 MB |
|                      WriteBinaryAsyncToDisk |  2.084 s | 0.0485 s | 0.1391 s |  2.053 s |  786000.0000 | 273000.0000 | 1000.0000 |    8.82 MB |
|                    WriteBinaryAsyncToMemory |  1.922 s | 0.0794 s | 0.2341 s |  1.907 s |  787000.0000 | 273000.0000 | 1000.0000 |    8.81 MB |
