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
        public string SportName { get; set; }

        

    }
}