using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SpeedInvoices.Web.Models;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace SpeedInvoices.Web.Controllers
{
    public class ClientController : Controller
    {
        string LienClientApi = "http://localhost:81/SpeedInvoices.Service/api/Client";
        // GET: Client
        public async Task<ActionResult> Index()
        {
            IEnumerable<ClientModel> model = new List<ClientModel>();
            StructureModel structure = (StructureModel)Session["utilisateur"];
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienClientApi + "?IdStructure=" + structure.IdStructure);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<IEnumerable<ClientModel>>(json);
                }
            }
            return View(model);
        }

        public ActionResult Create()
        {
            ClientModel model = new ClientModel();

            StructureModel structure = (StructureModel)Session["utilisateur"];
            model.IdStructure = structure.IdStructure;
            model.Structure = structure;

            return View("Edit", model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            ClientModel model = new ClientModel();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(LienClientApi + "?id=" + id);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ClientModel>(json);
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(ClientModel model)
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
                        if (model.IdClient == 0)
                        {
                            response = await client.PostAsync(LienClientApi, content);
                        }
                        else
                        {
                            response = await client.PutAsync(LienClientApi, content);
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
                var response = await client.DeleteAsync(LienClientApi + "?id=" + id);
            }
            return RedirectToAction("Index");
        }
    }
}