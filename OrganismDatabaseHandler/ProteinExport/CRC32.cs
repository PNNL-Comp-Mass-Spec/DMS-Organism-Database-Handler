using System;
using System.IO;

namespace OrganismDatabaseHandler.ProteinExport
{
    [Obsolete("Use CRC32 in PRISM.dll", true)]
    public class CRC32
    {

        // This is v2 of the VB CRC32 algorithm provided by Paul
        // (wpsjr1@succeed.net) - much quicker than the nasty
        // original version I posted.  Excellent work!

        private readonly uint[] crc32Table;
        private const int BUFFER_SIZE = 1024;

        public uint GetCrc32(Stream stream)
        {
            var crc32Result = 0xFFFFFFFF;

            var buffer = new byte[1025];
            var readSize = BUFFER_SIZE;

            var count = stream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                for (var i = 0; i < count; i++)
                {
                    var iLookup = crc32Result & 0xFF ^ buffer[i];
                    crc32Result = (crc32Result & 0xFFFFFF00) / 0x100 & 0xFFFFFF;   // nasty shr 8 with vb :/
                    crc32Result = crc32Result ^ crc32Table[iLookup];
                }

                count = stream.Read(buffer, 0, readSize);
            }

            return ~crc32Result;
        }

        public CRC32()
        {

            // This is the official polynomial used by CRC32 in PKZip.
            // Often the polynomial is shown reversed (04C11DB7).
            var dwPolynomial = 0xEDB88320;

            crc32Table = new uint[257];

            for (uint i = 0; i <= 255; i++)
            {
                var dwCrc = i;
                // ReSharper disable once RedundantAssignment
                for (var j = 8; j >= 1; j -= 1)
                {
                    if (Convert.ToBoolean(dwCrc & 1))
                    {
                        dwCrc = (uint)((dwCrc & 0xFFFFFFFE) / 2L & 0x7FFFFFFFL);
                        dwCrc = dwCrc ^ dwPolynomial;
                    }
                    else
                    {
                        dwCrc = (uint)((dwCrc & 0xFFFFFFFE) / 2L & 0x7FFFFFFFL);
                    }
                }

                crc32Table[i] = dwCrc;
            }
        }
    }
}