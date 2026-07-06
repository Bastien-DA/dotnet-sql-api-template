var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithDataVolume()
    .WithPgWeb();

var appdb = postgres.AddDatabase("appdb");

var migrations = builder.AddProject<Projects.MigrationService>("migrations")
    .WithReference(appdb)
    .WaitFor(appdb);

builder.AddProject<Projects.Api>("api")
    .WithReference(appdb)
    .WaitForCompletion(migrations);

builder.Build().Run();