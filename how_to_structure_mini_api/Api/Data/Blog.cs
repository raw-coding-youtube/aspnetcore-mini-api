public class Blog
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string CreatedBy { get; set; }

    public List<Post> Posts { get; set; } = new();
}
