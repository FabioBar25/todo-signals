using Microsoft.EntityFrameworkCore;
using TodoSignals.Access;
using TodoSignals.Access.Persistence;
using TodoSignals.Manager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddManager();
builder.Services.AddAccess(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("TodoSignalsClient", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "Data"));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TasksDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("TodoSignalsClient");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
