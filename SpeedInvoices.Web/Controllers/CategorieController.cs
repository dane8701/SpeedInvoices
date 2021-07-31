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
    public class CategorieController : Controller
    {
        string LienCategorieApi = "http://localhost:81/SpeedInvoices.Service/api/Categorie";
        
        // GET: Categorie
        public async Task<ActionResult> Index()
        {
            IEnumerable<CategorieModel> model = new List<CategorieModel>();
            StructureModel structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienCategorieApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<CategorieModel>>(json);
                }
            }
            return View(model);
        }

        public ActionResult Create()
        {
            CategorieModel model = new CategorieModel();

            StructureModel structure = (StructureModel)Session["utilisateur"];
            model.IdStructure = structure.IdStructure;
            model.Structure = structure;

            return View("Edit", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            CategorieModel model = new CategorieModel();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienCategorieApi + "?id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<CategorieModel>(json);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CategorieModel model)
        {
            try
            {
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
                        if (model.IdCategorie == 0)
                        {
                            response = await client.PostAsync(LienCategorieApi, content);
                        }
                        else
                        {
                            response = await client.PutAsync(LienCategorieApi, content);
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
                var response = await client.DeleteAsync(LienCategorieApi + "?id=" + id);
            }
            return RedirectToAction("Index");
        }
    }
}