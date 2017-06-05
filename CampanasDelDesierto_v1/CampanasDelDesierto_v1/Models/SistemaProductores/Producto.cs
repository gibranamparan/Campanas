using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CampanasDelDesierto_v1.HerramientasGenerales;
using OfficeOpenXml;

namespace CampanasDelDesierto_v1.Models
{
    public class Producto
    {
        public enum ExcelColumns
        {
            NOMBRE = 0, DESCRIPCION = 1,
            PRECIO = 2, UM = 3
        }

        /// <summary>
        /// Se construye un producto en base a informacion proveniente de Excel.
        /// </summary>
        /// <param name="rowProducto"></param>
        /// <param name="error"></param>
        public Producto(ExcelRange rowProducto, ref ExcelTools.ExcelParseError error)
        {
            error = new ExcelTools.ExcelParseError();
            try
            {
                this.nombreProducto = rowProducto.ElementAt((int)ExcelColumns.NOMBRE).Value.ToString().Trim();
                this.descripcion = rowProducto.ElementAt((int)ExcelColumns.DESCRIPCION).Value.ToString();
                this.costo = decimal.Parse(rowProducto.ElementAt((int)ExcelColumns.PRECIO).Value.ToString());
                string unidadMedida = rowProducto.ElementAt((int)ExcelColumns.UM).Value.ToString();

                this.UnidadMedida = Producto.UnidadDeMedida.GetByName(unidadMedida);
            }
            catch (NullReferenceException exc)
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowProducto);
                error.registro = new Producto(this);
            }
            catch (Exception exc)
            {
                error = ExcelTools.ExcelParseError.errorFromException(exc, rowProducto);
                error.registro = new Producto(this);
            }
            if (String.IsNullOrEmpty(this.nombreProducto))
            {
                error = ExcelTools.ExcelParseError.errorFromException(new NullReferenceException(), rowProducto);
                error.registro = new Producto(this);
            }
        }

        public Producto(Producto producto)
        {
            this.nombreProducto = producto.nombreProducto;
            this.descripcion = producto.descripcion;
            this.costo = producto.costo;
            this.unidadMedida = this.unidadMedida;
        }

        public Producto(){}

        [Key]
        public int idProducto { get; set; }

        [Display(Name = "Nombre")]
        public string nombreProducto { get; set; }

        [DisplayFormat(DataFormatString = "{0:C}",
        ApplyFormatInEditMode = true)]
        [Display(Name = "Costo")]
        public decimal costo { get; set; }
        
        //Tipo de material que es.
        [Display(Name = "Descripción")]
        [DataType(DataType.MultilineText)]
        public string descripcion { get; set; }

        private string unidadMedida { get; set; }

        [Display(Name = "um.")]
        public UnidadDeMedida UnidadMedida
        {
            get {
                return UnidadDeMedida.GetByName(this.unidadMedida);
            }
            set
            {
                this.unidadMedida = value.nombre;
            }
        }

        [Display(Name = "Es árbol")]
        public bool isArbolAceituna { get; set;}

        internal static int importarProductores(HttpPostedFileBase xlsFile,
            ApplicationDbContext db, out List<ExcelTools.ExcelParseError> errores,
            out ExcelTools.ExcelParseError errorGeneral)
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
                var workSheet = package.Workbook.Worksheets[1];//Se toma la 1ra hoja de excel
                var noOfCol = workSheet.Dimension.End.Column;//Se determina el ancho de la tabla en no. columnas
                var noOfRow = workSheet.Dimension.End.Row;//El alto de la tabla en numero de renglores

                ExcelTools.ExcelParseError error = new ExcelTools.ExcelParseError();
                //Se recorre cada renglon de la hoja
                for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                {
                    //Se toma renglon
                    var rowProducto = workSheet.Cells[rowIterator, 1, rowIterator, noOfCol];
                    //Se convierte renglon en registro para DB con registro de errores
                    var productoReg = new Producto(rowProducto, ref error);

                    if (!error.isError)
                    {
                        var productoDB = db.Productos.ToList()
                            .FirstOrDefault(rp => rp.nombreProducto == productoReg.nombreProducto);

                        //Si el registro no se encontró, se agrega
                        if (productoDB == null)
                            db.Productos.Add(productoReg);
                    }
                    else
                        errores.Add(error);
                }

                regsSaved = db.SaveChanges();
            }

            return regsSaved;
        }

        private static UnidadDeMedida[] getUnidadesArray()
        {
            UnidadDeMedida[] ums = new UnidadDeMedida[]{
                UnidadDeMedida.NombresDeUM.KILO,
                UnidadDeMedida.NombresDeUM.GRAMO,
                UnidadDeMedida.NombresDeUM.GALON,
                UnidadDeMedida.NombresDeUM.PIEZA,
                UnidadDeMedida.NombresDeUM.OTRO,
            };

            return ums;
        }

        public static SelectList getUnidadesDeMedida()
        {
            return new SelectList(getUnidadesArray(), "nombre", "nombre");
        }

        public static SelectList getUnidadesDeMedida(String sel)
        {
            var sl = new SelectList(getUnidadesArray(), "nombre", "nombre", sel);
            sl.FirstOrDefault(op => op.Value == sel).Selected = true;
            return sl;
        }

        public override string ToString()
        {
            return String.Format("{0}, {1}, UM: {2}", this.nombreProducto, this.costo, this.UnidadMedida);
        }

        public class UnidadDeMedida
        {
            public string nombre { get; set; }
            public string abreviacion { get; set; }

            public static UnidadDeMedida GetByName(string UM)
            {
                if (!String.IsNullOrEmpty(UM))
                    UM = UM.Replace("(s)", "").Trim();

                if (UM == NombresDeUM.KILO.nombre)
                    return NombresDeUM.KILO;
                if (UM == NombresDeUM.PIEZA.nombre)
                    return NombresDeUM.PIEZA;
                if (UM == NombresDeUM.GRAMO.nombre)
                    return NombresDeUM.GRAMO;
                if (UM == NombresDeUM.GALON.nombre)
                    return NombresDeUM.GALON;

                return NombresDeUM.OTRO;
            }

            public override string ToString()
            {
                return String.Format("{0}", this.abreviacion);
            }

            public class NombresDeUM
            {
                public static UnidadDeMedida KILO = new UnidadDeMedida
                {
                    nombre = "Kilo",
                    abreviacion = "kg."
                };
                public static UnidadDeMedida PIEZA = new UnidadDeMedida
                {
                    nombre = "Pieza",
                    abreviacion = "pza."
                };
                public static UnidadDeMedida GRAMO = new UnidadDeMedida
                {
                    nombre = "Gramo",
                    abreviacion = "gr."
                };
                public static UnidadDeMedida GALON = new UnidadDeMedida
                {
                    nombre = "Galón",
                    abreviacion = "gal."
                };
                public static UnidadDeMedida OTRO = new UnidadDeMedida
                {
                    nombre = "n/a",
                    abreviacion = "n/a"
                };
            }
        }
}