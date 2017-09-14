# DES Encryption Utility

Implemented in C# (.NET Framework 4.6.1) adhering to the Data Encryption Standard algorithm.

### Documentation

Type DEScli for help:

`cmd> DEScli help`

**The output:**

```
DES CLI Utility 1.1.2.33456
Copyright (C) 2017

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