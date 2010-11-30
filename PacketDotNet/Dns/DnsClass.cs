using System;
namespace PacketDotNet.Dns
{
    public enum DnsClass
    {
        /// <summary> Internet </summary>
        In = 0x0001,
        /// <summary> CSNET class (obsolete) </summary>
        Cs = 0x0002,
        /// <summary> CHAOS class </summary>
        Ch = 0x0003,
        /// <summary> Hesiod </summary>
        Hs = 0x0004,

        /// <summary> QClass any class </summary>
        AnyClass = 0xFF
    }
}

