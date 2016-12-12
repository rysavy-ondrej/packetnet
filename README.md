Packet.Net

Packet.Net is a high performance .Net assembly for dissecting and constructing
network packets such as ethernet, ip, tcp, udp etc.

Author: Chris Morgan <chmorgan@gmail.com>

https://github.com/chmorgan/packetnet

Code is found in the PacketDotNet namespace.

This branch adds some extensions:
* Visitor pattern - It is possible to traverse all encapsulated packets using the visitor pattern.

Some other extensions are on the plan:
* Light-weight parsing - No Packet instances are created. ```ByteArraySegment```
is used to provide data to subsequent parsers.
* TCP reassembling - Support for TCP stream reassembling will be implemented.
* Application protocols - Interface for parsing application protocols will be provided.


Getting started
===============

A few basic examples can be found in the Examples/ directory.


Debug vs. Release builds
========================

The Debug build depends on log4net and has log4net calls in some of its classes and
code paths.

The Release build does NOT depend on log4net and, taking advantage of conditional
method attributes, does not include any calls to log4net methods. This ensures that there
is no performance impact on release builds.


Performance benchmarks
======================

The Test/ directory contains a few benchmarks that were used to guide the design
and implementation of Packet.Net. These benchmarks either contain 'performance' or
'benchmark' in their names.

If you have a performance concern or issue you'll want to write a concise test that reproduces
your usage case in a controlled manner. It will then be possible to run and re-run
this test case in various profiling modes in order to look at potential ways of
optimizing code. The tests will also provide a baseline from which to compare
any proposed performance improvements in order to ensure that changes are not
inadvertantly reducing instead of increasing performance.



Packet Parsing and Analysis
===========================
Packet parsing is simple by calling ```Parse``` method of ```Packet``` class.
To analyze the packet and enumerate all/selected fields from the packet,
several options are possible. One option relies on testing
packet's concrete class and behave accordingly (the following code is using new C#7 feature):
```CSharp

var packet = Packet.ParsePacket(linkLayer, frameBytes);
while (packet != null)
{
    switch (packet)
    {
      case EthernetPacket eth:
        ...
      break;
      case ARPPacket arp:
      ...
      break;
      case LLDPPacket lldp:
      ...
      break;
      case IPv4Packet ipv4:
      ...
      break;
      case IPv6Packet ipv6:
      ...
      break;
      case ICMPv4Packet icmpv4:
      ...
      break;
      case ICMPv6Packet icmpv6:
      ...
      break;
      case IGMPv2Packet igmpv2:
      ...
      break;
      case Ieee8021QPacket ieee8021q:
      ...
      break;
      case TcpPacket tcp:
      ...
      break;
      case UdpPacket udp:
      ...
      break;
    }
    packet = packet.PayloadPacket;
}
```

Other option is to deploy the Visitor pattern:
```CSharp
// TODO: implement selected Visit* methods:
class Visitor : PacketVisitor
{
  public abstract void VisitArpPacket(ARPPacket packet) { ...}
          public override void VisitEthernetPacket(EthernetPacket packet) { ...}
          public override void VisitICMPv4Packet(ICMPv4Packet packet) { ...}
          public override void VisitICMPv6Packet(ICMPv6Packet packet) { ...}

          public override void VisitIPv4Packet(IPv4Packet packet) { ...}
          public override void VisitIPv6Packet(IPv6Packet packet) { ...}

          public void VisitTcpPacket(TcpPacket packet) { ...}
          public override void VisitUdpPacket(UdpPacket packet) { ...}
}

var packet = Packet.ParsePacket(linkLayer, frameBytes);
packet.Accept(visitor);
```
