namespace SharpECS.Internal.Messages
{
    internal readonly record struct RegistryCreatedMessage(ushort RegistryID);
    internal readonly record struct RegistryDisposedMessage(ushort RegistryID);
}
