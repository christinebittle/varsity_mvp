using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace varsity_w_auth.Models.ViewModels
{
    public class ListSports
    {
        //conditionally render update/delete links if admin or not
        public bool isadmin { get; set; }

        public IEnumerable<SportDto> sports { get; set; }
    }
}