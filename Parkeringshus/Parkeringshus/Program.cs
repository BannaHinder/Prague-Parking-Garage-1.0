using Microsoft.VisualBasic.FileIO;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Parkeringshus
{
    class Program
    {
        //ANVÄND JOIN OCH SPLIT???
        static int PrintMenu()
        {
            Console.WriteLine("\n\nIf you want to park, press 1\n" +
               "If you want to unpark your vehicle, press 2 \n" +
               "If you want to move a vehicle, press 3\n" +
               "If you want to search a vehicle, press 4\n" +
               "If you want to print entire parking area, press 5");
            
            ///Kör en tryparse för att kolla att det överhuvudtaget går att convertera. 
            var isInt=Int32.TryParse(Console.ReadLine(),out int optionAnswer);
            if (isInt)
            {
                return optionAnswer;
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input, try again.");
                return PrintMenu();
            }

        }
        static string InputParkVehicle()
        {
            Console.Write("\nEnter the type of vehicle you want to park: 1 for Mc and 2 for Car.");

            bool isInt = Int32.TryParse(Console.ReadLine(), out var typeOfVehicle);
            
            if (!isInt||!(typeOfVehicle == 1 || typeOfVehicle == 2))
            {
                Console.WriteLine("Invalid input, try again.");
                return InputParkVehicle();
            }
            else
            {
                Console.Write("\nEnter the registration number of your vehicle: ");
                var regNumber = Console.ReadLine();
                if (regNumber.Length > 10)
                {
                    Console.WriteLine("Your regnumber is too long, please write one again in 10 charachers or less.");
                    return InputParkVehicle();
                }
                else
                {
                    string mcOrCar = typeOfVehicle.ToString();
                    string vehicleANDregNr = mcOrCar + "," + regNumber;
                    return vehicleANDregNr;
                }
            }
        }
        static string GetRegNumber()
        {
            Console.Write("\nIn order to identify, please enter the registration number of your vehicle: ");
            var regNumber = Console.ReadLine();
            if (regNumber.Length > 10)
            {
                Console.WriteLine("Your regnumber is too long, please write one again in 10 charachers or less.");
                return GetRegNumber();
            }
            else
            {
                return regNumber;
            }
        }
        static int SearchForRegNumber(string regNumber,string[] parkingslots)
        {
            int index=0;
            foreach (string slot in parkingslots)
            {
                if (string.IsNullOrEmpty(slot)) { // tom plats, hoppa över
                    index++; 
                    continue; 
                }
                var slotInfo = slot.Split(',');
                // om regnummer är samma eller om det finns två fordon och andra är samma
                if (slotInfo[1] == regNumber || (slotInfo.Length > 2 && slotInfo[3] == regNumber))
                {
                    return index;
                }
                index++;
            }
            // no vechicle found
            return -1;
        }//Ger bara regnummer och uppdaterad array tillbaka
        static string GetVehicleInfo(int index, string regnummer, string[] parkingSLOTS)// Kollar om givet index är bil eller mc
        {
            string type;
            var slot = parkingSLOTS[index].Split(',');
            if (slot[1] == regnummer)
            {
                type = slot[0];
            }
            else
            {
                type = slot[3];
            }
            var vehicleInfo = type + "," + regnummer;
            return vehicleInfo;
        }
        static string[] MoveVehicle(int index,string vehicleinfo,string[] parkingSLOTS,string regNumber)
        {
            Console.WriteLine("So uh... where do you want your vehicle to park instead???????? 1-100.");
            var isInt = Int32.TryParse(Console.ReadLine(), out int slotIndex);
            
            
            if (isInt&&slotIndex<=100&&slotIndex>0)
            {
                var slot = parkingSLOTS[slotIndex - 1];
                if (vehicleinfo[0]=='1') {
                    if (!string.IsNullOrEmpty(slot) && slot[0] == '1' && slot.Split(',').Length < 4) //denna funker tack vare att den slutar kolla efter första 
                                                                                                     //statement som inte stämmer eftersom kompilatorn vet att && betyder att ALLT måste stämma. så även om nästa statement kan ge ett error
                    {   //så kollar den inte det då den redan avbrutit vid första false statement! (detta fick jag lära mig av en programmerare) 
                        parkingSLOTS = UnPark(index, parkingSLOTS, regNumber);
                        parkingSLOTS[slotIndex-1] += "," + vehicleinfo;
                        Console.WriteLine($"Your vehicle has been successfully moved to parkinglot nr{slotIndex}.");
                        return parkingSLOTS;
                    }
                }
                if (string.IsNullOrEmpty(slot))
                {
                    parkingSLOTS = UnPark(index,parkingSLOTS,regNumber);
                    parkingSLOTS[slotIndex-1]= vehicleinfo;
                    Console.WriteLine($"Your vehicle has been successfully moved to parkinglot nr{slotIndex}.");
                    return parkingSLOTS;
                }
                
                Console.WriteLine("Unfortunately, car couldn't be parkD, occupied. Please refer to Menu option No. 5: print parking lot.");
                return parkingSLOTS; 
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Invalid input, try again.");
                return MoveVehicle(index, vehicleinfo,parkingSLOTS,regNumber);
            }
        }
        static  string[] UnPark(int index, string[] parkingslots,string regNumber)
        {
            //splittar upp strängen med parkeringsindex till en array av strängar me "," som skiljetecken
            var slot = parkingslots[index].Split(',');
            if (slot.Length == 2)
            { //Här är det antagligen bara en bil och då nollas hela strängen på en gång :)
                parkingslots[index]="";
            }
            else
            {
                if (slot[1] == regNumber)
                {
                    //sparelisparar bara de delar av splittade strängen ´som ska fortsätta perkilera men med ett komma 
                    //for future purposes you know
                    parkingslots[index] = slot[2] + "," + slot[3];
                }
                else
                {
                    //samma här men andra delen då
                    parkingslots[index] = slot[0]+ "," + slot[1];
                }
            }
            return parkingslots; //skickas tillbaka uppdateraaaaaaaaaaaaaaad
        }
        //FUNKTION FÖR ATT LÄGGA IN vehicleANDregNr i String ARRAY/////////////////////////////////////////////////////////
        //söka igenom arrayen efter tomma strängar först;
        //Sen lägga till 
        static string[] ParkVehicle(string vehicleInfo, string[] parkingSLOTS)
        {
            int index = 0;
            if (vehicleInfo[0]=='1')
            {
                foreach (string slot in parkingSLOTS)
                {

                    if ( !string.IsNullOrEmpty(slot) && slot[0] == '1' && slot.Split(',').Length < 4) //denna funker tack vare att den slutar kolla efter första 
                        //statement som inte stämmer eftersom kompilatorn vet att && betyder att ALLT måste stämma. så även om nästa statement kan ge ett error
                    {   //så kollar den inte det då den redan avbrutit vid första false statement! (detta fick jag lära mig av en programmerare) 
                        parkingSLOTS[index] = parkingSLOTS[index] + "," + vehicleInfo;
                        return parkingSLOTS;
                    }
                    index++;
                }
            }
            index = 0;
            foreach (string slot in parkingSLOTS)
            {
                
                if (string.IsNullOrEmpty(slot))
                {
                    parkingSLOTS[index] = vehicleInfo;
                    return parkingSLOTS;
                }
                index++;
            }
            return parkingSLOTS;
        }

        static void PrintParkingSlotArray(string[] parkingSLOTS) ////////// ENDAST PRINTA PARKERINGEN///////////////////7
        {
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++" +
                "\n1= MC\n2= CAR\nN/A= Empty parkingslot.\n++++++++++++++++++++++++++++++++++++++++++++++");
            for (int i = 0; i < 100; i++)
            {
                if (i % 10 == 0) ////ny rad var 10:e index///// plussa på en 1 på varje index för att det ska stå "rätt"
                {
                    if (string.IsNullOrEmpty(parkingSLOTS[i]))
                    {
                        Console.Write($"\n {i + 1} [N/A]");
                    }
                    else
                    {
                        Console.Write($"\n {i + 1} [ {parkingSLOTS[i]} ]");
                    }  
                }
                else
                {
                    if (string.IsNullOrEmpty(parkingSLOTS[i]))
                    {
                        Console.Write($" {i + 1} [N/A]");
                    }
                    else
                    {
                        Console.Write($" {i + 1} [ {parkingSLOTS[i]} ]");
                    }
                }
            }
        }
        static bool QuitOrContinue()
        {
            Console.WriteLine("\nDo you want to return to the Main Menu? y/n");
            string yesOrNo = Console.ReadLine();
            Console.Clear();
            return (yesOrNo == "y" || yesOrNo == "yes");           
        }

        static void Main(string[] args) /////HÄR BÖRJAR MITT PROGRAM///////////////////////////////////////////////////////////////
        {
            string regNumber;
            bool running = true; // SNODDE DENIS IDE OM EN WHILE RUNNING BOOL FÖR ATT SLIPPA KÖRA OM HELA MAIN FUNKTIONEN VARJE GÅNG///////////////////
            ////////////////////////////////TILLVERKNGING AV STRING ARRAY TILL PARKERINGSPLATS///////////////////////////////////////////////////////////////////////////////
            string[] parkingSlotArray = new string[100];
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n" +
                " Welcome to Prague Parking!! \n Opening hours are 0.00 - 24.00." +
                    "Any vehicles left after 24.00 will be deported.\n" +
                    "+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++\n " +
                    "What do you want to do?");
            while (running) //SJÄLVA HUVUDLOOPEN//
            {
                var optionAnswer = PrintMenu();
                switch (optionAnswer)
                {
                    case 1:
                        //PARKERA BIL/MC /////////////////////////////////////////////////
                        string vehicleInfo = InputParkVehicle();
                        parkingSlotArray = ParkVehicle(vehicleInfo, parkingSlotArray);
                        Console.WriteLine("PARKED!");
                        running = QuitOrContinue();
                        break;
                    case 2:
                        //UNPARK BIL/MC///////////////////////////////////////////////////////////////////////////////////////////////////////////
                        regNumber = GetRegNumber();
                        var index = SearchForRegNumber(regNumber, parkingSlotArray);
                        if (index == -1)
                        { // OM DET RETURNERATS -1 SÅ FINNs EJ FORDONET OCH FELMEDDELANDE SÄNDS
                            Console.WriteLine("COulddn't  find any vehicle with matching index. \n   beep beep boop boop");
                            running = QuitOrContinue();
                            break;
                        }
                        else
                        {
                            UnPark(index, parkingSlotArray, regNumber);
                        }
                        Console.WriteLine($"Your vehicle has been succesfully unparked from slot {index + 1}.");
                        running = QuitOrContinue();
                        break;
                    case 3:
                        //FLYTTA PÅ BIL/MC//////// först: få användar input: spara det: söka igenom array efter fordon, få fordonsinfo; kolla efter efterfrågad
                        //parkeringsruta, om möjligt flytta fordon dit.
                        regNumber = GetRegNumber();
                        index = SearchForRegNumber(regNumber,parkingSlotArray);
                        if (index == -1)
                        { 
                            Console.WriteLine("COulddn't  find any vehicle with matching index, so can't repark. \n   beep beep boop boop");
                            running = QuitOrContinue();
                            break;
                        }
                        else
                        {
                            vehicleInfo = GetVehicleInfo(index,regNumber,parkingSlotArray);
                            parkingSlotArray=MoveVehicle(index, vehicleInfo, parkingSlotArray, regNumber);
                        }
                        running = QuitOrContinue();
                        break;
                    case 4:
                        //SÖKA FORDON PÅ INDEX ELLER REGNUMMER/////////////////////////////////////////////////////////////////////////////////
                        regNumber = GetRegNumber();
                        index = SearchForRegNumber(regNumber, parkingSlotArray);
                        if (index == -1) { Console.WriteLine("Couldn't find any vehicle with matching regnumber."); running = QuitOrContinue(); break; }
                        else
                        {
                            Console.WriteLine($"Your vehicle is in slot nr.{index + 1}. ");
                            running = QuitOrContinue();
                            break;
                        }
                    case 5:
                        //SKRIVA UT PARKERINGEN
                        //KLAR//////////////////////////////////////////////////////////////////////////////////////////////////////////////
                        PrintParkingSlotArray(parkingSlotArray);
                        running = QuitOrContinue();
                        break;
                        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid Input.");
                        running = QuitOrContinue();
                        break;
                }
            }
        }
    }
}
