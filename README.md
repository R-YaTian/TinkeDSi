# TinkeDSi
<p align="left">
<a href="http://www.gnu.org/copyleft/gpl.html"><img alt="license" src="https://img.shields.io/badge/license-GPL%20V3-blue.svg?style=flat"/></a>
<a title="Crowdin" target="_blank" href="https://crowdin.com/project/tinkedsi"><img src="https://badges.crowdin.net/tinkedsi/localized.svg"></a>
<a title="GitHub all releases" target="_blank" href="https://github.com/R-YaTian/TinkeDSi/releases/latest"><img src="https://img.shields.io/github/downloads/R-YaTian/TinkeDSi/total"></a>
<a title="GitHub Workflow Status" href="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop.yml"><img src="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop.yml/badge.svg"></a>
<a title="GitHub Workflow Status" href="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop-x86.yml"><img src="https://github.com/R-YaTian/TinkeDSi/actions/workflows/dotnet-desktop-x86.yml/badge.svg"></a>
</p>

> TinkeDSi is a program to view, convert, and edit the **files of NDS(i) games**. You can view a lot of file format like images, text, sounds, fonts and textures. Furthermore it works with **plugins** made in NET Framework languages (C# and VB.NET) so it's so easy to support new formats.

To run the program you must have installed **[.NET Framework 4.5](https://www.microsoft.com/en-us/download/details.aspx?id=30653)** or **[mono](http://www.mono-project.com/download)**. In the case of *mono* on *Linux*, make sure you have installed the package **mono-locale-extras** too. For Mac, you need *mono* and *pkg-config* installed and configured, you'll use *mono32* to exec Tinke.


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

# Translating

You can help translate TinkeDSi on [Crowdin](https://crwd.in/tinkedsi). If you'd like to request a new language be added please open an issue for that.

# File Format Used by TinkeDSi

## Tinke Icon Data (`.idat`)

| Offset | Size | Description |
|--------|------|-------------|
| `0x0000` | `0x200` | **Icon Bitmap** 32×32 pixels, 4×4 tiles, 4-bit depth, 4×8 bytes per tile |
| `0x0200` | `0x20` | **Icon Palette** 16 colors, 16-bit format, range 0x0000 – 0x7FFF |

**Technical Notes (From GBATEK):**
- Color 0 is transparent, so the 1st palette entry is ignored

## Tinke Animation Data (`.adat`)

| Offset | Size | Description |
|--------|------|-------------|
| `0x0000` | `0x1000` | **Icon Animation Bitmaps 0 – 7** 8 bitmaps, 0x200 bytes each (same format as `.idat` bitmap) |
| `0x1000` | `0x100` | **Icon Animation Palettes 0 – 7** 8 palettes, 0x20 bytes each (same format as `.idat` palette) |
| `0x1100` | `0x80` | **Icon Animation Sequence** 16-bit animation tokens |

## Tinke Donor iHeader (`.ihdr`)

The `.ihdr` file contains DSi-specific header information extracted from the ROM header. This file is used to fix DSi binaries in ROM hacks or bad dumps.

### Structure

| Offset | Size | Description |
|--------|------|-------------|
| `0x0000` | 4 | **Gamecode** Game identifier (4 ASCII characters) |

<details>
<summary><span><code>0x0004</code>&emsp;0xE80&emsp;<strong>From DSi ROM header offset 0x180–0x1000</strong></span></summary>

| Offset | Size | Description |
|--------|------|-------------|
| `0x0004` | 20 | **Global MBK1 – MBK5 Settings** WRAM slot configuration |
| `0x0018` | 12 | **ARM9 Local MBK6 – MBK8** WRAM areas for ARM9 |
| `0x0024` | 12 | **ARM7 Local MBK6 – MBK8** WRAM areas for ARM7 |
| `0x0030` | 3 | **Global MBK9 Setting** WRAM slot write protection |
| `0x0033` | 1 | **Global WRAMCNT** WRAM control setting |
| `0x0034` | 4 | **Region Flags** `bit0=JPN, bit1=USA, bit2=EUR, bit3=AUS, bit4=CHN, bit5=KOR, bit6-31=Reserved` (0xFFFFFFFF = Region Free) |
| `0x0038` | 4 | **Access Control** AES key select |
| `0x003C` | 4 | **ARM7 SCFG_EXT7** |
| `0x0040` | 4 | **App Flags** |
| `0x0044` | 4 | **ARM9i ROM Offset** |
| `0x0048` | 4 | **Reserved** (zero) |
| `0x004C` | 4 | **ARM9i RAM Address** |
| `0x0050` | 4 | **ARM9i Size** |
| `0x0054` | 4 | **ARM7i ROM Offset** |
| `0x0058` | 4 | **SD/MMC Device List ARM7 Address** |
| `0x005C` | 4 | **ARM7i RAM Address** |
| `0x0060` | 4 | **ARM7i Size** |
| `0x0064` | 4 | **Digest NTR Offset** Usually same as ARM9 rom offset |
| `0x0068` | 4 | **Digest NTR Length** |
| `0x006C` | 4 | **Digest TWL Offset** usually same as ARM9i rom offset |
| `0x0070` | 4 | **Digest TWL Length** |
| `0x0074` | 4 | **Digest Sector Hashtable Offset** |
| `0x0078` | 4 | **Digest Sector Hashtable Length** |
| `0x007C` | 4 | **Digest Block Hashtable Offset** |
| `0x0080` | 4 | **Digest Block Hashtable Length** |
| `0x0084` | 4 | **Digest Sector Size** |
| `0x0088` | 4 | **Digest Block Sector Count** |
| `0x008C` | 4 | **Icon/Title Size** Usually 0x23C0 for DSi |
| `0x0090` | 1 | **SD/MMC shared2\0000 Size** In 32KB units |
| `0x0091` | 1 | **SD/MMC shared2\0001 Size** In 32KB units |
| `0x0092` | 1 | **EULA Version** |
| `0x0093` | 1 | **Use Ratings** |
| `0x0094` | 4 | **Total Used ROM Size** Including DSi area (optional, can be 0) |
| `0x0098` | 1 | **SD/MMC shared2\0002 Size** In 32KB units |
| `0x0099` | 1 | **SD/MMC shared2\0003 Size** In 32KB units |
| `0x009A` | 1 | **SD/MMC shared2\0004 Size** In 32KB units |
| `0x009B` | 1 | **SD/MMC shared2\0005 Size** In 32KB units |
| `0x009C` | 4 | **ARM9i Parameters Table Offset** |
| `0x00A0` | 4 | **ARM7i Parameters Table Offset** |
| `0x00A4` | 4 | **Modcrypt Area 1 Offset** Usually same as ARM9i ROM offset |
| `0x00A8` | 4 | **Modcrypt Area 1 Size** |
| `0x00AC` | 4 | **Modcrypt Area 2 Offset** (0 = None) |
| `0x00B0` | 4 | **Modcrypt Area 2 Size** (0 = None) |
| `0x00B4` | 4 | **Title ID Low** Emagcode (aka Gamecode spelled backwards) |
| `0x00B8` | 4 | **Title ID High** |
| `0x00BC` | 4 | **DSiWare public.sav Size** In bytes (0 = none) |
| `0x00C0` | 4 | **DSiWare private.sav Size** In bytes (0 = none) |
| `0x00C4` | 176 | **Reserved** Zero-filled |
| `0x0174` | 16 | **Parental Control Age Ratings** |
| `0x0184` | 32 | **SHA1-HMAC ARM9** With encrypted secure area |
| `0x0198` | 32 | **SHA1-HMAC ARM7** |
| `0x01AC` | 32 | **SHA1-HMAC Digest Master** |
| `0x01C0` | 32 | **SHA1-HMAC Icon/Title** (also in newer NDS titles) |
| `0x01D4` | 32 | **SHA1-HMAC ARM9i** Decrypted |
| `0x01E8` | 32 | **SHA1-HMAC ARM7i** Decrypted |
| `0x01FC` | 64 | **Reserved** Zero-filled (but used by non-whitelisted NDS titles) |
| `0x0224` | 32 | **SHA1-HMAC ARM9 (no secure)** Without 16KB secure area |
| `0x0238` | 2636 | **Reserved** Zero-filled |
| `0x0C84` | 384 | **Debug Arguments** |
| `0x0E04` | 128 | **RSA-SHA1 Signature** |
</details>

#### Notes:
- File size: 0x0E84 bytes (3716 bytes)
- Parts of the above structural description are sourced from GBATEK

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
 * Tokimeki Memorial Girl's Side: 1st Love Plus (C4GJ)
 * Jump! Ultimate Stars (ALAR, DSIG, DSCP)
 * Time Ace (AE3E) (Also works for Powerbike (C2BE))
 * Cake Mania 2 (CAKX)
 * Sonic Rush Adventure (ASCx, A3Yx, BXSx)
 * The Prince of Tennis Games (BTGJ and TENJ)
 * Inazuma Eleven Games
 * Tales Of The Tempest (ALEJ)
 * A Witch's Tale (USA: CW3E)
 * Death Note: Kira Game (YDNJ)
 * Gakuen Hetalia DS (THTJ)
 * Club Penguin (CLPE, CY9P)
 * Big Hero 6 (TB6X)

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
