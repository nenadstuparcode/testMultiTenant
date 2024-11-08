namespace FinbuckleMultiTenant.Models;

public record CreateBlogRequest {
    public string Title { get; init; }
    public string Content { get; init; }
}
