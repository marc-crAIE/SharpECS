using SharpECS;

namespace Sandbox
{
    internal class TestComponent
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

            ref string str = ref registry.Add(e, "Hello");
            str = "Hello World!";
            Console.WriteLine(registry.Get<string>(e));

            Console.WriteLine(registry.Get<TestComponent>(e).TestData);
        }
    }
}