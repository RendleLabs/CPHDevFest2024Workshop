using NBomber.CSharp;
using NBomber.Http.CSharp;

var httpClient = new HttpClient();

var scenario = Scenario.Create("http_scenario", async context =>
    {
        var request =
            Http.CreateRequest("GET", "http://localhost:8080/weatherforecast")
                .WithHeader("Accept", "application/json");

        var response = await Http.Send(httpClient, request);
            
        return response.IsError
            ? Response.Fail()
            : Response.Ok(statusCode: response.StatusCode, sizeBytes: response.SizeBytes);
    })
    .WithLoadSimulations(
        Simulation.Inject(rate: 100,
            interval: TimeSpan.FromSeconds(1),
            during: TimeSpan.FromMinutes(1))
    );

NBomberRunner
    .RegisterScenarios(scenario)
    .Run();