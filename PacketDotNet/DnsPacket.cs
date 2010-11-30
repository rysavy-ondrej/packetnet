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
﻿using System;
using System.Collections.Generic;
using PacketDotNet.Dns;
using PacketDotNet.Utils;
using MiscUtil.Conversion;

//todo:
// - use single bytes when retrieving the flag bytes, its much faster
// than using the bit converter
// - perform some validation in the byte parser
// - finish implementing the DnsQuestionEntry and other payload formats

namespace PacketDotNet
{
    /// <summary>
    /// Dns packet
    /// See http://www.faqs.org/rfcs/rfc1035.html section 4.1.1
    /// 
    /// NOTE: DnsPacket is partially parsed when constructed unlike many other
    ///       packet types where all values are kept in place in the physical memory
    ///       used to represent the packet. DnsPacket is partially parsed because
    ///       adding or removing entries affects the total packet length and its easier
    ///       to store and manipulate the fully dissected packet vs. resizing and shifting
    ///       memory around.
    ///
    ///       Becuase of this parsing, QDCOUNT, ANCOUNT, NSCOUNT and ARCOUNT
    ///       are a product of the number of entries inside of the clasa and not of the
    ///       value in physical memory. eg. Adding a new
    ///       query would increase the reported QDCOUNT by one.
    /// </summary>
    public class DnsPacket : ApplicationPacket
    {
#if DEBUG
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#else
        // NOTE: No need to warn about lack of use, the compiler won't
        //       put any calls to 'log' here but we need 'log' to exist to compile
#pragma warning disable 0169
        private static readonly ILogInactive log;
#pragma warning restore 0169
#endif

        /// <summary> Fetch the transaction id</summary>
        virtual public ushort TransactionId
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset);
            }

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset);
            }
        }

        /// <summary> Retrieve the type of dns packet</summary>
        virtual public DnsPacketType DnsType
        {
            get
            {
                bool val = ((Flags >> 15) == 1) ? true : false;
                if(val == false)
                    return DnsPacketType.Query;
                else
                    return DnsPacketType.Response;
            }

            set
            {
                ushort val;
                if(value == DnsPacketType.Response)
                {
                    val = 0x8000;
                } else
                {
                    val = 0;
                }

                Flags = (ushort)((Flags & 0x7FFF) | val);
            }
        }

        /// <summary> Retrieve the opcode</summary>
        public int Opcode
        {
            get
            {
                return (Flags >> 11) & 0xF;
            }

            set
            {
                Flags = (ushort)((Flags & 0x87FF) | ((value & 0xF) << 11));
            }
        }

        /// <summary>Authoritative answer</summary>
        public bool AuthoritativeAnswer
        {
            get
            {
                return ((Flags & 0x400) != 0) ? true : false;
            }

            set
            {
                ushort val = (ushort)(value ? 1 : 0);
                Flags = (ushort)((Flags & 0xFBFF) | (val << 10));
            }
        }

        /// <summary>Dns packet was truncated</summary>
        public bool Truncation
        {
            get
            {
                return ((Flags & 0x200) != 0) ? true : false;
            }

            set
            {
                int val = value ? 1 : 0;
                Flags = (ushort)((Flags & 0xFDFF) | (val << 9));
            }
        }

        /// <summary>Recursion desired </summary>
        public bool RecursionDesired
        {
            get
            {
                return ((Flags & 0x100) != 0) ? true : false;
            }

            set
            {
                ushort val = (ushort)(value ? 1 : 0);
                Flags = (ushort)((Flags & 0xFEFF) | (val << 8));
            }
        }

        /// <summary>Recursion setting </summary>
        public bool RecursionAvailable
        {
            get
            {
                return (Flags & 0x80) != 0 ? true : false;
            }

            set
            {
                var val = value ? 1 : 0;
                Flags = (ushort)((Flags & 0xFF7F) | (val << 7));
            }
        }

        /// <summary>
        /// Reserved, must be zero 
        /// </summary>
        public int Z
        {
            get
            {
                return (Flags >> 4) & 0xF;
            }

            set
            {
                Flags = (ushort)((Flags & 0xFF8F) | ((value & 0x7) << 4));
            }
        }

        /// <summary>Response code </summary>
        public DnsResponseCode ResponseCode
        {
            get
            {
                return (DnsResponseCode)(Flags & 0xF);
            }

            set
            {
                Flags = (ushort)((Flags & 0xFFF0) | (int)value);
            }
        }

        private ushort Flags
        {
            get
            {
                return EndianBitConverter.Big.ToUInt16(header.Bytes, header.Offset + DnsFields.FlagsPosition);
            }

            set
            {
                var val = value;
                EndianBitConverter.Big.CopyBytes(val, header.Bytes, header.Offset + DnsFields.FlagsPosition);
            }
        }

        public List<DnsQuestionEntry> Questions
        {
            get; set;
        }

#if false
        /// <value>
        /// Length in bytes of the header and payload
        /// </value>
        virtual public int Length
        {
            get
            {
                return EndianBitConverter.Big.ToInt16(header.Bytes,
                                                      header.Offset + UdpFields.HeaderLengthPosition);
            }

            // Internal because it is updated based on the payload when
            // its bytes are retrieved
            internal set
            {
                var val = (Int16)value;
                EndianBitConverter.Big.CopyBytes(val,
                                                 header.Bytes,
                                                 header.Offset + UdpFields.HeaderLengthPosition);
            }
        }
#endif

        /// <summary> Fetch ascii escape sequence of the color associated with this packet type.</summary>
        override public System.String Color
        {
            get
            {
                return AnsiEscapeSequences.Cyan;
            }
        }

#if false
        /// <summary>
        /// Create from values
        /// </summary>
        /// <param name="SourcePort">
        /// A <see cref="System.UInt16"/>
        /// </param>
        /// <param name="DestinationPort">
        /// A <see cref="System.UInt16"/>
        /// </param>
        public DnsPacket(ushort SourcePort, ushort DestinationPort)
            : base(new PosixTimeval())
        {
            log.Debug("");

            // allocate memory for this packet
            int offset = 0;
            int length = UdpFields.HeaderLength;
            var headerBytes = new byte[length];
            header = new ByteArraySegment(headerBytes, offset, length);

            // set instance values
            this.SourcePort = SourcePort;
            this.DestinationPort = DestinationPort;
        }
#endif

        /// <summary>
        /// byte[]/int offset constructor, timeval defaults to the current time
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        public DnsPacket(byte[] Bytes, int Offset) :
            this(Bytes, Offset, new PosixTimeval())
        { }

        /// <summary>
        /// byte[]/int offset/PosixTimeval constructor
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        public DnsPacket(byte[] Bytes, int Offset, PosixTimeval Timeval) :
            base(Timeval)
        {
            // set the header field, header field values are retrieved from this byte array
//            header = new ByteArraySegment(Bytes, Offset, DnsFields.HeaderLength);

            // we consider the fixed position header portion to be the independent entries
            // at the front of the packet, not including anything beyond the flags
            header = new ByteArraySegment(Bytes, Offset, DnsFields.QuestionCountPosition);

#if false
            // store the payload bytes
            payloadPacketOrData = new PacketOrByteArraySegment();
            payloadPacketOrData.TheByteArraySegment = header.EncapsulatedBytes();
#else
            var byteArraySegment = header.EncapsulatedBytes();

            // read the count of questions, answers, name server records and
            // additional records
            int position = byteArraySegment.Offset;
            int qdcount = EndianBitConverter.Big.ToUInt16(byteArraySegment.Bytes, position);
            position+=2;
            int ancount = EndianBitConverter.Big.ToUInt16(byteArraySegment.Bytes, position);
            position+=2;
            int nscount = EndianBitConverter.Big.ToUInt16(byteArraySegment.Bytes, position);
            position+=2;
            int arcount = EndianBitConverter.Big.ToUInt16(byteArraySegment.Bytes, position);
            position+=2;

            log.DebugFormat("qdcount {0}, ancount {1}, nscount {2}, arcount {3}",
                            qdcount, ancount, nscount, arcount);

            Questions = new List<DnsQuestionEntry>();
            int bytesConsumed;
            for(int x = 0; x < qdcount; x++)
            {
                var dnsquestionentry = new DnsQuestionEntry(byteArraySegment.Bytes, position, out bytesConsumed);
                position += bytesConsumed;
                Questions.Add(dnsquestionentry);
            }
#endif
        }

        /// <summary>
        /// Constructor when this packet is encapsulated in another packet
        /// </summary>
        /// <param name="Bytes">
        /// A <see cref="System.Byte"/>
        /// </param>
        /// <param name="Offset">
        /// A <see cref="System.Int32"/>
        /// </param>
        /// <param name="Timeval">
        /// A <see cref="PosixTimeval"/>
        /// </param>
        /// <param name="ParentPacket">
        /// A <see cref="Packet"/>
        /// </param>
        public DnsPacket(byte[] Bytes, int Offset, PosixTimeval Timeval,
                         Packet ParentPacket) :
            this(Bytes, Offset, Timeval)
        {
            this.ParentPacket = ParentPacket;
        }

        /// <summary> Convert this UDP packet to a readable string.</summary>
        public override System.String ToString()
        {
            return ToColoredString(false);
        }

        /// <summary> Generate string with contents describing this UDP packet.</summary>
        /// <param name="colored">whether or not the string should contain ansi
        /// color escape sequences.
        /// </param>
        public override System.String ToColoredString(bool colored)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append('[');
            if (colored)
                buffer.Append(Color);
            buffer.Append("DnsPacket");
            if (colored)
                buffer.Append(AnsiEscapeSequences.Reset);
            buffer.AppendFormat("TransactionId {0:x4}", TransactionId);
            foreach(var q in Questions)
            {
                buffer.Append(q.ToString());
            }
            buffer.Append(']');

            return buffer.ToString();
        }

        /// <summary>
        /// Returns the DnsPacket inside of the Packet p or null if
        /// there is no encapsulated packet
        /// </summary>
        /// <param name="p">
        /// A <see cref="Packet"/>
        /// </param>
        /// <returns>
        /// A <see cref="DnsPacket"/>
        /// </returns>
        public static DnsPacket GetEncapsulated(Packet p)
        {
            if(p is InternetLinkLayerPacket)
            {
                var payload = InternetLinkLayerPacket.GetInnerPayload((InternetLinkLayerPacket)p);
                if(payload is IpPacket)
                {
                    var innerPayload = payload.PayloadPacket;
                    if(innerPayload is UdpPacket)
                    {
#if false
                        // attempt to convert these bytes to a DnsPacket
                        return new DnsPacket(innerPayload.PayloadData);
#endif
                    }
                }
            }

            return null;
        }

#if false
        /// <summary>
        /// Generate a random packet 
        /// </summary>
        /// <returns>
        /// A <see cref="DnsPacket"/>
        /// </returns>
        public static DnsPacket RandomPacket()
        {
            var rnd = new Random();
            var SourcePort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
            var DestinationPort = (ushort)rnd.Next(ushort.MinValue, ushort.MaxValue);
#if false
            return new UdpPacket(SourcePort, DestinationPort);
#endif
        }
#endif
    }
}
