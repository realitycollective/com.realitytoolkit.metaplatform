// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityToolkit.CameraSystem.Definitions;
using RealityToolkit.CameraSystem.Interfaces;
using RealityToolkit.CameraSystem.Providers;
using RealityToolkit.MetaPlatform.Plugins;

namespace RealityToolkit.MetaPlatform.CameraService
{
    [RuntimePlatform(typeof(MetaPlatform))]
    [System.Runtime.InteropServices.Guid("83EFF552-ADF4-47C8-AD53-DF7406856D3F")]
    public class MetaCameraServiceModule : BaseCameraServiceModule, IMetaCameraServiceModule
    {
        /// <inheritdoc />
        public MetaCameraServiceModule(string name, uint priority, BaseMixedRealityCameraServiceModuleProfile profile, IMixedRealityCameraSystem parentService)
            : base(name, priority, profile, parentService)
        {
        }

        /// <inheritdoc />
        public override void Update()
        {
            OculusApi.UpdateHMDEvents();
            OculusApi.UpdateUserEvents();
        }
    }
}
