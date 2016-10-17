using System.Web.Mvc;
using PagedList;
using System.Collections.Generic;
using Gerador.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using IdentitySample.Models;
using System.Data.Entity;
using Gerador.Controllers;

namespace IdentitySample.Controllers
{
    public class HomeController : BaseController
    {
		private ApplicationDbContext db = new ApplicationDbContext();
		public async Task<ActionResult> Index(int? page, string searchString, string currentFilter)
        {
			List<Empreendimentos> empreendimentos = await db.Empreendimentos.ToListAsync();

			//var tipoUsuario = RepositorioUsuarios.VerificaTipoUsuario();
			//var empresa = RepositorioUsuarios.VerificaEmpresaUsuario();

			//if (tipoUsuario == 0)
			//{
			//	empreendimentos = await db.Empreendimentos.ToListAsync();
			//}
			//else if (tipoUsuario == 1)
			//{
			//	empreendimentos = await db.Empreendimentos.Where(e => e.IDEmpresa == empresa).ToListAsync();
			//}
			//else
			//{
			//	empreendimentos = await db.Empreendimentos.Where(e => e.IDEmpresa == empresa).ToListAsync();
			//}

			if (!String.IsNullOrEmpty(searchString))
			{
				page = 1;
				empreendimentos = empreendimentos.Where(e => e.Nome.ToUpper().Contains(searchString.ToUpper())).ToList();
			}
			else
			{
				searchString = currentFilter;
			}

			ViewBag.CurrentFilter = searchString;

			if (!String.IsNullOrEmpty(searchString))
			{
				empreendimentos = empreendimentos.Where(u => u.Nome.ToUpper().Contains(searchString.ToUpper())).ToList();
			}

			int pageSize = 5;
			int pageNumber = (page ?? 1);
			return View(empreendimentos.ToPagedList(pageNumber, pageSize));
		}

		public ActionResult Administracao()
		{
			return View();
		}

        [Authorize]
        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
