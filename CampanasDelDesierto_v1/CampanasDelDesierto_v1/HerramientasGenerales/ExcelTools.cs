using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    public class ExcelTools
    {
        public class ExcelParseError
        {
            public object registro { get; set; }

            [DisplayName("Error")]
            public string errorMsg { get; set; }

            public bool isError = false;
            public string errorDetails;

            public override string ToString()
            {
                return this.errorMsg;
            }

            public static ExcelTools.ExcelParseError errorFromException(Exception exc, ExcelRange excelRang)
            {
                ExcelTools.ExcelParseError error = new ExcelTools.ExcelParseError();
                error.isError = true;
                error.errorDetails = exc.Message;
                string errorMsg = "";
                if (exc is NullReferenceException)
                    errorMsg = "No contiene información.";
                else if (exc is FormatException)
                    errorMsg = "Contiene un dato que no es posible transformar.";
                else if (exc is Exception)
                    errorMsg = "Error inesperado, favor de ver los detalles.";

                error.errorMsg = String.Format("Renglon : <strong>{0}</strong>. Error: {1}", excelRang.Address, errorMsg);

                return error;
            }
        }
    }
}