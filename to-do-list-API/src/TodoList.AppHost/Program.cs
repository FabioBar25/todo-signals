var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var tasksDatabase = postgres.AddDatabase("tasksdb");

var api = builder.AddProject<Projects.TodoList_Host>("host")
    .WithReference(tasksDatabase)
    .WaitFor(tasksDatabase);

var proxy = builder.AddProject<Projects.TodoList_Proxy>("proxy")
    .WaitFor(api)
    .WithExternalHttpEndpoints();

builder.AddNpmApp("frontend", "..\\..\\..", "start:aspire")
    .WithHttpEndpoint(env: "PORT", port: 4300)npm start
    .WithEnvironment("BROWSER", "none");

builder.Build().Run();
