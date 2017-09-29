using Xunit;
using UtilsTests.Utilities;
using Neo.VM;
using Neo.Cryptography;
using System;

namespace UtilsTests {
   public class UnitTest1 {
      private static readonly byte[] SCRIPT_HASH = new byte[] {
         48, 162, 176, 65, 57, 215, 20, 86, 78, 185,
         86, 137, 100, 152, 97, 108, 248, 172, 200, 219 };

      [Fact]
      public void TestMethod1() {
         byte[] program = ExecutionHelper.Compile("HubContract");
         var engine = new ExecutionEngine(null, Crypto.Default);
         engine.LoadScript(program);

         using (ScriptBuilder sb = new ScriptBuilder())
         {
            sb.EmitPush(0);  // arg3
            sb.EmitPush(new byte[] { });  // arg2
            sb.EmitPush(new byte[] { });  // arg1
            sb.EmitPush(SCRIPT_HASH);  // arg0
            sb.EmitPush("test_hash160reverse");  // operation

            engine.LoadScript(sb.ToArray());
         }

         engine.Execute();
         Assert.False(engine.State == VMState.FAULT);

         byte[] result = engine.EvaluationStack.Peek().GetByteArray();
         byte[] reversedScriptHash = (byte[])SCRIPT_HASH.Clone();
         Array.Reverse(reversedScriptHash);
         Assert.Equal(reversedScriptHash, result);
      }
   }
}
