using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MediaModelClasses;

namespace WebFrontend.Controllers
{
    public class artistsController : ApiController
    {
        private Media db = new Media();

        // GET: api/artists
        /// <summary>
        /// API method for accessing list of artists.
        /// </summary>
        /// <returns>List of artists and their basic data.</returns>
        public IQueryable<artist> Getartist()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return db.artist;
        }

        // GET: api/artists/5
        /// <summary>
        /// API method for detailed info for one artist. 
        /// TODO Add more detailed data for each artist, beyond just "Name and album list".
        /// </summary>
        /// <param name="id">artist_id</param>
        /// <returns>Detailed data for that artist.</returns>
        [ResponseType(typeof(artist))]
        public async Task<IHttpActionResult> Getartist(long id)
        {
            artist artist = await db.artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            return Ok(artist);
        }

        // PUT: api/artists/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putartist(long id, artist artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != artist.artist_ID)
            {
                return BadRequest();
            }

            db.Entry(artist).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!artistExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/artists
        [ResponseType(typeof(artist))]
        public async Task<IHttpActionResult> Postartist(artist artist)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.artist.Add(artist);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = artist.artist_ID }, artist);
        }

        // DELETE: api/artists/5
        [ResponseType(typeof(artist))]
        public async Task<IHttpActionResult> Deleteartist(long id)
        {
            artist artist = await db.artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }

            db.artist.Remove(artist);
            await db.SaveChangesAsync();

            return Ok(artist);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool artistExists(long id)
        {
            return db.artist.Count(e => e.artist_ID == id) > 0;
        }
    }
}