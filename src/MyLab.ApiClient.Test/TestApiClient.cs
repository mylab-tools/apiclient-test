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
        public async Task<TestCallDetails<string>> Call(Expression<Func<TApiContact, Task>> invoker)
        {
            CallDetails<string> details;
            Exception respProcError = null;

            try
            {
                details = await _apiClient.Call(invoker).GetDetailed();
            }
            catch (DetailedResponseProcessingException<string> e)
            {
                respProcError = e.InnerException;
                details = e.CallDetails;
            }
            
            var resDetails = new TestCallDetails<string>(details)
            {
                ResponseProcessingError = respProcError != null,
                ProcessingError =  respProcError
            };

            Log(resDetails);

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
                details = await _apiClient.Call(invoker).GetDetailed();
            }
            catch (DetailedResponseProcessingException<TRes> e)
            {
                respProcError = e.InnerException;
                details = e.CallDetails;
            }

            var resDetails = new TestCallDetails<TRes>(details)
            {
                ResponseProcessingError = respProcError != null,
                ProcessingError = respProcError
            };

            Log(resDetails);

            return resDetails;
        }

        void Log<TRes>(TestCallDetails<TRes> call)
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

            if (call.ResponseProcessingError)
            {
                Output.WriteLine($"===== RESPONSE PROC ERROR ({typeof(TApiContact).Name}) =====");
                Output.WriteLine("");
                Output.WriteLine(call.ProcessingError.ToString());
                Output.WriteLine("===== RESPONSE PROC ERROR END =====");
            }
        }
    }
}