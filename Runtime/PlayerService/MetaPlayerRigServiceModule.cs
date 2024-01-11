// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Attributes;
using RealityToolkit.PlayerService.Definitions;
using RealityToolkit.PlayerService.Interfaces;
using RealityToolkit.PlayerService.Modules;
using RealityToolkit.MetaPlatform.Plugins;

namespace RealityToolkit.MetaPlatform.PlayerService
{
    [RuntimePlatform(typeof(MetaPlatform))]
    [System.Runtime.InteropServices.Guid("83EFF552-ADF4-47C8-AD53-DF7406856D3F")]
    public class MetaPlayerRigServiceModule : BasePlayerRigServiceModule, IMetaPlayerRigServiceModule
    {
        /// <inheritdoc />
        public MetaPlayerRigServiceModule(string name, uint priority, BasePlayerRigServiceModuleProfile profile, IPlayerService parentService)
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
