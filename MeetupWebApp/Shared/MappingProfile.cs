using AutoMapper;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using MeetupWebApp.Features.LeaveAComment;

namespace MeetupWebApp.Shared
{
    public class MappingProfile: Profile
    {
        public MappingProfile() 
        {
            CreateMap<EventViewModel, Event>();
            CreateMap<Event, EventViewModel>();
            CreateMap<Comment, CommentViewModel>();
            CreateMap<CommentViewModel, Comment>();
        }
    }
}
