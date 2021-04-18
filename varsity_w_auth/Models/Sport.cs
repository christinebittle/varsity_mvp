using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace varsity_w_auth.Models
{
    public class Sport
    {

        [Key]
        public int SportID { get; set; }
        [Required]
        public string SportName { get; set; }

        //A sport can have many teams
        public ICollection<Team> Teams { get; set; }

    }

    public class SportDto
    {
        public int SportID { get; set; }
        [Required(ErrorMessage ="Please enter a Name.")]
        public string SportName { get; set; }

    }
}