using Chirp.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Server.Controllers.Post;

[ApiController]
[Route("api/post")]
public class PostController : ControllerBase
{

  private readonly ILogger<PostController> _logger;
  private readonly ChirpContext _context;

  public PostController(ILogger<PostController> logger, ChirpContext context) {
    _logger = logger;
    _context = context;
  }

  /// <summary>
  /// Get post
  /// </summary>
  /// <response code="200">Post found</response>
  /// <response code="404">Post not found</response>
  [HttpPost]
  public async Task<IActionResult> GetPost() {
    return Ok("Post Get");
  }

  /// <summary>
  /// Create post
  /// </summary>
  /// <response code="200">Post created</response>
  /// <response code="500">Failed to update post</response>
  [HttpPut]
  public async Task<IActionResult> CreatePost() {
    return Ok("Post Created");
  }

  /// <summary>
  /// Update post
  /// </summary>
  /// <response code="200">Post updated</response>
  /// <response code="404">Post not found</response>
  /// <response code="500">Failed to update post</response>
  [HttpPatch]
  public async Task<IActionResult> UpdatePost() {
    return Ok("Post Updated");
  }

  /// <summary>
  /// Delete post
  /// </summary>
  /// <response code="200">Post deleted</response>
  /// <response code="404">Post not found</response>
  /// <response code="500">Failed to update post</response>
  [HttpDelete]
  public async Task<IActionResult> DeletePost()
  {
    return Ok("Post Deleted");
  }

}