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

    using SeaBotCore.BotMethods.ShipManagment;
    using SeaBotCore.BotMethods.ShipManagment.SendShip;
    using SeaBotCore.Cache;
    using SeaBotCore.Config;
    using SeaBotCore.Data;
    using SeaBotCore.Data.Definitions;
    using SeaBotCore.Data.Materials;
    using SeaBotCore.Localizaion;
    using SeaBotCore.Logger;
    using SeaBotCore.Utils;

    #endregion

    public static class Ships
    {
        public static void AutoShip()
        {
            LoadShips.LoadAllShips();
            UnloadShips.UnloadAllShips();
            SendShips.SendShipsDestination(Core.Config.shipdesttype);
        }
    }


}