using Microsoft.EntityFrameworkCore;
using NextGym.Application.Members;
using NextGym.Infrastructure;
using NextGym.Infrastructure.Members;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (MySQL)
var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<NextGymDbContext>(options =>
{
    options.UseMySql(cs, ServerVersion.AutoDetect(cs));
});

// DI for Member module
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMemberService, MemberService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
