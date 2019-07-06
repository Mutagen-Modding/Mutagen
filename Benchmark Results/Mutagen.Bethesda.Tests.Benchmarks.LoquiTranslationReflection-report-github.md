``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.829 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984650 Hz, Resolution=250.9631 ns, Timer=TSC
.NET Core SDK=2.1.800-preview-009696
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|  Method |     Mean |    Error |   StdDev |
|-------- |---------:|---------:|---------:|
|  Direct | 135.5 ns | 1.249 ns | 1.168 ns |
| Wrapped | 137.9 ns | 2.131 ns | 1.889 ns |
