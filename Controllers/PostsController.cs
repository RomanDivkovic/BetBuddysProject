using Microsoft.AspNetCore.Mvc;
using MyDotNetProject.Models;
using MyDotNetProject.Services;

namespace MyDotNetProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(string id)
        {
            var post = await _postService.GetPostByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpGet("group/{groupId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetGroupPosts(string groupId)
        {
            var posts = await _postService.GetGroupPostsAsync(groupId);
            return Ok(posts);
        }

        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost([FromBody] Post post)
        {
            try
            {
                var createdPost = await _postService.CreatePostAsync(post);
                return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Post>> UpdatePost(string id, [FromBody] Post post)
        {
            if (id != post.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedPost = await _postService.UpdatePostAsync(post);
                return Ok(updatedPost);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePost(string id)
        {
            var result = await _postService.DeletePostAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("{postId}/comments")]
        public async Task<ActionResult<IEnumerable<Comment>>> GetPostComments(string postId)
        {
            var comments = await _postService.GetPostCommentsAsync(postId);
            return Ok(comments);
        }

        [HttpPost("comments")]
        public async Task<ActionResult<Comment>> CreateComment([FromBody] Comment comment)
        {
            try
            {
                var createdComment = await _postService.CreateCommentAsync(comment);
                return CreatedAtAction(nameof(GetComment), new { commentId = createdComment.Id }, createdComment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("comments/{commentId}")]
        public async Task<ActionResult<Comment>> GetComment(string commentId)
        {
            var comment = await _postService.GetCommentByIdAsync(commentId);
            if (comment == null)
            {
                return NotFound();
            }
            return Ok(comment);
        }

        [HttpPut("comments/{commentId}")]
        public async Task<ActionResult<Comment>> UpdateComment(string commentId, [FromBody] Comment comment)
        {
            if (commentId != comment.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updatedComment = await _postService.UpdateCommentAsync(comment);
                return Ok(updatedComment);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<ActionResult> DeleteComment(string commentId)
        {
            var result = await _postService.DeleteCommentAsync(commentId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("{postId}/react")]
        public async Task<ActionResult> ReactToPost(string postId, [FromBody] ReactionRequest request)
        {
            try
            {
                var result = await _postService.ReactToPostAsync(postId, request.UserId, request.ReactionType);
                if (!result)
                {
                    return BadRequest("Failed to react to post");
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{postId}/react/{userId}")]
        public async Task<ActionResult> RemoveReaction(string postId, string userId)
        {
            var result = await _postService.RemoveReactionAsync(postId, userId);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }

    public class ReactionRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string ReactionType { get; set; } = string.Empty;
    }
}
