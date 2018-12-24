﻿// SeabotGUI
// Copyright (C) 2018 Weespin
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
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SeaBotCore;
using SeaBotCore.Data;
using SeaBotCore.Data.Defenitions;
using SeaBotCore.Data.Materials;
using SeaBotCore.Logger;
using SeaBotCore.Utils;
using Task = SeaBotCore.Task;
using SeaBotGUI.BotLoop;
namespace SeaBotGUI
{
    public partial class Form1 : Form
    {
        
        public static Thread BotThread;
        public static Config _config = new Config();
        public static Thread BarrelThread; 
        public Form1()
        {
            InitializeComponent();
            ConfigSer.Load();
            this.MaximizeBox = false;
            textBox2.Text = _config.server_token;
            checkBox1.Checked = _config.debug;
            Core.Debug = _config.debug;
            chk_autofish.Checked = _config.collectfish;
            chk_prodfact.Checked = _config.prodfactory;
            chk_collectmat.Checked = _config.collectfactory;
            chk_barrelhack.Checked = _config.barrelhack;
            chk_finishupgrade.Checked = _config.finishupgrade;
            chk_aupgrade.Checked = _config.autoupgrade;
            num_ironlimit.Value = _config.ironlimit;
            num_woodlimit.Value = _config.woodlimit;
            num_stonelimit.Value = _config.stonelimit;
            label7.Text =
                $"Version: {FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion}";
            Logger.Event.LogMessageChat.OnLogMessage += LogMessageChat_OnLogMessage;
            linkLabel1.Links.Add(new LinkLabel.Link(){LinkData = "https://github.com/weespin/SeaBot/wiki/Getting-server_token"});
            //Check for cache
        }

        private void Inventory_CollectionChanged(object sender,
            NotifyCollectionChangedEventArgs e)
        {
            FormateResources(SeaBotCore.Core.GlobalData);
        }

        private void LogMessageChat_OnLogMessage(Logger.Message e)
        {
            if (InvokeRequired)
            {
                MethodInvoker inv = delegate
                {
                    RichTextBoxExtensions.AppendText(richTextBox1, e.message + "\n", e.color);
                };
                richTextBox1.BeginInvoke(inv);
            }
            else
            {
                RichTextBoxExtensions.AppendText(richTextBox1, e.message + "\n", e.color);
            }
        }

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

        public void FormateResources(GlobalData data)
        {
            //Gold: Fish: Iron:
            //Gems: Wood: Rock:

            if (textBox1.InvokeRequired)
            {
                MethodInvoker inv = delegate
                {
                    if (data.Inventory != null)
                    {
                        StringBuilder txt = new StringBuilder();
                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("coins").DefId) != null)
                        {
                            txt.Append($"Gold: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("coins").DefId).Amount}");

                        }

                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("fish").DefId) != null)
                        {
                            txt.Append($" Fish: {data.Inventory.First(n => n.Id == (int)MaterialDB.GetItem("fish").DefId).Amount} ");
                        }
                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("iron").DefId) != null)
                        {
                            txt.Append($" Iron: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("iron").DefId).Amount} ");
                        }

                        txt.Append(Environment.NewLine);
                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("gem").DefId) != null)
                        {
                            txt.Append($" Gems: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("gem").DefId).Amount} ");

                        }

                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("wood").DefId) != null)
                        {
                            txt.Append($" Wood: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("wood").DefId).Amount}");
                        }
                        if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("stone").DefId) != null)
                        {
                            txt.Append($" Stone: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("stone").DefId).Amount}");
                        }

                        textBox1.Text = txt.ToString();
                    }
                };
                textBox1.Invoke(inv);
            }
            else
            {
                if (data.Inventory != null)
                {
            
                    StringBuilder txt = new StringBuilder();
                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("coins").DefId) != null)
                    {
                        txt.Append($"Gold: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("coins").DefId).Amount}");

                    }

                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("fish").DefId) != null)
                    {
                        txt.Append($" Fish: {data.Inventory.First(n => n.Id == (int)MaterialDB.GetItem("fish").DefId).Amount} ");
                    }
                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("iron").DefId) != null)
                    {
                        txt.Append($" Iron: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("iron").DefId).Amount} ");
                    }

                    txt.Append(Environment.NewLine);
                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("gem").DefId) != null)
                    {
                        txt.Append($" Gems: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("gem").DefId).Amount} ");

                    }

                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("wood").DefId) != null)
                    {
                        txt.Append($" Wood: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("wood").DefId).Amount}");
                    }
                    if (data.Inventory.FirstOrDefault(n => n.Id == MaterialDB.GetItem("stone").DefId) != null)
                    {
                        txt.Append($" Stone: {data.Inventory.First(n => n.Id == MaterialDB.GetItem("stone").DefId).Amount}");
                    }

                    textBox1.Text = txt.ToString();
                }
            }

            var a = new List<ListViewItem>();
            foreach (var dataa in data.Inventory.Where(n =>
                n.Id != (int) 1 && n.Id != (int) 2 &&
                n.Id != (int) 3 && n.Id != (int) 4 &&
                n.Id != (int) 5 && n.Id != (int) 6))
            {
                string[] row = {MaterialDB.GetItem(dataa.Id).Name, dataa.Amount.ToString()};
                a.Add(new ListViewItem(row));
            }

            if (listView1.InvokeRequired)
            {
                MethodInvoker inv = delegate
                {
                    listView1.Items.Clear();
                    foreach (var list in a)
                    {
                        listView1.Items.Add(list);
                    }
                };
                listView1.BeginInvoke(inv);
            }
            else
            {
                listView1.Items.Clear();
                foreach (var list in a)
                {
                    listView1.Items.Add(list);
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(_config.server_token))
            {
                MessageBox.Show("Empty server_token\nPlease fill server token in Settings tab", "Error");
                return;
            }

            if (!Directory.Exists("cache"))
            {
                Cache.DownloadCache();
            }

            button3.Enabled = true;
            button2.Enabled = false;
            Core.ServerToken = textBox2.Text;
            
            Networking.Login();
            FormateResources(Core.GlobalData);
           Core.GlobalData.Inventory.CollectionChanged += Inventory_CollectionChanged;
            Core.GlobalData.Inventory.ItemPropertyChanged += Inventory_ItemPropertyChanged;
            BarrelThread = new Thread(BarrelVoid){IsBackground = true};
            BarrelThread.Start();
           
            Networking.StartThread();
            BotThread = new Thread(BotVoid);
            BotThread.IsBackground = true;
            BotThread.Start();
            var a = Defenitions.BuildingDef;
        }

        private void Inventory_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            FormateResources(Core.GlobalData);
        }

        void BarrelVoid()
        {
            while (true)
            {
                Thread.Sleep(11*1000);

                if (chk_barrelhack.Checked)
                {
                    BotLoop.BotLoop.CollectBarrel();

                }
            }
        }
        void BotVoid()
        {
            while (true)
            {
                if (chk_aupgrade.Checked)
                {
                    BotLoop.BotLoop.AutoUpgrade();
                }
                if (chk_autofish.Checked)
                {
                    BotLoop.BotLoop.CollectFish();
                }


                if (chk_collectmat.Checked)
                {
                    BotLoop.BotLoop.CollectMaterials();
                }

                if (chk_prodfact.Checked)
                {
                    BotLoop.BotLoop.ProduceFactories((int)num_ironlimit.Value,(int)num_stonelimit.Value,(int)num_woodlimit.Value);
                    }

                if (chk_finishupgrade.Checked)
                {
                 BotLoop.BotLoop.FinishUpgrade();
                }

            
                
                Thread.Sleep(60 * 1000);
            }
        }


        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _config.debug = checkBox1.Checked;
            Core.Debug = checkBox1.Checked;
            ConfigSer.Save();
        }

        private void chk_autofish_CheckedChanged(object sender, EventArgs e)
        {
            _config.collectfish = chk_autofish.Checked;
            ConfigSer.Save();
        }

        private void chk_prodfact_CheckedChanged(object sender, EventArgs e)
        {
            _config.prodfactory = chk_prodfact.Checked;
            ConfigSer.Save();
        }

        private void chk_collectmat_CheckedChanged(object sender, EventArgs e)
        {
            _config.collectfactory = chk_collectmat.Checked;
            ConfigSer.Save();
        }


        private void num_woodlimit_Leave(object sender, EventArgs e)
        {
            _config.woodlimit = (int) num_woodlimit.Value;
            ConfigSer.Save();
        }


        private void num_ironlimit_Leave(object sender, EventArgs e)
        {
            _config.ironlimit = (int) num_ironlimit.Value;
            ConfigSer.Save();
        }

        private void num_stonelimit_Leave(object sender, EventArgs e)
        {
            _config.stonelimit = (int) num_stonelimit.Value;
            ConfigSer.Save();
        }

        private void textBox2_Leave_1(object sender, EventArgs e)
        {
            _config.server_token = textBox2.Text;
            ConfigSer.Save();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            button2.Enabled = true;
            button3.Enabled = false;
            Core.StopBot();
            SeaBotCore.Utils.ThreadKill.KillTheThread(BotThread);
            SeaBotCore.Utils.ThreadKill.KillTheThread(BarrelThread);
      
            Core.GlobalData = null;
     
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData as string);
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            _config.debug = checkBox1.Checked;
            Core.Debug = checkBox1.Checked;
            ConfigSer.Save();
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
          //  var seed = textBox3.Text;
           // BarrelController.SetSeed(Convert.ToDouble(seed));
           // BarrelController.GetNextBarrel()
        }

        private void chk_aupgrade_CheckedChanged(object sender, EventArgs e)
        {
            _config.autoupgrade = chk_aupgrade.Checked;
            ConfigSer.Save();
        }

        private void chk_finishupgrade_CheckedChanged(object sender, EventArgs e)
        {
            _config.finishupgrade = chk_finishupgrade.Checked;
            ConfigSer.Save();
        }

        private void chk_barrelhack_CheckedChanged(object sender, EventArgs e)
        {
            _config.barrelhack = chk_barrelhack.Checked;
            ConfigSer.Save();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/weespin/SeaBot");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://t.me/nullcore");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://steamcommunity.com/id/wspin/");
        }

    
    }
}