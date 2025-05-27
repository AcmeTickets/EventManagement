var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Api>("Api");
var message = builder.AddProject<Projects.Message>("Message");

builder.Build().Run();
