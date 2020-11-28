using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.Misc.IssueManagement.Domain;
using Nop.Plugin.Misc.IssueManagement.Models;

namespace Nop.Plugin.Misc.IssueManagement.Infrastructure.Mapper
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public MapperConfiguration()
        {
            CreateMap<AddIssueModel, Issue>()
                .ForMember(entity => entity.Priority, options => options.DoNotAllowNull())
                .ForMember(entity => entity.Status, options => options.DoNotAllowNull());

            CreateMap<EditBasicInfoPanelModel, Issue>()
                .ForMember(entity => entity.Priority, options => options.DoNotAllowNull())
                .ForMember(entity => entity.Status, options => options.DoNotAllowNull());

            CreateMap<Issue, IssueGridItemModel>();
            CreateMap<IssueHistory, IssueHistoryGridItem>();
            CreateMap<IssueAssignment, IssueAssignmentsGridItem>();
            CreateMap<AddIssueCommentModel, IssueComment>();
            CreateMap<IssueComment, IssueCommentsGridItem>();
        }

        public int Order => 1;
    }
}