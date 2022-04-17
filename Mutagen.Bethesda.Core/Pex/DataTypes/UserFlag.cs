using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mutagen.Bethesda.Pex;

public record NullableUserFlag(string? Name, byte Index);
public record UserFlag(string Name, byte Index);