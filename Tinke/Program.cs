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
using CommandLine;
using System.Collections.Generic;
using System.Linq;

[Verb("extract", HelpText = "Extract all files from nds rom.")]
internal class ExtractOptions
{
    [Option('f', "file", Required = true, HelpText = "The path of nds rom file to be extract.")]
    public string FilePath { get; set; }

    [Option('o', "out", HelpText = "The output path. If not provided, will use the same path of rom file.")]
    public string OutputPath { get; set; }
}

[Verb("replace", HelpText = "Replace all nitrofs files by dir.")]
internal class ReplaceOptions
{
    [Option('f', "file", Required = true, HelpText = "The path of the input nds rom file.")]
    public string FilePath { get; set; }

    [Option('d', "dir", Required = true, HelpText = "The folder path which contains all nitrofs files. It should be named 'Root'.")]
    public string ResPath { get; set; }

    [Option('o', "out", Required = true, HelpText = "Set the output rom path.")]
    public string OutputFile { get; set; }

    [Option('t', "trim", HelpText = "Safe trim the output rom.")]
    public bool SafeTrim { get; set; }

    [Option('k', "keep-sig", HelpText = "Keep Original RSA SHA1 Signature for output rom.")]
    public bool KeepSig { get; set; }

    [Option('r', "re-comp", HelpText = "Recompress ARM9 binary for output rom.")]
    public bool Recompress { get; set; }

    [Option('b', "blz-cue", HelpText = "Use better compress method to compress ARM9 binary (BLZ-Cue). Will be ignore if -r/--re-comp not passed.")]
    public bool BlzCue { get; set; }
}

[Verb("open", HelpText = "Open file(s) or a folder via TinkeDSi GUI")]
internal class OpenOptions
{
    [Option('f', "folder", HelpText = "Call a folder select dialog then open the selected folder.")]
    public bool IsFolder { get; set; }

    [Option('d', "dir", HelpText = "Open the folder directly instead of calling folder select dialog. Will be ignore if -f/--folder not passed.")]
    public string DirPath { get; set; }

    [Value(0, MetaName = "RomPath", HelpText = "Path of the file(s). Can be provided multiple. Will be ignore if -f/--folder passed.")]
    public IEnumerable<string> Props
    {
        get;
        set;
    }
}

namespace Tinke
{
    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool AttachConsole(int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleOutputCP(uint wCodePageID);

        public static string extractFilePath;
        public static string extractOutputPath;
        public static string replaceResPath;
        public static string replaceInputFile;
        public static string replaceOutputFile;
        public static bool safeTrim = false;
        public static bool keepSig = false;
        public static bool recompressA9 = false;
        public static bool blzcueA9 = false;
        public static int curCommand = -1;
        public static List<string> tblRoms;
        public static bool bIsFolder = false;
        public static bool bOpenDefault = false;
        public static string openDirPath;

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            #region Comprobación de archivos necesarios
            string[] archivos = new string[] { "Ekona.dll", "DSDecmp.dll" , "Be.Windows.Forms.HexBox.dll", "CommandLine.dll" };
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

            if (Environment.GetCommandLineArgs().Length >= 2)
            {
                if (Type.GetType("Mono.Runtime") == null)
                {
                    Version osVersion = Environment.OSVersion.Version;
                    Version win7Version = new Version(6, 1);
                    AttachConsole(-1);
                    if (osVersion >= win7Version)
                        SetConsoleOutputCP(65001);
                    Console.WriteLine();
                }
                Parser.Default.ParseArguments<ExtractOptions, ReplaceOptions, OpenOptions>(args)
                              .WithParsed<ExtractOptions>(RunExtract)
                              .WithParsed<ReplaceOptions>(RunReplace)
                              .WithParsed<OpenOptions>(RunOpen)
                              .WithNotParsed(HandleErrors);
                if (Type.GetType("Mono.Runtime") == null && curCommand == 0)
                {
                    FreeConsole();
                    SendKeys.SendWait("{ENTER}");
                }
            }

            if (curCommand == 0)
            {
                Application.Exit();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Sistema());
        }

        //process ExtractOptions
        private static void RunExtract(ExtractOptions opts)
        {
            extractFilePath = opts.FilePath;
            extractOutputPath = opts.OutputPath;
            curCommand = 1;
        }

        //process ReplaceOptions
        private static void RunReplace(ReplaceOptions opts)
        {
            replaceResPath = opts.ResPath;
            replaceInputFile = opts.FilePath;
            replaceOutputFile = opts.OutputFile;
            safeTrim = opts.SafeTrim;
            keepSig = opts.KeepSig;
            recompressA9 = opts.Recompress;
            blzcueA9 = opts.BlzCue;
            curCommand = 2;
        }

        //process OpenOptions
        private static void RunOpen(OpenOptions opts)
        {
            if (Environment.GetCommandLineArgs().Length <= 2)
            {
                bOpenDefault = true;
                curCommand = -1;
                return;
            }
            else
                curCommand = 3;
            tblRoms = opts.Props.ToList();
            openDirPath = opts.DirPath;
            bIsFolder = opts.IsFolder;
        }

        private static void HandleErrors(IEnumerable<Error> obj)
        {
            curCommand = 0;
        }
    }
}
