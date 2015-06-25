using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MadProps.AppArgs;

namespace Fody4Unity
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new ConsoleLogger();

            CommandLineArguments clargs;
            try
            {
                clargs = args.As<CommandLineArguments>();
            }
            catch (ArgumentException ex)
            {
                logger.LogException(ex);

                Console.WriteLine();
                Console.WriteLine(AppArgs.HelpFor<CommandLineArguments>());
                return;
            }

            var weavers = FindWeavers(clargs);

            using (IInnerWeaver innerWeaver = new InnerWeaver())
            {
                // Default
                innerWeaver.KeyFilePath = "";
                innerWeaver.SignAssembly = false;

                // Not from args
                innerWeaver.Logger = logger;
                innerWeaver.Weavers = weavers.ToList();

                // From args
                innerWeaver.AssemblyFilePath = clargs.AssemblyFilePath;
                innerWeaver.References = clargs.References;
                innerWeaver.SolutionDirectoryPath = clargs.SolutionDirectory;
                innerWeaver.IntermediateDirectoryPath = Path.GetDirectoryName(clargs.AssemblyFilePath);
                innerWeaver.DefineConstants = clargs.DefineConstants.Split(';').ToList();
                innerWeaver.ProjectDirectoryPath = clargs.ProjectDirectoryPath;

                innerWeaver.ReferenceCopyLocalPaths = clargs.ReferenceCopyLocalPaths?.Split(';').ToList();

                innerWeaver.Execute();
            }
        }

        private static IEnumerable<WeaverEntry> FindWeavers(CommandLineArguments clargs)
        {
            yield break;
        }
    }
}