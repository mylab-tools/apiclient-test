using System;
using System.Net.Http;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Contains tools to operate with test API
    /// </summary>
    public class TestApiSet
    {
        /// <summary>
        /// Application service provider
        /// </summary>
        public IServiceProvider ServiceProvider { get; set; }
        /// <summary>
        /// An <see cref="HttpClient"/> which will be used for interaction with API
        /// </summary>
        public HttpClient HttpClient { get; set; }
    }

    /// <summary>
    /// Contains tst API tools and <see cref="TestApiClient{TApiContact}"/> intended for interaction with API
    /// </summary>
    /// <typeparam name="TContract">API contract</typeparam>
    public class ClientTestApiSet<TContract> : TestApiSet
        where TContract : class
    {
        /// <summary>
        /// A client intended for interaction with API
        /// </summary>
        public TestApiClient<TContract> ApiClient { get; set; }
    }

    /// <summary>
    /// Contains tst API tools and proxy intended for interaction with API
    /// </summary>
    /// <typeparam name="TContract">API contract</typeparam>
    public class ProxyTestApiSet<TContract> : TestApiSet
        where TContract : class
    {
        /// <summary>
        /// A client intended for interaction with API
        /// </summary>
        public TContract ApiClient { get; set; }
    }
}