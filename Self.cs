//  Self - A Self-Driving Car simulator application. Calls RESTful API https://smallchallenge.azurewebsites.net/api
//  Author: Leena Kudalkar
//  Dependencies: SelfDrivingCar.DLL - Server class library for car and road entities to call REST Api https://smallchallenge.azurewebsites.net
//                SelfDrivingCar.Client.DLL - Client class library to send POST and GET HTTP Requests and collect response from the RESTful Web API.
//  Version 1.0 - Initial version. Dec 21 2019

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SelfDrivingCar.Entities;
using SelfDrivingCar.Client;

namespace Self
{
    public class Self
    {
        static String AppVersion = "1.0";
        static Double SelfAvgSpeed = 0.0; // m/s
        static Double STOPSIGNDISTANCE = 50; // meters
        static String Applicant = "LeenaKudalkar@yahoo.com";
        static UInt16 MyCourse = 1;

        public static Logger LogIt = new Logger(@".\SelfLOG.txt");
        public static void Main(string[] args)
        {
            Double ZoneDistance = 0.0;
            try
            {
                Console.BackgroundColor = System.ConsoleColor.Black;
                Console.ForegroundColor = System.ConsoleColor.Blue;
                
                LogIt.WriteLine("Self version " + AppVersion , true);

                if (args.Length < 2 || args[1] == "-help" || args[1] == "-?")
                {
                    Usage();
                    Environment.Exit(0);
                }
                else
                {
                    Applicant = args[0];
                    MyCourse = Convert.ToUInt16(args[1]);
                    if (MyCourse < 1 || MyCourse > 3)
                    {
                        Usage();
                        MyCourse = 1;
                    }
                }
            
                LogIt.WriteLine("Hi! I am CarAnnA, your new car. Wish me good luck as I go to the Endpoint to retrieve road details and learn about myself.", true);
                LogIt.WriteLine("STEP 1: I am going to register myself by requesting for a token.", true);
                
                SelfDrivingCarRestClient Driver = new SelfDrivingCarRestClient(5000);
                Driver.Register(new TokenRequest { Name = Applicant, CourseLayout = MyCourse });
                
                CarAction MyCarAct = new CarAction ();
                MyCarAct.Action = "IgnitionOn";
                Driver.DoAction(MyCarAct);
                
                Car MyCar = Driver.GetCar();
                Road MyRoad = Driver.GetRoad();

                LogIt.WriteLine("CarAnnA received a token for Applicant " + Applicant + " ,Road Course " + MyCourse + ". Current speed limit is " + MyRoad.CurrentSpeedLimit.Min  + "-" + MyRoad.CurrentSpeedLimit.Max + " m/s.", true);
                                
                LogIt.WriteLine("Hi, this is CarAnnA again. My engine is " + MyCar.Engine.State + " at the moment, but I will take you on a long drive now. Let's begin...",  true);
                LogIt.WriteLine("For details of variable car and road parameters during the course, check the log SelfLOG.txt after CarAnnA finishes the driving session.", true);
                CruiseControl(10);

                while ( (MyCar.Ignition == "On" || MyCar.Engine.State == "Running") ) 
                {
                    if (MyRoad.CurrentSpeedLimit.Max == 0 && MyRoad.CurrentSpeedLimit.Min == 0 && MyRoad.SpeedLimitAhead.Max == null)
                    {
                        MyCarAct.Action = "Brake";
                        MyCarAct.Force = 6;
                        Driver.DoAction(MyCarAct);
                        LogIt.WriteLine("CarAnnA's driving session is over. I drove " + MyCar.TotalDistanceTravelled + " miles in " + MyCar.TotalTimeTravelled + " seconds at an average speed of " + SelfAvgSpeed, true);
                        MyCarAct.Action = "IgnitionOff";
                        Driver.DoAction(MyCarAct);
                        break;
                    }
                    else if (Convert.ToDouble(MyCar.CurrentVelocity) <= MyRoad.CurrentSpeedLimit.Min)
                    {
                        LogIt.WriteLine("Stepping up on that accelerator!", true);
                        MyCarAct.Action = "Accelerate";
                        MyCarAct.Force = 6;
                        Driver.DoAction(MyCarAct);
                    }
                    else if (Convert.ToDouble(MyCar.CurrentVelocity) <= MyRoad.CurrentSpeedLimit.Max ) //&& MyRoad.SpeedLimitAhead.RemainingDistanceToEnforcement > STOPSIGNDISTANCE)
                    {
                        LogIt.WriteLine("I can go a bit faster than that...", true);
                        MyCarAct.Action = "Accelerate";
                        MyCarAct.Force = 3;
                        Driver.DoAction(MyCarAct);
                    }
                    //7. If the car is approaching a stop sign, it will have a future speed limit maximum and minimum of 0 m/s.  
                    //   It will be this way for 50 meters.  Once in the zone, the future speed limit will return to a standard speed limit with the new speed.
                    else if (MyRoad.SpeedLimitAhead.Max == 0 && MyRoad.SpeedLimitAhead.Min == 0)
                    {
                        ZoneDistance = (Double)MyCar.TotalDistanceTravelled;
                        while ((Double)MyCar.TotalDistanceTravelled - ZoneDistance < STOPSIGNDISTANCE)
                        {
                            LogIt.WriteLine("Approaching STOP sign...", true);
                            MyCarAct.Action = "Brake";
                            MyCarAct.Force = 4;
                            Driver.DoAction(MyCarAct);
                            MyCar = Driver.GetCar();
                            MyRoad = Driver.GetRoad();
                        }
                        MyCarAct.Action = "Brake";
                        MyCarAct.Force = 6;
                        Driver.DoAction(MyCarAct);
                        LogIt.WriteLine("Stopped at the STOP sign...", true);
                    }
                    else    // CASE: (Convert.ToDouble(MyCar.CurrentVelocity) > MyRoad.CurrentSpeedLimit.Min)
                    {
                        LogIt.WriteLine("Slowing down to stay within limits...", true);
                        MyCarAct.Action = "Brake";
                        MyCarAct.Force = 6; 
                        Driver.DoAction(MyCarAct);
                    }
       
                    //Refresh Details
                    MyCar = Driver.GetCar();
                    MyRoad = Driver.GetRoad();
                    SelfAvgSpeed = (SelfAvgSpeed + Convert.ToDouble(MyCar.CurrentVelocity)) / 2;
                    LogIt.WriteLine("CarAnnA Progress: Current Speed: " + MyCar.CurrentVelocity + "; Average speed : " + SelfAvgSpeed + " ; Distance traveled: " + MyCar.TotalDistanceTravelled + " ; Time spent on road : " + MyCar.TotalTimeTravelled, false);
            
                } // While

                LogIt.WriteLine("", true);
                LogIt.WriteLine("Hope you enjoyed the drive. Please check SelfLOG*.txt to see how well I drove, and give your feedback. Good Bye!", true);
                CruiseControl(10);

            }
            catch (Exception e)
            {
                LogIt.errorout("Main", e.Message, -2, true);
            }

            LogIt.CloseLog();

        }

        // CruiseControl - wait for a few seconds 
        public static void CruiseControl(Double seconds)
        {
            DateTime startTime, endTime;

            startTime = System.DateTime.Now;
            endTime = startTime.AddSeconds(seconds);
            do { } while (DateTime.Now < endTime);

        }

        public static void Usage()
        {
            LogIt.WriteLine("USAGE: Self.exe <Applicant@domain.com> <Road Course - values {1,2,3}>", true);
            LogIt.WriteLine("       e.g. Self yourname@gmail.com 2", true);
        }
    }
}

