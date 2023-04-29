// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityCollective.ServiceFramework.Interfaces;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Editor.BuildPipeline;
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
        public override IPlatform BuildPlatform => new MetaPlatform();

        /// <inheritdoc />
        public override void OnPreProcessBuild(BuildReport report)
        {
            base.OnPreProcessBuild(report);

            if (!ServiceManager.ActivePlatforms.Contains(BuildPlatform) ||
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
