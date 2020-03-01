using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.DbContexts;
using CourseLibrary.Repository;
using CourseLibrary.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace CourseLibrary {
    public class Startup {
        public Startup (IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services) {
            
            services.AddHttpCacheHeaders((expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 60;
                expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationModelOptions) =>
            {
                validationModelOptions.MustRevalidate = true;
            });
            services.AddResponseCaching ();
            services.AddControllers (setupAction => {
                    setupAction.ReturnHttpNotAcceptable = true;
                    setupAction.CacheProfiles.Add ("240SecondsCacheProfile",
                        new CacheProfile () {
                            Duration = 240
                        });

                }).AddNewtonsoftJson (setupAction => {
                    setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver ();
                })
                .AddXmlDataContractSerializerFormatters ()
                .ConfigureApiBehaviorOptions (setupAction => {
                    setupAction.InvalidModelStateResponseFactory = context => {
                    var problemDetails = new ValidationProblemDetails (context.ModelState) {
                    Type = "https://courselibrary.com/modelvalidationproblem",
                    Title = "One or more model validation errors occurred.",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = "See the errors property for details.",
                    Instance = context.HttpContext.Request.Path
                        };

                        problemDetails.Extensions.Add ("traceId", context.HttpContext.TraceIdentifier);

                        return new UnprocessableEntityObjectResult (problemDetails) {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.AddDbContext<CourseLibraryContext> (options => options.UseSqlServer (Configuration.GetConnectionString ("ConexaoSQL")));
            services.AddAutoMapper (AppDomain.CurrentDomain.GetAssemblies ());
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository> ();
            services.AddTransient<IPropertyMappingService, PropertyMappingService> ();
            services.AddTransient<IPropertyCheckerService, PropertyCheckerService> ();
            services.Configure<MvcOptions> (config => {
                //resolvendo o problema do output para o hateoas,configurando novamente o suporte ao hateoas, procura uma saida do  formatador do tipo
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                    .OfType<NewtonsoftJsonOutputFormatter> ()?.FirstOrDefault ();
                if (newtonsoftJsonOutputFormatter != null) {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add ("application/vnd.marvin.hateoas+json");
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment ()) {
                app.UseDeveloperExceptionPage ();
            } else {
                app.UseExceptionHandler (appBuilder => {
                    appBuilder.Run (async context => {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync ("An unexpected fault happened. Try again later.");
                    });
                });

            }
      
            app.UseHttpCacheHeaders();
            app.UseRouting ();

            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
        }
    }
}