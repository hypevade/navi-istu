using System.Security.Claims;
using AutoMapper;
using Istu.Navigation.Api.Extensions;
using Istu.Navigation.Api.Filters;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Models.Cards;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Domain.Services.Cards;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Public.Models.Cards;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.CardsRoutes.CardsApi)]
public class ObjectCardsController : ControllerBase
{
    private readonly ILuceneService service;
    private readonly IMapper mapper;
    private readonly ILogger<ObjectCardsController> logger;
    private readonly ICommentsService commentsService;
    private readonly ICardsService cardsService;

    public ObjectCardsController(ILuceneService service,
        IMapper mapper,
        ILogger<ObjectCardsController> logger,
        ICommentsService commentsService,
        ICardsService cardsService)
    {
        this.service = service;
        this.mapper = mapper;
        this.logger = logger;
        this.commentsService = commentsService;
        this.cardsService = cardsService;
    }

    [HttpGet]
    [Route(ApiRoutes.CardsRoutes.GetByIdPart)]
    public async Task<ActionResult<Card>> GetById(Guid objectId)
    {
        var getCardOperation = await cardsService.GetCard(objectId).ConfigureAwait(false);
        return getCardOperation.IsFailure
            ? StatusCode(getCardOperation.ApiError.StatusCode, getCardOperation.ApiError.ToErrorDto())
            : Ok(getCardOperation.Data);
    }

    [HttpGet]
    [Route(ApiRoutes.CardsRoutes.SearchPart)]
    public ActionResult<SearchResponse> Search([FromQuery] string text, [FromQuery] int limit = 10)
    {
        var foundDocuments = service.Search(text);
        if (foundDocuments.Count == 0)
            return Ok(new SearchResponse());

        var results = mapper.Map<List<SearchResultDto>>(foundDocuments);
        return Ok(new SearchResponse { Results = results });
    }
    
    [HttpPost]
    [Route(ApiRoutes.CardsRoutes.AddCommentPart)]
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
        
        return createOperation.IsFailure
            ? StatusCode(createOperation.ApiError.StatusCode, createOperation.ApiError.ToErrorDto())
            : Ok(createOperation.Data.Id);
    }
    
    [HttpDelete]
    [Route(ApiRoutes.CardsRoutes.DeleteCommentPart)]
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

        return deleteOperation.IsFailure
            ? StatusCode(deleteOperation.ApiError.StatusCode, deleteOperation.ApiError.ToErrorDto())
            : NoContent();
    }
    
    [HttpGet]
    [Route(ApiRoutes.CardsRoutes.GetCommentsPart)]
    [AuthorizationFilter(UserRole.User)]
    public async Task<ActionResult<List<CommentDto>>> GetCommentsByFilter([FromQuery] CommentFilter filter)
    {
        var getOperation = await commentsService.GetCommentsByFilter(filter).ConfigureAwait(false);
        
        return getOperation.IsFailure
            ? StatusCode(getOperation.ApiError.StatusCode, getOperation.ApiError.ToErrorDto())
            : Ok(mapper.Map<List<CommentDto>>(getOperation.Data));
    }
}