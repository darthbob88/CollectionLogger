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
    public class moviesController : ApiController
    {
        private Media db = new Media();

        // GET: api/movies
        public IQueryable<movies> Getmovies()
        {
            return db.movies;
        }

        // GET: api/movies/5
        [ResponseType(typeof(movies))]
        public async Task<IHttpActionResult> Getmovies(long id)
        {
            movies movies = await db.movies.FindAsync(id);
            if (movies == null)
            {
                return NotFound();
            }

            return Ok(movies);
        }

        // PUT: api/movies/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putmovies(long id, movies movies)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movies.movie_ID)
            {
                return BadRequest();
            }

            db.Entry(movies).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!moviesExists(id))
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

        // POST: api/movies
        [ResponseType(typeof(movies))]
        public async Task<IHttpActionResult> Postmovies(movies movies)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.movies.Add(movies);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = movies.movie_ID }, movies);
        }

        // DELETE: api/movies/5
        [ResponseType(typeof(movies))]
        public async Task<IHttpActionResult> Deletemovies(long id)
        {
            movies movies = await db.movies.FindAsync(id);
            if (movies == null)
            {
                return NotFound();
            }

            db.movies.Remove(movies);
            await db.SaveChangesAsync();

            return Ok(movies);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool moviesExists(long id)
        {
            return db.movies.Count(e => e.movie_ID == id) > 0;
        }
    }
}