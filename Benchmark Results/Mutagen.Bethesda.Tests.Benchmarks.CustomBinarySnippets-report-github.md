``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.885 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984651 Hz, Resolution=250.9630 ns, Timer=TSC
.NET Core SDK=2.1.800-preview-009696
  [Host]     : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.12 (CoreCLR 4.6.27817.01, CoreFX 4.6.27818.01), 64bit RyuJIT


```
|            Method |     Mean |     Error |    StdDev |
|------------------ |---------:|----------:|----------:|
| PathGridImporting | 220.3 us | 0.7815 us | 0.6526 us |
