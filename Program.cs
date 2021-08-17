using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


List<string> cdaLinks = new List<string>();
List<string> videoLinks = new List<string>();
List<string> videoTitles = new List<string>();


SortedDictionary<string, string> videos = new SortedDictionary<string, string>();


string path = @"E:\Boruto\bruh.json";

bool isFileExist = false;

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
            //videoLinks.Add(videoUrl);
            videoLinks[i] = videoUrl;
            i++;
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
                        if (isFileExist == false)
                        {
                            Console.WriteLine("bruh1");
                            videoTitles.Add(link.InnerText);
                            videoLinks.Add("");

                            Console.WriteLine("TytułList1: " + videoTitles[i]);
                            Console.WriteLine("TytułHtml1: " + link.InnerText);
                            if (videoTitles[i] == link.InnerText && videoLinks[i].Length == 0)
                            {
                                HtmlAttribute src = link3.Attributes["src"];
                                html = src.Value;
                                //return;
                                cdaLinks.Add(html);
                                Console.WriteLine("Zaktualizowano");
                                i++;
                            }
                            else if (videoTitles[i] == link.InnerText && videoLinks[i].Length != 0)
                            {
                                i++;
                                Console.WriteLine("Zostawiono");
                            }
                            else if (videoTitles[i] != link.InnerText)
                            {
                                HtmlAttribute src = link3.Attributes["src"];
                                html = src.Value;

                                cdaLinks.Add(html);
                                videos.Add(link.InnerText, "");
                                Console.WriteLine("Dodano");
                            }
                        }
                        else
                        {
                            Console.WriteLine("bruh2");
                            Console.WriteLine("TytułList2: " + videoTitles[i]);
                            Console.WriteLine("TytułHtml2: " + link.InnerText);
                            if (videoTitles[i] == link.InnerText && videoLinks[i].Length == 0)
                            {
                                HtmlAttribute src = link3.Attributes["src"];
                                html = src.Value;
                                //return;
                                cdaLinks.Add(html);
                                Console.WriteLine("Zaktualizowano");
                                i++;
                            }
                            else if (videoTitles[i] == link.InnerText && videoLinks[i].Length != 0)
                            {
                                i++;
                                Console.WriteLine("Zostawiono");
                            }
                            else if (videoTitles[i] != link.InnerText)
                            {
                                HtmlAttribute src = link3.Attributes["src"];
                                html = src.Value;

                                cdaLinks.Add(html);
                                videos.Add(link.InnerText, "");
                                Console.WriteLine("Dodano");
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
    /*for (int i = 0; i < videoLinks.Count; i++)
    {
        videos[videoTitles[i]] = videoLinks[i];
        videos.ToList().ForEach(x => Console.WriteLine(x));
        Console.WriteLine("bruhasjikfghdkujiasfghkjasdf");
    }*/

    videos.ToList().ForEach(x => Console.WriteLine("Klucz: " + x.Key + " Wartość: " + x.Value));
    Console.WriteLine("bruh");
    Console.WriteLine(videos.Count());
    //videos.ToList().ForEach(x => Console.WriteLine(x));
    //string json = JsonConvert.SerializeObject(videos.Reverse());

    //File.WriteAllText(path, json);

}
void ReadJson()
{
    if (File.Exists(path))
    {
        videos = JsonConvert.DeserializeObject<SortedDictionary<string, string>>(File.ReadAllText(path));
        
        isFileExist = true;
    }
    
    if (videos.Count() > 0)
    {
        videoTitles = videos.Keys.Reverse().ToList();
        videoLinks = videos.Values.Reverse().ToList();
    }

    //Console.WriteLine(videoLinks[50].Length);

    //videoTitles.ForEach(x => Console.WriteLine("Klucz: " + x));
    //videoLinks.ForEach(x => Console.WriteLine("Wartość: " + x));
    //videos.Reverse().ToList().ForEach(x => Console.WriteLine("Klucz: " + x.Key + " Wartość: " + x.Value));

}


ReadJson();
GetCdaLinks();
//GetVideoSource(cdaLinks);
SaveToJson();



