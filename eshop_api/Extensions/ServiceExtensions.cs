using eshop_api.Exceptions;
using eshop_api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace eshop_api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureSqlConnection(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EshopContext>(opts =>
                opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"))
            );
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(o =>
            {
                o.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                );
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole<int>>(q =>
            {
                q.User.RequireUniqueEmail = true;
                q.Password.RequiredLength = 8;
                q.Password.RequireDigit = true;
                q.Password.RequireNonAlphanumeric = true;
                q.Password.RequireUppercase = true;
                q.Password.RequireLowercase = true;
            }).AddEntityFrameworkStores<EshopContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
           {
               o.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = false,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = configuration.GetSection("JWT:ValidIssuer").Value,
                   ValidAudience = configuration.GetSection("JWT:ValidAudience").Value,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Secret").Value))
               };


           });
        }

        public static void ConfugureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(error =>
            {
                error.Run(async context =>
               {
                   context.Response.ContentType = "application/json";
                   var feature = context.Features.Get<IExceptionHandlerFeature>();
                   var statusCode = 500;
                   var title = "Internal Server Error";
                   var detail = "Something went wrong. Please try again later.";
                   if (feature != null)
                   {
                       var exception = feature.Error;
                       if (exception is EntityNotFoundException)
                       {
                           statusCode = 404;
                           title = "Resource Not Found";
                           detail = exception.Message;
                       }
                       else if (exception is EntityAlreadyExistExcption)
                       {
                           statusCode = 401;
                           title = "Resource Already Exist";
                           detail = exception.Message;
                       }
                       var apiError = new ApiError(statusCode, title, detail).ToString();
                       context.Response.StatusCode = statusCode;
                       await context.Response.WriteAsync(apiError);
                   }
               });
            });
        }
    }
}
