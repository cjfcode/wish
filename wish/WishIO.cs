using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace Wish
{
    public static class WishIO
    {
        /// <summary>
        /// Begins the IO process.
        /// </summary>
        public static void Begin()
        {
            Console.OutputEncoding = Encoding.UTF8;
            DisplayInitialMessage();
            PromptUserForInput(true);
            GetData();
            WriteFile(WishData.GetPath(), WishData.GetData(), ConnectionManager.GetWebClient());
            AlphabetizeFile();
            DisplayOutput(WishData.GetAllData());
        }

        /// <summary>
        /// Displays information related to the program and its usage.
        /// </summary>
        private static void DisplayInitialMessage()
        {
            string title = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyTitleAttribute>().Title;
            string version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            string programInfo = $"{title} v{version}\n";
            string usageInformationHeader = $"About Wish\n==========";
            string aboutInfo = $"This program parses public Steam wishlists and outputs the result as an alphabetically-sorted text file.";
            string fileLocationInfo = "The output file will be located in the same directory as the program executable.";
            string usageCondition = $"The wishlist or profile must be PUBLIC.\n";
            string exitInfo = "Send \'exit\' as input to quit.\n";
            Console.WriteLine(programInfo);
            Console.WriteLine(usageInformationHeader);
            Console.WriteLine(aboutInfo);
            Console.WriteLine(fileLocationInfo);
            Console.WriteLine(usageCondition);
            Console.WriteLine(exitInfo);
        }

        /// <summary>
        /// Takes input from the user.
        /// </summary>
        /// <param name="first">Represents whether the user has been prompted for input for the first time.</param>
        private static void PromptUserForInput(bool first)
        {
            string promptUserForSteamID = "Please enter a SteamID: ";
            string promptUserForSteamIDOnError = "\nThe SteamID you have entered is invalid.\n\nPlease enter a valid steam ID: ";
            string prompt = first ? promptUserForSteamID : promptUserForSteamIDOnError;
            string input;

            do
            {
                Console.Write(prompt);
                input = Console.ReadLine();

                if (input.Equals("exit"))
                    Environment.Exit(0);

            } while (!long.TryParse(input, out _));

            SteamApiAccess.SetSteamID(input);
        }

        /// <summary>
        /// Attempts to establish a connection with the client. If successful, sets the data to be parsed.
        /// </summary>
        private static void GetData()
        {
            bool successfulConnection = false;

            while (!successfulConnection)
            {
                try
                {
                    WishData.SetData(ConnectionManager.GetWebClient().DownloadString(SteamApiAccess.GetApiUrl()));
                    successfulConnection = true;
                }
                catch
                {
                    PromptUserForInput(false);
                }
            }
        }

        /// <summary>
        /// Handles writing the data to a file.
        /// </summary>
        /// <param name="path">The location to save the file.</param>
        /// <param name="data">The data to save to the file.</param>
        /// <param name="client">The WebClient object to use for downloading the data.</param>
        private static void WriteFile(string path, string data, WebClient client)
        {
            Console.WriteLine("\nFetching data from the server. This process could take awhile for large wishlists, please wait...\n");

            using (StreamWriter streamWriter = new StreamWriter(path))
            {            
                while (true)
                {
                    if (!data.Contains("name"))
                    {
                        SteamApiAccess.SetApiUrl(SteamApiAccess.GetNextPage());
                        data = client.DownloadString(SteamApiAccess.GetApiUrl());

                        if (!data.Contains("name"))
                            break;
                    }

                    data = data.Substring(data.IndexOf("name") + 7);
                    int locationOfEndingQuote = data.IndexOf('\"');
                    string gameTitle = Regex.Unescape(data.Substring(0, locationOfEndingQuote));
                    data = data.Substring(locationOfEndingQuote);
                    streamWriter.WriteLine(char.ToUpper(gameTitle[0]) + gameTitle.Substring(1));
                }
            }
            ConnectionManager.ReleaseWebClient();
        }

        /// <summary>
        /// Enumerates the collection, writing the results to the console.
        /// </summary>
        /// <param name="gameCollection">The collection to display.</param>
        private static void DisplayOutput(string[] gameCollection)
        {
            foreach (string gameTitle in gameCollection)
                Console.WriteLine(gameTitle);

            Console.WriteLine("\nWriting completed. Press any key to exit...");
            Console.ReadKey();
        }

        /// <summary>
        /// Sorts the output file alphabetically.
        /// </summary>
        private static void AlphabetizeFile()
        {
            WishData.SetAllData(File.ReadAllLines(WishData.GetPath()));
            Array.Sort(WishData.GetAllData());
            File.WriteAllText(WishData.GetPath(), string.Join("\n", WishData.GetAllData()));
        }
    }
}