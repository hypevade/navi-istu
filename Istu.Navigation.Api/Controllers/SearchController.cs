using AutoMapper;
using Istu.Navigation.Api.Paths;
using Istu.Navigation.Domain.Services;
using Istu.Navigation.Public.Models.Search;
using Microsoft.AspNetCore.Mvc;

namespace Istu.Navigation.Api.Controllers;

[ApiController]
[Route(ApiRoutes.Search.SearchApi)]
public class SearchController(ILuceneService service, IMapper mapper) : ControllerBase
{
    private readonly IMapper mapper = mapper;


    [HttpGet]
    [Route(ApiRoutes.Search.SearchPart)]
    public ActionResult<SearchResponse> Search([FromQuery] string text, [FromQuery] int limit = 10)
    {
        var foundDocuments = service.Search(text);
        if (foundDocuments.Count == 0)
            return Ok(new SearchResponse());
        
        var results = mapper.Map<List<SearchResultDto>>(foundDocuments);
        return Ok(new SearchResponse { Results = results });
    }
}