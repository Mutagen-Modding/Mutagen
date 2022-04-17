using System;
using System.Collections.Generic;
using System.Text;

namespace Mutagen.Bethesda.Plugins.Records;

[Flags]
public enum ModHeaderCommonFlag
{
    Master = 0x0000_0001,
    Localized = 0x0000_0080,
    LightMaster = 0x0000_0200,
}