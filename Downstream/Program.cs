using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Downstream
{
    public class Program
    {
        private static int _requests;

        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(async (context) =>
                {
                    // Simulate a service which is occasionally slow
                    if (Interlocked.Increment(ref _requests) % 2000 == 0)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3));
                    }

                    await context.Response.WriteAsync("Hello World!");
                }))
                .UseUrls("http://*:5000")
                .Build()
                .Run();
        }
    }
}
