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
using Gerador.Controllers;
using Microsoft.AspNet.Identity;
using PagedList;
using Gerador.Filtros;

namespace GeradorDeProcessos.Controllers
{
    [FiltroPermissao]
    public class AnalisesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Analises
        public async Task<ActionResult> Index(int? page, string searchString, string currentFilter)
        {
            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;
            List<Analises> analises;
            if (User.IsInRole("Administrador,Analista"))
            {
                analises = await db.Analises.Include(a => a.Clientes).Include(a => a.Unidades).ToListAsync();
            }
            else if (User.IsInRole("Gestor"))
            {
                analises = await db.Analises.Where(a => a.Unidades.Empreendimentos.IDEmpresa == empresaUserLogado).ToListAsync();
            }
            else
            {
                analises = await db.Analises.Where(a => a.Unidades.Empreendimentos.IDEmpresa == empresaUserLogado && a.Clientes.User.Id == idUserLogado).ToListAsync();
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                page = 1;
                analises = analises.Where(e => e.Unidades.Numero.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                analises = analises.Where(u => u.Unidades.Numero.ToUpper().Contains(searchString.ToUpper())).ToList();
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            
            return View(analises.ToPagedList(pageNumber, pageSize));
        }

        // GET: Analises/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;
            if (id == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            Analises analises = await db.Analises.FindAsync(id);
            if (!User.IsInRole("Administrador") || analises.Clientes.User.IDEmpresa != empresaUserLogado)
            {
                return RedirectToAction("PermissaoNegada", "Usuarios", null);
            }
            if (analises == null)
            {
                return HttpNotFound();
            }
            return View(analises);
        }

        
        // GET: Analises/Create
        public async Task<ActionResult> Create(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home", null);
            };

            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;

            ViewBag.DataEntrega = db.Unidades.Find(id).Empreendimentos.DataEntrega;

            if (User.IsInRole("Administrador"))
            {

                ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Cliente");
                ViewBag.IDunidade = new SelectList(db.Unidades, "IDUnidade", "Numero", id);
            }
            else if (User.IsInRole("Gestor"))
            {

                ViewBag.IDCliente = new SelectList(db.Clientes.Where(c => c.User.IDEmpresa == empresaUserLogado), "IDCliente", "Nome");
                ViewBag.IDunidade = new SelectList(db.Unidades, "IDUnidade", "Numero", id);
            }
            else
            {
                ViewBag.IDCliente = new SelectList(db.Clientes.Where(c => c.IDUsuario == idUserLogado), "IDCliente", "Nome");
                ViewBag.IDunidade = new SelectList(db.Unidades, "IDUnidade", "Numero", id);
            }

            ViewBag.TipoAnalise = Analises.Tipos();
            return View();
        }

        // POST: Analises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDAnalise,DataEntrega,ValorFinanciamento,ValorTotal,SaldoDevedor,Observacao,TipoAnalise,IDCliente,IDUnidade")] Analises analises)
        {
            if (ModelState.IsValid)
            {
                db.Analises.Add(analises);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TipoAnalise = Analises.Tipos();
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            //ViewBag.DataEntrega = db.Unidades.Find(id).Empreendimentos.DataEntrega.ToShortDateString();
            return View(analises);
        }

        // GET: Analises/Edit/5
        [FiltroPermissao(Roles = "Administrador")]
        public async Task<ActionResult> Edit(int? id)
        {
            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;

            if (id == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            Analises analises = await db.Analises.FindAsync(id);
            //if (tipoUsuario == 1 && analises.Unidades.Empreendimentos.IDEmpresa != empresaUserLogado)
            //{
            //    return View("AcessoNegado");
            //}
            //else if (tipoUsuario == 2 && analises.Clientes.IDUsuario != idUserLogado)
            //{
            //    return View("AcessoNegado");
            //}

            ViewBag.DataEntrega = db.Unidades.Find(id).Empreendimentos.DataEntrega;
            if (analises == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoAnalise = Analises.Tipos();
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            return View(analises);
        }

        // POST: Analises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IDAnalise,DataEntrega,ValorFinanciamento,ValorTotal,SaldoDevedor,Observacao,TipoAnalise,IDCliente,IDUnidade")] Analises analises)
        {
            if (ModelState.IsValid)
            {
                db.Entry(analises).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.TipoAnalise = Analises.Tipos();
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            return View(analises);
        }

        // GET: Analises/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home", null);
            }
            Analises analises = await db.Analises.FindAsync(id);
            if (analises == null)
            {
                return HttpNotFound();
            }

            ViewBag.TipoAnalise = Analises.Tipos();
            return View(analises);
        }

        // POST: Analises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Analises analises = await db.Analises.FindAsync(id);
            db.Analises.Remove(analises);
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