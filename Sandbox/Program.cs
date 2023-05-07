using SharpECS;

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

            TestComponent c = registry.Emplace<TestComponent>(e);
            c.TestData = "It works?";

            Console.WriteLine(registry.Get<TestComponent>(e).TestData);
        }
    }
}