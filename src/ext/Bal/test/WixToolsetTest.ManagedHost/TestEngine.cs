// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolsetTest.ManagedHost
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using WixInternal.TestSupport;
    using WixInternal.Core.TestPackage;

    public class TestEngine
    {
        private static readonly string TestEngineFile = TestData.Get(@"..\x64\examples\Example.TestEngine\Example.TestEngine.exe");
        private static readonly string TestEngineFileX86 = TestData.Get(@"..\x86\examples\Example.TestEngine\Example.TestEngine.exe");

        public TestEngineResult RunReloadEngine(string bundleFilePath, string tempFolderPath, bool x86 = false)
        {
            return this.RunTestEngine("reload", bundleFilePath, tempFolderPath, x86);
        }

        public TestEngineResult RunShutdownEngine(string bundleFilePath, string tempFolderPath, bool x86 = false)
        {
            return this.RunTestEngine("shutdown", bundleFilePath, tempFolderPath, x86);
        }

        private TestEngineResult RunTestEngine(string engineMode, string bundleFilePath, string tempFolderPath, bool x86 = false)
        {
            var baFolderPath = Path.Combine(tempFolderPath, "ba");
            var extractFolderPath = Path.Combine(tempFolderPath, "extract");
            var extractResult = BundleExtractor.ExtractBAContainer(null, bundleFilePath, baFolderPath, extractFolderPath);
            extractResult.AssertSuccess();

            var args = new string[] {
                engineMode,
                '"' + bundleFilePath + '"',
                '"' + extractResult.GetBAFilePath(baFolderPath) + '"',
            };
            return RunProcessCaptureOutput(x86 ? TestEngineFileX86 : TestEngineFile, args);
        }

        private static TestEngineResult RunProcessCaptureOutput(string executablePath, string[] arguments = null, string workingFolder = null)
        {
            var startInfo = new ProcessStartInfo(executablePath)
            {
                Arguments = String.Join(' ', arguments),
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = workingFolder,
            };

            var exitCode = 0;
            var output = new List<string>();

            using (var process = Process.Start(startInfo))
            {
                process.OutputDataReceived += (s, e) => { if (e.Data != null) { output.Add(e.Data); } };
                process.ErrorDataReceived += (s, e) => { if (e.Data != null) { output.Add(e.Data); } };

                process.BeginErrorReadLine();
                process.BeginOutputReadLine();

                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return new TestEngineResult
            {
                ExitCode = exitCode,
                Output = output,
            };
        }
    }
}
