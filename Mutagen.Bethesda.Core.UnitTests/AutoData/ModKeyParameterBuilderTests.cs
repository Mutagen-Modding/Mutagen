﻿using AutoFixture.Kernel;
using Shouldly;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Testing.AutoData;
using Noggog.Testing.Extensions;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.UnitTests.AutoData;

public class ModKeyParameterBuilderTests
{
    class NonInterestingClass
    {
        public void Method(ModKey someName)
        {
        }
    }
        
    [Theory, BasicAutoData]
    public void UninterestingNameReturnsParameterNameAsModKey(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        var param = typeof(NonInterestingClass).Methods().First().GetParameters().First();
        ModKey mk = (ModKey)sut.Create(param, context);
        param.Name.ShouldNotBeNull();
        mk.Name.ShouldStartWith(param.Name);
    }
        
    class ExistingName
    {
        public void Start(ModKey existingName)
        {
        }
            
        public void End(ModKey nameExisting)
        {
        }
            
        public void Sandwich(ModKey nameExistingName)
        {
        }
    }
        
    [Theory, BasicAutoData]
    public void ExistingNameReturnsParameterNameAsModKey(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        foreach (var method in typeof(ExistingName).Methods())
        {
            var param = method.GetParameters().First();
            ModKey mk = (ModKey)sut.Create(param, context);
            param.Name.ShouldNotBeNull();
            mk.Name.ShouldStartWith(param.Name);
        }
    }
        
    [Theory, BasicAutoData]
    public void ExistingNameCallsToMakeExist(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        foreach (var method in typeof(ExistingName).Methods())
        {
            var param = method.GetParameters().First();
            sut.MakeModExist.ClearReceivedCalls();
            ModKey mk = (ModKey)sut.Create(param, context);
            sut.MakeModExist
                .Received(1)
                .MakeExist(mk, context);
        }
    }
        
    class PluginName
    {
        public void Plugin(ModKey plugin)
        {
        }
    }
        
    [Theory, BasicAutoData]
    public void PluginNameShouldBePlugin(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        foreach (var method in typeof(PluginName).Methods())
        {
            var param = method.GetParameters().First();
            ModKey mk = (ModKey)sut.Create(param, context);
            mk.Type.ShouldBe(ModType.Plugin);
        }
    }
        
    class MasterName
    {
        public void Master(ModKey master)
        {
        }
    }
        
    [Theory, BasicAutoData]
    public void MasterNameShouldBeMaster(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        foreach (var method in typeof(MasterName).Methods())
        {
            var param = method.GetParameters().First();
            ModKey mk = (ModKey)sut.Create(param, context);
            mk.Type.ShouldBe(ModType.Master);
        }
    }
        
    class LightMasterName
    {
        public void Light(ModKey light)
        {
        }
            
        public void LightMaster(ModKey lightMaster)
        {
        }
    }
        
    [Theory, BasicAutoData]
    public void LightMasterNameShouldBeLightMaster(
        ISpecimenContext context,
        ModKeyParameterBuilder sut)
    {
        foreach (var method in typeof(LightMasterName).Methods())
        {
            var param = method.GetParameters().First();
            ModKey mk = (ModKey)sut.Create(param, context);
            mk.Type.ShouldBe(ModType.Light);
        }
    }
}