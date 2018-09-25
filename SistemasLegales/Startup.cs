﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EnviarCorreo;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SistemasLegales.Models.Entidades;
using SistemasLegales.Models.Utiles;
using SistemasLegales.Services;

namespace SistemasLegales
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SistemasLegalesContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfire(_ => _.UseSqlServerStorage(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<SistemasLegalesContext>()
                .AddDefaultTokenProviders();

            services.AddMvc(config => {
                config.ModelBindingMessageProvider.ValueMustBeANumberAccessor = (value) => $"El valor del campo {value} es inválido.";
                config.ModelBindingMessageProvider.ValueMustNotBeNullAccessor = value => $"Debe introducir el {value}";
            });

            services.AddAuthorization(opts => {
                opts.AddPolicy("Administracion", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrador");
                });

                opts.AddPolicy("Gestion", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Gestor");
                });

                opts.AddPolicy("Gerencia", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Gerencia");
                });

                opts.AddPolicy("GerenciaGestion", policy => {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Gerencia", "Gestor");
                });
            });

            services.AddMvc();
            
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<TimedHostedService>();
            services.AddSingleton<IUploadFileService, UploadFileService>();

            services.AddMemoryCache();
            services.AddSession();

            // Configuración del correo
            MailConfig.HostUri = Configuration.GetSection("Smtp").Value;
            MailConfig.PrimaryPort = Convert.ToInt32(Configuration.GetSection("PrimaryPort").Value);
            MailConfig.SecureSocketOptions = Convert.ToInt32(Configuration.GetSection("SecureSocketOptions").Value);

            MailConfig.RequireAuthentication = Convert.ToBoolean(Configuration.GetSection("RequireAuthentication").Value);
            MailConfig.UserName = Configuration.GetSection("UsuarioCorreo").Value;
            MailConfig.Password = Configuration.GetSection("PasswordCorreo").Value;

            MailConfig.EmailFrom = Configuration.GetSection("EmailFrom").Value;
            MailConfig.NameFrom = Configuration.GetSection("NameFrom").Value;

            ConstantesCorreo.MensajeCorreoSuperior = Configuration.GetSection("MensajeCorreoSuperior").Value;

            //Constantes de envio de notificación por email
            ConstantesTimerEnvioNotificacion.Hora = int.Parse(Configuration.GetSection("Hora").Value);
            ConstantesTimerEnvioNotificacion.Minutos = int.Parse(Configuration.GetSection("Minutos").Value);
            ConstantesTimerEnvioNotificacion.Segundos = int.Parse(Configuration.GetSection("Segundos").Value);
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, TimedHostedService timedHostedService/*, IServiceProvider serviceProvider*/)
        {
            var defaultCulture = new CultureInfo("es-ec");
            defaultCulture.NumberFormat.NumberDecimalSeparator = ".";
            defaultCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(defaultCulture),
                SupportedCultures = new List<CultureInfo> { defaultCulture },
                SupportedUICultures = new List<CultureInfo> { defaultCulture },
                FallBackToParentCultures = false,
                FallBackToParentUICultures = false,
                RequestCultureProviders = new List<IRequestCultureProvider> { }
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Principal/Error");
            }
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Login}/{id?}");
            });
            //CreateRoles(serviceProvider);
            //CreateUsers(serviceProvider);

            app.UseHangfireDashboard();
            app.UseHangfireServer();

            BackgroundJob.Enqueue(() => timedHostedService.EnviarNotificacionRequisitos() );
            RecurringJob.AddOrUpdate(() => timedHostedService.EnviarNotificacionRequisitos(), $"{ConstantesTimerEnvioNotificacion.Minutos} {ConstantesTimerEnvioNotificacion.Hora} * * *");
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] rolesName = new string[] { Perfiles.Administrador, Perfiles.Gerencia, Perfiles.Gestor };
            IdentityResult result;
            foreach (var item in rolesName)
            {
                var roleExist = roleManager.RoleExistsAsync(item).Result;
                if (!roleExist)
                {
                    //Se crean los roles si no existen en la BD
                    result = roleManager.CreateAsync(new IdentityRole(item)).Result;
                }
            }
        }

        private void CreateUsers(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var usersName = new ApplicationUser[]
            {
                new ApplicationUser { UserName = "Administrador", Email = "administrador@bekaert.com" },
                new ApplicationUser { UserName = "Gerencia", Email = "gerencia@bekaert.com" },
                new ApplicationUser { UserName = "Gestor", Email = "gestor@bekaert.com" }
            };
            IdentityResult result;
            foreach (var item in usersName)
            {
                var user = userManager.FindByNameAsync(item.UserName).Result;
                if (user == null)
                {
                    //Se crean los usuarios si no existen en la BD
                    switch (item.UserName)
                    {
                        case "Administrador": result = userManager.CreateAsync(item, "Administrador2018*").Result; break;
                        case "Gerencia": result = userManager.CreateAsync(item, "Gerencia2018*").Result; break;
                        case "Gestor": result = userManager.CreateAsync(item, "Gestor2018*").Result; break;
                    }
                }

                //Se asignan los roles a los usuarios si no existen en la BD
                switch (item.UserName)
                {
                    case "Administrador": result = userManager.AddToRoleAsync(item, Perfiles.Administrador).Result; break;
                    case "Gerencia": result = userManager.AddToRoleAsync(item, Perfiles.Gerencia).Result; break;
                    case "Gestor": result = userManager.AddToRoleAsync(item, Perfiles.Gestor).Result; break;
                }
            }
        }
    }
}
