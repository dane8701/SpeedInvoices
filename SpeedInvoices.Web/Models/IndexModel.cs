using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class IndexModel
    {
        public int NombreC { get; set; }
        public int NombreFNS { get; set; }
        public int NombreFS { get; set; }
        public int NombreP { get; set; }
        public int Pc { get; set; }
        public int Pfns { get; set; }
        public int Pfs { get; set; }
        public int Pp { get; set; }
        public string NomStructure { get; set; }
        public string Email { get; set; }
        public string AdresseStructure { get; set; }
        public int TelephoneStructure { get; set; }
        public virtual ClientModel Client { get; set; }
        public virtual FactureModel Facture { get; set; }
        public virtual ProduitModel Produit { get; set; }
    }
}