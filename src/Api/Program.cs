using Api;
using Application;
using Application.CreatePost;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder
    .Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));

builder.Services.AddValidatorsFromAssemblyContaining<CreatePostRequest>(ServiceLifetime.Singleton);

var app = builder.Build();

app.MapPost("/posts", async ([Validate] CreatePostRequest request, IMediator mediator) => TypedResults.Ok(await mediator.Send(request)))
    .AddEndpointFilterFactory(ValidationFilter.ValidationFilterFactory);

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    // More exceptions thrown from mediator in case of missing identification etc. can be caught here and return a 
    // proper error message
    catch (Exception ex)
    {
        await Results.Problem().ExecuteAsync(context);
    }
});

app.Run();

public partial class Program {}