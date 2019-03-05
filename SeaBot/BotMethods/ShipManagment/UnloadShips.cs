﻿// SeaBotCore
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaBotCore.BotMethods.ShipManagment
{
    using SeaBotCore.BotMethods.ShipManagment.SendShip;
    using SeaBotCore.Data;
    using SeaBotCore.Data.Definitions;
    using SeaBotCore.Data.Extensions;
    using SeaBotCore.Utils;

    public static class UnloadShips
    {
        public static void UnloadAllShips()
        {
            var unloadedships = new List<int>();
            foreach (var shi in Core.LocalPlayer.Ships)
            {
                var ship = Core.LocalPlayer.Ships.Where(n => n.InstId == shi.InstId).First();
                if (ship.TargetId != 0 && ship.Activated != 0 && ship.IsVoyageCompleted())
                {

                    switch (ship.Type)
                    {
                        case "upgradeable":
                            {
                                if (ship.Loaded == 1)
                                {
                                    
                                    unloadedships.Add(ship.DefId);
                                    var ship1 = ship;
                                    int uniqueid = unloadedships.Count(n => n == ship1.DefId);
                                    UnloadUpgradable(ship, uniqueid);
                                }

                                break;
                            }
                        case "marketplace":
                            {
                                unloadedships.Add(ship.DefId);
                                int uniqueid = unloadedships.Count(n => n == ship.DefId);
                                UnloadMarketplace(ship,uniqueid);
                                break;
                            }
                        case "wreck":
                            {
                                unloadedships.Add(ship.DefId);
                                int uniqueid = unloadedships.Count(n => n == ship.DefId);
                               UnloadWreck(ship,uniqueid);
                                break;
                            }
                        case "contractor":
                            {

                                if (ship.Loaded == 1)
                                {
                                    unloadedships.Add(ship.DefId);
                                    int uniqueid = unloadedships.Count(n => n == ship.DefId);
                                    UnloadContractor(ship);
                                }
                                break;
                            }
                        case "outpost":
                            {
                                unloadedships.Add(ship.DefId);
                                int uniqueid = unloadedships.Count(n => n == ship.DefId);
                                UnloadOutpost(ship,uniqueid);
                                break;
                            }
                        case "social_contract":
                            {
                               
                                if (ship.Loaded == 1)
                                {
                                    unloadedships.Add(ship.DefId);
                                    int uniqueid = unloadedships.Count(n => n == ship.DefId);
                                    UnloadSocialcontract(ship, uniqueid);
                                    
                                }
                                break;
                            }
                    }
                }
            }
            unloadedships.Clear();
        }
        public static void UnloadDealer(Ship ship, int uniqueid)
        {
            // if (ship.Type == "dealer")
            // {
            // var predefined = Definitions.DealerDef.Barrels.Barrel.Where(n => n.DefId == ship.TargetId).FirstOrDefault();
            // if (SendingHelper.isVoyageCompleted(ship))
            // {

            // _deship.Add(ship);
            // Networking.AddTask(new Task.UnloadShipTask(ship.InstId,
            // Core.LocalPlayer.BoatLevel, Enums.EObject.dealer,
            // SendingHelper.GetCapacity(ship),
            // 0,
            // SendingHelper.GetSailors(ship), (int)predefined.Sailors,
            // ship.TargetLevel,
            // null, _deship.Count(n => n.DefId == ship.DefId)));
            // SendingHelper.NullShip(Core.LocalPlayer.Ships[index]);
            // }
            // }
        }
        public static void UnloadTreasure(Ship ship, int uniqueid)
        {
            // if (ship.Type == "treasure")
            // {
            // var predefined = Definitions.TreasureDef.Barrels.Barrel.Where(n => n.DefId == ship.TargetId).FirstOrDefault();
            // if (SendingHelper.isVoyageCompleted(ship))
            // {
            // _deship.Add(ship);
            // Networking.AddTask(new Task.UnloadShipTask(ship.InstId,
            // Core.LocalPlayer.BoatLevel, Enums.EObject.treasure,
            // SendingHelper.GetCapacity(ship),
            // 0,
            // SendingHelper.GetSailors(ship), 0,
            // ship.TargetLevel,
            // null, _deship.Count(n => n.DefId == ship.DefId)));
            // SendingHelper.NullShip(Core.LocalPlayer.Ships[index]);
            // }
            // }
        }
        public static void UnloadSocialcontractor(Ship ship, int uniqueid)
        {
            // if (ship.Type == "global_contractor")
            // {
            // var predefined = Definitions.GConDef.Barrels.Barrel.Where(n => n.DefId == ship.TargetId).FirstOrDefault();
            // if (SendingHelper.isVoyageCompleted(ship))
            // {
            // _deship.Add(ship);
            // Networking.AddTask(new Task.UnloadShipGlobalContractorTask(ship.InstId));
            // SendingHelper.NullShip(Core.LocalPlayer.Ships[index]);
            // }
            // }
        }

        public static void UnloadSocialcontract(Ship ship, int uniqueid)
        {
            var co = Core.LocalPlayer.SocialContracts.Where(n => n.InstId == ship.TargetId)
                .FirstOrDefault();
            if (co == null)
            {
                return;
            }

            ship.LogUnload();
            Networking.AddTask(
                new Task.DockShipSocialContractor(
                    ship,
                    false,
                    ship.Capacity(),
                    co.MaterialKoef / co.Amount,
                    ship.Sailors(),
                    co.Sailors,
                    ship.TargetLevel,
                    uniqueid));
            SendingHelper.NullShip(ship);
        }


        public static void UnloadOutpost(Ship ship, int uniqueid)
        {
          
            var loc = Core.LocalPlayer.Outposts.Where(n => n.DefId == ship.TargetId).First();
            ship.LogUnload();
            var outp = Core.LocalPlayer.Outposts.Where(n => n.DefId == ship.TargetId).FirstOrDefault();
            if (outp.CargoOnTheWay <= ship.Sailors())
            {
                outp.CargoOnTheWay -= ship.Sailors();
            }

            Networking.AddTask(
                new Task.DockShipTaskOutPost(
                    ship,
                    false,
                    SendingHelper.GetCapacity(ship),
                    ship.Cargo,
                    SendingHelper.GetSailors(ship),
                    ship.Crew,
                    ship.TargetLevel,
                    loc.CargoOnTheWay + loc.Crew,
                    loc.RequiredCrew,
                  uniqueid));
            SendingHelper.NullShip(ship);
        }
        public static void UnloadUpgradable(Ship ship,int uniqueid)
        {
            var defenition = LocalDefinitions.Upgradables.FirstOrDefault(n => n.DefId == ship.TargetId);
            var lvl = defenition?.Levels.Level.FirstOrDefault(n => n.Id == ship.TargetLevel);
            if (lvl != null)
            {
                ship.LogUnload();
                var upg = Core.LocalPlayer.Upgradeables.FirstOrDefault(n => n.DefId == ship.TargetId);
                if (upg != null)
                {
                    upg.Progress += lvl.MaterialKoef * SendingHelper.GetCapacity(ship);
                }

                Networking.AddTask(
                    new Task.UnloadShipTask(
                        ship.InstId,
                        Core.LocalPlayer.Level,
                        Enums.EObject.upgradeable,
                        SendingHelper.GetCapacity(ship),
                        lvl.MaterialKoef * SendingHelper.GetCapacity(ship),
                        SendingHelper.GetSailors(ship),
                        lvl.Sailors,
                        ship.TargetLevel,
                        null,uniqueid));
                SendingHelper.NullShip(ship);
            }
        }

        public static void UnloadWreck(Ship ship, int uniqueid)
        {
            var wrk = Core.LocalPlayer.Wrecks.Where(n => n.InstId == ship.TargetId).FirstOrDefault();
                    
            if (wrk != null)
            {
             
                ship.LogUnload();
                Networking.AddTask(
                    new Task.UnloadShipTask(
                        ship.InstId,
                        Core.LocalPlayer.Level,
                        Enums.EObject.wreck,
                        SendingHelper.GetCapacity(ship),
                        0,
                        SendingHelper.GetSailors(ship),
                        wrk.Sailors,
                        ship.TargetLevel,
                        null,
                        uniqueid));
                SendingHelper.NullShip(ship);
            }
        }

        public static void UnloadContractor(Ship ship)
        {
            var currentcontractor = LocalDefinitions.Contractors
                .FirstOrDefault(n => n.DefId == ship.TargetId);

            var quest = currentcontractor?.Quests.Quest.FirstOrDefault(n => n.Id == ship.TargetLevel);
            if (quest == null)
            {
                return;
            }
            var uniqueid = ship.TargetId;
            if (currentcontractor.Type == "static")
            {
                var usedshit = quest.MaterialKoef * SendingHelper.GetCapacity(ship);
                var lcontract = Core.LocalPlayer.Contracts.FirstOrDefault(n => n.DefId == ship.TargetId);
                // TODO: increasing of progress or amount!
                ship.LogUnload();
                Networking.AddTask(
                    new Task.DockShipTaskContractor(
                        ship,
                        false,
                        SendingHelper.GetCapacity(ship),
                        usedshit,
                        SendingHelper.GetSailors(ship),
                        currentcontractor.Sailors,
                        ship.TargetLevel,
                        ship.TargetLevel,
                        lcontract.Progress,
                        (int)quest.InputAmount(),
                        quest.ObjectiveTypeId,
                        uniqueid));
            }
            else
            {
                var usedshit = quest.MaterialKoef * SendingHelper.GetCapacity(ship);
                var lcontract = Core.LocalPlayer.Contracts.FirstOrDefault(n => n.DefId == ship.TargetId);
                var inp = quest.InputAmount();
                // TODO: increasing of progress or amount!
                ship.LogUnload();
                Networking.AddTask(
                    new Task.DockShipTaskContractor(
                        ship,
                        false,
                        SendingHelper.GetCapacity(ship),
                        usedshit,
                        SendingHelper.GetSailors(ship),
                        currentcontractor.Sailors,
                        ship.TargetLevel,
                        ship.TargetLevel,
                        lcontract.Progress,
                        (int)quest.InputAmount(),
                        quest.ObjectiveTypeId,
                        uniqueid));  
            }

            SendingHelper.NullShip(ship);
        }


        public static void UnloadMarketplace(Ship ship, int uniqueid)
        {
            var lvl = LocalDefinitions.Marketplaces.FirstOrDefault(n => n.DefId == ship.TargetId);
            var mat = lvl?.Materials.Material.FirstOrDefault(n => n.Id == ship.MaterialId);
            if (mat != null)
            {
                ship.LogUnload();
                Networking.AddTask(
                    new Task.UnloadShipTask(
                        ship.InstId,
                        Core.LocalPlayer.Level,
                        Enums.EObject.marketplace,
                        SendingHelper.GetCapacity(ship),
                        mat.InputKoef * SendingHelper.GetCapacity(ship),
                        SendingHelper.GetSailors(ship),
                        lvl.Sailors,
                        ship.TargetLevel,
                        null,
                        uniqueid));
                SendingHelper.NullShip(ship);
            }
        }
    }
}
