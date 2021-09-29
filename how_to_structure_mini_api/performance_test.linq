<Query Kind="Program">
  <NuGetReference>NBomber</NuGetReference>
  <NuGetReference>NBomber.Http</NuGetReference>
  <Namespace>NBomber.Plugins.Http.CSharp</Namespace>
  <Namespace>NBomber.CSharp</Namespace>
  <Namespace>FsToolkit.ErrorHandling</Namespace>
  <Namespace>NBomber.Contracts</Namespace>
  <Namespace>System.Net.Http.Json</Namespace>
</Query>

void Main()
{
	var factory = HttpClientFactory.Create();
	var content = JsonContent.Create(new {title = "test title"});

	var mvc = Step.Create("mvc", factory, async context =>
	{
		var response = await context.Client.PostAsync("http://localhost:5001/test/1?v=test", content);

		return response.IsSuccessStatusCode
			? Response.Ok(statusCode: (int) response.StatusCode)
			: Response.Fail(statusCode: (int) response.StatusCode);
	});

	var minApi = Step.Create("min_api", factory, async context =>
	{
		var response = await context.Client.PostAsync("http://localhost:5000/test/1?v=test", content);

		return response.IsSuccessStatusCode
			? Response.Ok(statusCode: (int)response.StatusCode)
			: Response.Fail(statusCode: (int)response.StatusCode);
	});

	var mvc_scenario = ScenarioBuilder
		.CreateScenario("mvc_scenario", mvc)
		.WithWarmUpDuration(TimeSpan.FromSeconds(10))
		.WithLoadSimulations(Simulation.KeepConstant(24, TimeSpan.FromSeconds(60)));

	var minApiScenario = ScenarioBuilder
		.CreateScenario("min_api_scenario", minApi)
		.WithWarmUpDuration(TimeSpan.FromSeconds(10))
		.WithLoadSimulations(Simulation.KeepConstant(24, TimeSpan.FromSeconds(60)));
		
	NBomberRunner
	.RegisterScenarios(minApiScenario, mvc_scenario)
	.Run();
}

