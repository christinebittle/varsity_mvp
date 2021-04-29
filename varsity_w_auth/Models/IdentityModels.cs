using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;


namespace varsity_w_auth.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        //The messages of support sent by this logged in user.
        public ICollection<Support> SupportMessages { get; set; }
        public string NickName { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    //useful for transmitting data about the user via API
    public class ApplicationUserDto
    {
        public string id { get; set; }
        public string NickName { get; set; }
    }

    public class VarsityDataContext : IdentityDbContext<ApplicationUser>
    {
        /* Local Connection String*/
        /*
        public VarsityDataContext()
            : base("name=VarsityDataContextwAuth", throwIfV1Schema: false)
        {
        }*/
        

        /* AWS Connection*/
        
        public VarsityDataContext()
            : base(AWSConnector.GetRDSConnectionString())
        {
        }
        


        public static VarsityDataContext Create()
        {
            return new VarsityDataContext();
        }


        //Instruction to set the models as tables in our database.
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }

        public DbSet<Sport> Sports { get; set; }

        public DbSet<Support> Supports { get; set; }


        //To Run the database, use code-first migrations
        //https://www.entityframeworktutorial.net/code-first/code-based-migration-in-code-first.aspx

        //Tools > NuGet Package Manager > Package Manager Console
        //enable-migrations (only once)
        //add-migration {migration name}
        //update-database

        //To View the Database Changes sequentially, go to Project/Migrations folder

        //To View Database itself, go to View > SQL Server Object Explorer
        // (localdb)\MSSQLLocalDB
        // Right Click {ProjectName}.Models.DataContext > Tables
        // Can Right Click Tables to View Data
        // Can Right Click Database to Query

        // You will have to add in some example data to the database locally

    }
}