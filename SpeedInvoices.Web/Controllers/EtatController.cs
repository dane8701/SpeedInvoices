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
    public class EtatController : Controller
    {
        string LienEtatApi = "http://localhost:81/SpeedInvoices.Service/api/Etat";
        // GET: Etat
        public async Task<ActionResult> Index()
        {
            IEnumerable<EtatModel> model = new List<EtatModel>();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienEtatApi);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<EtatModel>>(json);
                }
            }
            return View(model);
        }

        public ActionResult Create()
        {
            EtatModel model = new EtatModel();

            return View("Edit", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            EtatModel model = new EtatModel();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienEtatApi + "?id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<EtatModel>(json);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(EtatModel model)
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
                        if (model.IdEtat == 0)
                        {
                            response = await client.PostAsync(LienEtatApi, content);
                        }
                        else
                        {
                            response = await client.PutAsync(LienEtatApi, content);
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
                var response = await client.DeleteAsync(LienEtatApi + "?id=" + id);
            }
            return RedirectToAction("Index");
        }
    }
}