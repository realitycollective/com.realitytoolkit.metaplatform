﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using XRTK.Attributes;
using XRTK.Definitions.CameraSystem;
using XRTK.Interfaces.CameraSystem;
using XRTK.MetaPlatform.Plugins;
using XRTK.Services.CameraSystem.Providers;

namespace XRTK.MetaPlatform.CameraSystem.Providers
{
    [RuntimePlatform(typeof(MetaPlatform))]
    [System.Runtime.InteropServices.Guid("83EFF552-ADF4-47C8-AD53-DF7406856D3F")]
    public class MetaCameraDataProvider : BaseCameraDataProvider
    {
        /// <inheritdoc />
        public MetaCameraDataProvider(string name, uint priority, BaseMixedRealityCameraDataProviderProfile profile, IMixedRealityCameraSystem parentService)
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
