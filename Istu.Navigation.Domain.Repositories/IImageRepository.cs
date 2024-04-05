using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Models.Entities;
using Istu.Navigation.Domain.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace Istu.Navigation.Domain.Repositories;

public interface IImageRepository : IRepository<ImageLinkEntity>;

public class ImageRepository : Repository<ImageLinkEntity>, IImageRepository
{
    public ImageRepository(DbContext context) : base(context)
    { }
}