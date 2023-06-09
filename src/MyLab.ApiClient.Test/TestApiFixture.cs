using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Provides tools to create test API application
    /// </summary>
    public class TestApiFixture<TStartup, TApiContact> : IDisposable
        where TStartup : class
        where TApiContact : class
    {
        private readonly WebApplicationFactory<TStartup> _appFactory;

        /// <summary>
        /// Overrides services
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
        /// Initializes a new instance of <see cref="TestApiFixture{TStartup, TApiContract}"/>
        /// </summary>
        public TestApiFixture()
        {
            _appFactory = new WebApplicationFactory<TStartup>();
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public ClientTestApiSet<TApiContact> Start(
            Action<IServiceCollection> serviceOverrider = null, 
            Action<HttpClient> httpClientTuner = null)
        {
            return Start(out _, serviceOverrider, httpClientTuner);
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public ClientTestApiSet<TApiContact> Start(out HttpClient innerHttpClient,
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
            {
                ServiceOverrider?.Invoke(srv);
                serviceOverrider?.Invoke(srv);
            }));

            var opt = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            innerHttpClient = factory.CreateClient(opt);

            HttpClientTuner?.Invoke(innerHttpClient);
            httpClientTuner?.Invoke(innerHttpClient);

            var client = new ApiClient<TApiContact>(new SingleHttpClientProvider(innerHttpClient));

            return new ClientTestApiSet<TApiContact>
            {
                HttpClient = innerHttpClient,
                ApiClient = new TestApiClient<TApiContact>(client)
                {
                    Output = Output
                },
                ServiceProvider = factory.Services
            };
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public ProxyTestApiSet<TApiContact> StartWithProxy(
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            return StartWithProxy(out _, serviceOverrider, httpClientTuner);
        }

        /// <summary>
        /// Starts test API instance and returns <see cref="ApiClient{TApiContract}"/>
        /// </summary>
        public ProxyTestApiSet<TApiContact> StartWithProxy(out HttpClient innerHttpClient,
            Action<IServiceCollection> serviceOverrider = null,
            Action<HttpClient> httpClientTuner = null)
        {
            var factory = _appFactory.WithWebHostBuilder(builder => builder.ConfigureTestServices(srv =>
            {
                ServiceOverrider?.Invoke(srv);
                serviceOverrider?.Invoke(srv);
            }));

            var opt = new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            };

            innerHttpClient = factory.CreateClient(opt);

            HttpClientTuner?.Invoke(innerHttpClient);
            httpClientTuner?.Invoke(innerHttpClient);

            var proxy = ApiProxy<TApiContact>.Create(
                new SingleHttpClientProvider(innerHttpClient), 
                null,
                new TestOutputApiCallObserver(Output, typeof(TApiContact)));

            return new ProxyTestApiSet<TApiContact>
            {
                ApiClient = proxy,
                HttpClient = innerHttpClient,
                ServiceProvider = factory.Services
            };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _appFactory?.Dispose();
        }
    }
}
