// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityToolkit.CameraService.Definitions;
using RealityToolkit.CameraService.Interfaces;
using RealityToolkit.CameraService.Modules;
using RealityToolkit.MetaPlatform.Plugins;

namespace RealityToolkit.MetaPlatform.CameraService
{
    [RuntimePlatform(typeof(MetaPlatform))]
    [System.Runtime.InteropServices.Guid("83EFF552-ADF4-47C8-AD53-DF7406856D3F")]
    public class MetaCameraRigServiceModule : BaseCameraServiceModule, IMetaCameraRigServiceModule
    {
        /// <inheritdoc />
        public MetaCameraRigServiceModule(string name, uint priority, BaseCameraServiceModuleProfile profile, ICameraService parentService)
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
