using System;
using System.Linq;
using Xunit;
using UtilsTests.Utilities;
using Neo.VM;
using Neo.Cryptography;

namespace UtilsTests {
   public class UnitTest1 {
      [Fact]
      public void TestMethod1() {
         byte[] program = ExecutionHelper.Compile("HubContract");

         using (ScriptBuilder sb = new ScriptBuilder())
         using (ExecutionEngine ee = new ExecutionEngine(null, Crypto.Default, null, null)) {
            ee.LoadScript(program);

            sb.EmitPush(new byte[] { 1, 2, 3, 4, 5 });
            sb.EmitPush("test_arrayreverse");

            ee.LoadScript(sb.ToArray());
            ee.Execute();

            byte[] result = ee.EvaluationStack.Peek().GetByteArray();
            Assert.Equal(new byte[] { 5, 4, 3, 2, 1 }, result);
         }
      }
   }
}
