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
            string url = $"https://www.amazon.com/s?i=stripbooks&rh=p_66%3A{isbn}&s=relevanceexprank&Adv-Srch-Books-Submit.x=11&Adv-Srch-Books-Submit.y=13&unfiltered=1&ref=sr_adv_b";
            var httpClient = new HttpClient();
            string html = httpClient.GetStringAsync(url).Result; //loads the entire html page of the url

            var htmlDocument = new HtmlDocument(); //a document to save html of url
            htmlDocument.LoadHtml(html); //loads the html to the document

            return htmlDocument;   
        }

        static string GetScrapedPrice(HtmlDocument htmlDocument)
        {
            var priceList = htmlDocument.DocumentNode.Descendants("span")
                .Where(node => node.GetAttributeValue("class", "")
                .Equals("a-offscreen"))
                .ToList();
            if(priceList.Count>0)
            {
                string itemPrice = priceList
                .FirstOrDefault()
                .InnerHtml
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
