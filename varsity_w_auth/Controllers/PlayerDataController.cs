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
    public class PlayerDataController : ApiController
    {
        //This variable is our database access point
        private VarsityDataContext db = new VarsityDataContext();

        //This code is mostly scaffolded from the base models and database context
        //New > WebAPIController with Entity Framework Read/Write Actions
        //Choose model "Player"
        //Choose context "Varsity Data Context"
        //Note: The base scaffolded code needs many improvements for a fully
        //functioning MVP.


        /// <summary>
        /// Gets a list or players in the database alongside a status code (200 OK).
        /// </summary>
        /// <returns>A list of players including their ID, bio, first name, last name, and teamid.</returns>
        /// <example>
        /// GET: api/PlayerData/GetPlayers
        /// </example>
        [ResponseType(typeof(IEnumerable<PlayerDto>))]
        public IHttpActionResult GetPlayers()
        {
            List<Player> Players = db.Players.ToList();
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
        /// Finds a particular player in the database with a 200 status code. If the player is not found, return 404.
        /// </summary>
        /// <param name="id">The player id</param>
        /// <returns>Information about the player, including player id, bio, first and last name, and teamid</returns>
        // <example>
        // GET: api/PlayerData/FindPlayer/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(PlayerDto))]
        public IHttpActionResult FindPlayer(int id)
        {
            //Find the data
            Player Player = db.Players.Find(id);
            //if not found, return 404 status code.
            if (Player == null)
            {
                return NotFound();
            }

            //put into a 'friendly object format'
            PlayerDto PlayerDto = new PlayerDto
            {
                PlayerID = Player.PlayerID,
                PlayerBio = Player.PlayerBio,
                PlayerFirstName = Player.PlayerFirstName,
                PlayerLastName = Player.PlayerLastName,
                TeamID = Player.TeamID
            };


            //pass along data as 200 status code OK response
            return Ok(PlayerDto);
        }

        /// <summary>
        /// Finds a particular Team in the database given a player id with a 200 status code. If the Team is not found, return 404.
        /// </summary>
        /// <param name="id">The player id</param>
        /// <returns>Information about the Team, including Team id, bio, first and last name, and teamid</returns>
        // <example>
        // GET: api/TeamData/FindTeamForPlayer/5
        // </example>
        [HttpGet]
        [ResponseType(typeof(TeamDto))]
        public IHttpActionResult FindTeamForPlayer(int id)
        {
            //Finds the first team which has any players
            //that match the input playerid
            Team Team = db.Teams
                .Where(t=> t.Players.Any(p=> p.PlayerID==id))
                .FirstOrDefault();
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
        /// Updates a player in the database given information about the player.
        /// </summary>
        /// <param name="id">The player id</param>
        /// <param name="player">A player object. Received as POST data.</param>
        /// <returns></returns>
        /// <example>
        /// POST: api/PlayerData/UpdatePlayer/5
        /// FORM DATA: Player JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdatePlayer(int id, [FromBody] Player player)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != player.PlayerID)
            {
                return BadRequest();
            }

            db.Entry(player).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerExists(id))
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
        /// Adds a player to the database.
        /// </summary>
        /// <param name="player">A player object. Sent as POST form data.</param>
        /// <returns>status code 200 if successful. 400 if unsuccessful</returns>
        /// <example>
        /// POST: api/PlayerData/AddPlayer
        ///  FORM DATA: Player JSON Object
        /// </example>
        [ResponseType(typeof(Player))]
        [HttpPost]
        public IHttpActionResult AddPlayer([FromBody] Player player)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Players.Add(player);
            db.SaveChanges();

            return Ok(player.PlayerID);
        }

        /// <summary>
        /// Deletes a player in the database
        /// </summary>
        /// <param name="id">The id of the player to delete.</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        /// <example>
        /// POST: api/PlayerData/DeletePlayer/5
        /// </example>
        [HttpPost]
        public IHttpActionResult DeletePlayer(int id)
        {
            Player player = db.Players.Find(id);
            if (player == null)
            {
                return NotFound();
            }

            db.Players.Remove(player);
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
        /// Finds a player in the system. Internal use only.
        /// </summary>
        /// <param name="id">The player id</param>
        /// <returns>TRUE if the player exists, false otherwise.</returns>
        private bool PlayerExists(int id)
        {
            return db.Players.Count(e => e.PlayerID == id) > 0;
        }
    }

}
