using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace varsity_w_auth.Models
{
    //This class describes a player entity.
    //It is used for actually generating a database table
    public class Player
    {
        [Key]
        public int PlayerID { get; set; }

        public string PlayerFirstName { get; set; }

        public string PlayerLastName { get; set; }

        [AllowHtml]
        public string PlayerBio { get; set; }

        public bool PlayerHasPic { get; set; }

        //If the player has an image, record the extension of the image (.png, .gif, .jpg, etc.)
        public string PicExtension { get; set; }

        //Foreign keys in Entity Framework
        /// https://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx

        //A player plays for one team
        [ForeignKey("Team")]
        public int TeamID { get; set; }
        public virtual Team Team { get; set; }
    }

    //This class can be used to transfer information about a player.
    //also known as a "Data Transfer Object"
    //https://docs.microsoft.com/en-us/aspnet/web-api/overview/data/using-web-api-with-entity-framework/part-5
    //You can think of this class as the 'Model' that was used in 5101.
    //It is simply a vessel of communication
    public class PlayerDto
    {
        public int PlayerID { get; set; }
        [DisplayName("First Name")]
        public string PlayerFirstName { get; set; }
        [DisplayName("Last Name")]
        public string PlayerLastName { get; set; }
        [DisplayName("Player Bio")]
        public string PlayerBio { get; set; }

        public bool PlayerHasPic { get; set; }
        public string PicExtension { get; set; }

        public int TeamID { get; set; }
    }
}