using AutoMapper;

namespace Genealogy.Models
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CemeteryDto, Cemetery>();
            CreateMap<Cemetery, CemeteryDto>();

            CreateMap<PersonDto, Person>();
            CreateMap<Person, PersonDto>();

            CreateMap<PageDto, Page>();
            CreateMap<Page, PageDto>();
            CreateMap<Page, PageWithLinksDto>();

            CreateMap<Page, PageListItemDto>();

            CreateMap<Link, LinkDto>();
            CreateMap<LinkDto, Link>();

            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();


        }
    }
}