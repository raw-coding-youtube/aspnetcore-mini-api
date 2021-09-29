using System.Security.Claims;

namespace Api.Framework
{
    public interface IRequest
    {
    }

    public interface IFromQuery
    {
        void BindFromQuery(IQueryCollection queryCollection);
    }
    public interface IFromRoute
    {
        void BindFromRoute(RouteValueDictionary routeValues);
    }
    public interface IWithUserContext
    {
        void BindFromUser(ClaimsPrincipal user);
    }

    public interface IFromJsonBody
    {
    }

    public interface IRequest<TOut> : IRequest
    {
    }
}
