﻿using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Extentions;
using Mango.Services.EmailAPI.Messaging;
using Mango.Services.EmailAPI.Service;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var ConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseMySQL(ConnectionString!));
builder.Services.AddSingleton<IAzureServiceBusConsumer, AzureServiceBusConsumer>();

var optionBuilder = new DbContextOptionsBuilder<DataContext>();
optionBuilder.UseMySQL(ConnectionString!);
builder.Services.AddSingleton(new EmailService(optionBuilder.Options));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

app.UseAzureServiceBusConsumer();

app.Run();

void ApplyMigration()
{

    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<DataContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
