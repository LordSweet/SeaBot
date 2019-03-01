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
            public void SendToGenerated()
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
                    Assert.AreEqual(task.CustomObjects["player_level"], int.MaxValue);
                    Assert.AreEqual(task.CustomObjects["material_id"], 5);
                    Assert.AreEqual(task.CustomObjects["quest_id"], 1);
                    Assert.AreEqual(task.CustomObjects["contractor_id"], 323);
                    Assert.AreEqual(task.CustomObjects["amount"], 132);
                    Assert.AreEqual(task.CustomObjects["inst_id"], 24);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Progress, 102);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Amount, 2060);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].CargoOnTheWay, 132);

                }
            }
            [TestMethod]
            public void SendToStatic()
            {

                SeaBotCore.Core.LocalPlayer = new SeaBotCore.Data.PlayerData();
                var ship = new SeaBotCore.Data.Ship() { InstId = 3, DefId = 1, Level = 1, Activated = 1547398452, Type = "", TargetId = 0, TargetLevel = 0, Sent = 0, Cargo = 0, MaterialId = 0, Loaded = 0, CaptainId = 0, Crew = 0, SourceType = "shop", NextLevel = 1, NextCapacityLevel = 1, NextSailorsLevel = 1, CapacityLevel = 1, SailorsLevel = 1 };
                Core.LocalPlayer.Ships = new System.Collections.Generic.List<SeaBotCore.Data.Ship> { ship };
                Core.LocalPlayer.Level = 9;
                Core.LocalPlayer.Sailors = int.MaxValue;
                Core.LocalPlayer.Inventory = new SeaBotCore.Utils.FullyObservableCollection<SeaBotCore.Data.Item>() { new SeaBotCore.Data.Item { Id = 5, Amount = int.MaxValue } };
                Core.LocalPlayer.Contracts = new System.Collections.Generic.List<SeaBotCore.Data.Contractor>();
                Core.LocalPlayer.Contracts.Add(new SeaBotCore.Data.Contractor() { DefId = 1, QuestId = 2, Progress = 0, Done = 0, CargoOnTheWay = 12, Amount = 0, PlayerLevel = 0 });
                var task = SeaBotCore.BotMethods.ShipManagment.SendShip.Destinations.SendToContractor(ship);
                if (task == null)
                {
                    Assert.Fail();
                }
                else
                {
                    Assert.AreEqual(task.CustomObjects["player_level"], 2147483647);
                    Assert.AreEqual(task.CustomObjects["material_id"], 5);
                    Assert.AreEqual(task.CustomObjects["quest_id"], 2);
                    Assert.AreEqual(task.CustomObjects["contractor_id"], 1);
                    Assert.AreEqual(task.CustomObjects["amount"], 2);
                    Assert.AreEqual(task.CustomObjects["inst_id"], 3);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Progress, 0);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].Amount, 0);
                    Assert.AreEqual(Core.LocalPlayer.Contracts[0].CargoOnTheWay, 14);

                }
            }
        }
        [TestClass]
        public class Upgradable
        {
           
            [TestMethod]
            public void Send()
            {
                
                SeaBotCore.Core.LocalPlayer = new SeaBotCore.Data.PlayerData();
                var ship = new SeaBotCore.Data.Ship() { InstId = 1, DefId = 536, Level = 5, Activated = 1547398452, Type = "", TargetId = 0, TargetLevel = 0, Sent = 0, Cargo = 0, MaterialId = 0, Loaded = 0, CaptainId = 0, Crew = 0, SourceType = "shop", NextLevel = 5, NextCapacityLevel = 5, NextSailorsLevel = 5, CapacityLevel = 5, SailorsLevel = 5 };
                Core.LocalPlayer.Ships = new System.Collections.Generic.List<SeaBotCore.Data.Ship> { ship };
                Core.LocalPlayer.Level = 9;
                Core.LocalPlayer.Sailors = int.MaxValue;
                Core.LocalPlayer.Upgradeables = new System.Collections.Generic.List<SeaBotCore.Data.Upgradeable>();
                Core.LocalPlayer.Upgradeables.Add(new SeaBotCore.Data.Upgradeable() { DefId =1, Level = 2, Progress = 120, Done = 0, CargoOnTheWay = 25, Amount = 9000, PlayerLevel = 9,ConfirmedTime = 154674,MaterialKoef = 5,Sailors = 8});
                var task = SeaBotCore.BotMethods.ShipManagment.SendShip.Destinations.SendToUpgradable(ship,"coins");
                if (task == null)
                {
                    Assert.Fail();
                }
                else
                {
                    Assert.AreEqual(task.CustomObjects["player_level"], 9);
                    Assert.AreEqual(task.CustomObjects["dest_sailors"], 8);
                    Assert.AreEqual(task.CustomObjects["dest_id"], 1);
                    Assert.AreEqual(task.CustomObjects["dest_amount"], 9000);
                    Assert.AreEqual(task.CustomObjects["inst_id"], 1);
                    Assert.AreEqual(task.CustomObjects["amount"], 20);
                    Assert.AreEqual(task.CustomObjects["dest_material_koef"], 5);
                    //Assert.AreEqual(Core.LocalPlayer.Upgradeables[0].Progress, 1);
                    //Assert.AreEqual(Core.LocalPlayer.Upgradeables[0].Amount, 2060);
                    //Assert.AreEqual(Core.LocalPlayer.Upgradeables[0].CargoOnTheWay, 132);
    
                }
            }
     
        }
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
