using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace varsity_w_auth.Models
{
    public class Sponsor
    {
        [Key]
        public int SponsorID { get; set; }
        [Required]
        public string SponsorName { get; set; }
        [Required]
        public string SponsorURL { get; set; }

        //Gold, Silver, Platinum etc.
        public int SponsorLevel { get; set; }


        //Utilizes the inverse property to specify the "Many"
        //https://www.entityframeworktutorial.net/code-first/inverseproperty-dataannotations-attribute-in-code-first.aspx
        //One Sponsor Many Teams
        public ICollection<Team> Teams { get; set; }
    }

    public class SponsorDto
    {
        public int SponsorID { get; set; }

        [Required(ErrorMessage="Please Enter a Name")]
        public string SponsorName { get; set; }
        [Required(ErrorMessage ="Please Enter a URL.")]
        public string SponsorURL { get; set; }
    }
}