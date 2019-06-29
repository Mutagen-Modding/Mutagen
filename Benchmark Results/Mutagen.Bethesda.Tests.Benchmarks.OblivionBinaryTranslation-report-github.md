``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.829 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984642 Hz, Resolution=250.9636 ns, Timer=TSC
.NET Core SDK=2.1.800-preview-009696
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|            Method |            Mean |          Error |         StdDev |        Gen 0 |       Gen 1 |     Gen 2 |    Allocated |
|------------------ |----------------:|---------------:|---------------:|-------------:|------------:|----------:|-------------:|
|      CreateBinary | 17,183,603.3 us | 329,680.827 us | 428,678.198 us | 1802000.0000 | 391000.0000 | 1000.0000 | 271272.19 KB |
|       WriteBinary |  3,637,136.8 us |  15,991.982 us |  14,958.910 us |  107000.0000 |  34000.0000 |         - | 638137.77 KB |
| PathGridImporting |        210.8 us |       1.885 us |       1.763 us |      74.2188 |     37.1094 |         - |    439.87 KB |
