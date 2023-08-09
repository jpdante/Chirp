using System.Reflection;
using System.Text;
using Chirp.Repository;
using Chirp.Server.Services;
using Chirp.Server.Services.Mail;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Chirp.Server;

public static class Program
{
  public static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
      c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chirp", Version = "v1" });

      c.AddSecurityDefinition("Bearer",
        new OpenApiSecurityScheme
        {
          Description = "JWT Authorization header using the Bearer scheme.\n\n" +
                        "Example: 'Bearer 12345abcdef'",
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
            },
            Scheme = "oauth2",
            Name = "Bearer",
            In = ParameterLocation.Header,
          },
          new List<string>()
        }
      });

      var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
      var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
      c.IncludeXmlComments(xmlPath);
    });

    builder.Services.AddDbContext<ChirpContext>(options => options
      .UseNpgsql(builder.Configuration.GetConnectionString("Default"), o => o.MigrationsAssembly("Chirp.Server"))
      .UseSnakeCaseNamingConvention()
    );
    
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
      options.TokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issue"] ?? "http://localhost:5000",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "http://localhost:5000",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
      };
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
  }

  public static void SetupMail(WebApplicationBuilder builder) {
    var mailConfigs = builder.Configuration.GetRequiredSection("Mail");
    switch (mailConfigs.GetValue<string>("Provider")?.ToLower()) {
      case "smtp":
        var smtpMailConfig = mailConfigs.GetRequiredSection("Config").Get<SmtpMailService.SmtpMailConfig>() ?? throw new Exception("Mail provider 'SMTP' is not configured.");
        builder.Services.AddSingleton<IMailService, SmtpMailService>(x => new SmtpMailService(smtpMailConfig)); 
        break;
      default:
        throw new Exception("Unknown mail provider or not configured. Please configure an email provider.");
    }
  }
}