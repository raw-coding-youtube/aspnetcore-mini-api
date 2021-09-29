using Api;
using Api.Framework;
using Api.Services;
using static Api.EndpointAuthenticationDeclaration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiServices();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

Anonymous(
    app.MapGet<GetBlogsRequest>("/blogs"),
    app.MapGet<GetBlogRequest>("/blogs/{id}"),
    // curl -i -X POST -H "Content-Type: application/json" -d "{\"title\":\"test body\"}" "http://localhost:5000/test/1?v=test"
    app.MapPost<TestRequest>("/test/{id}")
);

Admin(
    // curl -i -X POST -H "Content-Type: application/json" -d "{\"title\":\"boi\"}" http://localhost:5000/admin/blogs
    app.MapPost<CreateBlogRequest>("/admin/blogs")
);

app.Run();