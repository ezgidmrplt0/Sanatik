using System;
using System.Collections.Generic;

namespace Sanatik.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public int PostCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }

        public List<Photo> Photos { get; set; }
        public List<UserFollowViewModel> Followers { get; set; }
        public List<UserFollowViewModel> Following { get; set; }
        public bool IsCurrentUser { get; set; }
        public bool IsFollowing { get; set; }
    }

    public class UserFollowViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public bool IsFollowing { get; set; }
    }

}
