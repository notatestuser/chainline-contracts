using System;
using System.Diagnostics;
using System.IO;
using Neo;
using Neo.Core;
using Neo.Cryptography.ECC;
using Neo.Implementations.Blockchains.LevelDB;
using Neo.IO.Caching;
using Neo.SmartContract;
using Neo.VM;

namespace UtilsTests.Utilities {
   class ConvertTask {
      public static bool Execute(string dllPath) {
         string dllName = Path.GetFileName(dllPath);

         ProcessStartInfo pinfo = new ProcessStartInfo {
            FileName = "cmd.exe",
            WorkingDirectory = Path.GetDirectoryName(dllName),
            UseShellExecute = false,
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8
         };

         Process p = Process.Start(pinfo);
         p.StandardInput.AutoFlush = true;
         p.StandardInput.WriteLine("neon " + dllName);
         p.StandardInput.WriteLine("exit");

         p.WaitForExit();
         
         if (p.ExitCode == 0) {
            return true;
         }
         return false;
      }
   }

   class ExecutionHelper {

#if DEBUG 
      private const string Directory = "Debug";
#else
      private const string Directory = "Release";
#endif

      private const string BasePath = "../../../";
      
      public static byte[] Compile(string projectName)
      {
         string dllPath = Path.Combine(BasePath, projectName, $@"bin\{Directory}", $"{projectName}.dll");
         string workingDir = Path.GetDirectoryName(dllPath);
         string fullPath = Path.GetFullPath(dllPath);

         Console.WriteLine("Compiling DLL at: " + fullPath);
         
         if (! ConvertTask.Execute(fullPath)) {
            throw new Exception("Compile task failed!");
         }

         string avmPath = Path.GetFullPath(Path.Combine(workingDir, $"{projectName}.avm"));
         Console.WriteLine("Using AVM at: " + avmPath);
         
         return File.ReadAllBytes(avmPath);
      }

      public static ExecutionEngine GetExecutionEngine() {
         InvocationTransaction tx = new InvocationTransaction();
         tx.Version = 1;
         if (tx.Attributes == null) tx.Attributes = new TransactionAttribute[0];
         if (tx.Inputs == null) tx.Inputs = new CoinReference[0];
         if (tx.Outputs == null) tx.Outputs = new TransactionOutput[0];
         if (tx.Scripts == null) tx.Scripts = new Witness[0];
         Blockchain.RegisterBlockchain(new LevelDBBlockchain("TestDB"));
         LevelDBBlockchain blockchain = (LevelDBBlockchain)Blockchain.Default;
         DataCache<UInt160, AccountState> accounts = blockchain.GetTable<UInt160, AccountState>();
         DataCache<ECPoint, ValidatorState> validators = blockchain.GetTable<ECPoint, ValidatorState>();
         DataCache<UInt256, AssetState> assets = blockchain.GetTable<UInt256, AssetState>();
         DataCache<UInt160, ContractState> contracts = blockchain.GetTable<UInt160, ContractState>();
         DataCache<StorageKey, StorageItem> storages = blockchain.GetTable<StorageKey, StorageItem>();
         //CachedScriptTable script_table = new CachedScriptTable(contracts);
         StateMachine service = new StateMachine(accounts, validators, assets, contracts, storages);
         return new ApplicationEngine(TriggerType.Application, tx, null, service, Fixed8.Zero, true);
      }
   }
}
