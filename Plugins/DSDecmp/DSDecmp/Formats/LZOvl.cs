using System;
using System.IO;

namespace DSDecmp.Formats
{
    /// <summary>
    /// The LZ-Overlay compression format. Compresses part of the file from end to start.
    /// Is used for the 'overlay' files in NDS games, as well as arm9.bin.
    /// Note that the last 12 bytes should not be included in the 'inLength' argument when
    /// decompressing arm9.bin. This is done automatically if a file is given instead of a stream.
    /// </summary>
    public class LZOvl : CompressionFormat
    {
        #region Method: Supports(string file)
        public override bool Supports(string file)
        {
            using (FileStream fstr = File.OpenRead(file))
            {
                long fLength = fstr.Length;
                // arm9.bin is special in the sense that the last 12 bytes should/can be ignored.
                if (Path.GetFileName(file) == "arm9.bin")
                    fLength -= 0xC;
                return this.Supports(fstr, fLength);
            }
        }
        #endregion

        #region Method: Supports(Stream, long)
        public override bool Supports(System.IO.Stream stream, long inLength)
        {
            // assume the 'inLength' does not include the 12 bytes at the end of arm9.bin

            // only allow integer-sized files
            if (inLength > 0xFFFFFFFFL)
                return false;
            // the header is 4 bytes minimum
            if (inLength < 4)
                return false;
            long streamStart = stream.Position;
            byte[] header = new byte[Math.Min(inLength, 0x20)];
            stream.Position += inLength - header.Length;
            stream.Read(header, 0, header.Length);
            // reset the stream
            stream.Position = streamStart;

            uint extraSize = IOUtils.ToNDSu32(header, header.Length - 4);
            if (extraSize == 0)
                return false; // do not decompress whenevr the last 4 bytes are 0; too many files have that.
            // if the extrasize is nonzero, the minimum header length is 8  bytes
            if (header.Length < 8)
                return false;
            byte headerLen = header[header.Length - 5];
            if (inLength < headerLen)
                return false;

            // the compressed length should fit in the input file
            int compressedLen = header[header.Length - 6] << 16
                                | header[header.Length - 7] << 8
                                | header[header.Length - 8];
            if (compressedLen >= inLength - headerLen && compressedLen != inLength)
                return false;

            // verify that the rest of the header is filled with 0xFF
            for (int i = header.Length - 9; i >= header.Length - headerLen; i--)
                if (header[i] != 0xFF)
                    return false;
            return true;
        }
        #endregion

        #region Method: Decompress(string, string)
        public override void Decompress(string infile, string outfile)
        {
            // make sure the output directory exists
            string outDirectory = Path.GetDirectoryName(outfile);
            if (!Directory.Exists(outDirectory))
                Directory.CreateDirectory(outDirectory);
            // open the two given files, and delegate to the format-specific code.
            using (FileStream inStream = new FileStream(infile, FileMode.Open),
                             outStream = new FileStream(outfile, FileMode.Create))
            {
                long fLength = inStream.Length;
                // arm9.bin needs special attention
                if (Path.GetFileName(infile) == "arm9.bin")
                    fLength -= 0xC;
                this.Decompress(inStream, fLength, outStream);
            }
        }
        #endregion

        #region Decompression method
        public override long Decompress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            #region Format description
            // Overlay LZ compression is basically just LZ-0x10 compression.
            // however the order if reading is reversed: the compression starts at the end of the file.
            // Assuming we start reading at the end towards the beginning, the format is:
            /*
             * u32 extraSize; // decompressed data size = file length (including header) + this value
             * u8 headerSize;
             * u24 compressedLength; // can be less than file size (w/o header). If so, the rest of the file is uncompressed.
             *                       // may also be the file size
             * u8[headerSize-8] padding; // 0xFF-s
             * 
             * 0x10-like-compressed data follows (without the usual 4-byte header).
             * The only difference is that 2 should be added to the DISP value in compressed blocks
             * to get the proper value.
             * the u32 and u24 are read most significant byte first.
             * if extraSize is 0, there is no headerSize, decompressedLength or padding.
             * the data starts immediately, and is uncompressed.
             * 
             * arm9.bin has 3 extra u32 values at the 'start' (ie: end of the file),
             * which may be ignored. (and are ignored here) These 12 bytes also should not
             * be included in the computation of the output size.
             */
            #endregion

            #region First read the last 4 bytes of the stream (the 'extraSize')

            // first go to the end of the stream, since we're reading from back to front
            // read the last 4 bytes, the 'extraSize'
            instream.Position += inLength - 4;

            byte[] buffer = new byte[4];
            try
            {
                instream.Read(buffer, 0, 4);
            }
            catch (System.IO.EndOfStreamException)
            {
                // since we're immediately checking the end of the stream, 
                // this is the only location where we have to check for an EOS to occur.
                throw new StreamTooShortException();
            }
            uint extraSize = IOUtils.ToNDSu32(buffer, 0);

            #endregion

            // if the extra size is 0, there is no compressed part, and the header ends there.
            if (extraSize == 0)
            {
                #region just copy the input to the output

                // first go back to the start of the file. the current location is after the 'extraSize',
                // and thus at the end of the file.
                instream.Position -= inLength;
                // no buffering -> slow
                buffer = new byte[inLength - 4];
                instream.Read(buffer, 0, (int)(inLength - 4));
                outstream.Write(buffer, 0, (int)(inLength - 4));

                // make sure the input is positioned at the end of the file
                instream.Position += 4;

                return inLength - 4;

                #endregion
            }
            else
            {
                // get the size of the compression header first.
                instream.Position -= 5;
                int headerSize = instream.ReadByte();

                // then the compressed data size.
                instream.Position -= 4;
                instream.Read(buffer, 0, 3);
                int compressedSize = buffer[0] | (buffer[1] << 8) | (buffer[2] << 16);

                // the compressed size sometimes is the file size.
                //if (compressedSize + headerSize >= inLength)
                //    compressedSize = (int)(inLength - headerSize);

                #region copy the non-compressed data

                // copy the non-compressed data first.
                buffer = new byte[inLength - compressedSize];
                instream.Position -= (inLength - 5);
                instream.Read(buffer, 0, buffer.Length);
                outstream.Write(buffer, 0, buffer.Length);

                #endregion

                // buffer the compressed data, such that we don't need to keep
                // moving the input stream position back and forth
                compressedSize -= headerSize;
                buffer = new byte[compressedSize];
                instream.Read(buffer, 0, compressedSize);

                // we're filling the output from end to start, so we can't directly write the data.
                // buffer it instead (also use this data as buffer instead of a ring-buffer for
                // decompression)
                byte[] outbuffer = new byte[compressedSize + headerSize + extraSize];

                int currentOutSize = 0;
                int decompressedLength = outbuffer.Length;
                int readBytes = 0;
                byte flags = 0, mask = 1;
                while (currentOutSize < decompressedLength)
                {
                    // (throws when requested new flags byte is not available)
                    #region Update the mask. If all flag bits have been read, get a new set.
                    // the current mask is the mask used in the previous run. So if it masks the
                    // last flag bit, get a new flags byte.
                    if (mask == 1)
                    {
                        if (readBytes >= compressedSize)
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        flags = buffer[buffer.Length - 1 - readBytes];
                        readBytes++;
                        mask = 0x80;
                    }
                    else
                    {
                        mask >>= 1;
                    }
                    #endregion

                    // bit = 1 <=> compressed.
                    if ((flags & mask) > 0)
                    {
                        // (throws when < 2 bytes are available)
                        #region Get length and displacement('disp') values from next 2 bytes
                        // there are < 2 bytes available when the end is at most 1 byte away
                        if (readBytes + 1 >= inLength)
                        {
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        }
                        int byte1 = buffer[compressedSize - 1 - readBytes];
                        readBytes++;

                        if (readBytes == compressedSize)
                        {
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        }
                        int byte2 = buffer[compressedSize - 1 - readBytes];
                        readBytes++;

                        // the number of bytes to copy
                        int length = byte1 >> 4;
                        length += 3;

                        // from where the bytes should be copied (relatively)
                        int disp = ((byte1 & 0x0F) << 8) | byte2;
                        disp += 3;

                        if (disp > currentOutSize)
                        {
                            if (currentOutSize < 2)
                                throw new InvalidDataException(String.Format(Main.Get_Traduction("S0D"), disp.ToString("X"),
                                    currentOutSize.ToString("X")));
                            // HACK. this seems to produce valid files, but isn't the most elegant solution.
                            // although this _could_ be the actual way to use a disp of 2 in this format,
                            // as otherwise the minimum would be 3 (and 0 is undefined, and 1 is less useful).
                            disp = 2;
                        }
                        #endregion

                        int bufIdx = currentOutSize - disp;
                        for (int i = 0; i < length; i++)
                        {
                            byte next = outbuffer[outbuffer.Length - 1 - bufIdx];
                            bufIdx++;
                            outbuffer[outbuffer.Length - 1 - currentOutSize] = next;
                            currentOutSize++;
                        }
                    }
                    else
                    {
                        if (readBytes >= inLength)
                            throw new NotEnoughDataException(currentOutSize, decompressedLength);
                        byte next = buffer[buffer.Length - 1 - readBytes];
                        readBytes++;

                        outbuffer[outbuffer.Length - 1 - currentOutSize] = next;
                        currentOutSize++;
                    }
                }

                // write the decompressed data
                outstream.Write(outbuffer, 0, outbuffer.Length);

                // make sure the input is positioned at the end of the file; the stream is currently
                // at the compression header.
                instream.Position += headerSize;

                return decompressedLength + (inLength - headerSize - compressedSize);
            }
        }
        #endregion

        #region Compression method; delegates to CUE's BLZ_Encoder
        public override int Compress(System.IO.Stream instream, long inLength, System.IO.Stream outstream)
        {
            // don't bother trying to get the optimal not-compressed - compressed ratio for now.
            // Either compress fully or don't compress (as the format cannot handle decompressed
            // sizes that are smaller than the compressed file).

            if (inLength > 0xFFFFFF)
                throw new InputTooLargeException();

            // read the input.
            byte[] indata = new byte[inLength];
            instream.Read(indata, 0, (int)inLength);

            byte[] outdata = this.BLZ_Encode(indata, false);

            int totalCompFileLength = outdata.Length;

            if (totalCompFileLength < inLength)
            {
                outstream.Write(outdata, 0, totalCompFileLength);
                return totalCompFileLength;
            }
            else
            {
                outstream.Write(indata, 0, (int)inLength);
                outstream.WriteByte(0);
                outstream.WriteByte(0);
                outstream.WriteByte(0);
                outstream.WriteByte(0);
                return (int)inLength + 4;
            }
        }
        #endregion

        #region BLZ_Encoder by CUE
        /*----------------------------------------------------------------------------*/
        /*--  blz.c - Bottom LZ coding for Nintendo GBA/DS                          --*/
        /*--  Copyright (C) 2011 CUE                                                --*/
        /*--                                                                        --*/
        /*--  This program is free software: you can redistribute it and/or modify  --*/
        /*--  it under the terms of the GNU General Public License as published by  --*/
        /*--  the Free Software Foundation, either version 3 of the License, or     --*/
        /*--  (at your option) any later version.                                   --*/
        /*--                                                                        --*/
        /*--  This program is distributed in the hope that it will be useful,       --*/
        /*--  but WITHOUT ANY WARRANTY; without even the implied warranty of        --*/
        /*--  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the          --*/
        /*--  GNU General Public License for more details.                          --*/
        /*--                                                                        --*/
        /*--  You should have received a copy of the GNU General Public License     --*/
        /*--  along with this program. If not, see <http://www.gnu.org/licenses/>.  --*/
        /*----------------------------------------------------------------------------*/
        public const uint BLZ_SHIFT = 1;          // bits to shift
        public const byte BLZ_MASK = 0x80;       // bits to check:
                                                 // ((((1 << BLZ_SHIFT) - 1) << (8 - BLZ_SHIFT)

        public const uint BLZ_THRESHOLD = 2;          // max number of bytes to not encode
        public const uint BLZ_N = 0x1002;     // max offset ((1 << 12) + 2)
        public const uint BLZ_F = 0x12;       // max coded ((1 << 4) + BLZ_THRESHOLD)

        public const uint RAW_MINIM = 0x00000000; // empty file, 0 bytes
        public const uint RAW_MAXIM = 0x00FFFFFF; // 3-bytes length, 16MB - 1

        public const uint BLZ_MINIM = 0x00000004; // header only (empty RAW file)
        public const uint BLZ_MAXIM = 0x01400000; // 0x0120000A, padded to 20MB:

        private static bool lookAhead = false;
        /// <summary>
        /// Sets the flag that determines if "LZ-CUE" method should be used when compressing
        /// with the LZ-Ovl format. The default is false, which is what is used in the original
        /// implementation.
        /// </summary>
        public static bool LookAhead
        {
            set { lookAhead = value; }
        }

        void BLZ_Invert(byte[] buffer, uint start, uint length)
        {
            byte ch;
            uint bottom = start + length - 1;

            while (start < bottom)
            {
                ch = buffer[start];
                buffer[start++] = buffer[bottom];
                buffer[bottom--] = ch;
            }
        }

        byte[] Memory(int length, int size)
        {
            return new byte[length * size];
        }

        public byte[] BLZ_Encode(byte[] raw_buffer, bool arm9)
        {
            byte[] pak_buffer, new_buffer;
            uint raw_len, pak_len, new_len;

            raw_len = (uint)raw_buffer.Length;

            pak_buffer = null;
            pak_len = BLZ_MAXIM + 1;

            new_buffer = BLZ_Code(raw_buffer, raw_len, out new_len, arm9);
            if (new_len < pak_len)
            {
                pak_buffer = new_buffer;
                pak_len = new_len;
            }

            if (pak_buffer.Length != pak_len)
            {
                byte[] retbuf = new byte[pak_len];
                for (int i = 0; i < pak_len; ++i)
                {
                    retbuf[i] = pak_buffer[i];
                }
                pak_buffer = retbuf;
            }

            return pak_buffer;
        }

        private void SEARCH(ref uint l, ref uint p, ref byte[] raw_buffer, ref uint raw, ref uint raw_end, ref uint max, ref uint pos, ref uint len)
        {
            l = BLZ_THRESHOLD;

            max = raw >= BLZ_N ? BLZ_N : raw;
            for (pos = 3; pos <= max; pos++)
            {
                for (len = 0; len < BLZ_F; len++)
                {
                    if (raw + len == raw_end) break;
                    if (len >= pos) break;
                    if (raw_buffer[raw + len] != raw_buffer[raw + len - pos]) break;
                }

                if (len > l)
                {
                    p = pos;
                    if ((l = len) == BLZ_F) break;
                }
            }
        }

        byte[] BLZ_Code(byte[] raw_buffer, uint raw_len, out uint new_len, bool arm9)
        {
            byte[] pak_buffer;
            uint pak, raw, raw_end, flg = 0;
            byte[] tmp;
            uint pak_len, inc_len, hdr_len, enc_len, len = 0, pos = 0, max = 0;
            uint len_best = 0, pos_best = 0, len_next = 0, pos_next = 0, len_post = 0, pos_post = 0;
            uint pak_tmp, raw_tmp, raw_new;
            byte mask;

            pak_tmp = 0;
            raw_tmp = raw_len;

            pak_len = raw_len + ((raw_len + 7) / 8) + 11;
            pak_buffer = Memory((int)pak_len, 1);

            raw_new = raw_len;
            if (arm9)
            {
                if (raw_len < 0x4000)
                {
                    Console.WriteLine("WARNING: ARM9 must be greater than 16KB, switch [arm9] disabled");
                }
                else
                {
                    raw_new -= 0x4000;
                }
            }

            BLZ_Invert(raw_buffer, 0, raw_len);

            pak = 0;
            raw = 0;
            raw_end = raw_new;

            mask = 0;

            while (raw < raw_end)
            {
                mask = (byte)(((uint)mask) >> ((int)BLZ_SHIFT));

                if (mask == 0)
                {
                    flg = pak++;
                    pak_buffer[flg] = 0;
                    mask = BLZ_MASK;
                }

                SEARCH(ref len_best, ref pos_best, ref raw_buffer, ref raw, ref raw_end, ref max, ref pos, ref len);

                // LZ-CUE optimization start
                if (lookAhead)
                {
                    if (len_best > BLZ_THRESHOLD)
                    {
                        if (raw + len_best < raw_end)
                        {
                            raw += len_best;
                            SEARCH(ref len_next, ref pos_next, ref raw_buffer, ref raw, ref raw_end, ref max, ref pos, ref len);
                            raw -= len_best - 1;
                            SEARCH(ref len_post, ref pos_post, ref raw_buffer, ref raw, ref raw_end, ref max, ref pos, ref len);
                            raw--;

                            if (len_next <= BLZ_THRESHOLD) len_next = 1;
                            if (len_post <= BLZ_THRESHOLD) len_post = 1;

                            if (len_best + len_next <= 1 + len_post) len_best = 1;
                        }
                    }
                }
                // LZ-CUE optimization end

                pak_buffer[flg] <<= 1;
                if (len_best > BLZ_THRESHOLD)
                {
                    raw += len_best;
                    pak_buffer[flg] |= 1;
                    pak_buffer[pak] = (byte)(((len_best - (BLZ_THRESHOLD + 1)) << 4) | ((pos_best - 3) >> 8));
                    pak++;
                    pak_buffer[pak] = (byte)((pos_best - 3) & 0xFF);
                    pak++;
                }
                else
                {
                    pak_buffer[pak] = raw_buffer[raw];
                    pak++;
                    raw++;
                }

                if (pak + raw_len - raw < pak_tmp + raw_tmp)
                {
                    pak_tmp = pak;
                    raw_tmp = raw_len - raw;
                }
            }

            while ((mask != 0) && (mask != 1))
            {
                mask = (byte)(((uint)mask) >> ((int)BLZ_SHIFT));
                pak_buffer[flg] <<= 1;
            }

            pak_len = pak;

            BLZ_Invert(raw_buffer, 0, raw_len);
            BLZ_Invert(pak_buffer, 0, pak_len);

            if ((pak_tmp == 0) || (raw_len + 4 < ((pak_tmp + raw_tmp + 3) & -4) + 8))
            {
                pak = 0;
                raw = 0;
                raw_end = raw_len;

                while (raw < raw_end)
                {
                    pak_buffer[pak] = raw_buffer[raw];
                    pak++;
                    raw++;
                }

                while ((pak & 3) != 0)
                {
                    pak_buffer[pak] = 0;
                    pak++;
                }

                pak_buffer[pak] = 0;
                pak_buffer[pak + 1] = 0;
                pak_buffer[pak + 2] = 0;
                pak_buffer[pak + 3] = 0;
                pak += 4;
            }
            else
            {
                tmp = Memory((int)(raw_tmp + pak_tmp + 11), 1);

                for (len = 0; len < raw_tmp; len++)
                    tmp[len] = raw_buffer[len];

                for (len = 0; len < pak_tmp; len++)
                    tmp[raw_tmp + len] = pak_buffer[len + pak_len - pak_tmp];

                pak_buffer = tmp;
                pak = raw_tmp + pak_tmp;

                enc_len = pak_tmp;
                hdr_len = 8;
                inc_len = raw_len - pak_tmp - raw_tmp;

                while ((pak & 3) != 0)
                {
                    pak_buffer[pak] = 0xFF;
                    pak++;
                    hdr_len++;
                }

                byte[] tmpbyte = BitConverter.GetBytes(enc_len + hdr_len);
                tmpbyte.CopyTo(pak_buffer, pak);
                pak += 3;
                pak_buffer[pak] = (byte)hdr_len;
                pak++;
                tmpbyte = BitConverter.GetBytes(inc_len - hdr_len);
                tmpbyte.CopyTo(pak_buffer, pak);
                pak += 4;
            }

            new_len = pak;

            return (pak_buffer);
        }
        /*----------------------------------------------------------------------------*/
        /*--  EOF                                           Copyright (C) 2011 CUE  --*/
        /*----------------------------------------------------------------------------*/
        #endregion
    }
}
