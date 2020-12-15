using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Test;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTest
{
    public class TestApiBehavior 
    {
        private readonly TestApi<Startup, ITestApi> _api;

        public TestApiBehavior(ITestOutputHelper output) 
        {
            _api = new TestApi<Startup, ITestApi>
            {
                Output = output,
                //HttpClientTuner = client => {},
                //ServiceOverrider = services => {} 
            };
        }

        [Fact]
        public async Task ShouldInvokeServerCall()
        {
            //Arrange 
            var client = _api.Start();

            //Act
            var val = await client.Call(service => service.AddSalt("test"));

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeServerCallWithProxy()
        {
            //Arrange 
            var client = _api.StartWithProxy();

            //Act
            var val = await client.AddSalt("test");

            //Assert
            Assert.Equal("test-foo", val);
        }

        [Fact]
        public async Task ShouldInvokeWithServiceConfiguration()
        {
            //Arrange
            var client = _api.Start(
                srv => srv.AddSingleton(new StringProcessorService("bar"))
            );

            //Act
            var val = await client.Call(s => s.AddSalt("test"));

            //Assert
            Assert.Equal("test-bar", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeWithHttpModifying()
        {
            //Arrange
            var client = _api.Start(
                httpClientTuner: c => c.DefaultRequestHeaders.Add("ArgumentHeader", "test")
            );

            //Act
            var val = await client.Call(s => s.AddSaltToHeader());

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }
    }
}
