﻿// SeabotGUI
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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeaBotCore;
using SeaBotCore.Data;
using SeaBotCore.Data.Defenitions;
using SeaBotCore.Utils;
using Task = System.Threading.Tasks.Task;

namespace SeaBotGUI.GUIBinds
{
    public static class RichTextBoxExtensions
    {
        public static void AppendText(RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
            box.ScrollToCaret();
        }
    }

    public static class ResourcesBox
    {
        public static void Update()
        {
            var coins = Core.GlobalData.GetAmountItem("coins");
            if (Form1.instance.CoinsLabel.InvokeRequired)
            {
                Form1.instance.CoinsLabel.Invoke(
                    new Action(() => { Form1.instance.CoinsLabel.Text = coins.ToString(); }));
            }
            else
            {
                Form1.instance.CoinsLabel.Text = coins.ToString();
            }

            var fish = Core.GlobalData.GetAmountItem("fish");
            if (Form1.instance.FishLabel.InvokeRequired)
            {
                Form1.instance.FishLabel.Invoke(new Action(() => { Form1.instance.FishLabel.Text = fish.ToString(); }));
            }
            else
            {
                Form1.instance.FishLabel.Text = fish.ToString();
            }

            var iron = Core.GlobalData.GetAmountItem("iron");
            if (Form1.instance.IronLabel.InvokeRequired)
            {
                Form1.instance.IronLabel.Invoke(new Action(() => { Form1.instance.IronLabel.Text = iron.ToString(); }));
            }
            else
            {
                Form1.instance.IronLabel.Text = iron.ToString();
            }

            var gem = Core.GlobalData.GetAmountItem("gem");
            if (Form1.instance.GemLabel.InvokeRequired)
            {
                Form1.instance.GemLabel.Invoke(new Action(() => { Form1.instance.GemLabel.Text = gem.ToString(); }));
            }
            else
            {
                Form1.instance.GemLabel.Text = gem.ToString();
            }

            var wood = Core.GlobalData.GetAmountItem("wood");
            if (Form1.instance.WoodLabel.InvokeRequired)
            {
                Form1.instance.WoodLabel.Invoke(new Action(() => { Form1.instance.WoodLabel.Text = wood.ToString(); }));
            }
            else
            {
                Form1.instance.WoodLabel.Text = wood.ToString();
            }

            var stone = Core.GlobalData.GetAmountItem("stone");
            if (Form1.instance.StoneLabel.InvokeRequired)
            {
                Form1.instance.StoneLabel.Invoke(
                    new Action(() => { Form1.instance.StoneLabel.Text = stone.ToString(); }));
            }
            else
            {
                Form1.instance.StoneLabel.Text = stone.ToString();
            }
        }

    }

    public static class BuildingGrid
    {
        private static Thread BuildingThread;

        public static void Start()
        {
            if (BuildingThread == null)
            {
                BuildingThread = new Thread(UpdateGrid);
                BuildingThread.IsBackground = true;
                BuildingThread.Start();
            }
        }

        public static void Stop()
        {
            if (BuildingThread.IsAlive)
            {
                BuildingThread.Abort();
                BuildingThread = null;
            }
        }

        public static void UpdateGrid()
        {
            DateTime _lastupdatedTime = DateTime.Now;
            while (true)
            {
                Thread.Sleep(50);
                if ((DateTime.Now - _lastupdatedTime).TotalSeconds >= 1)
                {

                    if (Form1.instance.WindowState == FormWindowState.Minimized)
                    {
                        continue;
                    }

                    _lastupdatedTime = DateTime.Now;
                    if (Form1.instance.BuildingGrid.InvokeRequired)
                    {
                        var newbuild = BuildingBinding.GetBuildings();
                        MethodInvoker meth = () =>
                        {
                            foreach (DataGridViewTextBoxColumn clmn in Form1.instance.BuildingGrid.Columns)
                            {
                                clmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                clmn.Resizable = DataGridViewTriState.False;
                            }

                            foreach (var bld in newbuild)
                            {
                                if (BuildingBinding.Buildings.Where(n => n.ID == bld.ID)
                                        .FirstOrDefault() == null)
                                {
                                    var bld2 = bld;
                                    if (bld2.Name == "Small Workshop")
                                    {
                                        bld2.Name = "Fishing Pier";
                                    }

                                    if (bld2.Name == "Big Workshop")
                                    {
                                        bld2.Name = "Main Dock";
                                    }

                                    BuildingBinding.Buildings.Add(bld2);
                                }
                                else
                                {
                                    var old = BuildingBinding.Buildings.First(n => n.ID == bld.ID);
                                    if (old.Level != bld.Level)
                                    {
                                        old.Level = bld.Level;
                                    }

                                    if (old.Producing != bld.Producing)
                                    {
                                        old.Producing = bld.Producing;
                                    }

                                    if (old.Upgrade != bld.Upgrade)
                                    {
                                        old.Upgrade = bld.Upgrade;
                                    }

                                    //edit
                                }
                            }

                            Form1.instance.BuildingGrid.Refresh();
                            Form1.instance.BuildingGrid.Update();
                        };

                        Form1.instance.BuildingGrid.BeginInvoke(meth);
                    }
                }
            }
        }

        public static class BuildingBinding
        {
            public static BindingList<Building> Buildings = new BindingList<Building>();

            public static BindingList<Building> GetBuildings()
            {
                var ret = new BindingList<Building>();
                if (Core.GlobalData == null)
                {
                    return ret;
                }

                if (Core.GlobalData.Buildings == null)
                {
                    return ret;
                }

                foreach (var building in Core.GlobalData.Buildings)
                {
                    var Building = new Building();
                    Building.ID = building.InstId;
                    Building.Name = Cache.GetBuildingDefenitions().Items.Item.Where(n => n.DefId == building.DefId)
                        .First().Name;
                    Building.Level = building.Level;
                    var producing = "";
                    if (building.ProdStart != 0)
                    {
                        var willbeproducedat = building.ProdStart + Cache.GetBuildingDefenitions().Items.Item
                                                   .Where(n => n.DefId == building.DefId).First().Levels.Level
                                                   .Where(n => n.Id == (long) building.Level).First().ProdOutputs
                                                   .ProdOutput[0].Time;
                        //lol xD

                        producing =
                            (DateTime.UtcNow - TimeUtils.FromUnixTime(willbeproducedat))
                            .ToString(@"hh\:mm\:ss");
                    }

                    Building.Producing = producing;
                    var upgrade = "";
                    if (building.UpgStart != 0)
                    {
                        var willbeproducedat = building.UpgStart + Cache.GetBuildingDefenitions().Items.Item
                                                   .Where(n => n.DefId == building.DefId).First().Levels.Level
                                                   .Where(n => n.Id == (long) building.Level + 1).First().UpgradeTime;

                        upgrade = (DateTime.UtcNow - TimeUtils.FromUnixTime(willbeproducedat))
                            .ToString(@"hh\:mm\:ss");
                    }

                    Building.Upgrade = upgrade;
                    ret.Add(Building);
                }

                return ret;
            }

            public class Building
            {
                public int ID { get; set; }
                public string Name { get; set; }
                public int Level { get; set; }
                public string Producing { get; set; }
                public string Upgrade { get; set; }
            }
        }
    }

    public class ShipGrid
    {
        public static BindingList<Ship> Ships = new BindingList<Ship>();

        public class Ship
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Route { get; set; }
            public string InPortAt { get; set; }
        }

        public static Thread ShipThread;

        public static void Start()
        {
            if (ShipThread == null)
            {
                ShipThread = new Thread(UpdateGrid);
                ShipThread.IsBackground = true;
                ShipThread.Start();
            }
        }

        public static void Stop()
        {
            if (ShipThread.IsAlive)
            {
                ShipThread.Abort();
                ShipThread = null;
            }
        }

        public static void UpdateGrid()
        {
            DateTime _lastupdatedTime = DateTime.Now;
            while (true)
            {
                Thread.Sleep(50);
                if ((DateTime.Now - _lastupdatedTime).TotalSeconds >= 1)
                {

                    if (Form1.instance.WindowState == FormWindowState.Minimized)
                    {
                        continue;
                    }

                    _lastupdatedTime = DateTime.Now;
                    if (Form1.instance.ShipGrid.InvokeRequired)
                    {
                        var newbuild = ShipBinding.GetShips();
                        MethodInvoker meth = () =>
                        {
                            foreach (DataGridViewTextBoxColumn clmn in Form1.instance.ShipGrid.Columns)
                            {
                                clmn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                                clmn.Resizable = DataGridViewTriState.False;
                            }

                            foreach (var bld in newbuild)
                            {
                                if (ShipBinding.Ships.Where(n => n.ID == bld.ID)
                                        .FirstOrDefault() == null)
                                {
                                    var bld2 = bld;
                                   
                                    ShipBinding.Ships.Add(bld2);
                                }
                                else
                                {
                                    var old = ShipBinding.Ships.First(n => n.ID == bld.ID);
                                    if (old.InPortAt != bld.InPortAt)
                                    {
                                        old.InPortAt = bld.InPortAt;
                                    }

                                    if (old.Route != bld.Route)
                                    {
                                        old.Route = bld.Route;
                                    }

                                  

                                    //edit
                                }
                            }

                            Form1.instance.ShipGrid.Refresh();
                            Form1.instance.ShipGrid.Update();
                        };

                        Form1.instance.ShipGrid.BeginInvoke(meth);
                    }
                }
            }
        }

        public static class ShipBinding
        {
            public static BindingList<Ship> Ships = new BindingList<Ship>();

            public static BindingList<Ship> GetShips()
            {
                var ret = new BindingList<Ship>();
                if (Core.GlobalData == null)
                {
                    return ret;
                }

                if (Core.GlobalData.Buildings == null)
                {
                    return ret;
                }

                foreach (var ship in Core.GlobalData.Ships.Where(n=>n.Activated!=0))
                {
                    var Ship = new Ship();
                    Ship.ID = ship.InstId;
                    Ship.Name = Defenitions.ShipDef.Items.Item.Where(n => n.DefId == ship.DefId)
                        .First().Name;

                    var willatportat = "";
                    if (ship.Sent != 0)
                    {
                        var lvl = Defenitions.UpgrDef.Items.Item.First(n => n.DefId == ship.TargetId).Levels.Level
                            .First(n => n.Id == ship.TargetLevel);
                        Ship.Route = Defenitions.UpgrDef.Items.Item.First(n => n.DefId == ship.TargetId).Name;
                        var willatportattime = ship.Sent + lvl.TravelTime;
                        //lol xD 
                        if ((DateTime.UtcNow - TimeUtils.FromUnixTime(willatportattime)).TotalSeconds> 0)
                        {
                            willatportat = "--:--:--";
                        }
                        else
                        {


                            willatportat =
                                (DateTime.UtcNow - TimeUtils.FromUnixTime(willatportattime))
                                .ToString(@"hh\:mm\:ss");
                        }
                    }

                    Ship.InPortAt = willatportat;
                    ret.Add(Ship);
                }

                return ret;
            }

        }
    }
}