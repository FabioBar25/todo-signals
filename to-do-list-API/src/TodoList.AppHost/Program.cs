var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres")
    .WithLifetime(ContainerLifetime.Persistent);

var tasksDatabase = postgres.AddDatabase("tasksdb");

var api = builder.AddProject<Projects.TodoList_Host>("host")
    .WithReference(tasksDatabase)
    .WaitFor(postgres)
    .WaitFor(tasksDatabase);

var frontend = builder.AddNpmApp("frontend", "..\\..\\..\\to-do-list-Front", "start:aspire")
    .WithHttpEndpoint(env: "PORT")
    .WithEnvironment("BROWSER", "none");

var proxy = builder.AddProject<Projects.TodoList_Proxy>("proxy")
    .WaitFor(api)
    .WaitFor(frontend)
    .WithExternalHttpEndpoints()
    .WithEnvironment(
        "ReverseProxy__Clusters__frontend-cluster__Destinations__frontend__Address",
        frontend.GetEndpoint("http"));

builder.Build().Run();
