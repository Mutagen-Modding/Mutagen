using FluentAssertions;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Testing;
using Noggog;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records;

public class EqualityTests
{
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
            .Should().BeTrue();
    }
        
    [Fact]
    public void SelfEquality()
    {
        var someWeapon = GetSomeWeapon();
        someWeapon.Equals(someWeapon)
            .Should().BeTrue();
    }
        
    [Fact]
    public void DuplicatedEquality()
    {
        var someWeapon = GetSomeWeapon();
        var other = someWeapon.DeepCopy();

        someWeapon.Equals(other)
            .Should().BeTrue();
    }
        
    [Fact]
    public void DuplicatedInequality()
    {
        var someWeapon = GetSomeWeapon();
        var other = someWeapon.DeepCopy();
        other.BasicStats!.Damage *= 10;

        someWeapon.Equals(other)
            .Should().BeFalse();
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
        }).Should().BeTrue();
    }
}