// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Platforms;
using RealityToolkit.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Meta
{
    [System.Runtime.InteropServices.Guid("DB1ACC26-EC8D-4BC6-AFCA-C51351B2DA2E")]
    public class MetaPlatform : BasePlatform
    {
        private const string xrDisplaySubsystemDescriptorId = "oculus display";
        private const string xrInputSubsystemDescriptorId = "oculus input";

        /// <inheritdoc />
        public override IMixedRealityPlatform[] PlatformOverrides { get; } =
        {
            new AndroidPlatform(),
            new WindowsStandalonePlatform()
        };

        /// <inheritdoc />
        public override bool IsAvailable
        {
            get
            {
                var displaySubsystems = new List<XRDisplaySubsystem>();
                SubsystemManager.GetSubsystems(displaySubsystems);
                var xrDisplaySubsystemDescriptorFound = false;

                for (var i = 0; i < displaySubsystems.Count; i++)
                {
                    var displaySubsystem = displaySubsystems[i];
                    if (displaySubsystem.SubsystemDescriptor.id.Equals(xrDisplaySubsystemDescriptorId) &&
                        displaySubsystem.running)
                    {
                        xrDisplaySubsystemDescriptorFound = true;
                    }
                }

                // The XR Display Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrDisplaySubsystemDescriptorFound)
                {
                    return false;
                }

                var inputSubsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetSubsystems(inputSubsystems);
                var xrInputSubsystemDescriptorFound = false;

                for (var i = 0; i < inputSubsystems.Count; i++)
                {
                    var inputSubsystem = inputSubsystems[i];
                    if (inputSubsystem.SubsystemDescriptor.id.Equals(xrInputSubsystemDescriptorId) &&
                        inputSubsystem.running)
                    {
                        xrInputSubsystemDescriptorFound = true;
                    }
                }

                // The XR Input Subsystem is not available / running,
                // the platform doesn't seem to be available.
                if (!xrInputSubsystemDescriptorFound)
                {
                    return false;
                }

                // Only if both, Display and Input XR Subsystems are available
                // and running, the platform is considered available.
                return true;
            }
        }

#if UNITY_EDITOR
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