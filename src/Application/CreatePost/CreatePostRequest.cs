using FluentValidation;
using MediatR;

namespace Application.CreatePost;

public record CreatePostRequest(Guid CategoryIdentification, Guid UserIdentification, string Topic, string Content) : IRequest<CreatePostResponse>
{
    // Some BindAsync implementation could go here?
};

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(m => m.Content).NotEmpty().WithMessage($"{nameof(CreatePostRequest.Content)} should not be empty.");
        RuleFor(m => m.Topic).NotEmpty().WithMessage($"{nameof(CreatePostRequest.Topic)} should not be empty.");
        RuleFor(m => m.CategoryIdentification).NotEmpty().WithMessage($"{nameof(CreatePostRequest.CategoryIdentification)} should not be empty.");
        RuleFor(m => m.UserIdentification).NotEmpty().WithMessage($"{nameof(CreatePostRequest.UserIdentification)} should not be empty.");
    }
}
