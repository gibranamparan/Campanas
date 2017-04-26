using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using CampanasDelDesierto_v1.HerramientasGenerales;

namespace CampanasDelDesierto_v1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {   
        
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<MovimientoFinanciero> MovimientosFinancieros { get; set; }
        public DbSet<PagoPorProducto> PagosPorProductos { get; set; }
        public DbSet<PrestamoYAbonoCapital> PrestamosYAbonosCapital { get; set; }
        public DbSet<Productor> Productores { get; set; }
        public DbSet<VentaACredito> VentasACreditos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<Activo> Activos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<PrestamoActivo> PrestamoActivos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Precision attribute for decimals
            DecimalPrecision.ConfigureModelBuilder(modelBuilder);
        }

        public System.Data.Entity.DbSet<CampanasDelDesierto_v1.Models.TemporadaDeCosecha> TemporadaDeCosechas { get; set; }
    }
}