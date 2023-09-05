// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Input.Hands;
using UnityEngine;

namespace RealityToolkit.MetaPlatform.InputService.Profiles
{
    /// <summary>
    /// Configuration profile for Oculus hand controllers.
    /// </summary>
    public class MetaHandControllerServiceModuleProfile : BaseHandControllerServiceModuleProfile
    {
        [SerializeField]
        [Tooltip("The minimum hand tracking confidence expected.")]
        private int minConfidenceRequired = 0;

        /// <summary>
        /// The minimum hand tracking confidence expected.
        /// </summary>
        public int MinConfidenceRequired => minConfidenceRequired;
    }
}