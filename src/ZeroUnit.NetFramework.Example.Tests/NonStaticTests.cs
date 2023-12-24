using System.Threading.Tasks;

namespace ZeroUnit.NetFramework.Example.Tests
{
    public class Tests
    {
        public void Synchronous()
        {
        }

        public Task Asynchronous1()
        {
            return Task.CompletedTask;
        }
#pragma warning disable CS1998
        public async Task Asynchronous2()
        {
        }
#pragma warning restore CS1998

        public System.Threading.Tasks.Task Asynchronous3()
        {
            return Task.CompletedTask;
        }

        public static void Static()
        {
        }
    }
}
