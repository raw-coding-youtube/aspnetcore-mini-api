using Api.Framework;

namespace Api.Services
{
    public class TestRequest : IRequest<object>, IFromJsonBody, IFromRoute, IFromQuery
    {
        public string Title { get; set; }
        public int FromRoute { get; set; }
        public string FromQuery { get; set; }

        public void BindFromQuery(IQueryCollection queryCollection)
        {
            if(queryCollection.TryGetValue("v", out var v))
            {
                FromQuery = v;
            }
        }

        public void BindFromRoute(RouteValueDictionary routeValues)
        {
            FromRoute = routeValues.GetInt("id");
        }
    }

    public class Test : Handler<TestRequest, object>
    {
        public override Task<object> Run(TestRequest v)
        {
            return Task.FromResult((object) new
            {
                result = $"{v.Title}_{v.FromRoute}_{v.FromQuery}"
            });
        }
    }
}
