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
    public class FactureController : BaseController
    {
        [HttpPost]
        public IHttpActionResult AjouterFacture([FromBody] Facture model)
        {
            try
            {
                Facture facture = new Facture
                {
                    IdFacture = model.IdFacture,
                    ReferenceFacture = model.ReferenceFacture,
                    DateCreationFacture = DateTime.Now,
                    RemiseFacture = model.RemiseFacture,
                    Tva = model.Tva,
                    MontantHtFacture = model.MontantHtFacture,
                    MontantTtcFacture = model.MontantTtcFacture,
                    IdEtat = model.IdEtat,
                    IdClient = model.IdClient,
                    IdStructure = model.IdStructure
                };
                var p = db.Factures.FirstOrDefault(x => x.ReferenceFacture.ToLower() == facture.ReferenceFacture.ToLower());
                if (p != null)
                    return Content(HttpStatusCode.Conflict, "Cette reférence de facture existe déjà.");
                facture.IdFacture = 0;
                facture.DateCreationFacture = DateTime.Now;
                db.Factures.Add(facture);
                db.SaveChanges();

                var factures = db.Factures.ToList();
                Facture lastFacture = new Facture();
                lastFacture.IdFacture = -1;
                foreach (var item in factures)
                {
                    if (lastFacture.IdFacture < item.IdFacture)
                        lastFacture = item;
                }

                foreach (var pf in model.Produit_Facture)
                {
                    db.Produit_Facture.Add(
                        new Produit_Facture { IdFacture = (int)lastFacture.IdFacture, IdProduit = pf.IdProduit, QuantiteProduit = pf.QuantiteProduit }
                        );
                }
                db.SaveChanges();
                return Ok(facture);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult ListerFacture(int IdStructure, int IdEtat)
        {
            try
            {
                var factures = db.Factures.Where(x => x.IdStructure == IdStructure && x.IdEtat == IdEtat)
                    .OrderBy(x => x.DateCreationFacture).ToList();
                return Ok(factures);
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
        public IHttpActionResult ListerFactureClient(int IdClient, int IdEtat)
        {
            try
            {
                var factures = db.Factures.Where(x => x.IdClient == IdClient && x.IdEtat == IdEtat)
                    .OrderBy(x => x.DateCreationFacture).ToList();
                return Ok(factures);
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
        public IHttpActionResult DetailsFacture(int IdStructure, int id)
        {
            try
            {
                var facture = db.Factures.Where(x => x.IdStructure == IdStructure && x.IdFacture == id);
                return Ok(facture);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult DetailsFactureClient(int IdClient, int id)
        {
            try
            {
                var facture = db.Factures.Where(x => x.IdClient == IdClient && x.IdFacture == id);
                return Ok(facture);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult SupprimerFacture(int id)
        {
            try
            {
                var facture = db.Factures.Find(id);
                if (facture == null)
                    return Content(HttpStatusCode.NotFound, "La facture " + id + " n'existe pas.");
                db.Factures.Remove(facture);
                db.SaveChanges();

                var produits = db.Produit_Facture.Where(x => x.IdFacture == id).ToList();
                db.Produit_Facture.RemoveRange(produits);
                db.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
