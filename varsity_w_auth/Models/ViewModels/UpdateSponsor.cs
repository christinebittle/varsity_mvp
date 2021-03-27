using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class UpdateSponsor
    {
        //this is also used for showsponsor

        //conditional rendering for update/delete links depending on if the user is logged in
        public bool isadmin { get; set; }

        //base information about the sponsor
        public SponsorDto sponsor { get; set; }
        //display all teams that this sponsor is sponsoring
        public IEnumerable<TeamDto> sponsoredteams { get; set; }
        //display teams which could be sponsored in a dropdownlist
        public IEnumerable<TeamDto> allteams { get; set; }
    }
}