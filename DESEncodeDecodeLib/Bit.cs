﻿namespace DESEncodeDecodeLib
{
    /// <summary>
    ///     <para>This custom data type simulates manipulating elementary binary data (bit).</para>
    ///     <example>
    ///         Bit bit1 = 0;
    ///         Bit bit2 = 1;
    ///         Console.Out.WriteLine(bit1.IsSet ? $"Bit is 1 [{bit1}]" : $"Bit is 0 [{bit1}]"); //Bit is 0 [0]
    ///         Console.Out.WriteLine(bit1 ? $"Bit is 1 [{bit1}]" : $"Bit is 0 [{bit1}]"); //Bit is 0 [0]
    ///         Console.Out.WriteLine(bit2 ? $"Bit is 1 [{bit2}]" : $"Bit is 0 [{bit2}]"); //Bit is 1 [1]
    ///         Console.Out.WriteLine((byte)bit1); //0
    ///         Console.Out.WriteLine((byte)bit2); //1
    ///         Console.Out.WriteLine("bit1 == bit2 is {0}", bit1.Equals(bit2)); //bit1 == bit2 is False
    ///     </example>
    /// </summary>
    public struct Bit
    {
        public bool IsSet { get; }

        public bool Equals(Bit other)
        {
            return IsSet == other.IsSet;
        }

        public override int GetHashCode()
        {
            return IsSet.GetHashCode();
        }

        private Bit(bool flag)
        {
            IsSet = flag;
        }

        // From Bit
        public static implicit operator bool(Bit bit)
        {
            return bit.IsSet;
        }

        public static implicit operator byte(Bit bit)
        {
            return (byte) (bit.IsSet ? 1 : 0);
        }

        // To Bit
        public static implicit operator Bit(bool flag)
        {
            return new Bit(flag);
        }

        public static implicit operator Bit(byte octet)
        {
            return new Bit(octet != 0);
        }

        // Overrides
        public override string ToString()
        {
            return IsSet ? "1" : "0";
        }

        public override bool Equals(object obj)
        {
            return obj is Bit bit
                ? this == bit
                : base.Equals(obj);
        }

        // Operators Overloads
        public static Bit operator &(Bit b1, Bit b2)
        {
            return b1.IsSet && b2.IsSet;
        }

        public static Bit operator |(Bit b1, Bit b2)
        {
            return b1.IsSet | b2.IsSet;
        }

        public static Bit operator ^(Bit b1, Bit b2)
        {
            return b1.IsSet ^ b2.IsSet;
        }

        public static Bit operator ~(Bit b)
        {
            return !b.IsSet;
        }

        public static Bit operator !(Bit b)
        {
            return !b.IsSet;
        }

        // Comparison operators
        public static bool operator ==(Bit b1, Bit b2)
        {
            return b1.IsSet && b2.IsSet || !b1.IsSet && !b2.IsSet;
        }

        public static bool operator !=(Bit b1, Bit b2) => !(b1 == b2);

        public static bool operator ==(Bit bit, byte octet)
        {
            return bit.IsSet && octet == 0 || !bit.IsSet && octet == 1;
        }

        public static bool operator ==(byte octet, Bit bit) => bit == octet;

        public static bool operator !=(byte octet, Bit bit) => NotEqual(bit, octet);

        public static bool operator !=(Bit bit, byte octet) => NotEqual(bit, octet);

        private static bool NotEqual(Bit bit, byte octet) => !(bit == octet);
    }
}