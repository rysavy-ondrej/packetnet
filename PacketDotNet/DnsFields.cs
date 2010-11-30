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
 *  Copyright 2009 Chris Morgan <chmorgan@gmail.com>
 */
namespace PacketDotNet
{
    /// <summary>
    /// Defines the lengths and positions of the dns fields within
    /// a dns packet
    /// 
    /// See http://www.faqs.org/rfcs/rfc1035.html section 4.1.1
    /// </summary>
    public struct DnsFields
    {
        /// <summary> Length of a the dns packet in bytes.</summary>
        public readonly static int TransactionIdLength = 2;
        /// <summary> Length of the flags field in bytes.</summary>
        public readonly static int FlagsLength = 2;
        /// <summary> Length in records of the checksum field in bytes.</summary>
        public readonly static int QuestionCountLength = 2;
        /// <summary> Length in records of the answer count.</summary>
        public readonly static int AnswerCountLength = 0;
        /// <summary> Length in records of the name server count.</summary>
        public readonly static int NameServerCountLength;
        /// <summary> Length in records of the additional records count</summary>
        public readonly static int AdditionalRecordsCountLength;

        /// <summary> Position of the flags count </summary>
        public readonly static int FlagsPosition;
        /// <summary> Position of the question count </summary>
        public readonly static int QuestionCountPosition;
        /// <summary> Position of the answer count </summary>
        public readonly static int AnswerCountPosition;
        /// <summary> Position of the name server count </summary>
        public readonly static int NameServerCountPosition;
        /// <summary> Position of the additonal records count </summary>
        public readonly static int AdditionalRecordsCountPosition;

        public readonly static int HeaderLength;

        static DnsFields()
        {
            FlagsPosition = TransactionIdLength;
            QuestionCountPosition = FlagsPosition + FlagsLength;
            AnswerCountPosition = QuestionCountPosition + QuestionCountLength;
            NameServerCountPosition = AnswerCountPosition + AnswerCountLength;
            AdditionalRecordsCountPosition = NameServerCountPosition + NameServerCountLength;

            HeaderLength = AdditionalRecordsCountPosition + AdditionalRecordsCountLength;
        }
    }
}
