using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace Downstream
{
    public class Program
    {
        private const int _responseLength = 1024;
        private static readonly byte[] _response = Encoding.UTF8.GetBytes(new string('a', _responseLength));

        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(context => context.Response.Body.WriteAsync(_response, 0, _responseLength)))
                .UseUrls("http://*:5000")
                .Build()
                .Run();
        }
    }
}
