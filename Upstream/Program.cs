using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Upstream
{
    public class Program
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly HttpClientPool _httpClientPool = new HttpClientPool(Environment.ProcessorCount * 2);

        public static void Main(string[] args)
        {
            var downstream = Environment.GetEnvironmentVariable("Downstream");
            downstream = string.IsNullOrEmpty(downstream) ? "http://localhost:5000" : downstream;

            Console.WriteLine($"Downstream: {downstream}" + Environment.NewLine);

            _httpClient.BaseAddress = new Uri(downstream);
            _httpClientPool.BaseAddress = _httpClient.BaseAddress;

            new WebHostBuilder()
                .UseKestrel()
                .Configure(app => app.Run(async (context) =>
                {
                    int.TryParse(context.Request.Path.Value.Substring(1, 2), out var count);
                    count = (count == 0) ? 1 : count;

                    var pool = context.Request.Path.Value.Contains("pool");

                    var tasks = new Task<string>[count];
                    for (var i = 0; i < count; i++)
                    {
                        var httpClient = pool ? _httpClientPool.GetInstance() : _httpClient;
                        try
                        {
                            tasks[i] = httpClient.GetStringAsync("/");
                        }
                        finally
                        {
                            if (pool) _httpClientPool.ReturnInstance(httpClient);
                        }
                    }
                    await Task.WhenAll(tasks);

                    await context.Response.WriteAsync(await tasks[0]);
                }))
                .UseUrls("http://*:5001")
                .Build()
                .Run();
        }
    }

    public class HttpClientPool
    {
        private readonly HttpClient[] _clients;
        private readonly int[] _users;

        public HttpClientPool(int size)
        {
            _clients = new HttpClient[size];
            for (var i = 0; i < size; i++)
            {
                _clients[i] = new HttpClient();
            }

            _users = new int[size];
        }

        public Uri BaseAddress
        {
            get
            {
                return _clients[0].BaseAddress;
            }
            set
            {
                foreach (var client in _clients)
                {
                    client.BaseAddress = value;
                }
            }
        }

        public HttpClient GetInstance()
        {
            var leastUsed = LeastUsed();
            Interlocked.Increment(ref _users[leastUsed]);
            return _clients[leastUsed];
        }

        public void ReturnInstance(HttpClient httpClient)
        {
            for (var i = 0; i < _clients.Length; i++)
            {
                if (_clients[i] == httpClient)
                {
                    Interlocked.Decrement(ref _users[i]);
                }
            }
        }

        private int LeastUsed()
        {
            var leastUsedIndex = 0;
            var leastUsedCount = _users[0];

            for (var i = 1; i < _clients.Length; i++)
            {
                var count = _users[i];
                if (count < leastUsedCount)
                {
                    leastUsedIndex = i;
                    leastUsedCount = count;
                }
            }

            return leastUsedIndex;
        }

        public override string ToString()
        {
            return String.Join(" ", _users);
        }
    }
}
