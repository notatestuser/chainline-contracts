using Neo;
using Neo.Cryptography;
using Neo.VM;
using UtilsTests.Utilities;
using Xunit;

namespace UtilsTests {
   public class UnitTest1 {
      [Fact]
      public void TestMethod1() {
         byte[] program = ExecutionHelper.Compile("HubContract");

         using (ScriptBuilder sb = new ScriptBuilder())
         using (ExecutionEngine ee = new ExecutionEngine(null, Crypto.Default, null, null)) {
            ee.LoadScript(program);

            sb.EmitPush(new byte[] { 1, 2, 3, 4, 5 });
            //sb.EmitPush("test_arrayreverse");

            ee.LoadScript(sb.ToArray(), false);
            ee.Execute();

            string result = ee.EvaluationStack.Peek().GetByteArray().ToHexString();
            Assert.Equal("0504030201", result);
         }
      }
   }
}
