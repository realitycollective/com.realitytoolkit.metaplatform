// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.IO;
using UnityEditor;
using RealityToolkit.Editor;
using RealityToolkit.Extensions;
using RealityToolkit.Editor.Utilities;

namespace RealityToolkit.MetaPlatform.Editor
{
    [InitializeOnLoad]
    internal static class OculusPackageInstaller
    {
        private static readonly string DefaultPath = $"{MixedRealityPreferences.ProfileGenerationPath}Meta";
        private static readonly string HiddenPath = Path.GetFullPath($"{PathFinderUtility.ResolvePath<IPathFinder>(typeof(OculusPathFinder)).BackSlashes()}{Path.DirectorySeparatorChar}{MixedRealityPreferences.HIDDEN_PACKAGE_ASSETS_PATH}");

        static OculusPackageInstaller()
        {
            EditorApplication.delayCall += CheckPackage;
        }

        [MenuItem(MixedRealityPreferences.Editor_Menu_Keyword + "/Packages/Install Meta Package Assets...", true)]
        private static bool ImportPackageAssetsValidation()
        {
            return !Directory.Exists($"{DefaultPath}{Path.DirectorySeparatorChar}");
        }

        [MenuItem(MixedRealityPreferences.Editor_Menu_Keyword + "/Packages/Install Meta Package Assets...")]
        private static void ImportPackageAssets()
        {
            EditorPreferences.Set($"{nameof(OculusPackageInstaller)}.Assets", false);
            EditorApplication.delayCall += CheckPackage;
        }

        private static void CheckPackage()
        {
            if (!EditorPreferences.Get($"{nameof(OculusPackageInstaller)}.Assets", false))
            {
                EditorPreferences.Set($"{nameof(OculusPackageInstaller)}.Assets", PackageInstaller.TryInstallAssets(HiddenPath, DefaultPath));
            }
        }
    }
}
