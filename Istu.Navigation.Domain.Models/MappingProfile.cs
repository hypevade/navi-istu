using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;

namespace Istu.Navigation.Domain.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BuildingObject, BuildingObjectEntity>();
        CreateMap<BuildingObjectEntity, BuildingObject>().ConstructUsing(x =>
            new BuildingObject(x.Id, x.BuildingId, x.Title, x.Floor, x.Type, x.X, x.Y, x.Description));
        CreateMap<BuildingEntity, Building>();
        CreateMap<Building, BuildingEntity>();
        
        CreateMap<Floor, FloorEntity>()
            .ForMember(x => x.ImageId, x => x.MapFrom(floor => floor.ImageLink.Id));
    }
}