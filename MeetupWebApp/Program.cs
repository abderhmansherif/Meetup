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
using MeetupWebApp.Shared.Authentication;
using MeetupWebApp.Shared.Endpoints;
using MeetupWebApp.Shared.Policies.SameUserPolicy;
using MeetupWebApp.Shared.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
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
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.AccessDeniedPath = "/AccessDenied";
    })
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
                await GoogleOAuthEvents.OnCreatingTicketEventAsync(context);
            },  
        };
    });

builder.Services.AddAuthorization(op =>
{
    op.AddPolicy("SameUser", policy =>
    {
        policy.AddRequirements(new SameUserRequirement());
    });
});


builder.Services.AddDbContextFactory<ApplicationDbContext>(op =>
{
    op.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<IAuthorizationHandler, SameUserHandler>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<CreateEventService>();
builder.Services.AddHttpContextAccessor();
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




