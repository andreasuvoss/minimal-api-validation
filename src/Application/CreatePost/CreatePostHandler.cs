using MediatR;

namespace Application.CreatePost;

public class CreatePostHandler : IRequestHandler<CreatePostRequest, CreatePostResponse>
{
    public async Task<CreatePostResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        // In a real world scenario we check if the identification is present in the database
        var categoryIdentification = request.CategoryIdentification;

        // In a real world scenario we check if the identification is present in the database
        var userIdentification = request.UserIdentification;

        return new CreatePostResponse(Guid.NewGuid(), 
            categoryIdentification, 
            userIdentification,
            request.Topic, 
            request.Content, 
            DateTimeOffset.UtcNow);
    }
}