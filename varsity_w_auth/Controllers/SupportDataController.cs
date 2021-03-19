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
        public IHttpActionResult GetSupportsForTeam(int id)
        {
            IEnumerable<Support> SupportMessages = db.Supports.Where(s=>s.TeamID == id);
            List<SupportDto> SupportMessageDtos = new List<SupportDto>() { };

            foreach(var Message in SupportMessages)
            {
                SupportDto MessageDto = new SupportDto { 
                    SupportID = Message.SupportID,
                    SupportMessage = Message.SupportMessage,
                    SupportDate = Message.SupportDate,
                    DSupportDate = Message.SupportDate.ToString("MMM d yyyy"),
                    UserName = Message.ApplicationUser.UserName
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
        public IHttpActionResult GetSupportsForUser(string id)
        {
            IEnumerable<Support> SupportMessages = db.Supports.Where(s => s.Id == id);
            List<SupportDto> SupportMessageDtos = new List<SupportDto>() { };

            foreach (var Message in SupportMessages)
            {
                SupportDto MessageDto = new SupportDto
                {
                    SupportID = Message.SupportID,
                    SupportMessage = Message.SupportMessage,
                    SupportDate = Message.SupportDate,
                    DSupportDate = Message.SupportDate.ToString("MMM d yyyy"),
                    UserName = Message.ApplicationUser.UserName
                };
                SupportMessageDtos.Add(MessageDto);
            }

            return Ok(SupportMessageDtos);
        }

    }
}
