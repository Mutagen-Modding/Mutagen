using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Testing;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EqualityTests
{
    [Fact]
    public void FreshEquals()
    {
        var npc1 = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        var npc2 = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE);
        Assert.Equal(npc1, npc2);
    }

    [Fact]
    public void SimpleEquals()
    {
        var npc1 = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE)
        {
            Name = "TEST"
        };
        var npc2 = new Npc(TestConstants.Form1, SkyrimRelease.SkyrimSE)
        {
            Name = "TEST"
        };
        Assert.Equal(npc1, npc2);
    }
    
    private Weapon GetSomeWeapon()
    {
        return new Weapon(FormKey.Null, SkyrimRelease.SkyrimSE)
        {
            Description = "Test",
            BasicStats = new WeaponBasicStats()
            {
                Damage = 5
            },
            Unused = new byte[] {1, 2, 3},
            AttackSound = TestConstants.Form2.ToNullableLink<ISoundDescriptorGetter>()
        };
    }
        
    [Fact]
    public void WeaponBasicStats()
    {
        Weapon.TranslationMask _verificationMask = new(defaultOn: true)
        {
            BasicStats = new WeaponBasicStats.TranslationMask(defaultOn: true)
            {
                Damage = false,
                Value = false,
                Weight = false,
            }
        };

        var someWeapon = GetSomeWeapon();

        var other = someWeapon.DeepCopy();
        other.BasicStats!.Damage = 10;

        someWeapon.Equals(other, _verificationMask)
            .ShouldBeTrue();
    }
        
    [Fact]
    public void SelfEquality()
    {
        var someWeapon = GetSomeWeapon();
        someWeapon.Equals(someWeapon)
            .ShouldBeTrue();
    }
        
    [Fact]
    public void DuplicatedEquality()
    {
        var someWeapon = GetSomeWeapon();
        var other = someWeapon.DeepCopy();

        someWeapon.Equals(other)
            .ShouldBeTrue();
    }
        
    [Fact]
    public void DuplicatedInequality()
    {
        var someWeapon = GetSomeWeapon();
        var other = someWeapon.DeepCopy();
        other.BasicStats!.Damage *= 10;

        someWeapon.Equals(other)
            .ShouldBeFalse();
    }

    [Fact]
    public void DeepLoquiMasking()
    {
        VirtualMachineAdapter vc = new VirtualMachineAdapter()
        {
            Scripts = new Noggog.ExtendedList<ScriptEntry>()
            {
                new ScriptEntry()
                {
                    Name = "Testing123",
                    Properties = new ExtendedList<ScriptProperty>()
                    {
                        new ScriptProperty()
                        {
                            Name = "Hello World"
                        }
                    }
                }
            }
        };
        VirtualMachineAdapter vc2 = new VirtualMachineAdapter()
        {
            Scripts = new Noggog.ExtendedList<ScriptEntry>()
            {
                new ScriptEntry()
                {
                    Name = "Testing123"
                }
            }
        };

        vc.Equals(vc2, new VirtualMachineAdapter.TranslationMask(true)
        {
            Scripts = new ScriptEntry.TranslationMask(true)
            {
                Properties = false,
            }
        }).ShouldBeTrue();
    }

    [Theory, MutagenModAutoData]
    public void AssetLinkListEquality(
        SkyrimMod mod,
        Weather weather1,
        Weather weather2,
        Weather weather3,
        AssetLink<SkyrimTextureAssetType> assetLink1,
        AssetLink<SkyrimTextureAssetType> assetLink2,
        AssetLink<SkyrimTextureAssetType> assetLink3)
    {
        var mask = new Weather.TranslationMask(false, false)
        {
            CloudTextures = true
        };

        weather1.CloudTextures[0] = assetLink1;
        weather1.CloudTextures[1] = assetLink2;
        
        weather2.CloudTextures[0] = assetLink2;
        weather2.CloudTextures[1] = assetLink3;
        
        weather3.CloudTextures[0] = assetLink1;
        weather3.CloudTextures[1] = assetLink2;

        weather1.Equals(weather1, mask)
            .ShouldBeTrue();
        weather1.Equals(weather3, mask)
            .ShouldBeTrue();
        weather1.Equals(weather2, mask)
            .ShouldBeFalse();
    }
}