using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class ClientModel
    {
        public int IdClient { get; set; }
        public string NomClient { get; set; }
        public string PrenomClient { get; set; }
        public string AdresseClient { get; set; }
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
        public int TelephoneClient { get; set; }
        public int IdStructure { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public virtual StructureModel Structure { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FactureModel> Factures { get; set; }
    }
}