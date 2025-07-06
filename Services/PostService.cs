using Microsoft.EntityFrameworkCore;
using MyDotNetProject.Data;
using MyDotNetProject.Models;

namespace MyDotNetProject.Services
{
    public class PostService : IPostService
    {
        private readonly BetBuddysDbContext _context;

        public PostService(BetBuddysDbContext context)
        {
            _context = context;
        }

        public async Task<Post?> GetPostByIdAsync(string postId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .FirstOrDefaultAsync(p => p.Id == postId);
        }

        public async Task<IEnumerable<Post>> GetGroupPostsAsync(string groupId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Reactions)
                .Where(p => p.GroupId == groupId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<Post> CreatePostAsync(Post post)
        {
            if (string.IsNullOrEmpty(post.Id))
            {
                post.Id = Guid.NewGuid().ToString();
            }

            post.CreatedAt = DateTime.UtcNow;

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<Post> UpdatePostAsync(Post post)
        {
            var existingPost = await _context.Posts.FindAsync(post.Id);
            if (existingPost == null)
            {
                throw new InvalidOperationException("Post not found");
            }

            existingPost.Content = post.Content;
            existingPost.UpdatedAt = DateTime.UtcNow;
            existingPost.Edited = true;

            await _context.SaveChangesAsync();
            return existingPost;
        }

        public async Task<bool> DeletePostAsync(string postId)
        {
            var post = await _context.Posts.FindAsync(postId);
            if (post == null)
            {
                return false;
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Comment?> GetCommentByIdAsync(string commentId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task<IEnumerable<Comment>> GetPostCommentsAsync(string postId)
        {
            return await _context.Comments
                .Include(c => c.User)
                .Where(c => c.PostId == postId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            if (string.IsNullOrEmpty(comment.Id))
            {
                comment.Id = Guid.NewGuid().ToString();
            }

            comment.CreatedAt = DateTime.UtcNow;

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task<Comment> UpdateCommentAsync(Comment comment)
        {
            var existingComment = await _context.Comments.FindAsync(comment.Id);
            if (existingComment == null)
            {
                throw new InvalidOperationException("Comment not found");
            }

            existingComment.Content = comment.Content;
            existingComment.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingComment;
        }

        public async Task<bool> DeleteCommentAsync(string commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
            {
                return false;
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReactToPostAsync(string postId, string userId, string reactionType)
        {
            // Check if reaction already exists
            var existingReaction = await _context.PostReactions
                .FirstOrDefaultAsync(pr => pr.PostId == postId && pr.UserId == userId);

            if (existingReaction != null)
            {
                // Update existing reaction
                existingReaction.ReactionType = reactionType;
                existingReaction.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                // Create new reaction
                var reaction = new PostReaction
                {
                    Id = Guid.NewGuid().ToString(),
                    PostId = postId,
                    UserId = userId,
                    ReactionType = reactionType,
                    CreatedAt = DateTime.UtcNow
                };
                _context.PostReactions.Add(reaction);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveReactionAsync(string postId, string userId)
        {
            var reaction = await _context.PostReactions
                .FirstOrDefaultAsync(pr => pr.PostId == postId && pr.UserId == userId);

            if (reaction == null)
            {
                return false;
            }

            _context.PostReactions.Remove(reaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
