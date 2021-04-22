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
        static ScrapingBrowser _scrapingBrowser = new ScrapingBrowser();

        public static double GetPrice (string isbn)
        {
            GetHtml(isbn);

            double price = 0.0;
            return price;
        }
        static void GetHtml(string isbn)
        {
            string url = $"https://www.amazon.com/s?i=stripbooks&rh=p_66%3A{isbn}&s=relevanceexprank&Adv-Srch-Books-Submit.x=11&Adv-Srch-Books-Submit.y=13&unfiltered=1&ref=sr_adv_b";
            var httpClient = new HttpClient();
            string html = httpClient.GetStringAsync(url).Result; //loads the entire html page of the url

            var htmlDocument = new HtmlDocument(); //a document to save html of url
            htmlDocument.LoadHtml(html); //loads the html to the document

            var itemPrice = htmlDocument.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("a-offscreen"))
                .Where(val => val.InnerText != "0.00")
                .FirstOrDefault();    
        }
    }
}
