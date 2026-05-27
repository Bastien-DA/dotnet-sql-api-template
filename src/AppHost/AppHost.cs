var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb();

var appdb = postgres.AddDatabase("appdb");

builder.AddProject<Projects.Api>("api")
    .WithReference(appdb)
    .WaitFor(appdb);

builder.Build().Run();