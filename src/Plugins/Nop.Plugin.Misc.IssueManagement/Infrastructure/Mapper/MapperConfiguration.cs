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
            CreateMap<Issue, AddIssueModel>()
                .ForMember(model => model.Priority, options => options.DoNotAllowNull())
                .ForMember(model => model.Status, options => options.DoNotAllowNull())
                .ForMember(model => model.PrioritySelectList, options => options.Ignore())
                .ForMember(model => model.StatusSelectList, options => options.Ignore());               
            CreateMap<AddIssueModel, Issue>()
                .ForMember(entity => entity.Deleted, options => options.Ignore())
                .ForMember(entity => entity.CreatedAt, options => options.Ignore())
                .ForMember(entity => entity.CreatedByCustomerId, options => options.Ignore())
                .ForMember(entity => entity.LastModified, options => options.Ignore());
        }

        public int Order => 1;
    }
}