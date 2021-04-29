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
using Microsoft.AspNet.Identity;

namespace varsity_w_auth.Controllers
{
    public class SupportDataController : ApiController
    {
        //This variable is our database access point
        private VarsityDataContext db = new VarsityDataContext();

        //Why doesn't this class have a 'SupportController'?
        //The data access layer is the critical element. Records can be added/removed on the Team Details View.
        
        

        /// <summary>
        /// Returns a list of Support Messages for a given team.
        /// </summary>
        /// <param name="id">The input team id</param>
        /// <returns>A list of support messages for that team</returns>
        /// <example>
        /// GET api/SupportData/GetSupportsForTeam/2
        /// </example>
        public IHttpActionResult GetSupportsForTeam(int id)
        {
            

            IEnumerable<Support> SupportMessages = db.Supports.Where(s=>s.TeamID == id).ToList();
            List<SupportDto> SupportMessageDtos = new List<SupportDto>() { };

            foreach(var Message in SupportMessages)
            {
                SupportDto MessageDto = new SupportDto { 
                    SupportID = Message.SupportID,
                    SupportMessage = Message.SupportMessage,
                    SupportDate = Message.SupportDate,
                    DSupportDate = Message.SupportDate.ToString("MMM d yyyy"),
                    UserName = Message.ApplicationUser.NickName,
                    Id = Message.Id
                };
                SupportMessageDtos.Add(MessageDto);
            }

            return Ok(SupportMessageDtos);
        }

        /// <summary>
        /// Returns a list of Support Messages for a given user.
        /// </summary>
        /// <param name="id">The input UserID (string)</param>
        /// <returns>A list of support messages written by that user</returns>
        /// <example>
        /// GET api/SupportData/GetSupportsForUser/abcedf-12345-ghijkl
        /// </example>
        public IHttpActionResult GetSupportsForUser(string id)
        {
            IEnumerable<Support> SupportMessages = db.Supports.Where(s => s.Id == id).ToList();
            List<SupportDto> SupportMessageDtos = new List<SupportDto>() { };

            foreach (var Message in SupportMessages)
            {
                SupportDto MessageDto = new SupportDto
                {
                    SupportID = Message.SupportID,
                    SupportMessage = Message.SupportMessage,
                    SupportDate = Message.SupportDate,
                    DSupportDate = Message.SupportDate.ToString("MMM d yyyy"),
                    UserName = Message.ApplicationUser.NickName,
                    TeamID = Message.TeamID,
                    TeamName=Message.Team.TeamName,
                    Id = Message.Id
                };
                SupportMessageDtos.Add(MessageDto);
            }

            return Ok(SupportMessageDtos);
        }

        /// <summary>
        /// Adds a new support message to the database
        /// </summary>
        /// <param name="TeamSupportMessage">The message of support</param>
        /// <returns>Status code of 200(ok) along with the newly inserted Support ID</returns>
        /// <example>
        /// POST api/SupportData/AddSupport
        /// FORM DATA: JSON support object
        /// </example>
        [HttpPost]
        //both admin and fan roles are allowed to add support
        [Authorize(Roles="Admin,Fan")]
        public IHttpActionResult AddSupport(Support TeamSupportMessage)
        {
            //Will Validate according to data annotations specified on model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            db.Supports.Add(TeamSupportMessage);
            db.SaveChanges();

            return Ok(TeamSupportMessage.SupportID);
        }

        /// <summary>
        /// Removes a particular Message of Support.
        /// </summary>
        /// <param name="id">The supporting message to remove</param>
        /// <returns>200 if successful. 404 if not successful.</returns>
        [HttpPost]
        [Authorize(Roles="Admin,Fan")]
        public IHttpActionResult DeleteSupport(int id)
        {

            Support TeamSupportMessage = db.Supports.Find(id);
            if (TeamSupportMessage == null)
            {
                return NotFound();
            }

            //An admin can delete any message of support.
            //A fan can only delete a message of support if their ID matches the record.
            if (!User.IsInRole("Admin"))
            {
                if (User.Identity.GetUserId() != TeamSupportMessage.Id) return Unauthorized();
            }

            db.Supports.Remove(TeamSupportMessage);
            db.SaveChanges();

            return Ok();
        }

    }
}
