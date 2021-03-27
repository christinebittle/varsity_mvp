using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ShowTeam
    {
        //Conditionally render update/delete links if admin
        public bool isadmin { get; set; }

        //conditionally render 'add new support' message if fan
        public bool isfan { get; set; }

        //Conditionally render support delete links depending on the fans user id
        public string userid { get; set; }

        //Information about the team
        public TeamDto team { get; set; }

        //Information about all players on that team
        public IEnumerable<PlayerDto> teamplayers { get; set; }

        //Information about all sponsors for that team
        public IEnumerable<SponsorDto> teamsponsors { get; set; }

        //Information about all support messages for that team
        public IEnumerable<SupportDto> supportmessages { get; set; }

        //Information about the sport associated with the team
        public Sport sport { get; set; }

    }
}