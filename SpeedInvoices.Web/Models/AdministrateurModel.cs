using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpeedInvoices.Web.Models
{
    public class AdministrateurModel
    {
        public int IdAdministrateur { get; set; }
        public string NomUtilisateur { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; }
        public string ReturnUrl { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StructureModel> Structures { get; set; }
    }
}