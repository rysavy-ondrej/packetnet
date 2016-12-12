using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PacketDotNet.Ieee80211;

namespace PacketDotNet
{
    /// <summary>
    /// Base class for packet visitors. 
    /// </summary>
    abstract public class PacketVisitor
    {
        
        public abstract void VisitWakeOnLanPacket(WakeOnLanPacket packet);
        public abstract void VisitUdpPacket(UdpPacket packet);
        public abstract void VisitTcpPacket(TcpPacket packet);
        public abstract void VisitPPPPacket(PPPPacket packet);
        public abstract void VisitPPPoEPacket(PPPoEPacket packet);
        public abstract void VisitOSPFv2Packet(OSPFv2Packet packet);
        public abstract void VisitArpPacket(ARPPacket packet);
        public abstract void VisitEthernetPacket(EthernetPacket packet);
        public abstract void VisitLLDPPacket(LLDPPacket packet);
        public abstract void VisitRadioPacket(RadioPacket packet);
        public abstract void VisitPpiPacket(PpiPacket packet);
        public abstract void VisitLinuxSLLPacket(LinuxSLLPacket packet);
        public abstract void VisitICMPv4Packet(ICMPv4Packet packet);
        public abstract void VisitICMPv6Packet(ICMPv6Packet packet);
        public abstract void VisitIGMPv2Packet(IGMPv2Packet packet);
        public abstract void VisitIPv4Packet(IPv4Packet packet);
        public abstract void VisitIPv6Packet(IPv6Packet packet);
        public abstract void VisitIeee8021QPacket(Ieee8021QPacket packet);
        public abstract void VisitIeee80211ManagementFrame(ManagementFrame frame);
        public abstract void VisitIeee80211ControlFrame(ControlFrame frame);
        public abstract void VisitIeee80211DataFrame(DataFrame frame);
    }
}
