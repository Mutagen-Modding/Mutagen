using Xunit;

// Sometimes the static loqui classes get unregistered somehow when running
// This might be due to parallelization?  Turning off for now.  Could perhaps
// also decide to put all Loqui related tests into a single xUnit test collection
// so they ran serially
[assembly: CollectionBehavior(DisableTestParallelization = true)]