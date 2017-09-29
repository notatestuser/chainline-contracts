using Xunit;
using UtilsTests.Utilities;
using Neo.VM;
using Neo.Cryptography;
using System;

namespace UnitTests {
   public class SanityTests {
      [Fact]
      public void TestByteArrayEquality() {
         byte[] program = ExecutionHelper.Compile("HubContract");
         var engine = new ExecutionEngine(null, Crypto.Default);
         engine.LoadScript(program);

         using (ScriptBuilder sb = new ScriptBuilder())
         {
            sb.EmitPush(0);  // arg3
            sb.EmitPush(new byte[] { });  // arg2
            sb.EmitPush(new byte[] { 1, 2, 3, 4, 5 });  // arg1
            sb.EmitPush(new byte[] { 1, 2, 3, 4, 5 });  // arg0
            sb.EmitPush("test_sanity_bytearrayeq");  // operation

            engine.LoadScript(sb.ToArray());
         }

         engine.Execute();
         Assert.False(engine.State == VMState.FAULT);

         bool result = engine.EvaluationStack.Peek().GetBoolean();
         Assert.True(result);
      }
   }
}
