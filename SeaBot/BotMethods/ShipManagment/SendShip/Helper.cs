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
using System.Text;
using System.Threading.Tasks;

namespace SeaBotCore.BotMethods.ShipManagment.SendShip
{
    using SeaBotCore.Cache;
    using SeaBotCore.Config;
    using SeaBotCore.Data;
    using SeaBotCore.Data.Definitions;
    using SeaBotCore.Data.Materials;
    using SeaBotCore.Localizaion;
    using SeaBotCore.Logger;
    using SeaBotCore.Utils;
    using SeaBotCore.Data.Extensions;

    public static class SendingHelper
    {
         public static int GetPercentage(int procent, int total)
        {
            var globalkoef = total / 100D;
            var loccoef = globalkoef * procent;
            return (int) Math.Truncate(loccoef);
        }

        private static int _upgcounter;
        public static bool Between(int num, int lower, int upper, bool inclusive = false)
        {
            return inclusive
                       ? lower <= num && num <= upper
                       : lower < num && num < upper;
        }
        public static int GetNextUpgradableItem()
        {
            var ret = 0;
            if(Core.Config.upgradableType== UpgradableType.Manual)
            {
                if(Core.Config.upgitems.Count==0)
                {
                    return 0;
                }
                if(_upgcounter>Core.Config.upgitems.Count)
                {
                    _upgcounter = 0;
                }
                var item = Core.Config.upgitems[_upgcounter];
                _upgcounter++;
                return item;
            }
            if (Core.Config.upgradableType == UpgradableType.FullAuto)
            {
                //todo: Fuel priority
                //calculate how much is for contractors 
                int noncontractor = 0;
                var workingships = Core.LocalPlayer.Ships.Where(n => n.Activated != 0 && n.Type == "upgradeable" && n.TravelTime() > 0);
                foreach (var ship in workingships)
                {
                    var destinationid = LocalDefinitions.Upgradables.Where(n => n.DefId == ship.TargetId).First().MaterialId;
                    if (Between(destinationid, 1, 6, true))
                        {
                        noncontractor++;
                    }
                }
                if(noncontractor<workingships.Count()/2)
                {
                    //return contractor item
                    var neededitems = AutoTools.NeededItemsForContractor();
                    if(neededitems.Count==0) { return 0; }
                    return neededitems.Min(n => n.Value);
                }
                else
                {
                    var neededitems = AutoTools.NeededItemsForUpgradePercentage().OrderByDescending(n => n.Value).Select(b => b.Key).ToList();
                    return neededitems.FirstOrDefault();
                    //return 
                }

            }
            return ret;
        }
        public static Upgradeable GetBestUpgPlace(int id, int sailors, UpgradablyStrategy upgradablestrategy)
        {
            var mat =id;
            var needed = new List<UpgradeableDefenition.Upgradable>();
            foreach (var item in LocalDefinitions.Upgradables)
            {
                if (item.MaterialId == mat)
                {
                    needed.Add(item);
                }
            }

            var sito = needed.Where(
                shtItem => Core.LocalPlayer.Upgradeables.FirstOrDefault(
                               n => n.DefId == shtItem.DefId && n.Amount != 0 && n.Progress < n.Amount
                                    || n.DefId == shtItem.DefId && shtItem.MaxLevel == 1 && shtItem.EventId == 0)
                           != null);
            var p = sito.ToDictionary(
                shtItem => shtItem,
                shtItem => Core.LocalPlayer.Upgradeables.First(n => n.DefId == shtItem.DefId));
            var best = new Dictionary<Upgradeable, decimal>();
            foreach (var up in p)
            {
                if (Core.Config.ignoreddestination.Any(
                        b => b.Destination == ShipDestType.Upgradable && b.DefId == up.Key.DefId))
                {
                    continue;
                }
                if (up.Key.Levels.Level.First(n => n.Id == up.Value.Level).Sailors > sailors)
                {
                    continue;
                }

                if (upgradablestrategy == UpgradablyStrategy.Loot)
                {
                    var itemFirst = up.Key.Levels.Level.First(n => n.Id == up.Value.Level);
                    var time = (decimal)itemFirst.TravelTime;
                    var koef = itemFirst.MaterialKoef;
                    var timepercoin = koef / time;
                    best.Add(up.Value, timepercoin);
                }
                else
                {
                    var koef = (decimal)up.Key.Levels.Level.First(n => n.Id == up.Value.Level).MaterialKoef;
                    var timepercoin = koef / sailors;
                    best.Add(up.Value, timepercoin);
                }
            }

            var bestplace = best.OrderBy(n => n.Value).LastOrDefault();
            return bestplace.Key;
        }

        public static int GetCapacity(Ship ship)
        {
           
                if (LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId)?.Levels == null)
                {
                    var capacity = LocalDefinitions.Ships.First(n => n.DefId == ship.DefId).CapacityLevels.Level
                        .FirstOrDefault(n => n.Id == ship.CapacityLevel)?.Capacity;
                    if (capacity != null)
                    {
                        return capacity.Value;
                    }
                }

                ShipDefenitions.Ship first = null;
                foreach (var n in LocalDefinitions.Ships)
                {
                    if (n.DefId == ship.DefId)
                    {
                        first = n;
                        break;
                    }
                }

                if (first != null)
                {
                    var pr = first.Levels.Level.FirstOrDefault(n => n.Id == ship.Level);
                    if (pr != null)
                    {
                        return pr.Capacity;
                    }
                }

                return 0;
            
         
        }

        public static ShipDefenitions.LevelsLevel GetLevels(Ship ship, int level)
        {
            return GetShipDefId(ship).Levels.Level.FirstOrDefault(n => n.Id == level);
        }

        public static int GetSailors(Ship ship)
        {
            if (LocalDefinitions.Ships.First(n => n.DefId == ship.DefId).SailorsLevels == null)
            {
                ShipDefenitions.Ship first = null;
                foreach (var n in LocalDefinitions.Ships)
                {
                    if (n.DefId == ship.DefId)
                    {
                        first = n;
                        break;
                    }
                }

                if (first != null)
                {
                    return first.Levels.Level.First(n => n.Id == ship.Level).Sailors;
                }
            }

            var sailors = LocalDefinitions.Ships.First(n => n.DefId == ship.DefId).SailorsLevels.Level
                .First(n => n.Id == ship.SailorsLevel).Sailors;
            if (sailors != null)
            {
                return sailors.Value;
            }

            return int.MinValue;
        }

        public static ShipDefenitions.Ship GetShipDefId(Ship ship)
        {
            return LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId);
        }

        public static string GetTravelName(Ship ship)
        {
            var pointname = string.Empty;
            switch (ship.Type)
            {
                case "upgradeable":
                    pointname = LocalDefinitions.Upgradables.FirstOrDefault(n => n.DefId == ship.TargetId)?.NameLoc;
                    break;
                case "marketplace":
                    pointname = LocalDefinitions.Marketplaces.FirstOrDefault(n => n.DefId == ship.TargetId)?.NameLoc;
                    break;
                case "wreck":

                    var wrk = Core.LocalPlayer.Wrecks.FirstOrDefault(n => n.InstId == ship.TargetId);
                    pointname = LocalDefinitions.Wrecks.FirstOrDefault(n => n.DefId == wrk.DefId)?.NameLoc;
                    break;
                case "contractor":

                    pointname = LocalDefinitions.Contractors.FirstOrDefault(n => n.DefId == ship.TargetId)?.NameLoc;

                    break;
                case "global_contractor":

                    pointname = LocalDefinitions.GlobalContractors.FirstOrDefault(n => n.DefId == ship.TargetId)?.NameLoc;

                    break;
                case "outpost":

                    OutpostDefinitions.Outpost first = null;
                    foreach (var n in LocalDefinitions.Outposts)
                    {
                        if (n.DefId == ship.TargetId)
                        {
                            first = n;
                            break;
                        }
                    }

                    pointname = first?.NameLoc;
                    break;
                case "social_contract":

                    pointname = LocalDefinitions.SocialContracts.FirstOrDefault(n => n.DefId == ship.TargetId)
                        ?.NameLoc;

                    break;
                case "dealer":

                    pointname = LocalDefinitions.Dealers.FirstOrDefault(n => n.DefId == ship.TargetId)?.NameLoc;

                    break;
            }

            return pointname;
        }

        public static int GetTravelTime(Ship ship)
        {
            int? traveltime = null;
            switch (ship.Type)
            {
                case "upgradeable":
                    traveltime = LocalDefinitions.Upgradables.FirstOrDefault(n => n.DefId == ship.TargetId)?.Levels
                        .Level.FirstOrDefault(n => n.Id == ship.TargetLevel)?.TravelTime;
                    break;
                case "marketplace":
                    traveltime = LocalDefinitions.Marketplaces.FirstOrDefault(n => n.DefId == ship.TargetId)
                        ?.TravelTime;
                    break;
                case "wreck":

                    var wrk = Core.LocalPlayer.Wrecks.FirstOrDefault(n => n.InstId == ship.TargetId);
                    var tm = LocalDefinitions.Wrecks.FirstOrDefault(n => n.DefId == wrk.DefId)?.TravelTime;
                    if (tm != null)
                    {
                        traveltime = (int)tm;
                    }

                    break;
                case "contractor":

                    var l = LocalDefinitions.Contractors.FirstOrDefault(n => n.DefId == ship.TargetId)?.TravelTime;
                    if (l != null)
                    {
                        traveltime = (int)l;
                    }

                    break;
                case "global_contractor":

                    var time = LocalDefinitions.GlobalContractors.FirstOrDefault(n => n.DefId == ship.TargetId)?.TravelTime;
                    if (time != null)
                    {
                        traveltime = (int)time;
                    }

                    break;
                case "outpost":

                    OutpostDefinitions.Outpost first = null;
                    foreach (var n in LocalDefinitions.Outposts)
                    {
                        if (n.DefId == ship.TargetId)
                        {
                            first = n;
                            break;
                        }
                    }

                    traveltime = first.TravelTime;
                    break;
                case "social_contract":
                    var soccontract = Core.LocalPlayer.SocialContracts.Where(n => n.InstId == ship.TargetId)
                        .FirstOrDefault();
                    var travelTime = LocalDefinitions.SocialContracts
                        .FirstOrDefault(n => n.DefId == soccontract?.DefId)?.TravelTime;
                    if (travelTime != null)
                    {
                        traveltime = (int)travelTime;
                    }

                    break;
                case "dealer":

                    var o = LocalDefinitions.Dealers.FirstOrDefault(n => n.DefId == ship.TargetId)?.TravelTime;
                    if (o != null)
                    {
                        traveltime = (int)o;
                    }

                    break;
            }

            return !traveltime.HasValue ? int.MaxValue : traveltime.Value;
        }

        public static List<OutpostDefinitions.Outpost> GetUnlockableOutposts()
        {
            var lockedspots = new List<OutpostDefinitions.Outpost>();
            foreach (var lpost in LocalDefinitions.Outposts)
            {
                if (Core.LocalPlayer.Outposts.Any(n => n.DefId == lpost.DefId))
                {
                    continue;
                }

                // check for level
                if (lpost.ReqLevel < Core.LocalPlayer.Level)
                {
                    if (lpost.UnlockTime <= TimeUtils.GetEpochTime())
                    {
                        // check if unlocked spot
                        if (lpost.RequiredLocations == null)
                        {
                            continue;
                        }

                        if (lpost.EventId != TimeUtils.GetCurrentEvent().DefId)
                        {
                            if (lpost.EventId != 0)
                            {
                                continue;
                            }
                        }

                        foreach (var postreq in lpost.RequiredLocations.Location)
                        {
                            if (postreq.Type == "outpost")
                            {
                                var enogh = true;
                                var reqlids = postreq.Ids.Split(',');
                                foreach (var req in reqlids)
                                {
                                    var num = 0;
                                    if (int.TryParse(req, out num))
                                    {
                                        if (Core.LocalPlayer.Outposts.Where(n => n.Done).All(n => n.DefId != num))
                                        {
                                            enogh = false;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        enogh = false;
                                        break;
                                    }
                                }

                                if (enogh)
                                {
                                    lockedspots.Add(lpost);
                                }
                            }
                        }
                    }
                }
            }

            return lockedspots;
        }

        public static bool IsVoyageCompleted(this Ship ship, int offset = 0)
        {
            return (TimeUtils.FixedUTCTime - TimeUtils.FromUnixTime(ship.Sent)).TotalSeconds  > ship.TravelTime()+offset;
        }

       
        public static void LogUnload(this Ship ship)
        {
            Logger.Info(
                Localization.SHIPS_UNLOADING + LocalizationCache.GetNameFromLoc(
                    LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId)?.NameLoc,
                    LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId)?.Name));
        }

        public static void NullShip(Ship ship)
        {
            ship.Cargo = 0;
            ship.Crew = 0;
            ship.Loaded = 0;
            ship.MaterialId = 0;
            ship.Sent = 0;
            ship.Type = string.Empty;
            ship.TargetId = 0;
            ship.TargetLevel = 0;
        }

       

    }
}
