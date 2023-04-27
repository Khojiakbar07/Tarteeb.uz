//=================================
// Copyright (c) Coalition of Good-Hearted Engineers
// Free to use to bring order in your workplace
//=================================

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Tarteeb.Api.Brokers.DateTimes;
using Tarteeb.Api.Brokers.Emails;
using Tarteeb.Api.Brokers.Loggings;
using Tarteeb.Api.Brokers.Storages;
using Tarteeb.Api.Brokers.Tokens;
using Tarteeb.Api.Services.Foundations.Emails;
using Tarteeb.Api.Services.Foundations.Milestones;
using Tarteeb.Api.Services.Foundations.Scores;
using Tarteeb.Api.Services.Foundations.Securities;
using Tarteeb.Api.Services.Foundations.Teams;
using Tarteeb.Api.Services.Foundations.Tickets;
using Tarteeb.Api.Services.Foundations.Times;
using Tarteeb.Api.Services.Foundations.Users;
using Tarteeb.Api.Services.Orchestrations;
using Tarteeb.Api.Services.Processings.Users;

namespace Tarteeb.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration) =>
            Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddOData(options => options.Select().Filter().OrderBy());
            services.AddDbContext<StorageBroker>();
            services.AddCors(option =>
            {
                option.AddPolicy("MyPolicy", config =>
                {
                    config.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });
            RegisterBrokers(services);
            AddFoundationServices(services);
            AddProcessingServices(services);
            AddOrchestrationServices(services);
            RegisterJwtConfigurations(services, Configuration);

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc(
                    name: "v1",
                    info: new OpenApiInfo { Title = "Tarteeb.Api", Version = "v1" });

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();

            app.UseSwaggerUI(config => config.SwaggerEndpoint(
                url: "/swagger/v1/swagger.json",
                name: "Tarteeb.Api v1"));

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());
        }

        private static void RegisterBrokers(IServiceCollection services)
        {
            services.AddTransient<IStorageBroker, StorageBroker>();
            services.AddTransient<ILoggingBroker, LoggingBroker>();
            services.AddTransient<IDateTimeBroker, DateTimeBroker>();
            services.AddTransient<ITokenBroker, TokenBroker>();
            services.AddTransient<IEmailBroker, EmailBroker>();
        }

        private static void AddFoundationServices(IServiceCollection services)
        {
            services.AddTransient<ITicketService, TicketService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<ITeamService, TeamService>();
            services.AddTransient<ITimeService, TimeService>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<IScoreService, ScoreService>();
            services.AddTransient<ITimeService, TimeService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IMilestoneService, MilestoneService>();
            services.AddTransient<IEmailService, EmailService>();
        }

        private static void AddProcessingServices(IServiceCollection services) =>
            services.AddTransient<IUserProcessingService, UserProcessingService>();

        private static void AddOrchestrationServices(IServiceCollection services) =>
            services.AddTransient<IUserSecurityOrchestrationService, UserSecurityOrchestrationService>();

        private static void RegisterJwtConfigurations(IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    string key = configuration.GetSection("Jwt").GetValue<string>("Key");
                    byte[] convertKeyToBytes = Encoding.UTF8.GetBytes(key);

                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(convertKeyToBytes),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                });
        }
    }
}