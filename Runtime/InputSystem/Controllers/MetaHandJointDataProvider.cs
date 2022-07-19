// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using System.Collections.Generic;
using UnityEngine.XR;

namespace RealityToolkit.Meta.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="MetaHandController"/>.
    /// </summary>
    public class MetaHandJointDataProvider : IUnityXRHandJointDataProvider
    {
        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<XRHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            throw new System.NotImplementedException();
        }
    }
}
