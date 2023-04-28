using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.Interfaces;
using PlatformService.Repositories;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#region Repositories
builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
#endregion
#region Services
builder.Services.AddGrpc();
#endregion


if(builder.Environment.IsDevelopment()){
    Console.WriteLine("--> Using inMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMem"));
}
if(builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using SQLServer DB");
    Console.WriteLine(builder.Configuration.GetConnectionString("PlatformsConn"));
    
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}


#region Services

builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());   
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
#endregion

var app = builder.Build();

//define env variables
IWebHostEnvironment env = app.Environment;

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRouting();
app.MapGet("/protos/platforms.proto", async context => 
        {
            await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
        });
app.MapGrpcService<GrpcPlatformService>();
#region Load database

PrepDb.PrepPopulation(app, env.IsProduction());

#endregion

#region Show configuration
Console.WriteLine($"--> CommandService endpoint {builder.Configuration["CommandService"]}");
#endregion

app.Run();
