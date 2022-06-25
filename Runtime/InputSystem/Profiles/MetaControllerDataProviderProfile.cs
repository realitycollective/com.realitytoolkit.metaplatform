// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityToolkit.Definitions.Controllers;
using RealityToolkit.Meta.InputSystem.Controllers;

namespace RealityToolkit.Meta.InputSystem.Profiles
{
    /// <summary>
    /// Configuration profile for the <see cref="Providers.MetaControllerDataProvider"/>.
    /// </summary>
    public class MetaControllerDataProviderProfile : BaseMixedRealityControllerDataProviderProfile
    {
        public override ControllerDefinition[] GetDefaultControllerOptions()
        {
            return new[]
            {
                new ControllerDefinition(typeof(MetaRemoteController)),
                new ControllerDefinition(typeof(MetaTouchController), Handedness.Left),
                new ControllerDefinition(typeof(MetaTouchController), Handedness.Right),
                new ControllerDefinition(typeof(MetaHandController), Handedness.Left),
                new ControllerDefinition(typeof(MetaHandController), Handedness.Right)
            };
        }
    }
}