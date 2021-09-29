using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Api.Framework;
using FluentValidation;

namespace Api
{
    public static class RegisterServices
    {
        public static void AddApiServices(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase("myDb"));

            services.AddServices<Program>()
                .AddValidation<Program>();

            services.AddAuthentication("bearer")
                .AddJwtBearer("bearer", opt =>
                {
                    opt.Events = new();
                    opt.Events.OnMessageReceived = (ctx) =>
                    {
                        var claims = new Claim[] { new("username", "bob master 3000") };
                        var identity = new ClaimsIdentity(claims, "bearer");
                        ctx.Principal = new ClaimsPrincipal(identity);
                        ctx.Success();
                        return Task.CompletedTask;
                    };
                });

            services.AddAuthorization(config =>
            {
                config.AddPolicy("admin", pb => pb
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("bearer"));
            });
        }

        public static IServiceCollection AddServices<T>(this IServiceCollection services)
        {
            var targetServices = typeof(T).Assembly.GetTypes()
                .Select(t => (t, t.BaseType))
                .Where((tuple) => tuple.BaseType != null)
                .Where((tuple) => tuple.BaseType.IsGenericType && tuple.BaseType.GetGenericTypeDefinition().IsEquivalentTo(typeof(Handler<,>)));

            foreach (var s in targetServices)
            {
                services.AddTransient(s.Item2, s.Item1);
            }

            return services;
        }

        public static IServiceCollection AddValidation<T>(this IServiceCollection services)
        {
            var validators = typeof(T).Assembly.GetTypes()
                .Select(t => (t, t.BaseType))
                .Where((tuple) => tuple.BaseType != null)
                .Where((tuple) => tuple.BaseType.IsGenericType
                    && tuple.BaseType.IsAbstract
                    && tuple.BaseType.GetGenericTypeDefinition().IsEquivalentTo(typeof(AbstractValidator<>)))
                .Select((tuple) => (tuple.t, tuple.BaseType.GetGenericArguments()[0]));

            foreach (var v in validators)
            {
                var validorInterfaceType = typeof(IValidator<>).MakeGenericType(v.Item2);
                services.AddTransient(validorInterfaceType, v.Item1);
            }

            return services;
        }
    }
}
