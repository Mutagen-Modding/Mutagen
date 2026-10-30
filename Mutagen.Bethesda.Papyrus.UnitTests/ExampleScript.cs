namespace Mutagen.Bethesda.Papyrus.UnitTests;

public static class ExampleScript
{
    public const string Source = """
ScriptName BSKDefaultMoveActorsToSelfOnLoaded extends ObjectReference
{Only use this script for actors that are supposed to be always persistent - like the cross province ship crews}

Actor[] Property TeleportActors Auto

Event OnLoad()
    int i = 0
    While (i < TeleportActors.Length)
        Actor akActor = TeleportActors[i]
        If (akActor)
            Teleport(akActor)
        EndIf
        i += 1
    EndWhile
EndEvent

Function Teleport(Actor akActor)
    If (!akActor.Is3DLoaded())
        akActor.MoveTo(Self)
    EndIf
EndFunction
""";
}
