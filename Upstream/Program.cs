using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;

namespace Upstream
{
    public class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient() { BaseAddress = new Uri("http://localhost:5000") };

        public static void Main(string[] args)
        {
            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(async (context) =>
                {
                    await context.Response.WriteAsync(await _httpClient.GetStringAsync("/"));
                }))
                .UseUrls("http://*:5001")
                .Build()
                .Run();
        }
    }
}
