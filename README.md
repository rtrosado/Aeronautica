# aeronautica
flight book management tool

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

        static void Main(string[] args)
        {        
            Console.SetWindowSize(100, 40);
            char login = '\0';
            while (login != 'e') MenuLogin(ref login);
        }

        static void MenuLogin(ref char loginchoice)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("trip planner ");
            MenuOptions("(l)ogin", "(n)ew user", "(a)bout", "(e)xit program");
            loginchoice = Console.ReadKey().KeyChar;
            string[,] login = new string[1, 6];
            FileLoader("login.txt", 6, ref login);

            switch (loginchoice)
            {
                case 'l':
                    Console.Clear();

                    bool loginvalidation = false;
                    string[] logindetails = new string[login.GetLength(1)];
                    Console.Write("Login name: ");
                    string loginname = Console.ReadLine();
                    Console.Write("Login password: ");
                    string loginpassword = Console.ReadLine();

                    for (int i = 0; i < login.GetLength(0); i++)
                    {
                        if (login[i, 1] == loginname)
                        {
                            if (login[i, 2] == loginpassword)
                            {
                                loginvalidation = true;
                                for (int j = 0; j < logindetails.Length; j++)
                                {
                                    logindetails[j] = login[i, j];
                                }
                                break;
                            }
                        }
                    }
                
                    if (!loginvalidation)  MsgGeneral("Invalid Login!");
                    else
                    {
                        char flight = '\0';
                        Console.Clear();
                        MenuHeader("login");
                        Console.WriteLine("\nPersonal details:\n");
                        for (int i = 1; i < logindetails.Length - 1; i++)
                        {
                            if (i != 2) Console.WriteLine(logindetails[i]);
                        }
                        Console.WriteLine("\n(y)es, confirm.");
                        Console.WriteLine("(c)hange details.");
                        Console.WriteLine("any other key to quit");
                        char loginconfirm = Console.ReadKey().KeyChar;
                        if (loginconfirm == 'y')
                        {
                            if (logindetails[logindetails.Length - 1] == "admin")
                            {
                                char loginadmin = '\0';
                                while (loginadmin != 'f') MenuLoginAdmin(login, ref loginadmin);
                            }
                            while (flight != 'l') MenuFlight(ref flight, ref logindetails);
                        }
                        else if (loginconfirm == 'c')
                        {
                            MsgBuildingSolution();
                        }
                    }

                    return;
                case 'n':
                    Console.Clear();
                    string[] newlogin = new string[6];
                    int newloginid = 0;
                    for ( int i = 0; i < login.GetLength(0); i++)
                    {
                        if (Convert.ToInt32(login[i, 0]) > newloginid) newloginid = Convert.ToInt32(login[i, 0]);
                    }
                    newloginid += 1;

                    newlogin[0] = Convert.ToString(newloginid);
                    for (int i = 1; i < 5; i++)
                    {
                        Console.Write($"Insert {Enum.GetName(typeof(LoginHeader), i)}: ");
                        newlogin[i] = Console.ReadLine();
                    }
                    newlogin[5] = "general";

                    FileAppend("login.txt", newlogin);

                    return;
                case 'a':
                    MsgAbout();
                    return;
                case 'e':
                    return;
                default:
                    MsgNotOption();
                    return;
            }
        }

        static void MenuLoginAdmin(string[,] login,  ref char loginadmin)
        {
            Console.Clear();
            MenuHeader("login viewer");
            MenuOptions("(v)iew users", "(d)elete user", "(f)light menu");
            loginadmin = Console.ReadKey().KeyChar;
            switch (loginadmin)
            {
                case 'v':
                    Console.Clear();
                    int[] length = new int[] { 3, 10, 10, 40, 15, 10 };
                    DrawList(login, length, "Login");
                    MsgPressKey();
                    return;
                case 'd':
                    MsgBuildingSolution();
                    return;
                case 'f':
                    return;
                default:
                    MsgNotOption();
                    return;
            }
        }

        static void MenuFlight(ref char flightchoice, ref string[] userlogin)
        {
            Console.Clear();
            DrawBigPlane();

            MenuHeader("flight");
            bool admin = false;
            if (userlogin[userlogin.Length - 1] == "admin") admin = true;

            if (admin) MenuOptions("(f)light", "(q)ueries", "(c)reate flight", "(e)rase flight", "(p)lane configuration", "(l)og out");
            else MenuOptions("(f)light", "(q)ueries", "(l)og out");

            flightchoice = Console.ReadKey().KeyChar;

            if (admin)
            {
                switch (flightchoice)
                {
                    case 'c':
                        MsgBuildingSolution();
                        return;
                    case 'e':
                        MsgBuildingSolution();
                        return;
                    case 'p':
                        char plane = '\0';
                        string planename = "";
                        while (plane != 'g') MenuPlane(ref plane, ref planename);
                        return;
                }
            }

            switch (flightchoice)
            {
                case 'f':
                    DrawWorld();
                    Console.ReadKey();
                    MsgBuildingSolution();
                    return;
                case 'q':
                    MsgBuildingSolution();
                    return;
                case 'l':
                    return;
                default:
                    MsgNotOption();
                    return;
            }
            
        }
        
        static void MenuPlane (ref char planechoice, ref string planename)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("plane builder");
            MenuOptions("(n)ame", "(s)eat planner", "(a)lready built planes","(g)o back");
            planechoice = Console.ReadKey().KeyChar;
            string[] seatplan = new string[150];

            switch (planechoice)
            {
                case 'n':
                    Console.Clear();
                    Console.WriteLine("Choose plane name: ");
                    planename = Console.ReadLine();
                    return;
                case 's':
                    if (planename == "")
                    {
                        Console.Clear();
                        Console.WriteLine("You must first choose a plane name.");
                        MsgPressKey();
                        return;
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

                    return;
                case 'a':
                    MsgBuildingSolution();
                    return;
                case 'g':
                    return;
                default:
                    return;
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
            File.AppendAllText(file, "\n*");
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
            Console.WriteLine("Trip Planner\nDeveloper: Ricardo Rosado\nVersion: 1.0");
            MsgPressKey();
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
