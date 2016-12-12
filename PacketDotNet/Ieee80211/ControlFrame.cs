using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacketDotNet.Ieee80211
{
    public abstract class ControlFrame : MacFrame
    {
        public override void Accept(PacketVisitor visitor)
        {
            visitor.VisitIeee80211ControlFrame(this);
        }
    }
}
