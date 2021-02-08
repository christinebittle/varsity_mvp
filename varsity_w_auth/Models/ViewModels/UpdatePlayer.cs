using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    //The View Model required to update a player
    public class UpdatePlayer
    {
        //Information about the player
        public PlayerDto player { get; set; }
        //Needed for a dropdownlist which presents the player with a choice of teams to play for
        public IEnumerable<TeamDto> allteams { get; set; }
    }
}