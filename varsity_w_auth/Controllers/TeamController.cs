using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using varsity_w_auth.Models;
using varsity_w_auth.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;
using Microsoft.AspNet.Identity;

namespace varsity_w_auth.Controllers
{
    public class TeamController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        /// <summary>
        /// This allows us to access a pre-defined C# HttpClient 'client' variable for sending POST and GET requests to the data access layer.
        /// </summary>
        static TeamController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44334/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

        }


        /// <summary>
        /// Grabs the authentication credentials which are sent to the Controller.
        /// This is NOT considered a proper authentication technique for the WebAPI. It piggybacks the existing authentication set up in the template for Individual User Accounts. Considering the existing scope and complexity of the course, it works for now.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }



        // GET: Team/List
        public ActionResult List()
        {
            ListTeams ViewModel = new ListTeams();
            ViewModel.isadmin = User.IsInRole("Admin");

            string url = "teamdata/getteams";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<TeamDto> SelectedTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                ViewModel.teams = SelectedTeams;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Team/Details/5
        public ActionResult Details(int id)
        {
            ShowTeam ViewModel = new ShowTeam();
            ViewModel.isadmin = User.IsInRole("Admin");
            ViewModel.isfan = User.IsInRole("Fan");
            ViewModel.userid = User.Identity.GetUserId();

            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                ViewModel.team = SelectedTeam;

                //Grab the sport associated with this team
                url = "TeamData/FindSportForTeam/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                SportDto Sport = response.Content.ReadAsAsync<SportDto>().Result;
                ViewModel.sport = Sport;

                //We don't need to throw any errors if this is null
                //A team not having any players is not an issue.
                url = "teamdata/getplayersforteam/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                ViewModel.teamplayers = SelectedPlayers;


                url = "teamdata/getsponsorsforteam/" + id;
                response = client.GetAsync(url).Result;
                //Can catch the status code (200 OK, 301 REDIRECT), etc.
                //Debug.WriteLine(response.StatusCode);
                //Put data into Team data transfer object
                IEnumerable<SponsorDto> SelectedSponsors = response.Content.ReadAsAsync<IEnumerable<SponsorDto>>().Result;
                ViewModel.teamsponsors = SelectedSponsors;

                //Grab the messages of support for this team
                url = "supportdata/getsupportsforteam/" + id;
                response = client.GetAsync(url).Result;
                IEnumerable<SupportDto> SupportMessages = response.Content.ReadAsAsync<IEnumerable<SupportDto>>().Result;
                ViewModel.supportmessages = SupportMessages;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Adds a Supporting Message to the Team. Add form in /Team/Details
        /// </summary>
        /// <param name="id">The Team ID</param>
        /// <param name="message">The supporting message</param>
        [HttpPost]
        [Authorize(Roles="Admin,Fan")]
        public ActionResult AddSupport(int id, string message)
        {
            //pass authorization up to the data access layer
            GetApplicationCookie();

            //create a model with the information that we need to send
            Support TeamSupportMessage = new Support() {
                SupportMessage = message,
                SupportDate = DateTime.Now,
                TeamID = id,
                Id = User.Identity.GetUserId()
            };
            

            Debug.WriteLine("Message of Support is "+TeamSupportMessage.SupportMessage);
            Debug.WriteLine("Recipient Team ID is "+TeamSupportMessage.TeamID);
            Debug.WriteLine("User ID submitting message is "+TeamSupportMessage.Id);

            string url = "supportdata/addsupport";
            Debug.WriteLine(jss.Serialize(TeamSupportMessage));
            HttpContent content = new StringContent(jss.Serialize(TeamSupportMessage));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details", new {id = id});
        }

        /// <summary>
        /// Removes a Supporting Message from a Team.
        /// </summary>
        /// <param name="id">The Support Message ID</param>
        [HttpGet]
        [Route("Team/DeleteSupport/{supportid}/{teamid}")]
        [Authorize(Roles="Admin,Fan")]
        public ActionResult DeleteSupport(int supportid, int teamid)
        {
            GetApplicationCookie();

            string url = "supportdata/deletesupport/"+supportid;
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url,content).Result;
            
            return RedirectToAction("Details", new { id = teamid });
        }

        // GET: Team/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            UpdateTeam ViewModel = new UpdateTeam();
            //get information about sports this team could be about.
            string url = "sportdata/getsports";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<SportDto> PotentialSports = response.Content.ReadAsAsync<IEnumerable<SportDto>>().Result;
            ViewModel.sports = PotentialSports;

            return View(ViewModel);
        }

        // POST: Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Team TeamInfo)
        {
            GetApplicationCookie();

            Debug.WriteLine(TeamInfo.TeamName);
            string url = "Teamdata/addTeam";
            Debug.WriteLine(jss.Serialize(TeamInfo));
            HttpContent content = new StringContent(jss.Serialize(TeamInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Teamid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Teamid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Team/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            UpdateTeam ViewModel = new UpdateTeam();
            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                //Put data into Team object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                ViewModel.team = SelectedTeam;

                //get information about sports this team could be about.
                url = "sportdata/getsports";
                response = client.GetAsync(url).Result;
                IEnumerable<SportDto> PotentialSports = response.Content.ReadAsAsync<IEnumerable<SportDto>>().Result;
                ViewModel.sports = PotentialSports;


                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Team TeamInfo)
        {
            GetApplicationCookie();

            Debug.WriteLine(TeamInfo.TeamName);
            string url = "teamdata/updateteam/"+id;
            Debug.WriteLine(jss.Serialize(TeamInfo));
            HttpContent content = new StringContent(jss.Serialize(TeamInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Team/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "teamdata/findteam/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Team data transfer object
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                return View(SelectedTeam);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Team/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();
            string url = "teamdata/deleteteam/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {

                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        public ActionResult Error()
        {
            return View();
        }
    }
}