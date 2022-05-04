// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using RealityToolkit.Attributes;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Devices;
using RealityToolkit.Definitions.InputSystem;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem;
using RealityToolkit.MetaPlatform.Plugins;
using RealityToolkit.MetaPlatform.Profiles;
using RealityToolkit.MetaPlatform.Utilities;
using RealityToolkit.Services;
using RealityToolkit.Services.InputSystem.Controllers.Hands;
using UnityEngine;

namespace RealityToolkit.MetaPlatform.InputSystem.Controllers
{
    [RuntimePlatform(typeof(MetaPlatform))]
    [System.Runtime.InteropServices.Guid("EA666456-BAEF-4412-A829-A4C7132E98C3")]
    public class MetaHandControllerDataProvider : BaseHandControllerDataProvider
    {
        /// <inheritdoc />
        public MetaHandControllerDataProvider(string name, uint priority, MetaHandControllerDataProviderProfile profile, IMixedRealityInputSystem parentService)
            : base(name, priority, profile, parentService)
        {
            if (!MixedRealityToolkit.TryGetSystemProfile<IMixedRealityInputSystem, MixedRealityInputSystemProfile>(out var inputSystemProfile))
            {
                throw new ArgumentException($"Unable to get a valid {nameof(MixedRealityInputSystemProfile)}!");
            }

            MinConfidenceRequired = (OculusApi.TrackingConfidence)profile.MinConfidenceRequired;
            handDataProvider = new MetaHandDataConverter();

            var isGrippingThreshold = profile.GripThreshold != inputSystemProfile.GripThreshold
                ? profile.GripThreshold
                : inputSystemProfile.GripThreshold;

            postProcessor = new HandDataPostProcessor(TrackedPoses, isGrippingThreshold)
            {
                PlatformProvidesPointerPose = true
            };
        }

        private readonly MetaHandDataConverter handDataProvider;
        private readonly HandDataPostProcessor postProcessor;
        private readonly Dictionary<Handedness, MixedRealityHandController> activeControllers = new Dictionary<Handedness, MixedRealityHandController>();

        /// <summary>
        /// The minimum required tracking confidence for hands to be registered.
        /// </summary>
        public OculusApi.TrackingConfidence MinConfidenceRequired { get; set; }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

            if (handDataProvider.TryGetHandData(Handedness.Left, RenderingMode == HandRenderingMode.Mesh, MinConfidenceRequired, out var leftHandData))
            {
                var controller = GetOrAddController(Handedness.Left);
                leftHandData = postProcessor.PostProcess(Handedness.Left, leftHandData);
                controller?.UpdateController(leftHandData);
            }
            else
            {
                RemoveController(Handedness.Left);
            }

            if (handDataProvider.TryGetHandData(Handedness.Right, RenderingMode == HandRenderingMode.Mesh, MinConfidenceRequired, out var rightHandData))
            {
                var controller = GetOrAddController(Handedness.Right);
                rightHandData = postProcessor.PostProcess(Handedness.Right, rightHandData);
                controller?.UpdateController(rightHandData);
            }
            else
            {
                RemoveController(Handedness.Right);
            }
        }

        /// <inheritdoc />
        public override void Disable()
        {
            foreach (var activeController in activeControllers)
            {
                RemoveController(activeController.Key, false);
            }

            activeControllers.Clear();
        }

        private bool TryGetController(Handedness handedness, out MixedRealityHandController controller)
        {
            if (activeControllers.ContainsKey(handedness))
            {
                var existingController = activeControllers[handedness];
                Debug.Assert(existingController != null, $"Hand Controller {handedness} has been destroyed but remains in the active controller registry.");
                controller = existingController;
                return true;
            }

            controller = null;
            return false;
        }

        private MixedRealityHandController GetOrAddController(Handedness handedness)
        {
            // If a device is already registered with the handedness, just return it.
            if (TryGetController(handedness, out var existingController))
            {
                return existingController;
            }

            MixedRealityHandController detectedController;
            try
            {
                detectedController = new MixedRealityHandController(this, TrackingState.Tracked, handedness, GetControllerMappingProfile(typeof(MixedRealityHandController), handedness));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to create {nameof(MixedRealityHandController)}!\n{e}");
                return null;
            }

            detectedController.TryRenderControllerModel();
            AddController(detectedController);
            activeControllers.Add(handedness, detectedController);
            InputSystem?.RaiseSourceDetected(detectedController.InputSource, detectedController);

            return detectedController;
        }

        private void RemoveController(Handedness handedness, bool removeFromRegistry = true)
        {
            if (TryGetController(handedness, out var controller))
            {
                InputSystem?.RaiseSourceLost(controller.InputSource, controller);

                if (removeFromRegistry)
                {
                    RemoveController(controller);
                    activeControllers.Remove(handedness);
                }
            }
        }
    }
}