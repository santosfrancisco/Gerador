using Gerador.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Gerador.Controllers;
using PagedList;
using Gerador.Filtros;

namespace Gerador.Controllers
{
    //[Authorize]
    [FiltroPermissao(Roles = "Administrador, Gestor")]
    public class UsersAdminController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public UsersAdminController()
        {
        }

        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager {
            get {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager {
            get {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set {
                _roleManager = value;
            }
        }

        // GET: Empresa usuario logado
        public async Task<int> GetEmpresa(string id)
        {
            var usuario = await UserManager.FindByIdAsync(id);

            return usuario.IDEmpresa;

        }
        public void GetPerfil(string id)
        {

            var roles = UserManager.GetRoles(id);


        }
        //
        // GET: /Users/
        public async Task<ActionResult> Index(int? page, string searchString, string currentFilter)
        {
            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;

            List<ApplicationUser> usuarios = await UserManager.Users.ToListAsync();
            if (!User.IsInRole("Administrador"))
            {
                usuarios = usuarios.Where(e => e.IDEmpresa == empresaUserLogado).ToList();
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                page = 1;
                usuarios = usuarios.Where(e => e.Nome.ToUpper().Contains(searchString.ToUpper()) || e.Email.ToUpper().Contains(searchString.ToUpper())).ToList();
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            //if (!String.IsNullOrEmpty(searchString))
            //{
            //	usuarios = usuarios.Where(e => e.Nome.ToUpper().Contains(searchString.ToUpper()) || e.Email == searchString.ToUpper()).ToList();
            //}

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(usuarios.ToPagedList(pageNumber, pageSize));
            //return View(await UserManager.Users.ToListAsync());
        }

        //
        // GET: /Users/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);
            ViewBag.Empresa = user.Empresas.Nome;

            return View(user);
        }

        //
        // GET: /Users/Create
        [FiltroPermissao(Roles = "Administrador, Analista, Gestor")]
        public async Task<ActionResult> Create()
        {
            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                if (selectedRoles != null)
                {
                    var user = new ApplicationUser
                    {
                        Nome = userViewModel.Nome,
                        UserName = userViewModel.Email,
                        Email = userViewModel.Email,
                        IDEmpresa = userViewModel.IDEmpresa
                    };

                    var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                    //Add User to the selected Roles 
                    if (adminresult.Succeeded)
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);
                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
                            return View();
                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", adminresult.Errors.First());
                        ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                        ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
                        return View();

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Selecione um tipo de usuário.");
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
                    ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
                    return View();
                }

                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
            return View();
        }

        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(string id)
        {
            var idUserLogado = User.Identity.GetUserId();
            var userLogado = await UserManager.FindByIdAsync(idUserLogado);
            var empresaUserLogado = userLogado.IDEmpresa;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (user.IDEmpresa != empresaUserLogado && !User.IsInRole("Administrador"))
            {
                ViewBag.Msg = "Você não pode editar usuários de outra empresa.";
                return View("AcessoNegado");
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            if (!User.IsInRole("Administrador"))
            {
                ViewBag.IDEmpresa = new SelectList(db.Empresas.Where(u => u.IDEmpresa == empresaUserLogado), "IDEmpresa", "Nome", user.IDEmpresa);
            }
            else
            {
                ViewBag.IDEmpresa = new SelectList(db.Empresas, "IDEmpresa", "Nome");
            }

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Nome = user.Nome,
                Email = user.Email,
                IDEmpresa = user.IDEmpresa,
                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Nome,UserName,Email,IDEmpresa,Id")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.Nome = editUser.Nome;
                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                user.IDEmpresa = editUser.IDEmpresa;

                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
