using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Api.Attributes;
using Api.Extensions;
using AutoMapper;
using Dal;
using Dal.Configs;
using Dal.Interfaces;
using Dal.ServiceApi;
using EFCache;
using EfCoreRepository.Extensions;
using JavaScriptEngineSwitcher.ChakraCore;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using Lamar;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Models;
using Models.Constants;
using Models.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using React;
using React.AspNet;
using StackExchange.Redis;
using WebMarkupMin.AspNetCore2;
using static Dal.Utilities.ConnectionStringUtility;
using static Models.Constants.ApplicationConstants;

namespace Api
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        private readonly IConfigurationRoot _configuration;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="env"></param>
        public Startup(IWebHostEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureContainer(ServiceRegistry services)
        {
            services.AddMiniProfiler(opt =>
            {
                // opt.RouteBasePath = "/profiler";
                opt.ShouldProfile = _ => true;
                opt.ShowControls = true;
                opt.StackMaxLength = short.MaxValue;
                opt.PopupStartHidden = false;
                opt.PopupShowTrivial = true;
                opt.PopupShowTimeWithChildren = true;
            });

            services.AddHttpsRedirection(options => options.HttpsPort = 443);

            // Add framework services
            // Add functionality to inject IOptions<T>
            services.AddOptions();

            services.AddResponseCompression();

            services.AddLogging();

            services.AddRouting(options => options.LowercaseUrls = true);

            if (_env.IsDevelopment())
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                var redisConnectionString =
                    ConnectionStringUrlToRedisResource(_configuration.GetValue<string>("REDISTOGO_URL"));

                services.AddStackExchangeRedisCache(c => c.Configuration = redisConnectionString);
            }

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromMinutes(60);
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = ApiConstants.AuthenticationSessionCookieName;
                options.Cookie.SecurePolicy = CookieSecurePolicy.None;
            });
            
            services.AddReact();

            // Make sure a JS engine is registered, or you will get an error!
            services.AddJsEngineSwitcher(options => options.DefaultEngineName = ChakraCoreJsEngine.EngineName)
                .AddChakraCore();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = ApplicationName, Version = "v1"});

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                c.AddSecurityDefinition("Bearer", // Name the security scheme
                    new OpenApiSecurityScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme.",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer"
                    });
            });

            services.AddMvc(x =>
                {
                    x.ModelValidatorProviders.Clear();

                    // Not need to have https
                    x.RequireHttpsPermanent = false;

                    // Allow anonymous for localhost
                    if (_env.IsDevelopment())
                    {
                        x.Filters.Add<AllowAnonymousFilter>();
                    }

                    // Exception filter attribute
                    x.Filters.Add<ExceptionFilterAttribute>();
                }).AddNewtonsoftJson(x =>
                    {
                        x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        x.SerializerSettings.Converters.Add(new StringEnumConverter());
                    })
                .AddRazorPagesOptions(x => x.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute()));

            services.AddWebMarkupMin(opt =>
                {
                    opt.AllowMinificationInDevelopmentEnvironment = true;
                    opt.AllowCompressionInDevelopmentEnvironment = true;
                })
                .AddHtmlMinification()
                .AddHttpCompression();

            services.AddDbContext<EntityDbContext>(opt =>
            {
                if (_env.IsDevelopment())
                {
                    opt.UseSqlite(_configuration.GetValue<string>("ConnectionStrings:Sqlite"));
                }
                else
                {
                    var postgresConnectionString =
                        ConnectionStringUrlToPgResource(_configuration.GetValue<string>("DATABASE_URL")
                                                        ?? throw new Exception("DATABASE_URL is null"));
                    opt.UseNpgsql(postgresConnectionString);
                }
            });

            services.AddIdentity<User, IdentityRole<int>>(x => x.User.RequireUniqueEmail = true)
                .AddEntityFrameworkStores<EntityDbContext>()
                .AddRoles<IdentityRole<int>>()
                .AddDefaultTokenProviders();

            // L2 EF cache
            if (_env.IsDevelopment())
            {
                EntityFrameworkCache.Initialize(new InMemoryCache());
            }
            else
            {
                var redisConnectionString =
                    ConnectionStringUrlToRedisResource(_configuration.GetValue<string>("REDISTOGO_URL"));

                var redisConfigurationOptions = ConfigurationOptions.Parse(redisConnectionString);

                // Important
                redisConfigurationOptions.AbortOnConnectFail = false;

                EntityFrameworkCache.Initialize(new EFCache.Redis.RedisCache(redisConfigurationOptions));
            }

            services.AddEfRepository<EntityDbContext>(x =>
            {
                x.Profiles(Assembly.Load("Dal"), Assembly.Load("Models"));
            });

            services.AddAutoMapper(Assembly.Load("Models"));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(x =>
            {
                x.Cookie.MaxAge = TimeSpan.FromMinutes(60);
            });

            // If environment is localhost then use mock email service
            if (_env.IsDevelopment())
            {
                services.For<IS3Service>().Use(new S3Service()).Singleton();
            }
            else
            {
                var (accessKeyId, secretAccessKey, url) = (
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_ACCESS_KEY_ID"),
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_SECRET_ACCESS_KEY"),
                    _configuration.GetRequiredValue<string>("CLOUDCUBE_URL")
                );

                var prefix = new Uri(url).Segments.GetValue(1)?.ToString();
                const string bucketName = "cloud-cube";

                // Generally bad practice
                var credentials = new BasicAWSCredentials(accessKeyId, secretAccessKey);

                // Create S3 client
                services.For<IAmazonS3>().Use(new AmazonS3Client(credentials, RegionEndpoint.USEast1));
                services.For<S3ServiceConfig>().Use(new S3ServiceConfig(bucketName, prefix));

                services.For<IS3Service>().Use(ctx => new S3Service(
                    ctx.GetInstance<ILogger<S3Service>>(),
                    ctx.GetInstance<IAmazonS3>(),
                    ctx.GetInstance<S3ServiceConfig>()
                ));
            }

            // Register stuff in container, using the StructureMap APIs...
            services.Scan(_ =>
            {
                _.AssemblyContainingType(typeof(Startup));
                _.Assembly("Api");
                _.Assembly("Logic");
                _.Assembly("Dal");
                _.WithDefaultConventions();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();

            if (_env.IsDevelopment())
            {
                app.UseDatabaseErrorPage();

                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));
            }

            // Not necessary for this workshop but useful when running behind kubernetes
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                // Read and use headers coming from reverse proxy: X-Forwarded-For X-Forwarded-Proto
                // This is particularly important so that HttpContent.Request.Scheme will be correct behind a SSL terminating proxy
                ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                                   ForwardedHeaders.XForwardedProto
            });
            
            app.UseReact(config =>
            {
                config.SetBabelConfig(new BabelConfig
                    {
                        Plugins = new HashSet<string> { "transform-regenerator", "transform-runtime"},
                        Presets = new HashSet<string> {"es2015", "es2016", "es2017"}
                    })
                    .AddScript("~/scripts/script.jsx")
                    .SetLoadBabel(true);
            });

            // Use wwwroot folder as default static path
            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseCookiePolicy()
                .UseSession()
                .UseRouting()
                .UseCors("CorsPolicy")
                .UseAuthentication()
                .UseAuthorization()
                .UseEndpoints(endpoints => { endpoints.MapDefaultControllerRoute(); });

            Console.WriteLine("Application Started!");
        }
    }
}