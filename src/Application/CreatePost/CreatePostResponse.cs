namespace Application.CreatePost;
public record CreatePostResponse(Guid PostIdentification, Guid CategoryIdentification, Guid UserIdentification,
    string Topic, string Content, DateTimeOffset CreatedAtUtc);
