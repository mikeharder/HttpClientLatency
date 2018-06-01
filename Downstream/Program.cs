using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Text;

namespace Downstream
{
    public class Program
    {
        private static byte[] _response = Encoding.UTF8.GetBytes(new string('a', 1024));

        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(async (context) =>
                {
                    await context.Response.Body.WriteAsync(_response, 0, _response.Length);
                }))
                .UseUrls("http://*:5000")
                .Build()
                .Run();
        }
    }
}
