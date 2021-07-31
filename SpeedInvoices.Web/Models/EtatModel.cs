using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class EtatModel
    {
        public int IdEtat { get; set; }
        public string NomEtat { get; set; }
        public string DescriptionEtat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactureModel> Factures { get; set; }
    }
}