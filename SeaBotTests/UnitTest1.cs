using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SeaBotCore;
namespace SeaBotTests
{
    [TestClass]
    public class SendShipDestinations
    {
        [TestClass]
        public class Contractors
        {


            [TestMethod]
            public void SendToStatic()
            {

                SeaBotCore.Core.LocalPlayer = new SeaBotCore.Data.PlayerData();
                var ship = new SeaBotCore.Data.Ship() { InstId = 24, DefId = 28, Level = 1, Activated = 1547398452, Type = "", TargetId = 0, TargetLevel = 0, Sent = 0, Cargo = 0, MaterialId = 0, Loaded = 0, CaptainId = 0, Crew = 0, SourceType = "shop", NextLevel = 1, NextCapacityLevel = 1, NextSailorsLevel = 1, CapacityLevel = 1, SailorsLevel = 1 };
                Core.LocalPlayer.Ships = new System.Collections.Generic.List<SeaBotCore.Data.Ship> { ship };
                Core.LocalPlayer.Level = 102;
                Core.LocalPlayer.Sailors = int.MaxValue;
                Core.LocalPlayer.Inventory = new SeaBotCore.Utils.FullyObservableCollection<SeaBotCore.Data.Item>() { new SeaBotCore.Data.Item { Id = 5, Amount = int.MaxValue } };
                Core.LocalPlayer.Contracts = new System.Collections.Generic.List<SeaBotCore.Data.Contractor>();
                Core.LocalPlayer.Contracts.Add(new SeaBotCore.Data.Contractor() { DefId = 323, QuestId = 1, Progress = 0, Done = 0, CargoOnTheWay = 102, Amount = 2060, PlayerLevel = 102 });
                var task = SeaBotCore.BotMethods.ShipManagment.SendShip.Destinations.SendToContractor(ship);
                if (task == null)
                {
                    Assert.Fail();
                }
                else
                {
                    Assert.AreEqual(task.CustomObjects["player_level"], 102);
                    Assert.AreEqual(task.CustomObjects["material_id"], 5);
                    Assert.AreEqual(task.CustomObjects["quest_id"], 1);
                    Assert.AreEqual(task.CustomObjects["contractor_id"], 323);
                    Assert.AreEqual(task.CustomObjects["amount"], 132);
                    Assert.AreEqual(task.CustomObjects["inst_id"], 24);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Progress , 102);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Amount, 2060);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].CargoOnTheWay, 132);

                }
            }
            //[TestMethod]
            //public void SendToGenerated()
            //{
            //}
        }
        //[TestClass]
        //public class Outposts
        //{
        //    Outposts()
        //    {
        //        //Load some outposts
        //        SeaBotCore.Core.LocalPlayer = new SeaBotCore.Data.PlayerData();
        //       // Core.LocalPlayer.Outposts = new System.Collections.Generic.List<SeaBotCore.Data.Outpost>;
        //     //   Core.LocalPlayer.Outposts.Add(new SeaBotCore.Dat
               
        //    }
        //    [TestMethod]
        //    public void SendToStatic()
        //    {
                
        //    }
        //    [TestMethod]
        //    public void SendToGenerated()
        //    {
        //    }
        //}
        //[TestClass]
        //public class Marketplace
        //{
          
        //    [TestMethod]
        //    public void Send()
        //    {
        //    }
        //}
    }
}
