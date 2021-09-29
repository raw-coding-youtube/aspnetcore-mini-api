using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class TestController : ControllerBase
    {
        [HttpPost("/test/{id}")]
        public object GetBlog(int id, [FromQuery] string v, [FromBody] TestRequest request)
        {
            return new
            {
                result = $"{request.Title}_{id}_{v}"
            };
        }

        public class TestRequest
        {
            public string Title { get; set; }
        }
    }
}
