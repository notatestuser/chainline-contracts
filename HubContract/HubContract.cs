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
   public class Settings {
      public static readonly bool DEBUG = true;
   }

   public class Constants {
      public static int BigIntSize() => ((BigInteger)100000000).ToByteArray().Length;

      // GAS asset ID
      // 602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7
      public static readonly byte[] GasAssetId = {
         96, 44, 121, 113, 139, 22, 228, 66, 222, 88, 119, 142, 20, 141,
         11, 16, 132, 227, 178, 223, 253, 93, 230, 183, 177, 108, 238, 121,
         105, 40, 45, 231
      };

      // Wallet contract bytecode for user wallet validation
      // Revision: 7cd15d3 (tag: wallet-v0.2)

      // The script before the public key
      public static readonly byte[] WalletScriptP1 = {
         84, 197, 107, 108, 118, 107, 0, 82, 122, 196, 97, 104, 22, 78, 101, 111,
         46, 82, 117, 110, 116, 105, 109, 101, 46, 71, 101, 116, 84, 114, 105, 103,
         103, 101, 114, 100, 8, 0, 0, 97, 108, 117, 102, 33
         // ... public key goes here ...
      };

      // Between the public key and script hash
      public static readonly byte[] WalletScriptP2 = {
         // ... public key goes here ...
         108, 118, 107, 81, 82, 122, 196, 108, 118, 107, 0, 195, 108, 118, 107, 81,
         195, 172, 99, 8, 0, 0, 97, 108, 117, 102, 108, 118, 107, 0, 195, 97, 101, 207,
         0, 108, 118, 107, 82, 82, 122, 196, 97, 104, 41, 83, 121, 115, 116, 101, 109,
         46, 69, 120, 101, 99, 117, 116, 105, 111, 110, 69, 110, 103, 105, 110, 101,
         46, 71, 101, 116, 83, 99, 114, 105, 112, 116, 67, 111, 110, 116, 97, 105, 110,
         101, 114, 97, 104, 29, 78, 101, 111, 46, 84, 114, 97, 110, 115, 97, 99, 116,
         105, 111, 110, 46, 71, 101, 116, 82, 101, 102, 101, 114, 101, 110, 99, 101,
         115, 0, 195, 97, 104, 21, 78, 101, 111, 46, 79, 117, 116, 112, 117, 116, 46,
         71, 101, 116, 65, 115, 115, 101, 116, 73, 100, 108, 118, 107, 83, 82, 122, 196,
         9, 114, 101, 113, 117, 101, 115, 116, 116, 120, 84, 197, 118, 0, 108, 118, 107,
         0, 195, 196, 118, 81, 108, 118, 107, 81, 195, 196, 118, 82, 108, 118, 107, 83,
         195, 196, 118, 83, 108, 118, 107, 82, 195, 196, 97, 124, 103
         // ... hub script hash goes here ...
      };

      // Everything after the script hash
      public static readonly byte[] WalletScriptP3 = {
         // ... hub script hash goes here ...
         97, 108, 117, 102, 82, 197, 107, 108, 118, 107, 0, 82, 122, 196, 108, 118, 107,
         81, 82, 122, 196, 86, 197, 107, 108, 118, 107, 0, 82, 122, 196, 97, 104, 45, 83,
         121, 115, 116, 101, 109, 46, 69, 120, 101, 99, 117, 116, 105, 111, 110, 69, 110,
         103, 105, 110, 101, 46, 71, 101, 116, 69, 120, 101, 99, 117, 116, 105, 110, 103,
         83, 99, 114, 105, 112, 116, 72, 97, 115, 104, 108, 118, 107, 81, 82, 122, 196,
         97, 104, 41, 83, 121, 115, 116, 101, 109, 46, 69, 120, 101, 99, 117, 116, 105,
         111, 110, 69, 110, 103, 105, 110, 101, 46, 71, 101, 116, 83, 99, 114, 105, 112,
         116, 67, 111, 110, 116, 97, 105, 110, 101, 114, 0, 108, 118, 107, 82, 82, 122,
         196, 97, 104, 26, 78, 101, 111, 46, 84, 114, 97, 110, 115, 97, 99, 116, 105, 111,
         110, 46, 71, 101, 116, 79, 117, 116, 112, 117, 116, 115, 108, 118, 107, 83, 82,
         122, 196, 0, 108, 118, 107, 84, 82, 122, 196, 98, 120, 0, 108, 118, 107, 83, 195,
         108, 118, 107, 84, 195, 195, 108, 118, 107, 85, 82, 122, 196, 108, 118, 107, 85,
         195, 97, 104, 24, 78, 101, 111, 46, 79, 117, 116, 112, 117, 116, 46, 71, 101, 116,
         83, 99, 114, 105, 112, 116, 72, 97, 115, 104, 108, 118, 107, 81, 195, 97, 124, 101,
         84, 0, 99, 43, 0, 108, 118, 107, 82, 195, 108, 118, 107, 85, 195, 97, 104, 19, 78,
         101, 111, 46, 79, 117, 116, 112, 117, 116, 46, 71, 101, 116, 86, 97, 108, 117, 101,
         147, 108, 118, 107, 82, 82, 122, 196, 108, 118, 107, 84, 195, 81, 147, 108, 118,
         107, 84, 82, 122, 196, 108, 118, 107, 84, 195, 108, 118, 107, 83, 195, 192, 159, 99,
         127, 255, 108, 118, 107, 82, 195, 97, 108, 117, 102, 83, 197, 107, 108, 118, 107,
         0, 82, 122, 196, 108, 118, 107, 81, 82, 122, 196, 108, 118, 107, 0, 195, 192, 108,
         118, 107, 81, 195, 192, 156, 99, 8, 0, 0, 97, 108, 117, 102, 0, 108, 118, 107, 82,
         82, 122, 196, 98, 50, 0, 108, 118, 107, 0, 195, 108, 118, 107, 82, 195, 81, 127, 
         108, 118, 107, 81, 195, 108, 118, 107, 82, 195, 81, 127, 156, 99, 8, 0, 0, 97, 108,
         117, 102, 108, 118, 107, 82, 195, 81, 147, 108, 118, 107, 82, 82, 122, 196, 108, 118,
         107, 82, 195, 108, 118, 107, 0, 195, 192, 159, 99, 197, 255, 81, 97, 108, 117, 102
      };
   }

   public class Events {
      [DisplayName("Exception")]
      private static event Action<string> EvException;

      [DisplayName("ValidateWalletOK")]
      private static event Action<byte[], byte[]> EvValidateWalletOK;

      [DisplayName("TransactionOut")]
      private static event Action<BigInteger, bool> EvTransactionOut;
      
      public static void RaiseException(string message) {
         if (Settings.DEBUG)
            EvException(message);
      }
      
      public static void RaiseValidateWalletOK(byte[] scriptHash, byte[] pubKey) {
         if (Settings.DEBUG)
            EvValidateWalletOK(scriptHash, pubKey);
      }

      public static void RaiseTransactionOut(BigInteger amount, bool success) {
         if (Settings.DEBUG)
            EvTransactionOut(amount, success);
      }
   }

   public struct ReservedFunds {
      public BigInteger ExpiryTime;
      public BigInteger Value;
      public byte[] Destination;
      public bool Unlocked;  // simple multi-sig.
   }

   public class WalletOperations {
      [OpCode(OpCode.HASH160)]
      protected static extern byte[] Hash160(byte[] data);
      
      public static bool ValidateWallet(byte[] scriptHash, byte[] pubKey, bool doThrow = false) {
         // This is cheaper than the Blockchain.GetContract method of getting the entire script
         byte[] reversedScriptHash = Utils.ReverseHash160(scriptHash);
         byte[] expectedScript = 
               Constants.WalletScriptP1
                  .Concat(pubKey)
                  .Concat(Constants.WalletScriptP2)
                  .Concat(reversedScriptHash)
                  .Concat(Constants.WalletScriptP3);
         byte[] expectedHash = Hash160(expectedScript);
         if (! Utils.ArraysEqual(expectedHash, scriptHash)) {
            Events.RaiseException("InvalidWallet");
            if (doThrow) throw new Exception("This is not a valid Chain Line user wallet.");
            return false;
         }
         Events.RaiseValidateWalletOK(scriptHash, pubKey);
         return true;
      }

      public static BigInteger GetBalance(byte[] scriptHash) {
         byte[] gasAssetId = Constants.GasAssetId;
         Account account = Blockchain.GetAccount(
                              Utils.ReverseHash160(scriptHash));
         return account.GetBalance(gasAssetId);
      }

      public static BigInteger GetReserved(byte[] scriptHash, byte[] pubKey) {
         ValidateWallet(scriptHash, pubKey, true);
         return Storage.Get(Storage.CurrentContext, scriptHash).AsBigInteger();
      }

      public static bool CanTransferOut(byte[] sig, byte[] pubKey, byte[] assetId, BigInteger amount) {
         byte[] scriptHash = ExecutionEngine.CallingScriptHash;
         if (! ValidateWallet(scriptHash, pubKey)) return false;
         if (amount <= 2 * 100000000) {  // TODO: just a test!
            Events.RaiseTransactionOut(amount, true);
            return true;
         }
         Events.RaiseException("X");
         return false;
      }
   }

   public class Contract : SmartContract {
      //public static object Main(string operation, params object[] args) {
      public static object Main(string operation, byte[] arg0, byte[] arg1, byte[] arg2, BigInteger arg3) {
         //if (! Runtime.CheckWitness(originator)) return false;

         //Runtime.Notify("BigInteger Size", Constants.BigIntSize());

         // -= Test Entry Points =-
         switch (operation) {
            case "test_hash160reverse":
               return Utils.ReverseHash160(arg0);
         }
         
         // -= Wallets =-
         switch (operation) {
            case "validate":
               return WalletOperations.ValidateWallet(arg0, arg1);
            case "getbalance":
               return WalletOperations.GetBalance(arg0);
            case "getreserved":
               return WalletOperations.GetReserved(arg0, arg1);
            case "requesttx":
               return WalletOperations.CanTransferOut(arg0, arg1, arg2, arg3);
         }

         // -= Unsupported Operation! =-
         Events.RaiseException("Unsupported Operation");

         return false;
      }
   }
   
   public class Utils {
      public static bool ArraysEqual(byte[] a1, byte[] a2) {
         if (a1.Length != a2.Length)
            return false;
         for (int i = 0; i < a1.Length; i++)
            if (a1[i] != a2[i])
               return false;
         return true;
      }

      public static byte[] ReverseHash160(byte[] hash) {
         string input = hash.AsString();
         string reversed = "";
         // This triggers an InvalidOperationException at runtime:
         // for (var i = 0; i < input.Length; i++)
         //    reversed = input[i] + reversed;
         reversed = input[0] + reversed;
         reversed = input[1] + reversed;
         reversed = input[2] + reversed;
         reversed = input[3] + reversed;
         reversed = input[4] + reversed;
         reversed = input[5] + reversed;
         reversed = input[6] + reversed;
         reversed = input[7] + reversed;
         reversed = input[8] + reversed;
         reversed = input[9] + reversed;
         reversed = input[10] + reversed;
         reversed = input[11] + reversed;
         reversed = input[12] + reversed;
         reversed = input[13] + reversed;
         reversed = input[14] + reversed;
         reversed = input[15] + reversed;
         reversed = input[16] + reversed;
         reversed = input[17] + reversed;
         reversed = input[18] + reversed;
         reversed = input[19] + reversed;
         return reversed.AsByteArray();
      }
   }
}
