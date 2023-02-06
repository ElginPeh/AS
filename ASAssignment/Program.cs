using ASAssignment.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders()
    .AddPasswordValidator<PasswordValidator<ApplicationUser>>();
builder.Services.ConfigureApplicationCookie(Config =>
{
    Config.LoginPath = "/Login";
});
builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options=>
{
    options.Cookie.Name = "MyCookieAuth";
    options.AccessDeniedPath = "/errors/403";
});

/*builder.Services.AddRecaptcha(new RecaptchaOptions
{
    SiteKey = Configuration["ReCaptcha:SiteKey"],
    SecretKey = Configuration["ReCaptcha:SecretKey"]
});*/

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("MustBelongToAdmin",
        policy => policy.RequireClaim("Role", "Admin"));

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();  
app.UseStaticFiles();

//Error handling
app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();
app.UseAuthentication();

/*app.Use(async (context, next) =>
{
    if (!context.User.Identity.IsAuthenticated && !context.Request.Path.Value.Contains("/Login"))
    {
        context.Response.Redirect("/Login");
        return;
    }
    await next.Invoke();
});*/

app.UseAuthorization();

app.MapRazorPages();

app.Run();
