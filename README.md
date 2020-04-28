# MyLab.ApiClient.Test
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.ApiClient.Test)](https://www.nuget.org/packages/MyLab.ApiClient.Test)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Обзор

`MyLab.ApiClient.Test` представляет набор инструментов для написания функциональных и интеграционных тестов на базе `xUnit`, связанных с вызовами `WEB-API` с использованием [MyAuth.ApiClient](https://github.com/ozzy-ext-mylab/apiclient).

## Базовый класс теста API-клиента

Класс `ApiClientTest<TStartup, TService>` предназначен для инкапсуляции однотипных операций для тестов, выполняющих обращения к `web`-сервисам.

Чтобы воспользоваться этим классом, необходимо:

* унаследовать класс теста от `ApiClientTest`;
* указать `TStartup` - класс `Startup` из проекта веб-приложения;
* указать `TService` - интерфейс - контракт сервиса из проекта веб-приложения;
* передать в конструктор базового класса объект вывода результатов тестирования `ITestOutputHelper` или `null`.

Пример исходного теста:

```C#
public class ApiClientTestBehaviorBefore : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _appFactory;
    private readonly ITestOutputHelper _output;

    public ApiClientTestBehaviorBefore(
        WebApplicationFactory<Startup> appFactory, 
        ITestOutputHelper output)
    {
        _appFactory = appFactory;
        _output = output;
    }

    [Fact]
    public async Task ShouldInvokeServerCall()
    {
        //Arrange
        var clProvider = new DelegateHttpClientProvider(
            () => _appFactory.CreateClient());
        var client = new ApiClient<IWeatherForecastService>(clProvider);

        //Act
        var detailedResponse = await client
            .Call(service => service.Get())
            .GetDetailed();

        _output.WriteLine(detailedResponse.RequestDump);
        _output.WriteLine("");
        _output.WriteLine(detailedResponse.ResponseDump);

        //Assert
        Assert.NotNull(detailedResponse.ResponseContent);
    }
}
```

Результат выполнения теста:

```
GET http://localhost/WeatherForecast/

Cookie: <empty>


200 OK

Content-Type: application/json; charset=utf-8
Content-Length: 503

Payload here
```

Тот же тест, переделанный с использованием базового класса `ApiClientTest<TStartup, TService>`:

```C#
public class ApiClientTestBehaviorAfter : ApiClientTest<Startup, IWeatherForecastService>
{
    public ApiClientTestBehaviorAfter(ITestOutputHelper output) : base(output)
    {
    }

    [Fact]
    public async Task ShouldInvokeServerCall()
    {
        //Act
        var weather = await TestCall(service => service.Get());

        //Assert
        Assert.NotNull(weather);
    }
}
```

Результат выполнения теста:

```
===== REQUEST BEGIN =====

GET http://localhost/WeatherForecast/

Cookie: <empty>

===== REQUEST END =====

===== RESPONSE BEGIN =====

200 OK

Content-Type: application/json; charset=utf-8
Content-Length: 503

Payload here

===== RESPONSE END =====

```



