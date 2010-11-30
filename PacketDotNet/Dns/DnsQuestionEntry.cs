/*
This file is part of PacketDotNet

PacketDotNet is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

PacketDotNet is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with PacketDotNet.  If not, see <http://www.gnu.org/licenses/>.
*/
/*
 *  Copyright 2010 Chris Morgan <chmorgan@gmail.com>
 */
using System;
using System.IO;
using MiscUtil.Conversion;
using PacketDotNet.Utils;

namespace PacketDotNet.Dns
{
    /// <summary>
    /// See http://www.faqs.org/rfcs/rfc1035.html section 4.1.2
    /// </summary>
    public class DnsQuestionEntry
    {
        public DnsQuestionEntry (byte[] bytes, int offset, out int bytesConsumed)
        {
            int length = -1;
            int currentOffset = offset;
            while(length != 0)
            {
                length = bytes[currentOffset];
                currentOffset++;

                // zero length terminates the ascii portion of the entry
                if(length == 0)
                {
                    break;
                }

                // read the text segment
                var str = System.Text.ASCIIEncoding.ASCII.GetString(bytes, currentOffset, length);
                currentOffset+=length;

                // add '.' in between entries for all entries except the first entry
                if(QName == null)
                {
                    QName = str;
                } else
                {
                    QName = QName + "." + str;
                }
            }

            // read the qtype
            QType = (DnsType)EndianBitConverter.Big.ToUInt16(bytes, currentOffset);
            currentOffset+=2;

            // read the qclass
            QClass = (DnsClass)EndianBitConverter.Big.ToUInt16(bytes, currentOffset);
            currentOffset+=2;

            bytesConsumed = offset;
        }

        /// <summary>
        /// Domain name being requested in this query
        /// </summary>
        public string QName
        {
            get; set;
        }

        public DnsType QType
        {
            get; set;
        }

        public DnsClass QClass
        {
            get; set;
        }

        public override string ToString ()
        {
            return string.Format("[DnsQuestionEntry: QName={0}, QType={1}, QClass={2}]", QName, QType, QClass);
        }
    }
}

