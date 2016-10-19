using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gerador.Filtros
{
	public class FiltroPermissao : AuthorizeAttribute
	{
		public override void OnAuthorization(AuthorizationContext filterContext)
		{
			base.OnAuthorization(filterContext);
			
			if (filterContext.Result is HttpUnauthorizedResult)
			{
				if (HttpContext.Current.User.Identity.IsAuthenticated)
				{
					filterContext.HttpContext.Response.Redirect("/Home/AcessoNegado", true);
				} else
				{
					filterContext.HttpContext.Response.Redirect("/Account/Login", true);
				}
			}
		}
	}
}