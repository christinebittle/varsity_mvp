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
using Microsoft.AspNet.Identity;

namespace varsity_w_auth.Controllers
{
    public class UserDataController : ApiController
    {
        //This variable is our database access point
        private VarsityDataContext db = new VarsityDataContext();

        /// <summary>
        /// Finds a user by their id (string)
        /// </summary>
        /// <param name="id">The input ID of the user</param>
        /// <returns>Status code of 200, along with base profile information about the user.</returns>
        [Route("api/userdata/finduser/{id}")]
        [HttpGet]
        public IHttpActionResult FindUser(string id)
        {
            //Note: this method is currently public. Could be useful if we had a feature where people can navigate to other's profiles.
            ApplicationUser SelectedUser = db.Users.Where(u => u.Id == id).First();

            //benefit of the Dto to limit exposure to the very sensitive ApplicationUser object!
            ApplicationUserDto User = new ApplicationUserDto()
            {
                id=SelectedUser.Id,
                NickName=SelectedUser.NickName
                //could expose any information about the user that we wished here.
            };

            return Ok(SelectedUser);
        }

        /// <summary>
        /// Updates information about a user in the database
        /// </summary>
        /// <param name="">The user information to update</param>
        /// <returns>Status result of 200 OK if successful</returns>
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateUser([FromBody] ApplicationUserDto UserData)
        {
            //only the user can adjust their profile.
            string userid = User.Identity.GetUserId();
            if (userid != UserData.id) return Unauthorized();

            ApplicationUser SelectedUser = db.Users.Where(u => u.Id == userid).First();
            if (SelectedUser == null) return NotFound();

            SelectedUser.NickName = UserData.NickName;


            db.SaveChanges();


            return Ok();
        }

    }
}
