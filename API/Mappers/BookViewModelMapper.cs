using API.Models;
using AutoMapper;

namespace API.Mappers
{
    /// <summary>
    /// View model mapper to only get necessary information from entity to display to user
    /// </summary>
    public static class BookViewModelMapper
    {
        internal static IMapper Mapper { get; }

        static BookViewModelMapper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<BookViewModelMapperProfile>())
                .CreateMapper();
        }

        public static IEnumerable<BookViewModel> ToViewModel(this IEnumerable<BookWithIdDto> dtos)
        {
            return Mapper.Map<IEnumerable<BookViewModel>>(dtos);
        }

        public static BookViewModel ToViewModel(this BookWithIdDto dtos)
        {
            return Mapper.Map<BookViewModel>(dtos);
        }
    }

    public class BookViewModelMapperProfile : Profile
    {
        public BookViewModelMapperProfile()
        {
            CreateMap<BookWithIdDto, BookViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.ISBN, opt => opt.MapFrom(src => src.ISBN))
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate));
        }
    }
}
