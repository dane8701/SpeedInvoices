using Newtonsoft.Json;
using Rotativa;
using SpeedInvoices.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Controllers
{
    public class ClientDashController : Controller
    {
        string LienClientApi = "http://localhost:81/SpeedInvoices.Service/api/Client";
        string LienFactureApi = "http://localhost:81/SpeedInvoices.Service/api/Facture";
        string LienApiCompte = "http://localhost:81/SpeedInvoices.Service/api/Structure";
        string LienProduit_FactureApi = "http://localhost:81/SpeedInvoices.Service/api/Produit_Facture";
        string LienProduitApi = "http://localhost:81/SpeedInvoices.Service/api/Produit";
        string LienEtatApi = "http://localhost:81/SpeedInvoices.Service/api/Etat";
        static ClientModel clientStatic;
        // GET: ClientDash
        public async Task<ActionResult> Index()
        {
            IndexClientModel model = new IndexClientModel();
            ClientModel clientt = clientStatic;
            //Compter nombre de facture non soldée
            using (HttpClient client = new HttpClient())
            {
                IEnumerable<FactureModel> factureNS = new List<FactureModel>();
                var response = await client.GetAsync(LienFactureApi + "?IdClient=" + clientt.IdClient + "&IdEtat=" + 2);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    factureNS = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
                model.NombreFNS = factureNS.Count();
            }

            //Compter nombre de facture soldée
            using (HttpClient client = new HttpClient())
            {
                IEnumerable<FactureModel> factureS = new List<FactureModel>();
                var response = await client.GetAsync(LienFactureApi + "?IdClient=" + clientt.IdClient + "&IdEtat=" + 3);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    factureS = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
                model.NombreFS = factureS.Count();
            }

            //Calcul des pourcentages
            model.Pfns = model.NombreFNS;
            model.Pfs = model.NombreFS;

            //Structure
            model.NomClient = clientt.NomClient;
            model.PrenomClient = clientStatic.PrenomClient;
            model.AdresseClient = clientt.AdresseClient;
            model.TelephoneClient = clientt.TelephoneClient;
            model.Email = clientt.Email;

            return View(model);
        }

        public async Task<ActionResult> FactureS()
        {
            int IdEtat = 2;
            IEnumerable<FactureModel> model = new List<FactureModel>();
            
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdClient=" + clientStatic.IdClient + "&IdEtat=" + IdEtat);
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
            int IdEtat = 3;
            IEnumerable<FactureModel> model = new List<FactureModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdClient=" + clientStatic.IdClient + "&IdEtat=" + IdEtat);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
            }
            return View(model);
        }


        public ActionResult Login(string returnUrl)
        {
            ClientModel c = clientStatic;
            if (c != null)
            {
                return RedirectToAction("Index");
            }
            return View(new ClientModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<ActionResult> Login(ClientModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IList<StructureModel> structures = new List<StructureModel>();
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(LienApiCompte);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            structures = JsonConvert.DeserializeObject<IList<StructureModel>>(json);
                        }
                    }

                    IEnumerable<ClientModel> models = new List<ClientModel>();
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(LienClientApi);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            models = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                        }
                    }

                    foreach (var m in models)
                    {
                        if (m.Email.ToLower().Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            if (m.MotDePasse.Equals(model.MotDePasse, StringComparison.OrdinalIgnoreCase))
                            {
                                foreach(var s in structures)
                                {
                                    if(m.IdStructure == s.IdStructure)
                                    {
                                        Session["Client"] = m;
                                        Session["IdClient"] = m.IdClient;
                                        Session["NomClient"] = m.NomClient + " " + m.PrenomClient;
                                        m.Structure = s;
                                        clientStatic = m;
                                        return RedirectToAction("Index");
                                    }
                                }
                                
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            model.IsError = true;
            model.Message = "Email ou mot de passe invalide !";
            return View(model);
        }

        public ActionResult Forgot()
        {
            ClientModel model = new ClientModel();
            model.Message = null;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Forgotten(ClientModel model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<ClientModel> models = new List<ClientModel>();
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienClientApi);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        models = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                    }
                }

                foreach (var m in models)
                {
                    if (m.Email.ToLower().Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        return View("ForgotOK");
                    }
                }
            }
            model.IsError = true;
            model.Message = "Cet email n'existe pas !";
            return View("Forgot", model);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Accueil", "Accueil");
        }

        public ActionResult Parametre()
        {
            ClientModel model = clientStatic;
            model.MotDePasse = null;
            return View(model);
        }

        [HttpPost]
        // Ne marche pas !! ca ne change pas le mot de passe.
        public async Task<ActionResult> Changer(ClientModel model)
        {
            try
            {
                ClientModel clientModel = clientStatic;
                model = clientModel;
                if (ModelState.IsValid)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(LienClientApi + "?IdClient" + model.IdClient + "&NMotDePasse=" + model.NMotDePasse);

                        if (response.IsSuccessStatusCode)
                            return RedirectToAction("Index");
                        else
                            ModelState.AddModelError("", await response.Content.ReadAsStringAsync());
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            return View("Parametre", model);
        }

        public async Task<ActionResult> Details(int id)
        {
            FactureModel model = new FactureModel();

            model.Structure = clientStatic.Structure;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + model.Structure.IdStructure + "&id=" + id);
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

            Session["facture"] = model;

            return View(model);
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


            model.Structure = clientStatic.Structure;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + model.Structure.IdStructure + "&id=" + id);
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
    }
}