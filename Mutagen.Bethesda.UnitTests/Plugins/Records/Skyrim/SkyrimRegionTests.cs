﻿using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records.Skyrim;

public class SkyrimRegionTests : ASpecificCaseTest<Region, IRegionGetter>
{
    public override ModPath Path => TestDataPathing.SkyrimSoundRegionDataWithNoSecondSubrecord;
    public override GameRelease Release => GameRelease.SkyrimSE;
    public override bool TestPassthrough => false;
    
    public override void TestItem(IRegionGetter item)
    {
        item.Sounds.ShouldNotBeNull();
        item.Sounds!.Sounds.ShouldBeNull();
    }
}