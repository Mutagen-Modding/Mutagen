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
| CreateBinary | 17.130 s | 0.3308 s | 0.2933 s | 1647000.0000 | 371000.0000 | 1000.0000 | 264.93 MB |
|  WriteBinary |  3.790 s | 0.0339 s | 0.0317 s |  107000.0000 |  34000.0000 |         - | 623.18 MB |
