using Orleans;
using Orleans.Hosting;
using OrleansSilo;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(builder =>
{
    builder.AddMemoryGrainStorage("orleans-silo");
    builder.UseLocalhostClustering();
    builder.ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StockGrain).Assembly));
});
builder.Services.AddControllers();
var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();
