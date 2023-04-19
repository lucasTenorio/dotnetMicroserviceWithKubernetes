using CommandService.AsyncDataServices;
using CommandService.Data;
using CommandService.Data.Repositories;
using CommandService.EventProcessing;
using CommandService.Intefaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICommandRepository, CommandRepository>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();

if(builder.Environment.IsDevelopment()){
    Console.WriteLine("--> Using inMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMem"));
    
}

if(builder.Environment.IsProduction())
{
    Console.WriteLine("--> Using Database: " + builder.Configuration.GetConnectionString("CommandsConn"));
    builder.Services.AddDbContextFactory<AppDbContext>( opt => 
        opt.UseSqlServer(
            builder.Configuration.GetConnectionString("CommandsConn")));
}


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

if(app.Environment.IsProduction())
{
    ApplyMigrations.apply(app, app.Environment.IsProduction());
}

app.UseAuthorization();

app.MapControllers();

app.Run();
