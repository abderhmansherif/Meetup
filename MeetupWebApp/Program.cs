using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.DeleteEvent;
using MeetupWebApp.Features.Events.CreateEvent;
using MeetupWebApp.Features.Events.EditEvents;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Features.Events.ViewEvents;
using MeetupWebApp.Features.ManageUserRSVPEvents;
using MeetupWebApp.Features.RSVPEvent;
using MeetupWebApp.Features.ViewSingleEvent;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.Endpoints;
using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
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
        options.Events = new OAuthEvents()
        {
            OnCreatingTicket = async context =>
            {
                if (context.Principal is null || context.Principal.Claims is null || context.Identity is null)
                {
                    // Erorr Here
                    context.HttpContext.Response.Redirect("/");
                    return;
                }

                var UsernameClaim = context.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name);
                var EmailClaim = context?.Principal?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Email);

                // DbContextFactory
                var Factory = context.HttpContext.RequestServices.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
                using var contextDb = Factory.CreateDbContext();

                if (UsernameClaim is null && EmailClaim is null)
                {
                    // Adding Error page
                    return;
                }

                var userExist = await contextDb.Users.FirstOrDefaultAsync(x => x.Email == EmailClaim!.Value);

                if (userExist is null)
                {
                    // that means user is not exists
                    userExist = new User()
                    {
                        Username = UsernameClaim!.Value,
                        Email = EmailClaim!.Value,
                        UserRole = SharedHelper.GetAttendeeRole()
                    };

                    await contextDb.Users.AddAsync(userExist);
                    await contextDb.SaveChangesAsync();
                }
                else
                {
                    userExist.Username = UsernameClaim!.Value;
                    await contextDb.SaveChangesAsync();
                }

                // Adding UserID to calims to be in the cookie 
                context.Identity.AddClaim(new Claim(SharedHelper.GetUserIdClaimType(), userExist.Id.ToString()));

            },  
        };
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
builder.Services.AddTransient<RSVPEventService>();
builder.Services.AddTransient<ManageUserRSVPEventsService>();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapAuth();

app.Run();
