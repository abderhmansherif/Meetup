using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.DeleteEvent;
using MeetupWebApp.Features.Events.CreateEvent;
using MeetupWebApp.Features.Events.EditEvents;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Features.Events.ViewEvents;
using MeetupWebApp.Features.ViewSingleEvent;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.Security.Claims;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddAuthentication( op =>
{
    op.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    op.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    op.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? string.Empty;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? string.Empty;
        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddDbContextFactory<ApplicationDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<CreateEventService>();
builder.Services.AddTransient<ViewEventService>();
builder.Services.AddTransient<SharedHelper>();
builder.Services.AddTransient<EditEventsService>();
builder.Services.AddTransient<EventValidationService>();
builder.Services.AddTransient<DeleteEventService>();
builder.Services.AddTransient<DiscoverEventsService>();
builder.Services.AddTransient<ViewSingleEventService>();
builder.Services.AddSingleton<LayoutService>();
builder.Services.AddMudServices();

var app = builder.Build();

app.UseStaticFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapGet("/Authentication/{provider}", async (HttpContext ctx, string provider) =>
{
    var returnUrl = ctx.Request.Query["returnurl"].ToString();

    var redirectUrl = "signin-callback";

    if(!string.IsNullOrEmpty(returnUrl))
    {
        redirectUrl = redirectUrl + $"?returnurl={returnUrl}";
    }

    var properties = new AuthenticationProperties() { RedirectUri = redirectUrl };

    await ctx.ChallengeAsync(provider, properties);
});

app.MapGet("/signin-callback", async (HttpContext ctx) =>
{
    var returnUrl = ctx.Request.Query["returnurl"].ToString();

    var DecodedUrl = HttpUtility.UrlDecode(returnUrl);

    var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

    if(result.Succeeded && result.Principal is not null)
    {
        // Store a User
        var UsernameClaim = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
        var EmailClaim = result.Principal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email);

        // DbContextFactory
        var Factory = ctx.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        using var context = Factory.CreateDbContext();

        if(UsernameClaim is null && EmailClaim is null)
        {
            // Adding Error page
            return;
        }

        var userExist = await context.Users.FirstOrDefaultAsync(x => x.Email == EmailClaim!.Value);

        if(userExist is null)
        {
            // that means user is not exists
            userExist = new User()
            {
                Username = UsernameClaim!.Value,
                Email = EmailClaim!.Value,
                UserRole = SharedHelper.GetAttendeeRole()
            };

            await context.Users.AddAsync(userExist);
            await context.SaveChangesAsync();
        }
        else
        {
            userExist.Username = UsernameClaim!.Value;
            await context.SaveChangesAsync();
        }

        // Signin the Cookie
        await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Principal);

        if(!string.IsNullOrEmpty(DecodedUrl))
            ctx.Response.Redirect(DecodedUrl);

        return;
    }

    ctx.Response.Redirect("/");
    // TODO: return ErrorMessage
    return;
});

app.MapGet("logout", async (HttpContext ctx) =>
{
    if((ctx.User?.Identity?.IsAuthenticated ?? false))
    {
        await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
    ctx.Response.Redirect("/");
    return;
});


app.Run();
