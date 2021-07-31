using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Models
{
    public class CommandeModel
    {
        public int IdProduit { get; set; }
        public int Quantite { get; set; }
        public IEnumerable<SelectListItem> Produits { get; set; }
        public IList<Produit_FactureModel> Paniers { get; set; }
    }
}