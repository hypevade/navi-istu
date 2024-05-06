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
            new BuildingObject(x.Id, x.BuildingId, x.Title, x.Floor, x.Type, x.X, x.Y, x.Description));
        CreateMap<BuildingEntity, Building>();
        CreateMap<Building, BuildingEntity>();
        
        CreateMap<ImageLink, ImageLinkEntity>();
        CreateMap<ImageLinkEntity, ImageLink>();
        CreateMap<User, UserEntity>();
        CreateMap<UserEntity, User>();
    }
}