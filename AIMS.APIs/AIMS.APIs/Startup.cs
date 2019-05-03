﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AIMS.DAL.EF;
using AutoMapper;
using AIMS.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AIMS.APIs.AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AIMS.APIs.Scheduler;
using Microsoft.Extensions.Caching.Distributed;

namespace AIMS.APIs
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
            services.AddCors();
            string connectionString = Configuration.GetSection("ConnectionStrings:DefaultConnection").Value;
            services.AddDbContext<AIMSDbContext>(
                options =>
                {
                    options.UseSqlServer(connectionString,
                    //sqlOptions => sqlOptions.EnableRetryOnFailure());
                    sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    });
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "APIs for AIMS, FG of Somalia",
                    Description = "APIs for AIMS, Federal Govt of Somalia",
                    TermsOfService = "None",
                    Contact = new Contact() { Name = "UNDP Somalia", Email = "raashid.ahmad@gmail.com", Url = "www.google.com.pk" }
                });

                //Locate the XML file being generated by ASP.NET...
                /*var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.XML";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                //... and tell Swagger to use those XML comments.
                c.IncludeXmlComments(xmlPath);*/
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AIMSDbContext>()
                .AddDefaultTokenProviders();


            // ===== Add Jwt Authentication ========
            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    //ValidIssuer = Configuration["JwtIssuer"],
                    //ValidAudience = Configuration["JwtAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["JwtKey"]))
                };
            });



            services.AddAutoMapper(a => a.AddProfile(new MappingProfile()));
            services.AddScoped<ISectorTypesService, SectorTypesService>();
            services.AddScoped<ISectorMappingsService, SectorMappingsService>();
            services.AddScoped<ISectorService, SectorService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrganizationTypeService, OrganizationTypeService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IIATIService, IATIService>();
            services.AddScoped<ISMTPSettingsService, SMTPSettingsService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IFinancialYearService, FinancialYearService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportNamesService, ReportNamesService>();
            services.AddScoped<IReportSubscriptionService, ReportSubscriptionService>();
            services.AddScoped<ICurrencyService, CurrencyService>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();
            services.AddScoped<IEnvelopeService, EnvelopeService>();
            services.AddScoped<ICustomFieldsService, CustomFieldsService>();
            services.AddScoped<IGrantTypeService, GrantTypeService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
            services.AddSingleton<IConfiguration>(Configuration);

            services.AddHttpClient();
            services.AddHttpClient<IExchangeRateHttpService, ExchangeRateHttpService>();
            //Need to work on this scheduled task in future
            //services.AddSingleton<IHostedService, ScheduleTask>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Included full qualified for Ihosting environment because of ambiguity for same name with other namespace
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env,
            Microsoft.AspNetCore.Hosting.IApplicationLifetime lifetime, IDistributedCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors(builder =>
                builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
             );
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIMS APIs Version 1, FG of Somalia");
            });

            app.UseHttpsRedirection();
            app.UseAuthentication();

            /*Enabling cache and setting expiration time*/
            app.UseMvc();
        }
    }
}
