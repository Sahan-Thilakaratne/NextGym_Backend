using Application.Auth;
using Application.Billing;
using Application.Members;
using Application.Members;
using Infrastructure;
using Infrastructure;
using Infrastructure.Auth;
using Infrastructure.Billing;
using Infrastructure.Members;
using Infrastructure.Members;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

//billing di
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

builder.Services.AddScoped<IMemberAccountRepository, MemberAccountRepository>();
builder.Services.AddScoped<IMemberAccountService, MemberAccountService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();


// JWT options
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Authentication (JWT Bearer)
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddAuthorization();

//Make swagger bearer support



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
