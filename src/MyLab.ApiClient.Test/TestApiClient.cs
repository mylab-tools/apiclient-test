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
        public async Task<TestCallDetails> Call(Expression<Func<TApiContact, Task>> invoker)
        {
            CallDetails details;
            Exception respProcError = null;

            try
            {
                details = await _apiClient.Request(invoker).GetDetailedAsync();
            }
            catch (DetailedResponseProcessingException<CallDetails> e)
            {
                respProcError = e.InnerException;
                details = e.CallDetails;
            }
            
            var resDetails = new TestCallDetails(details)
            {
                ResponseProcessingError = respProcError != null,
                ProcessingError =  respProcError
            };

            Output?.WriteLine(resDetails.ToTestDump(typeof(TApiContact)));

            return resDetails;
        }

        /// <summary>
        /// Performs server method calling
        /// </summary>
        public async Task<TestCallDetails<TRes>> Call<TRes>(Expression<Func<TApiContact, Task<TRes>>> invoker)
        {
            CallDetails<TRes> details;
            Exception respProcError = null;

            try
            {
                details = await _apiClient.Request(invoker).GetDetailedAsync();
            }
            catch (DetailedResponseProcessingException<CallDetails<TRes>> e)
            {
                respProcError = e.InnerException;
                details = e.CallDetails;
            }

            var resDetails = new TestCallDetails<TRes>(details)
            {
                ResponseProcessingError = respProcError != null,
                ProcessingError = respProcError
            };

            Output?.WriteLine(resDetails.ToTestDump(typeof(TApiContact)));

            return resDetails;
        }
    }
}