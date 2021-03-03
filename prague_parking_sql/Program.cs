using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prague_parking_sql
{
    class Program
    {
        public static SqlCommunicator sqlCommunicator = new SqlCommunicator();

        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            bool showMenu = true;
            while (showMenu)
            {
                showMenu = Menu();
            }
        }

        static bool Menu()
        {
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1) Add vehicle");
            Console.WriteLine("2) Get vehicle");
            Console.WriteLine("3) Move vehicle");
            Console.WriteLine("4) Search vehicle");
            Console.WriteLine("5) Information about current parking lots");
            Console.WriteLine("6) Parking lot history");
            Console.WriteLine("7) Save & exit");

            switch (Console.ReadLine())
            {
                case "1":
                    Console.Clear();
                    AddVehicle();
                    return true;
                case "2":
                    Console.Clear();
                    GetVehicle();
                    return true;
                case "3":
                    Console.Clear();
                    MoveVehicle();
                    return true;
                case "4":
                    Console.Clear();
                    SearchVehicle();
                    return true;
                case "5":
                    Console.Clear();
                    GetCurrentPlots();
                    return true;

                case "6":
                    Console.Clear();
                    GetHistoryPlots();
                    return true;

                case "7":
                    Console.Clear();
                    Exit();
                    return false;

                default:
                    return true;
            }
        }
        
        static void AddVehicle()
        {
        restartRegNumber:
            Console.WriteLine("Enter registration number: ");
            string regNumber = Console.ReadLine();
            char[] regNumberContent = new char[regNumber.Length];

            for (int i = 0; i < regNumber.Length; i++)
            {
                if (!Char.IsLetterOrDigit(regNumber[i]))
                {
                    Console.WriteLine("Only letters and digits please");
                    goto restartRegNumber;
                }
            }

        restartVehicleType:
            Console.WriteLine("Is it a car or a mc: ");
            string vehicleType = Console.ReadLine();

            if (vehicleType == "car" || vehicleType == "Car" || vehicleType == "CAR")
            {
                //Call add vehicle from sqlCommunicator
                Console.WriteLine(sqlCommunicator.Save(regNumber, 1) + "\n");
            }
            else if (vehicleType == "mc" || vehicleType == "Mc" || vehicleType == "MC")
            {
                //Call add vehicle from sqlCommunicator
                Console.WriteLine(sqlCommunicator.Save(regNumber, 2) + "\n");
            }
            else
            {
                Console.WriteLine("That's not an alternative, try again");
                goto restartVehicleType;
            }
        }

        static void GetVehicle()
        {
            Console.WriteLine("Enter registration number: ");
            string regNumber = Console.ReadLine();

            Console.WriteLine(sqlCommunicator.GetVehicle(regNumber));
        }
       
        static void GetCurrentPlots()
        {
            Console.WriteLine(sqlCommunicator.CurrentPlots());                        
        }

        private static void GetHistoryPlots()
        {
            Console.WriteLine(sqlCommunicator.HistoryPlots());
        }

        static void MoveVehicle()
        {
        restart:
            Console.WriteLine("Enter registration number: ");
            string regNumber = Console.ReadLine();

        
            int placeToMove;
            Console.WriteLine("Where do you want to move it (1-100)?");
            bool couldParse = int.TryParse(Console.ReadLine(), out placeToMove);
            

            if(couldParse && placeToMove >= 1 && placeToMove <= 100 && !string.IsNullOrEmpty(regNumber))
            {
                Console.WriteLine(sqlCommunicator.MoveVehicle(regNumber, placeToMove));
            
            } else
            {
                Console.WriteLine("Wrong input, try again \n");
                goto restart;
            }
        }

        private static void SearchVehicle()
        {
            Console.WriteLine("Enter registration number: ");
            string regNumber = Console.ReadLine();

            Console.WriteLine(sqlCommunicator.SearchVehicle(regNumber));
        }

        static void Exit()
        {
            Environment.Exit(0);
        }
    }
}
