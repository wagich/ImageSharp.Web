using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;

namespace SixLabors.ImageSharp.Web.Benchmarks.Middleware
{
    [InProcess]
    [MemoryDiagnoser]
    [GcServer(true)]
    public class MiddlewareBenchmarks
    {
        private HttpClient client;

        [GlobalSetup]
        public void Setup()
        {
            var factory = new AppFactory();
            this.client = factory.CreateClient();
        }

        [Benchmark(Baseline = true, Description = "Sync Callbacks")]
        public async Task MiddlewareWithSyncCallbacksAsync()
        {
            HttpResponseMessage syncResponse = await this.client.GetAsync("/sync/imagesharp-logo.png?width=100");
            syncResponse.EnsureSuccessStatusCode();
        }

        [Benchmark(Description = "Async Task Callbacks")]
        public async Task MiddlewareWithAsyncCallbacksAsync()
        {
            HttpResponseMessage asyncResponse = await this.client.GetAsync("/task-async/imagesharp-logo.png?width=100");
            asyncResponse.EnsureSuccessStatusCode();
        }

        [Benchmark(Description = "Async ValueTask Callbacks")]
        public async Task MiddlewareWithValueTaskCallbacksAsync()
        {
            HttpResponseMessage asyncResponse = await this.client.GetAsync("/valuetask-async/imagesharp-logo.png?width=100");
            asyncResponse.EnsureSuccessStatusCode();
        }

        public class AppFactory : WebApplicationFactory<FakeStartup>
        {
            protected override IWebHostBuilder CreateWebHostBuilder()
            {
                var wwwrootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Middleware", "wwwroot"));
                Console.WriteLine($"wwwroot path '{wwwrootPath}'");
                IWebHostBuilder builder = WebHost
                    .CreateDefaultBuilder()
                    .UseStartup<FakeStartup>()
                    .UseContentRoot(wwwrootPath)
                    .UseWebRoot(wwwrootPath)
                    .ConfigureLogging(logging =>
                    {
                        logging.SetMinimumLevel(LogLevel.Critical); // disable log output
                    });

                return builder;
            }
        }

        public class FakeStartup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddImageSharp();
            }

            public void Configure(IApplicationBuilder appBase)
            {
                appBase.Map("/sync", app => app.UseMiddleware<SyncMiddleware>());
                appBase.Map("/task-async", app => app.UseMiddleware<AsyncMiddleware>());
                appBase.Map("/valuetask-async", app => app.UseMiddleware<ValueTaskMiddleware>());
            }
        }
    }
}
