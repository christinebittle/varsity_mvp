using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ShowPlayer
    {
        //Conditionally render the page depending on if the admin is logged in.
        public bool isadmin { get; set; }
        public PlayerDto player { get; set; }
        //information about the team the player plays for
        public TeamDto team { get; set; }
    }
}