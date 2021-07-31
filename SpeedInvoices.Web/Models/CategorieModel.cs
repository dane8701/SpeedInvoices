using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class CategorieModel
    {
        public int IdCategorie { get; set; }
        public string NomCategorie { get; set; }
        public int IdStructure { get; set; }

        public virtual StructureModel Structure { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProduitModel> Produits { get; set; }
    }
}