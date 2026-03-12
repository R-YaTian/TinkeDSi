using System;
using System.Collections.Generic;
using System.Text;

namespace Be.Windows.Forms
{
    /// <summary>
    /// The interface for objects that can translate between characters and bytes.
    /// </summary>
    public interface IByteCharConverter
    {
        /// <summary>
        /// Returns the character to display for the byte passed across.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        char ToChar(byte b);

        /// <summary>
        /// Returns the byte to use when the character passed across is entered during editing.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        byte[] ToByte(char c);

        string ToString();

        Encoding ToEncoding();
    }

    /// <summary>
    /// The default <see cref="IByteCharConverter"/> implementation.
    /// </summary>
    public class DefaultByteCharConverter : IByteCharConverter
    {
        /// <summary>
        /// Returns the character to display for the byte passed across.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual char ToChar(byte b)
        {
            return b > 0x1F && !(b > 0x7E && b < 0xA0) ? (char)b : '.';
        }

        /// <summary>
        /// Returns the byte to use for the character passed across.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual byte[] ToByte(char c)
        {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)c;
            return bytes;
        }

        /// <summary>
        /// Returns a description of the byte char provider.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ASCII (Default)";
        }

        public Encoding ToEncoding()
        {
            return Encoding.Default;
        }
    }

    /// <summary>
    /// A byte char provider that can translate bytes encoded in codepage 500 EBCDIC
    /// </summary>
    public class EbcdicByteCharProvider : IByteCharConverter
    {
        /// <summary>
        /// The IBM EBCDIC code page 500 encoding. Note that this is not always supported by .NET,
        /// the underlying platform has to provide support for it.
        /// </summary>
        private Encoding _ebcdicEncoding = Encoding.GetEncoding(500);

        /// <summary>
        /// Returns the EBCDIC character corresponding to the byte passed across.
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public virtual char ToChar(byte b)
        {
            string encoded = _ebcdicEncoding.GetString(new byte[] { b });
            return encoded.Length > 0 ? encoded[0] : '.';
        }

        /// <summary>
        /// Returns the byte corresponding to the EBCDIC character passed across.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public virtual byte[] ToByte(char c)
        {
            byte[] decoded = _ebcdicEncoding.GetBytes(new char[] { c });
            return decoded.Length > 0 ? decoded : (new byte[1] { 0 });
        }

        /// <summary>
        /// Returns a description of the byte char provider.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "EBCDIC (Code Page 500)";
        }

        public Encoding ToEncoding()
        {
            return this._ebcdicEncoding;
        }
    }

    public class MultiByteCharConverter : IByteCharConverter
    {
        Encoding encoding;
        List<byte> requiredChar;

        public MultiByteCharConverter(string encoding)
        {
            this.encoding = Encoding.GetEncoding(encoding);
            requiredChar = new List<byte>();
        }

        public byte[] ToByte(char c)
        {
            byte[] decoded = encoding.GetBytes(new char[] { c });
            return decoded.Length > 0 ? decoded : (new byte[1] { 0 });
        }

        public char ToChar(byte b)
        {
            if (encoding.WebName == "shift_jis" || encoding.WebName == "gb2312")
                return ToCharShiftJisOrGBK(b);
            if (encoding.WebName == "utf-8")
                return ToCharUtf8(b);
            if (encoding.WebName == "utf-16")
                return ToCharUtf16Le(b);
            if (encoding.WebName == "utf-16BE")
                return ToCharUtf16Be(b);

            return encoding.GetChars(new byte[] { b })[0];
        }

        private char ToCharShiftJisOrGBK(byte b)
        {
            if (requiredChar.Count == 0 && b > 0x7F)
            {
                requiredChar.Add(b);
                return '\x20';
            }

            requiredChar.Add(b);
            string c = new String(encoding.GetChars(requiredChar.ToArray()));
            requiredChar.Clear();
            return (c[0] > '\x1F' ? c[0] : '.');
        }

        private char ToCharUtf16Le(byte b)
        {
            requiredChar.Add(b);

            if (requiredChar.Count == 2)
            {
                char c = BitConverter.ToChar(requiredChar.ToArray(), 0);
                requiredChar.Clear();
                return (c > '\x1F' ? c : '.');
            }

            return '\x20';
        }

        private char ToCharUtf16Be(byte b)
        {
            requiredChar.Add(b);

            if (requiredChar.Count == 2)
            {
                byte[] bytes = requiredChar.ToArray();
                Array.Reverse(bytes);
                char c = BitConverter.ToChar(bytes, 0);
                requiredChar.Clear();
                return (c > '\x1F' ? c : '.');
            }

            return '\x20';
        }

        private char ToCharUtf8(byte b)
        {
            if (requiredChar.Count == 0 && !((b & 0x80) == 0))
            {
                requiredChar.Add(b);
                return '\x20';
            }
            else if (requiredChar.Count == 1 && !((requiredChar[0] & 0xE0) == 0xC0))
            {
                requiredChar.Add(b);
                return '\x20';
            }
            else if (requiredChar.Count == 2 && !((requiredChar[0] & 0xF0) == 0xE0))
            {
                requiredChar.Add(b);
                return '\x20';
            }

            requiredChar.Add(b);
            string c = new String(encoding.GetChars(requiredChar.ToArray()));
            requiredChar.Clear();
            return (c[0] > '\x1F' ? c[0] : '.');
        }

        public override string ToString()
        {
            if (encoding.WebName == "utf-16" || encoding.WebName == "utf-16BE")
            {
                return "Unicode";
            }
            return encoding.WebName;
        }

        public Encoding ToEncoding()
        {
            return this.encoding;
        }

        public void ClearCache()
        {
            requiredChar.Clear();
        }
    }
}
