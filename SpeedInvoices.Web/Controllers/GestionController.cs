using Newtonsoft.Json;
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
    public class GestionController : Controller
    {
        string LienApiCompte = "http://localhost:81/SpeedInvoices.Service/api/Structure";
        string LienClientApi = "http://localhost:81/SpeedInvoices.Service/api/Client";
        string LienProduitApi = "http://localhost:81/SpeedInvoices.Service/api/Produit";
        string LienFactureApi = "http://localhost:81/SpeedInvoices.Service/api/Facture";

        // GET: Gestion
        public async Task<ActionResult> Index()
        {
            IndexModel model = new IndexModel();
            StructureModel structure = (StructureModel)Session["utilisateur"];

            //Compter nombre clients
            using (HttpClient client = new HttpClient())
            {
                IEnumerable<ClientModel> clients = new List<ClientModel>();
                var response = await client.GetAsync(LienClientApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    clients = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                }
                model.NombreC = clients.Count();
            }

            //Compter nombre produits
            using (HttpClient client = new HttpClient())
            {
                IEnumerable<ProduitModel> produits = new List<ProduitModel>();
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    produits = JsonConvert.DeserializeObject<IEnumerable<ProduitModel>>(json);
                    model.NombreP = produits.Count();
                }
                else
                    model.NombreP = structure.Produits.Count();
            }

            //Compter nombre de facture non soldée
            using (HttpClient client = new HttpClient())
            {
                IEnumerable<FactureModel> factureNS = new List<FactureModel>();
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&IdEtat=" + 2);
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
                var response = await client.GetAsync(LienFactureApi + "?IdStructure=" + structure.IdStructure + "&IdEtat=" + 3);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    factureS = JsonConvert.DeserializeObject<IEnumerable<FactureModel>>(json);
                }
                model.NombreFS = factureS.Count();
            }

            //Calcul des pourcentages
            model.Pc = model.NombreC;
            model.Pfns = model.NombreFNS;
            model.Pfs = model.NombreFS;
            model.Pp = model.NombreP;

            //Structure
            model.NomStructure = structure.NomStructure;
            model.AdresseStructure = structure.AdresseStructure;
            model.TelephoneStructure = structure.TelephoneStructure;
            model.Email = structure.Email;

            return View(model);
        }

        public ActionResult Parametre()
        {
            StructureModel model = (StructureModel)Session["utilisateur"];
            model.MotDePasse = null;
            return View(model);
        }

        public async Task<ActionResult> Editer(int id)
        {
            StructureModel model = new StructureModel();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienApiCompte + "?id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<StructureModel>(json);
                }
            }

            return View("Edit", model);
        }

        [HttpPost]
        // Ne marche pas !! pour editer les infos d'une structure a partir des parametres.
        public async Task<ActionResult> Editer(StructureModel model)
        {
            try
            {
                StructureModel structureModel = (StructureModel)Session["utilisateur"];
                model.IdStructure = structureModel.IdStructure;/*
                model.DateCreationStructure = structureModel.DateCreationStructure;
                model.MotDePasse = structureModel.MotDePasse;*/

                MultipartFormDataContent multipart = new MultipartFormDataContent();
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(model);
                    StringContent content = new StringContent
                    (
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );
                    multipart.Add(content, "data");

                    if (model.Image.ContentLength > 0)
                    {
                        byte[] picture = new byte[model.Image.ContentLength];
                        model.Image.InputStream.Read(picture, 0, picture.Length);
                        ByteArrayContent byteContent = new ByteArrayContent(picture);
                        multipart.Add(byteContent, "picture", model.Image.FileName);
                    }

                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response;
                        if (model.IdStructure == 0)
                        {
                            response = await client.PostAsync(LienApiCompte, multipart);
                        }
                        else
                        {
                            response = await client.PutAsync(LienApiCompte, multipart);
                        }

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

            return View("Edit", model);
        }

        [HttpPost]
        // Ne marche pas !! ca ne change pas le mot de passe.
        public async Task<ActionResult> Changer(StructureModel model)
        {
            try
            {
                StructureModel structureModel = (StructureModel)Session["utilisateur"];
                model = structureModel;
                if (ModelState.IsValid)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(LienApiCompte + "?IdStructure" + model.IdStructure + "&NMotDePasse=" + model.NMotDePasse);

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
    }
}