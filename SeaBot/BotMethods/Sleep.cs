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
namespace SeaBotCore.BotMethods
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using SeaBotCore.BotMethods.ShipManagment.SendShip;
    using SeaBotCore.Data.Definitions;
    using SeaBotCore.Localizaion;
    using SeaBotCore.Logger;
    using SeaBotCore.Utils;

    #endregion
    using SeaBotCore.Data.Extensions;
    public class Sleeping
    {
        public static void Sleep()
        {
            if (Core.Config.sleepevery != 0)
            {
                if ((DateTime.Now - Core.lastsleep).TotalMinutes >= (Core.Config.sleepeveryhrs
                                                                         ? Core.Config.sleepevery * 60
                                                                         : Core.Config.sleepevery))
                {
                    var sleeptimeinmin = 0;

                    Core.lastsleep = DateTime.Now;
                    if (Core.Config.smartsleepenabled)
                    {
                        // 10 min
                        var thresholdinmin = 20;
                        var DelayMinList = new List<int>();
                        foreach (var ship in Core.LocalPlayer.Ships.Where(n => n.Activated != 0))
                        {
                            if (ship.Sent != 0)
                            {
                                DelayMinList.Add(ship.TravelTime() / 60);
                            }
                        }

                        foreach (var building in Core.LocalPlayer.Buildings)
                        {
                            if (building.ProdStart != 0)
                            {
                                var willbeproducedat = building.ProdStart + LocalDefinitions.Buildings
                                                           .First(n => n.DefId == building.DefId).BuildingLevels.Level
                                                           .First(n => n.Id == building.Level).ProdOutputs.ProdOutput[0]
                                                           .Time;

                                // lol xD
                                DelayMinList.Add(
                                    (int)Math.Ceiling(
                                        (TimeUtils.FromUnixTime(willbeproducedat) - TimeUtils.FixedUTCTime)
                                        .TotalMinutes));
                            }

                            if (building.UpgStart != 0)
                            {
                                var willbeproducedat = building.UpgStart + LocalDefinitions.Buildings.First(n => n.DefId == building.DefId).BuildingLevels.Level.First(n => n.Id == building.Level + 1).UpgradeTime;

                                DelayMinList.Add(
                                    (int)Math.Ceiling(
                                        (TimeUtils.FromUnixTime(willbeproducedat) - TimeUtils.FixedUTCTime)
                                        .TotalMinutes));
                            }
                        }

                        // Find center
                        var a = DelayMinList.Where(n => n > thresholdinmin).GroupBy(i => i);
                        var b = a.OrderByDescending(grp => grp.Count());
                        var mostlikely = b.Select(grp => grp.Key).FirstOrDefault();
                        var avg = (int)DelayMinList.Average();

                        if (mostlikely > avg * 1.5)
                        {
                            sleeptimeinmin = avg > thresholdinmin ? avg : thresholdinmin;
                        }
                        else
                        {
                            sleeptimeinmin = mostlikely > thresholdinmin ? mostlikely : thresholdinmin;
                        }
                    }
                    else
                    {
                        sleeptimeinmin = Core.Config.sleepforhrs ? Core.Config.sleepfor * 60 : Core.Config.sleepfor;
                    }

                    new Task(
                        () =>
                            {
                                Core.StopBot();
                                Logger.Info(string.Format(Localization.SLEEP_STARTING, sleeptimeinmin));
                                Thread.Sleep(sleeptimeinmin * 60 * 1000);
                                Core.StartBot();
                            }).Start();

                    // StartSleeping
                }
            }
        }
    }
}