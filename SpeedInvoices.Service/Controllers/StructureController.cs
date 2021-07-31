using Newtonsoft.Json;
using SpeedInvoices.Service.Entities;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SpeedInvoices.Service.Controllers
{
    public class StructureController : BaseController
    {
        [HttpPost]
        public async Task<IHttpActionResult> AjouterStructure()
        {
            try
            {
                var json = HttpContext.Current.Request.Form["data"];
                Structure model = JsonConvert.DeserializeObject<Structure>(json);

                string uploadFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    var file = HttpContext.Current.Request.Files[0];
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(uploadFolder, filename));
                    model.LogoStructure = filename;
                }

                model.IdStructure = 0;
                model.DateCreationStructure = DateTime.Now;
                db.Structures.Add(model);
                await db.SaveChangesAsync();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> ListerStructure()
        {
            try
            {
                var models = await db.Structures
                    .OrderByDescending(x => x.DateCreationStructure)
                    .ToArrayAsync();
                foreach (var p in models)
                {
                    p.LogoStructure = Url.Content("~/Uploads/" + p.LogoStructure).ToString();
                }
                return Ok(models);
            }
            catch (DbUpdateException ex)
            {
                var exception = ex.InnerException?.InnerException as SqlException;
                return BadRequest(exception?.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> DetailsStructure(int id)
        {
            try
            {
                var models = await db.Structures.FindAsync(id);
                models.LogoStructure = Request.RequestUri.GetLeftPart(UriPartial.Authority) +
                    "/Uploads/" + models.LogoStructure;
                return Ok(models);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> SupprimerStructure(int id)
        {
            try
            {
                var model = db.Structures.Find(id);
                if (model == null)
                    return Content(HttpStatusCode.NotFound, "La model " + id + " n'existe pas.");
                db.Structures.Remove(model);
                await db.SaveChangesAsync();

                string uploadFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                if (!string.IsNullOrEmpty(model.LogoStructure) &&
                        File.Exists(Path.Combine(uploadFolder, model.LogoStructure)))
                {
                    File.Delete(Path.Combine(uploadFolder, model.LogoStructure));
                }


                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult EditerStructure()
        {
            try
            {
                var json = HttpContext.Current.Request.Form["data"];
                Structure newStructure = JsonConvert.DeserializeObject<Structure>(json);

                var oldStructure = db.Structures.AsNoTracking().FirstOrDefault(x => x.IdStructure == newStructure.IdStructure);
                if (oldStructure == null)
                    return Content(HttpStatusCode.NotFound, "La structure " + newStructure.NomStructure + " n'existe pas.");

                string uploadFolder = HttpContext.Current.Server.MapPath("~/Uploads");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    if (!string.IsNullOrEmpty(newStructure.LogoStructure) &&
                        File.Exists(Path.Combine(uploadFolder, oldStructure.LogoStructure)))
                    {
                        File.Delete(Path.Combine(uploadFolder, oldStructure.LogoStructure));
                    }
                    var file = HttpContext.Current.Request.Files[0];
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(uploadFolder, filename));
                    newStructure.LogoStructure = filename;
                }
                else
                {
                    newStructure.LogoStructure = oldStructure.LogoStructure;
                }
                newStructure.DateCreationStructure = oldStructure.DateCreationStructure;
                db.Entry(newStructure).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(newStructure);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public IHttpActionResult ChangeMotDePasse(int IdStructure, string NMotDePasse)
        {
            try
            {
                var model = db.Structures.AsNoTracking().FirstOrDefault(x => x.IdStructure == IdStructure);
                model.MotDePasse = NMotDePasse;
                db.Entry(model.MotDePasse).State = EntityState.Modified;
                model.MotDePasse = NMotDePasse;

                db.SaveChanges();
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
