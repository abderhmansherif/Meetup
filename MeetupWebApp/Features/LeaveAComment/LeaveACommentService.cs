using AutoMapper;
using MeetupWebApp.Data;
using MeetupWebApp.Data.Entities;
using MeetupWebApp.Features.Events.Shared;
using Microsoft.EntityFrameworkCore;

namespace MeetupWebApp.Features.LeaveAComment
{
    public class LeaveACommentService(IDbContextFactory<ApplicationDbContext> Factory, IMapper mapper)
    {
        public IDbContextFactory<ApplicationDbContext> Factory { get; } = Factory;
        public IMapper Mapper { get; } = mapper;


        public async Task<List<CommentViewModel>> GetCommentsByEventIdAsync (int eventid)
        {
            using var context = Factory.CreateDbContext ();

            List<Comment> comments = context.Comments.AsNoTracking()
                                            .Where(x => x.EventId == eventid)
                                            .OrderByDescending(x => x.PostedOn)
                                            .ToList();

            return Mapper.Map<List<CommentViewModel>>(comments);
        }

        public string ValidateComment(CommentViewModel comment)
        {
            if (comment is null)
                return "there is no comment to submit";

            if (string.IsNullOrWhiteSpace(comment.Message))
                return "Where the comment you wrote";

            if (comment.Message.Length > 1000)
                return "the comment is too big";

            return string.Empty;
        }

        public async Task AddCommentAsync(CommentViewModel comment)
        {
            using var context = await Factory.CreateDbContextAsync();

            var EventEntity = Mapper.Map<Comment>(comment);

            await context.AddAsync(EventEntity);
            await context.SaveChangesAsync();
        }

    }
}
