using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


var cdaLinks = new List<string>();
var videoLinks = new List<string>();
var videoTitles = new List<string>();

var videos = new SortedDictionary<string, string>();
var cdaLinksD = new SortedDictionary<string, string>();

bool isFileExist = false;

string pathVideos = @"E:\Boruto\boruto.json";
string pathCda = @"E:\Boruto\borutoCda.json";

MySqlConnection connection;
string server;
string database;
string uid;
string password;
bool isConnected;

void GetVideoSource(List<string> urls)
{
    Delete();
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
            InsertBoruto(videoTitles[i], videoUrl);
            videoLinks.Add(videoUrl);
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
    const string Xpath4 = @"//ul[@class='pb-menu-slave pb-menu-slave-indent']";

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

                        //var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;
                        //Console.WriteLine(href);

                        if (isFileExist == false)
                        {

                            Console.WriteLine("FIleNotExist");
                            videoTitles.Add(link.InnerText);
                            //videoLinks.Add("");

                            Console.WriteLine("TytułList1: " + videoTitles[i]);
                            Console.WriteLine("TytułHtml1: " + link.InnerText);

                            //var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;

                            //cdaLinks.Add(html);
                            cdaLinksD.Add(link.InnerText, html);
                            InsertBorutoCda(link.InnerText, html);
                            //videos.Add(link.InnerText, "");
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
                                //var href = link4.ChildNodes[link4.ChildNodes.Count() - 2].Attributes["href"].Value;

                                //cdaLinks.Add(html);
                                cdaLinksD.Add(link.InnerText, html);
                                videoTitles.Add(link.InnerText);
                                //videos.Add(link.InnerText, "");
                                InsertBorutoCda(link.InnerText, html);
                                
                                Console.WriteLine("Dodano");

                            }
                            else if (videoTitles[i] == link.InnerText)
                            {
                                videoTitles.Sort();
                                videoTitles.Reverse();
                                SortTable("BorutoCdaLinks");
                                return;
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

void Initialize()
{
    server = "bvoadt6syedirie96kzk-mysql.services.clever-cloud.com";
    database = "bvoadt6syedirie96kzk";
    uid = "uassz4f3hcs0xafp";
    password = "f3mxd7kQVaKhvrYTYbtt";
    string connectionString;
    connectionString = "SERVER=" + server + ";" + "DATABASE=" +
    database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

    connection = new MySqlConnection(connectionString);
}
void OpenConnection()
{
    try
    {
        connection.Open();
        Console.WriteLine("Connection Open");
        isConnected = true;
    }
    catch (MySqlException ex)
    {
        switch (ex.Number)
        {
            case 0:
                Console.WriteLine("Cannot connect to server.  Contact administrator");
                break;

            case 1045:
                Console.WriteLine("Invalid username/password, please try again");
                break;
        }
        isConnected = false;
    }
}
bool CloseConnection()
{
    try
    {
        connection.Close();
        return true;
    }
    catch (MySqlException ex)
    {
        Console.WriteLine(ex.Message);
        return false;
    }
}
void InsertBoruto(string title, String videoSource)
{
    string query = $"INSERT INTO Boruto (Title, VideoSource) VALUES('{title}', '{videoSource}')";

    //open connection
    if (isConnected == true)
    {
        //create command and assign the query and connection from the constructor
        MySqlCommand cmd = new MySqlCommand(query, connection);

        //Execute command
        cmd.ExecuteNonQuery();


    }
}
void InsertBorutoCda(string title, string link)
{
    string query = $"INSERT INTO BorutoCdaLinks (Title,Link) VALUES('{title}','{link}')";

    //open connection
    if (isConnected == true)
    {
        //create command and assign the query and connection from the constructor
        MySqlCommand cmd = new MySqlCommand(query, connection);

        //Execute command
        cmd.ExecuteNonQuery();
    }
}
void Delete()
{
    string query = $"DELETE FROM Boruto";

    if (isConnected == true)
    {
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.ExecuteNonQuery();

    }
}
void Select()
{
    string query = "SELECT * FROM BorutoCdaLinks";

    if (isConnected == true)
    {
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader dataReader = cmd.ExecuteReader();


        while (dataReader.Read())
        {
            cdaLinksD.Add((string)dataReader["Title"], (string)dataReader["Link"]);
        }

        if (cdaLinksD.Count > 0)
        {
            isFileExist = true;
            videoTitles = cdaLinksD.Keys.Reverse().ToList();
        }
        dataReader.Close();
    }

}
void SortTable(string tableName)
{
    string query = $"ALTER TABLE {tableName} ORDER BY Title DESC";

    if (isConnected == true)
    {
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.ExecuteNonQuery();

    }
}

Initialize();
OpenConnection();
Select();
Console.WriteLine(isFileExist);

GetCdaLinks();
GetVideoSource(cdaLinksD.Values.Reverse().ToList());

CloseConnection();





