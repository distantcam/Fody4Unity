using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Fody4Unity.Scripts
{
    [InitializeOnLoad]
    public class AssemblyPostProcessor
    {
        static AssemblyPostProcessor()
        {
            if (!OnWindows())
            {
                Debug.LogWarning("Fody Post Processing only supported on Windows.");
                return;
            }

            try
            {
                // Lock assemblies while they may be altered
                EditorApplication.LockReloadAssemblies();

                var mono = FindMonoBleedingEdge();

                if (!File.Exists(mono))
                {
                    Debug.LogWarning("Could not determine location of Mono Bleeding Edge.");
                    return;
                }

                var fodyPath = FindFodyPath();

                if (!File.Exists(fodyPath))
                {
                    Debug.LogWarning("Could not determine location of Fody.");
                    return;
                }

                var projects = MonoProject.GetProjects();
                foreach (var project in projects)
                {
                    RunFody4Unity(mono, fodyPath, project);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
            finally
            {
                // Unlock now that we're done
                EditorApplication.UnlockReloadAssemblies();
            }
        }

        private static void RunFody4Unity(string mono, string fodyPath, MonoProject project)
        {
            var projectPath = Path.GetDirectoryName(Application.dataPath);

            var referenceList = new string[project.References.Length];
            for (int i = 0; i < referenceList.Length; i++)
            {
                if (Path.IsPathRooted(project.References[i]))
                {
                    referenceList[i] = CombinePaths(projectPath, "Library", "UnityAssemblies", Path.GetFileName(project.References[i]));
                }
                else
                {
                    referenceList[i] = CombinePaths(projectPath, project.References[i]);
                }
            }
            var references = string.Join(";", referenceList);

            var assemblyFilePath = CombinePaths(projectPath, "Library", "ScriptAssemblies", project.FileName);
            var defines = string.Join(";", project.Defines);

            var arguments = string.Format("{0} /assemblyfilepath={1} /references={2} /defineconstants={3} /solutiondirectory={4} /projectdirectorypath={4}",
                QuoteIfNeeded(fodyPath),
                QuoteIfNeeded(assemblyFilePath),
                QuoteIfNeeded(references),
                defines,
                QuoteIfNeeded(projectPath));

            var startInfo = new System.Diagnostics.ProcessStartInfo(mono, arguments);
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardInput = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = projectPath;

            var process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();

            Debug.Log(process.StandardOutput.ReadToEnd());
        }

        static bool OnWindows() => Environment.OSVersion.Platform == PlatformID.Win32NT;

        static string FindAppPath()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var name = assembly.GetName();
                if (name.Name == "mscorlib")
                {
                    var fileInfo = new FileInfo(assembly.Location);
                    return fileInfo.Directory.Parent.Parent.Parent.Parent.ToString();
                }
            }

            return "";
        }

        static string FindMonoBleedingEdge()
        {
            var appPath = FindAppPath();

            if (string.IsNullOrEmpty(appPath))
            {
                Debug.LogWarning("Could not determine location of Unity app.");
                return "";
            }

            return CombinePaths(appPath, "MonoBleedingEdge", "bin", "mono.exe");
        }

        static string FindFodyPath() => CombinePaths(Application.dataPath, "Fody", "Editor", "bin", "Fody4Unity.exe");

        static string QuoteIfNeeded(string str) => str.Contains(" ") ? "\"" + str + "\"" : str;

        static string CombinePaths(params string[] args) => string.Join("/", args);
    }
}