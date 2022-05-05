﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;
using RealityToolkit.Editor.Extensions;
using RealityToolkit.Editor.Profiles.InputSystem.Controllers;
using RealityToolkit.MetaPlatform.Plugins;
using RealityToolkit.MetaPlatform.Profiles;

namespace RealityToolkit.MetaPlatform.Editor
{
    /// <summary>
    /// Default inspector for <see cref="MetaHandControllerDataProviderProfile"/>.
    /// </summary>
    [CustomEditor(typeof(MetaHandControllerDataProviderProfile))]
    public class OculusHandControllerDataProviderProfileInspector : BaseMixedRealityHandControllerDataProviderProfileInspector
    {
        private SerializedProperty minConfidenceRequired;

        private bool showOculusHandTrackingSettings = true;
        private GUIContent confidenceContent;
        private static readonly GUIContent oculusHandSettingsFoldoutHeader = new GUIContent("Oculus Hand Tracking Settings");

        protected override void OnEnable()
        {
            base.OnEnable();

            minConfidenceRequired = serializedObject.FindProperty(nameof(minConfidenceRequired));
            confidenceContent = new GUIContent(minConfidenceRequired.displayName, minConfidenceRequired.tooltip);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            showOculusHandTrackingSettings = EditorGUILayoutExtensions.FoldoutWithBoldLabel(showOculusHandTrackingSettings, oculusHandSettingsFoldoutHeader, true);
            if (showOculusHandTrackingSettings)
            {
                EditorGUI.indentLevel++;
                minConfidenceRequired.intValue = (int)(OculusApi.TrackingConfidence)EditorGUILayout.EnumPopup(confidenceContent, (OculusApi.TrackingConfidence)minConfidenceRequired.intValue);
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}