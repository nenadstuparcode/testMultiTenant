using FinbuckleMultiTenant.Data;
using FinbuckleMultiTenant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinbuckleMultiTenant.Controllers;

[ApiController]
[Route("[controller]")]
public class BlogController(AppDbContext context) : ControllerBase
{

    [HttpGet()]
    public async Task<ActionResult> GetAllPosts()
    {
        var posts = await context.Blogs.ToListAsync();
        if(posts == null) return NotFound();
        
        return Ok(posts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetPostById([FromRoute]Guid id)
    {
        var post = await context.Blogs.FindAsync(id);
        if(post == null) return NotFound();
        
        return Ok(post);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeletePost([FromRoute] Guid id)
    {
        var blogToDelete = await context.Blogs.FindAsync(id);
        if(blogToDelete == null) return NotFound();

        context.Blogs.Remove(blogToDelete);
        
        return Ok(await context.SaveChangesAsync());
    }

    [HttpPost]
    public async Task<ActionResult> AddPost([FromBody] CreateBlogRequest blog)
    {
        Blog newBlog = new()
        {
            Title = blog.Title,
            Content = blog.Content,
        };

        var blogCreated = await context.Blogs.AddAsync(newBlog);
        
        await context.SaveChangesAsync();

        return Ok(blogCreated);

    }
}