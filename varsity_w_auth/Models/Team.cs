using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace varsity_w_auth.Models
{
    public class Team
    {
        public int TeamID { get; set; }

        public string TeamName { get; set; }

        public string TeamBio { get; set; }

        //A team can have many players
        public ICollection<Player> Players { get; set; }

        //A team can have many sponsors
        public ICollection<Sponsor> Sponsors { get; set; }

        //A team is associated with one sport
        [ForeignKey("Sport")]
        public int? SportID { get; set; }
        public virtual Sport Sport { get; set; }
    }

    public class TeamDto
    {
        public int TeamID { get; set; }

        [DisplayName("Team Name")]
        public string TeamName { get; set; }
        [DisplayName("Team Bio")]
        public string TeamBio { get; set; }
    }
}