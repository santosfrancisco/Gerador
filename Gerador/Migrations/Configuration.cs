namespace Gerador.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Gerador.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            if (!context.Roles.Any())
            {
                var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
                var role = new IdentityRole
                {
                    Name = "Administrador"
                };
                roleManager.Create(role);
            }

            if (!context.Empresas.Any())
            {
                var empresa = new Empresas
                {
                    Nome = "Anapro",
                    Responsavel = "Admin",
                    Responsavel_Email = "admin@anapro.com.br",
                    Responsavel_Telefone = "1150823353"
                };
                db.Empresas.Add(empresa);
                db.SaveChanges();
            }

            if (!context.Users.Any())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new ApplicationUserManager(userStore);

                var user = new ApplicationUser
                {
                    Email = "admin@anapro.com.br",
                    UserName = "admin@anapro.com.br",
                    Nome = "Admin",
                    IDEmpresa = 1
                };
                userManager.Create(user, "Admin@123");
                userManager.AddToRole(user.Id, "Administrador");
            }

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
