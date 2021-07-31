using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class IndexClientModel
    {
        public int NombreFNS { get; set; }
        public int NombreFS { get; set; }
        public int Pfns { get; set; }
        public int Pfs { get; set; }
        public string NomClient { get; set; }
        public string PrenomClient { get; set; }
        public string AdresseClient { get; set; }
        public string Email { get; set; }
        public int TelephoneClient { get; set; }
        public int IdStructure { get; set; }

        public virtual StructureModel Structure { get; set; }
    }
}