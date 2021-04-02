using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using varsity_w_auth.Models;
using System.Diagnostics;

namespace varsity_w_auth.Controllers
{
    public class SportDataController : ApiController
    {
        private VarsityDataContext db = new VarsityDataContext();

        /// <summary>
        /// Returns all sports in our system
        /// </summary>
        /// <returns>A status code of 200 along with a list of sports in the database.</returns>
        [ResponseType(typeof(IEnumerable<SportDto>))]
        public IHttpActionResult GetSports()
        {
            IEnumerable<Sport> Sports = db.Sports.ToList();
            List<SportDto> SportDtos = new List<SportDto> { };

            foreach (var Sport in Sports)
            {
                SportDto SportDto = new SportDto
                {
                    SportID = Sport.SportID,
                    SportName = Sport.SportName,
                };
                SportDtos.Add(SportDto);
            }

            return Ok(SportDtos);
        }

        /// <summary>
        /// returns a list of teams associated with this sport.
        /// </summary>
        /// <param name="id">The input Sport ID</param>
        /// <returns>All teams associated with this sport</returns>
        [Route("api/sportdata/getteamsforsport/{id}")]
        public IHttpActionResult GetTeamsforSport(int id)
        {
            List<Team> Teams = db.Teams.Where(t=>t.SportID==id).ToList();
            List<TeamDto> TeamDtos = new List<TeamDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Team in Teams)
            {
                TeamDto NewTeam = new TeamDto
                {
                    TeamID = Team.TeamID,
                    TeamName = Team.TeamName,
                    TeamBio = Team.TeamBio
                };
                TeamDtos.Add(NewTeam);
            }

            return Ok(TeamDtos);

        }

        /// <summary>
        /// Finds a particular sport in the database with its ID
        /// </summary>
        /// <param name="id">The sport id primary key</param>
        /// <returns>the associated sport in the database with a 200 status code. 404 if that sport is not found.</returns>
        [HttpGet]
        [ResponseType(typeof(SportDto))]
        public IHttpActionResult FindSport(int id)
        {
            //Find the data
            Sport Sport = db.Sports.Find(id);
            //if not found, return 404 status code.
            if (Sport == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            SportDto SportDto = new SportDto
            {
                SportID = Sport.SportID,
                SportName = Sport.SportName
            };
            //pass along data as 200 status code OK response
            return Ok(SportDto);
        }


        /// <summary>
        /// Updates a sport in the database.
        /// </summary>
        /// <param name="id">The primary key of the sport to update</param>
        /// <param name="Sport">POST data for information about the sport.</param>
        /// <returns>Returns status code of 200 if successful.</returns>
        /// <example>
        /// POST api/sportdata/updatesport
        /// FORM DATA: sport json object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult UpdateSport(int id, [FromBody] Sport Sport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Sport.SportID)
            {
                return BadRequest();
            }

            db.Entry(Sport).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SportExists(id))
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

        /// <summary>
        /// Adds a Sport to the database.
        /// </summary>
        /// <param name="Sport">A Sport object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/SportData/AddSport
        ///  FORM DATA: Sport JSON Object
        /// </example>
        [ResponseType(typeof(Sport))]
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult AddSport([FromBody] Sport Sport)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sports.Add(Sport);
            db.SaveChanges();

            return Ok(Sport.SportID);
        }


        /// <summary>
        /// Deletes a Sport in the database
        /// </summary>
        /// <param name="id">The id of the Sport to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/SportData/DeleteSport/5
        /// </example>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult DeleteSport(int id)
        {
            Sport Sport = db.Sports.Find(id);
            if (Sport == null)
            {
                return NotFound();
            }

            db.Sports.Remove(Sport);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SportExists(int id)
        {
            return db.Sports.Count(e => e.SportID == id) > 0;
        }
    }
}