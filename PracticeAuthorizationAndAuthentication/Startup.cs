using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PracticeAuthorizationAndAuthentication.DataMapper;
using PracticeAuthorizationAndAuthentication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticeAuthorizationAndAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var securityKey = "vagshadjad-abkbkajdl-adjdkaj-anldnaklkaladj-anldkn";

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            services.AddControllers();
            services.Configure<ConnectionStringSetting>(
                Configuration.GetSection(ConnectionStringSetting.SectionName)
                );
            services.AddScoped<ITokenDataMapper, TokenDataMapper>();
            services.AddSingleton<IAuthorizationHandler, EntitlementHandler>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = "Arunava",
                        ValidAudience = "all",
                        IssuerSigningKey = symmetricSecurityKey
                    }
                     );

            services.AddAuthorization(opt =>
                                     opt.AddPolicy("Staff",
                                     policy => policy.AddRequirements(new EntitlementRequirement())
                                         ));

            services.AddAuthorization(opt =>
               opt.AddPolicy("SendMailPolicy", policy =>
               policy.AddRequirements(new EntitlementRequirement())));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var x = env.EnvironmentName;

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}