using Api.Framework;
using Microsoft.EntityFrameworkCore;

namespace Api.Services
{
    public class GetBlogsRequest : IRequest<List<Blog>>
    {
    }

    public class GetBlogs : Handler<GetBlogsRequest, List<Blog>>
    {
        private readonly AppDbContext ctx;

        public GetBlogs(AppDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override Task<List<Blog>> Run(GetBlogsRequest v)
        {
            return ctx.Blogs.ToListAsync();
        }
    }
}
