using Mutagen.Bethesda.Plugins;

namespace Mutagen.Bethesda.Skyrim;

public static class ActorValueMapper
{
    private static readonly IReadOnlyDictionary<FormKey, ActorValue> _mapping;

    static ActorValueMapper()
    {
        _mapping = new Dictionary<FormKey, ActorValue>()
        {
            { FormKey.Factory("0123456:Skyrim.esm"), ActorValue.Aggression },
            // ...
        };
    }

    public static bool TryGetActorValue(IActorValueInformationGetter actorValueInformation, out ActorValue actorValue)
    {
        return _mapping.TryGetValue(actorValueInformation.FormKey, out actorValue);
    }
}