using Newtonsoft.Json;
using Rotativa;
using SpeedInvoices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Controllers
{
    public class FactureController : Controller
    {
        static StructureModel structure ;
        // GET: Facture
        string LienProduit_FactureApi = "http://localhost:81/SpeedInvoices.Service/api/Produit_Facture";
        string LienFactureApi = "http://localhost:81/SpeedInvoices.Service/api/Facture";
        string LienProduitApi = "http://localhost:81/SpeedInvoices.Service/api/Produit";
        string LienClientApi = "http://localhost:81/SpeedInvoices.Service/api/Client";
        string LienEtatApi = "http://localhost:81/SpeedInvoices.Service/api/Etat";
        // GET: Produit
        public async Task<ActionResult> Ajouter()
        {
            CommandeModel model = new CommandeModel();
            structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var produits = JsonConvert.DeserializeObject<IEnumerable<ProduitModel>>(json);
                    model.Produits = produits.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.IntituleProduit,
                            Value = x.IdProduit.ToString()
                        }
                    );
                }
            }

            return View(model);
        }


        static IList<Produit_FactureModel> Paniers = new List<Produit_FactureModel>();
        public async Task<ActionResult> AjouterProduit(CommandeModel model)
        {
            structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var produits = JsonConvert.DeserializeObject<IEnumerable<ProduitModel>>(json);
                    model.Produits = produits.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.IntituleProduit,
                            Value = x.IdProduit.ToString()
                        }
                    );
                }
            }

            /*foreach(var p in Paniers)
            {
                if (p.IdProduit != model.IdProduit)
                {
                    Paniers.Add(new Produit_FactureModel(model.IdProduit, model.Quantite));
                }
                else
                {
                    p.QuantiteProduit += model.Quantite;
                }
            }*/

            /*Paniers = (IList<Produit_FactureModel>)Session["Paniers"];*/
            Paniers.Add(new Produit_FactureModel(model.IdProduit, model.Quantite));
            Session["Paniers"] = Paniers;
            model.Paniers = Paniers;
            foreach (var p in model.Paniers)
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure + "&id=" + p.IdProduit);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var produit = JsonConvert.DeserializeObject<ProduitModel[]>(json);
                        p.Produit = produit[0];
                    }
                }
            }
            
            return View("Ajouter", model);
        }

        public async Task<ActionResult> Valider()
        {
            FactureModel model = new FactureModel();

            model.Produit_Facture = (ICollection<Produit_FactureModel>)Session["Paniers"];

            structure = (StructureModel)Session["utilisateur"];
            model.IdStructure = structure.IdStructure;
            model.Structure = structure;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienClientApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var clients = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                    model.Clients = clients.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.NomClient,
                            Value = x.IdClient.ToString()
                        }
                    );
                }
            }

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienEtatApi);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var etats = JsonConvert.DeserializeObject<IEnumerable<EtatModel>>(json);
                    model.Etats = etats.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.NomEtat,
                            Value = x.IdEtat.ToString()
                        }
                    );
                }
            }

            return View("Valider", model);
        }

        public async Task<ActionResult> ValiderFacture(FactureModel model)
        {
            try
            {
                structure = (StructureModel)Session["utilisateur"];
                model.IdStructure = structure.IdStructure;
                model.Structure = structure;

                model.Produit_Facture = Paniers;

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienClientApi + "?IdStructure=" + structure.IdStructure);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var clients = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                        model.Clients = clients.Select
                        (
                            x =>
                            new SelectListItem
                            {
                                Text = x.NomClient,
                                Value = x.IdClient.ToString()
                            }
                        );
                    }
                }

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienEtatApi);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var etats = JsonConvert.DeserializeObject<IEnumerable<EtatModel>>(json);
                        model.Etats = etats.Select
                        (
                            x =>
                            new SelectListItem
                            {
                                Text = x.NomEtat,
                                Value = x.IdEtat.ToString()
                            }
                        );
                    }
                }

                model.DateCreationFacture = DateTime.Now;
                foreach(var p in model.Produit_Facture)
                {
                    model.MontantHtFacture += (p.QuantiteProduit * p.Produit.PrixUnitaireProduit);
                }

                model.MontantTtcFacture = model.MontantHtFacture + ((model.MontantHtFacture * model.Tva) / 100) - ((model.MontantHtFacture * model.RemiseFacture) / 100);

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent
                    (
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response;
                        if (model.IdFacture == 0)
                        {
                            response = await client.PostAsync(LienFactureApi, content);
                        }
                        else
                        {
                            response = await client.PutAsync(LienFactureApi, content);
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            Paniers.Clear();
                            return RedirectToAction("Index", "Gestion");
                        }
                        else
                            ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Valider", model);
        }

        public async Task<ActionResult> Index()
        {
            int IdEtat = 3;
            IEnumerable<FactureModel> model = new List<FactureModel>();
            structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&IdEtat=" + IdEtat);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
            }
            return View(model);
        }

        public async Task<ActionResult> FactureNS()
        {
            int IdEtat = 2;
            IEnumerable<FactureModel> model = new List<FactureModel>();
            structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&IdEtat=" + IdEtat);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
            }
            return View(model);
        }

        public async Task<ActionResult> Details(int id)
        {
            FactureModel model = new FactureModel();
            structure = (StructureModel)Session["utilisateur"];

            model.Structure = structure;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var facture = JsonConvert.DeserializeObject<FactureModel[]>(json);
                    model = facture[0];
                }
            }

            IList<Produit_FactureModel> model1 = new List<Produit_FactureModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduit_FactureApi + "?IdFacture=" + model.IdFacture);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model1 = JsonConvert.DeserializeObject<IList<Produit_FactureModel>>(json);
                }
                foreach(var m in model1)
                {
                    model.Produit_Facture.Add(m);
                }
            }


            IList<ProduitModel> produits = new List<ProduitModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    produits = JsonConvert.DeserializeObject<IList<ProduitModel>>(json);
                }
                foreach (var p in produits)
                {
                    foreach (var pf in model.Produit_Facture)
                    {
                        if (p.IdProduit == pf.IdProduit)
                        {
                            pf.Produit = p;
                        }
                    }
                }
            }

            IList<ClientModel> clients = new List<ClientModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienClientApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    clients = JsonConvert.DeserializeObject<IList<ClientModel>>(json);
                }
                foreach (var c in clients)
                {
                    if (c.IdClient == model.IdClient)
                    {
                        model.Client = c;
                    }
                }
            }

            IList<EtatModel> etats = new List<EtatModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienEtatApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    etats = JsonConvert.DeserializeObject<IList<EtatModel>>(json);
                }
                foreach (var e in etats)
                {
                    if (e.IdEtat == model.IdEtat)
                    {
                        model.Etat = e;
                    }
                }
            }

            Session["facture"] = model;

            return View(model);
        }

        public ActionResult SolderFacture(int id)
        {

            return RedirectToAction("FactureNS");
        }

        static int IdFacture;
        public ActionResult ImprimerFacture(int id)
        {
            IdFacture = id;
            return new ActionAsPdf("Facture");
        }

        public async Task<ActionResult> Facture()
        {
            int id = IdFacture;
            FactureModel model = new FactureModel();
            

            model.Structure = structure;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var facture = JsonConvert.DeserializeObject<FactureModel[]>(json);
                    model = facture[0];
                }
            }

            IList<Produit_FactureModel> model1 = new List<Produit_FactureModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduit_FactureApi + "?IdFacture=" + model.IdFacture);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model1 = JsonConvert.DeserializeObject<IList<Produit_FactureModel>>(json);
                }
                foreach (var m in model1)
                {
                    model.Produit_Facture.Add(m);
                }
            }


            IList<ProduitModel> produits = new List<ProduitModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    produits = JsonConvert.DeserializeObject<IList<ProduitModel>>(json);
                }
                foreach (var p in produits)
                {
                    foreach (var pf in model.Produit_Facture)
                    {
                        if (p.IdProduit == pf.IdProduit)
                        {
                            pf.Produit = p;
                        }
                    }
                }
            }

            IList<ClientModel> clients = new List<ClientModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienClientApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    clients = JsonConvert.DeserializeObject<IList<ClientModel>>(json);
                }
                foreach (var c in clients)
                {
                    if (c.IdClient == model.IdClient)
                    {
                        model.Client = c;
                    }
                }
            }

            IList<EtatModel> etats = new List<EtatModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienEtatApi + "?IdStructure=" + model.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    etats = JsonConvert.DeserializeObject<IList<EtatModel>>(json);
                }
                foreach (var e in etats)
                {
                    if (e.IdEtat == model.IdEtat)
                    {
                        model.Etat = e;
                    }
                }
            }

            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(LienFactureApi + "?id=" + id);
            }
            return RedirectToAction("Index");
        }
    }
}