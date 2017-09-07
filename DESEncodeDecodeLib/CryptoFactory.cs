using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DESEncodeDecodeLib
{
    public class CryptoFactory
    {
        public DESEncryptor GetDESEncryptor(string input) => default(DESEncryptor);
    }

    public class DESEncryptor { }
}