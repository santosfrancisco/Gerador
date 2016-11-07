using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gerador.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PagedList;
using Gerador.Filtros;

namespace Gerador.Controllers
{
	[FiltroPermissao]
    public class ClientesController : BaseController
    {
		private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Clientes
        public async Task<ActionResult> Index(int? page, string searchString, string currentFilter)
        {
            if (!User.Identity.IsAuthenticated)
            {
                HttpContext.Response.Redirect("/Account/Login", true);
            }

			var idUserLogado = User.Identity.GetUserId();
			var userLogado = await UserManager.FindByIdAsync(idUserLogado);
			var empresaUserLogado = userLogado.IDEmpresa;

			List<Clientes> clientes;

			if (User.IsInRole("Administrador") || User.IsInRole("Analista"))
			{
				clientes = await db.Clientes.Include(c => c.User).ToListAsync();
			}
			else if (User.IsInRole("Gestor"))
			{
				clientes = await db.Clientes.Where(c => c.User.IDEmpresa == empresaUserLogado).ToListAsync();
			}
			else
			{

				clientes = await db.Clientes.Where(c => c.IDUsuario == idUserLogado).ToListAsync();
			}


			if (!String.IsNullOrEmpty(searchString))
			{
				page = 1;
				clientes = clientes.Where(e => e.Nome.ToUpper().Contains(searchString.ToUpper())).ToList();
			}
			else
			{
				searchString = currentFilter;
			}

			ViewBag.CurrentFilter = searchString;

			if (!String.IsNullOrEmpty(searchString))
			{
				clientes = clientes.Where(u => u.Nome.ToUpper().Contains(searchString.ToUpper())).ToList();
			}

			int pageSize = 5;
			int pageNumber = (page ?? 1);

			//var clientes = db.Clientes.Include(c => c.User);
			return View(clientes.ToPagedList(pageNumber, pageSize));
        }

        // GET: Clientes/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // GET: Clientes/Create
        public async Task<ActionResult> Create()
        {
			var idUserLogado = User.Identity.GetUserId();
			var userLogado = await UserManager.FindByIdAsync(idUserLogado);
			var empresaUserLogado = userLogado.IDEmpresa;

			if (User.IsInRole("Administrador") || User.IsInRole("Analista"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull", idUserLogado);
			}
			else if (User.IsInRole("Gestor"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.IDEmpresa == empresaUserLogado), "Id", "UsuarioFull", idUserLogado);
			}
			else
			{

				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.Id == idUserLogado), "Id", "UsuarioFull", idUserLogado);
			}

			// lista de tipos de pessoa
			ViewBag.TipoPessoa = Clientes.ListaTipo();
			// lista sexos
			ViewBag.Sexo = Clientes.ListaSexo();
			// lista de estados civis
			ViewBag.EstadoCivil = Clientes.ListaEstadoCivil();
			// lista de regimes de casamento
			ViewBag.RegimeCasamento = Clientes.ListaRegimeCasamento();

			return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDCliente,TipoPessoa,Nome,CpfCnpj,Sexo,Profissao,DataNascimento,Renda,EstadoCivil,RegimeCasamento,Conjuge_Cpf,Conjuge_Nome,IDUsuario")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.Clientes.Add(clientes);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

			var idUserLogado = User.Identity.GetUserId();
			var userLogado = await UserManager.FindByIdAsync(idUserLogado);
			var empresaUserLogado = userLogado.IDEmpresa;

			if (User.IsInRole("Administrador") || User.IsInRole("Analista"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else if (User.IsInRole("Gestor"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.IDEmpresa == empresaUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else
			{

				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.Id == idUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			// lista de tipos de pessoa
			ViewBag.TipoPessoa = Clientes.ListaTipo();
			// lista sexos
			ViewBag.Sexo = Clientes.ListaSexo();
			// lista de estados civis
			ViewBag.EstadoCivil = Clientes.ListaEstadoCivil();
			// lista de regimes de casamento
			ViewBag.RegimeCasamento = Clientes.ListaRegimeCasamento();
			return View(clientes);
        }

        // GET: Clientes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {

			var idUserLogado = User.Identity.GetUserId();
			var userLogado = await UserManager.FindByIdAsync(idUserLogado);
			var empresaUserLogado = userLogado.IDEmpresa;

			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Clientes clientes = await db.Clientes.FindAsync(id);

            if (clientes == null)
            {
                return HttpNotFound();
            }

			if (User.IsInRole("Administrador") || User.IsInRole("Analista"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else if (User.IsInRole("Gestor"))
			{
				if(empresaUserLogado != db.Clientes.Find(id).User.IDEmpresa)
				{
					return View("AcessoNegado");
				}
				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.IDEmpresa == empresaUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else
			{
				if (idUserLogado != db.Clientes.Find(id).IDUsuario)
				{
					return View("AcessoNegado");
				}
				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.Id == idUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			// lista de tipos de pessoa
			ViewBag.TipoPessoa = Clientes.ListaTipo();
			// lista sexos
			ViewBag.Sexo = Clientes.ListaSexo();
			// lista de estados civis
			ViewBag.EstadoCivil = Clientes.ListaEstadoCivil();
			// lista de regimes de casamento
			ViewBag.RegimeCasamento = Clientes.ListaRegimeCasamento();
			return View(clientes);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IDCliente,TipoPessoa,Nome,CpfCnpj,Sexo,Profissao,DataNascimento,Renda,EstadoCivil,RegimeCasamento,Conjuge_Cpf,Conjuge_Nome,IDUsuario")] Clientes clientes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(clientes).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
			var idUserLogado = User.Identity.GetUserId();
			var userLogado = await UserManager.FindByIdAsync(idUserLogado);
			var empresaUserLogado = userLogado.IDEmpresa;

			if (User.IsInRole("Administrador") || User.IsInRole("Analista"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else if (User.IsInRole("Gestor"))
			{
				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.IDEmpresa == empresaUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			else
			{

				ViewBag.IDUsuario = new SelectList(db.Users.Where(u => u.Id == idUserLogado), "Id", "UsuarioFull", clientes.IDUsuario);
			}
			// lista de tipos de pessoa
			ViewBag.TipoPessoa = Clientes.ListaTipo();
			// lista sexos
			ViewBag.Sexo = Clientes.ListaSexo();
			// lista de estados civis
			ViewBag.EstadoCivil = Clientes.ListaEstadoCivil();
			// lista de regimes de casamento
			ViewBag.RegimeCasamento = Clientes.ListaRegimeCasamento();
			return View(clientes);
        }

        // GET: Clientes/Delete/5
        [FiltroPermissao(Roles = "Administrador")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Clientes clientes = await db.Clientes.FindAsync(id);
            if (clientes == null)
            {
                return HttpNotFound();
            }
            return View(clientes);
        }

        // POST: Clientes/Delete/5
        [FiltroPermissao(Roles = "Administrador")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Clientes clientes = await db.Clientes.FindAsync(id);
            db.Clientes.Remove(clientes);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
