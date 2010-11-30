using System;
namespace PacketDotNet
{
    public enum DnsResponseCode
    {
        NoError = 0,
        FormatError,
        ServerFailure,
        NameError,
        NotImplemented,
        Refused
    }
}

