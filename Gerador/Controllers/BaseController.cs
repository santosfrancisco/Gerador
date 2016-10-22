using Gerador.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gerador.Controllers
{
    public class BaseController : Controller
    {
		public BaseController()
		{
		}

		public BaseController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
		{
			UserManager = userManager;
			RoleManager = roleManager;
		}

		private ApplicationUserManager _userManager;
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		private ApplicationRoleManager _roleManager;
		public ApplicationRoleManager RoleManager
		{
			get
			{
				return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
			}
			private set
			{
				_roleManager = value;
			}
		}

    }
}