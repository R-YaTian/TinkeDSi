# TinkeDSi
<p align="left">
<a href="http://www.gnu.org/copyleft/gpl.html"><img alt="license" src="https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat"/></a>
<a title="Crowdin" target="_blank" href="https://crowdin.com/project/tinkedsi"><img src="https://badges.crowdin.net/tinkedsi/localized.svg"></a>
<a title="GitHub all releases" target="_blank" href="https://github.com/R-YaTian/TinkeDSi/releases/latest"><img src="https://img.shields.io/github/downloads/R-YaTian/TinkeDSi/total"></a>
<a title="GitHub Workflow Status" href="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop.yml"><img src="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop.yml/badge.svg"></a>
</p>

> TinkeDSi is a program to see, convert, and edit the **files of NDS(i) games**. You can see a lot of format files like images, text, sounds, fonts and textures. Furthermore it works with **plugins** made in NET Framework languages (C# and VB.NET) so it's so easy to support new formats.

To run the program you must have installed **[.NET Framework 4.5](https://www.microsoft.com/es-es/download/details.aspx?id=30653)** or **[mono](http://www.mono-project.com/download)**. In the case of *mono* on *Linux*, make sure you have installed the package **mono-locale-extras** too. For Mac, you need *mono* and *pkg-config* installed and configured, you'll use *mono32* to exec Tinke.


# Features

 * Show the ROM header with the banner and edit it. (Support edit and/or fix DSi banner)
 * Show and convert to common format a lot of files.
 * Edit a lot of image files from BMP files (NCLR, NCGR, NSCR, NCER), audio files from WAV (SWAV, SWAR, STRM) and fonts (NFTR).
 * Hexadecimal viewer.
 * Change the content of the files and save the new ROM. (Support change files by dir)
 * Multilanguage support.
 * A way to fix DSi header. (Then you can import arm9/7i file to fix "DSi binary file is missing" error for romhacks and/or bad dumps)
    * How to fix "DSi binary file is missing" error?
    * Step 1: Open "good dump" with TinkeDSi, then export iheader file on ROM info window and export arm9/7i.bin files on nitrofs window.
    * Step 2: Open "bad dump" and import iheader on ROM info window, then save the ROM as a new file.
    * Step 3: Open the new ROM file, replace arm9/7i.bin on nitrofs window then save the ROM.

#  Translating

You can help translate TinkeDSi on [Crowdin](https://crwd.in/tinkedsi). If you'd like to request a new language be added please open an issue for that.

# Supported formats

## Images
 * NCLR => Nitro CoLouR(palette)
 * NCGR => Nitro Character Graphic Resource (tiles)
 * NBGR => Nitro Basic Graphic Resource (tiles)
 * NSCR => Nitro Screen Resource (map)
 * NCER => Nitro CEll Resource (cell/puzzle)
 * NANR => Nitro ANimation Resource (animation)
 * CHAR / CHR => CHARacter (tiles)
 * PLT / PAL => PaLeTte (palette)
 * NBFS => Nitro Basic File Screen (map)
 * NBFP => Nitro Basic File Palette (palette)
 * NBFC => Nitro Basic File Character (tiles)
 * NTFT => NiTro File Tiles (tiles)
 * NTFP => NiTro File Palette (palette)
 * RAW => Raw image (tiles)
 * MAP => Raw map info (map)
 * Common formats => PNG, JPG, TGA, GIF, BMP

## Textures
 * BTX0 (NSBTX)
 * BMD0 (NSBMD)

## Audio
 * SDAT => Sound DATa
 * SWAV => Sound WAVe
 * SWAR => Sound Wave ARchive
 * STRM => STReaM
 * SADL
 * Common formats => WAV

## Text
 * Sound definition => SADL, XSADL, SARC, SBDL, SMAP.
 * BMG => Pack text file
 * Common formats => TXT, XML, INI, H, BAT, C, MAKEFILE, LUA, CSV, BUILDTIME, HTML, CSS, JS, NAIX, DTD, BSF, NBSD

## Compression
  Thanks to DSDEcmp library [DSDecmp](http://code.google.com/p/dsdecmp) (credits to *barubary*)
 * Huffman (id = 0x20)
 * LZ77    (id = 0x10)
 * LZSS    (id = 0x11)
 * RLE     (id = 0x30)

## Pack
 * NARC or ARC => Nintendo ARChives
 * Utility.bin => Wifi data files

### Note: Additional formats that are not currently recognized by the program as openable files, for example, plain text files that have an unknown extension, can be opened with the "Open As..." dialog.

# Specific plugin for games
 * 999, nine hours nine persons nine doors (BSKE)
 * Itsu Demo Doko Demo Dekiru Igo (AIIJ)
 * Blood of Bahamut (CYJJ)
 * Dragon Ball Kai Ultimate Butouden (TDBJ)
 * Ace Attorney Investigations: Miles Edgeworth (C32P, C32J)
 * Gyakuten Kenji 2 (BXOJ)
 * Kirby Squeak Squad (AKWE)
 * Last Window The Secret of Cape West (YLUP)
 * Professor Layton and the Mysterious Village (A5FE, A5FP)
 * Professor Layton and Pandora's Box (YLTS)
 * Maple Story DS (YMPK)
 * Ninokuni Shikkoku no Madoushi (B2KJ)
 * Rune Factory 3 (BRFE, BRFJ)
 * The world end with you (AWLJ)
 * Tetris DS (YLUP)
 * Tokimeki Memorial Girl's Side 3rd Story (B3SJ)
 * Jump! Ultimate Stars (ALAR, DSIG, DSCP)
 * Time Ace (AE3E) (Also works for Powerbike (C2BE))
 * Cake Mania 2 (CAKX)
 * Sonic Rush Adventure (ASCx, A3Yx, BXSx)
 * Inazuma Eleven
 * Tales Of The Tempest
 * A Witch's Tale
 * Death Note: Kira Game
 * Gakuen Hetalia DS
 * Club Penguin
 * Big Hero 6

----

Link to web pages with NDS info:

 * http://llref.emutalk.net/docs
 * http://problemkaputt.de/gbatek.htm
 * http://sites.google.com/site/kiwids/sdat.html

----

## Compile instructions
* Windows: run compile.bat
* Unix: ./compile.sh

## Screenshots
![Tinke 0.8.1](https://lh5.googleusercontent.com/-GRKvfv-TAaI/ToBy1_eFrfI/AAAAAAAAASA/9WDkc_OQPC4/s800/Tinke%2525200.8.1.PNG)
![Header editor](https://lh5.googleusercontent.com/-W6YUKmyV3JM/ToBzRa0_pwI/AAAAAAAAASI/D7g1JKFvgC8/s400/header%252520editor.PNG)
![ROM Info](https://lh5.googleusercontent.com/_H6ACRUcYPos/TV1ITC1_ceI/AAAAAAAAAG8/cYKNoa3du98/s400/inforom.PNG)

![BTX support](https://lh4.googleusercontent.com/-0Rv5v3JQ0AQ/Tn-J8C1gaxI/AAAAAAAAARg/4HvC4j-5olU/s400/btx.PNG)
![Layton support](https://lh6.googleusercontent.com/_H6ACRUcYPos/TV1IT9DBy8I/AAAAAAAAAHM/ePmPUmTa4w8/s400/ani.PNG)
![Sprite support](https://lh3.googleusercontent.com/-Un-1FO1rlD4/ToB0NvJ03ZI/AAAAAAAAASU/iNdHYvEehBc/s400/ncerV2.PNG)
![Animation support](https://lh3.googleusercontent.com/_H6ACRUcYPos/TV8C0RtGTzI/AAAAAAAAAHk/wO9ps1DP-EU/s400/nanr.PNG)
![Font support](https://lh6.googleusercontent.com/-pSP4NY3Y9Rw/TqPSrsRc6eI/AAAAAAAAAUg/-QjuDfRdQc4/s400/nftr-2.PNG)
![Sound support](https://lh4.googleusercontent.com/-VSJCC9q9TPQ/TmlKbnvgTaI/AAAAAAAAAOg/s7DFYgpeo3c/s400/sdat.PNG)

![Layton 1](https://lh3.googleusercontent.com/_H6ACRUcYPos/TV1ITRjI1WI/AAAAAAAAAHE/aClaJQdH7xU/s144/imgs2.PNG)
![Layton 2](https://lh6.googleusercontent.com/_H6ACRUcYPos/TV1ITJsYn5I/AAAAAAAAAHA/yAz7oiEKOa4/s144/imgs1.PNG)
![BMG support](https://lh4.googleusercontent.com/_H6ACRUcYPos/TV1IYiOYTOI/AAAAAAAAAHQ/Vdf4K030mdU/s144/text.PNG)
