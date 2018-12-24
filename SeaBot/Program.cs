﻿// SeaBotCore
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

using System.Net.Http;
using SeaBotCore.Data;
using SeaBotCore.Utils;

namespace SeaBotCore
{
    public static class Core
    {
        private static readonly HttpClient Client = new HttpClient();
        public static string Ssid = "";
        public static GlobalData GlobalData = new GlobalData();
        public static bool Debug;

        public static string ServerToken = "";

        public static void StopBot()
        {
            ThreadKill.KillTheThread(Networking._syncThread);
        }
    }
}