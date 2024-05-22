using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Cards;
using Istu.Navigation.Domain.Models.ExternalRoutes;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Buildings;
using Istu.Navigation.Public.Models.Cards;
using Istu.Navigation.Public.Models.ExternalRoutes;
using Istu.Navigation.Public.Models.Images;
using Istu.Navigation.Public.Models.Users;

namespace Istu.Navigation.Public.Models;

public class PublicMappingProfile : Profile
{
    public PublicMappingProfile()
    {
        CreateMap<BuildingObject, BuildingObjectDto>();
        CreateMap<Edge, EdgeDto>()
            .ForMember(x => x.FromId, x => x.MapFrom(src => src.From.Id))
            .ForMember(x => x.ToId, x => x.MapFrom(src => src.To.Id));
        CreateMap<FloorInfo, FloorInfoDto>();
        CreateMap<Floor, FloorDto>();
        CreateMap<Edge, EdgeDto>()
            .ForMember(x => x.FromId,
                x => x.MapFrom(src => src.From.Id))
            .ForMember(x => x.ToId,
                x => x.MapFrom(src => src.To.Id));
        CreateMap<FloorRoute, FloorRouteDto>();
        
        CreateMap<BuildingRoute, BuildingRouteResponse>()
            .ForMember(x => x.BuildingId, x => x.MapFrom(src => src.Building.Id))
            .ForMember(x => x.Floors, x => x.MapFrom(src => src.FloorRoutes));
        CreateMap<BuildingObjectDto, BuildingObject>();
        CreateMap<BuildingDto, Building>();
        CreateMap<Building, BuildingDto>();
        
        CreateMap<User, UserDto>();

        CreateMap<ExternalPoint, ExternalPointDto>();
        CreateMap<ExternalPointDto, ExternalPoint>();
        CreateMap<ExternalRoute, ExternalRouteResponse>()
            .ForMember(x => x.Points, x => x.MapFrom(src => src.Points))
            .ForMember(x => x.TotalTime, x => x.MapFrom(src => src.TotalTime));
        CreateMap<ImageInfo, ImageInfoDto>();


        CreateMap<SearchResult, SearchResultDto>();
        CreateMap<Comment, CommentDto>();
    }
}