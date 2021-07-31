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
    public class StructureController : Controller
    {
        string LienApiCompte = "http://localhost:81/SpeedInvoices.Service/api/Structure";
        // GET: Compte
        public ActionResult Login(string returnUrl)
        {
            StructureModel s = (StructureModel)Session["utilisateur"];
            if ( s != null)
            {
                return RedirectToAction("Index", "Gestion");
            }
            return View(new StructureModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<ActionResult> Login(StructureModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    IEnumerable<StructureModel> models = new List<StructureModel>();
                    using (HttpClient client = new HttpClient())
                    {
                        var response = await client.GetAsync(LienApiCompte);
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            models = JsonConvert.DeserializeObject<IEnumerable<StructureModel>>(json);
                        }
                    }

                    foreach(var m in models)
                    {
                        if (m.Email.ToLower().Equals(model.Email, StringComparison.OrdinalIgnoreCase))
                        {
                            if (m.MotDePasse.Equals(model.MotDePasse, StringComparison.OrdinalIgnoreCase))
                            {
                                Session["utilisateur"] = m;
                                Session["id"] = m.IdStructure;
                                Session["nom"] = m.NomStructure;
                                Session["LogoStructure"] = m.LogoStructure;
                                return RedirectToAction("Index", "Gestion");
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

        public ActionResult Register(string returnUrl)
        {
            return View(new StructureModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<ActionResult> Register(StructureModel model)
        {
            try
            {
                MultipartFormDataContent multipart = new MultipartFormDataContent();

                if (ModelState.IsValid)
                {
                    model.DateCreationStructure = DateTime.Now;
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
                        HttpResponseMessage response = await client.PostAsync(LienApiCompte, multipart);
                        if (response.IsSuccessStatusCode)
                            return RedirectToAction("Login");
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

        public ActionResult Forgot()
        {
            StructureModel model = new StructureModel();
            model.Message = null;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Forgotten(StructureModel model)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<StructureModel> models = new List<StructureModel>();
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(LienApiCompte);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        models = JsonConvert.DeserializeObject<IEnumerable<StructureModel>>(json);
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

        public ActionResult About()
        {
            return RedirectToAction("Index", "Accueil", "Accueil");
        }
    }
}