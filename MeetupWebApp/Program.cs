using MeetupWebApp.Data;
using MeetupWebApp.Features.Events.CreateEvent;
using MeetupWebApp.Features.Events.DeleteEvent;
using MeetupWebApp.Features.Events.EditEvents;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Features.Events.ViewEvents;
using MeetupWebApp.Features.Events.ViewSingleEvent;
using MeetupWebApp.Shared;
using MeetupWebApp.Shared.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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
builder.Services.AddTransient<LayoutService>();
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


app.Run();
