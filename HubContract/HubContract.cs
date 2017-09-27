using System;
using System.Numerics;
using System.ComponentModel;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;
using Neo.VM;

//                  __/___
//            _____/______|
//    _______/_____\_______\_____
//    \              < < <       |
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//     C  H  A  I  N    L  I  N  E

namespace HubContract {

   public class Contract : SmartContract {
      public static object Main(string operation, params object[] args) {
         //if (! Runtime.CheckWitness(originator)) return false;
         
         //Runtime.Notify("BigInteger Size", Constants.BigIntSize());

         // -= Test Entry Points =-
         return ReverseByteArray(operation.AsByteArray());
      }

      public static byte[] ReverseByteArray(byte[] input) {
         return input.Concat(input);
      }
   }
}
