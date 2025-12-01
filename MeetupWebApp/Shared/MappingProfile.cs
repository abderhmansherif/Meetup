using AutoMapper;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Shared.ViewModels;

namespace MeetupWebApp.Shared
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<EventViewModel, Event>();
            CreateMap<Event, EventViewModel>();
        }
    }
}
