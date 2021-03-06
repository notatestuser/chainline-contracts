﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

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

      private static Dictionary<string, byte[]> Cache = new Dictionary<string, byte[]>();
      
      [MethodImpl(MethodImplOptions.Synchronized)]
      public static byte[] Compile(string projectName) {
         if (Cache.ContainsKey(projectName))
            if (Cache.TryGetValue(projectName, out byte[] cachedBytes))
               return cachedBytes;

         string dllPath = Path.Combine(BasePath, projectName, $@"bin\{Directory}", $"{projectName}.dll");
         string workingDir = Path.GetDirectoryName(dllPath);
         string fullPath = Path.GetFullPath(dllPath);

         Console.WriteLine("Compiling DLL at: " + fullPath);
         
         if (! ConvertTask.Execute(fullPath)) {
            throw new Exception("Compile task failed!");
         }

         string avmPath = Path.GetFullPath(Path.Combine(workingDir, $"{projectName}.avm"));
         Console.WriteLine("Using AVM at: " + avmPath);

         byte[] bytes = File.ReadAllBytes(avmPath);
         Cache.Add(projectName, bytes);
         
         return bytes;
      }
   }
}
