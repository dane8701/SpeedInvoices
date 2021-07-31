using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SpeedInvoices.Web.Models;
using System.Web.Mvc;

namespace SpeedInvoices.Web.Controllers
{
    public class ProduitController : Controller
    {
        string LienProduitApi = "http://localhost:81/SpeedInvoices.Service/api/Produit";
        string LienCategorieApi = "http://localhost:81/SpeedInvoices.Service/api/Categorie";
        // GET: Produit
        public async Task<ActionResult> Index()
        {
            IEnumerable<ProduitModel> model = new List<ProduitModel>();
            StructureModel structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<ProduitModel>>(json);
                    return View(model);
                }
            }
            model = structure.Produits;
            return View(model);
        }

        public async Task<ActionResult> Create()
        {
            ProduitModel model = new ProduitModel();
            StructureModel structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienCategorieApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<IEnumerable<CategorieModel>>(json);
                    model.Categories = categories.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.NomCategorie,
                            Value = x.IdCategorie.ToString()
                        }
                    );
                }
            }

            model.IdStructure = structure.IdStructure;
            model.Structure = structure;

            return View("Edit", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            ProduitModel model = new ProduitModel();
            StructureModel structure = (StructureModel)Session["utilisateur"];

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienProduitApi + "?IdStructure=" + structure.IdStructure + "&id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProduitModel>(json);
                }

                response = await client.GetAsync(LienCategorieApi);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var categories = JsonConvert.DeserializeObject<IEnumerable<CategorieModel>>(json);
                    model.Categories = categories.Select
                    (
                        x =>
                        new SelectListItem
                        {
                            Text = x.NomCategorie,
                            Value = x.IdCategorie.ToString(),
                            Selected = x.IdCategorie == model.Categorie.IdCategorie
                        }
                    );
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ProduitModel model)
        {
            StructureModel structure = (StructureModel)Session["utilisateur"];
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienCategorieApi + "?IdStructure=" + structure.IdStructure);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var categories = JsonConvert.DeserializeObject<IEnumerable<CategorieModel>>(json);
                        model.Categories = categories.Select
                        (
                            x =>
                            new SelectListItem
                            {
                                Text = x.NomCategorie,
                                Value = x.IdCategorie.ToString(),
                            }
                        );
                    }
                }

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
                        if (model.IdProduit == 0)
                        {
                            response = await client.PostAsync(LienProduitApi, content);
                        }
                        else
                        {
                            response = await client.PutAsync(LienProduitApi, content);
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

            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.DeleteAsync(LienProduitApi + "?id=" + id);
            }
            return RedirectToAction("Index");
        }
    }
}