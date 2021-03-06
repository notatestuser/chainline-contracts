﻿using System.Numerics;
using Neo.SmartContract.Framework;
using Neo.SmartContract.Framework.Services.Neo;
using Neo.SmartContract.Framework.Services.System;

//                  __/___
//            _____/______|
//    _______/_____\_______\_____
//    \              < < <       |
// ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
//     C  H  A  I  N    L  I  N  E

// This contract verifies transactions out of a single Chain Line user's wallet

namespace WalletContract {
   public class Constants {
      // The account owner's public key
      // Included in at wallet creation time
      // Converter: https://conv.darkbyte.ru
      // neowallet_contracts13.db3
      public static readonly byte[] OwnerPubKey = {
         3, 114, 247, 98, 137, 198, 155, 181, 138, 142, 92, 125, 43, 79, 
         21, 38, 234, 139, 38, 192, 131, 178, 169, 88, 194, 30, 188, 3, 25, 
         110, 0, 188, 192
      };

      // GAS asset ID
      // 602c79718b16e442de58778e148d0b1084e3b2dffd5de6b7b16cee7969282de7
      public static readonly byte[] GasAssetId = {
         96, 44, 121, 113, 139, 22, 228, 66, 222, 88, 119, 142, 20, 141,
         11, 16, 132, 227, 178, 223, 253, 93, 230, 183, 177, 108, 238, 121,
         105, 40, 45, 231
      };
   }

   public class Externals {
      // Represents the instance of Chain Line's HubContract on the network
      // Current version: 0.8 (30a2b04139d714564eb956896498616cf8acc8db)
      [Appcall(new byte[] { 48, 162, 176, 65, 57, 215, 20, 86, 78, 185, 86,
         137, 100, 152, 97, 108, 248, 172, 200, 219 })]  // reversed by the compiler
      public static extern bool HubContract(string operation, params object[] args);
   }

   public class Contract : SmartContract {
      public static object Main(byte[] signature) {
         // Ensure that we're processing a withdrawal
         if (Runtime.Trigger != TriggerType.Verification)
            return false;

         // Verify the tx against the wallet owner's pubkey
         byte[] pubKey = Constants.OwnerPubKey;  // makes it appear once in bytecode
         if (! VerifySignature(signature, pubKey))
            return false;

         // Get the tx amount (count the gas in outputs)
         BigInteger txValue = GetTransactionValue(signature);

         // Get the withdraw transaction that triggered the contract verification
         Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
         TransactionOutput reference = tx.GetReferences()[0];
         byte[] assetId = reference.AssetId;

         return Externals.HubContract("requesttx", signature, pubKey, assetId, txValue);
      }

      private static BigInteger GetTransactionValue(byte[] signature) {
         byte[] accScriptHash = ExecutionEngine.ExecutingScriptHash;
         Transaction tx = (Transaction)ExecutionEngine.ScriptContainer;
         
         // We only care about GAS transactions
         // TODO: Check whether asset is GAS

         // Count up the outgoing amount
         BigInteger outgoingValue = 0;
         foreach (TransactionOutput output in tx.GetOutputs()) {
            // Only count outgoing outputs
            // For some reason `a != b` wasn't working here
            if (Utils.ArraysEqual(output.ScriptHash, accScriptHash))
               continue;
            outgoingValue += output.Value;
         }

         return outgoingValue;
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
   }
}
