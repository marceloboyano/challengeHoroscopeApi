using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Text;
using FluentValidation;
using HoroscopeApi.Config;
using HoroscopeApi.Config.Json;
using HoroscopeApi.DataAccess;
using HoroscopeApi.DTOs.Responses;
using HoroscopeApi.Repositories;
using HoroscopeApi.Repositories.Interfaces;
using HoroscopeApi.Services.Implementations;
using HoroscopeApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
});

// Return model-binding/validation errors in the standard ApiResponse shape (instead of the default ProblemDetails)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var message = string.Join(" ", context.ModelState.Values
            .SelectMany(state => state.Errors)
            .Select(error => error.ErrorMessage));

        if (string.IsNullOrWhiteSpace(message))
        {
            message = "Invalid request.";
        }

        return new BadRequestObjectResult(new ApiResponse<object>(message, StatusCodes.Status400BadRequest));
    };
});

// FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// Database (EF Core - SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHoroscopeQueryRepository, HoroscopeQueryRepository>();

// Business services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHoroscopeService, HoroscopeService>();
builder.Services.AddScoped<IStatsService, StatsService>();

// Typed HttpClient for the external horoscope API (configured from appsettings)
builder.Services.Configure<NewAstroSettings>(builder.Configuration.GetSection("NewAstroSettings"));
var newAstroSettings = builder.Configuration.GetSection("NewAstroSettings").Get<NewAstroSettings>()!;

builder.Services.AddHttpClient<INewAstroClient, NewAstroClient>(client =>
{
    client.BaseAddress = new Uri(newAstroSettings.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(newAstroSettings.TimeoutSeconds);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("HoroscopeApi/1.0");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
});

// Cross-cutting
builder.Services.AddMemoryCache();

// JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(policy => policy
        .AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod());
});

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Horoscope API",
        Version = "v1",
        Description = "Horoscope REST API (challenge). Registration, JWT login, daily horoscope, profile, statistics and history."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT authentication. Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Horoscope API V1");
        c.DocumentTitle = "Horoscope API";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();
