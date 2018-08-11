using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aeronautica
{

    class Program
    {
    
        // Imported here are code lines borrowed from the web just to test my console app on full screen mode.

        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;

        // Import ends here.

        enum LoginHeader { id, name, password, adress, telephone, permission }
        enum FlightHeader { id, planename, origin, destiny, departure, arrival, cost }
        enum PlaneHeader { id, name, configuration, businesscostpercentage}
        enum TicketHeader { id, loginname, buydate, returnticket}

        static int[] loginlength = new int[] { 3, 10, 10, 40, 15, 10 };
        static string[] main = new string[0];
        static readonly string[] seatline = new string[22] 
              { "(1) | |_|_|_| |_|_|_|_| |_|_|_| |", "(2) | |_|_|_|  |_|_|_|  |_|_|_| |",
                "(3) | |_|_|   |_|_|_|_|   |_|_| |", "(4) | |_|_|    |_|_|_|    |_|_| |",
                "(5) | |_|_|     |_|_|     |_|_| |", "(6) | |_|_|      |_|      |_|_| |",
                "(7) | |_|_|     |_|_|     |_|_| |", "(8) | |_|_|      |_|      |_|_| |",
                "(9) | |_|_|               |_|_| |", "(10)| |_|_|_|           |_|_|_| |",
                "(11)|          |_|_|_|    |_|_| |", "(12)|         |_|_|_|_|   |_|_| |",
                "(13)|          |_|_|_|  |_|_|_| |", "(14)|         |_|_|_|_| |_|_|_| |",
                "(15)| |_|_|    |_|_|_|          |", "(16)| |_|_|   |_|_|_|_|         |",
                "(17)| |_|_|_|  |_|_|_|          |", "(18)| |_|_|_| |_|_|_|_|         |",
                "(19)| |_|_|_|                   |", "(20)| |_|_|                     |",
                "(21)|          |_|_|_|          |", "(22)|         |_|_|_|_|         |" };

        /// <summary>
    /// Main: sets environment and starts login.
    /// </summary>
    /// <param name="args"></param>

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE); // Call on main for the method to maximize and center this console app.
            MenuLogin();
        }

        /// <summary>
        /// Plots login menu.
        /// </summary>
        
        static void MenuLogin()
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("trip planner ");
            MenuOptions("(l)ogin", "(n)ew user", "(a)bout", "(e)xit program");

            //Initializes login list.
            string[,] login = new string[1, 6];
            FileLoader("login.txt", 6, ref login);

            //Starts menu and deals with menu options.
            char loginchoice;
            loginchoice = Console.ReadKey().KeyChar;
            switch (loginchoice)
            {
                case 'l':
                    Console.Clear();

                    int[] loginlenght = new int[] { loginlength[1], loginlength[2] };
                    string[] logintitles = new string[] { "Login name", "Login password" };
                    string[] logininsert = new string[2];
                    logininsert = MsgInsert(loginlenght, logintitles);

                    bool loginvalidation = false;
                    string[] logindetails = new string[login.GetLength(1)];

                    for (int i = 0; i < login.GetLength(0); i++)
                    {
                        if (login[i, 1] == logininsert[0])
                        {
                            if (login[i, 2] == logininsert[1])
                            {
                                loginvalidation = true;
                                for (int j = 0; j < logindetails.Length; j++) logindetails[j] = login[i, j];                             
                                break;
                            }
                        }
                    }

                    if (!loginvalidation) MsgGeneral("Invalid Login!");
                    else MenuLoginValidation(login, logindetails);
                    MenuLogin();

                    break;

                case 'n':
                    Console.Clear();
                    string[] newlogin = new string[6];
                    string[] subnewlogin = new string[4];
                    int[] subloginlenght = new int[] { 10, 10, 40, 15 };
                    string[] sublogintitles = new string[4];
                    for (int i = 1; i <= 4; i++) sublogintitles[i - 1] = Enum.GetName(typeof(LoginHeader), i);
                    
                    newlogin[0] = DbNewId(login);
                    subnewlogin = MsgInsert(subloginlenght, sublogintitles);
                    for (int i = 1; i < 5; i++) newlogin[i] = subnewlogin[i - 1];
                    newlogin[5] = "general";

                    FileAppend("login.txt", newlogin);
                    MenuLogin();
                    break;

                case 'a':
                    MsgAbout();
                    MenuLogin();
                    break;

                //Backdoor to go directly into flight menu with admin credencials
                case '@':
                    string[] loginadmin = new string[login.GetLength(1)];
                    for (int i = 0; i < login.GetLength(1); i++) loginadmin[i] = login[0, i];
                    MenuFlight(loginadmin);
                    break;

                case 'e':
                    break;

                default:
                    MsgNotOption();
                    MenuLogin();
                    break;
            }
        }

        /// <summary>
        /// Menu to confirm if login details are accurate.
        /// </summary>
        /// <param name="login"> General login list information. </param>
        /// <param name="logindetails"> Specific login information. </param>

        static void MenuLoginValidation(string[,] login, string[] logindetails)
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("Personal details:");
            Console.WriteLine();
            for (int i = 1; i < logindetails.Length - 1; i++)
            {
                if (i != 2) Console.WriteLine(logindetails[i]);
            }
            MenuOptions("(y)es, confirm.", "(c)hange details.", "(l)og out");

            //Starts menu and deals with menu options.
            char loginconfirm = Console.ReadKey().KeyChar;
            switch (loginconfirm)
            {
                case 'y':
                    if (logindetails[logindetails.Length - 1] == "admin") MenuLoginAdmin(login, logindetails);
                    else MenuFlight(logindetails);
                    break;

                case 'c':
                    string[,] loginlist = new string[1, 6];
                    FileLoader("login.txt", 6, ref loginlist);

                    int[] changelength = new int[] { 10, 10, 40, 15 };
                    string[] changetitles = new string[changelength.Length];
                    for (int i = 1; i <= changelength.Length; i++) changetitles[i - 1] = Enum.GetName(typeof(LoginHeader), i);
                    DbChange(Convert.ToInt32(logindetails[0]), changelength, changetitles, ref loginlist);
                    FileReplace("login.txt", loginlist);

                    MenuLogin();
                    break;

                case 'l':
                    break;

                default:
                    MsgNotOption();
                    MenuLoginValidation(login, logindetails);
                    break;
            }
        }

        /// <summary>
        /// Admin menu to manage login information.
        /// </summary>
        /// <param name="login"> General login list information. </param>
        /// <param name="logindetails"> Specific login information. </param>

        static void MenuLoginAdmin(string[,] login, string[] logindetails)
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("login viewer");
            MenuOptions("(v)iew users", "(d)elete user", "(f)light menu", "(l)og out");

            //Starts menu and deals with menu options.
            char loginadmin = Console.ReadKey().KeyChar;
            switch (loginadmin)
            {
                case 'v':
                    Console.Clear();
                    DrawList(login, loginlength, "Login");
                    MsgPressKey();
                    MenuLoginAdmin(login, logindetails);
                    break;
                case 'd':
                    Console.Clear();
                    DrawList(login, loginlength, "Login");

                    int listid = 0;
                    do { Console.WriteLine("Choose id to erase"); } while (!Int32.TryParse(Console.ReadLine(), out listid) || listid < 1 || listid > login.GetLength(0));
                    Console.WriteLine("(y)es to proceed, you cannot undo this operation.");
                    if (Console.ReadKey().KeyChar == 'y')
                    {
                        DbErase(listid, ref login);
                        FileReplace("login.txt", login);
                        Console.Clear();
                        DrawList(login, loginlength, "Login");
                        MsgPressKey();
                    }

                    MenuLoginAdmin(login, logindetails);
                    break;
                case 'f':
                    MenuFlight(logindetails);
                    break;
                case 'l':
                    break;
                default:
                    MsgNotOption();
                    MenuLoginAdmin(login, logindetails);
                    break;
            }
        }

        /// <summary>
        /// Plots flight menu.
        /// </summary>
        /// <param name="userlogin"> Specific login information. </param>

        static void MenuFlight(string[] userlogin)
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("flight");
            bool admin = false;
            if (userlogin[userlogin.Length - 1] == "admin") admin = true;
            if (admin) MenuOptions("(f)light", "(q)ueries", "(c)reate flight", "(e)rase flight", "(p)lane configuration", "(l)og out");
            else MenuOptions("(f)light", "(q)ueries", "(l)og out");

            //Starts admin menu and deals with menu options.
            char flightchoice = Console.ReadKey().KeyChar;
            if (admin)
            {
                switch (flightchoice)
                {
                    case 'c':
                        MsgBuildingSolution();
                        MenuFlight(userlogin);
                        break;

                    case 'e':
                        MsgBuildingSolution();
                        MenuFlight(userlogin);
                        break;

                    case 'p':
                        string planename = "";
                        MenuPlane(planename, userlogin);
                        return;
                }
            }

            //Starts general menu and deals with menu options.
            switch (flightchoice)
            {
                case 'f':
                    DrawWorld();

                    string ogoption = "";
                    string dtoption = "";
                    Console.Write("Choose origin: ");
                    ogoption = Console.ReadLine().ToLower();

                    switch (ogoption)
                    {
                        case "london":
                            DrawWorldPlane(11, 24);
                            break;
                        case "moscow":
                            DrawWorldPlane(12, 38);
                            break;
                        case "ottawa":
                            DrawWorldPlane(14, 12);
                            break;
                        case "paris":
                            DrawWorldPlane(14, 28);
                            break;
                        case "new york":
                            DrawWorldPlane(16, 13);
                            break;
                        case "rome":
                            DrawWorldPlane(16, 32);
                            break;
                        case "xangai":
                            DrawWorldPlane(16, 47);
                            break;
                        case "lisboa":
                            DrawWorldPlane(17, 25);
                            break;
                        case "bombaim":
                            DrawWorldPlane(20, 40);
                            break;
                        case "casablanca":
                            DrawWorldPlane(21, 24);
                            break;
                        case "rio de janeiro":
                            DrawWorldPlane(28, 18);
                            break;
                        case "sydney":
                            DrawWorldPlane(29, 52);
                            break;
                        case "cape city":
                            DrawWorldPlane(31, 30);
                            break;
                        default:
                            MenuFlight(userlogin);
                            break;
                    }

                    Console.Write("\nChoose destination: ");
                    dtoption = Console.ReadLine();

                    Console.ReadKey();
                    MenuFlight(userlogin);
                    break;
                case 'q':
                    MsgBuildingSolution();
                    MenuFlight(userlogin);
                    break;
                case 'l':
                    break;
                default:
                    MsgNotOption();
                    MenuFlight(userlogin);
                    break;
            }
        }

        /// <summary>
        /// Plots plane menu.
        /// </summary>
        /// <param name="planename"> Plane name to be built into the system. </param>
        /// <param name="userlogin"> Specific login information. </param>

        static void MenuPlane (string planename, string[] userlogin)
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("plane builder");
            MenuOptions("(n)ame", "(s)eat planner", "(a)lready built planes","(g)o back", "(l)og out");
            string[] seatplan = new string[10000];

            //Starts menu and deals with menu options.
            char planechoice = Console.ReadKey().KeyChar;
            switch (planechoice)
            {
                case 'n':
                    Console.Clear();
                    Console.Write("Choose plane name: ");
                    planename = Console.ReadLine();
                    MenuPlane(planename, userlogin);
                    break;
                case 's':
                    if (planename == "")
                    {
                        Console.Clear();
                        Console.WriteLine("You must first choose a plane name.");
                        MsgPressKey();

                        MenuPlane(planename, userlogin);         
                        break;
                    }

                    int i = 0, pattern = 0, repeat = 0;
                    char cl;
                    char buildplane = '\0';

                    do
                    {
                        DrawSeatLine();
                        do
                        {
                            do
                            {
                                Console.WriteLine("\nChoose seat line pattern, beginning from the top of the plane.");
                            } while (!int.TryParse(Console.ReadLine(), out pattern));
                        } while (pattern < 1 || pattern > 22);
                        seatplan[i] = Convert.ToString(pattern - 1);
                        i++;

                        do
                        {
                            Console.WriteLine("\nChoose type of seat: (e)conomy or (b)usiness.");
                            Char.TryParse(Console.ReadLine(), out cl);
                        } while (cl != 'e' && cl != 'b');
                        seatplan[i] = Convert.ToString(cl);
                        i++;

                        do
                        {
                            Console.WriteLine("\nChoose how many times it repeats.");
                        } while (!int.TryParse(Console.ReadLine(), out repeat) || repeat < 1);
                        seatplan[i] = Convert.ToString(repeat);
                        i++;
                        DrawSeatLine();

                        MenuOptions("(q)uit seat planner", "(g)enerate plane", "any other key to continue building");
 
                        buildplane = Console.ReadKey().KeyChar;
                        if (buildplane == 'g') DrawSeatPlane(seatplan, planename);
                        
                    } while (buildplane != 'q');
                    MenuPlane(planename, userlogin);

                    break;

                case 'a':
                    string contents = null;
                    FileReader("plane.txt", ref contents);
                    Console.Clear();
                    Console.Write(contents);
                    Console.ReadKey();
                    MenuPlane(planename, userlogin);
                    break;

                case 'g':
                    MenuFlight(userlogin);
                    break;

                case 'l':
                    break;

                default:
                    MsgNotOption();
                    MenuPlane("", userlogin);
                    break;
            }
        }

        /// <summary>
        /// Prints menu options on screen.
        /// </summary>
        /// <param name="options"> Indeterminate number of string variables to be printed. </param>

        static void MenuOptions(params string[] options)
        {
            Console.WriteLine();
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(options[i]);
            }
        }

        /// <summary>
        /// Prints the menu title inside a predetermined stared box.
        /// </summary>
        /// <param name="title"> Title description </param>

        static void MenuHeader(string title)
        {
            string stars = new String('*', 60);
            string empty = new String(' ', 58);
            string spacebefore = new String(' ', 58 / 2 - title.Length / 2);
            string spaceafter = spacebefore;
            if (title.Length % 2 == 1) spaceafter = new String(' ', 58 / 2 - title.Length / 2 - 1);

            Console.WriteLine($"{stars}\n*{empty}*\n*{spaceafter}{title.ToUpper()}{spacebefore}*\n*{empty}*\n{stars}");
        }

        /// <summary>
        /// Replaces file contents with new list formated according to file methods here.
        /// </summary>
        /// <param name="file"> Txt file name present on executable directory. </param>
        /// <param name="matrix"> New list to replace the file with. </param>

        static void FileReplace(string file, string[,] matrix)
        {
            string contents = null;

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    contents += $"{matrix[i, j]}" + Environment.NewLine;
                }
                if ( i + 1 == matrix.GetLength(0)) contents += "*";
                else contents += "*" + Environment.NewLine;
            }

            File.WriteAllText(file, contents);
        }

        static int[,] FileCoordinates(string file, int x, int y)
        {
            string contents = null;
            FileReader(file, ref contents);
            int[,] coordinates = new int[x, y];
            int n = 0;

            for (int i = 0; i < coordinates.GetLength(0); i++)
            {
                for (int j = 0; j < coordinates.GetLength(1); j++)
                {
                    coordinates[i, j] = Convert.ToChar(contents.Substring(n, 1));
                    n++;
                }
                // To remove carriage return and new line
                n += 2;
            }
            return coordinates;
        }

        /// <summary>
        /// Reads first and loads file into a list string variable when file formated accordingly.
        /// </summary>
        /// <param name="file"> Txt file name present on executable directory. </param>
        /// <param name="colcount"> Number of columns the list accepts. </param>
        /// <param name="matrix"> List to write the file contents on to. </param>

        static void FileLoader(string file, int colcount, ref string[,] matrix)
        {
            string contents = null;
            FileReader(file, ref contents);
            int linecount = (contents.Split('\n').Length - (contents.Split('*').Length - 1)) / colcount;
            matrix = new string[linecount, colcount];
            for (int i = 0; i < linecount; i++)
            {
                for(int j = 0; j < colcount; j++)
                {
                    matrix[i, j] = contents.Substring(0, contents.IndexOf("\n")-1);
                    contents = contents.Substring(contents.IndexOf("\n") + 1, contents.Length - (contents.IndexOf("\n") + 1));
                }
                if (contents.Length > 3) contents = contents.Substring(3, contents.Length - 3);
            }
        }

        /// <summary>
        /// Opens a txt file and reads its contents into a string.
        /// </summary>
        /// <param name="file"> Txt file name present on executable directory. </param>
        /// <param name="contents"> String to write to. </param>

        static void FileReader(string file, ref string contents)
        {
            MsgFileException(file);
            StreamReader sr = new StreamReader(file);
            contents = sr.ReadToEnd();
            sr.Close();
        }

        /// <summary>
        /// Appends contents to an existing file.
        /// </summary>
        /// <param name="file"> Text file name present on executable directory.</param>
        /// <param name="contents"> Contents to be appended. </param>

        static void FileAppend(string file, string[] contents)
        {
            MsgFileException(file);
            File.AppendAllText(file, Environment.NewLine);
            for (int i = 0; i < contents.Length; i++)
            {
                File.AppendAllText(file, contents[i] + Environment.NewLine);
            }
            File.AppendAllText(file, "*");
        }

        /// <summary>
        /// Sends a message to the screen saying the solution's still building.
        /// </summary>

        static void MsgBuildingSolution()
        {
            Console.Clear();
            Console.WriteLine("Building solution, be patient...");
            MsgPressKey();
        }

        /// <summary>
        /// Sends a message to the screen confirming the file exist on the executable directory.
        /// </summary>
        /// <param name="file"> Name of the file to search. </param>

        static void MsgFileException(string file)
        {
            try { using (StreamReader sr = new StreamReader(file)); }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine($"Fatal error : {e.Message}\n");
                Console.WriteLine("Press any key to exit the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Sends a message to the screen whenever the menu option is non-existent.
        /// </summary>

        static void MsgNotOption()
        {
            Console.Clear();
            Console.WriteLine("Not an option!");
            MsgPressKey();
        }

        /// <summary>
        /// Sends a general message to the screen and prompts the user to continue on key pressing.
        /// </summary>
        /// <param name="message"> Message to be sent to the screen. </param>

        static void MsgGeneral(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            MsgPressKey();
        }

        /// <summary>
        /// Prompts the user to continue on pressing a key.
        /// </summary>

        static void MsgPressKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        /// <summary>
        /// Prints application developing information.
        /// </summary>

        static void MsgAbout()
        {
            Console.Clear();
            Console.WriteLine("Trip Planner\n\nDeveloper: Ricardo Rosado\nSchool: Cinel\nClass: CET36\nVersion: 1.0");
            MsgPressKey();
        }

        /// <summary>
        /// Prints a clock, current date and time.
        /// </summary>

        static void MsgClock()
        {
            Console.WriteLine();
            DateTime localDate = DateTime.Now;
            Console.WriteLine(localDate);
        }

        /// <summary>
        /// Asks the user to insert n number of string variables and returns an array with the response.
        /// </summary>
        /// <param name="length"> The maximum lenght each field accepts. </param>
        /// <param name="insert"> The name of the fields to insert data.</param>
        /// <returns></returns>

        static string[] MsgInsert(int[] length, string[] insert)
        {
            Console.Clear();
            string[] response = new string[length.Length];

            if (length.Length != insert.Length)
            {
                Console.WriteLine("Parameters lenght doesn't match");
                return response;
            }

            for (int i = 0; i < insert.Length; i++)
            {
                do {
                    do {
                        Console.Write($"{insert[i]}: ");
                        response[i] = Console.ReadLine();
                        if (response[i].Length > length[i]) Console.WriteLine($"maximum length {length[i]}, choose accordingly:");
                    } while (response[i].Length > length[i]);
                    if (response[i] == "*") Console.WriteLine("'*' is a reserved word, choose another.");
                } while (response[i] == "*");
            }
            return response;
        }

        /// <summary>
        /// Picks a line id of a given list and changes its contents accordingly.
        /// </summary>
        /// <param name="listid"> The id of the list line to change. </param>
        /// <param name="length"> The maximum lenght each field accepts. </param>
        /// <param name="matrixtitles"> The names of the list columns where data is going to be changed. </param>
        /// <param name="matrix"> The list to be changed. </param>

        static void DbChange(int listid, int[] length, string[] matrixtitles, ref string[,] matrix)
        {
            listid -= 1;
            string[] matrixchange = new string[length.Length];
            matrixchange = MsgInsert(length, matrixtitles);

            for (int i = 1; i <= matrixchange.Length; i++) matrix[listid, i] = matrixchange[i - 1];
        }

        /// <summary>
        /// Replaces a line id of a given list and replaces its contents with "" except the id.
        /// </summary>
        /// <param name="listid"> The id of the list line to erase.</param>
        /// <param name="matrix"> The list to be changed. </param>

        static void DbErase(int listid, ref string[,] matrix)
        {
            listid -= 1;
            for (int i = 1; i < matrix.GetLength(1); i++)
            {
                matrix[listid, i] = "";
            }
        }

        /// <summary>
        /// Given a list, it gives the number of a new id to be inserted.
        /// </summary>
        /// <param name="matrix"> The list from which a new id is to be retrieved. </param>
        /// <returns> The new id number. </returns>

        static string DbNewId(string[,] matrix)
        {
            int nid = 0;
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                if (Convert.ToInt32(matrix[i, 0]) > nid) nid = Convert.ToInt32(matrix[i, 0]);
            }
            nid += 1;
            string snid = Convert.ToString(nid);
            return snid;
        }

        /// <summary>
        /// Draws a list with the contents of a given data set.
        /// </summary>
        /// <param name="list"> The data set from which to draw the list. </param>
        /// <param name="length"> The maximum length admitted on each list field. </param>
        /// <param name="header"> A designated string from where the list headers may be draw upon. </param>

        static void DrawList(string[,] list, int[] length, string header)
        {
            Console.Clear();
            int maxlength = 0;
            for (int i = 0; i < length.Length; i++) maxlength += length[i];
            string line = new string('-', maxlength + length.Length + 1);
            
            Console.WriteLine(line);
            Console.Write("|");
            for (int i = 0; i < list.GetLength(1); i++)
            {
                if (header == "Login")
                {
                    string space = new String(' ', length[i] - Enum.GetName(typeof(LoginHeader), i).Length);
                    string headername = Enum.GetName(typeof(LoginHeader), i);
                    Console.Write($"{headername.ToUpper()}{space}");
                }
                else if (header == "Flight")
                {
                    string space = new String(' ', length[i] - Enum.GetName(typeof(FlightHeader), i).Length);
                    string headername = Enum.GetName(typeof(FlightHeader), i);
                    Console.Write($"{headername.ToUpper()}{space}");
                }
                else if (header == "Plane")
                {
                    string space = new String(' ', length[i] - Enum.GetName(typeof(PlaneHeader), i).Length);
                    string headername = Enum.GetName(typeof(PlaneHeader), i);
                    Console.Write($"{headername.ToUpper()}{space}");
                }
                else if (header == "Ticket")
                {
                    string space = new String(' ', length[i] - Enum.GetName(typeof(TicketHeader), i).Length);
                    string headername = Enum.GetName(typeof(TicketHeader), i);
                    Console.Write($"{headername.ToUpper()}{space}");
                }
                Console.Write("|");
            }
            Console.WriteLine($"\n{line}");

            for (int i = 0; i < list.GetLength(0); i++)
            {
                Console.Write("|");
                for (int j = 0; j < list.GetLength(1); j++)
                {
                    if (list[i, j] == null) list[i, j] = "";
                    string space = new String(' ', length[j] - list[i, j].Length);
                    Console.Write($"{list[i, j]}{space}|");
                }
                Console.WriteLine($"\n{line}");
            }
        }

        /// <summary>
        /// Draws the line of seats for the user to choose and build the plane plan.
        /// </summary>

        static void DrawSeatLine()
        {
            Console.Clear();
            for (int i = 0; i < 22; i++) Console.WriteLine(seatline[i]);
            Console.WriteLine();
        }

        /// <summary>
        /// Draws the seat plane blueprint.
        /// </summary>
        /// <param name="seatplan"> A string containing the blueprint of the plane. </param>
        /// <param name="planename"> The name of the plane. </param>

        static void DrawSeatPlane(string[] seatplan, string planename)
        {
            Console.Clear();

            string a = "", line = "";
            int xaxis = 1;
            Console.WriteLine($"{planename.ToUpper()}\n");
            for (int i = 0; i < seatplan.Length / 3; i += 3)
            {
                for (int j = 0; j < Convert.ToInt32(seatplan[i + 2]); j++)
                {
                    //if (seatplan[i + 1] == "b") Console.Write("Business   ");
                    //else Console.Write("Economy    ");
                    a = seatline[Convert.ToInt32(seatplan[i])];
                    if (xaxis < 10) line = Convert.ToString(xaxis) + "  " + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|"));
                    else line = Convert.ToString(xaxis) + " " + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|"));
                    Console.WriteLine(line);
                    xaxis++;
                }
            }
            Console.ReadKey();
        }

        /// <summary>
        /// Draws an ascii plane to use on for menu headers.
        /// </summary>

        static void DrawBigPlane()
        {
            string plane = "";
            FileReader("bigplanedraw.txt", ref plane);
            Console.WriteLine();
            Console.Write(plane);
            Console.WriteLine("\n\n");
        }

        /// <summary>
        /// Draws a schema of the world showing possible origins and destinations for travel.
        /// </summary>

        static void DrawWorld()
        {
            Console.Clear();
            string world = "";
            FileReader("world.txt", ref world);
            Console.Write(world);
            Console.WriteLine("\n\n");
        }

        static void DrawWorldPlane(int xcoord, int ycoord)
        {
            Console.Clear();

            xcoord -= 1; ycoord -= 6;

            int[,] wcoordinates = new int[35, 60];
            wcoordinates = FileCoordinates("world.txt", 35, 60);

            int[,] spcoordinates = new int[3, 13];
            spcoordinates = FileCoordinates("smallplanedraw.txt", 3, 13);

            int[,] fcoordinates = new int[35, 60];

            int a, b = 0;

            for (int i = 0; i < wcoordinates.GetLength(0); i++)
            {
                for (int j = 0; j < wcoordinates.GetLength(1); j++)
                {
                    if ((i == xcoord) && (j >= ycoord && j <= ycoord + 12))
                    {
                        a = 0;
                        fcoordinates[i,j] = spcoordinates[a, b];
                        b++;
                        if (b > 12) b = 0;
                    }
                    else if ((i == xcoord + 1) && (j >= ycoord && j <= ycoord + 12))
                    {
                        a = 1;
                        fcoordinates[i, j] = spcoordinates[a, b];
                        b++;
                        if (b > 12) b = 0;
                    }
                    else if ((i == xcoord + 2) && (j >= ycoord && j <= ycoord + 12))
                    {
                        a = 2;
                        fcoordinates[i, j] = spcoordinates[a, b];
                        b++;
                        if (b > 12) b = 0;
                    }
                    else
                    {
                        fcoordinates[i, j] = wcoordinates[i, j];
                    }
                }
            }

            for (int i = 0; i < 35; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    Console.Write((char)fcoordinates[i, j]);
                }
                Console.WriteLine();
            }
        }

    }
}
