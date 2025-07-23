using Lumina.Api.ASP.Infrastructure;
using Lumina.Models;
using Lumina.Repository;
using Lumina.Services;
using Lumina.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Lumina.Api.ASP.DTO;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

//Add Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Lumina API",
        Version = "v1",
        Description = "API for the Lumina mental wellness app",
    });

    // Enable JWT Bearer token support
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Enter: `Bearer {token}`",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
     {
         {
             new Microsoft.OpenApi.Models.OpenApiSecurityScheme
             {
                 Reference = new Microsoft.OpenApi.Models.OpenApiReference
                 {
                     Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                     Id = "Bearer"
                 }
             },
             new string[] {}
         }
     });
});


//Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.WithOrigins("http://localhost:5173") // Vite default port
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Add ASP.NET Core Identity
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        // User settings
        options.User.RequireUniqueEmail = true;

        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<LuminaDbContext>()
    .AddDefaultTokenProviders(); // needed for reset password, email confirm etc.

//TODO: Check if this is needed in a webapi
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.HttpOnly = true;
//    options.LoginPath = "/Identity/SignIn";
//    options.AccessDeniedPath = "/Identity/SignIn";
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
//    options.SlidingExpiration = true;
//});


//TODO:  add the OnChallenge event to existing JWT Bearer configuration (to display custom message on unauthenticated access) --> example code is commented
//TODO: Refactor IssuerSigningKey = --> evade null reference warning
//Add jwt token bearer
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

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

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
        };
        //options.Events = new JwtBearerEvents
        //{
        //    OnChallenge = context =>
        //    {
        //        // Skip the default logic
        //        context.HandleResponse();

        //        context.Response.StatusCode = 401;
        //        context.Response.ContentType = "application/json";

        //        var errorResponse = new ErrorResponse()
        //        {
        //            Message = "Authentication failed",
        //            Details = "User identity could not be verified from the authentication token"
        //        };

        //        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(errorResponse));
        //    }
        //};
    });


//Add db context with options
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<LuminaDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});


//Services
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<IJournalEntryService, JournalEntryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IActivityService, ActivityService>();

//Automapper
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<Lumina.Services.Mapping.MappingProfile>();
    cfg.AddProfile<Lumina.Api.ASP.Mapping.MappingProfile>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Lumina API v1");
        options.RoutePrefix = string.Empty; // Swagger UI at root `/`
    });
    await SeedData.SeedAsync(app.Services);

}

app.UseHttpsRedirection();


app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
