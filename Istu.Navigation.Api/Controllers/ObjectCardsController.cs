using System.Security.Claims;
using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.Cards;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.Cards;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Cards.CardsApi)]

public class ObjectCardsController(
    ILuceneService service,
    IMapper mapper,
    ILogger<ObjectCardsController> logger,
    ICommentsService commentsService) : ControllerBase
{
    [HttpGet]
    [Route(ApiRoutes.Cards.GetByIdPart)]
    public async Task<IActionResult> GetById(Guid objectId)
    {
        
        return Ok();
    }

    [HttpGet]
    [Route(ApiRoutes.Cards.SearchPart)]
    public ActionResult<SearchResponse> Search([FromQuery] string text, [FromQuery] int limit = 10)
    {
        var foundDocuments = service.Search(text);
        if (foundDocuments.Count == 0)
            return Ok(new SearchResponse());

        var results = mapper.Map<List<SearchResultDto>>(foundDocuments);
        return Ok(new SearchResponse { Results = results });
    }
    
    [HttpPost]
    [Route(ApiRoutes.Cards.AddCommentPart)]
    [AuthorizationFilter(UserRole.Student)]
    public async Task<ActionResult<Guid>> AddComment(Guid objectId, [FromQuery] string text)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            logger.LogError("Unexpected error, userIdClaim is null, but it shouldn't be");
            return Unauthorized();
        }
        var userId = Guid.Parse(userIdClaim.Value);
        var createOperation = await commentsService.CreateComment(objectId, userId, text).ConfigureAwait(false);
        if (createOperation.IsFailure)
        {
            var apiError = createOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(createOperation.Data.Id);
    }
    
    [HttpDelete]
    [Route(ApiRoutes.Cards.DeleteCommentPart)]
    [AuthorizationFilter(UserRole.Student)]
    public async Task<IActionResult> DeleteComment(Guid objectId, Guid commentId)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            logger.LogError("Unexpected error, userIdClaim is null, but it shouldn't be");
            return Unauthorized();
        }
        var userId = Guid.Parse(userIdClaim.Value);
        var deleteOperation = await commentsService.DeleteComment(commentId, userId).ConfigureAwait(false);
        if (deleteOperation.IsFailure)
        {
            var apiError = deleteOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }
        return NoContent();
    }
    
    [HttpGet]
    [Route(ApiRoutes.Cards.GetCommentsPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsByFilter([FromQuery] CommentFilter filter)
    {
        var getOperation = await commentsService.GetCommentsByFilter(filter).ConfigureAwait(false);
        if (getOperation.IsFailure)
        {
            var apiError = getOperation.ApiError;
            return StatusCode(apiError.StatusCode, apiError.ToErrorDto());
        }

        return Ok(mapper.Map<List<CommentDto>>(getOperation.Data));
    }
}