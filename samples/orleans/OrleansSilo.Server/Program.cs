// See https://aka.ms/new-console-template for more 
using Orleans.Hosting;
using Orleans;


using Microsoft.Extensions.Hosting;
using OrleansSilo;
using Orleans.Configuration;

var invariant = "System.Data.SqlClient";
var connString = "Data Source = 106.14.43.4; Initial Catalog = orleans.silo; Integrated Security = False; User ID = sa; Password = Wzx123@abc; MultipleActiveResultSets=true;Application Name=orleans-silo.Product;connection timeout=600";
await Host.CreateDefaultBuilder()
    .UseOrleans(builder =>
{
    builder.Configure<ClusterOptions>(opt =>
    {
        opt.ServiceId = "orleans-silo";
        opt.ClusterId = "orleans-silo";
    }).UseAdoNetClustering(opt =>
    {
        opt.ConnectionString = connString;
        opt.Invariant = invariant;
    })
    .AddAdoNetGrainStorage("orleans-silo", opt =>
    {
        opt.ConnectionString = connString;
        opt.Invariant = invariant;
    })
    .ConfigureEndpoints(11111, 30000)
    .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(StockGrain).Assembly));
})
    .Build()
    .RunAsync();
