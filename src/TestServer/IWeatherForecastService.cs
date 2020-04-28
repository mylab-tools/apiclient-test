using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ApiClient;

namespace TestServer
{
    [Api("WeatherForecast")]
    public interface IWeatherForecastService
    {
        [Get]
        Task<IEnumerable<WeatherForecast>> Get();
    }
}
