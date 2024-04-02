using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BuildingObject, BuildingObjectEntity>();
        CreateMap<BuildingObjectEntity, BuildingObject>();
        CreateMap<BuildingEntity, Building>();
        CreateMap<Building, BuildingEntity>();
        CreateMap<Building, BuildingEntity>();
    }
}