using System;
using Xunit.Abstractions;

namespace MyLab.ApiClient.Test
{
    class TestOutputApiCallObserver : IObserver<CallDetails>
    {
        private readonly ITestOutputHelper _output;
        private readonly Type _contract;

        /// <summary>
        /// Initializes a new instance of <see cref="TestOutputApiCallObserver"/>
        /// </summary>
        public TestOutputApiCallObserver(ITestOutputHelper output, Type contract)
        {
            _output = output;
            _contract = contract;
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            
        }

        public void OnNext(CallDetails value)
        {
            _output?.WriteLine(value.ToTestDump(_contract));
        }
    }
}