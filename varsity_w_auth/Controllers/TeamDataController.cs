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
    public class TeamDataController : ApiController
    {
        //This variable is our database access point
        private VarsityDataContext db = new VarsityDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Team"
        //Choose context "Varsity Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.


        /// <summary>
        /// Gets a list or Teams in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of Teams including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/TeamData/GetTeams
        /// </example>
        [ResponseType(typeof(IEnumerable<TeamDto>))]
        public IHttpActionResult GetTeams()
        {
            List<Team> Teams = db.Teams.ToList();
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
        /// Gets a list of players in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input teamid</param>
        /// <returns>A list of players associated with the team</returns>
        /// <example>
        /// GET: api/TeamData/GetTeams
        /// </example>
        [ResponseType(typeof(IEnumerable<PlayerDto>))]
        public IHttpActionResult GetPlayersForTeam(int id)
        {
            List<Player> Players = db.Players.Where(p => p.TeamID==id)
                .ToList();
            List<PlayerDto> PlayerDtos = new List<PlayerDto> { };

            //Here you can choose which information is exposed to the API
            foreach (var Player in Players)
            {
                PlayerDto NewPlayer = new PlayerDto
                {
                    PlayerID = Player.PlayerID,
                    PlayerBio = Player.PlayerBio,
                    PlayerFirstName = Player.PlayerFirstName,
                    PlayerLastName = Player.PlayerLastName,
                    TeamID = Player.TeamID
                };
                PlayerDtos.Add(NewPlayer);
            }

            return Ok(PlayerDtos);
        }

        /// <summary>
        /// Gets a list or Sponsors in the database alongside a status code (200 OK).
        /// </summary>
        /// <param name="id">The input teamid</param>
        /// <returns>A list of Sponsors including their ID, name, and URL.</returns>
        /// <example>
        /// GET: api/SponsorData/GetSponsors
        /// </example>
        [ResponseType(typeof(IEnumerable<SponsorDto>))]
        public IHttpActionResult GetSponsorsForTeam(int id)
        {
            List<Sponsor> Sponsors = db.Sponsors
                .Where(s => s.Teams.Any(t => t.TeamID == id))
                .ToList();
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
        /// Finds a particular Team in the database with a 200 status code. If the Team is not found, return 404.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>Information about the Team, including Team id, bio, first and last name, and teamid</returns>
        // <example>
        // GET: api/TeamData/FindTeam/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(TeamDto))]
        public IHttpActionResult FindTeam(int id)
        {
            //Find the data
            Team Team = db.Teams.Find(id);
            //if not found, return 404 status code.
            if (Team == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            TeamDto TeamDto = new TeamDto
            {
                TeamID = Team.TeamID,
                TeamName = Team.TeamName,
                TeamBio = Team.TeamBio
            };


            //pass along data as 200 status code OK response
            return Ok(TeamDto);
        }

        /// <summary>
        /// Updates a Team in the database given information about the Team.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <param name="Team">A Team object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/TeamData/UpdateTeam/5
        /// FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateTeam(int id, [FromBody] Team Team)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Team.TeamID)
            {
                return BadRequest();
            }

            db.Entry(Team).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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
        /// Adds a Team to the database.
        /// </summary>
        /// <param name="Team">A Team object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/TeamData/AddTeam
        ///  FORM DATA: Team JSON Object
        /// </example>
        [ResponseType(typeof(Team))]
        [HttpPost]
        public IHttpActionResult AddTeam([FromBody] Team Team)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Teams.Add(Team);
            db.SaveChanges();

            return Ok(Team.TeamID);
        }

        /// <summary>
        /// Deletes a Team in the database
        /// </summary>
        /// <param name="id">The id of the Team to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/TeamData/DeleteTeam/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeleteTeam(int id)
        {
            Team Team = db.Teams.Find(id);
            if (Team == null)
            {
                return NotFound();
            }

            db.Teams.Remove(Team);
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
        /// Finds a Team in the system. Internal use only.
        /// </summary>
        /// <param name="id">The Team id</param>
        /// <returns>TRUE if the Team exists, false otherwise.</returns>
        private bool TeamExists(int id)
        {
            return db.Teams.Count(e => e.TeamID == id) > 0;
        }
    }
}
