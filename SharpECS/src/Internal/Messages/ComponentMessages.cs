namespace SharpECS.Internal.Messages
{
    internal readonly record struct ComponentCopyMessage(Entity fromEntity, Entity toEntity);
}
