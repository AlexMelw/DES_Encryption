namespace DESEncodeDecodeLib.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    static class BinaryNumbersMapper
    {
        public static Dictionary<Bit[], int> TwoBitsDecimalMapping { get; } =
            new Dictionary<Bit[], int>(new BitArrayEqualityComparer())
            {
                [new Bit[] { 0, 0 }] = 0,
                [new Bit[] { 0, 1 }] = 1,
                [new Bit[] { 1, 0 }] = 2,
                [new Bit[] { 1, 1 }] = 3
            };

        public static Dictionary<Bit[], int> FourBitsDecimalMapping { get; } =
            new Dictionary<Bit[], int>(new BitArrayEqualityComparer())
        {
            [new Bit[] { 0, 0, 0, 0 }] = 0,
            [new Bit[] { 0, 0, 0, 1 }] = 1,
            [new Bit[] { 0, 0, 1, 0 }] = 2,
            [new Bit[] { 0, 0, 1, 1 }] = 3,
            [new Bit[] { 0, 1, 0, 0 }] = 4,
            [new Bit[] { 0, 1, 0, 1 }] = 5,
            [new Bit[] { 0, 1, 1, 0 }] = 6,
            [new Bit[] { 0, 1, 1, 1 }] = 7,
            [new Bit[] { 1, 0, 0, 0 }] = 8,
            [new Bit[] { 1, 0, 0, 1 }] = 9,
            [new Bit[] { 1, 0, 1, 0 }] = 10,
            [new Bit[] { 1, 0, 1, 1 }] = 11,
            [new Bit[] { 1, 1, 0, 0 }] = 12,
            [new Bit[] { 1, 1, 0, 1 }] = 13,
            [new Bit[] { 1, 1, 1, 0 }] = 14,
            [new Bit[] { 1, 1, 1, 1 }] = 15,
        };
    }

    class BitArrayEqualityComparer : IEqualityComparer<Bit[]>
    {
        public bool Equals(Bit[] x, Bit[] y)
        {
            if (x == null || y == null)
            {
                return false;
            }

            bool notEqual = !x.Where((t, i) => t != y[i]).Any();

            return notEqual;
        }

        public int GetHashCode(Bit[] obj)
        {
            int hashCode = 0;

            for (var index = 0; index < obj.Length; index++)
            {
                Bit bit = obj[index];

                if (bit.IsSet)
                {
                    hashCode |= 1 << index;
                }
            }

            return hashCode;
        }
    }
}