namespace SharpECS.Internal.Messages
{
    internal readonly record struct EntityCreatedMessage(uint EntityID);
    internal readonly record struct EntityDisposedMessage(uint EntityID);
}
