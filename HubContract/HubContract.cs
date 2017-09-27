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

      // VM WORKAROUND
      // Becuase the SETITEM opcode doesn't work with byte arrays, we can't make them on the fly.
      public static readonly byte[] EmptyBytes = new byte[] { };
      public static readonly byte[][] Bytes = new byte[][] {
         new byte[] { 0 }, new byte[] { 1 }, new byte[] { 2 }, new byte[] { 3 }, new byte[] { 4 }, new byte[] { 5 }, new byte[] { 6 }, new byte[] { 7 }, new byte[] { 8 }, new byte[] { 9 }, new byte[] { 10 }, new byte[] { 11 }, new byte[] { 12 }, new byte[] { 13 }, new byte[] { 14 }, new byte[] { 15 }, new byte[] { 16 }, new byte[] { 17 }, new byte[] { 18 }, new byte[] { 19 }, new byte[] { 20 }, new byte[] { 21 }, new byte[] { 22 }, new byte[] { 23 }, new byte[] { 24 }, new byte[] { 25 }, new byte[] { 26 }, new byte[] { 27 }, new byte[] { 28 }, new byte[] { 29 }, new byte[] { 30 }, new byte[] { 31 }, new byte[] { 32 }, new byte[] { 33 }, new byte[] { 34 }, new byte[] { 35 }, new byte[] { 36 }, new byte[] { 37 }, new byte[] { 38 }, new byte[] { 39 }, new byte[] { 40 }, new byte[] { 41 }, new byte[] { 42 }, new byte[] { 43 }, new byte[] { 44 }, new byte[] { 45 }, new byte[] { 46 }, new byte[] { 47 }, new byte[] { 48 }, new byte[] { 49 }, new byte[] { 50 }, new byte[] { 51 }, new byte[] { 52 }, new byte[] { 53 }, new byte[] { 54 }, new byte[] { 55 }, new byte[] { 56 }, new byte[] { 57 }, new byte[] { 58 }, new byte[] { 59 }, new byte[] { 60 }, new byte[] { 61 }, new byte[] { 62 }, new byte[] { 63 }, new byte[] { 64 }, new byte[] { 65 }, new byte[] { 66 }, new byte[] { 67 }, new byte[] { 68 }, new byte[] { 69 }, new byte[] { 70 }, new byte[] { 71 }, new byte[] { 72 }, new byte[] { 73 }, new byte[] { 74 }, new byte[] { 75 }, new byte[] { 76 }, new byte[] { 77 }, new byte[] { 78 }, new byte[] { 79 }, new byte[] { 80 }, new byte[] { 81 }, new byte[] { 82 }, new byte[] { 83 }, new byte[] { 84 }, new byte[] { 85 }, new byte[] { 86 }, new byte[] { 87 }, new byte[] { 88 }, new byte[] { 89 }, new byte[] { 90 }, new byte[] { 91 }, new byte[] { 92 }, new byte[] { 93 }, new byte[] { 94 }, new byte[] { 95 }, new byte[] { 96 }, new byte[] { 97 }, new byte[] { 98 }, new byte[] { 99 }, new byte[] { 100 }, new byte[] { 101 }, new byte[] { 102 }, new byte[] { 103 }, new byte[] { 104 }, new byte[] { 105 }, new byte[] { 106 }, new byte[] { 107 }, new byte[] { 108 }, new byte[] { 109 }, new byte[] { 110 }, new byte[] { 111 }, new byte[] { 112 }, new byte[] { 113 }, new byte[] { 114 }, new byte[] { 115 }, new byte[] { 116 }, new byte[] { 117 }, new byte[] { 118 }, new byte[] { 119 }, new byte[] { 120 }, new byte[] { 121 }, new byte[] { 122 }, new byte[] { 123 }, new byte[] { 124 }, new byte[] { 125 }, new byte[] { 126 }, new byte[] { 127 }, new byte[] { 128 }, new byte[] { 129 }, new byte[] { 130 }, new byte[] { 131 }, new byte[] { 132 }, new byte[] { 133 }, new byte[] { 134 }, new byte[] { 135 }, new byte[] { 136 }, new byte[] { 137 }, new byte[] { 138 }, new byte[] { 139 }, new byte[] { 140 }, new byte[] { 141 }, new byte[] { 142 }, new byte[] { 143 }, new byte[] { 144 }, new byte[] { 145 }, new byte[] { 146 }, new byte[] { 147 }, new byte[] { 148 }, new byte[] { 149 }, new byte[] { 150 }, new byte[] { 151 }, new byte[] { 152 }, new byte[] { 153 }, new byte[] { 154 }, new byte[] { 155 }, new byte[] { 156 }, new byte[] { 157 }, new byte[] { 158 }, new byte[] { 159 }, new byte[] { 160 }, new byte[] { 161 }, new byte[] { 162 }, new byte[] { 163 }, new byte[] { 164 }, new byte[] { 165 }, new byte[] { 166 }, new byte[] { 167 }, new byte[] { 168 }, new byte[] { 169 }, new byte[] { 170 }, new byte[] { 171 }, new byte[] { 172 }, new byte[] { 173 }, new byte[] { 174 }, new byte[] { 175 }, new byte[] { 176 }, new byte[] { 177 }, new byte[] { 178 }, new byte[] { 179 }, new byte[] { 180 }, new byte[] { 181 }, new byte[] { 182 }, new byte[] { 183 }, new byte[] { 184 }, new byte[] { 185 }, new byte[] { 186 }, new byte[] { 187 }, new byte[] { 188 }, new byte[] { 189 }, new byte[] { 190 }, new byte[] { 191 }, new byte[] { 192 }, new byte[] { 193 }, new byte[] { 194 }, new byte[] { 195 }, new byte[] { 196 }, new byte[] { 197 }, new byte[] { 198 }, new byte[] { 199 }, new byte[] { 200 }, new byte[] { 201 }, new byte[] { 202 }, new byte[] { 203 }, new byte[] { 204 }, new byte[] { 205 }, new byte[] { 206 }, new byte[] { 207 }, new byte[] { 208 }, new byte[] { 209 }, new byte[] { 210 }, new byte[] { 211 }, new byte[] { 212 }, new byte[] { 213 }, new byte[] { 214 }, new byte[] { 215 }, new byte[] { 216 }, new byte[] { 217 }, new byte[] { 218 }, new byte[] { 219 }, new byte[] { 220 }, new byte[] { 221 }, new byte[] { 222 }, new byte[] { 223 }, new byte[] { 224 }, new byte[] { 225 }, new byte[] { 226 }, new byte[] { 227 }, new byte[] { 228 }, new byte[] { 229 }, new byte[] { 230 }, new byte[] { 231 }, new byte[] { 232 }, new byte[] { 233 }, new byte[] { 234 }, new byte[] { 235 }, new byte[] { 236 }, new byte[] { 237 }, new byte[] { 238 }, new byte[] { 239 }, new byte[] { 240 }, new byte[] { 241 }, new byte[] { 242 }, new byte[] { 243 }, new byte[] { 244 }, new byte[] { 245 }, new byte[] { 246 }, new byte[] { 247 }, new byte[] { 248 }, new byte[] { 249 }, new byte[] { 250 }, new byte[] { 251 }, new byte[] { 252 }, new byte[] { 253 }, new byte[] { 254 }, new byte[] { 255 }
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
         byte[] reversedScriptHash = Utils.ArrayReverse(scriptHash);
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
                              Utils.ArrayReverse(scriptHash));
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
      public static object Main(string operation, params object[] args) {
         //if (! Runtime.CheckWitness(originator)) return false;
         
         //Runtime.Notify("BigInteger Size", Constants.BigIntSize());

         // -= Test Entry Points =-
         switch (operation) {
            case "test_arrayreverse":
               //return Utils.ArrayReverse((byte[])args[0]);
               return new byte[] { 5, 4, 3, 2, 1 };
         }
         
         // -= Wallets =-
         switch (operation) {
            case "validate": 
               return WalletOperations.ValidateWallet((byte[])args[0], (byte[])args[1]);
            case "getbalance": 
               return WalletOperations.GetBalance((byte[])args[0]);
            case "getreserved": 
               return WalletOperations.GetReserved((byte[])args[0], (byte[])args[1]);
            case "requesttx": 
               return WalletOperations.CanTransferOut((byte[])args[0], 
                        (byte[])args[1], (byte[])args[2], (BigInteger)args[3]);
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

      public static byte[] ArrayReverse(byte[] input) {
         //byte[] reversed = input.Take(input.Length);
         byte[] reversed = Constants.EmptyBytes;
         int index;
         for (int i = 0; i < input.Length; i++) {
            //reversed[i] = input[input.Length - 1 - i];  <- puts the VM in a FAULT state
            index = input.Length - 1 - i;
            reversed = reversed.Concat(Constants.Bytes[input[index]]);
         }
         return reversed;
      }
   }
}
