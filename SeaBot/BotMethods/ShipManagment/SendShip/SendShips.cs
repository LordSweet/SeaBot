// SeaBotCore
// Copyright (C) 2018 - 2019 Weespin
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//  
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System.Collections.Generic;
using System.Linq;

namespace SeaBotCore.BotMethods.ShipManagment.SendShip
{
    using SeaBotCore.Config;
    using SeaBotCore.Data;
    using System.Threading;
    using static SeaBotCore.Task;

    public static  class SendShips
    {
        public static void SendShipsDestination(ShipDestType type)
        {
            var bestships = new Queue<Ship>(Core.LocalPlayer.Ships.Where(n => n.TargetId == 0 && n.Activated != 0 && n.Sent == 0 && !Core.Config.ignoredships.Contains(n.InstId))
                .OrderByDescending(SendingHelper.GetCapacity));
            Thread.Sleep(2000);
            switch (type)
            {
                case ShipDestType.Upgradable:
                    foreach (var ship in bestships)
                    {
                        var dest = Destinations.SendToUpgradable(ship);
                        if (dest != null)
                        {
                            Networking.AddTask(dest);
                        }
                    }
                    break;
                case ShipDestType.Outpost:
                    foreach (var ship in bestships)
                    {

                        var dest = Destinations.SendToOutpost(ship);
                        if (dest != null)
                        {
                            Networking.AddTask(dest);
                        }
                    }
                    break;
                case ShipDestType.Marketplace:
                    foreach (var ship in bestships)
                    {
                       
                       var dest = Destinations.SendToMarketplace(ship);
                        if (dest != null)
                        {
                            Networking.AddTask(dest);
                        }
                    }
                    break;
                case ShipDestType.Contractor:
                    foreach (var ship in bestships)
                    {
                     
                        var dest =   Destinations.SendToContractor(ship);
                        if (dest != null)
                        {
                            Networking.AddTask(dest);
                        }
                    }
                    break;
                case ShipDestType.Auto:
                    
                    SendShips.SendShipsAutoDestination();
                    break;
                case ShipDestType.Wreck:
                    foreach (var ship in bestships)
                    {

                        var dest = Destinations.SendToWreck(ship);
                        if (dest != null)
                        {
                            Networking.AddTask(dest);
                        }
                    }
                    break;


            }
        }

        private const int upgproc = 30;

        private const int outpostproc = 20;

        private const int marketproc = 15;

        private const int contractorproc = 30;

        private const int wreckproc = 5;

        public static void SendShipsAutoDestination()
        {

            var ships = 
                        Core.LocalPlayer.Ships.Where(n => n.Activated != 0 && n.Sent == 0)
                            .OrderByDescending(SendingHelper.GetCapacity);
            IGameTask task;
               foreach (var ship in ships)
               {
                        var perc = PercentageDest();
                        if (contractorproc > perc[ShipDestType.Contractor])
                        {
                            task = Destinations.SendToContractor(ship);
                            if (task != null)
                            {
                                 Networking.AddTask(task);
                                 continue;
                            }
                        }
                        if (upgproc > perc[ShipDestType.Upgradable])
                        {
                     task = Destinations.SendToUpgradable(ship);
                    if (task != null)
                    {
                        Networking.AddTask(task);
                        continue;
                    }
                }
                        if (marketproc > perc[ShipDestType.Marketplace])
                        {
                     task = Destinations.SendToMarketplace(ship);
                    if (task != null)
                    {
                        Networking.AddTask(task);
                        continue;
                    }
                }
                        if (outpostproc > perc[ShipDestType.Outpost])
                        {
                     task = Destinations.SendToOutpost(ship) ;
                    if (task != null)
                    {
                        Networking.AddTask(task);
                        continue;
                    }

                }

                 task = Destinations.SendToContractor(ship);
                if (task != null)
                {
                    Networking.AddTask(task);
                    continue;
                }

                 task = Destinations.SendToUpgradable(ship);
                if (task != null)
                {
                    Networking.AddTask(task);
                    continue;
                }

                 task = Destinations.SendToMarketplace(ship);
                if (task != null)
                {
                    Networking.AddTask(task);
                    continue;
                }

                 task = Destinations.SendToOutpost(ship);
                if (task != null)
                {
                    Networking.AddTask(task);
                    continue;
                }
               

            }
                  
        }

        public static Dictionary<ShipDestType,double> PercentageDest()
        {
            var temp = new Dictionary<ShipDestType,double>();
            temp.Add(ShipDestType.Upgradable,GetPercentage(ShipDestType.Upgradable));
            temp.Add(ShipDestType.Contractor,GetPercentage(ShipDestType.Contractor));
            temp.Add(ShipDestType.Marketplace,GetPercentage(ShipDestType.Marketplace));
            temp.Add(ShipDestType.Outpost,GetPercentage(ShipDestType.Outpost));
            temp.Add(ShipDestType.Wreck,GetPercentage(ShipDestType.Wreck));
            return temp;
        }
        public static double GetPercentage(ShipDestType dest)
        {
            int count = 0;
            switch (dest)
            {
                case ShipDestType.Upgradable:
                    count = Core.LocalPlayer.Ships.Where(n => n.Sent != 0 && n.Activated!=0).Count(n => n.Type == "upgradeable");
                    break;
                case ShipDestType.Outpost:
                    count = Core.LocalPlayer.Ships.Where(n => n.Sent != 0 && n.Activated!=0).Count(n => n.Type == "outpost");
                    break;
                case ShipDestType.Marketplace:
                    count = Core.LocalPlayer.Ships.Where(n => n.Sent != 0 && n.Activated!=0).Count(n => n.Type == "marketplace");
                    break;
                case ShipDestType.Contractor:
                    count = Core.LocalPlayer.Ships.Where(n => n.Sent != 0 && n.Activated!=0).Count(n => n.Type == "contractor");
                    break;
                
                case ShipDestType.Wreck:
                    count = Core.LocalPlayer.Ships.Where(n => n.Sent != 0 && n.Activated!=0).Count(n => n.Type == "wreck");
                    break;
            }

            if (count == 0)
            {
                return 0;
            }
            var perc = 100D / (Core.LocalPlayer.Ships.Count(n => n.Sent != 0 && n.Activated != 0)
                               / (double)count);
            return perc;
        }
    }
}
