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
    public class EtatController : BaseController
    {
        [HttpPost]
        public IHttpActionResult AjouterEtat([FromBody] Etat etat)
        {
            try
            {
                var p = db.Etats.FirstOrDefault(x => x.NomEtat.ToLower() == etat.NomEtat.ToLower());
                if (p != null)
                    return Content(HttpStatusCode.Conflict, "Cette référence de etat existe déjà.");
                etat.IdEtat = 0;
                db.Etats.Add(etat);
                db.SaveChanges();
                return Ok(etat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IHttpActionResult> ListerEtat()
        {
            try
            {
                var models = await db.Etats
                    .ToArrayAsync();
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
        public IHttpActionResult DetailsEtat(int id)
        {
            try
            {
                var etat = db.Etats.Find(id);
                return Ok(etat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult SupprimerEtat(int id)
        {
            try
            {
                var etat = db.Etats.Find(id);
                if (etat == null)
                    return Content(HttpStatusCode.NotFound, "L'etat " + id + " n'existe pas.");
                db.Etats.Remove(etat);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult EditerEtat([FromBody] Etat newEtat)
        {
            try
            {
                var oldEtat = db.Etats.AsNoTracking().FirstOrDefault(x => x.IdEtat == newEtat.IdEtat);
                if (oldEtat == null)
                    return Content(HttpStatusCode.NotFound, "L'etat " + newEtat.IdEtat + " n'existe pas.");
                db.Entry(newEtat).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(newEtat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
