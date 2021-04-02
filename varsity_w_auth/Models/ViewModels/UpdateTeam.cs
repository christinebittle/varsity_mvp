using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class UpdateTeam
    {
        //information about the team itself
        public TeamDto team { get; set; }

        //information about possible sports this team could play for
        public IEnumerable<SportDto> sports { get; set; }
    }
}