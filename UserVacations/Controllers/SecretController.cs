using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UserVacations.Controllers
{
    [Authorize(Users="admin")]
    public class SecretController : Controller
    
    {
        public ContentResult Secret()
        {
            return Content("This is secret");
        }
        [AllowAnonymous]
        public ContentResult Overt()
        {
            return Content("This is not secret");
        }
    }
}