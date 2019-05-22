``` ini

BenchmarkDotNet=v0.11.5, OS=Windows 10.0.17134.765 (1803/April2018Update/Redstone4)
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
Frequency=3984652 Hz, Resolution=250.9629 ns, Timer=TSC
.NET Core SDK=2.1.700-preview-009618
  [Host]     : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT
  DefaultJob : .NET Core 2.1.11 (CoreCLR 4.6.27617.04, CoreFX 4.6.27617.02), 64bit RyuJIT


```
|          Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
|    ArrayRenting | 71.68 ns | 1.6647 ns | 2.4917 ns | 0.0132 |     - |     - |      56 B |
| ArrayAllocation | 37.37 ns | 0.9595 ns | 0.9853 ns | 0.0267 |     - |     - |     112 B |
|     UnsafeAlloc | 29.57 ns | 0.4986 ns | 0.4420 ns | 0.0133 |     - |     - |      56 B |
