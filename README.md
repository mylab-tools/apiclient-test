# MyLab.ApiClient.Test
[![NuGet Version and Downloads count](https://buildstats.info/nuget/MyLab.ApiClient.Test)](https://www.nuget.org/packages/MyLab.ApiClient.Test)

```
Поддерживаемые платформы: .NET Core 3.1+
```
Ознакомьтесь с последними изменениями в [журнале изменений](/changelog.md).

## Обзор

`MyLab.ApiClient.Test` представляет набор инструментов для написания функциональных и интеграционных тестов на базе `xUnit`, связанных с вызовами `WEB-API` с использованием [MyLab.ApiClient](https://github.com/ozzy-ext-mylab/apiclient).

## ApiClientTest - Базовый класс теста API-клиента

### Обзор 

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
===== REQUEST BEGIN (IWeatherForecastService) =====

GET http://localhost/WeatherForecast/

Cookie: <empty>

===== REQUEST END =====

===== RESPONSE BEGIN (IWeatherForecastService) =====

200 OK

Content-Type: application/json; charset=utf-8
Content-Length: 503

Payload here

===== RESPONSE END =====

```

### Вызов метода API

Вызов метода `API` осуществляется асинхронным методом `TestCall`. Этот метод имеет перегрузку.

* `TestCall` - для вызова методов, которые не возвращают результат выполнения;

* `TestCall<TRes>` - для вызова методов, которые возращают результат выполнения.

В качестве первого и обязательного параметра указывается выражение,которое должно вызывать метод контракта `API`. Результат выполнение - детализация вызова `CallDetails<TRes>` (`CallDetails<string>` - для метода, не возвращающего результат).

При использовании метода `TestCall` выполняются следующие действия:

* с помощью фабрики `WebApplicationFactory<TStartup>` создаётся клиент`HttpClient`;
* на основе него создаётся `ApiClient<TContract>` для отправки запросов в `API`;
* используется переданное выражение и выполнется отправка запроса в `APIёж
* логи дампов запроса и ответа записываются в тестовый лог, если он был передан в конструкторе;
* возвращается детализация вызова `CallDetails<TRes>`.

В примере ниже происходит вызов метода `API`:

```C#
[Fact]
public async Task ShouldInvokeServerCall()
{
    //Act
    var val = await TestCall(s => s.AddSalt("test"));

    //Assert
    Assert.Equal("test-foo", val.ResponseContent);
}
```

### Тестовая настройка сервера

Имеется возможность переопределить зависимости главного DI контейнера веб-приложения. Это можно сделать двумя способами:

* по месту, в методе `TestCall` указать параметр `overrideServices` типа `Action<IServiceCollection>`;
* для всех тестов класса - переопределив виртуальный метод класса `ApiClientTest` - `void OverrideServices(IServiceCollection services)`

Очерёдность вызова методов конфигурирования сервисов приложения:

* `Startup.ConfigureServices`- стандартный метод конфигурирования веб-приложения;
* `ApiClientTest.OverrideServices` - виртуальный метод базового класса теста;
* `TestCall(..., Action<IServiceCollection> overrideServices)` - параметр метода тестового вызова `API`. 

В примере выше, сервис подмешивает соль к переданному значению, путём добавления "-foo" к значению. В тесте ниже приведён пример переопределения реализации сервиса в веб-приложении, который осуществляет примесь:

```C#
[Fact]
public async Task ShouldInvokeWithServiceConfiguration()
{
    //Act
    var val = await TestCall(
    	s => s.AddSalt("test"), 
        overrideServices: srv => 
        {
            srv.AddSingleton(new StringProcessorService("bar"));
        }
        );

    //Assert
    Assert.Equal("test-bar", val.ResponseContent);
}
```

Вариант с переопределением метода базового класса:

```c#
[Fact]
public async Task ShouldInvokeWithServiceConfiguration()
{
    //Act
    var val = await TestCall(s => s.AddSalt("test"));

    //Assert
    Assert.Equal("test-bar", val.ResponseContent);
}

protected override void OverrideServices(IServiceCollection srv)
{
	srv.AddSingleton(new StringProcessorService("bar"));
}
```



В этом примере, осуществляется замена уже зарегистрированного синглтона `StringProcessorService` на новый, который будет подмешивать другую соль.

### Тюнинг HttpClient

Для возможности дополнительно настраивать объект `HttpClient` перед использованием, в класс `ApiClientTest`  добавлены следующие механизмы:

* настройка по месту, в методе `TestCall` нужно указать параметр `httpClientPostInit`  типа `Action<HttpClient>`;
* переопределение метода `HttpClientPostInit(HttpClient httpClient)`, для определения настроек для всех тестов класса.

Очерёдность вызова методов настройки `HttpClient`:

* `ApiClientTest.HttpClientPostInit` - виртуальный метод базового класса теста;
* `TestCall(..., Action<HttpClient> httpClientPostInit)` - параметр метода тестового вызова `API`. 

В примере ниже приведён пример, где сервис подмешивает соль к переданному значению в заголовке `ArgumentHeader`, путём добавления "-foo" к этому значению.

> Пример демонстрационный. В реальности, вместо такого хака, лучше добавить в метод контракта API входной строковой параметр и пометить его атрибутом [Header("ArgumentHeader")]. 

```C#
[Fact]
public async Task ShouldInvokeWithHttpModifying()
{
    //Act
    var val = await TestCall(
        s => s.AddSaltToHeader(), 
        httpClientPostInit: client => 
        {
            client.DefaultRequestHeaders.Add("ArgumentHeader", "test")
    	}
    );

    //Assert
    Assert.Equal("test-foo", val.ResponseContent);
}
```

Вариант с переопределением метода базового класса:

```C#
[Fact]
public async Task ShouldInvokeWithHttpModifying()
{
    //Act
    var val = await TestCall(s => s.AddSaltToHeader());

    //Assert
    Assert.Equal("test-foo", val.ResponseContent);
}

protected virtual void HttpClientPostInit(HttpClient client)
{
	client.DefaultRequestHeaders.Add("ArgumentHeader", "test")
}
```

## TestApi - объектная модель тестового API

### Обзор

`TestApi` позволяет использовать одно или несколько веб-приложений в тестах. 

При использовании `TestApi` есть следующие рекомендации:

*  создавать объекты `TestApi` в конструкторе класса теста;
* создавать по одному объекту `TestApi` на каждый `API`;
* при инициализации в конструкторе присваивать `ITestOutputHelper`;
* при необходимости, во время инициализации в конструкторе, указывать общие для всех тестов класса переопределения сервисов и настройки `HttpClient` через определение полей  `ServiceOverrider` и `HttpClientTuner`;
* в каждом тестовом методе запускать приложения `API` и получать по клиенту на действующие в этом тесте экземпляры `API` с помощью метода `Start` класса `TestApi`.

### Инициализация

```C#
public TestApiBehavior(ITestOutputHelper output) 
{
    _api = new TestApi<Startup, ITestApi>
    {
        Output = output,
        //ServiceOverrider = services => {},
        //HttpClientTuner = client => {}
    };
}
```

### Использование

```C#
[Fact]
public async Task ShouldInvokeServerCall()
{
    //Arrange 
    var client = _api.Start(
    	//serviceOverrider: services => {},
	    //httpClientTuner: client => {}
    );

    //Act
    var val = await client.Call(service => service.AddSalt("test"));

    //Assert
    Assert.Equal("test-foo", val.ResponseContent);
}
```

У метода `Start` есть перегрузка для получения внутреннего `HttpClient`:

```C#
var client = _api.Start(
		out var innerClient,
    	//serviceOverrider: services => {},
	    //httpClientTuner: client => {}
    );
```



