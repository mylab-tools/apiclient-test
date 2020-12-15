using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Test;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTest
{
    public class ApiClientTestBehaviorAfter : ApiClientTest<Startup, ITestApi>
    {
        public ApiClientTestBehaviorAfter(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldInvokeServerCall()
        {
            //Act
            var val = await TestCall(service => service.AddSalt("test"));

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeWithServiceConfiguration()
        {
            //Act
            var val = await TestCall(
                s => s.AddSalt("test"), 
                overrideServices: srv => srv.AddSingleton(new StringProcessorService("bar"))
                );

            //Assert
            Assert.Equal("test-bar", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeWithHttpModifying()
        {
            //Act
            var val = await TestCall(
                s => s.AddSaltToHeader(), 
                httpClientPostInit: client => client.DefaultRequestHeaders.Add("ArgumentHeader", "test")
                );

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }
    }
}
