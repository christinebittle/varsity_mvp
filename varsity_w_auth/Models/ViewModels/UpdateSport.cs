using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class UpdateSport
    {

        //conditionally render depending if the user is an admin or not
        public bool isadmin { get; set; }

        public SportDto sport { get; set; }

        //teams that play this sport
        public IEnumerable<TeamDto> teams { get; set; }
    }
}