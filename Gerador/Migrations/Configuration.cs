namespace Gerador.Migrations
{
	using IdentitySample.Models;
	using Microsoft.AspNet.Identity;
	using Microsoft.AspNet.Identity.EntityFramework;
	using System;
	using System.Data.Entity;
	using System.Data.Entity.Migrations;
	using System.Linq;

	internal sealed class Configuration : DbMigrationsConfiguration<IdentitySample.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(IdentitySample.Models.ApplicationDbContext context)
        {
			// Cria o user admin. A EMPRESA ainda tem que ser criada manualmente 
			// antes de executar o update-database para que o admin seja criado
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
					IDEmpresa = 3
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
