using Api.Framework;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Api.Services
{
    public class GetBlogRequest : IRequest<Blog>, IFromRoute
    {
        public int Id { get; private set; }

        public void BindFromRoute(RouteValueDictionary routeValues)
        {
            Id = routeValues.GetInt("id");
        }
    }

    public class GetBlogRequestValidation : AbstractValidator<GetBlogRequest>
    {
        public GetBlogRequestValidation()
        {
            RuleFor(r => r.Id).Must(v => v > 0).WithMessage("Id needs to be more than 0.");
        }
    }

    public class GetBlog : Handler<GetBlogRequest, Blog>
    {
        private readonly AppDbContext ctx;

        public GetBlog(AppDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override Task<Blog> Run(GetBlogRequest v)
        {
            return ctx.Blogs.FirstOrDefaultAsync(x => x.Id == v.Id);
        }
    }
}
