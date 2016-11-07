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
using PagedList;
using Microsoft.AspNet.Identity;

namespace Gerador.Controllers
{
    public class AnalisesController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Analises1
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

        // GET: Analises1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analises analises = await db.Analises.FindAsync(id);
            if (analises == null)
            {
                return HttpNotFound();
            }
            return View(analises);
        }

        // GET: Analises1/Create
        public ActionResult Create(int id)
        {
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome");
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero");
            ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull");
            ViewBag.TipoAnalise = Analises.Tipos();
            ViewBag.DataEntrega = db.Unidades.Find(id).Empreendimentos.DataEntrega;
            return View();
        }

        // POST: Analises1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "IDAnalise,DataEntrega,ValorFinanciamento,ValorTotal,SaldoDevedor,Observacao,TipoAnalise,IDCliente,IDUnidade,IDUsuario")] Analises analises)
        {
            if (ModelState.IsValid)
            {
                db.Analises.Add(analises);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            ViewBag.IDUsuario = new SelectList(db.Users, "Id", "UsuarioFull", analises.IDUsuario);
            ViewBag.TipoAnalise = Analises.Tipos();
            return View(analises);
        }

        // GET: Analises1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analises analises = await db.Analises.FindAsync(id);
            if (analises == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            ViewBag.IDUsuario = new SelectList(db.Users, "Id", "Nome", analises.IDUsuario);
            ViewBag.TipoAnalise = Analises.Tipos();
            return View(analises);
        }

        // POST: Analises1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "IDAnalise,DataEntrega,ValorFinanciamento,ValorTotal,SaldoDevedor,Observacao,TipoAnalise,IDCliente,IDUnidade,IDUsuario")] Analises analises)
        {
            if (ModelState.IsValid)
            {
                db.Entry(analises).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.IDCliente = new SelectList(db.Clientes, "IDCliente", "Nome", analises.IDCliente);
            ViewBag.IDUnidade = new SelectList(db.Unidades, "IDUnidade", "Numero", analises.IDUnidade);
            ViewBag.IDUsuario = new SelectList(db.Users, "Id", "Nome", analises.IDUsuario);
            ViewBag.TipoAnalise = Analises.Tipos();
            return View(analises);
        }

        // GET: Analises1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Analises analises = await db.Analises.FindAsync(id);
            if (analises == null)
            {
                return HttpNotFound();
            }
            return View(analises);
        }

        // POST: Analises1/Delete/5
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
