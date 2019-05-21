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
| CreateBinary | 17.160 s | 0.3305 s | 0.4412 s | 1654000.0000 | 396000.0000 | 1000.0000 | 264.92 MB |
|  WriteBinary |  3.875 s | 0.0692 s | 0.0540 s |  107000.0000 |  34000.0000 |         - | 623.18 MB |
| PathGridImporting | 233.8 us | 3.797 us | 2.965 us | 78.3691 | 39.0625 |     - | 432.67 KB |