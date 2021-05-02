using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace BookStore.MyWebScraper
{
    public class WebScraper
    {
        public double Price { get; set; }

        public double GetPrice (string isbn)
        {
            var htmlDoc = GetHtml(isbn);

            Price = Double.Parse(GetScrapedPrice(htmlDoc));

            return Price;
        }
        static HtmlDocument GetHtml(string isbn)
        {
            //string url = $"https://www.amazon.com/s?i=stripbooks&rh=p_66%3A{isbn}&s=relevanceexprank&Adv-Srch-Books-Submit.x=11&Adv-Srch-Books-Submit.y=13&unfiltered=1&ref=sr_adv_b";
            string url = $"https://www.ebay.com/sch/i.html?_nkw={isbn}&_in_kw=1&_ex_kw=&_sacat=267&_udlo=&_udhi=&_ftrt=901&_ftrv=1&_sabdlo=&_sabdhi=&_samilow=&_samihi=&_sadis=15&_stpos=64093&_sargn=-1%26saslc%3D1&_salic=1&_sop=12&_dmd=1&_ipg=200&_fosrp=1";
            //var httpClient = new HttpClient();

            //string html = httpClient.GetStringAsync(url).Result; //loads the entire html page of the url

            //var htmlDocument = new HtmlDocument(); //a document to save html of url
            //htmlDocument.LoadHtml(html); //loads the html to the document
            HtmlWeb web = new HtmlWeb();
            HtmlDocument htmlDocument = web.Load(url);

            return htmlDocument;   
        }

        static string GetScrapedPrice(HtmlDocument htmlDocument)
        {
            var priceList = htmlDocument.DocumentNode.Descendants("li")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("lvprice prc"))
                .ToList();
            

            if(priceList.Count>0)
            {
                var targetPrice = priceList[0].Descendants("span")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("bold"));

                string itemPrice = targetPrice
                .FirstOrDefault()
                .InnerHtml
                .TrimStart()
                .Substring(1);

                return itemPrice;
            }

            else
            {
                //if book not found on amazon
                return "0";
            }
        }
    }
}
