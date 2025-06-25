using System;
using Microsoft.EntityFrameworkCore;
using testRestApi.data;
using testRestApi.data.models;

var builder = WebApplication.CreateBuilder(args);


//make dependency injection


builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditSaveChangesInterceptor>();

//builder.Services.AddSingleton<AuditSaveChangesInterceptor>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

    });


});

builder.Services.AddDbContext<appdbcontext>((serviceProvider, options) =>
{
    //options.UseSqlServer("DefaultConnection");
    // Add the interceptor
    var interceptor = serviceProvider.GetRequiredService<AuditSaveChangesInterceptor>();
    options = options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    options.AddInterceptors(interceptor);

});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
Console.WriteLine("hello console");

app.Run();
