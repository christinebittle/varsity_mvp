using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace varsity_w_auth.Models
{
    public class Support
    {
        //represents a message of support which is left by a user on a team.
        //used to demonstrate semi-access to a resource. If you own it, you can delete/modify it.

        [Key]
        public int SupportID { get; set; }

        //the content of the supporting message
        public string SupportMessage { get; set; }

        //the date of the supporting message
        public DateTime SupportDate { get; set; }

        //the support message is for one team
        [ForeignKey("Team")]
        public int TeamID { get; set; }
        public virtual Team Team { get; set; }

        //the message is sent by one logged in user.
        [ForeignKey("ApplicationUser")]
        public string Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }

    public class SupportDto
    {
        //A data transfer object can also be used to 'flatten' a relationship.
        //when looking at a message of support, we would want to see
        //supportid, message, date, username
        public int SupportID { get; set; }
        public string SupportMessage { get; set; }
        public DateTime SupportDate { get; set; }

        public string DSupportDate { get; set; }
        public string UserName { get; set; }
    }
}