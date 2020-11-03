using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    /// <summary>
    /// Provides wrapped test API calls 
    /// </summary>
    /// <typeparam name="TApiContact">API contract</typeparam>
    public class TestApiClient<TApiContact>
        where TApiContact : class
    {
        private readonly ApiClient<TApiContact> _apiClient;

        /// <summary>
        /// Test output for log writing
        /// </summary>
        public ITestOutputHelper Output { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="TestApiClient{TApiContract}"/>
        /// </summary>
        public TestApiClient(ApiClient<TApiContact> apiClient)
        {
            _apiClient = apiClient;
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        public async Task<CallDetails<string>> Call(Expression<Func<TApiContact, Task>> invoker)
        {
            var details = await _apiClient.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        public async Task<CallDetails<TRes>> Call<TRes>(Expression<Func<TApiContact, Task<TRes>>> invoker)
        {
            var details = await _apiClient.Call(invoker).GetDetailed();

            Log(details);

            return details;
        }

        void Log<TRes>(CallDetails<TRes> call)
        {
            if (Output == null) return;

            Output.WriteLine("");
            Output.WriteLine($"===== REQUEST BEGIN ({typeof(TApiContact).Name}) =====");
            Output.WriteLine("");
            Output.WriteLine(call.RequestDump);
            Output.WriteLine("===== REQUEST END =====");
            Output.WriteLine("");
            Output.WriteLine($"===== RESPONSE BEGIN ({typeof(TApiContact).Name}) =====");
            Output.WriteLine("");
            Output.WriteLine(call.ResponseDump);
            Output.WriteLine("===== RESPONSE END =====");
        }
    }
}