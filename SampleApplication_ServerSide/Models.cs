using Southpaw.Runtime.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class Post
    {
        public int Id { get; set; }
        public User Author { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Blog Blog { get; set; }
        public DateTime PostedAt { get; set; }
    }
}
