using AutoMapper;
using Istu.Navigation.Domain.Models.Cards;
using Istu.Navigation.Domain.Models.Entities.Cards;
using Istu.Navigation.Domain.Models.Entities.User;
using Istu.Navigation.Domain.Models.Users;
using Istu.Navigation.Domain.Repositories.Buildings;
using Istu.Navigation.Domain.Repositories.Cards;
using Istu.Navigation.Domain.Repositories.Users;
using Istu.Navigation.Infrastructure.EF.Filters;
using Istu.Navigation.Infrastructure.Errors;
using Istu.Navigation.Infrastructure.Errors.UsersApiErrors;
using Microsoft.Extensions.Logging;

namespace Istu.Navigation.Domain.Services.Cards;

public interface ICommentsService
{
    public Task<OperationResult<Comment>> CreateComment(Guid objectId, Guid userId, string text);
    public Task<OperationResult> DeleteComment(Guid commentId, Guid userId);
    public Task<OperationResult<List<Comment>>> GetCommentsByFilter(CommentFilter filter);
}

public class CommentsService : ICommentsService
{
    private readonly ICommentsRepository commentsRepository;
    private readonly IUsersRepository usersRepository;
    private readonly IBuildingsRepository buildingsRepository;
    private readonly IBuildingObjectsRepository buildingObjectsRepository;
    private readonly ILogger<CommentsService> logger;
    private readonly IMapper mapper;  
    

    public CommentsService(ICommentsRepository commentsRepository, IUsersRepository usersRepository,
        IBuildingsRepository buildingsRepository, IBuildingObjectsRepository buildingObjectsRepository,
        ILogger<CommentsService> logger, IMapper mapper)
    {
        this.commentsRepository = commentsRepository;
        this.usersRepository = usersRepository;
        this.buildingsRepository = buildingsRepository;
        this.buildingObjectsRepository = buildingObjectsRepository;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task<OperationResult<Comment>> CreateComment(Guid objectId, Guid userId, string text)
    {
        var checkComment = await CheckCommentAndGetUser(objectId, userId, text).ConfigureAwait(false);
        if (checkComment.IsFailure)
            return OperationResult<Comment>.Failure(checkComment.ApiError);
        var user = checkComment.Data;
        var comment = new CommentEntity
        {
            Id = Guid.NewGuid(),
            CreatorId = userId,
            Text = text,
            ObjectId = objectId,
            CreationDate = DateTime.UtcNow
        };
        await commentsRepository.AddAsync(comment).ConfigureAwait(false);
        await commentsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult<Comment>.Success(Comment.FromEntity(comment, user.FirstName, user.LastName));
    }

    public async Task<OperationResult> DeleteComment(Guid commentId, Guid userId)
    {
        var filter = new CommentFilter {CommentId = commentId};
        var getComments = await commentsRepository.GetAllByFilter(filter).ConfigureAwait(false);
        if (getComments.Count == 0)
            return OperationResult.Success();
        
        var comment = getComments.First();
        var checkAccess = await CheckAccessToComment(comment, userId).ConfigureAwait(false);
        if(checkAccess.IsFailure)
            return checkAccess;
        
        await commentsRepository.RemoveByIdAsync(commentId).ConfigureAwait(false);
        await commentsRepository.SaveChangesAsync().ConfigureAwait(false);
        return OperationResult.Success();
    }

    public async Task<OperationResult<List<Comment>>> GetCommentsByFilter(CommentFilter filter)
    {
        var entities = await commentsRepository.GetCommentsWithUsersByFilterAsync(filter);
        var comments = mapper.Map<List<Comment>>(entities);
        return OperationResult<List<Comment>>.Success(comments);
    }

    private async Task<OperationResult> CheckAccessToComment(CommentEntity comment, Guid userId)
    {
        var user = await usersRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user is null)
            return OperationResult.Failure(UsersApiErrors.UserWithIdNotFoundError(userId));
        
        if (user.Role == UserRole.Admin)
            return OperationResult.Success();
        
        return user.Id == comment.CreatorId 
            ? OperationResult.Success() 
            : OperationResult.Failure(UsersApiErrors.AccessDeniedError());
    }

    private async Task<OperationResult<UserEntity>> CheckCommentAndGetUser(Guid objectId, Guid userId, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return OperationResult<UserEntity>.Failure(CommonErrors.InternalServerError());
        }

        var user = await usersRepository.GetByIdAsync(userId).ConfigureAwait(false);
        if (user is null)
            return OperationResult<UserEntity>.Failure(UsersApiErrors.UserWithIdNotFoundError(userId));
        if (user.Role == UserRole.User)
        {
            logger.LogError(
                $"User with id {user.Id} and role {user.Role.ToString()} tried to comment on object, but it is not allowed");
            return OperationResult<UserEntity>.Failure(UsersApiErrors.AccessDeniedError());
        }

        var buildingEntity = await buildingsRepository.GetByIdAsync(objectId).ConfigureAwait(false);
        var buildingObjectEntity = await buildingObjectsRepository.GetByIdAsync(objectId).ConfigureAwait(false);
        if (buildingEntity is null && buildingObjectEntity is null)
            return OperationResult<UserEntity>.Failure(CommonErrors.InternalServerError());
        return OperationResult<UserEntity>.Success(user);
    }
}