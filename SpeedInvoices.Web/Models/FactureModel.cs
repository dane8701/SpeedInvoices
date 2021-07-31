using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Models
{
    public class FactureModel
    {
        public int IdFacture { get; set; }
        public string ReferenceFacture { get; set; }
        public System.DateTime DateCreationFacture { get; set; }
        public int RemiseFacture { get; set; }
        public float Tva { get; set; }
        public float MontantHtFacture { get; set; }
        public float MontantTtcFacture { get; set; }
        public int IdEtat { get; set; }
        public int IdClient { get; set; }
        public int IdStructure { get; set; }
        public IEnumerable<SelectListItem> Clients { get; set; }
        public IEnumerable<SelectListItem> Etats { get; set; }
        public virtual ClientModel Client { get; set; }
        public virtual EtatModel Etat { get; set; }
        public virtual StructureModel Structure { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Produit_FactureModel> Produit_Facture { get; set; }

    }
}