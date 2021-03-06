﻿using CommandLine;

namespace Emb.Cli.NetFramework
{
    public class CommandLineOptions
    {
        [Option('s', "schema", Required = false, HelpText = @"Generate JSON Schemas for all configuration types", Default = false)]
        public bool GenerateSchema { get; set; }

        [Option('d', "directory", Required = false, HelpText = @"JSON Schemas directory path")]
        public string Directory { get; set; }
    }
}
