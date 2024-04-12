using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.BuildingRoutes;
using Istu.Navigation.Public.Models.Buildings;

namespace Istu.Navigation.Public.Models;

public class PublicMappingProfile : Profile
{
    public PublicMappingProfile()
    {
        CreateMap<BuildingObject, BuildingRoutes.BuildingObjectDto>();
        CreateMap<PublicObjectType, BuildingObjectType>();
        CreateMap<Edge, EdgeDto>()
            .ForMember(x => x.FromId, x => x.MapFrom(src => src.From.Id))
            .ForMember(x => x.ToId, x => x.MapFrom(src => src.To.Id));
        
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
        CreateMap<BuildingObjectDto, BuildingObject>();
        CreateMap<BuildingDto, Building>();
        CreateMap<Building, BuildingDto>();
        
        /*CreateMap<FullBuildingObjectDto, BuildingObject>()
            .ForMember(x => x.BuildingId, x => x.MapFrom(src => src.BuildingId))
            .ForMember(x => x.FloorNumber, x => x.MapFrom(src => src.FloorNumber))
            .ForMember(x => x.Id, x => x.MapFrom(src => src.Id))
            .ForMember(x => x.Type, x => x.MapFrom(src => src.Type))
            .ForMember(x => x.Title, x => x.MapFrom(src => src.Title))
            .ForMember(x => x.X, x => x.MapFrom(src => src.X))
            .ForMember(x => x.Y, x => x.MapFrom(src => src.Y))
            .ForMember(x => x.Description, x => x.MapFrom(src => src.Description));
    */
    }
}