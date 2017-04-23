using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Microsoft.Web.Services3;
using Microsoft.Web.Services3.Addressing;
using Microsoft.Web.Services3.Messaging;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    public class BaxicoWebService : SoapClient
    {
        public string url { get; set; }

        public BaxicoWebService(String url):base(new EndpointReference(new Uri(url)))
        {
            this.url = url;
        }

        [SoapMethod("RequestResponseMethod")]
        public SoapEnvelope RequestResponseMethod(SoapEnvelope envelope)
        {
            return base.SendRequestResponse("RequestResponseMethod", envelope);
        }

    }
}