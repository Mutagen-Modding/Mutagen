using FluentAssertions;
using Mutagen.Bethesda.Core.UnitTests;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.Plugins.Records
{
    public class Equality_Tests
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
                AttackSound = TestConstants.Form2.AsNullableLink<ISoundDescriptorGetter>()
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
    }
}