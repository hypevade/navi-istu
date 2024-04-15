using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.BuildingRoutes;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.Buildings;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Buildings.BuildingsApi)]
public class BuildingsController : ControllerBase
{
    private readonly IMapper mapper;
    private readonly IBuildingsService buildingsService;

    public BuildingsController(IMapper mapper, IBuildingsService buildingsService)
    {
        this.mapper = mapper;
        this.buildingsService = buildingsService;
    }

    [HttpPost]
    [Route(ApiRoutes.Buildings.CreatePart)]
    public async Task<ActionResult<CreateBuildingResponse>> Create([FromBody] CreateBuildingRequest request)
    {
        var building = new Building()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Floors = request.Floors.Select(x => new FloorInfo(x.FloorNumber, x.ImageLink)).ToList()
        };
        var createOperation = await buildingsService.Create(building).ConfigureAwait(false);

        if (createOperation.IsFailure)
            return StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto());

        return Ok(new CreateBuildingResponse { BuildingId = createOperation.Data });
    }

    //Todo: add patch method
    [HttpPatch]
    [Route(ApiRoutes.Buildings.UpdatePart)]
    public async Task<IActionResult> Update([FromBody] UpdateBuildingRequest request)
    {
        return Ok();
        /*var building = mapper.Map<Building>(request.Building);

        var patchBuildings = await buildingsService.Patch(building).ConfigureAwait(false);
        if (patchBuildings.IsFailure)
        {
            var apiError = patchBuildings.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Accepted();*/
    }

    [HttpDelete]
    [Route(ApiRoutes.Buildings.DeletePart)]
    public async Task<IActionResult> Delete(Guid buildingId)
    {
        var createOperation = await buildingsService.Delete(buildingId).ConfigureAwait(false);
        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Accepted();
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetPart)]
    public async Task<ActionResult<BuildingDto>> GetById(Guid buildingId)
    {
        var getBuilding = await buildingsService.GetById(buildingId).ConfigureAwait(false);
        if (getBuilding.IsFailure)
        {
            var apiError = getBuilding.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        var buildingDto = mapper.Map<BuildingDto>(getBuilding.Data);
        return buildingDto;
    }

    [HttpGet]
    [Route(ApiRoutes.Buildings.GetAllPart)]
    public async Task<ActionResult<List<BuildingDto>>> GetAllByFilter([FromQuery] BuildingFilter filter)
    {
        var getBuilding = await buildingsService.GetAllByFilter(filter).ConfigureAwait(false);
        if (getBuilding.IsFailure)
        {
            var apiError = getBuilding.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<BuildingDto>>(getBuilding.Data));
    }
}

