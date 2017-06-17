using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;
using System.Xml.Linq;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    public class BaxicoWebService
    {
        public string url { get; set; }
        Banxico.DgieWSPortClient ws;

        public BaxicoWebService()
        {
            ws = new Banxico.DgieWSPortClient();
        }

        public decimal getCambioDolar(ref String errorMsg)
        {
            XElement node = null;
            try
            {
                string strXml = ws.tiposDeCambioBanxico();
                XDocument xdoc = XDocument.Parse(@strXml);
                //FIX Change IDSERIE=SF43718
                //Dentro del cuerpo del XML recibido, se busca el nodo que contiene el tipo de cambio
                node = xdoc.Descendants().Elements()
                    .SingleOrDefault(nod => nod.HasAttributes
                        && nod.Attributes()
                        .SingleOrDefault(attr => attr.Name == "IDSERIE" && attr.Value == "SF60653") != null
                    );

                //Se busca el nodo hijo que contiene el atributo con el dato buscado
                node = node.Elements()
                    .SingleOrDefault(nod => nod.Attributes().SingleOrDefault(attr => attr.Name == "OBS_VALUE") != null);
            }
            catch (Exception exc ){
                errorMsg = exc.Message;
            }

            //Se parsea el dato encontrado
            decimal res = 0;
            Boolean parsed = false;
            if (node != null)
                parsed = decimal.TryParse(node.Attribute("OBS_VALUE").Value, out res);

            return res;
        }

    }
}