using api.Configurations;
using api.Mapping;
using api.Middlewares;
using App.Domain.Core.Repositories;
using ApplicationCore.Interfaces;
using AutoMapper;
using Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Otus.Teaching.PromoCodeFactory.DataAccess.Data;
using Serilog;
using Services.Abstractions;
using Services.Implementation;
using api;
using Services.Contracts;
using System.Collections.Concurrent;
using Redis.OM;
using Services.Implementation.HostedServices;
using ApplicationUsers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json.Serialization;
using MassTransit;
using api.Hubs;

internal class Program
{
    private static void Main(string[] args)
    {
        static MapperConfiguration GetMapperConfiguration()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<OrderMappingProfile>();

            });
            configuration.AssertConfigurationIsValid();
            return configuration;
        }


        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowRequests",
                policy =>
                {
                    policy.WithOrigins("https://localhost:5173", "http://localhost:5173", "http://reactclient:5173", "https://deliverytrue.ru", "https://deliverytrue.ru:5173") // React app URL
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
        });

        var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            //options.UseSqlite(connectionString, b => b.MigrationsAssembly("api")));
            options.UseLazyLoadingProxies(). // lazy loading
             UseNpgsql(connectionString, b => b.MigrationsAssembly("api")));

        builder.Services.AddIdentityApiEndpoints<ApplicationUser>().AddRoles<IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 0;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;

        })
        .AddEntityFrameworkStores<ApplicationDbContext>();


        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


        builder.Services.AddScoped<IUnitOfWork, EFUnitOfWork>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IInvoiceService, InvoiceService>();
        builder.Services.AddScoped<IDeliveryContractService, DeliveryContractService>();
        builder.Services.AddScoped<IContragentService, ContragentService>();
        builder.Services.AddScoped<ICargoTypeService, CargoTypeService>();
        builder.Services.AddScoped<ILogisticPriceService, LogisticPriceService>();
        builder.Services.AddScoped<IGeoService, GeoService>();
        builder.Services.AddScoped<IIdentityService, IdentityService>();
        builder.Services.AddSingleton<IMapper>(new Mapper(GetMapperConfiguration()));

       

        builder.Services.AddSingleton<IOffersService, OffersRedisOmService>();
        builder.Services.AddLogger(builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IDbInitializer, DbInitializer>();
        builder.Services.AddMvc().AddJsonOptions(o =>
        {
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        }); ;

        builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration["REDIS_CONNECTION_STRING"]));
        //builder.Services.AddHostedService<IndexCreationService>();

        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        // You probably want to use in-memory cache if not developing using docker-compose
        builder.Services.AddMemoryCache();
        //builder.Services.AddDistributedRedisCache(action => { action.Configuration = Configuration["Redis:InstanceName"]; });

        //builder.Services.AddStackExchangeRedisCache(options =>
        //{
        //    options.Configuration = builder.Configuration.GetConnectionString("MyRedisConStr");
        //    options.InstanceName = "Redis";
        //});

        builder.Services.AddSession(options =>
        {
            // Set a short timeout for easy testing.
            options.IdleTimeout = TimeSpan.FromMinutes(60);
            options.Cookie.Name = "CreativeTim.Argon.DotNetCore.SessionCookie";
            // You might want to only set the application cookies over a secure connection:
            // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.Cookie.HttpOnly = true;
            // Make the session cookie essential
            options.Cookie.IsEssential = true;
        });

        builder.Services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(builder.Configuration.GetConnectionString("RabbitMQ"), c =>
                {
                    c.Username("guest");
                    c.Password("guest");
                });

                cfg.ClearSerialization();
                cfg.UseRawJsonSerializer();
                cfg.ConfigureEndpoints(context);
            });
        });
        builder.Services.AddSignalR();
        var app = builder.Build();


        app.UseMiddleware<LoggerMiddleware>();
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        using (var scope = app.Services.CreateScope())
        {
            var scopedProvider = scope.ServiceProvider;
            try
            {

                var dbInitializer = scope.ServiceProvider.GetService<IDbInitializer>();

                //var userManager = scopedProvider.GetRequiredService<UserManager<ApplicationUser>>();
                //var roleManager = scopedProvider.GetRequiredService<RoleManager<IdentityRole>>();
                if (dbInitializer != null)
                {
                    dbInitializer.Initialize();
                }
                else throw new Exception("Ошибка инициализации БД!");

                //var identityContext = scopedProvider.GetRequiredService<AppIdentityDbContext>();
                // await AppIdentityDbContextSeed.SeedAsync(identityContext, userManager, roleManager);
            }
            catch (Exception ex)
            {
                //app.Logger.LogError(ex, "An error occurred seeding the DB.");
                Log.Logger.Fatal(ex, $"The {app.Environment.ApplicationName} error... An error occurred seeding the DB.");
            }



            try
            {
                //var scope = app.Services.GetService<IServiceScopeFactory>();
                //CreateRolesUser(scope?.CreateScope()?.ServiceProvider!);
                CreateRolesUser(scopedProvider);
                CreateSampleUsers(scopedProvider).Wait();
                var orderSevice = scopedProvider.GetRequiredService<IOrderService>();
                var contragentSevice = scopedProvider.GetRequiredService<IContragentService>();
                var offersService = scopedProvider.GetRequiredService<IOffersService>();
                var logisticPriceService = scopedProvider.GetRequiredService<ILogisticPriceService>();
                var redisProvider = scopedProvider.GetRequiredService<RedisConnectionProvider>();
                FillData fillData = new FillData(orderSevice, offersService, contragentSevice, logisticPriceService, redisProvider);
                fillData.FillOffers().Wait();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Ошибка добавления ролей");
            }

        }


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        var summaries = new[]
        {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.UseRouting();
        app.UseCors("AllowRequests");

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapIdentityApi<ApplicationUser>();

        //app.UseAuthorization();
        Log.Logger.Information($"The {app.Environment.ApplicationName} started...");
        app.MapControllerRoute(
           name: "default",
           pattern: "{controller=Home}/{action=Index}/{id?}");




        app.MapFallbackToFile("/index.html");
        app.MapHub<EventsExchange>("/exchangeHub");
        app.Run();
    }

    private static void CreateRolesUser(IServiceProvider serviceProvider)
    {
        //initializing custom roles 
        var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        string[] roleNames = { UserRoles.Administrator, UserRoles.Customer, UserRoles.Logist, UserRoles.BookEditor };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = RoleManager.RoleExistsAsync(roleName).Result;
            if (!roleExist)
            {
                //create the roles and seed them to the database: Question 1
                roleResult = RoleManager.CreateAsync(new IdentityRole(roleName)).Result;
            }
        }

        //Here you could create a super user who will maintain the web app
        var poweruser = new ApplicationUser
        {

            UserName = "admin@admin.com",
            Email = "admin@admin.com",
            EmailConfirmed = true,
        };
        //Ensure you have these values in your appsettings.json file
        string userPWD = "Topol$110";


        var _user = UserManager.GetUsersInRoleAsync("Administrator").Result;

        if (_user.Count == 0)
        {
            var sameuser = UserManager.FindByEmailAsync(poweruser.Email).Result;
            if (sameuser != null)
            {
                poweruser.UserName = sameuser.UserName + "_";
                poweruser.Email = "_" + sameuser.Email;
            }
            var createPowerUser = UserManager.CreateAsync(poweruser, userPWD).Result;
            if (createPowerUser.Succeeded)
            {
                //here we tie the new user to the role
                UserManager.AddToRoleAsync(poweruser, "Administrator").Wait();
                UserManager.AddToRoleAsync(poweruser, "BookEditor").Wait();
                //var res =  UserManager.GetRolesAsync(poweruser).Result;
            }
        }
    }

    private static async Task CreateSampleUsers(IServiceProvider serviceProvider)
    {
        //initializing custom roles 
        var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        string[] roleNames = { UserRoles.Administrator, UserRoles.Customer, UserRoles.Logist, UserRoles.BookEditor };
        IdentityResult roleResult;



        //Here you could create a super user who will maintain the web app
        var logist = new ApplicationUser
        {

            UserName = "logist@logist.com",
            Email = "logist@logist.com",
            EmailConfirmed = true,
        };
        //Ensure you have these values in your appsettings.json file
        string userPWD = "Topol$110";


        var _user = await UserManager.GetUsersInRoleAsync("Logist");

        if (_user.Count == 0)
        {

            var createLogist = await UserManager.CreateAsync(logist, userPWD);
            if (createLogist.Succeeded)
            {
                //here we tie the new user to the role
                await UserManager.AddToRoleAsync(logist, "Logist");

            }
        }

        var seller = new ApplicationUser
        {

            UserName = "seller@seller.com",
            Email = "seller@seller.com",
            EmailConfirmed = true,
        };
        //Ensure you have these values in your appsettings.json file


        _user = await UserManager.GetUsersInRoleAsync("Customer");

        if (_user.Count == 0)
        {

            var createSeller = await UserManager.CreateAsync(seller, userPWD);
            if (createSeller.Succeeded)
            {
                //here we tie the new user to the role
                await UserManager.AddToRoleAsync(seller, "Customer");

            }
        }

    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

