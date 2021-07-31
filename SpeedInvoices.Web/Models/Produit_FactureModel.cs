using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class Produit_FactureModel
    {
        public int IdProduit { get; set; }
        public int IdFacture { get; set; }
        public int QuantiteProduit { get; set; }

        public Produit_FactureModel(int idProduit, int quantiteProduit)
        {
            IdProduit = idProduit;
            QuantiteProduit = quantiteProduit;
        }

        public virtual FactureModel Facture { get; set; }
        public virtual ProduitModel Produit { get; set; }
    }
}