using System;
namespace PacketDotNet
{
    public enum DnsType
    {
        HostAddress_A                       = 0x0001,
        AuthoritativeNameServer_NS          = 0x0002,
        MailDestination_MD                  = 0x0003,
        MailForwarder_MF                    = 0x0004,
        CanonicalName_CNAME                 = 0x0005,
        StartZoneOfAuthority_SOA            = 0x0006,
        MailboxDomainName_MB                = 0x0007,
        MailGroupMember_MG                  = 0x0008,
        MailRenameDomainName_MR             = 0x0009,
        NullRR_NULL                         = 0x000A,
        WellKnownServiceDescription_WKS     = 0x000B,
        DomainNamePointer_PTR               = 0x000C,
        HostInformation_HINFO               = 0x000D,
        MailboxOrMailListInformation_MINFO  = 0x000E,
        MailExchange_MX                     = 0x000F,
        TextStrings_TXT                     = 0x0010,

        // qtype values
        RequestTransferZone_AXFR            = 0x0011,
        RequestForMailboxRelated_MAILB      = 0x0012,
        RequestForMailAgentRRs_MAILA        = 0x0013,
    }
}

