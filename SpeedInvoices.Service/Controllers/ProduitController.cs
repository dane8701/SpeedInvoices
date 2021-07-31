using SpeedInvoices.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpeedInvoices.Service.Controllers
{
    public class ProduitController : BaseController
    {
        [HttpPost]
        public IHttpActionResult AjouterProduit([FromBody] Produit produit)
        {
            try
            {
                var p = db.Produits.FirstOrDefault(x => x.ReferenceProduit.ToLower() == produit.ReferenceProduit.ToLower());
                if (p != null)
                    return Content(HttpStatusCode.Conflict, "Cette référence de produit existe déjà.");
                produit.IdProduit = 0;
                db.Produits.Add(produit);
                db.SaveChanges();
                return Ok(produit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> ListerProduit()
        {
            try
            {
                var produits = await db.Produits
                    .OrderBy(x => x.ReferenceProduit).ToArrayAsync();
                return Ok(produits);
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
        public IHttpActionResult ListerProduit(int IdStructure)
        {
            try
            {
                var produits = db.Produits.Where(x => x.IdStructure == IdStructure)
                    .OrderBy(x => x.ReferenceProduit).ToList();
                return Ok(produits);
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
        public IHttpActionResult DetailsProduit(int IdStructure, int id)
        {
            try
            {
                var produit = db.Produits.Where(x => x.IdStructure == IdStructure && x.IdProduit == id);
                return Ok(produit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult SupprimerProduit(int id)
        {
            try
            {
                var produit = db.Produits.Find(id);
                if (produit == null)
                    return Content(HttpStatusCode.NotFound, "Le produit " + id + " n'existe pas.");
                db.Produits.Remove(produit);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult EditerProduit([FromBody] Produit newProduit)
        {
            try
            {
                var oldProduit = db.Produits.AsNoTracking().FirstOrDefault(x => x.IdProduit == newProduit.IdProduit);
                if (oldProduit == null)
                    return Content(HttpStatusCode.NotFound, "Le produit " + newProduit.IdProduit + " n'existe pas.");
                db.Entry(newProduit).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(newProduit);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
