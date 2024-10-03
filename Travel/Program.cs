using Travel.Models;
using Microsoft.AspNetCore.Identity;
using Travel.Services;
using Microsoft.EntityFrameworkCore;

namespace Travel
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Рядок підключення для Core Identity з файла конфігурації.
            string? identityConnection = builder.Configuration.GetConnectionString("IdentityConnection");

            // Додаю контекст ApplicationDbContext в якості сервіса.
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(identityConnection));
            // Додаю з IdentitySample.Mvc (файл Startup.cs).
            // https://github.com/dotnet/aspnetcore/tree/main/src/Identity/samples/IdentitySample.Mvc
            builder.Services.AddMvc();
            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
                        .AddRoles<IdentityRole>()
                        .AddEntityFrameworkStores<ApplicationDbContext>()
                        .AddSignInManager()
                        .AddDefaultTokenProviders();
            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            }).AddIdentityCookies(o => { });
            builder.Services.AddDistributedMemoryCache(); // Додаю використання IDistributedCache.
            builder.Services.AddSession(); // Додаю сервіси сесії.

            // Add application services.
            builder.Services.AddTransient<IEmailSender, AuthMessageSender>();
            builder.Services.AddTransient<ISmsSender, AuthMessageSender>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    ApplicationDbContext applicationIdentityDbContext = services.GetRequiredService<ApplicationDbContext>();
                    applicationIdentityDbContext.Database.Migrate();

                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    string userRoleModerator = "Moderator";
                    string moderatorEmail = "moderator@example.com";
                    string moderatorUserName = "moderator";
                    string password = "Qwerty+1";
                    if (await roleManager.FindByNameAsync(userRoleModerator) == null)
                    {
                        await roleManager.CreateAsync(new IdentityRole(userRoleModerator));
                    }
                    if (await userManager.FindByNameAsync(moderatorUserName) == null)
                    {
                        ApplicationUser userModerator = new ApplicationUser
                        {
                            UserName = moderatorUserName,
                            Email = moderatorEmail
                        };
                        IdentityResult result = await userManager.CreateAsync(userModerator, password);
                        if (result.Succeeded == true)
                        {
                            await userManager.AddToRoleAsync(userModerator, userRoleModerator);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, ex.Message);
                }
            }
            app.UseSession(); // Додаю middleware для роботи з сесіями.

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Обробка StatusCodes.
            app.UseStatusCodePagesWithReExecute("/Home/HandleError/{0}");

            // Підключення аутентифікації.
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}