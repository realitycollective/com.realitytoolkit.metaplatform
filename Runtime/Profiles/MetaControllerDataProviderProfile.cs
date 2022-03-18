// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Definitions.Controllers;
using XRTK.Definitions.Utilities;
using XRTK.MetaPlatform.InputSystem.Controllers;

namespace XRTK.MetaPlatform.Profiles
{
    /// <summary>
    /// Configuration profile for Meta controllers.
    /// </summary>
    public class MetaControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MetaRemoteController)),
                new ControllerDefinition(typeof(MetaTouchController), Handedness.Left),
                new ControllerDefinition(typeof(MetaTouchController), Handedness.Right)
            };
        }
    }
}