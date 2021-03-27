using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ListPlayers
    {
        //Our View needs to conditionally render the page based on admin or non admin.
        //Admin will see "Create New" and "Edit" links, non-admin will not see these.
        public bool isadmin { get; set; }

        public IEnumerable<PlayerDto> players { get; set; }
    }
}