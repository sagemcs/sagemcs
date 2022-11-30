
//PORTAL DE PROVEDORES T|SYS|
//10 DE ENERO, 2019
//DESARROLLADO POR MULTICONSULTING S.A. DE C.V.

//REFERENCIAS UTILIZADAS
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(WebSite1.Startup))]
namespace WebSite1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }

        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager2 = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("Manager"))
            {

                // first we create Admin rool   
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Manager";
                roleManager.Create(role);

                //var user = new ApplicationUser();
                //user.UserName = "tsys";
                ////UserManager2.FindByName("tsys").Id;
                ////Here we create a Admin super user who will maintain the website

                //var result1 = UserManager2.AddToRole(UserManager2.FindByName("tsys").Id, "Manager");
            }


            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Admin"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Admin";
                roleManager.Create(role);

            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Validador"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Validador";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Empleado"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Empleado";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Gerente"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Gerente";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Tesoreria"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Tesoreria";
                roleManager.Create(role);
            }

            // creating Creating Manager role    
            if (!roleManager.RoleExists("T|SYS| - Finanzas"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Finanzas";
                roleManager.Create(role);
            }



            //var user = new ApplicationUser();
            //user.UserName = "tsys";
            //UserManager2.FindByName("tsys").Id;
            //Here we create a Admin super user who will maintain the website

            //var result1 = UserManager2.AddToRole(UserManager2.FindByName("tsys").Id, "Manager");

            // creating Creating Manager role    
            if (!roleManager.RoleExists("Proveedor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Proveedor";
                roleManager.Create(role);

            }

            if (!roleManager.RoleExists("Pre-Proveedor"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Pre-Proveedor";
                roleManager.Create(role);

            }

            // creating Creating Employee role    
            if (!roleManager.RoleExists("Employee"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Employee";
                roleManager.Create(role);

            }

            //Creating Gerencia de Capital Humano role
            if (!roleManager.RoleExists("T|SYS| - Gerencia de Capital Humano"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "T|SYS| - Gerencia de Capital Humano";
                roleManager.Create(role);
            }
        }
    }
}
