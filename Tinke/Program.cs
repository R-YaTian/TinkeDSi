/*
 * Copyright (C) 2011  pleoNeX
 *
 *   This program is free software: you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *
 *   This program is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 *
 * Programador: pleoNeX
 * 
 */
using System;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Tinke
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            #region Comprobación de archivos necesarios
            string[] archivos = new string[] { "Ekona.dll", "DSDecmp.dll" , "Be.Windows.Forms.HexBox.dll" };
            string faltan = "";
            for (int i = 0; i < archivos.Length; i++)
            {
                string file = Application.StartupPath + Path.DirectorySeparatorChar + archivos[i];
                if (!File.Exists(file))
                    faltan += '\n' + file;
            }
            if (faltan != "")
            {
                MessageBox.Show(Tools.Helper.GetTranslation("Messages", "S1F"), "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            #endregion

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Sistema());
        }
    }
}
