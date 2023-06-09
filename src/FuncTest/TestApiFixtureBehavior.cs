using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MyLab.ApiClient.Test;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTest
{
    public class TestApiFixtureBehavior : IClassFixture<TestApiFixture<Startup, ITestApi>>
    {
        private readonly TestApiFixture<Startup, ITestApi> _fxt;

        public TestApiFixtureBehavior(TestApiFixture<Startup, ITestApi> fxt, ITestOutputHelper output)
        {
            fxt.Output = output;
            _fxt = fxt;
        }

        [Fact]
        public async Task ShouldInvokeServerCall()
        {
            //Arrange 
            var client = _fxt.Start().ApiClient;

            //Act
            var val = await client.Call(service => service.AddSalt("test"));

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeServerCallWithProxy()
        {
            //Arrange 
            var client = _fxt.StartWithProxy().ApiClient;

            //Act
            var val = await client.AddSalt("test");

            //Assert
            Assert.Equal("test-foo", val);
        }

        [Fact]
        public async Task ShouldInvokeWithServiceConfiguration()
        {
            //Arrange
            var client = _fxt.Start(srv => 
                    srv.AddSingleton(new StringProcessorService("bar"))
                ).ApiClient;

            //Act
            var val = await client.Call(s => s.AddSalt("test"));

            //Assert
            Assert.Equal("test-bar", val.ResponseContent);
        }

        [Fact]
        public async Task ShouldInvokeWithHttpModifying()
        {
            //Arrange
            var client = _fxt.Start(httpClientTuner: c => 
                    c.DefaultRequestHeaders.Add("ArgumentHeader", "test")
                ).ApiClient;

            //Act
            var val = await client.Call(s => s.AddSaltToHeader());

            //Assert
            Assert.Equal("test-foo", val.ResponseContent);
        }
    }
}
