/************************************************************************************

Copyright   :   Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus SDK License Version 3.4.1 (the "License");
you may not use the Oculus SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

https://developer.oculus.com/licenses/sdk-3.4.1

Unless required by applicable law or agreed to in writing, the Oculus SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using RealityCollective.ServiceFramework.Editor;
using RealityToolkit.Editor;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace RealityToolkit.MetaPlatform.Editor.BuildPipeline
{
    /// <summary>
    /// https://developer.oculus.com/documentation/native/android/mobile-native-manifest/
    /// </summary>
    public class MetaManifestPreprocessor
    {
        private const string TEMPLATE_MANIFEST_FILE_NAME = "AndroidManifest.OVRSubmission.xml";

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Tools/Meta/Create Meta Quest compatible AndroidManifest.xml", false, 100000)]
        public static void GenerateManifestForSubmission()
        {
            var assetPath = PathFinderUtility.ResolvePath<IPathFinder>(typeof(MetaPackagePathFinder));
            var editorDir = $"{Path.GetFullPath(assetPath)}/Editor";
            var srcFile = $"{editorDir}/BuildPipeline/{TEMPLATE_MANIFEST_FILE_NAME}";

            if (!File.Exists(srcFile))
            {
                Debug.LogError("Cannot find Android manifest template for submission. Please reimport the RealityToolkit.Meta package.");
                return;
            }

            string manifestFolder = Application.dataPath + "/Plugins/Android";

            if (!Directory.Exists(manifestFolder))
            {
                Directory.CreateDirectory(manifestFolder);
            }

            string dstFile = manifestFolder + "/AndroidManifest.xml";

            if (File.Exists(dstFile))
            {
                if (!EditorUtility.DisplayDialog("AndroidManifest.xml Already Exists!", "Would you like to replace the existing manifest with a new one? All modifications will be lost.", "Replace", "Cancel"))
                {
                    return;
                }
            }

            PatchAndroidManifest(srcFile, dstFile, false);

            AssetDatabase.Refresh();
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Tools/Meta/Update Meta Quest AndroidManifest.xml", false, 100000)]
        public static void UpdateAndroidManifest()
        {
            string manifestFile = "Assets/Plugins/Android/AndroidManifest.xml";

            if (!File.Exists(manifestFile))
            {
                Debug.LogError("Unable to update manifest because it does not exist! Run \"Create store-compatible AndroidManifest.xml\" first");
                return;
            }

            if (!EditorUtility.DisplayDialog("Update AndroidManifest.xml", "This will overwrite all Meta specific AndroidManifest Settings. Continue?", "Overwrite", "Cancel"))
            {
                return;
            }

            PatchAndroidManifest(manifestFile, skipExistingAttributes: false);
            AssetDatabase.Refresh();
        }

        [MenuItem(RealityToolkitPreferences.Editor_Menu_Keyword + "/Tools/Meta/Remove AndroidManifest.xml", false, 100001)]
        public static void RemoveAndroidManifest()
        {
            AssetDatabase.DeleteAsset("Assets/Plugins/Android/AndroidManifest.xml");
            AssetDatabase.Refresh();
        }

        private static void AddOrRemoveTag(XmlDocument doc, string @namespace, string path, string elementName, string name, bool required, bool modifyIfFound, params string[] attrs) // name, value pairs
        {
            var nodes = doc.SelectNodes(path + "/" + elementName);
            XmlElement element = null;
            foreach (XmlElement e in nodes)
            {
                if (name == null || name == e.GetAttribute("name", @namespace))
                {
                    element = e;
                    break;
                }
            }

            if (required)
            {
                if (element == null)
                {
                    var parent = doc.SelectSingleNode(path);
                    element = doc.CreateElement(elementName);
                    element.SetAttribute("name", @namespace, name);
                    parent.AppendChild(element);
                }

                for (int i = 0; i < attrs.Length; i += 2)
                {
                    if (modifyIfFound || string.IsNullOrEmpty(element.GetAttribute(attrs[i], @namespace)))
                    {
                        if (attrs[i + 1] != null)
                        {
                            element.SetAttribute(attrs[i], @namespace, attrs[i + 1]);
                        }
                        else
                        {
                            element.RemoveAttribute(attrs[i], @namespace);
                        }
                    }
                }
            }
            else
            {
                if (element != null && modifyIfFound)
                {
                    element.ParentNode.RemoveChild(element);
                }
            }
        }

        public static void PatchAndroidManifest(string sourceFile, string destinationFile = null, bool skipExistingAttributes = true, bool enableSecurity = false)
        {
            if (destinationFile == null)
            {
                destinationFile = sourceFile;
            }

            bool modifyIfFound = !skipExistingAttributes;

            try
            {
                // Load android manfiest file
                XmlDocument doc = new XmlDocument();
                doc.Load(sourceFile);

                string androidNamespaceURI;
                XmlElement element = (XmlElement)doc.SelectSingleNode("/manifest");
                if (element == null)
                {
                    UnityEngine.Debug.LogError("Could not find manifest tag in android manifest.");
                    return;
                }

                // Get android namespace URI from the manifest
                androidNamespaceURI = element.GetAttribute("xmlns:android");
                if (string.IsNullOrEmpty(androidNamespaceURI))
                {
                    UnityEngine.Debug.LogError("Could not find Android Namespace in manifest.");
                    return;
                }

                ApplyRequiredManfiestTags(doc, androidNamespaceURI, modifyIfFound, enableSecurity);
                ApplyFeatureManfiestTags(doc, androidNamespaceURI, modifyIfFound);

                // The following manifest entries are all handled through Oculus XR SDK Plugin
    #if !PRIORITIZE_OCULUS_XR_SETTINGS
                ApplyOculusXRManifestTags(doc, androidNamespaceURI, modifyIfFound);
    #endif

                doc.Save(destinationFile);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }
        }

        private static void ApplyRequiredManfiestTags(XmlDocument doc, string androidNamespaceURI, bool modifyIfFound, bool enableSecurity)
        {
            //OVRProjectConfig projectConfig = OVRProjectConfig.GetProjectConfig();

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest/application/activity/intent-filter",
                "category",
                "android.intent.category.LEANBACK_LAUNCHER",
                required: false,
                modifyIfFound: true); // always remove leanback launcher

            // First add or remove headtracking flag if targeting Quest
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "android.hardware.vr.headtracking",
                true,
 //               OVRDeviceSelector.isTargetDeviceQuestFamily,
                true,
                "version", "1",
                "required", "true");
    //			"required", OVRProjectConfig.GetProjectConfig().allowOptional3DofHeadTracking ? "false" : "true");

            // make sure android label and icon are set in the manifest
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "application",
                null,
                true,
                modifyIfFound,
                "label", "@string/app_name",
                "icon", "@mipmap/app_icon",
                // Disable allowBackup in manifest and add Android NSC XML file
                "allowBackup", "false",
    //			"allowBackup", projectConfig.disableBackups ? "false" : "true",
                "networkSecurityConfig", null
    //			"networkSecurityConfig", projectConfig.enableNSCConfig && enableSecurity ? "@xml/network_sec_config" : null
                );
        }

        private static void ApplyFeatureManfiestTags(XmlDocument doc, string androidNamespaceURI, bool modifyIfFound)
        {
            //OVRProjectConfig projectConfig = OVRProjectConfig.GetProjectConfig();
            //OVRRuntimeSettings runtimeSettings = OVRRuntimeSettings.GetRuntimeSettings();

            //============================================================================
            // Hand Tracking
            // If Quest is the target device, add the handtracking manifest tags if needed
            // Mapping of project setting to manifest setting:
            // OVRProjectConfig.HandTrackingSupport.ControllersOnly => manifest entry not present
            // OVRProjectConfig.HandTrackingSupport.ControllersAndHands => manifest entry present and required=false
            // OVRProjectConfig.HandTrackingSupport.HandsOnly => manifest entry present and required=true
            //OVRProjectConfig.HandTrackingSupport targetHandTrackingSupport = OVRProjectConfig.GetProjectConfig().handTrackingSupport;
            //OVRProjectConfig.HandTrackingVersion targetHandTrackingVersion = OVRProjectConfig.GetProjectConfig().handTrackingVersion;
            bool handTrackingEntryNeeded = true;//OVRDeviceSelector.isTargetDeviceQuestFamily  && (targetHandTrackingSupport != OVRProjectConfig.HandTrackingSupport.ControllersOnly);
            bool handTrackingVersionEntryNeeded = true;//handTrackingEntryNeeded && (targetHandTrackingVersion != OVRProjectConfig.HandTrackingVersion.Default);
            string handTrackingVersionValue = "V2.0"; //(targetHandTrackingVersion == OVRProjectConfig.HandTrackingVersion.V2) ? "V2.0" : "V1.0";

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "oculus.software.handtracking",
                handTrackingEntryNeeded,
                modifyIfFound,
                "required",  "false");
    //			"required", (targetHandTrackingSupport == OVRProjectConfig.HandTrackingSupport.HandsOnly) ? "true" : "false");
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-permission",
                "com.oculus.permission.HAND_TRACKING",
                handTrackingEntryNeeded,
                modifyIfFound);

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest/application",
                "meta-data",
                "com.oculus.handtracking.frequency",
                handTrackingEntryNeeded,
                modifyIfFound,
                "value", "LOW");
    //			"value", projectConfig.handTrackingFrequency.ToString());

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest/application",
                "meta-data",
                "com.oculus.handtracking.version",
                handTrackingVersionEntryNeeded,
                modifyIfFound,
                "value", handTrackingVersionValue);

            //============================================================================
            // System Keyboard
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "oculus.software.overlay_keyboard",
                false,
    //			projectConfig.requiresSystemKeyboard,
                modifyIfFound,
                "required", "false");

            //============================================================================
            // Experimental Features
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "com.oculus.experimental.enabled",
                false,
    //			projectConfig.experimentalFeaturesEnabled,
                modifyIfFound,
                "required", "true");

            //============================================================================
            // Spatial Anchors
            //OVRProjectConfig.SpatialAnchorsSupport targetSpatialAnchorsSupport = OVRProjectConfig.GetProjectConfig().spatialAnchorsSupport;
            //bool spatialAnchorsEntryNeeded = OVRDeviceSelector.isTargetDeviceQuestFamily && (targetSpatialAnchorsSupport == OVRProjectConfig.SpatialAnchorsSupport.Enabled);

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-permission",
                "com.oculus.permission.USE_ANCHOR_API",
                false,
    //			spatialAnchorsEntryNeeded,
                modifyIfFound);

            //============================================================================
            // Passthrough
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "com.oculus.feature.PASSTHROUGH",
                false,
    //			projectConfig.insightPassthroughEnabled,
                modifyIfFound,
                "required", "true");

            //============================================================================
            // System Splash Screen
            /*
            if (projectConfig.systemSplashScreen != null)
            {
                AddOrRemoveTag(doc,
                    androidNamespaceURI,
                    "/manifest/application",
                    "meta-data",
                    "com.oculus.ossplash",
                    true,
                    modifyIfFound,
                    "value", "true");

                AddOrRemoveTag(doc,
                    androidNamespaceURI,
                    "/manifest/application",
                    "meta-data",
                    "com.oculus.ossplash.colorspace",
                    true,
                    modifyIfFound,
                    "value", ColorSpaceToManifestTag(runtimeSettings.colorSpace));
            }
            */
            //============================================================================
            // Render Model
            //OVRProjectConfig.RenderModelSupport renderModelSupport = OVRProjectConfig.GetProjectConfig().renderModelSupport;
            //bool renderModelEntryNeeded = OVRDeviceSelector.isTargetDeviceQuestFamily && (renderModelSupport == OVRProjectConfig.RenderModelSupport.Enabled);

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "com.oculus.feature.RENDER_MODEL",
                false,
    //			renderModelEntryNeeded,
                modifyIfFound);
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-permission",
                "com.oculus.permission.RENDER_MODEL",
                false,
    //			renderModelEntryNeeded,
                modifyIfFound);

            //============================================================================
            // Tracked Keyboard
            // If Quest is the target device, add the tracked keyboard manifest tags if needed
            // Mapping of project setting to manifest setting:
            // OVRProjectConfig.TrackedKeyboardSupport.None => manifest entry not present
            // OVRProjectConfig.TrackedKeyboardSupport.Supported => manifest entry present and required=false
            // OVRProjectConfig.TrackedKeyboardSupport.Required => manifest entry present and required=true
            //OVRProjectConfig.TrackedKeyboardSupport targetTrackedKeyboardSupport = OVRProjectConfig.GetProjectConfig().trackedKeyboardSupport;
            bool trackedKeyboardEntryNeeded = false;
            //bool trackedKeyboardEntryNeeded = OVRDeviceSelector.isTargetDeviceQuestFamily && (targetTrackedKeyboardSupport != OVRProjectConfig.TrackedKeyboardSupport.None);

            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-feature",
                "oculus.software.trackedkeyboard",
                trackedKeyboardEntryNeeded,
                modifyIfFound,
                "required", "false");
    //			"required", (targetTrackedKeyboardSupport == OVRProjectConfig.TrackedKeyboardSupport.Required) ? "true" : "false");
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest",
                "uses-permission",
                "com.oculus.permission.TRACKED_KEYBOARD",
                trackedKeyboardEntryNeeded,
            modifyIfFound);
        }


        private static void ApplyOculusXRManifestTags(XmlDocument doc, string androidNamespaceURI, bool modifyIfFound)
        {
            // Add focus aware tag if this app is targeting Quest Family
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest/application/activity",
                "meta-data",
                "com.oculus.vr.focusaware",
                true,
//                OVRDeviceSelector.isTargetDeviceQuestFamily,
                modifyIfFound,
                "value", "true");

            /* Not needed - check later
            // Add support devices manifest according to the target devices
            if (OVRDeviceSelector.isTargetDeviceQuestFamily)
            {
                string targetDeviceValue = "quest";
                if (OVRDeviceSelector.isTargetDeviceQuest && OVRDeviceSelector.isTargetDeviceQuest2)
                {
                    targetDeviceValue = "quest|quest2";
                }
                else if (OVRDeviceSelector.isTargetDeviceQuest2)
                {
                    targetDeviceValue = "quest2";
                }
                else if (OVRDeviceSelector.isTargetDeviceQuest)
                {
                    targetDeviceValue = "quest";
                }
                else
                {
                    Debug.LogError("Unexpected target devices");
                }
                AddOrRemoveTag(doc,
                    androidNamespaceURI,
                    "/manifest/application",
                    "meta-data",
                    "com.oculus.supportedDevices",
                    true,
                    modifyIfFound,
                    "value", targetDeviceValue);
            }
            */

            // Add VR intent filter tag in the manifest
            AddOrRemoveTag(doc,
                androidNamespaceURI,
                "/manifest/application/activity/intent-filter",
                "category",
                "com.oculus.intent.category.VR",
                required: true,
                modifyIfFound: true);
        }
/*
        private static string ColorSpaceToManifestTag(OVRManager.ColorSpace colorSpace)
        {
            switch (colorSpace)
            {
                case OVRManager.ColorSpace.Unmanaged:
                    return "!Unmanaged";
                case OVRManager.ColorSpace.Rec_2020:
                    return "Rec.2020";
                case OVRManager.ColorSpace.Rec_709:
                    return "Rec.709";
                case OVRManager.ColorSpace.Rift_CV1:
                    return "!RiftCV1";
                case OVRManager.ColorSpace.Rift_S:
                    return "!RiftS";
                case OVRManager.ColorSpace.Quest:
                    return "!Quest";
                case OVRManager.ColorSpace.P3:
                    return "P3";
                case OVRManager.ColorSpace.Adobe_RGB:
                    return "Adobe";
                default:
                    return "";
            }
        }
*/
    }
}