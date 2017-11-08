namespace DESEncodeDecodeLib.AlgorithmTables
{
    using System.Collections.Generic;

    public static class ShiftsIterationsRelation
    {
        public static Dictionary<int, int> ShiftsNumberDictionary { get; } = new Dictionary<int, int>
        {
            [1] = 1,
            [2] = 1,
            [3] = 2,
            [4] = 2,
            [5] = 2,
            [6] = 2,
            [7] = 2,
            [8] = 2,
            [9] = 1,
            [10] = 2,
            [11] = 2,
            [12] = 2,
            [13] = 2,
            [14] = 2,
            [15] = 2,
            [16] = 1
        };
    }
}