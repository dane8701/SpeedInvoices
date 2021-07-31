using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class StructureModel
    {
        public int IdStructure { get; set; }
        public string NomStructure { get; set; }
        public int TelephoneStructure { get; set; }

        public string AdresseStructure { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateCreationStructure { get; set; }
        public string LogoStructure { get; set; }
        public int IdAdministrateur { get; set; }
        public int IdClient { get; set; }
        public string Email { get; set; }
        /*[Required]
        [StringLength(15, MinimumLength = 6)]*/
        public string MotDePasse { get; set; }
        /*[Required]
        [StringLength(15, MinimumLength = 6)]*/
        public string NMotDePasse { get; set; }
        /*[Required]
        [StringLength(15, MinimumLength = 6)]
        [Compare(nameof(NCMotDePasse))]*/
        public string NCMotDePasse { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        [JsonIgnore]
        public HttpPostedFileBase Image { get; set; }

        public virtual AdministrateurModel Administrateur { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CategorieModel> Categories { get; set; }
        public virtual ClientModel Client { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactureModel> Factures { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProduitModel> Produits { get; set; }
    }
}