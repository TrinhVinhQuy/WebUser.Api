﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebUser.Application.Abstracts;
using WebUser.Application.Services;
using WebUser.Domain.Abstracts;
using WebUser.Domain.Entities;
using WebUser.Domain.Setting;
using WebUser.Persistence.Data;
using WebUser.Persistence.Repository;
using WebUser.Persistence.Services;

namespace WebUser.Persistence.Configuration
{
    public static class ConfigurationService
    {
        public static async Task AutoMigration(this WebApplication webApplication)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var appContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await appContext.Database.MigrateAsync();
            }
        }
        public static void ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
                options.Lockout.MaxFailedAccessAttempts = 3;
            });
        }
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddTransient<ITokenHandler, TokenHandler>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddScoped<PasswordHasher<ApplicationUser>>();
        }
        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
        public static async Task SeedData(this WebApplication webApplication, IConfiguration configuration)
        {
            using (var scope = webApplication.Services.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var defaultUser = configuration.GetSection("DefaultUser")?.Get<DefaultUser>() ?? new DefaultUser();
                var defaultRole = configuration.GetValue<string>("DefaultRole") ?? "Admin";

                try
                {
                    if (!await roleManager.RoleExistsAsync(defaultRole))
                    {
                        await roleManager.CreateAsync(new IdentityRole(defaultRole));
                    }

                    var existUser = await userManager.FindByNameAsync(defaultUser.Username);

                    if (existUser == null)
                    {
                        var user = new ApplicationUser
                        {
                            UserName = defaultUser.Username,
                            Fullname = defaultUser.Fullname,
                            Email = defaultUser.Email,
                            EmailConfirmed = true,
                            NormalizedEmail = defaultUser.Email.ToUpper(),
                            AccessFailedCount = 0,
                            IsActive = true,
                        };

                        var identityUser = await userManager.CreateAsync(user, defaultUser.Password);

                        if (identityUser.Succeeded)
                        {
                            await userManager.AddToRoleAsync(user, defaultRole);
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
