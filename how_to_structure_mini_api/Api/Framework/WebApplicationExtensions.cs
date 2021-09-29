using Api.Framework;
using FluentValidation;

namespace Api.Framework
{
    public static class WebApplicationExtensions
    {
        public static IEndpointConventionBuilder MapGet<TRequest>(
            this WebApplication app,
            string pattern
            )
            where TRequest : IRequest, new()
        {
            return app.MapGet(pattern, RequestHandler<TRequest>);
        }

        public static IEndpointConventionBuilder MapPost<TRequest>(
            this WebApplication app,
            string pattern
            )
            where TRequest : IRequest, new()
        {
            return app.MapPost(pattern, RequestHandler<TRequest>);
        }

        private static async Task RequestHandler<TRequest>(HttpContext ctx)
            where TRequest : IRequest, new()
        {
            var request = await ctx.ModelBindAsync<TRequest>();

            if (!await ctx.ValidateAsync(request))
            {
                return;
            }

            await ctx.HandleAsync(request);
        }

        private static async Task<TRequest> ModelBindAsync<TRequest>(this HttpContext ctx)
            where TRequest : IRequest, new()
        {
            var requestType = typeof(TRequest);
            var interfaces = requestType.GetInterfaces();
            
            TRequest result = interfaces.Any(x => x.Equals(typeof(IFromJsonBody)))
                ? (TRequest)await ctx.Request.ReadFromJsonAsync(requestType)
                : new TRequest();

            if(result is IFromRoute fr)
            {
                fr.BindFromRoute(ctx.Request.RouteValues);
            }

            if(result is IFromQuery fq)
            {
                fq.BindFromQuery(ctx.Request.Query);
            }

            if (result is IWithUserContext wu)
            {
                wu.BindFromUser(ctx.User);
            }

            return result;
        }

        private static async Task<bool> ValidateAsync<TRequest>(this HttpContext ctx, TRequest request)
            where TRequest : IRequest
        {
            var validorInterfaceType = typeof(IValidator<>).MakeGenericType(typeof(TRequest));
            var validator = (IValidator)ctx.RequestServices.GetService(validorInterfaceType);
            if (validator != null)
            {
                var context = new ValidationContext<object>(request);
                var validationResult = validator.Validate(context);
                if (!validationResult.IsValid)
                {
                    var validationErrors = new Dictionary<string, string>();

                    foreach (var error in validationResult.Errors)
                    {
                        validationErrors[error.PropertyName] = error.ErrorMessage;
                    }

                    ctx.Response.StatusCode = 400;
                    await ctx.Response.WriteAsJsonAsync(new
                    {
                        message = "failed validation.",
                        errors = validationErrors
                    });
                    return false;
                }
            }

            return true;
        }

        private static async Task HandleAsync<TRequest>(this HttpContext ctx, TRequest request)
            where TRequest : IRequest
        {
            var returnType = typeof(TRequest).GetInterfaces()[0].GetGenericArguments()[0];

            var handlerType = typeof(Handler<,>).MakeGenericType(typeof(TRequest), returnType);

            var handler = (IHandler)ctx.RequestServices.GetService(handlerType);

            var result = await handler.RunAsync(request);
            await ctx.Response.WriteAsJsonAsync(result);
        }
    }
}