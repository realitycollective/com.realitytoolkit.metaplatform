// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.Definitions.Utilities;
using RealityCollective.Extensions;
using RealityToolkit.Definitions.Controllers.Hands;
using RealityToolkit.Definitions.Utilities;
using RealityToolkit.Interfaces.CameraSystem;
using RealityToolkit.Interfaces.InputSystem.Providers.Controllers.Hands;
using RealityToolkit.Meta.Extensions;
using RealityToolkit.Meta.Plugins;
using RealityToolkit.Services;
using RealityToolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace RealityToolkit.Meta.InputSystem.Controllers
{
    /// <summary>
    /// Provides hand joints data for use with <see cref="MetaHandController"/>.
    /// </summary>
    public class MetaHandJointDataProvider : IUnityXRHandJointDataProvider
    {
        public MetaHandJointDataProvider(Handedness handedness)
        {
            this.handedness = handedness;
        }

        ~MetaHandJointDataProvider()
        {
            if (!conversionProxyRootTransform.IsNull())
            {
                conversionProxyTransforms.Clear();
                conversionProxyRootTransform.Destroy();
            }
        }

        private Transform conversionProxyRootTransform;
        private readonly Dictionary<OculusApi.BoneId, Transform> conversionProxyTransforms = new Dictionary<OculusApi.BoneId, Transform>();
        private readonly Handedness handedness;
        private OculusApi.Skeleton handSkeleton = new OculusApi.Skeleton();
        private OculusApi.HandState handState = new OculusApi.HandState();
        private IMixedRealityCameraRig cameraRig;

        /// <inheritdoc />
        public void UpdateHandJoints(InputDevice inputDevice, ref MixedRealityPose[] jointPoses, ref Dictionary<XRHandJoint, MixedRealityPose> jointPosesDictionary)
        {
            if (cameraRig == null)
            {
                FindCameraRig();
            }

            if (!(OculusApi.GetHandState(OculusApi.Step.Render, handedness.ToHand(), ref handState) &&
                OculusApi.GetSkeleton(handedness.ToSkeletonType(), out handSkeleton)))
            {
                return;
            }

            var pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_WristRoot]);
            jointPoses[(int)XRHandJoint.Wrist] = pose;
            jointPosesDictionary[XRHandJoint.Wrist] = pose;

            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Thumb1]);
            jointPoses[(int)XRHandJoint.ThumbMetacarpal] = pose;
            jointPosesDictionary[XRHandJoint.ThumbMetacarpal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Thumb2]);
            jointPoses[(int)XRHandJoint.ThumbProximal] = pose;
            jointPosesDictionary[XRHandJoint.ThumbProximal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Thumb3]);
            jointPoses[(int)XRHandJoint.ThumbDistal] = pose;
            jointPosesDictionary[XRHandJoint.ThumbDistal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_ThumbTip]);
            jointPoses[(int)XRHandJoint.ThumbTip] = pose;
            jointPosesDictionary[XRHandJoint.ThumbTip] = pose;

            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Index1]);
            jointPoses[(int)XRHandJoint.IndexProximal] = pose;
            jointPosesDictionary[XRHandJoint.IndexProximal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Index2]);
            jointPoses[(int)XRHandJoint.IndexIntermediate] = pose;
            jointPosesDictionary[XRHandJoint.IndexIntermediate] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Index3]);
            jointPoses[(int)XRHandJoint.IndexDistal] = pose;
            jointPosesDictionary[XRHandJoint.IndexDistal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_IndexTip]);
            jointPoses[(int)XRHandJoint.IndexTip] = pose;
            jointPosesDictionary[XRHandJoint.IndexTip] = pose;

            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Middle1]);
            jointPoses[(int)XRHandJoint.MiddleProximal] = pose;
            jointPosesDictionary[XRHandJoint.MiddleProximal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Middle2]);
            jointPoses[(int)XRHandJoint.MiddleIntermediate] = pose;
            jointPosesDictionary[XRHandJoint.MiddleIntermediate] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Middle3]);
            jointPoses[(int)XRHandJoint.MiddleDistal] = pose;
            jointPosesDictionary[XRHandJoint.MiddleDistal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_MiddleTip]);
            jointPoses[(int)XRHandJoint.MiddleTip] = pose;
            jointPosesDictionary[XRHandJoint.MiddleTip] = pose;

            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Ring1]);
            jointPoses[(int)XRHandJoint.RingProximal] = pose;
            jointPosesDictionary[XRHandJoint.RingProximal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Ring2]);
            jointPoses[(int)XRHandJoint.RingIntermediate] = pose;
            jointPosesDictionary[XRHandJoint.RingIntermediate] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Ring3]);
            jointPoses[(int)XRHandJoint.RingDistal] = pose;
            jointPosesDictionary[XRHandJoint.RingDistal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_RingTip]);
            jointPoses[(int)XRHandJoint.RingTip] = pose;
            jointPosesDictionary[XRHandJoint.RingTip] = pose;

            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Pinky0]);
            jointPoses[(int)XRHandJoint.LittleMetacarpal] = pose;
            jointPosesDictionary[XRHandJoint.LittleMetacarpal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Pinky1]);
            jointPoses[(int)XRHandJoint.LittleProximal] = pose;
            jointPosesDictionary[XRHandJoint.LittleProximal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Pinky2]);
            jointPoses[(int)XRHandJoint.LittleIntermediate] = pose;
            jointPosesDictionary[XRHandJoint.LittleIntermediate] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Pinky3]);
            jointPoses[(int)XRHandJoint.LittleDistal] = pose;
            jointPosesDictionary[XRHandJoint.LittleDistal] = pose;
            pose = GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_PinkyTip]);
            jointPoses[(int)XRHandJoint.LittleTip] = pose;
            jointPosesDictionary[XRHandJoint.LittleTip] = pose;

            // Estimated: These joint poses are not provided by the Oculus
            // hand tracking implementation. But with the data we now have, we can
            // estimate their poses fairly well.
            pose = HandUtilities.GetEstimatedPalmPose(jointPoses);
            jointPoses[(int)XRHandJoint.Palm] = pose;
            jointPosesDictionary[XRHandJoint.LittleTip] = pose;
            pose = HandUtilities.GetEstimatedIndexMetacarpalPose(jointPoses);
            jointPoses[(int)XRHandJoint.IndexMetacarpal] = pose;
            jointPosesDictionary[XRHandJoint.LittleTip] = pose;
            pose = HandUtilities.GetEstimatedMiddleMetacarpalPose(jointPoses);
            jointPoses[(int)XRHandJoint.MiddleMetacarpal] = pose;
            jointPosesDictionary[XRHandJoint.LittleTip] = pose;
            pose = HandUtilities.GetEstimatedRingMetacarpalPose(jointPoses);
            jointPoses[(int)XRHandJoint.RingMetacarpal] = pose;
            jointPosesDictionary[XRHandJoint.LittleTip] = pose;
        }

        private MixedRealityPose GetJointPose(Handedness handedness, OculusApi.Bone bone)
        {
            // The Pinky/Thumb 1+ bones depend on the Pinky/Thumb 0 bone
            // to be available, which the XRTK hand tracking does not use. We still have to compute them to
            // be able to resolve pose relation dependencies.
            if (bone.Id == OculusApi.BoneId.Hand_Thumb1)
            {
                GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Thumb0]);
            }
            else if (bone.Id == OculusApi.BoneId.Hand_Pinky1)
            {
                GetJointPose(handedness, handSkeleton.Bones[(int)OculusApi.BoneId.Hand_Pinky0]);
            }

            var boneProxyTransform = GetProxyTransform(handedness, bone.Id);
            var parentProxyTransform = GetProxyTransform(handedness, (OculusApi.BoneId)bone.ParentBoneIndex);

            boneProxyTransform.parent = parentProxyTransform;

            if (bone.ParentBoneIndex == (int)OculusApi.BoneId.Invalid)
            {
                var rootPose = FixRotation(handedness, new MixedRealityPose(conversionProxyRootTransform.localPosition, conversionProxyRootTransform.localRotation));
                boneProxyTransform.localPosition = rootPose.Position;
                boneProxyTransform.localRotation = rootPose.Rotation;
            }
            else
            {
                boneProxyTransform.localPosition = bone.Pose.Position;
                boneProxyTransform.localRotation = handState.BoneRotations[(int)bone.Id].ToQuaternionFlippedXY();
            }

            return FixRotation(handedness, new MixedRealityPose(
                conversionProxyRootTransform.InverseTransformPoint(boneProxyTransform.position),
                Quaternion.Inverse(conversionProxyRootTransform.rotation) * boneProxyTransform.rotation));
        }

        private MixedRealityPose FixRotation(Handedness handedness, MixedRealityPose jointPose)
        {
            if (handedness == Handedness.Left)
            {
                // Rotate bone 180 degrees on X to flip up.
                jointPose.Rotation *= Quaternion.Euler(180f, 0f, 0f);

                // Rotate bone 90 degrees on Y to align X with right.
                jointPose.Rotation *= Quaternion.Euler(0f, 90f, 0f);
            }
            else
            {
                // Rotate bone 90 degrees on Y to align X with left.
                jointPose.Rotation *= Quaternion.Euler(0f, -90f, 0f);
            }

            return jointPose;
        }

        private Transform GetProxyTransform(Handedness handedness, OculusApi.BoneId boneId)
        {
            if (conversionProxyRootTransform.IsNull())
            {
                conversionProxyRootTransform = new GameObject("Meta Hand Conversion Proxy").transform;
                conversionProxyRootTransform.transform.SetParent(cameraRig.RigTransform, false);
                conversionProxyRootTransform.gameObject.SetActive(false);
            }

            // Depending on the handedness we are currently working on, we need to
            // rotate the conversion root. Same dilemma as with FixRotation above.
            conversionProxyRootTransform.localRotation = Quaternion.Euler(0f, handedness == Handedness.Right ? 180f : 0f, 0f);

            if (boneId == OculusApi.BoneId.Invalid)
            {
                return conversionProxyRootTransform;
            }

            if (conversionProxyTransforms.ContainsKey(boneId))
            {
                return conversionProxyTransforms[boneId];
            }

            var transform = new GameObject($"Meta Hand {boneId} Proxy").transform;
            conversionProxyTransforms.Add(boneId, transform);

            return transform;
        }

        private void FindCameraRig()
        {
            if (!MixedRealityToolkit.TryGetService<IMixedRealityCameraSystem>(out var cameraSystem))
            {
                Debug.LogError($"{nameof(MetaHandJointDataProvider)} needs the {nameof(IMixedRealityCameraSystem)} to work.");
                return;
            }

            cameraRig = cameraSystem.MainCameraRig;
        }

    }
}
