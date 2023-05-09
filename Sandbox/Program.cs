using SharpECS;

namespace Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EntityRegistry registry = new EntityRegistry();
            Entity original = registry.Create();
            Entity original2 = registry.Create();

            registry.Add(original, "Hello World!");
            registry.Add(original, 256);

            EntityRegistry copyTo = new EntityRegistry();
            Entity copy = registry.CopyTo(original, copyTo);

            registry.Destroy(original);

            Console.WriteLine($"Original: {registry.Get<string>(original)}, Copy: {copyTo.Get<string>(copy)}");
            Console.WriteLine($"Original: {registry.Get<int>(original)}, Copy: {copyTo.Get<int>(copy)}");
        }
    }
}