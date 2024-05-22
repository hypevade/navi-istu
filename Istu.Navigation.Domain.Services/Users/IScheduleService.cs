using Istu.Navigation.Domain.Services.Buildings;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;

namespace Istu.Navigation.Domain.Services.Users;

public interface IScheduleService
{
    Task<OperationResult<List<Lesson>>> GetUserSchedule(Guid userId);
}

public class MockedScheduleService : IScheduleService
{
    private readonly IBuildingObjectsService buildingObjectsService;

    public MockedScheduleService(IBuildingObjectsService buildingObjectsService)
    {
        this.buildingObjectsService = buildingObjectsService;
    }

    public async Task<OperationResult<List<Lesson>>> GetUserSchedule(Guid userId)
    {
        var (year, month, day) = DateTime.UtcNow;
        var lessons = new List<Lesson>()
        {
            new()
            {
                Audience = "3-12",
                Lector = "Чернышев К.С.",
                StartTime = new DateTime(year, month, day, 6, 10, 0, DateTimeKind.Utc),
                EndTime = new DateTime(year, month, day, 7, 40, 0, DateTimeKind.Utc),
                Title = "Лекция 'Основы программирования'"
            },
            new()
            {
                Audience = "3-2",
                Lector = "Чернышев К.С.",
                StartTime = new DateTime(year, month, day, 8, 20, 0, DateTimeKind.Utc),
                EndTime = new DateTime(year, month, day, 9, 50, 0, DateTimeKind.Utc),
                Title = "Практика 'Основы программирования'"
            }
        };
        foreach (var lesson in lessons)
        {
            var buildingObjectId = await GetBuildingObjectId(lesson.Audience).ConfigureAwait(false);
            if(buildingObjectId is null)
                continue;
            lesson.AudienceId = buildingObjectId;
        }

        return OperationResult<List<Lesson>>.Success(lessons);
    }

    private async Task<Guid?> GetBuildingObjectId(string audience)
    {
        var filter = new BuildingObjectFilter
        {
            Title = audience
        };
        var buildingObject = await buildingObjectsService.GetAllByFilterAsync(filter).ConfigureAwait(false);
        if (buildingObject.IsFailure || buildingObject.Data.Count == 0)
            return null;
        return buildingObject.Data.First().Id;
    }
}

public class Lesson
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public required string Title { get; set; }
    public required string Lector { get; set; }
    public required string Audience { get; set; }
    public Guid? AudienceId { get; set; }
}