# Meetup Web App

**A Blazor Server clone of Meetup.com with payments, reviews, and organizer tools**

This is my side project – basically Meetup.com but built from scratch using **Blazor Server** (.NET).  
I wanted something where people can actually discover events, RSVP, pay if needed, leave reviews for the organizer, and everything feels smooth. No fancy frontend framework, just pure Blazor Server + vertical slices so the code stays sane even as it grows.

<img width="1366" height="616" alt="Capture" src="https://github.com/user-attachments/assets/9b300887-c226-42ef-b682-affdf32b2de6" />
<img width="1345" height="645" alt="Capture12" src="https://github.com/user-attachments/assets/961dbe6a-a73b-4211-ba20-bd0635263b8e" />
<img width="1344" height="646" alt="Capture01" src="https://github.com/user-attachments/assets/c01fc7dc-85f5-4ace-931f-dbd757abfd5e" />

<img width="1366" height="566" alt="sa" src="https://github.com/user-attachments/assets/7117528d-1e03-46e3-a7aa-3dc0faae2323" />


### What it does (the real capabilities)

**For normal users:**
- Browse and search events (with categories)
- See full event details + who’s attending
- RSVP for free or paid events
- Pay securely (with proper success/failure flows and refund support)
- Cancel RSVP anytime
- Leave comments on events
- Check your own upcoming/past RSVPs
- View your full payment & refund history

**For organizers:**
- Apply to become an organizer (with policy enforcement)
- Create events (title, description, date, location, price, image, category)
- Edit or delete your own events
- See attendee list for each event
- Get rated by attendees after the event
- View all your reviews + average rating
- See history of all your meetups

**General stuff:**
- Google OAuth login (super clean with custom events)
- Role-based authorization (OnlyOrganizersPolicy + SameUserPolicy)
- Nice shared UI components (modals, confirmation dialogs, error messages, search bar, etc.)
- Fully responsive with Bootstrap + custom styling
- Image uploads for events (stored in wwwroot)

### Architecture – Why Vertical Slice?

I went full **Vertical Slice Architecture** because the traditional layered approach gets messy fast in Blazor apps.  
Every feature lives in its own folder under `Features/` with:
- Razor component(s)
- Service class
- Any ViewModels it needs
- Endpoints if it’s API-related (like payments)

Shared stuff (auth, policies, base components, enums, helpers) stays in `Shared/`.  
Data layer is clean and separate.  
This way if I want to tweak “LeaveAComment”, I just open that folder – everything is there.

### Tech I used

- .NET 10 Blazor Server
- Entity Framework Core + Code-First + Migrations
- AutoMapper
- ASP.NET Core Identity + Google OAuth
- Bootstrap 5
- Custom authorization policies
- Minimal API endpoints for payment flow

### How to run it locally

1. Clone the repo
2. Update `appsettings.json` (connection string + Google ClientId/Secret)
3. `dotnet ef database update` (or just let it create on first run)
4. `dotnet run`

That’s it. Should spin up on https://localhost:7xxx (check launchSettings).
