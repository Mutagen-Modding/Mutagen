``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.18362
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.0.100-rc1-014190
  [Host]     : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.13 (CoreCLR 4.6.28008.01, CoreFX 4.6.28008.01), 64bit RyuJIT


```
|                                      Method |    Mean |    Error |   StdDev |  Median |       Gen 0 |       Gen 1 |     Gen 2 |  Allocated |
|-------------------------------------------- |--------:|---------:|---------:|--------:|------------:|------------:|----------:|-----------:|
|                                CreateBinary | 6.636 s | 0.1301 s | 0.2137 s | 6.675 s | 447000.0000 | 136000.0000 | 1000.0000 |  424.38 MB |
|           CreateAndWriteBinaryWrapperToDisk | 2.994 s | 0.0256 s | 0.0227 s | 2.989 s | 392000.0000 |  67000.0000 |         - | 2753.94 MB |
|         CreateAndWriteBinaryWrapperToMemory | 2.366 s | 0.0091 s | 0.0081 s | 2.365 s | 476000.0000 |   3000.0000 |         - | 2329.33 MB |
|   CreateAndWriteBinaryWrapperParallelToDisk | 2.918 s | 0.1430 s | 0.4126 s | 2.768 s | 760000.0000 | 237000.0000 | 1000.0000 |   429.7 MB |
| CreateAndWriteBinaryWrapperParallelToMemory | 2.681 s | 0.0857 s | 0.2331 s | 2.646 s | 891000.0000 | 244000.0000 | 1000.0000 |  428.84 MB |
|      CreateAndWriteBinaryWrapperAsyncToDisk | 2.892 s | 0.0695 s | 0.2039 s | 2.863 s | 963000.0000 | 330000.0000 | 1000.0000 |  462.47 MB |
|    CreateAndWriteBinaryWrapperAsyncToMemory | 2.787 s | 0.0665 s | 0.1961 s | 2.754 s | 971000.0000 | 320000.0000 | 1000.0000 |  462.47 MB |
|                           WriteBinaryToDisk | 1.916 s | 0.0078 s | 0.0065 s | 1.916 s | 186000.0000 |  45000.0000 |         - |  1100.7 MB |
|                         WriteBinaryToMemory | 1.320 s | 0.0044 s | 0.0041 s | 1.320 s | 168000.0000 |           - |         - |  676.11 MB |
|                   WriteBinaryParallelToDisk | 1.840 s | 0.0888 s | 0.2604 s | 1.818 s | 552000.0000 | 186000.0000 | 1000.0000 |    2.78 MB |
|                 WriteBinaryParallelToMemory | 1.439 s | 0.0538 s | 0.1518 s | 1.422 s | 550000.0000 | 184000.0000 | 1000.0000 |    1.24 MB |
|                      WriteBinaryAsyncToDisk | 1.923 s | 0.0544 s | 0.1603 s | 1.940 s | 738000.0000 | 262000.0000 | 1000.0000 |    7.84 MB |
|                    WriteBinaryAsyncToMemory | 1.726 s | 0.0466 s | 0.1359 s | 1.700 s | 737000.0000 | 264000.0000 | 1000.0000 |    7.83 MB |
