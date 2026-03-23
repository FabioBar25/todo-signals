using TodoList.Access;
using TodoList.Host.Infrastructure;
using TodoList.Manager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddAccessLayer(builder.Configuration);
builder.Services.AddManagerLayer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:5000")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("frontend");
app.UseAuthorization();

app.MapDefaultEndpoints();
app.MapControllers();

await app.InitializeDatabaseAsync();

app.Run();

public partial class Program;
