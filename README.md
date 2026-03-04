# Meetup Web App

**A Blazor Server clone of Meetup.com with payments, reviews, and organizer tools**

This is my side project – basically Meetup.com but built from scratch using **Blazor Server** (.NET).  
I wanted something where people can actually discover events, RSVP, pay if needed, leave reviews for the organizer, and everything feels smooth. No fancy frontend framework, just pure Blazor Server + vertical slices so the code stays sane even as it grows.

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

### Project Structure (the important parts)

MeetupWebApp/
├── Data/
│   ├── Entities/          ← User, Event, RSVP, Transaction, Comment, OrganizerReview...
│   └── ApplicationDbContext.cs
├── Features/
│   ├── CreateEvent/
│   ├── EditEvents/
│   ├── DeleteEvent/
│   ├── DiscoverEvents/
│   ├── ViewSingleEvent/   ← with AttendeesComponent
│   ├── RSVPEvent/
│   ├── CancelRSVP/
│   ├── MakePayment/       ← endpoints + payment status + refunds
│   ├── LeaveAComment/
│   ├── LeaveOrganizerReview/
│   ├── ViewOrganizerReviews/ + average
│   ├── ViewOrganizerMeetups/
│   ├── ManageUserRSVPEvents/
│   ├── ViewTransactions/
│   └── BeAnOrganizer/
├── Shared/
│   ├── Components/        ← reusable dialogs, nav, search, etc.
│   ├── Policies/
│   ├── Services/
│   ├── Authentication/
│   └── Enums/
├── Migrations/            ← all the evolution steps (payments, refunds, reviews...)
├── wwwroot/images/events/ ← sample event photos
└── Program.cs + Routes + App.razor

### Tech I used

- .NET 8 Blazor Server
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
