using System.ComponentModel.DataAnnotations;
using Southpaw.Runtime.Serverside;
using System;
using System.Collections.Generic;

namespace SampleApplication_ServerSide
{
    [HasViewModel()]
    public class Blog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<Post> Posts { get; set; }
        public User Owner { get; set; }
    }

    [HasViewModel()]
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    [HasViewModel()]
    public class AdminUser : User
    {
        public int Permission { get; set; } 
    }

    [HasViewModel()]
    public class Post
    {
        public int Id { get; set; }
        public User Author { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 4)]
        public string Content { get; set; }
        public Blog Blog { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
