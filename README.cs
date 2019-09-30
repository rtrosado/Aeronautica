using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Aeronautica
{

    class Program
    {
        // copied here are code lines borrowed from the web 
        // to test this console app on full screen mode.
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0; private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6; private const int RESTORE = 9;
        // copy ends here.

        enum LoginHeader { id, name, password, adress, telephone, permission }
        enum FlightHeader { id, planeid, origin, destiny, economy, business, date, hour }
        enum PlaneHeader { id, name }
        enum TicketHeader { id, flightid, loginid, seat, buydate }

        static int[] loginlength = new int[] { 4, 10, 10, 50, 15, 10 };
        static int[] flightlength = new int[] { 4, 8, 20, 20, 10, 10, 15, 10 };
        static int[] ticketlength = new int[] { 4, 8, 8, 4, 15 };

        static string[] main = new string[0];
        static readonly string[] seatline = new string[22] 
              { "(1) | |_|_|_| |_|_|_|_| |_|_|_| |", "(2) | |_|_|_| |_|_|_|   |_|_|_| |",
                "(3) | |_|_|   |_|_|_|_|   |_|_| |", "(4) | |_|_|   |_|_|_|     |_|_| |",
                "(5) | |_|_|     |_|_|     |_|_| |", "(6) | |_|_|     |_|       |_|_| |",
                "(7) | |_|_|     |_|_|     |_|_| |", "(8) | |_|_|     |_|       |_|_| |",
                "(9) | |_|_|               |_|_| |", "(10)| |_|_|_|           |_|_|_| |",
                "(11)|         |_|_|_|     |_|_| |", "(12)|         |_|_|_|_|   |_|_| |",
                "(13)|         |_|_|_|   |_|_|_| |", "(14)|         |_|_|_|_| |_|_|_| |",
                "(15)| |_|_|   |_|_|_|           |", "(16)| |_|_|   |_|_|_|_|         |",
                "(17)| |_|_|_| |_|_|_|           |", "(18)| |_|_|_| |_|_|_|_|         |",
                "(19)| |_|_|_|                   |", "(20)| |_|_|                     |",
                "(21)|         |_|_|_|           |", "(22)|         |_|_|_|_|         |" };
        static readonly string[,] capitalcoord = new string[13, 3]
            {{"london", "11", "24" },{"moscow", "12", "38" },{"ottawa", "14", "12" },{"paris", "14", "28" },
            {"new york", "16", "13" },{"rome", "16", "32" },{"xangai", "16", "47" },{"lisboa", "17", "25" },
            {"bombaim", "20", "40" },{"casablanca", "21", "24" },{"rio de janeiro", "28", "18" },{"sydney", "29", "52" },
            {"cape city", "31", "30" }};
        static readonly string[,] columncoord = new string[10, 2] 
            {{"A", "6" },{"B", "8"},{"C", "10"},{"D", "14"},{"E", "16"},
            {"F", "18"},{"G", "20"},{"H", "24"},{"I", "26"},{"J", "28"}};

        // Initializes main lists.
        static string[,] login = new string[1, 6];
        static string[,] flightlist = new string[1, 8];
        static string[,] plane = new string[1, 102];
        static string[,] ticket = new string[1, 5];
        static string[] seatplan = new string[297];

        static int doption = 0;
        static int roption = 0;
        static string returnseat;
        static string departureseat;

        /// <summary>
        /// Main: sets environment and starts login.
        /// </summary>
        /// <param name="args"></param>

        static void Main(string[] args)
        {
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            ShowWindow(ThisConsole, MAXIMIZE); // Call on main for the method to maximize and center this console app.
            Console.CursorVisible = false;

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
            MsgOptions("(l)ogin", "(n)ew user", "(a)bout", "(e)xit program");

            //Starts menu and deals with menu options.
            char loginchoice;
            loginchoice = Console.ReadKey().KeyChar;
            switch (loginchoice)
            {
                case 'l':
                    SubLoginLogin();
                    break;

                case 'n':
                    SubLoginNew();
                    break;

                case 'a':
                    SubLoginAbout();
                    break;

                //Backdoor to go directly into flight menu with admin credencials.
                case '@':
                    FileLoader("login.txt", 6, ref login);
                    string[] loginadminf = new string[login.GetLength(1)];
                    for (int i = 0; i < login.GetLength(1); i++) loginadminf[i] = login[0, i];
                    MenuFlight(loginadminf);
                    break;

                //Backdoor to go directly into ticket menu with admin credencials from lisboa to rome.
                case '_':
                    FileLoader("login.txt", 6, ref login);
                    string[] loginadmint = new string[login.GetLength(1)];
                    for (int i = 0; i < login.GetLength(1); i++) loginadmint[i] = login[0, i];
                    MenuTicket("Lisboa", "Rome", loginadmint);
                    break;

                //Quits program.
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

        static void MenuValidation(string[,] login, string[] logindetails)
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
            MsgOptions("(y)es, confirm.", "(c)hange details.", "(l)og out");

            //Starts menu and deals with menu options.
            char loginconfirm = Console.ReadKey().KeyChar;
            switch (loginconfirm)
            {
                case 'y':
                    SubValidationYes(logindetails);
                    break;

                case 'c':
                    SubValidationChange(logindetails);
                    break;

                case 'l':
                    MenuLogin();
                    break;

                default:
                    MsgNotOption();
                    MenuValidation(login, logindetails);
                    break;
            }
        }

        /// <summary>
        /// Admin menu to manage login information.
        /// </summary>
        /// <param name="login"> General login list information. </param>
        /// <param name="logindetails"> Specific login information. </param>

        static void MenuAdmin(string[,] login, string[] logindetails)
        {
            //Prints header and menu options.
            Console.Clear();
            DrawBigPlane();
            MenuHeader("login viewer");
            MsgOptions("(v)iew users", "(d)elete user", "(f)light menu", "(l)og out");

            //Starts menu and deals with menu options.
            char loginadmin = Console.ReadKey().KeyChar;
            switch (loginadmin)
            {
                case 'v':
                    SubAdminView(logindetails);
                    break;
                case 'd':
                    SubAdminDelete(logindetails);
                    break;
                case 'f':
                    MenuFlight(logindetails);
                    break;
                case 'l':
                    MenuLogin();
                    break;
                default:
                    MsgNotOption();
                    MenuAdmin(login, logindetails);
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
            if (admin) MsgOptions("(f)light", "l(i)st flights", "(c)reate flight", "(d)elete flight", "(p)lane configuration", "(m)onthly reports", "(l)og out");
            else MsgOptions("(f)light", "l(i)st flights", "(l)og out");

            //Added menu with admin privileges.
            char flightchoice = Console.ReadKey().KeyChar;
            if (admin)
            {
                switch (flightchoice)
                {
                    case 'c':
                        SubFlightCreate(userlogin);
                        break;

                    case 'd':
                        SubFlightDelete(userlogin);
                        break;

                    case 'm':
                        MsgBuildingSolution();
                        MenuFlight(userlogin);
                        return;

                    case 'p':
                        string planename = "";
                        MenuPlane(planename, userlogin);
                        return;
                }
            }

            //General menu.
            switch (flightchoice)
            {
                case 'f':
                    SubFlightFlight(userlogin);
                    break;

                case 'i':
                    SubFlightList(userlogin);
                    break;

                case 'l':
                    MenuLogin();
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
            MsgOptions("(n)ame", "(s)eat planner", "(a)lready built planes","(g)o back", "(l)og out");

            FileLoader("plane.txt", 102, ref plane);

            //Starts menu and deals with menu options.
            char planechoice = Console.ReadKey().KeyChar;
            switch (planechoice)
            {
                case 'n':
                    SubPlaneName(planename, userlogin);
                    break;

                case 's':
                    SubPlaneSeat(planename, userlogin);
                    break;

                case 'a':
                    SubPlaneBuilt(planename, userlogin);
                    break;

                case 'g':
                    MenuFlight(userlogin);
                    break;

                case 'l':
                    MenuLogin();
                    break;

                default:
                    MsgNotOption();
                    MenuPlane("", userlogin);
                    break;
            }
        }

        /// <summary>
        /// Plots ticket menu.
        /// </summary>
        /// <param name="origin">Departure city.</param>
        /// <param name="destination">Return city.</param>
        /// <param name="userlogin">Specific login information.</param>

        static void MenuTicket(string origin, string destination, string[] userlogin)
        {
            Console.Clear();
            DrawBigPlane();
            MenuHeader("ticket");
            MsgOptions("(d)eparture", "(r)eturn", "(b)uy ticket", "(g)o back", "(l)og out");

            origin = origin.ToUpper(); destination = destination.ToUpper();
            char ticketchoice = Console.ReadKey().KeyChar;

            switch (ticketchoice)
            {
                case 'd':
                    departureseat = SubTicketDeparture(origin, destination, userlogin);
                    MenuTicket(origin, destination, userlogin);
                    break;

                case 'r':
                    returnseat = SubTicketReturn(destination, origin, userlogin);
                    MenuTicket(origin, destination, userlogin);
                    break;

                case 'b':
                    SubTicketBuy(userlogin, departureseat, returnseat);
                    MenuTicket(origin, destination, userlogin);
                    break;

                case 'g':
                    doption = 0; roption = 0;
                    MenuFlight(userlogin);
                    break;

                case 'l':
                    doption = 0; roption = 0;
                    MenuLogin();
                    break;

                default:
                    MsgNotOption();
                    MenuTicket(origin, destination, userlogin);
                    break;
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
        /// Menu login, case login.
        /// </summary>

        static void SubLoginLogin()
        {
            Console.Clear();

            FileLoader("login.txt", 6, ref login);

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

            if (!loginvalidation)
            {
                MsgGeneral("Invalid Login!");
                MenuLogin();
            }
            else MenuValidation(login, logindetails);
        }

        /// <summary>
        /// Menu login, case new.
        /// </summary>

        static void SubLoginNew()
        {
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
        }

        /// <summary>
        /// Menu login, case about.
        /// </summary>

        static void SubLoginAbout()
        {
            MsgAbout();
            MenuLogin();
        }

        /// <summary>
        /// Menu validation, case yes.
        /// </summary>
        /// <param name="logindetails"></param>

        static void SubValidationYes(string[] logindetails)
        {
            if (logindetails[logindetails.Length - 1] == "admin") MenuAdmin(login, logindetails);
            else MenuFlight(logindetails);
        }

        /// <summary>
        /// Menu validation, case change.
        /// </summary>
        /// <param name="logindetails"></param>

        static void SubValidationChange(string[] logindetails)
        {
            string[,] loginlist = new string[1, 6];
            FileLoader("login.txt", 6, ref loginlist);

            int[] changelength = new int[] { 10, 10, 40, 15 };
            string[] changetitles = new string[changelength.Length];
            for (int i = 1; i <= changelength.Length; i++) changetitles[i - 1] = Enum.GetName(typeof(LoginHeader), i);
            DbChange(Convert.ToInt32(logindetails[0]), changelength, changetitles, ref loginlist);
            FileReplace("login.txt", loginlist);

            MenuLogin();
        }

        /// <summary>
        /// Menu admin, case view.
        /// </summary>
        /// <param name="logindetails"></param>

        static void SubAdminView(string[] logindetails)
        {
            Console.Clear();
            DrawList(login, loginlength, "Login");
            MsgPressKey();
            MenuAdmin(login, logindetails);
        }

        /// <summary>
        /// Menu admin, case delete.
        /// </summary>
        /// <param name="logindetails"></param>

        static void SubAdminDelete(string[] logindetails)
        {
            Console.Clear();
            DrawList(login, loginlength, "Login");

            FileLoader("ticket.txt", 5, ref ticket);
            bool confirm = false;

            int listid = 0;
            do { Console.WriteLine("Choose id to erase"); } while (!Int32.TryParse(Console.ReadLine(), out listid) || listid < 1 || listid > login.GetLength(0));

            for (int i = 0; i < ticket.GetLength(0); i++)
            {
                if (Convert.ToString(listid) == ticket[i, 2])
                {
                    confirm = true;                   
                    break;
                }
            }

            if (confirm)
            {
                Console.WriteLine("\nCannot erase selected login, it has tickets associated");
                MsgPressKey();
            }
            else
            {
                Console.WriteLine("(y)es to proceed, you cannot undo this operation.");
                if (Console.ReadKey().KeyChar == 'y')
                {
                    DbErase(listid, ref login);
                    FileReplace("login.txt", login);
                    Console.Clear();
                    DrawList(login, loginlength, "Login");
                    MsgPressKey();
                }
            }

            MenuAdmin(login, logindetails);
        }

        /// <summary>
        /// Menu flight, case create.
        /// </summary>
        /// <param name="userlogin">Specific login information.</param>

        static void SubFlightCreate(string[] userlogin)
        {
            FileLoader("plane.txt", 102, ref plane);
            FileLoader("flight.txt", 8, ref flightlist);
            string flightid = DbNewId(flightlist);

            int[] flightlengthtrim = new int[7];
            for (int i = 0; i < flightlengthtrim.Length; i++) flightlengthtrim[i] = flightlength[i + 1];

            string[] inserttrim = new string[7];
            for (int i = 0; i < inserttrim.Length; i++) inserttrim[i] = Enum.GetName(typeof(FlightHeader), i + 1);

            string[] resulttrim = new string[7];
            string[] result = new string[8];

            resulttrim = MsgInsert(flightlengthtrim, inserttrim);

            result[0] = flightid;
            for (int i = 1; i < result.Length; i++) result[i] = resulttrim[i - 1];

            result[2] = result[2].ToUpper();
            result[3] = result[2].ToUpper();

            bool confirm = true;
            bool planeid = false;
            for (int i = 0; i < plane.GetLength(0); i++)
            {
                if (result[1] == plane[i, 0]) { planeid = true; break; }
            }
            if (!planeid) { confirm = false; Console.WriteLine("Plane id does not exist, choose another or build it."); }

            bool capital = false;
            for (int i = 0; i < capitalcoord.GetLength(0); i++)
            {
                if (result[2] == capitalcoord[i, 0].ToUpper()) { capital = true; confirm = false; break; }
                if (result[3] == capitalcoord[i, 0].ToUpper()) { capital = true; confirm = false; break; }
            }
            if (!capital) { confirm = false; Console.WriteLine("City name not part of the network."); }

            uint res = 0;
            bool coste = UInt32.TryParse(result[4], out res);
            bool costb = UInt32.TryParse(result[5], out res);

            if (!coste) { confirm = false; Console.WriteLine("Economy seat cost must be a positive integer."); }
            if (!costb) { confirm = false; Console.WriteLine("Business seat cost must be a positive integer."); }

            DateTime dres;
            bool date = DateTime.TryParse(result[6], out dres);
            DateTime tres;
            bool time = DateTime.TryParse(result[7], out tres);

            if (!date) { confirm = false; Console.WriteLine("Date is not on a valid format."); }
            else { result[6] = dres.ToString("MM,dd,yyyy"); }
            if (!time) { confirm = false; Console.WriteLine("Hour, minutes not on a valid format."); }
            else { result[7] = tres.ToString("HH,mm"); }

            if (confirm)  FileAppend("flight.txt", result);

            MsgPressKey();
            MenuFlight(userlogin);
        }

        /// <summary>
        /// Menu flight, case delete.
        /// </summary>
        /// <param name="userlogin">Specific login information.</param>

        static void SubFlightDelete(string[] userlogin)
        {
            DrawList(flightlist, flightlength, "Flight");

            FileLoader("ticket.txt", 5, ref ticket);
            bool confirm = false;

            int listid = 0;
            do { Console.WriteLine("Choose id to erase"); } while (!Int32.TryParse(Console.ReadLine(), out listid) || listid < 1 || listid > flightlist.GetLength(0));

            for (int i = 0; i < ticket.GetLength(0); i++)
            {
                if (Convert.ToString(listid) == ticket[i, 1])
                {
                    confirm = true;
                    break;
                }
            }

            if (confirm)
            {
                Console.WriteLine("\nCannot erase selected flight, it has tickets associated");
                MsgPressKey();
            }
            else
            {
                Console.WriteLine("(y)es to proceed, you cannot undo this operation.");
                if (Console.ReadKey().KeyChar == 'y')
                {
                    DbErase(listid, ref flightlist);
                    FileReplace("flight.txt", flightlist);
                    Console.Clear();
                    DrawList(flightlist, flightlength, "Flight");
                }
            }

            MsgPressKey();
            MenuFlight(userlogin);
        }

        /// <summary>
        /// Menu flight, case reports.
        /// </summary>
        /// <param name="userlogin">Specific login information.</param>

        static void SubFlightReports(string[] userlogin)
        {
            MsgBuildingSolution();
        }

        /// <summary>
        /// Menu flight, case flight.
        /// </summary>
        /// <param name="userlogin">Specific login information.</param>

        static void SubFlightFlight(string[] userlogin)
        {
            DrawWorld();

            string ogoption = ""; string dtoption = "";
            bool ogoptionc = false; bool dtoptionc = false;
            int[,] movement = new int[0, 2];
            int ogx = 0, ogy = 0, dtx = 0, dty = 0;

            do
            {
                Console.Write("choose origin: ");
                ogoption = Console.ReadLine().ToLower();
                for (int i = 0; i < capitalcoord.GetLength(0); i++)
                {
                    if (capitalcoord[i, 0] == ogoption)
                    {
                        ogx = Convert.ToInt32(capitalcoord[i, 1]);
                        ogy = Convert.ToInt32(capitalcoord[i, 2]);
                        DrawWorldPlane(ogx, ogy);
                        ogoptionc = true;
                        break;
                    }
                }
            } while (!ogoptionc);

            do
            {
                Console.Write("choose destination: ");
                dtoption = Console.ReadLine().ToLower();
                if (dtoption == ogoption) Console.WriteLine("arrival mustn't coincide with departure.");
                else
                {
                    for (int i = 0; i < capitalcoord.GetLength(0); i++)
                    {
                        if (capitalcoord[i, 0] == dtoption)
                        {
                            dtx = Convert.ToInt32(capitalcoord[i, 1]);
                            dty = Convert.ToInt32(capitalcoord[i, 2]);

                            movement = DrawMovementBetween(ogx, ogy, dtx, dty);

                            for (int j = 0; j < movement.GetLength(0); j++)
                            {
                                DrawWait(500);
                                DrawWorldPlane(movement[j, 0], movement[j, 1]);
                            }

                            DrawWorldPlane(dtx, dty);
                            dtoptionc = true;
                            break;
                        }
                    }
                }
            } while (!dtoptionc);

            MsgOptions("(y)es to confirm flight info", "( ) any other key to go back");
            char flightcconfirm = '\0';
            flightcconfirm = Console.ReadKey().KeyChar;
            if (flightcconfirm == 'y')
            {
                MenuTicket(ogoption, dtoption, userlogin);
                return;
            }
            MenuFlight(userlogin);
        }

        /// <summary>
        /// Menu flight, case list.
        /// </summary>
        /// <param name="userlogin">Specific login information.</param>

        static void SubFlightList(string[] userlogin)
        {
            string[,] list = new string[1, 8];
            list = DbOrderFlightDate();
            DrawList(list, flightlength, "Flight");
            MsgPressKey();
            MenuFlight(userlogin);
        }

        /// <summary>
        /// Menu plane, case name.
        /// </summary>
        /// <param name="planename">Name of the plane to build.</param>
        /// <param name="userlogin">Specific login information.</param>

        static void SubPlaneName(string planename, string[] userlogin)
        {
            Console.Clear();
            Console.Write("Choose plane name: ");
            planename = Console.ReadLine();
            MenuPlane(planename, userlogin);
        }

        /// <summary>
        /// Menu plane, case seat.
        /// </summary>
        /// <param name="planename">Name of the plane to build.</param>
        /// <param name="userlogin">Specific login information.</param>

        static void SubPlaneSeat(string planename, string[] userlogin)
        {
            if (planename == "")
            {
                Console.Clear();
                Console.WriteLine("You must first choose a plane name.");
                MsgPressKey();

                MenuPlane(planename, userlogin);
                return;
            }

            bool planeexists = false;
            for (int i = 0; i < plane.GetLength(0); i++)
            {
                if (planename.ToUpper() == plane[i, 1])
                {
                    planeexists = true;
                    break;
                }
            }

            if (planeexists)
            {
                Console.Clear();
                Console.WriteLine("Plane already exists, choose another name.");
                MsgPressKey();

                MenuPlane(planename, userlogin);
                return;
            }

            int count = 0, pattern = 0, repeat = 0, frepeat = 0;
            char cl;
            char buildplane = '\0', saveplane = '\0';

            do
            {
                DrawSeatLine();
                do
                {
                    Console.WriteLine("\nChoose seat line pattern, beginning from the top of the plane.");
                } while (!int.TryParse(Console.ReadLine(), out pattern) || pattern < 1 || pattern > 22);

                seatplan[count] = Convert.ToString(pattern - 1);
                count++;

                do
                {
                    Console.WriteLine("\nChoose type of seat: (e)conomy or (b)usiness.");
                    Char.TryParse(Console.ReadLine(), out cl);
                } while (cl != 'e' && cl != 'b');
                seatplan[count] = Convert.ToString(cl);
                count++;

                frepeat += repeat;
                do
                {
                    Console.WriteLine("\nChoose how many times it repeats (99 is the maximum iterations permitted).");
                } while (!int.TryParse(Console.ReadLine(), out repeat) || repeat < 1 || repeat > 99 || repeat + frepeat > 99);
                seatplan[count] = Convert.ToString(repeat);
                count++;

                string[] blueprint = new string[102];

                if (repeat + frepeat == 99)
                {
                    string id = DbNewId(plane);
                    blueprint = DrawSeatPlane(id, seatplan, planename);
                    for (int i = 0; i < blueprint.Length; i++)
                    {
                        if (blueprint[i] == null) break;
                        Console.WriteLine(blueprint[i]);
                    }

                    MsgOptions("(s)ave current plane setup", "(q)uit without saving", "()any other key to continue building");
                    saveplane = Console.ReadKey().KeyChar;

                    if (saveplane == 's')
                    {
                        FileAppend("plane.txt", blueprint);
                        MenuPlane(planename, userlogin);
                    }
                    else if (saveplane == 'q')
                    {
                        MenuPlane(planename, userlogin);
                    }
                    break;
                }

                DrawSeatLine();

                MsgOptions("(q)uit seat planner", "(g)enerate plane",
                    "( )any other key to continue building");
                buildplane = Console.ReadKey().KeyChar;

                if (buildplane == 'g')
                {
                    string id = DbNewId(plane);
                    blueprint = DrawSeatPlane(id, seatplan, planename);
                    for (int i = 0; i < blueprint.Length; i++)
                    {
                        if (blueprint[i] == null) break;
                        Console.WriteLine(blueprint[i]);
                    }

                    MsgOptions("(s)ave current plane setup", "(q)uit without saving",
                        "()any other key to continue building");
                    saveplane = Console.ReadKey().KeyChar;

                    if (saveplane == 's')
                    {
                        FileAppend("plane.txt", blueprint);
                        MenuPlane(planename, userlogin);
                    }
                    else if (saveplane == 'q')
                    {
                        MenuPlane(planename, userlogin);
                    }
                }
            } while (buildplane != 'q');
            MenuPlane(planename, userlogin);
        }

        /// <summary>
        /// Menu plane, case built.
        /// </summary>
        /// <param name="planename">Name of the plane to build.</param>
        /// <param name="userlogin">Specific login information.</param>

        static void SubPlaneBuilt(string planename, string[] userlogin)
        {
            Console.Clear();

            string[,] planelist = new string[plane.GetLength(0), 2];

            for (int i = 0; i < plane.GetLength(0); i++)
            {
                planelist[i, 0] = plane[i, 0];
                planelist[i, 1] = plane[i, 1];
            }
            DrawList(planelist, new int[] { 3, 25 }, "Plane");

            int planeid;
            do
            {
                Console.WriteLine("\nChoose an existing plane id to generate seat plan: ");
            } while (!Int32.TryParse(Console.ReadLine(), out planeid) || planeid < 1 || planeid > plane.GetLength(0));

            Console.Clear();
            for (int i = 1; i < plane.GetLength(1); i++)
            {
                if (plane[planeid - 1, i] == "") break;
                Console.WriteLine(plane[planeid - 1, i]);
            }

            MsgPressKey();
            MenuPlane(planename, userlogin);
        }

        /// <summary>
        /// Menu ticket, case departure.
        /// </summary>
        /// <param name="origin">Chosen departure city.</param>
        /// <param name="destination">Chosen return city.</param>
        /// <param name="userlogin">Specific login information.</param>
        /// <returns>Chosen departure seat.</returns>

        static string SubTicketDeparture(string origin, string destination, string[] userlogin)
        {
            if (doption != 0)
            {
                Console.Clear();
                Console.WriteLine("Departure plane already set.");
                MsgPressKey();
                MenuTicket(origin, destination, userlogin);
            }

            string[,] dlist = new string[1, 8];
            dlist = DbOrderFlightDate();
            int dcount = 0;

            for (int i = 0; i < dlist.GetLength(0); i++)
            {
                if (dlist[i, 2] == origin)
                {
                    if (dlist[i, 3] == destination) dcount++;
                }
            }

            string[,] dflist = new string[dcount, 8];
            dcount = 0;
            for (int i = 0; i < dlist.GetLength(0); i++)
            {
                if (dlist[i, 2] == origin)
                {
                    if (dlist[i, 3] == destination)
                    {
                        for (int j = 0; j < dlist.GetLength(1); j++)
                        {
                            dflist[dcount, j] = dlist[i, j];
                        }
                        dcount++;
                    }
                }
            }

            DrawList(dflist, flightlength, "Flight");
            bool dconfirmid = false;

            do
            {
                do { Console.WriteLine("choose departure id: "); } while (!Int32.TryParse(Console.ReadLine(), out doption));

                for (int i = 0; i < dflist.GetLength(0); i++)
                {
                    if (Convert.ToInt32(dflist[i, 0]) == doption)
                    {
                        dconfirmid = true;
                    }
                }
            } while (!dconfirmid);

            Console.Clear();
            FileLoader("plane.txt", 102, ref plane);
            FileLoader("ticket.txt", 5, ref ticket);

            int planeid = Convert.ToInt32(flightlist[doption - 1, 1]);
            char[,] planechar = new char[99, 34];

            string[] seattaken = new string[ticket.GetLength(0)];
            for (int i = 0; i < ticket.GetLength(0); i++)
            {
                if (flightlist[doption - 1, 0] == ticket[i, 1]) seattaken[i] = ticket[i, 3];
            }

            int[,] seattakencoord = new int[seattaken.Length, 2];
            for (int i = 0; i < seattakencoord.GetLength(0); i++)
            {
                if (seattaken[i] != "")
                {
                    for (int j = 0; j < columncoord.GetLength(0); j++)
                    {
                        if (seattaken[i] == null) break;
                        if (seattaken[i].Substring(0, 1) == columncoord[j, 0])
                        {
                            seattakencoord[i, 0] = Convert.ToInt32(columncoord[j, 1]);
                            seattakencoord[i, 1] = Convert.ToInt32(seattaken[i].Substring(1, seattaken[i].Length - 1));
                        }
                    }
                }
            }

            Console.WriteLine(plane[planeid - 1, 2]);
            for (int i = 3; i < planechar.GetLength(0); i++)
            {
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    if (plane[planeid - 1, i] == "") break;
                    planechar[i - 3, j] = Convert.ToChar(plane[planeid - 1, i].Substring(j, 1));
                }
            }

            bool occupied = false;
            for (int i = 0; i < planechar.GetLength(0); i++)
            {
                if (planechar[i, 0] == '\0') break;
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    for (int k = 0; k < seattakencoord.GetLength(0); k++)
                    {
                        if (seattakencoord[k, 0] == 0 && seattakencoord[k, 1] == 0) ;
                        else if (seattakencoord[k, 0] == j && seattakencoord[k, 1] == i)
                        {
                            Console.Write('X');
                            planechar[i, j] = 'X';
                            occupied = true;
                        }
                    }
                    if (!occupied) Console.Write(planechar[i, j]);
                    occupied = false;
                }
                Console.WriteLine();
            }

            char cseatcol;
            int seatrow;

            Console.WriteLine();
            do { Console.Write("Choose seat row (1 to 99, B for business and E for economy seats): "); }
            while (!Int32.TryParse(Console.ReadLine(), out seatrow) || seatrow < 1 || seatrow > 99);
            do { Console.Write("Choose seat column (A to J): "); }
            while (!char.TryParse(Console.ReadLine(), out cseatcol) || cseatcol < 'A' || cseatcol > 'J');

            int[] newseat = new int[2];
            for (int i = 0; i < columncoord.GetLength(0); i++)
            {
                if (columncoord[i, 0] == Convert.ToString(cseatcol)) newseat[0] = Convert.ToInt32(columncoord[i, 1]);
            }
            newseat[1] = seatrow;

            for(int i = 0; i < planechar.GetLength(0); i++)
            {
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    if (newseat[0] == i)
                    {
                        if (newseat[1] == j)
                        {
                            if (planechar[j - 1, i] == 'X') { Console.WriteLine("Seat already taken, choose another."); doption = 0; Console.ReadKey(); return ""; }
                            else if (planechar[j - 1, i] == '_') { Console.WriteLine("Nice Choice."); string departureseat = $"{cseatcol}{seatrow}"; return departureseat; }
                            else { Console.WriteLine("Not a valid option."); doption = 0; Console.ReadKey(); return ""; }
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Menu ticket, case return.
        /// </summary>
        /// <param name="origin">Chosen departure city.</param>
        /// <param name="destination">Chosen return city.</param>
        /// <param name="userlogin">Specific login information.</param>
        /// <returns></returns>

        static string SubTicketReturn(string origin, string destination, string[] userlogin)
        {
            if (roption != 0)
            {
                Console.Clear();
                Console.WriteLine("Return plane already set.");
                MsgPressKey();
                MenuTicket(origin, destination, userlogin);
            }

            if (doption == 0)
            {
                Console.WriteLine("A departure must first be set.");
                MsgPressKey();
                MenuTicket(origin, destination, userlogin);
                return "";
            }

            string[,] rlist = new string[1, 8];
            rlist = DbOrderFlightDate();
            int rcount = 0;

            for (int i = 0; i < rlist.GetLength(0); i++)
            {
                if (rlist[i, 2] == origin)
                {
                    if (rlist[i, 3] == destination) rcount++;
                }
            }

            string[,] rflist = new string[rcount, 8];
            rcount = 0;
            for (int i = 0; i < rlist.GetLength(0); i++)
            {
                if (rlist[i, 2] == origin)
                {
                    if (rlist[i, 3] == destination)
                    {
                        for (int j = 0; j < rlist.GetLength(1); j++)
                        {
                            rflist[rcount, j] = rlist[i, j];
                        }
                        rcount++;
                    }
                }
            }

            DrawList(rflist, flightlength, "Flight");
            bool rconfirmid = false;

            do
            {
                do { Console.WriteLine("choose return id: "); } while (!Int32.TryParse(Console.ReadLine(), out roption));

                for (int i = 0; i < rflist.GetLength(0); i++)
                {
                    if (Convert.ToInt32(rflist[i, 0]) == roption)
                    {
                        rconfirmid = true;
                    }
                }
            } while (!rconfirmid);

            FileLoader("flight.txt", 8, ref flightlist);
            string ddate = $"{flightlist[doption - 1, 6].Substring(6, 4)}{flightlist[doption - 1, 6].Substring(3, 2)}" +
                $"{flightlist[doption - 1, 6].Substring(0, 2)}{flightlist[doption - 1, 7].Substring(0, 2)}{flightlist[doption - 1, 7].Substring(3, 2)}";
            string rdate = $"{flightlist[roption - 1, 6].Substring(6, 4)}{flightlist[roption - 1, 6].Substring(3, 2)}" +
                $"{flightlist[roption - 1, 6].Substring(0, 2)}{flightlist[roption - 1, 7].Substring(0, 2)}{flightlist[roption - 1, 7].Substring(3, 2)}";

            if (Convert.ToInt64(ddate) >= Convert.ToInt64(rdate))
            {
                roption = 0;
                Console.WriteLine("Departure date must be before return date.");
                MsgPressKey();
                MenuTicket(destination, origin, userlogin);
                return "";
            }

            Console.Clear();
            FileLoader("plane.txt", 102, ref plane);
            FileLoader("ticket.txt", 5, ref ticket);

            int planeid = Convert.ToInt32(flightlist[doption - 1, 1]);
            char[,] planechar = new char[99, 34];

            string[] seattaken = new string[ticket.GetLength(0)];
            for (int i = 0; i < ticket.GetLength(0); i++)
            {
                if (flightlist[doption - 1, 0] == ticket[i, 1]) seattaken[i] = ticket[i, 3];
            }

            int[,] seattakencoord = new int[seattaken.Length, 2];
            for (int i = 0; i < seattakencoord.GetLength(0); i++)
            {
                if (seattaken[i] != "")
                {
                    for (int j = 0; j < columncoord.GetLength(0); j++)
                    {
                        if (seattaken[i] == null) break;
                        if (seattaken[i].Substring(0, 1) == columncoord[j, 0])
                        {
                            seattakencoord[i, 0] = Convert.ToInt32(columncoord[j, 1]);
                            seattakencoord[i, 1] = Convert.ToInt32(seattaken[i].Substring(1, seattaken[i].Length - 1));
                        }
                    }
                }
            }

            Console.WriteLine(plane[planeid - 1, 2]);
            for (int i = 3; i < planechar.GetLength(0); i++)
            {
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    if (plane[planeid - 1, i] == "") break;
                    planechar[i - 3, j] = Convert.ToChar(plane[planeid - 1, i].Substring(j, 1));
                }
            }

            bool occupied = false;
            for (int i = 0; i < planechar.GetLength(0); i++)
            {
                if (planechar[i, 0] == '\0') break;
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    for (int k = 0; k < seattakencoord.GetLength(0); k++)
                    {
                        if (seattakencoord[k, 0] == 0 && seattakencoord[k, 1] == 0) ;
                        else if (seattakencoord[k, 0] == j && seattakencoord[k, 1] == i)
                        {
                            Console.Write('X');
                            planechar[i, j] = 'X';
                            occupied = true;
                        }
                    }
                    if (!occupied) Console.Write(planechar[i, j]);
                    occupied = false;
                }
                Console.WriteLine();
            }

            char cseatcol;
            int seatrow;

            Console.WriteLine();
            do { Console.Write("Choose seat row (1 to 99, B for business and E for economy seats): "); }
            while (!Int32.TryParse(Console.ReadLine(), out seatrow) || seatrow < 1 || seatrow > 99);
            do { Console.Write("Choose seat column (A to J): "); }
            while (!char.TryParse(Console.ReadLine(), out cseatcol) || cseatcol < 'A' || cseatcol > 'J');

            int[] newseat = new int[2];
            for (int i = 0; i < columncoord.GetLength(0); i++)
            {
                if (columncoord[i, 0] == Convert.ToString(cseatcol)) newseat[0] = Convert.ToInt32(columncoord[i, 1]);
            }
            newseat[1] = seatrow;

            for (int i = 0; i < planechar.GetLength(0); i++)
            {
                for (int j = 0; j < planechar.GetLength(1); j++)
                {
                    if (newseat[0] == i)
                    {
                        if (newseat[1] == j)
                        {
                            if (planechar[j - 1, i] == 'X') { Console.WriteLine("Seat already taken, choose another."); roption = 0; Console.ReadKey(); return ""; }
                            else if (planechar[j - 1, i] == '_') { Console.WriteLine("Nice Choice."); string returnseat = $"{cseatcol}{seatrow}"; return returnseat; }
                            else { Console.WriteLine("Not a valid option."); roption = 0; Console.ReadKey(); return ""; }
                        }
                    }
                }
            }
            return "";
        }

        /// <summary>
        /// Menu ticket, case buy.
        /// </summary>
        /// <param name="userlogin">Chosen departure city.</param>
        /// <param name="departureseat">Chosen return city.</param>
        /// <param name="returnseat">Specific login information.</param>

        static void SubTicketBuy(string[] userlogin, string departureseat, string returnseat)
        {
            Console.Clear();

            if (doption != 0)
            {
                FileLoader("ticket.txt", 5, ref ticket);
                FileLoader("flight.txt", 8, ref flightlist);

                DrawInvoice(userlogin, doption, departureseat, roption, returnseat);
                Console.WriteLine("\nTicket generated, have a great trip!!");
                MsgPressKey();

                string[] ticketinfo = new string[5];
                ticketinfo[0] = DbNewId(ticket);
                ticketinfo[1] = Convert.ToString(doption);
                ticketinfo[2] = userlogin[0];
                ticketinfo[3] = departureseat;
                ticketinfo[4] = DateTime.Now.ToString("MM/dd/yyyy");
                FileAppend("ticket.txt", ticketinfo);
                FileLoader("ticket.txt", 5, ref ticket);

                ticketinfo[0] = DbNewId(ticket);
                ticketinfo[1] = Convert.ToString(roption);
                ticketinfo[2] = userlogin[0];
                ticketinfo[3] = returnseat;
                ticketinfo[4] = DateTime.Now.ToString("MM/dd/yyyy");
                FileAppend("ticket.txt", ticketinfo);

                doption = 0; roption = 0;

                MenuFlight(userlogin);
            }
            else
            {
                Console.WriteLine("Departure must be set before generating ticket");

                MsgPressKey();
                MenuFlight(userlogin);
            }
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

        /// <summary>
        /// Converts a previously formatted matrix on a file onto a integer matrix.
        /// </summary>
        /// <param name="file">File to get the coordinates from.</param>
        /// <param name="x">Height of the matrix on file.</param>
        /// <param name="y">Length of the matrix on file.</param>
        /// <returns>Integer coordinate matrix.</returns>

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
            string empty = null;
            FileReader("plane.txt", ref empty);
            if (empty != "")
            {
                File.AppendAllText(file, Environment.NewLine);
            }
            for (int i = 0; i < contents.Length; i++)
            {
                File.AppendAllText(file, contents[i] + Environment.NewLine);
            }
            File.AppendAllText(file, "*");
        }

        /// <summary>
        /// Prints menu options on screen.
        /// </summary>
        /// <param name="options"> Indeterminate number of string variables to be printed. </param>

        static void MsgOptions(params string[] options)
        {
            Console.WriteLine();
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine(options[i]);
            }
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
        /// Orders a string list by its already set field of date and time.
        /// </summary>
        /// <returns>String list order by date time previously set.</returns>

        static string[,] DbOrderFlightDate()
        {
            FileLoader("flight.txt", 8, ref flightlist);
            string[,] flightsort = new string[flightlist.GetLength(0), 8];
            string[] flightdate = new string[flightlist.GetLength(0)];
            string[] id = new string[flightlist.GetLength(0)];

            for (int i = 0; i < flightlist.GetLength(0); i++)
            {
                flightdate[i] = $"{flightlist[i, 6].Substring(6, 4)}{flightlist[i, 6].Substring(3, 2)}" +
                    $"{flightlist[i, 6].Substring(0, 2)}{flightlist[i, 7].Substring(0, 2)}{flightlist[i, 7].Substring(3, 2)}";
                id[i] = flightlist[i, 0];
            }
            Array.Sort(flightdate, id);

            for (int i = 0; i < flightlist.GetLength(0); i++)
            {
                for (int j = 0; j < flightlist.GetLength(1); j++)
                {
                    flightsort[i, j] = flightlist[Convert.ToInt32(id[i]) - 1, j];
                }
            }

            return flightsort;
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
        /// Draws final ticket according to parameters set by the user.
        /// </summary>
        /// <param name="userlogin">Login details of the user.</param>
        /// <param name="doption">Flight departure id option.</param>
        /// <param name="departureseat">Flight departure seat option.</param>
        /// <param name="roption">Flight return id option.</param>
        /// <param name="returnseat">Flight return seat option.</param>

        static void DrawInvoice(string[] userlogin, int doption, string departureseat, int roption, string returnseat)
        {
            FileLoader("plane.txt", 102, ref plane);
            FileLoader("flight.txt", 8, ref flightlist);

            string[] flightdeparture = new string[flightlist.GetLength(1)];
            string[] flightreturn = new string[flightlist.GetLength(1)];

            for (int i = 0; i < flightlist.GetLength(1); i++)
            {
                flightdeparture[i] = flightlist[doption - 1, i];
                if(roption != 0) flightreturn[i] = flightlist[roption - 1, i];
            }

            string planedeparture = plane[Convert.ToInt32(flightdeparture[1]) - 1, 1];
            string planereturn = "";
            if (roption != 0) planereturn = plane[Convert.ToInt32(flightreturn[1]) - 1, 1];

            string typedeparture = "";
            int iddeparture = Convert.ToInt32(departureseat.Substring(1, departureseat.Length - 1)) + 2;
            string dt = plane[Convert.ToInt32(flightdeparture[1]) - 1, iddeparture].Substring(33, 1);
            string costdeparture = null;
            double finalcostd;
            if (dt == "B")
            {
                typedeparture = "Business";
                costdeparture = flightdeparture[5];
            }
            else if (dt == "E")
            {
                typedeparture = "Economy";
                costdeparture = flightdeparture[4];
            }
            finalcostd = Convert.ToDouble(costdeparture) * 1.23;

            string typereturn = "";
            double finalcostr = 0;
            string costreturn = null;
            if (roption != 0)
            {
                int idreturn = Convert.ToInt32(departureseat.Substring(1, departureseat.Length - 1)) + 2;
                string rt = plane[Convert.ToInt32(flightdeparture[1]) - 1, iddeparture].Substring(33, 1);
                if (rt == "B")
                {
                    typereturn = "Business";
                    costreturn = flightreturn[5];
                }
                else if (rt == "E")
                {
                    typereturn = "Economy";
                    costreturn = flightreturn[4];
                }
                finalcostr = Convert.ToDouble(costreturn) * 1.23;
            }

            Console.WriteLine($" _________________________________________________________________________________________ ");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|                                                            __/\\__                       |");
            Console.WriteLine($"|   Name:      {userlogin[1]}{new String(' ', 45 - userlogin[1].Length)}`==/\\==`                      |");
            Console.WriteLine($"|   Address:   {userlogin[3]}{new String(' ', 35 - userlogin[3].Length)}____________/__\\____________            |");
            Console.WriteLine($"|   Telephone: {userlogin[4]}{new String(' ', 34 - userlogin[4].Length)}/____________________________\\           |");
            Console.WriteLine($"|                                                  __||__||__/.--.\\__||__||__             |");
            Console.WriteLine($"|                                                 /__|___|___( >< )___|___|__\\            |");
            Console.WriteLine($"|                                                           _/`--`\\_                      |");
            Console.WriteLine($"|                                                          (/------\\)                     |");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|   DEPARTURE DETAILS: ________________________________________________________________   |");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|   Flight Id:   {flightdeparture[0]}{new String(' ', 37 - flightdeparture[0].Length)}Plane seat:  {departureseat}{new String(' ', 23 - departureseat.Length)}|");
            Console.WriteLine($"|   Plane:       {planedeparture}{new String(' ', 37 - planedeparture.Length)}Type:        {typedeparture}{new String(' ', 23 - typedeparture.Length)}|");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|   Origin:      {flightdeparture[2]}{new String(' ', 37 - flightdeparture[2].Length)}Cost:        {costdeparture}{new String(' ', 23 - costdeparture.Length)}|");
            Console.WriteLine($"|   Destination: {flightdeparture[3]}{new String(' ', 37 - flightdeparture[3].Length)}IVA:         23%                    |");
            Console.WriteLine($"|                                                     TOTAL:       {Math.Round(finalcostd, 2)} EUR{new String(' ', 19 - Convert.ToString(finalcostd).Length)}|");
            Console.WriteLine($"|   Day:         {flightdeparture[6]}{new String(' ', 73 - flightdeparture[6].Length)}|");
            Console.WriteLine($"|   Hour:        {flightdeparture[7]}{new String(' ', 73 - flightdeparture[7].Length)}|");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|   RETURN DETAILS: ___________________________________________________________________   |");
            if (roption == 0) for (int i = 0; i < 10; i++) Console.WriteLine($"|{new String(' ', 89)}|");
            else
            {
                Console.WriteLine($"|                                                                                         |");
                Console.WriteLine($"|   Flight Id:   {flightreturn[0]}{new String(' ', 37 - flightreturn[0].Length)}Plane seat:  {returnseat}{new String(' ', 23 - returnseat.Length)}|");
                Console.WriteLine($"|   Plane:       {planereturn}{new String(' ', 37 - planereturn.Length)}Type:        {typereturn}{new String(' ', 23 - typereturn.Length)}|");
                Console.WriteLine($"|                                                                                         |");
                Console.WriteLine($"|   Origin:      {flightreturn[2]}{new String(' ', 37 - flightreturn[2].Length)}Cost:        {costreturn}{new String(' ', 23 - costreturn.Length)}|");
                Console.WriteLine($"|   Destination: {flightreturn[3]}{new String(' ', 37 - flightreturn[3].Length)}IVA:         23%                    |");
                Console.WriteLine($"|                                                     TOTAL:       {Math.Round(finalcostr, 2)} EUR{new String(' ', 19 - Convert.ToString(finalcostr).Length)}|");
                Console.WriteLine($"|   Day:         {flightreturn[6]}{new String(' ', 73 - flightreturn[6].Length)}|");
                Console.WriteLine($"|   Hour:        {flightreturn[7]}{new String(' ', 73 - flightreturn[7].Length)}|");
                Console.WriteLine($"|                                                                                         |");
            }
            Console.WriteLine($"|   TOTAL: ____________________________________________________________________________   |");
            Console.WriteLine($"|                                                                                         |");
            Console.WriteLine($"|                                                     TOTAL:       {Math.Round(finalcostd + finalcostr, 2)} EUR{new String(' ', 19 - Convert.ToString(Math.Round(finalcostd + finalcostr, 2)).Length)}|");
            Console.WriteLine($"|_________________________________________________________________________________________|");
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

        static string[] DrawSeatPlane(string id, string[] seatplan, string planename)
        {
            Console.Clear();

            string[] blueprint = new string[102];
            string a = "";
            int xaxis = 1;

            blueprint[0] = id;
            blueprint[1] = $"{planename.ToUpper()}";
            blueprint[2] = $"{new string(' ', 6)}A B C   D E F G   H I J";
            int count = 3;

            for (int i = 0; i < seatplan.Length / 3; i += 3)
            {
                for (int j = 0; j < Convert.ToInt32(seatplan[i + 2]); j++)
                {
                    a = seatline[Convert.ToInt32(seatplan[i])];

                    if (seatplan[i + 1] == "e")
                    {
                        if (xaxis < 10)
                        {
                            blueprint[count] = Convert.ToString(xaxis) + "  " 
                                + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|")) + " Economy";
                        }
                        else
                        {
                            blueprint[count] = Convert.ToString(xaxis) + " " 
                                + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|")) + " Economy";
                        }
                    }
                    else if (seatplan[i + 1] == "b")
                    {
                        if (xaxis < 10)
                        {
                            blueprint[count] = Convert.ToString(xaxis) + "  "
                                + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|")) + " Business";
                        }
                        else
                        {
                            blueprint[count] = Convert.ToString(xaxis) + " "
                                + a.Substring(a.IndexOf("|"), a.Length - a.IndexOf("|")) + " Business";
                        }
                    }

                    xaxis++; count++;
                }
            }
            return blueprint;
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

        /// <summary>
        /// Draws a plane on sub coordinates inside a drawing matrix.
        /// </summary>
        /// <param name="xcoord">X coordinate of the place to start sub drawing.</param>
        /// <param name="ycoord">Y coordinate of the place to start sub drawing.</param>

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
            Console.WriteLine("\n");
        }

        /// <summary>
        /// Draws one of many possible best paths bettween two poins on a matrix.
        /// </summary>
        /// <param name="ogx">X coordinate of the origin.</param>
        /// <param name="ogy">Y coordinate of the origin.</param>
        /// <param name="dtx">X coordinate of the departure.</param>
        /// <param name="dty">Y coordinate of the departure.</param>
        /// <returns>A matrix integer with all point in between.</returns>

        static int[,] DrawMovementBetween(int ogx, int ogy, int dtx, int dty)
        {
            int timetoget;
            if (Math.Abs(dtx - ogx) > Math.Abs(dty - ogy)) timetoget = Math.Abs(dtx - ogx) - 1;
            else timetoget = Math.Abs(dty - ogy) - 1;

            int[,] movement = new int[timetoget, 2];

            int w = 0; 
            while ( w < timetoget)
            {
                if (ogx < dtx) ogx++;
                else if (ogx > dtx) ogx--;
                if (ogy < dty) ogy++;
                else if (ogy > dty) ogy--;

                movement[w, 0] = ogx;
                movement[w, 1] = ogy;

                w++;
            }
            return movement;
        }

        /// <summary>
        /// Make the executing thread to sleep.
        /// </summary>
        /// <param name="t">Time for the execution to freeze.</param>

        static void DrawWait(int t)
        {
            Thread.Sleep(t);
        }
    }
}
