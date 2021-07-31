using SpeedInvoices.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SpeedInvoices.Service.Controllers
{
    public class CategorieController : BaseController
    {
        [HttpPost]
        public IHttpActionResult AjouterCategorie([FromBody] Categorie categorie)
        {
            try
            {
                var p = db.Categories.FirstOrDefault(x => x.NomCategorie.ToLower() == categorie.NomCategorie.ToLower());
                if (p != null)
                    return Content(HttpStatusCode.Conflict, "Ce nom de catégorie existe déjà.");
                categorie.IdCategorie = 0;
                db.Categories.Add(categorie);
                db.SaveChanges();
                return Ok(categorie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult ListerCategorie(int IdStructure)
        {
            try
            {

                var categories = db.Categories.Where(x => x.IdStructure == IdStructure)
                    .OrderBy(x => x.NomCategorie).ToList();
                return Ok(categories);
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
        public IHttpActionResult DetailsCategorie(int IdStructure, int id)
        {
            try
            {
                var categorie = db.Categories.Where(x => x.IdStructure == IdStructure && x.IdCategorie == id);
                return Ok(categorie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult SupprimerCategorie(int id)
        {
            try
            {
                var categorie = db.Categories.Find(id);
                if (categorie == null)
                    return Content(HttpStatusCode.NotFound, "La catégorie " + id + " n'existe pas.");
                db.Categories.Remove(categorie);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult EditerCategorie([FromBody] Categorie newCategorie)
        {
            try
            {
                var oldCategorie = db.Categories.AsNoTracking().FirstOrDefault(x => x.IdCategorie == newCategorie.IdCategorie);
                if (oldCategorie == null)
                    return Content(HttpStatusCode.NotFound, "La catégorie " + newCategorie.IdCategorie + " n'existe pas.");
                db.Entry(newCategorie).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(newCategorie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
