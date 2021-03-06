// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Attributes;
using RealityToolkit.Editor.BuildPipeline;
using RealityToolkit.Interfaces;
using RealityToolkit.Services;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace RealityToolkit.MetaPlatform.Editor.BuildPipeline
{
    [RuntimePlatform(typeof(MetaPlatform))]
    public class OculusBuildInfo : AndroidBuildInfo
    {
        /// <inheritdoc />
        public override IMixedRealityPlatform BuildPlatform => new MetaPlatform();

        /// <inheritdoc />
        public override void OnPreProcessBuild(BuildReport report)
        {
            base.OnPreProcessBuild(report);

            if (!MixedRealityToolkit.ActivePlatforms.Contains(BuildPlatform) ||
                EditorUserBuildSettings.activeBuildTarget != BuildTarget)
            {
                return;
            }

            if (BuildPlatform.GetType() == typeof(MetaPlatform))
            {
                // TODO generate manifest
            }
        }
    }
}
