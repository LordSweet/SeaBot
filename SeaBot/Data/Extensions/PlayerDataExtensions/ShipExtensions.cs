using SeaBotCore.BotMethods.ShipManagment.SendShip;
using SeaBotCore.Cache;
using SeaBotCore.Data.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaBotCore.Data.Extensions
{
 public  static class ShipExtensions
    {
        public static int Capacity(this Ship ship)
        {
            return SendingHelper.GetCapacity(ship);
        }
        public static void Clear(this Ship ship)
        {
            SendingHelper.NullShip(ship);
        }
        public static int Sailors(this Ship ship)
        {
            return SendingHelper.GetSailors(ship);
        }

        public static int TravelTime(this Ship ship)
        {
            return SendingHelper.GetTravelTime(ship);
        }
        public static string TargetName(this Ship ship)
        {
            return SendingHelper.GetTravelName(ship);
        }
        public static string ShipName(this Ship ship)
        {
            return LocalizationCache.GetNameFromLoc(
                LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId)?.NameLoc,
                LocalDefinitions.Ships.FirstOrDefault(n => n.DefId == ship.DefId)?.Name);
        }
    }
}
