using ElectronicCourses.Data;
using ElectronicCourses.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddControllersWithViews();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // create a role ADMIN 
    // tworzymy role admina 
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    // setting up an ADMIN role for user with specific e-mail address
    // ustawiamy u¿ytkownika z dan¹ poczt¹ jako admin
    var adminEmail = "admin@demo.pl";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
        await userManager.AddToRoleAsync(adminUser, "Admin");
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
app.MapControllers();
app.MapRazorPages();
app.Run();
