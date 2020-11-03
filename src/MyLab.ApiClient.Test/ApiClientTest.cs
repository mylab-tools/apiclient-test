using System;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Contains base functional for test with API client
    /// </summary>
    public class ApiClientTest<TStartup, TApiContact> : IDisposable
        where TStartup : class
        where TApiContact : class
    {
        private readonly TestApi<TStartup, TApiContact> _testApi;


        /// <summary>
        /// Gets test output
        /// </summary>
        protected ITestOutputHelper Output { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiClientTest{TStartup,IApiContact}"/>
        /// </summary>
        public ApiClientTest(ITestOutputHelper output)
        {
            Output = output;
            _testApi = new TestApi<TStartup, TApiContact>()
            {
                Output = output,
                HttpClientTuner = HttpClientPostInit,
                ServiceOverrider = OverrideServices
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _testApi.Dispose();
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        protected Task<CallDetails<string>> TestCall(Expression<Func<TApiContact, Task>> invoker, 
            Action<IServiceCollection> overrideServices = null,
            Action<HttpClient> httpClientPostInit = null)
        {
            var client = CreateClient(overrideServices, httpClientPostInit);

            return client.Call(invoker);
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        protected Task<CallDetails<TRes>> TestCall<TRes>(Expression<Func<TApiContact, Task<TRes>>> invoker,
            Action<IServiceCollection> overrideServices = null,
            Action<HttpClient> httpClientPostInit = null)
        {
            var client = CreateClient(overrideServices, httpClientPostInit);

            return client.Call(invoker);
        }

        protected virtual void OverrideServices(IServiceCollection services)
        {

        }

        protected virtual void HttpClientPostInit(HttpClient httpClient)
        {

        }

        private TestApiClient<TApiContact> CreateClient(Action<IServiceCollection> overrideServices, Action<HttpClient> httpClientPostInit)
        {
            return _testApi.Start(overrideServices, httpClientPostInit);
        }
    }
}
