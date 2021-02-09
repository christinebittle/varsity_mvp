using System;
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
    public class SponsorDataController : ApiController
    {
        //This variable is our database access point
        private VarsityDataContext db = new VarsityDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Sponsor"
        //Choose context "Varsity Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.


        /// <summary>
        /// Gets a list or Sponsors in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Sponsors including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/SponsorData/GetSponsors
        /// </example>
        [ResponseType(typeof(IEnumerable<SponsorDto>))]
        public IHttpActionResult GetSponsors()
        {
            List<Sponsor> Sponsors = db.Sponsors.ToList();
            List<SponsorDto> SponsorDtos = new List<SponsorDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Sponsor in Sponsors)
            {
                SponsorDto NewSponsor = new SponsorDto
                {
                    SponsorID = Sponsor.SponsorID,
                    SponsorName = Sponsor.SponsorName,
                    SponsorURL = Sponsor.SponsorURL
                };
                SponsorDtos.Add(NewSponsor);
            }

            return Ok(SponsorDtos);
        }

        /// <summary>
        /// Gets a list or Teams in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input sponsor id</param>
        /// <returns>A list of Teams including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/TeamData/GetTeamsForSponsor
        /// </example>
        [ResponseType(typeof(IEnumerable<TeamDto>))]
        public IHttpActionResult GetTeamsForSponsor(int id)
        {
            //sql equivalent
            //select * from teams
            //inner join sponsorteams on sponsorteams.teamid = teams.teamid
            //inner join sponsors on sponsors.sponsorid = sponsorteams.sponsorid
            List<Team> Teams = db.Teams
                .Where(t => t.Sponsors.Any(s => s.SponsorID == id))
                .ToList();
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
        /// Finds a particular Sponsor in the database with a 200 status code. If the Sponsor is not found, return 404.
        /// </summary>
        /// <param name="id">The Sponsor id</param>
        /// <returns>Information about the Sponsor, including Sponsor id, bio, first and last name, and teamid</returns>
        // <example>
        // GET: api/SponsorData/FindSponsor/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(SponsorDto))]
        public IHttpActionResult FindSponsor(int id)
        {
            //Find the data
            Sponsor Sponsor = db.Sponsors.Find(id);
            //if not found, return 404 status code.
            if (Sponsor == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            SponsorDto SponsorDto = new SponsorDto
            {
                SponsorID = Sponsor.SponsorID,
                SponsorName = Sponsor.SponsorName,
                SponsorURL = Sponsor.SponsorURL
            };


            //pass along data as 200 status code OK response
            return Ok(SponsorDto);
        }

        /// <summary>
        /// Updates a Sponsor in the database given information about the Sponsor.
        /// </summary>
        /// <param name="id">The Sponsor id</param>
        /// <param name="Sponsor">A Sponsor object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/SponsorData/UpdateSponsor/5
        /// FORM DATA: Sponsor JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateSponsor(int id, [FromBody] Sponsor Sponsor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Sponsor.SponsorID)
            {
                return BadRequest();
            }

            db.Entry(Sponsor).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SponsorExists(id))
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
        /// Adds a Sponsor to the database.
        /// </summary>
        /// <param name="Sponsor">A Sponsor object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/Sponsors/AddSponsor
        ///  FORM DATA: Sponsor JSON Object
        /// </example>
        [ResponseType(typeof(Sponsor))]
        [HttpPost]
        public IHttpActionResult AddSponsor([FromBody] Sponsor Sponsor)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Sponsors.Add(Sponsor);
            db.SaveChanges();

            return Ok(Sponsor.SponsorID);
        }

        /// <summary>
        /// Deletes a Sponsor in the database
        /// </summary>
        /// <param name="id">The id of the Sponsor to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/Sponsors/DeleteSponsor/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteSponsor(int id)
        {
            Sponsor Sponsor = db.Sponsors.Find(id);
            if (Sponsor == null)
            {
                return NotFound();
            }

            db.Sponsors.Remove(Sponsor);
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

        /// <summary>
        /// Finds a Sponsor in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Sponsor id</param>
        /// <returns>TRUE if the Sponsor exists, false otherwise.</returns>
        private bool SponsorExists(int id)
        {
            return db.Sponsors.Count(e => e.SponsorID == id) > 0;
        }
    }
}
