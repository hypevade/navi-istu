using AutoMapper;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;

namespace Istu.Navigation.Domain.Models;

public class DomainMappingProfile : Profile
{
    public DomainMappingProfile()
    {
        CreateMap<BuildingObject, BuildingObjectEntity>();
        CreateMap<BuildingObjectEntity, BuildingObject>().ConstructUsing(x =>
            new BuildingObject(x.Id, x.BuildingId, x.Floor, x.Type, x.X, x.Y, x.Title, x.Description, null));
        CreateMap<BuildingEntity, Building>();
        CreateMap<Building, BuildingEntity>();
        
        CreateMap<ImageInfo, ImageInfoEntity>();
        CreateMap<ImageInfoEntity, ImageInfo>();
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>();
    }
}