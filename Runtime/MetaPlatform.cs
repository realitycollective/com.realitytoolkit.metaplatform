// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Platforms;
using RealityToolkit.Interfaces;
using RealityToolkit.MetaPlatform.Plugins;
using UnityEngine;

namespace RealityToolkit.MetaPlatform
{
    [System.Runtime.InteropServices.Guid("DB1ACC26-EC8D-4BC6-AFCA-C51351B2DA2E")]
    public class MetaPlatform : BasePlatform
    {
        private static readonly System.Version NoVersion = new System.Version();

        /// <inheritdoc />
        public override bool IsAvailable =>
            !Application.isEditor && OculusApi.Version > NoVersion && OculusApi.Initialized;

        /// <inheritdoc />
        public override IMixedRealityPlatform[] PlatformOverrides { get; } =
        {
            new AndroidPlatform(),
            new WindowsStandalonePlatform()
        };

#if UNITY_EDITOR
        /// <inheritdoc />
        public override bool IsBuildTargetAvailable => base.IsBuildTargetAvailable && OculusApi.Version > NoVersion;

        /// <inheritdoc />
        public override UnityEditor.BuildTarget[] ValidBuildTargets { get; } =
        {
            UnityEditor.BuildTarget.Android,
            UnityEditor.BuildTarget.StandaloneWindows64,
            UnityEditor.BuildTarget.StandaloneWindows
        };
#endif // UNITY_EDITOR
    }
}