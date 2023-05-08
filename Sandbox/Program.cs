using SharpECS;

namespace Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EntityRegistry registry = new EntityRegistry();
            Entity e1 = registry.Create();
            Entity e2 = registry.Create();
            Entity e3 = registry.Create();
            Entity e4 = registry.Create();
            Entity e5 = registry.Create();

            registry.Add(e1, "Hello!");
            registry.Add(e1, 20);

            registry.Add(e2, 10.0f);

            registry.Add(e3, 256);

            registry.Add(e4, 4.3f);
            registry.Add(e4, "World!");

            registry.Add(e5, "Testing!");

            EntityQuery query = registry.GetEntities().WithoutEither<float>().Or<int>().With<string>();

            foreach (Entity entity in query.AsArray())
            {
                Console.WriteLine(entity.ToString());
            }
        }
    }
}