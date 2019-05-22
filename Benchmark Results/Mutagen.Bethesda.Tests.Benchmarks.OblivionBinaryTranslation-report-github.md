``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|       Method |     Mean |    Error |   StdDev |        Gen 0 |       Gen 1 |     Gen 2 | Allocated |
|------------- |---------:|---------:|---------:|-------------:|------------:|----------:|----------:|
| CreateBinary | 16.323 s | 0.3169 s | 0.3391 s | 1635000.0000 | 389000.0000 | 1000.0000 | 264.92 MB |
|  WriteBinary |  3.771 s | 0.0077 s | 0.0060 s |  106000.0000 |  34000.0000 |         - | 623.18 MB |
| PathGridImporting | 192.7 us | 3.830 us | 5.493 us | 76.1719 | 31.9824 |     - | 404.94 KB |