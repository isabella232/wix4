// Copyright (c) .NET Foundation and contributors. All rights reserved. Licensed under the Microsoft Reciprocal License. See LICENSE.TXT file in the project root for full license information.

namespace WixToolset.Core.Burn.CommandLine
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using WixToolset.Core.Burn.Inscribe;
    using WixToolset.Data;
    using WixToolset.Extensibility.Data;
    using WixToolset.Extensibility.Services;

    internal class ReattachSubcommand : BurnSubcommandBase
    {
        public ReattachSubcommand(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
            this.Messaging = serviceProvider.GetService<IMessaging>();
        }

        private IServiceProvider ServiceProvider { get; }

        private IMessaging Messaging { get; }

        private string InputPath { get; set; }

        private string SignedEnginePath { get; set; }

        private string IntermediateFolder { get; set; }

        private string OutputPath { get; set; }

        public override CommandLineHelp GetCommandLineHelp()
        {
            return new CommandLineHelp("Reattaches a signed burn engine to a bundle.", "burn reattach [options] original.exe -engine signed.exe -o final.exe", new[]
            {
                new CommandLineHelpSwitch("-intermediateFolder", "Optional working folder. If not specified %TMP% will be used."),
                new CommandLineHelpSwitch("-out", "-o", "Output bundle with signed engine attached."),
            });
        }

        public override Task<int> ExecuteAsync(CancellationToken cancellationToken)
        {
            if (String.IsNullOrEmpty(this.InputPath))
            {
                this.Messaging.Write(ErrorMessages.FilePathRequired("input bundle"));
            }
            else if (String.IsNullOrEmpty(this.SignedEnginePath))
            {
                this.Messaging.Write(ErrorMessages.FilePathRequired("detached signed bundle bundle"));
            }
            else
            {
                if (String.IsNullOrEmpty(this.IntermediateFolder))
                {
                    this.IntermediateFolder = Path.GetTempPath();
                }

                if (String.IsNullOrEmpty(this.OutputPath))
                {
                    this.OutputPath = this.InputPath;
                }

                var command = new InscribeBundleCommand(this.ServiceProvider, this.InputPath, this.SignedEnginePath, this.OutputPath, this.IntermediateFolder);
                var didWork = command.Execute();

                // If the detach subcommand did not encounter an error but did no work
                // then return the special exit code that indicates no work was done (-1000).
                var exitCode = this.Messaging.LastErrorNumber;

                if (!didWork && exitCode == 0)
                {
                    exitCode = -1000;
                    return Task.FromResult(exitCode);
                }
            }

            return Task.FromResult(this.Messaging.LastErrorNumber);
        }

        public override bool TryParseArgument(ICommandLineParser parser, string argument)
        {
            if (parser.IsSwitch(argument))
            {
                var parameter = argument.Substring(1);
                switch (parameter.ToLowerInvariant())
                {
                    case "engine":
                        this.SignedEnginePath = parser.GetNextArgumentAsFilePathOrError(argument, "detached signed bundle bundle");
                        return true;

                    case "intermediatefolder":
                        this.IntermediateFolder = parser.GetNextArgumentAsDirectoryOrError(argument);
                        return true;

                    case "o":
                    case "out":
                        this.OutputPath = parser.GetNextArgumentAsFilePathOrError(argument, "output bundle");
                        return true;
                }
            }
            else if (String.IsNullOrEmpty(this.InputPath))
            {
                this.InputPath = argument;
                return true;
            }

            return false;
        }
    }
}
