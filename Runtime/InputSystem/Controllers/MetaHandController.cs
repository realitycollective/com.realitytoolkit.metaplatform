// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers;
using RealityToolkit.Meta.Extensions;
using RealityToolkit.Meta.Plugins;
using RealityToolkit.Services.InputSystem.Controllers.UnityXR;
using UnityEngine;

namespace RealityToolkit.Meta.InputSystem.Controllers
{
    /// <summary>
    /// Hand tracking based controller for the <see cref="MetaPlatform"/> platform.
    /// </summary>
    [System.Runtime.InteropServices.Guid("b8b90b51-8156-4d8b-a2fc-09b3a12d4fc5")]
    public class MetaHandController : UnityXRHandController
    {
        /// <inheritdoc />
        public MetaHandController() { }

        /// <inheritdoc />
        public MetaHandController(IMixedRealityControllerDataProvider controllerDataProvider, TrackingState trackingState, Handedness controllerHandedness, MixedRealityControllerMappingProfile controllerMappingProfile)
            : base(controllerDataProvider, trackingState, controllerHandedness, controllerMappingProfile)
        {
            handJointDataProvider = new MetaHandJointDataProvider(controllerHandedness);
        }

        private OculusApi.HandState handState = new OculusApi.HandState();

        /// <inheritdoc />
        protected override void UpdateSpatialPointerPose()
        {
            if (!(OculusApi.GetHandState(OculusApi.Step.Render, Handedness.ToHand(), ref handState)))
            {
                return;
            }

            var platformRootPosition = handState.RootPose.Position;
            var platformPointerPosition = cameraRigTransform.position + handState.PointerPose.Position - platformRootPosition;
            var platformPointerRotation = Quaternion.Inverse(cameraRigTransform.rotation) * cameraRigTransform.rotation * handState.PointerPose.Orientation.ToQuaternionFlippedXY();

            SpatialPointerPose = new MixedRealityPose(platformPointerPosition, platformPointerRotation);
        }
    }
}