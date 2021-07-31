using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Models
{
    public class ProduitModel
    {
        public int IdProduit { get; set; }
        public string ReferenceProduit { get; set; }
        public string IntituleProduit { get; set; }
        public string DescriptionProduit { get; set; }
        public int PrixUnitaireProduit { get; set; }
        public int IdCategorie { get; set; }
        public int IdStructure { get; set; }

        public virtual CategorieModel Categorie { get; set; }
        public virtual StructureModel Structure { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactureModel> Factures { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IEnumerable<SelectListItem> Structures { get; set; }
    }
}