using Microsoft.Extensions.Hosting;

namespace Michael.Net.Testing
{
    public interface IWebApplicationFactoryAble
    {
        public static IHostBuilder CreateHostBuilder(string[] args) => throw new NotImplementedException();
    }
}
