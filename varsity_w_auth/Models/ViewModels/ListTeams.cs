using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ListTeams
    {

        //Pass this flag to conditionally render update/new links
        public bool isadmin { get; set; }
        public IEnumerable<TeamDto> teams { get; set; }
    }
}