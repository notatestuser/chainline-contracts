using System;
using System.Numerics;
using System.ComponentModel;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;

//                  __/___
//            _____/______|
//    _______/_____\_______\_____
//    \              < < <       |
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//     C  H  A  I  N    L  I  N  E

namespace HubContract {
   public class Settings {
      public static readonly bool DEBUG = true;
   }

   public class Constants {
      // GAS asset ID
      // 602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7
      public static readonly byte[] GasAssetId = {
         96, 44, 121, 113, 139, 22, 228, 66, 222, 88, 119, 142, 20, 141,
         11, 16, 132, 227, 178, 223, 253, 93, 230, 183, 177, 108, 238, 121,
         105, 40, 45, 231
      };
   }

   public class Events {
      [DisplayName("Exception")]
      private static event Action<string> EvException;

      [DisplayName("TransactionOut")]
      private static event Action<BigInteger, bool> EvTransactionOut;

      // indirection required because we can't call these events outside the class easily
      public static void RaiseException(string message) {
         if (Settings.DEBUG)
            EvException(message);
      }

      public static void RaiseTransactionOut(BigInteger amount, bool success) {
         if (Settings.DEBUG)
            EvTransactionOut(amount, success);
      }
   }

   public struct Reservation {
      public uint ExpiryTime;
      public ulong Value;
   }

   public class WalletOperations {
      public static bool ValidateWallet(byte[] scriptHash, byte[] pubKey, bool doThrow = false) {
         // todo: hash the script with the given public key
         // it's cheaper to hash the script ourselves and compare it with scriptHash than to do the below
         //Blockchain.GetContract(scriptHash).Script
         throw new Exception("Not implemented yet.");
      }

      public static BigInteger GetBalance(byte[] scriptHash, byte[] pubKey) {
         ValidateWallet(scriptHash, pubKey, true);
         return Blockchain.GetAccount(scriptHash).GetBalance(Constants.GasAssetId);
      }

      public static BigInteger GetReserved(byte[] scriptHash, byte[] pubKey) {
         ValidateWallet(scriptHash, pubKey, true);
         return Storage.Get(Storage.CurrentContext, scriptHash).AsBigInteger();
      }

      public static bool CanTransferOut(byte[] scriptHash, byte[] pubKey, BigInteger amount) {
         if (! ValidateWallet(scriptHash, pubKey)) return false;
         if (amount <= 2 * 100000000) {
            Events.RaiseTransactionOut(amount, true);
            return true;
         }
         Events.RaiseTransactionOut(amount, false);
         return false;
      }
   }

   public class Contract : SmartContract {
      public static object Main(string operation, params object[] args) {
         //if (! Runtime.CheckWitness(originator)) return false;
         
         // -= Wallets =-

         byte[] callingScriptHash = ExecutionEngine.CallingScriptHash;
         switch (operation) {
            case "validate": 
               return WalletOperations.ValidateWallet(callingScriptHash, (byte[])args[0]);
            case "getbalance": 
               return WalletOperations.GetBalance(callingScriptHash, (byte[])args[0]);
            case "getreserved": 
               return WalletOperations.GetReserved(callingScriptHash, (byte[])args[0]);
            case "requesttx": 
               return WalletOperations.CanTransferOut(callingScriptHash, (byte[])args[0], (BigInteger)args[1]);
         }

         // -= Unsupported Operation :( =-

         Events.RaiseException("Unsupported Operation");

         return false;
      }
   }
}
