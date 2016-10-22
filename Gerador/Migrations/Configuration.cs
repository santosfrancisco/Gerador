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
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
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

            if (!context.Users.Any())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new ApplicationUserManager(userStore);
                var user = new ApplicationUser
                {
                    Email = "admin@admin.com",
                    UserName = "admin@admin.com",
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
