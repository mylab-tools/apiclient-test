using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Represent test api application instance
    /// </summary>
    public class TestApi<TStartup, TApiContact> : IDisposable
        where TStartup : class
        where TApiContact : class
    {
        private readonly WebApplicationFactory<TStartup> _appFactory;

        /// <summary>
        /// Overrides services for
        /// </summary>
        public Action<IServiceCollection> ServiceOverrider { get; set; }

        /// <summary>
        /// Additional tunes <see cref="HttpClient"/>
        /// </summary>
        public Action<HttpClient> HttpClientTuner { get; set; }
        
        /// <summary>
        /// Test output for log writing
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestApi{TStartup, TApiContract}"/>
        /// </summary>
        public TestApi()
        {
            _appFactory = new WebApplicationFactory<TStartup>();
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public TestApiClient<TApiContact> Start(
            Action<IServiceCollection> serviceOverrider = null, 
            Action<HttpClient> httpClientTuner = null)
        {
            return Start(out _, serviceOverrider, httpClientTuner);
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public TestApiClient<TApiContact> Start(out HttpClient innerHttpClient,
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
            {
                ServiceOverrider?.Invoke(srv);
                serviceOverrider?.Invoke(srv);
            }));

            innerHttpClient = factory.CreateClient();

            HttpClientTuner?.Invoke(innerHttpClient);
            httpClientTuner?.Invoke(innerHttpClient);

            var client = new ApiClient<TApiContact>(new SingleHttpClientProvider(innerHttpClient));

            return new TestApiClient<TApiContact>(client)
            {
                Output = Output
            };
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public TApiContact StartWithProxy(
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            return StartWithProxy(out _, serviceOverrider, httpClientTuner);
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public TApiContact StartWithProxy(out HttpClient innerHttpClient,
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
            {
                ServiceOverrider?.Invoke(srv);
                serviceOverrider?.Invoke(srv);
            }));

            innerHttpClient = factory.CreateClient();

            HttpClientTuner?.Invoke(innerHttpClient);
            httpClientTuner?.Invoke(innerHttpClient);

            return ApiProxy<TApiContact>.Create(new SingleHttpClientProvider(innerHttpClient), new TestOutputApiCallObserver(Output, typeof(TApiContact)));
        }

        public void Dispose()
        {
            _appFactory?.Dispose();
        }
    }
}
