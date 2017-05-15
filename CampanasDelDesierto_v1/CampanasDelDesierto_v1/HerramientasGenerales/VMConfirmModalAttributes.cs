using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CampanasDelDesierto_v1.HerramientasGenerales
{
    public class VMConfirmModalAttributes
    {
        public string controller, action, javascriptFunction, primaryMessage, modalTitle, modalID;
        public dynamic routeVals, confirmButtonHtmlAttributes;
        public Style modalStyle;
        public CallType callType = CallType.NONE;

        public class Style
        {
            private const string SUCCESS = "success";
            private const string INFO = "info";
            private const string WARNING = "warning";
            private const string DANGER = "danger";
            private const string DEFAULT = "default";

            public enum StyleTypes
            { SUCCESS, INFO, WARNING, DANGER }

            public StyleTypes modalType { get; set; }

            public Style(StyleTypes type)
            {
                this.modalType = type;
            }

            public override string ToString()
            {
                string res = DEFAULT;
                if (this.modalType == StyleTypes.DANGER)
                    res = DANGER;
                if (this.modalType == StyleTypes.SUCCESS)
                    res = SUCCESS;
                if (this.modalType == StyleTypes.INFO)
                    res = INFO;
                if (this.modalType == StyleTypes.WARNING)
                    res = WARNING;
                return res;
            }
        }
        public enum CallType
        {
            NONE, JAVASCRIPT, POSTBACK
        }
    }
}