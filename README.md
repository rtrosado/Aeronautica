# aeronautica
flight booking management tool

using System;
using System.IO;

namespace Aeronautica
{
    
    class Program
    {        
        enum LoginHeader { id, name, password, adress, telephone, permission }
        enum FlightHeader { id, planename, origin, destiny, departure, arrival, cost }
        enum PlaneHeader { id, name, configuration, businesscostpercentage}
        enum TicketHeader { id, loginname, buydate, returnticket}

        static string[] seatline = new string[]
        { "(1) | |_|_|_| |_|_|_|_| |_|_|_| |", "(2) | |_|_|_|  |_|_|_|  |_|_|_| |",
          "(3) | |_|_|   |_|_|_|_|   |_|_| |", "(4) | |_|_|    |_|_|_|    |_|_| |",
          "(5) | |_|_|     |_|_|     |_|_| |", "(6) | |_|_|      |_|      |_|_| |",
          "(7) | |_|_|               |_|_| |", "(8) | |_|_|_|           |_|_|_| |",
          "(9) |          |_|_|_|    |_|_| |", "(10)|         |_|_|_|_|   |_|_| |",
          "(11)|          |_|_|_|  |_|_|_| |", "(12)|         |_|_|_|_| |_|_|_| |",
          "(13)| |_|_|    |_|_|_|          |", "(14)| |_|_|   |_|_|_|_|         |",
          "(15)| |_|_|_|  |_|_|_|          |", "(16)| |_|_|_| |_|_|_|_|         |",
          "(17)| |_|_|_|                   |", "(18)| |_|_|                     |",
          "(19)|          |_|_|_|          |", "(20)|         |_|_|_|_|         |" };
        static int[] loginlength = new int[] { 3, 10, 10, 40, 15, 10 };
        static string[] main = new string[0];

        static void Main(string[] args)
        {
            Console.SetWindowSize(100, 40);
            MenuLogin();
        }

        static void MenuLogin()
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("trip planner ");
            MenuOptions("(l)ogin", "(n)ew user", "(a)bout", "(e)xit program");

            char loginchoice;
            loginchoice = Console.ReadKey().KeyChar;
            string[,] login = new string[1, 6];
            FileLoader("login.txt", 6, ref login);

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

                case 'e':
                    break;

                default:
                    MsgNotOption();
                    MenuLogin();
                    break;
            }
        }

        static void MenuLoginValidation(string[,] login, string[] logindetails)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("Personal details:");
            Console.WriteLine();

            for (int i = 1; i < logindetails.Length - 1; i++)
            {
                if (i != 2) Console.WriteLine(logindetails[i]);
            }

            MenuOptions("(y)es, confirm.", "(c)hange details.", "(l)og out");

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

        static void MenuLoginAdmin(string[,] login, string[] logindetails)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("login viewer");
            MenuOptions("(v)iew users", "(d)elete user", "(f)light menu", "(l)og out");

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

        static void MenuFlight(string[] userlogin)
        {
            Console.Clear();
            DrawBigPlane();

            MenuHeader("flight");
            bool admin = false;
            if (userlogin[userlogin.Length - 1] == "admin") admin = true;

            if (admin) MenuOptions("(f)light", "(q)ueries", "(c)reate flight", "(e)rase flight", "(p)lane configuration", "(l)og out");
            else MenuOptions("(f)light", "(q)ueries", "(l)og out");

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

            switch (flightchoice)
            {
                case 'f':
                    DrawWorld();
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
        
        static void MenuPlane (string planename, string[] userlogin)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("plane builder");
            MenuOptions("(n)ame", "(s)eat planner", "(a)lready built planes","(g)o back", "(l)og out");
            char planechoice = Console.ReadKey().KeyChar;
            string[] seatplan = new string[150];

            switch (planechoice)
            {
                case 'n':
                    Console.Clear();
                    Console.WriteLine("Choose plane name: ");
                    planename = Console.ReadLine();
                    break;
                case 's':
                    if (planename == "")
                    {
                        Console.Clear();
                        Console.WriteLine("You must first choose a plane name.");
                        MsgPressKey();
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
                        } while (pattern < 1 || pattern > 20);
                        seatplan[i] = Convert.ToString(pattern - 1);
                        i++;
                        DrawSeatLine();

                        do
                        {
                            Console.WriteLine("\nChoose type of seat: (e)conomy or (b)usiness.");
                            cl = Convert.ToChar(Console.ReadLine());
                        } while (cl != 'e' && cl != 'b');
                        seatplan[i] = Convert.ToString(cl);
                        i++;
                        DrawSeatLine();

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

                    break;
                case 'a':
                    MsgBuildingSolution();
                    break;
                case 'g':
                    MenuFlight(userlogin);
                    break;
                case 'l':
                    break;
                default:
                    break;
            }
        }

        static void MenuOptions(params string[] options)
        {
            Console.WriteLine();
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(options[i]);
            }
        }

        static void MenuHeader(string title)
        {
            string stars = new String('*', 60);
            string empty = new String(' ', 58);
            string spacebefore = new String(' ', 58 / 2 - title.Length / 2);
            string spaceafter = spacebefore;
            if (title.Length % 2 == 1) spaceafter = new String(' ', 58 / 2 - title.Length / 2 - 1);

            Console.WriteLine($"{stars}\n*{empty}*\n*{spaceafter}{title.ToUpper()}{spacebefore}*\n*{empty}*\n{stars}");
        }

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

        static void FileReader(string file, ref string contents)
        {
            MsgFileException(file);
            StreamReader sr = new StreamReader(file);
            contents = sr.ReadToEnd();
            sr.Close();
        }

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

        static void MsgBuildingSolution()
        {
            Console.Clear();
            Console.WriteLine("Building solution, be patient...");
            MsgPressKey();
        }

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

        static void MsgNotOption()
        {
            Console.Clear();
            Console.WriteLine("Not an option!");
            MsgPressKey();
        }

        static void MsgGeneral(string message)
        {
            Console.Clear();
            Console.WriteLine(message);
            MsgPressKey();
        }

        static void MsgPressKey()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
        }

        static void MsgAbout()
        {
            Console.Clear();
            Console.WriteLine("Trip Planner\n\nDeveloper: Ricardo Rosado\nSchool: Cinel\nClass: CET36\nVersion: 1.0");
            MsgPressKey();
        }

        static void MsgClock()
        {
            Console.WriteLine();
            DateTime localDate = DateTime.Now;
            Console.WriteLine(localDate);
        }

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

        static void DbChange(int listid, int[] length, string[] matrixtitles, ref string[,] matrix)
        {
            listid -= 1;
            string[] matrixchange = new string[length.Length];
            matrixchange = MsgInsert(length, matrixtitles);

            for (int i = 1; i <= matrixchange.Length; i++) matrix[listid, i] = matrixchange[i - 1];
        }

        static void DbErase(int listid, ref string[,] matrix)
        {
            listid -= 1;
            for (int i = 1; i < matrix.GetLength(1); i++)
            {
                matrix[listid, i] = "";
            }
        }

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
                    Console.Write($"{list[i, j]}{space}");
                    Console.Write("|");
                }
                Console.WriteLine($"\n{line}");
            }
        }

        static void DrawSeatLine()
        {
            Console.Clear();
            for (int i = 0; i < 20; i++) Console.WriteLine(seatline[i]);
            Console.WriteLine();
        }

        static void DrawSeatPlane(string[] seatplan, string planename)
        {
            Console.Clear();
            Console.WriteLine($"{planename.ToUpper()}\n");
            for (int i = 0; i < seatplan.Length / 3; i += 3)
            {
                for (int j = 0; j < Convert.ToInt32(seatplan[i + 2]); j++)
                {
                    if (seatplan[i + 1] == "b") Console.Write("Business  ");
                    else Console.Write("Economy   ");
                    Console.WriteLine(seatline[i].Substring(seatline[i].IndexOf("|") - 1, seatline[i].Length - seatline[i].IndexOf("|") + 1));
                }
            }
            Console.ReadKey();
        }

        static void DrawBigPlane()
        {
            string plane = "";
            FileReader("plane.txt", ref plane);
            Console.WriteLine();
            Console.Write(plane);
            Console.WriteLine("\n\n");
        }

        static void DrawWorld()
        {
            Console.Clear();
            string world = "";
            FileReader("world.txt", ref world);
            Console.Write(world);
            Console.WriteLine("\n\n");
        }

    }
}
