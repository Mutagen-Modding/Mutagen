using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Reactive;
using AutoFixture.Xunit2;
using DynamicData;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using Mutagen.Bethesda.Core.UnitTests.AutoData;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Order.DI;
using Noggog;
using Noggog.Testing.FileSystem;
using NSubstitute;
using Xunit;

namespace Mutagen.Bethesda.Core.UnitTests.Plugins.Order
{
    public class PluginLiveLoadOrderProviderTests
    {
        [Theory, MutagenAutoData]
        public void Integration(
            [Frozen]FilePath pluginsTxt,
            [Frozen]MockFileSystemWatcher watcher,
            [Frozen]MockFileSystem fs)
        {
            fs.File.WriteAllLines(pluginsTxt,
                new string[]
                {
                    TestConstants.PluginModKey.ToString(),
                    TestConstants.PluginModKey2.ToString(),
                    TestConstants.PluginModKey3.ToString(),
                });
            var live = PluginListings.GetLiveLoadOrder(
                GameRelease.SkyrimLE,
                pluginsTxt, 
                default,
                out var state, 
                fileSystem: fs);
            var list = live.AsObservableList();
            Assert.Equal(3, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey2, list.Items.ElementAt(1).ModKey);
            Assert.Equal(TestConstants.PluginModKey3, list.Items.ElementAt(2).ModKey);
            fs.File.WriteAllLines(pluginsTxt,
                new string[]
                {
                    TestConstants.PluginModKey.ToString(),
                    TestConstants.PluginModKey3.ToString(),
                });
            watcher.MarkChanged(pluginsTxt);
            Assert.Equal(2, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey3, list.Items.ElementAt(1).ModKey);
            fs.File.WriteAllLines(pluginsTxt,
                new string[]
                {
                    TestConstants.PluginModKey.ToString(),
                    TestConstants.PluginModKey3.ToString(),
                    TestConstants.PluginModKey4.ToString(),
                });
            watcher.MarkChanged(pluginsTxt);
            Assert.Equal(3, list.Count);
            Assert.Equal(TestConstants.PluginModKey, list.Items.ElementAt(0).ModKey);
            Assert.Equal(TestConstants.PluginModKey3, list.Items.ElementAt(1).ModKey);
            Assert.Equal(TestConstants.PluginModKey4, list.Items.ElementAt(2).ModKey);
        }

        [Theory, MutagenAutoData]
        public void QueriesOnceInitially(
            [Frozen]FilePath pluginPath,
            [Frozen]MockFileSystem fs)
        {
            var listings = Substitute.For<IPluginListingsProvider>();
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().BeEmpty();
            listings.Received(1).Get();
        }

        [Theory, MutagenAutoData]
        public void QueriesIfChanged(
            [Frozen]FilePath pluginPath,
            [Frozen]MockFileSystemWatcher modified,
            [Frozen]MockFileSystem fs)
        {
            var listings = Substitute.For<IPluginListingsProvider>();
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().BeEmpty();
            listings.Received(1).Get();
            
            modified.MarkChanged(pluginPath);
            listings.Received(2).Get();
        }

        [Theory, MutagenAutoData]
        public void ReturnsNewListings(
            [Frozen]FilePath pluginPath,
            [Frozen]MockFileSystemWatcher modified,
            [Frozen]MockFileSystem fs)
        {
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.MasterModKey, true),
                new ModListing(TestConstants.MasterModKey2, false),
            };
            var listingsProv = Substitute.For<IPluginListingsProvider>();
            listingsProv.Get().Returns(listings);
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listingsProv,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out _)
                .AsObservableList();
            list.Items.Should().Equal(listings);
            
            var listings2 = new ModListing[]
            {
                new ModListing(TestConstants.MasterModKey, true),
                new ModListing(TestConstants.MasterModKey2, false),
                new ModListing(TestConstants.MasterModKey3, true),
            };
            listingsProv.Get().Returns(listings2);
            modified.MarkChanged(pluginPath);
            list.Items.Should().Equal(listings2);
        }

        [Theory, MutagenAutoData]
        public void NoErrors(
            [Frozen]FilePath pluginPath,
            [Frozen]MockFileSystemWatcher modified,
            [Frozen]MockFileSystem fs)
        {
            var listings = Substitute.For<IPluginListingsProvider>();
            new PluginLiveLoadOrderProvider(
                    fs,
                    listings,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out var state)
                .AsObservableList();
            TestScheduler scheduler = new TestScheduler();
            var testableObs = scheduler.CreateObserver<ErrorResponse>();
            using var sub = state.Subscribe(testableObs);
            testableObs.Messages.Should().HaveCount(1);
            testableObs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[0].Value.Value.Succeeded.Should().BeTrue();
            modified.MarkChanged(pluginPath);
            testableObs.Messages.Should().HaveCount(2);
            testableObs.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[1].Value.Value.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void ErrorFlop(
            [Frozen]FilePath pluginPath,
            [Frozen]MockFileSystemWatcher modified,
            [Frozen]MockFileSystem fs)
        {
            TestScheduler scheduler = new();
            var listings = new ModListing[]
            {
                new ModListing(TestConstants.MasterModKey, true),
                new ModListing(TestConstants.MasterModKey2, false),
            };
            var listingsProv = Substitute.For<IPluginListingsProvider>();
            listingsProv.Get().Returns(
                _ => listings,
                _ => throw new NotImplementedException(),
                _ => listings);
            var list = new PluginLiveLoadOrderProvider(
                    fs,
                    listingsProv,
                    new PluginListingsPathInjection(pluginPath))
                .Get(out var state)
                .AsObservableList();
            list.Count.Should().Be(2);
            var testableObs = scheduler.CreateObserver<ErrorResponse>();
            using var sub = state.Subscribe(testableObs);
            testableObs.Messages.Should().HaveCount(1);
            testableObs.Messages[0].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[0].Value.Value.Succeeded.Should().BeTrue();

            modified.MarkChanged(pluginPath);
            testableObs.Messages.Should().HaveCount(2);
            testableObs.Messages[1].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[1].Value.Value.Succeeded.Should().BeFalse();

            modified.MarkChanged(pluginPath);
            list.Count.Should().Be(2);
            testableObs.Messages.Should().HaveCount(3);
            testableObs.Messages[2].Value.Kind.Should().Be(NotificationKind.OnNext);
            testableObs.Messages[2].Value.Value.Succeeded.Should().BeTrue();
        }

        [Theory, MutagenAutoData]
        public void Changed(
            [Frozen]FilePath existingPluginPath,
            [Frozen]MockFileSystemWatcher modified,
            [Frozen]MockFileSystem fs)
        {
            TestScheduler scheduler = new();
            var testableObs = scheduler.CreateObserver<Unit>();
            new PluginLiveLoadOrderProvider(
                    fs,
                    Substitute.For<IPluginListingsProvider>(),
                    new PluginListingsPathInjection(existingPluginPath))
                .Changed
                .Subscribe(testableObs);
            testableObs.ShouldNotBeCompleted();
            testableObs.ShouldHaveNoErrors();
            testableObs.Messages.Should().HaveCount(1);
            
            modified.MarkChanged(existingPluginPath);
            testableObs.ShouldNotBeCompleted();
            testableObs.ShouldHaveNoErrors();
            testableObs.Messages.Should().HaveCount(2);
        }
    }
}