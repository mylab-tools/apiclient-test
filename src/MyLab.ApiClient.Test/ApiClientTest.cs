using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Contains base functional for test with API client
    /// </summary>
    public class ApiClientTest<TStartup, TService> : IDisposable
        where TStartup : class
    {
        private readonly WebApplicationFactory<TStartup> _appFactory;

        /// <summary>
        /// Gets test output
        /// </summary>
        protected ITestOutputHelper Output { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientTest{TStartup,TService}"/>
        /// </summary>
        public ApiClientTest(ITestOutputHelper output)
        {
            Output = output;
            _appFactory = new WebApplicationFactory<TStartup>();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _appFactory?.Dispose();
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        protected async Task<CallDetails<string>> TestCall(Expression<Func<TService, Task>> invoker, 
            Action<IServiceCollection> overrideServices = null,
            Action<HttpClient> httpClientPostInit = null)
        {
            var client = CreateClient(overrideServices, httpClientPostInit);
            
            var details = await client.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        protected async Task<CallDetails<TRes>> TestCall<TRes>(Expression<Func<TService, Task<TRes>>> invoker,
            Action<IServiceCollection> overrideServices = null,
            Action<HttpClient> httpClientPostInit = null)
        {
            var client = CreateClient(overrideServices, httpClientPostInit);

            var details = await client.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        protected virtual void OverrideServices(IServiceCollection services)
        {

        }

        protected virtual void HttpClientPostInit(HttpClient httpClient)
        {

        }

        void Log<TRes>(CallDetails<TRes> call)
        {
            if(Output == null) return;
            
            Output.WriteLine("");
            Output.WriteLine("===== REQUEST BEGIN =====");
            Output.WriteLine("");
            Output.WriteLine(call.RequestDump);
            Output.WriteLine("===== REQUEST END =====");
            Output.WriteLine("");
            Output.WriteLine("===== RESPONSE BEGIN =====");
            Output.WriteLine("");
            Output.WriteLine(call.ResponseDump);
            Output.WriteLine("===== RESPONSE END =====");
        }

        private ApiClient<TService> CreateClient(Action<IServiceCollection> overrideServices, Action<HttpClient> httpClientPostInit)
        {
            var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
            {
                OverrideServices(srv);
                overrideServices?.Invoke(srv);
            }));

            var httpClient = factory.CreateClient();

            HttpClientPostInit(httpClient);
            httpClientPostInit?.Invoke(httpClient);

            var client = new ApiClient<TService>(new SingleHttpClientProvider(httpClient));
            return client;
        }
    }
}
