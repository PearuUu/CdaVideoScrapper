using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections;


var cdaLinks = new List<string>();
var videoLinks = new List<string>();
var videoTitles = new List<string>();



var videos = new SortedDictionary<string, string>();
var cdaLinksD = new SortedDictionary<string, string>();

bool isFileExist = false;

string pathVideos = @"E:\Boruto\boruto.json";
string pathCda = @"E:\Boruto\borutoCda.json";

void GetVideoSource(List<string> urls)
{
    IWebDriver driver;
    driver = new ChromeDriver();

    try
    {
        driver.Manage().Timeouts().ImplicitWait = System.TimeSpan.FromSeconds(10);
        driver.Manage().Timeouts().PageLoad = System.TimeSpan.FromSeconds(10);
        int i = 0;
        foreach (string url in urls)
        {
            driver.Navigate().GoToUrl(url);
            IWebElement video = driver.FindElement(By.XPath(@"//video[@class='pb-video-player']"));
            string videoUrl = video.GetAttribute("src");
            Console.WriteLine("Video: " + videoUrl);
            videoLinks.Add(videoUrl);
            Thread.Sleep(10000);
        }

        driver.Quit();

    }
    catch (Exception e)
    {
        Console.WriteLine("bruh");
        driver.Quit();
    }



}
void GetCdaLinks()
{
    HtmlWeb web = new HtmlWeb();
    var html = @"https://narutoboruto.wbijam.pl/boruto.html";
    var doc = web.Load(html);

    const string Xpath = @"//body/div[@id=""szkielet""]/div[@id=""tresc_lewa""]/table/tbody//a[@href]";
    const string Xpath2 = @"//span[@rel]";
    const string Xpath3 = @"//body/div[@id=""szkielet""]/div[@id=""tresc_lewa""]/center/iframe";
    const string Xpath4 = @"//div[@class='wrapqualitybtn']";

    int i = 0;


    foreach (HtmlNode link in doc.DocumentNode.SelectNodes(Xpath))
    {

        HtmlAttribute url = link.Attributes["href"];
        Console.WriteLine(url.Value);
        html = @"https://narutoboruto.wbijam.pl/" + url.Value;
        doc = web.Load(html);

        foreach (HtmlNode link2 in doc.DocumentNode.SelectNodes(Xpath2))
        {
            if (link2 != null)
            {

                HtmlAttribute rel = link2.Attributes["rel"];
                if (rel.Value.StartsWith("_PLU_"))
                {
                    html = @"https://narutoboruto.wbijam.pl/odtwarzacz-" + rel.Value + ".html";
                    doc = web.Load(html);

                    foreach (HtmlNode link3 in doc.DocumentNode.SelectNodes(Xpath3))
                    {
                        HtmlAttribute src = link3.Attributes["src"];
                        html = src.Value;
                        doc = web.Load(html);


                        foreach (HtmlNode link4 in doc.DocumentNode.SelectNodes(Xpath4))
                        {
                            //var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;
                            //Console.WriteLine(href);

                            if (isFileExist == false)
                            {

                                Console.WriteLine("FIleNotExist");
                                videoTitles.Add(link.InnerText);
                                videoLinks.Add("");

                                Console.WriteLine("TytułList1: " + videoTitles[i]);
                                Console.WriteLine("TytułHtml1: " + link.InnerText);

                                var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;

                                cdaLinks.Add(href);
                                cdaLinksD.Add(link.InnerText, href);
                                videoTitles.Add(link.InnerText);
                                videos.Add(link.InnerText, "");
                                Console.WriteLine("Dodano");
                                i++;

                                Thread.Sleep(10000);
                            }
                            else
                            {
                                Console.WriteLine("FileExist");
                                Console.WriteLine("TytułList2: " + videoTitles[i]);
                                Console.WriteLine("TytułHtml2: " + link.InnerText);
                                /*if (videoTitles[i] == link.InnerText && videoLinks[i].Length == 0)
                                {
                                    //var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;

                                    //cdaLinks.Add(href);
                                    Console.WriteLine("Zaktualizowano");
                                    i++;
                                }*/
                                if (videoTitles[i] != link.InnerText)
                                {
                                    var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;

                                    cdaLinks.Add(href);
                                    cdaLinksD.Add(link.InnerText, href);
                                    videoTitles.Add(link.InnerText);
                                    videos.Add(link.InnerText, "");
                                    Console.WriteLine("Dodano");

                                }
                                else if (videoTitles[i] == link.InnerText)
                                {
                                    videoTitles.Sort();
                                    videoTitles.Reverse();
                                    return;
                                }



                            }

                        }


                    }

                }
            }
        }
    }

    
}

void SaveToJson()
{
    for (int i = 0; i < videoLinks.Count; i++)
    {
        videos[videoTitles[i]] = videoLinks[i];

    }
    string videoJson = JsonConvert.SerializeObject(videos);
    string cdaJson = JsonConvert.SerializeObject(cdaLinksD);

    File.WriteAllText(pathVideos, videoJson);
    File.WriteAllText(pathCda, cdaJson);

}
void ReadJson()
{
    if (File.Exists(pathCda))
    {
        cdaLinksD = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(File.ReadAllText(pathCda));
        videos = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(File.ReadAllText(pathCda));
        isFileExist = true;
        

        if (cdaLinksD.Count() > 0)
        {
            Console.WriteLine(isFileExist);
            videoTitles = cdaLinksD.Keys.Reverse().ToList();
            cdaLinks = cdaLinksD.Values.Reverse().ToList();

        }

        //Console.WriteLine(videoLinks[50].Length);

        videoTitles.ForEach(x => Console.WriteLine("Klucz: " + x));
        //videoLinks.ForEach(x => Console.WriteLine("Wartość: " + x));
        //videos.Reverse().ToList().ForEach(x => Console.WriteLine("Klucz: " + x.Key + " Wartość: " + x.Value));
        //cdaLinks.ForEach(x => Console.WriteLine("Wartość: " + x));
        //Console.WriteLine("-----------------------------------------------------------------------------------------------------------");
    }
}


ReadJson();
GetCdaLinks();
GetVideoSource(cdaLinksD.Values.Reverse().ToList());
SaveToJson();



