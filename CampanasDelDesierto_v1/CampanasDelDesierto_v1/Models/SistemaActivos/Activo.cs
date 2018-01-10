using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.HerramientasGenerales;
using OfficeOpenXml;
using System.Security.Cryptography;
using System.Text;

namespace CampanasDelDesierto_v1.Models
{
    public class Activo
    {
        [Key]
        public int idActivo { get; set; }

        [Display(Name = "Partida #")]
        public string partidaNumActivo { get; set; }

        [Display(Name = "Codigo del Activo")]
        public string codigoActivo { get; set; }

        [Display(Name = "Descipcion de Activo")]
        public string descripcionActivo { get; set; }

        [Display(Name = "Compañia Unidades")]
        public string unidadesActivo { get; set; }

        [Display(Name = "MMC Unidades")]
        public string mmcUnidades { get; set; }

        [Display(Name = "Observaciones")]
        public string observacionesActivo { get; set; }

        [Display(Name = "Contabilidad")]
        public string contabilidadActivo { get; set; }

        [Display(Name= "Esta prestado")]
        public bool isPrestado { get; set; }

        //Un activo tiene una collecion de prestamos
        public virtual ICollection<ProductoActivo> ProductosActivos { get; set; }

        ////Cada activo tiene a una categoria
        public int? CategoriaActivoID { get; set; }
        public virtual CategoriaDeActivo CategoriaDeActivo { get; set; }

        public int? departamentoID { get; set; }
        public virtual Departamento Departamento { get; set; }

        //Conexion a la base de datos.
        ApplicationDbContext db = new ApplicationDbContext();

        public enum ExcelColumns
        {
            PARTIDANUM = 0, CODIGO = 1,
            DESCRIPCION = 2, COMP_UNIDADES = 3,
            MMC = 4, OBSERVACIONES = 5,
            CONTABILIDAD = 6
        }

        //Generador de claves unicas
        public static string GeneradorNoSerie()
        {    
            string noSerie = CreateKey(5);         
            return noSerie;
        }
        public static String CreateKey(int numBytes)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[numBytes];

            rng.GetBytes(buff);
            return BytesToHexString(buff);
        }
        public static String BytesToHexString(byte[] bytes)
        {
            StringBuilder hexString = new StringBuilder(64);

            for (int counter = 0; counter < bytes.Length; counter++)
            {
                hexString.Append(String.Format("{0:X2}", bytes[counter]));
            }
            return hexString.ToString();
        }


        public Activo() { }
        /// <summary>
        /// Constructor que crea una copia selectiva de otra instancia del productor
        /// </summary>
        /// <param name="otro"></param>
        public Activo(Activo otro)
        {
            this.partidaNumActivo = otro.partidaNumActivo;
            this.codigoActivo = otro.codigoActivo;
            this.descripcionActivo = otro.descripcionActivo;
            this.unidadesActivo = otro.unidadesActivo;
            this.mmcUnidades = otro.mmcUnidades;
            this.observacionesActivo = otro.observacionesActivo;
            this.contabilidadActivo = otro.contabilidadActivo;

        }

        public Activo(ExcelRange rowActivo, ref ExcelTools.ExcelParseError error, int departamentoID)
        {
            error = new ExcelTools.ExcelParseError();
            try
            {   /*Se toman los datos del renglon (rowActivo) y por cada uno
                se le asignan los valores a los atributos del modelo*/
                this.partidaNumActivo = rowActivo.ElementAt((int)ExcelColumns.PARTIDANUM).Value.ToString().Trim();
                this.codigoActivo = rowActivo.ElementAt((int)ExcelColumns.CODIGO).Text.ToString();
                this.descripcionActivo = rowActivo.ElementAt((int)ExcelColumns.DESCRIPCION).Value.ToString();
                this.unidadesActivo = rowActivo.ElementAt((int)ExcelColumns.COMP_UNIDADES).Text.ToString();
                this.mmcUnidades = rowActivo.ElementAt((int)ExcelColumns.MMC).Value.ToString();
                this.observacionesActivo = rowActivo.ElementAt((int)ExcelColumns.OBSERVACIONES).Text.ToString();
                this.contabilidadActivo = rowActivo.ElementAt((int)ExcelColumns.CONTABILIDAD).Text.ToString();
                this.departamentoID = departamentoID;                    
            }
            catch (NullReferenceException exc) //Si llega a haber una excepccion nula, entra al catch y asigna dicho error
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                error.registro = new Activo(this);
            }
            catch (Exception exc)
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                error.registro = new Activo(this);
            }
        }

        //importacion de excel
        public static int importarActivos(HttpPostedFileBase xlsFile, ApplicationDbContext db,
          out List<ExcelTools.ExcelParseError> errores, out ExcelTools.ExcelParseError errorGeneral, int departamentoID)
        {
            int regsSaved = 0;
            //Lista para recoleccion de errores
            errores = new List<ExcelTools.ExcelParseError>();
            errorGeneral = new ExcelTools.ExcelParseError();
            //Se verifica la validez del archivo recibido
            if ((xlsFile != null) && (xlsFile.ContentLength > 0) && !string.IsNullOrEmpty(xlsFile.FileName))
            {
                //Se toman los datos del archivo
                string fileName = xlsFile.FileName;//nombre
                string fileContentType = xlsFile.ContentType;//tipo
                byte[] fileBytes = new byte[xlsFile.ContentLength];//composicion en bytes
                var data = xlsFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(xlsFile.ContentLength)); //datos a leer

                //Se crea el archivo Excel procesable
                var package = new ExcelPackage(xlsFile.InputStream);
                //var workSheet = currentSheet.First();//Se toma la 1ra hoja de excel
                var workSheet = package.Workbook.Worksheets["INVENTARIO"];//Se toma la 1ra hoja de excel
                var noOfCol = workSheet.Dimension.End.Column;//Se determina el ancho de la tabla en no. columnas
                var noOfRow = workSheet.Dimension.End.Row;//El alto de la tabla en numero de renglores

                ExcelTools.ExcelParseError error = new ExcelTools.ExcelParseError();
                //Se recorre cada renglon de la hoja
                for (int rowIterator = 16; rowIterator <= noOfRow; rowIterator++)
                {
                    //Se toma renglon
                    var rowActivo = workSheet.Cells[rowIterator, 2, rowIterator, noOfCol];
                    //Se convierte renglon en registro para DB con registro de errores
                    var activoReg = new Activo(rowActivo, ref error, departamentoID);

                    if (!error.isError)
                    {                        
                        var activoDB = db.Activos.ToList()
                            .FirstOrDefault(rp => rp.partidaNumActivo == activoReg.partidaNumActivo && rp.descripcionActivo == activoReg.descripcionActivo);
                        //Si el registro no existe, se agrega
                        if (activoDB == null)
                        {
                            db.Activos.Add(activoReg);
                            db.SaveChanges();
                            int cantidad = int.Parse(rowActivo.ElementAt((int)ExcelColumns.MMC).Value.ToString());
                            ProductoActivo productoActivo = new ProductoActivo();
                            List<ProductoActivo> ProductoActivoList = new List<ProductoActivo>();
                            try
                            {                                
                                for (int i = 0; i < cantidad; i++)
                                {
                                    productoActivo.descripcionActivo = rowActivo.ElementAt((int)ExcelColumns.DESCRIPCION).Value.ToString();
                                    productoActivo.observacionesActivo = rowActivo.ElementAt((int)ExcelColumns.OBSERVACIONES).Text.ToString();
                                    productoActivo.noSerie = GeneradorNoSerie();
                                    productoActivo.Activo = activoReg;
                                    productoActivo.idActivo = activoReg.idActivo;
                                    //Si el registro no existe, se agrega 
                                    db.Entry(productoActivo).State = System.Data.Entity.EntityState.Added;                                                                                                                                   
                                    regsSaved = db.SaveChanges();                                 
                                    //Se recargan las entidades de productos asociadass a cada compra dentro del prestamo
                                    activoReg.ProductosActivos.ToList().ForEach(com => db.Entry(com).Reference(p => p.Activo).Load());
                                }                                
                               
                            }
                            catch (NullReferenceException exc) //Si llega a haber una excepccion nula, entra al catch y asigna dicho error
                            {
                                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                                error.registro = new Activo();
                            }
                            catch (Exception exc)
                            {
                                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                                error.registro = new Activo();
                            }
                        }
                        else
                        {
                            //Si ya existe, se identifica con el mismo ID y se marca como modificado                            
                            activoReg.idActivo = activoDB.idActivo;                            
                            db.Entry(activoDB).State = System.Data.Entity.EntityState.Detached;                          
                            db.Entry(activoReg).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                            ProductoActivo productoActivo = new ProductoActivo();
                            List<ProductoActivo> ProductoActivoList = new List<ProductoActivo>();
                            try
                            {                                
                                int cantidad = int.Parse(rowActivo.ElementAt((int)ExcelColumns.MMC).Value.ToString());
                                int cantidadDB = db.ProductosActivos.Where(proac=> proac.idActivo == activoReg.idActivo).Count();
                                if (cantidad > cantidadDB)
                                {
                                    int dif = cantidad - cantidadDB;
                                    for (int i = 0; i < dif; i++)
                                    {
                                        productoActivo.descripcionActivo = rowActivo.ElementAt((int)ExcelColumns.DESCRIPCION).Value.ToString();
                                        productoActivo.observacionesActivo = rowActivo.ElementAt((int)ExcelColumns.OBSERVACIONES).Text.ToString();
                                        productoActivo.noSerie = GeneradorNoSerie();
                                        productoActivo.Activo = activoReg;
                                        productoActivo.idActivo = activoReg.idActivo;
                                        //Si el registro no existe, se agrega 
                                        db.Entry(productoActivo).State = System.Data.Entity.EntityState.Added;
                                        regsSaved = db.SaveChanges();
                                        //Se recargan las entidades de productos asociadass a cada compra dentro del prestamo
                                        activoReg.ProductosActivos.ToList().ForEach(com => db.Entry(com).Reference(p => p.Activo).Load());
                                    }
                                }                                                            
                            }
                            catch (NullReferenceException exc) //Si llega a haber una excepccion nula, entra al catch y asigna dicho error
                            {
                                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                                error.registro = new Activo();
                            }
                            catch (Exception exc)
                            {
                                error = ExcelTools.ExcelParseError.errorFromException(exc, rowActivo);
                                error.registro = new Activo();
                            }
                        }
                    }
                    else
                        errores.Add(error);
                }
                              
                
            }
            
            return regsSaved;
        }      
    }
}