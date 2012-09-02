using Southpaw.Runtime.Serverside;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SampleApplication_ServerSide
{
    public class BlogController : Controller
    {
        [ClientService(typeof(Blog))]
        public ActionResult Show(int id)
        {
            return null;
        }

        [ClientService(typeof(List<Blog>))]
        public ActionResult Index()
        {
            return null;
        }
    }

    public class PostController : Controller
    {
        [ClientService(typeof(Post))]
        public ActionResult Show(int id)
        {
            return null;
        }
    }
}
