using Api.Framework;
using System.Security.Claims;

namespace Api.Services
{
    public class CreateBlogRequest : IRequest<Blog>, IFromJsonBody, IWithUserContext
    {
        public string Title { get; set; }
        public string CreatedBy { get; set; }

        public void BindFromUser(ClaimsPrincipal user)
        {
            CreatedBy = user.Claims.FirstOrDefault(x => x.Type == "username").Value;
        }
    }

    public class CreateBlog : Handler<CreateBlogRequest, Blog>
    {
        private readonly AppDbContext ctx;

        public CreateBlog(AppDbContext ctx)
        {
            this.ctx = ctx;
        }

        public override async Task<Blog> Run(CreateBlogRequest v)
        {
            var blog = new Blog
            {
                Title = v.Title,
                CreatedBy = v.CreatedBy
            };

            ctx.Add(blog);

            await ctx.SaveChangesAsync();

            return blog;
        }
    }
}
