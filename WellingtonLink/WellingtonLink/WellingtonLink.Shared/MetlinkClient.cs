using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace WellingtonLink
{
    class MetlinkClient
    {
        public async Task<List<ScheduleEntryModel>> GetScheduleFromStop(string stop)
        {
            List<ScheduleEntryModel> schedule = new List<ScheduleEntryModel>();
            string htmldata;
            //http://m.metlink.org.nz/stop/5006/departures
            var str = "http://m.metlink.org.nz/stop/" + stop + "/departures";
            Debug.WriteLine("Here is the schedule for stop: " + stop);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(str));

                if (response.IsSuccessStatusCode)
                {
                    htmldata = await response.Content.ReadAsStringAsync();
                    //return htmldata;
                }
                else
                {
                    // problems handling here
                    htmldata = null;
                    return null;
                }
            }


            var source = WebUtility.HtmlDecode(htmldata);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);

            HtmlNode toftitle = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "table")).ToList()[0];

            var entries = toftitle.Descendants();

            var entry = toftitle.Descendants().Where(x => (x.Name == "tr")).ToList();

            foreach (var tr in entry)
            {
                var scheduleEntry = new ScheduleEntryModel();
                var busNumber = tr.GetAttributeValue("data-code", null);
                var time = tr.Descendants().Where(x =>
                    (x.Name == "td" && x.Attributes["class"] != null &&
                    x.Attributes["class"].Value.Contains("time"))).ToList();

                var destItem = tr.Descendants().Where(x =>
                    (x.Name == "a" && x.Attributes["class"] != null &&
                    x.Attributes["class"].Value.Contains("rt-service-destination"))).ToList();

                if ((time.Count > 0) && (destItem.Count > 0))
                {
                    var preTime = time[0].InnerText;
                    var preDest = destItem[0].InnerText;


                    string timeToPrint = Regex.Replace(preTime, @"\r\n?|\n|\t", "");
                    string destination = Regex.Replace(preDest, @"\r\n?|\n|\t", "");
                    scheduleEntry.BusNumber = busNumber;

                    scheduleEntry.Destination = destination;
                    scheduleEntry.Time = timeToPrint;
                    Debug.WriteLine("Bus Number:" + busNumber + " ");
                    Debug.WriteLine("Time of arrival: " + timeToPrint);
                }
                else
                {
                    Debug.WriteLine("Nope");
                }

                schedule.Add(scheduleEntry);
            }
            return schedule;
        }




        public async Task<string> DoStuff(string stop)
        {
            string htmldata;
            Debug.WriteLine("Which bus stop?");
            //http://m.metlink.org.nz/stop/5006/departures
            var str = "http://m.metlink.org.nz/stop/" + stop + "/departures";
            Debug.WriteLine("Here is the schedule for stop: " + stop);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(new Uri(str));

                if (response.IsSuccessStatusCode)
                {
                    htmldata = await response.Content.ReadAsStringAsync();
                    //return htmldata;
                }
                else
                {
                    // problems handling here
                    htmldata = null;
                    return htmldata;
                }
            }


            var source = WebUtility.HtmlDecode(htmldata);
            HtmlDocument resultat = new HtmlDocument();
            resultat.LoadHtml(source);

            HtmlNode toftitle = resultat.DocumentNode.Descendants().Where
            (x => (x.Name == "table")).ToList()[0];

            var entries = toftitle.Descendants();

            var entry = toftitle.Descendants().Where(x => (x.Name == "tr")).ToList();

            foreach (var tr in entry)
            {
                var busNumber = tr.GetAttributeValue("data-code", null);
                var time = tr.Descendants().Where(x =>
                    (x.Name == "td" && x.Attributes["class"] != null &&
                    x.Attributes["class"].Value.Contains("time"))).ToList();

                if (time.Count > 0)
                {
                    var preTime = time[0].InnerText;


                    string timeToPrint = Regex.Replace(preTime, @"\r\n?|\n|\t", "");

                    Debug.WriteLine("Bus Number:" + busNumber + " ");
                    Debug.WriteLine("Time of arrival: " + timeToPrint);
                }
                else
                {
                    Debug.WriteLine("Nope");
                }
            }
            Debug.WriteLine("Press any key to quit...");
            return "Bus!";
        }
    }
}
