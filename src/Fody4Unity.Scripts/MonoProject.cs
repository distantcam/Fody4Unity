using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;

namespace Fody4Unity.Scripts
{
    enum MonoProjectType
    {
        GamePlugins,
        Game,
        EditorPlugins,
        Editor
    }

    class MonoProject
    {
        public static List<MonoProject> GetProjects()
        {
            var monoIslands = (IEnumerable)typeof(InternalEditorUtility).GetMethod("GetMonoIslands", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            var projects = new List<MonoProject>();
            foreach (var island in monoIslands)
            {
                var project = new MonoProject(island);
                if (!project.IsValid)
                    continue;
                projects.Add(project);
            }
            return projects;
        }

        object monoIsland;
        Type monoIslandType;

        private MonoProject(object monoIsland)
        {
            this.monoIsland = monoIsland;
            this.monoIslandType = monoIsland.GetType();

            var isFirstPass = FileName.Contains("firstpass");
            var isEditor = FileName.Contains("Editor");

            if (isFirstPass && isEditor)
                ProjectType = MonoProjectType.EditorPlugins;
            if (!isFirstPass && isEditor)
                ProjectType = MonoProjectType.Editor;
            if (isFirstPass && !isEditor)
                ProjectType = MonoProjectType.GamePlugins;
            if (!isFirstPass && !isEditor)
                ProjectType = MonoProjectType.Game;
        }

        private BuildTarget? buildTarget;
        public BuildTarget? BuildTarget => LazyLoad(ref buildTarget, "_target");

        private string classLibraryProfile;
        public string ClassLibraryProfile => LazyLoad(ref classLibraryProfile, "_classlib_profile");

        private string[] defines;
        public string[] Defines => LazyLoad(ref defines, "_defines");

        private string[] files;
        public string[] Files => LazyLoad(ref files, "_files");

        private string output;
        public string Output => LazyLoad(ref output, "_output");

        private string[] references;
        public string[] References => LazyLoad(ref references, "_references");

        public string FileName => Path.GetFileName(this.Output);

        public bool IsValid => Files.Length > 0;

        public MonoProjectType ProjectType { get; }

        private T GetFieldValue<T>(string name)
        {
            var field = monoIslandType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (field == null)
            {
                throw new ArgumentException($"Field '{name}' does not exist on type.", nameof(name));
            }
            return (T)field.GetValue(monoIsland);
        }

        private T LazyLoad<T>(ref T field, string name)
        {
            if (object.Equals(field, default(T)))
            {
                field = GetFieldValue<T>(name);
            }
            return field;
        }
    }
}