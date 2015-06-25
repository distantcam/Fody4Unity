using System.ComponentModel.DataAnnotations;

namespace Fody4Unity
{
    class CommandLineArguments
    {
        [Required]
        public string AssemblyFilePath { get; set; }

        [Required]
        public string References { get; set; }

        public string ReferenceCopyLocalPaths { get; set; }

        [Required]
        public string SolutionDirectory { get; set; }

        [Required]
        public string DefineConstants { get; set; }

        [Required]
        public string ProjectDirectoryPath { get; set; }
    }
}