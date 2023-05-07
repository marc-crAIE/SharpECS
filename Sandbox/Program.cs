using SharpECS;

namespace Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EntityRegistry registry = new EntityRegistry();
            Entity e = registry.Create();

            ref string str = ref registry.Add(e, "Hello");
            str = "Hello World!";
            Console.WriteLine(registry.Get<string>(e));

            registry.Remove(e);
        }
    }
}