﻿/*
 * Copyright (C) 2011  rafael1193
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
 * Programador: rafael1193, pleonex
 * 
 * Fecha: 16/07/2011
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDAT
{
    internal static class AdpcmDecompressor
    {
        public static byte[] DecompressADPCM1Channel(byte[] data)
        {
            List<byte> resul = new List<byte>();

            #region Preinitialized variables
            int index = 0;
            int stepsize = 7;
            int[] indexTable = new int[16] { -1, -1, -1, -1, 2, 4, 6, 8,
                                             -1, -1, -1, -1, 2, 4, 6, 8 };

            int[] stepsizeTable = new int[89] { 7, 8, 9, 10, 11, 12, 13, 14,
                                                16, 17, 19, 21, 23, 25, 28,
                                                31, 34, 37, 41, 45, 50, 55,
                                                60, 66, 73, 80, 88, 97, 107,
                                                118, 130, 143, 157, 173, 190, 209,
                                                230, 253, 279, 307, 337, 371, 408,
                                                449, 494, 544, 598, 658, 724, 796,
                                                876, 963, 1060, 1166, 1282, 1411, 1552,
                                                1707, 1878, 2066, 2272, 2499, 2749, 3024, 3327, 3660, 4026,
                                                4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630,
                                                9493, 10442, 11487, 12635, 13899, 15289, 16818,
                                                18500, 20350, 22385, 24623, 27086, 29794, 32767 };
            #endregion

            data = Bit8ToBit4(data);

            int difference, newSample = 0;
            for (int i = 0; i < data.Length; i++)
            {
                difference = 0;

                if ((data[i] & 4) != 0)
                    difference += stepsize;
                if ((data[i] & 2) != 0)
                    difference += stepsize >> 1;
                if ((data[i] & 1) != 0)
                    difference += stepsize >> 2;
                difference += stepsize >> 3;

                if ((data[i] & 8) != 0)
                    difference = -difference;
                newSample += difference;

                if (newSample > 32767)
                    newSample = 32767;
                else if (newSample < -32768)
                    newSample = -32768;

                resul.AddRange(BitConverter.GetBytes((short)newSample));

                index += indexTable[data[i]];
                if (index < 0)
                    index = 0;
                else if (index > 88)
                    index = 88;
                stepsize = stepsizeTable[index];
            }

            return resul.ToArray();
        }

        public static byte[] DecompressADPCM2Channel(byte[] data)
        {
            List<byte> resul = new List<byte>();

            #region Preinitialized variables
            int index1 = 0;
            int index2 = 0;
            int stepsize1 = 7;
            int stepsize2 = 7;
            int[] indexTable = new int[16] { -1, -1, -1, -1, 2, 4, 6, 8,
                                             -1, -1, -1, -1, 2, 4, 6, 8 };

            int[] stepsizeTable = new int[89] { 7, 8, 9, 10, 11, 12, 13, 14,
                                                16, 17, 19, 21, 23, 25, 28,
                                                31, 34, 37, 41, 45, 50, 55,
                                                60, 66, 73, 80, 88, 97, 107,
                                                118, 130, 143, 157, 173, 190, 209,
                                                230, 253, 279, 307, 337, 371, 408,
                                                449, 494, 544, 598, 658, 724, 796,
                                                876, 963, 1060, 1166, 1282, 1411, 1552,
                                                1707, 1878, 2066, 2272, 2499, 2749, 3024, 3327, 3660, 4026,
                                                4428, 4871, 5358, 5894, 6484, 7132, 7845, 8630,
                                                9493, 10442, 11487, 12635, 13899, 15289, 16818,
                                                18500, 20350, 22385, 24623, 27086, 29794, 32767 };
            #endregion

            data = Bit8ToBit4(data);

            int difference1, newSample1 = 0;
            int difference2, newSample2 = 0;

            for (int i = 0; i < data.Length; i++)
            {
                //Left Channel
                difference1 = 0;

                if ((data[i] & 4) != 0)
                    difference1 += stepsize1;
                if ((data[i] & 2) != 0)
                    difference1 += stepsize1 >> 1;
                if ((data[i] & 1) != 0)
                    difference1 += stepsize1 >> 2;
                difference1 += stepsize1 >> 3;

                if ((data[i] & 8) != 0)
                    difference1 = -difference1;
                newSample1 += difference1;

                if (newSample1 > 32767)
                    newSample1 = 32767;
                else if (newSample1 < -32768)
                    newSample1 = -32768;

                resul.AddRange(BitConverter.GetBytes((short)newSample1));

                index1 += indexTable[data[i]];
                if (index1 < 0)
                    index1 = 0;
                else if (index1 > 88)
                    index1 = 88;
                stepsize1 = stepsizeTable[index1];
                //
                i++;
                //Right Channel
                difference2 = 0;

                if ((data[i] & 4) != 0)
                    difference2 += stepsize2;
                if ((data[i] & 2) != 0)
                    difference2 += stepsize2 >> 1;
                if ((data[i] & 1) != 0)
                    difference2 += stepsize2 >> 2;
                difference2 += stepsize2 >> 3;

                if ((data[i] & 8) != 0)
                    difference2 = -difference2;
                newSample2 += difference2;

                if (newSample2 > 32767)
                    newSample2 = 32767;
                else if (newSample2 < -32768)
                    newSample2 = -32768;

                resul.AddRange(BitConverter.GetBytes((short)newSample2));

                index2 += indexTable[data[i]];
                if (index2 < 0)
                    index2 = 0;
                else if (index2 > 88)
                    index2 = 88;
                stepsize2 = stepsizeTable[index2];
                //
            }

            return resul.ToArray();
        }

        static byte[] Bit8ToBit4(byte[] data)
        {
            List<byte> bit4 = new List<byte>();

            for (int i = 0; i < data.Length; i++)
            {
                bit4.Add((byte)(data[i] & 0x0F));
                bit4.Add((byte)((data[i] & 0xF0) >> 4));
            }

            return bit4.ToArray();
        }
    }
}