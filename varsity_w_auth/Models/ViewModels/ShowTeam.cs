using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ShowTeam
    {
        //Information about the team
        public TeamDto team { get; set; }

        //Information about all players on that team
        public IEnumerable<PlayerDto> teamplayers { get; set; }

        //Information about all sponsors for that team
        public IEnumerable<SponsorDto> teamsponsors { get; set; }

        //Information about the sport associated with the team
        public Sport sport { get; set; }

    }
}