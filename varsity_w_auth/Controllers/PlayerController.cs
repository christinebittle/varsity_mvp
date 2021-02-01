using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using varsity_w_auth.Models;
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
                AllowAutoRedirect = false
            };
            client = new HttpClient(handler);
            //change this to match your own local port number
            client.BaseAddress = new Uri("https://localhost:44334/api/");
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ACCESS_TOKEN);

        }



        // GET: Player
        public ActionResult List()
        {
            string url = "playerdata/getplayers";
            HttpResponseMessage response = client.GetAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<PlayerDto> SelectedPlayers = response.Content.ReadAsAsync<IEnumerable<PlayerDto>>().Result;
                return View(SelectedPlayers);
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Player/Details/5
        public ActionResult Details(int id)
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

        // GET: Player/Create
        public ActionResult Create()
        {
            return View();
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

        // POST: Player/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Edit(int id, Player PlayerInfo)
        {
            Debug.WriteLine(PlayerInfo.PlayerFirstName);
            string url = "playerdata/addplayer";
            Debug.WriteLine(jss.Serialize(PlayerInfo));
            HttpContent content = new StringContent(jss.Serialize(PlayerInfo));
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {

                int playerid = response.Content.ReadAsAsync<int>().Result;
                return RedirectToAction("Details", new { id = playerid });
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
