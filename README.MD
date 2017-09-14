# DES Encryption Utility (CLI)

Implemented in C# (.NET Framework 4.6.1) adhering to the Data Encryption Standard algorithm.

### Documentation

To get some help concerning the use of CLI utility, type:

`cmd> DEScli --help` or just `cmd> DEScli help`

**The output:**

```
DES CLI Utility 1.1.3.41644
Copyright (C) 2017

  gsk        Generates a 64-bit Strong Key using RNGCryptoServiceProvider class that resides in
             System.Security.Cryptography namespace. (Recommended)

  enc        Enforces file encryption with the specified key.

  dec        Enforces file decrypt with the specified key.

  help       Display more information on a specific command.

  version    Display version information.
```
  
For more information related to each command verb, type:

`cmd> DEScli help verb`, where verb can be either `enc` or `dec`.

**The output:**

```
DES CLI Utility 1.1.2.33456
Copyright (C) 2017
USAGE:
Encryption:
  DEScli enc --input ToBeEncrypted.ext --key Key64bit.ext --output Encrypted.ext

  -o, --output    Output File Name. This file will contain the result of the decryption operation.

  -i, --input     Required. Source file containing the data to be encrypted with a given key.

  -k, --key       Required. Path to file containing 64-bit key for encryption of data from supplied file.

  --help          Display this help screen.

  --version       Display version information.
```