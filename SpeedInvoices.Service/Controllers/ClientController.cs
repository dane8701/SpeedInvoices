using SpeedInvoices.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SpeedInvoices.Service.Controllers
{
    public class ClientController : BaseController
    {
        [HttpPost]
        public IHttpActionResult AjouterClient([FromBody] Client client)
        {
            try
            {
                var p = db.Clients.FirstOrDefault(x => x.Email.ToLower() == client.Email.ToLower());
                if (p != null)
                    return Content(HttpStatusCode.Conflict, "Ce client existe déjà.");
                client.IdClient = 0;
                db.Clients.Add(client);
                db.SaveChanges();
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult ListerClient()
        {
            try
            {
                var clients = db.Clients
                    .OrderBy(x => x.NomClient).ToList();
                return Ok(clients);
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
        public IHttpActionResult ListerClient(int IdStructure)
        {
            try
            {
                var clients = db.Clients.Where(x => x.IdStructure == IdStructure)
                    .OrderBy(x => x.NomClient).ToList();
                return Ok(clients);
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
        public IHttpActionResult DetailsClient(int IdStructure, int id)
        {
            try
            {
                var client = db.Clients.Where(x => x.IdStructure == IdStructure && x.IdClient == id);
                return Ok(client);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IHttpActionResult SupprimerClient(int id)
        {
            try
            {
                var client = db.Clients.Find(id);
                if (client == null)
                    return Content(HttpStatusCode.NotFound, "La catégorie " + id + " n'existe pas.");
                db.Clients.Remove(client);
                db.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public IHttpActionResult EditerClient([FromBody] Client newClient)
        {
            try
            {
                var oldClient = db.Clients.AsNoTracking().FirstOrDefault(x => x.IdClient == newClient.IdClient);
                if (oldClient == null)
                    return Content(HttpStatusCode.NotFound, "La catégorie " + newClient.IdClient + " n'existe pas.");
                db.Entry(newClient).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return Ok(newClient);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
