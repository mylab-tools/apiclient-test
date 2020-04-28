using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using TestServer;
using Xunit;
using Xunit.Abstractions;

namespace FuncTest
{
    public class ApiClientTestBehaviorAfter : ApiClientTest<Startup, IWeatherForecastService>
    {
        public ApiClientTestBehaviorAfter(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldInvokeServerCall()
        {
            //Arrange
            

            //Act
            var weather = await TestCall(service => service.Get());

            //Assert
            Assert.NotNull(weather);
        }
    }

    public class ApiClientTestBehaviorBefore : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _appFactory;
        private readonly ITestOutputHelper _output;

        public ApiClientTestBehaviorBefore(WebApplicationFactory<Startup> appFactory, ITestOutputHelper output)
        {
            _appFactory = appFactory;
            _output = output;
        }

        [Fact]
        public async Task ShouldInvokeServerCall()
        {
            //Arrange
            var clProvider = new DelegateHttpClientProvider(() => _appFactory.CreateClient());
            var client = new ApiClient<IWeatherForecastService>(clProvider);

            //Act
            var detailedResponse = await client.Call(service => service.Get()).GetDetailed();

            _output.WriteLine(detailedResponse.RequestDump);
            _output.WriteLine("");
            _output.WriteLine(detailedResponse.ResponseDump);

            //Assert
            Assert.NotNull(detailedResponse.ResponseContent);
        }
    }
}
