using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using CampanasDelDesierto_v1.HerramientasGenerales;
using Proveedores = CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital.Proveedores;
using Conceptos = CampanasDelDesierto_v1.Models.PrestamoYAbonoCapital.Conceptos;
using System.ComponentModel;
using CampanasDelDesierto_v1.Models.SistemaProductores;

namespace CampanasDelDesierto_v1.Models
{

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
       
        public ApplicationUser() { }
        public ApplicationUser(RegisterViewModel model)
        {
            if (!System.String.IsNullOrEmpty(model.userID))
                this.Id = model.userID;
            this.UserName = model.Email;
            this.Email = model.Email;            
            this.nombre = model.nombre;
            this.apellidoPaterno = model.apellidoPaterno;
            this.apellidoMaterno = model.apellidoMaterno;         
            this.PasswordHash = model.hash;
            this.SecurityStamp = model.stamp;           
        }
        public ApplicationUser(RegisterViewModel model, ApplicationDbContext db) : this(model)
        {
            var userFromDB = db.Users.Find(model.userID);
            this.EmailConfirmed = userFromDB.EmailConfirmed;
            this.PhoneNumberConfirmed = this.PhoneNumberConfirmed;
            this.TwoFactorEnabled = this.TwoFactorEnabled;
            this.LockoutEnabled = this.LockoutEnabled;
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
        
        [DisplayName("Nombre")]
        public string nombre { get; set; }
        
        [DisplayName("Apellido Paterno")]
        public string apellidoPaterno { get; set; }
 
        [DisplayName("Apellido Materno")]
        public string apellidoMaterno { get; set; }
      
        [DisplayName("Nombre Completo")]
        public string nombreCompleto
        {
            get { return this.nombre + " " + this.apellidoPaterno + " " + this.apellidoMaterno; }
        }

        public static class RoleNames
        {
            public const string DEPARTAMENTO = "Departamento";
            public const string ADMIN = "Admin";
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //Sistema de productores
        public DbSet<MovimientoFinanciero> MovimientosFinancieros { get; set; }
        public DbSet<PagoPorProducto> PagosPorProductos { get; set; }
        public DbSet<PrestamoYAbonoCapital> PrestamosYAbonosCapital { get; set; }
        public DbSet<Productor> Productores { get; set; }
        public DbSet<VentaACredito> VentasACreditos { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedores> Proveedores { get; set; }
        public DbSet<Conceptos> Conceptos { get; set; }
        public DbSet<LiquidacionSemanal> LiquidacionesSemanales { get; set; }
        public DbSet<RecepcionDeProducto> RecepcionesDeProducto { get; set; }
        public DbSet<Retencion> Retenciones { get; set; }
        public DbSet<TemporadaDeCosecha> TemporadaDeCosechas { get; set; }
        public DbSet<PrestamoYAbonoCapital.Prestamo_Abono> Prestamo_Abono { get; set; }
        public DbSet<AdeudoInicial> AdeudosIniciales { get; set; }

        //Sistema de activos
        public DbSet<Activo> Activos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        public DbSet<PrestamoActivo> PrestamoActivos { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<CompraProducto> ComprasProductos { get; set; }
        public DbSet<AdminDepartamento> AdminsDepartamentos { get; set; }


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

       
    }
}