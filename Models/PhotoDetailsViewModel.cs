namespace Sanatik.Models;

public class PhotoDetailsViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ImagePath { get; set; }
    public DateTime UploadDate { get; set; }
    public string UserName { get; set; }
    
    public int LikeCount { get; set; }
    public int FavoriteCount { get; set; }
    
    public bool IsLiked { get; set; }

    public bool IsFavorited { get; set; }
    public bool IsFollowing { get; set; } // New property
    public string PhotoUserId { get; set; } // New, to know who to follow
    
    public List<CommentViewModel> Comments { get; set; }
}

public class CommentViewModel
{
    public string UserName { get; set; }
    public string UserId { get; set; } // Add User Id for navigation
    public string Content { get; set; }
    public string TimeAgo { get; set; }
}
