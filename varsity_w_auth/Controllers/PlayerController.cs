using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using varsity_w_auth.Models;
using varsity_w_auth.Models.ViewModels;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Web.Script.Serialization;


namespace varsity_w_auth.Controllers
{
    public class PlayerController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;
        
        
        static PlayerController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false,
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
            if (!User.Identity.IsAuthenticated) {client.DefaultRequestHeaders.Remove("Cookie"); return; }
            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: Player/List?{PageNum}
        // If the page number is not included, set it to 0
        public ActionResult List(int PageNum=0)
        {
            GetApplicationCookie();
            // Grab all players
            string url = "playerdata/getplayers";
            // Send off an HTTP request
            // GET : /api/playerdata/getplayers
            // Retrieve response
            HttpResponseMessage response = client.GetAsync(url).Result;
            // If the response is a success, proceed
            if (response.IsSuccessStatusCode)
            {
                // Fetch the response content into IEnumerable<PlayerDto>
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;

                // -- Start of Pagination Algorithm --

                // Find the total number of players
                int PlayerCount = SelectedPlayers.Count();
                // Number of players to display per page
                int PerPage = 8;
                // Determines the maximum number of pages (rounded up), assuming a page 0 start.
                int MaxPage = (int)Math.Ceiling((decimal)PlayerCount / PerPage) - 1;

                // Lower boundary for Max Page
                if (MaxPage < 0) MaxPage = 0;
                // Lower boundary for Page Number
                if (PageNum < 0) PageNum = 0;
                // Upper Bound for Page Number
                if (PageNum > MaxPage) PageNum = MaxPage;

                // The Record Index of our Page Start
                int StartIndex = PerPage * PageNum;

                //Helps us generate the HTML which shows "Page 1 of ..." on the list view
                ViewData["PageNum"] = PageNum;
                ViewData["PageSummary"] = " "+(PageNum + 1) + " of " + (MaxPage + 1)+" ";

                // -- End of Pagination Algorithm --


                // Send back another request to get players, this time according to our paginated logic rules
                // GET api/playerdata/getplayerspage/{startindex}/{perpage}
                url = "playerdata/getplayerspage/" + StartIndex + "/" + PerPage;
                response = client.GetAsync(url).Result;

                // Retrieve the response of the HTTP Request
                IEnumerable<PlayerDto> SelectedPlayersPage = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;

                //Return the paginated of players instead of the entire list
                return View(SelectedPlayersPage);

            }
            else
            {
                // If we reach here something went wrong with our list algorithm
                return RedirectToAction("Error");
            }

            
        }

        // GET: Player/Details/5
        public ActionResult Details(int id)
        {
            ShowPlayer ViewModel = new ShowPlayer();
            string url = "playerdata/findplayer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                PlayerDto SelectedPlayer = response.Content.ReadAsAsync<PlayerDto>().Result;
                ViewModel.player = SelectedPlayer;

                
                url = "playerdata/findteamforplayer/" + id;
                response = client.GetAsync(url).Result;
                TeamDto SelectedTeam = response.Content.ReadAsAsync<TeamDto>().Result;
                ViewModel.team = SelectedTeam;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Player/Create
        public ActionResult Create()
        {
            UpdatePlayer ViewModel = new UpdatePlayer();
            //get information about teams this player COULD play for.
            string url = "teamdata/getteams";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<TeamDto> PotentialTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
            ViewModel.allteams = PotentialTeams;

            return View(ViewModel);
        }

        // POST: Player/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Create(Player PlayerInfo)
        {
            Debug.WriteLine(PlayerInfo.PlayerFirstName);
            string url = "playerdata/addplayer";
            Debug.WriteLine(jss.Serialize(PlayerInfo));
            HttpContent content = new StringContent(jss.Serialize(PlayerInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url,content).Result;

            if (response.IsSuccessStatusCode)
            {
                
                int playerid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id=playerid });
            }
            else
            {
                return RedirectToAction("Error");
            }
            
            
        }

        // GET: Player/Edit/5
        public ActionResult Edit(int id)
        {
            UpdatePlayer ViewModel = new UpdatePlayer();

            string url = "playerdata/findplayer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                PlayerDto SelectedPlayer = response.Content.ReadAsAsync<PlayerDto>().Result;
                ViewModel.player = SelectedPlayer;

                //get information about teams this player COULD play for.
                url = "teamdata/getteams";
                response = client.GetAsync(url).Result;
                IEnumerable<TeamDto> PotentialTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                ViewModel.allteams = PotentialTeams;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Player/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Player PlayerInfo, HttpPostedFileBase PlayerPic)
        {
            //Debug.WriteLine(PlayerInfo.PlayerFirstName);
            string url = "playerdata/updateplayer/"+id;
            Debug.WriteLine(jss.Serialize(PlayerInfo));
            HttpContent content = new StringContent(jss.Serialize(PlayerInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                
                //only attempt to send player picture data if we have it
                if (PlayerPic != null) {
                    Debug.WriteLine("Calling Update Image method.");
                    //Send over image data for player
                    url = "playerdata/updateplayerpic/"+id;
                    //Debug.WriteLine("Received player picture "+PlayerPic.FileName);

                    MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                    HttpContent imagecontent = new StreamContent(PlayerPic.InputStream);
                    requestcontent.Add(imagecontent,"PlayerPic",PlayerPic.FileName);
                    response = client.PostAsync(url, requestcontent).Result;
                }

                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Player/Delete/5
        [HttpGet]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "playerdata/findplayer/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into player data transfer object
                PlayerDto SelectedPlayer = response.Content.ReadAsAsync<PlayerDto>().Result;
                return View(SelectedPlayer);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Player/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Delete(int id)
        {
            string url = "playerdata/deleteplayer/" + id;
            //post body is empty
            HttpContent content = new StringContent("");
            HttpResponseMessage response = client.PostAsync(url,content).Result;
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
