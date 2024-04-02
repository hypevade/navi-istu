using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;

namespace Istu.Navigation.Public.Models;

public class PublicMappingProfile : Profile
{
    public PublicMappingProfile()
    {
        CreateMap<BuildingObject, BuildingObjectDto>();
        CreateMap<Edge, EdgeDto>();
        CreateMap<FloorRoute, FloorDto>()
            .ForMember(x => x.ImageLink, x => x.MapFrom(src => src.Floor.ImageLink))
            .ForMember(x => x.Edges, x => x.MapFrom(src => src.Floor.Edges))
            .ForMember(x => x.Objects, x => x.MapFrom(src => src.Floor.Objects))
            .ForMember(x => x.Route, x => x.MapFrom(src => src.Route))
            .ForMember(x => x.Number, x => x.MapFrom(src => src.Floor.FloorNumber))
            .ForMember(x => x.BuildingId, x => x.MapFrom(src => src.Floor.BuildingId));
        
        CreateMap<BuildingRoute, BuildingRouteResponse>()
            .ForMember(x => x.BuildingId, x => x.MapFrom(src => src.Building.Id))
            .ForMember(x => x.BuildingName, x => x.MapFrom(src => src.Building.Title))
            .ForMember(x => x.Floors, x => x.MapFrom(src => src.FloorRoutes))
            .ForMember(x => x.RouteId, x => x.MapFrom(src => src.RouteId))
            .ForMember(x => x.StartObjectDto, x => x.MapFrom(src => src.StartObject))
            .ForMember(x => x.FinishObjectDto, x => x.MapFrom(src => src.FinishObject));
    }
}