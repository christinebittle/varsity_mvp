﻿using System;
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

namespace varsity_w_auth.Controllers
{
    public class SponsorController : Controller
    {
        //Http Client is the proper way to connect to a webapi
        //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0

        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private static readonly HttpClient client;

        /// <summary>
        /// This allows us to access a pre-defined C# HttpClient 'client' variable for sending POST and GET requests to the data access layer.
        /// </summary>
        static SponsorController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("http://varsityproject-env.eba-f83mxjuc.us-east-2.elasticbeanstalk.com/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));


            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

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

        // GET: Sponsor/List
        public ActionResult List()
        {
            ListSponsors ViewModel = new ListSponsors();
            ViewModel.isadmin = User.IsInRole("Admin");

            string url = "sponsordata/getsponsors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<SponsorDto> SelectedSponsors = response.Content.ReadAsAsync<IEnumerable<SponsorDto>>().Result;
                ViewModel.sponsors = SelectedSponsors;
                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Sponsor/Details/5
        public ActionResult Details(int id)
        {
            UpdateSponsor ViewModel = new UpdateSponsor();
            ViewModel.isadmin = User.IsInRole("Admin");


            string url = "sponsordata/findsponsor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Sponsor data transfer object
                SponsorDto SelectedSponsor = response.Content.ReadAsAsync<SponsorDto>().Result;
                ViewModel.sponsor = SelectedSponsor;

                //find teams that are sponsored by this sponsor
                url = "sponsordata/getteamsforsponsor/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Sponsor data transfer object
                IEnumerable<TeamDto> SelectedTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                ViewModel.sponsoredteams = SelectedTeams;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }

        }

        // GET: Sponsor/Create
        [Authorize(Roles="Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Sponsor/Create
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Create(Sponsor SponsorInfo)
        {
            //pass along credentials to api
            GetApplicationCookie();

            Debug.WriteLine(SponsorInfo.SponsorName);
            string url = "sponsordata/addsponsor";
            Debug.WriteLine(jss.Serialize(SponsorInfo));
            HttpContent content = new StringContent(jss.Serialize(SponsorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int Sponsorid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = Sponsorid });
            }
            else
            {
                return RedirectToAction("Error");
            }


        }

        // GET: Sponsor/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {

            UpdateSponsor ViewModel = new UpdateSponsor();

            string url = "sponsordata/findsponsor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Sponsor data transfer object
                SponsorDto SelectedSponsor = response.Content.ReadAsAsync<SponsorDto>().Result;
                ViewModel.sponsor = SelectedSponsor;

                //find teams that are sponsored by this sponsor
                url = "sponsordata/getteamsforsponsor/" + id;
                response = client.GetAsync(url).Result;

                //Put data into Sponsor data transfer object
                IEnumerable<TeamDto> SelectedTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                ViewModel.sponsoredteams = SelectedTeams;

                //find teams that are not sponsored by this sponsor
                url = "sponsordata/getteamsnotsponsored/" + id;
                response = client.GetAsync(url).Result;

                //put data into data transfer object
                IEnumerable<TeamDto> UnsponsoredTeams = response.Content.ReadAsAsync<IEnumerable<TeamDto>>().Result;
                ViewModel.allteams = UnsponsoredTeams;

                return View(ViewModel);
            }
            else
            {
                return RedirectToAction("Error");

            }
        }

        /// <summary>
        /// This method collects the required information to remove an association between a team and a sponsor.
        /// </summary>
        /// <param name="teamid">The team to be unassociated with the sponsor</param>
        /// <param name="sponsorid">The sponsor to be unassociated with the team</param>
        [HttpGet]
        [Route("Sponsor/Unsponsor/{teamid}/{sponsorid}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Unsponsor(int teamid, int sponsorid)
        {
            //pass along credentials to api
            GetApplicationCookie();

            string url = "sponsordata/unsponsor/" + teamid + "/" + sponsorid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = sponsorid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// This method collects the required information to add an association between a team and a sponsor.
        /// </summary>
        /// <param name="teamid">The team to be sponsored</param>
        /// <param name="sponsorid">The sponsor to sponsor the team.</param>
        [HttpPost]
        [Route("Sponsor/sponsor/{teamid}/{sponsorid}")]
        [Authorize(Roles = "Admin")]
        public ActionResult Sponsor(int teamid, int sponsorid)
        {
            //pass along credentials to api
            GetApplicationCookie();

            string url = "sponsordata/sponsor/" + teamid + "/" + sponsorid;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Edit", new { id = sponsorid });
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Sponsor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Sponsor SponsorInfo)
        {
            //pass along credentials to api
            GetApplicationCookie();

            Debug.WriteLine(SponsorInfo.SponsorName);
            string url = "sponsordata/updatesponsor/" + id;
            Debug.WriteLine(jss.Serialize(SponsorInfo));
            HttpContent content = new StringContent(jss.Serialize(SponsorInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("update sponsor request succeeded");
                return RedirectToAction("Details", new { id = id });
            }
            else
            {
                Debug.WriteLine("update sponsor request failed");
                Debug.WriteLine(response.StatusCode.ToString());
                return RedirectToAction("Error");
            }
        }

        // GET: Sponsor/Delete/5
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirm(int id)
        {
            string url = "sponsordata/findsponsor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            //Can catch the status code (200 OK, 301 REDIRECT), etc.
            //Debug.WriteLine(response.StatusCode);
            if (response.IsSuccessStatusCode)
            {
                //Put data into Sponsor data transfer object
                SponsorDto SelectedSponsor = response.Content.ReadAsAsync<SponsorDto>().Result;
                return View(SelectedSponsor);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Sponsor/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            //pass along credentials to api
            GetApplicationCookie();

            string url = "sponsordata/deletesponsor/" + id;
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