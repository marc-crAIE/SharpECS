using SharpECS;
using SharpECS.Internal;

namespace Sandbox
{
    internal class TestComponent : IComponent
    {
        public string TestData = "Hello World!";
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            EntityRegistry registry = new EntityRegistry();
            Entity e = registry.Create();
            Entity e2 = registry.Create();

            TestComponent c = registry.Emplace<TestComponent>(e);
            c.TestData = "It works?";

            Console.WriteLine(registry.Get<TestComponent>(e).TestData);

            ComponentManager<int>.GetOrCreate(registry.ID).Set(e, 1);
            ComponentManager<int>.Get(registry.ID).Set(e2, 2);

            Console.WriteLine(ComponentManager<int>.Get(registry.ID).Get(e));
            Console.WriteLine(ComponentManager<int>.Get(registry.ID).Get(e2));

            ref int i = ref ComponentManager<int>.Get(registry.ID).Get(e);
            i = 100;

            Console.WriteLine(ComponentManager<int>.Get(registry.ID).Get(e));
            Console.WriteLine(ComponentManager<int>.Get(registry.ID).Get(e2));
        }
    }
}