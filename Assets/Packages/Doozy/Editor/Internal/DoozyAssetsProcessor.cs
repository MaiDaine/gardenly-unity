// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Editor.Nody.Settings;
using Doozy.Editor.Settings;
using Doozy.Editor.Utils;
using Doozy.Engine.Soundy;
using Doozy.Engine.Touchy;
using Doozy.Engine.UI.Animation;
using Doozy.Engine.UI.Settings;
using Doozy.Engine.Utils;
using UnityEditor;

namespace Doozy.Editor.Internal
{
    [InitializeOnLoad]
    public static class DoozyAssetsProcessor
    {
        static DoozyAssetsProcessor() { ExecuteProcessor(); }

        private static void ExecuteProcessor()
        {
            if (!ProcessorsSettings.Instance.RunDoozyAssetsProcessor) return;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            DelayedCall.Run(1f, Run);
        }

        public static void Run()
        {
            CreateSettingsAssets();
            CreateDatabaseAssets();
            RegenerateDatabaseAssets();

            ProcessorsSettings.Instance.RunDoozyAssetsProcessor = false;
            ProcessorsSettings.Instance.SetDirty(false);

            DoozyUtils.DisplayProgressBar("Hold On", "Saving Processor Settings...", 0.95f);
            DoozyUtils.SaveAssets();
            DoozyUtils.ClearProgressBar();
        }

        private static void CreateSettingsAssets()
        {
            DoozyUtils.ClearProgressBar();
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.1f);
            DoozyWindowSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.2f);
            NodyWindowSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.3f);
            SoundySettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.4f);
            TouchySettings.Instance.SetDirty(false);

            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.5f);
            UIButtonSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.6f);
            UICanvasSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.7f);
            UIDrawerSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.8f);
            UIPopupSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.85f);
            UIToggleSettings.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Settings Assets...", 0.9f);
            UIViewSettings.Instance.SetDirty(false);
            
            DoozyUtils.DisplayProgressBar("Hold On", "Saving Settings Assets...", 0.95f);
            DoozyUtils.SaveAssets();
            DoozyUtils.ClearProgressBar();
        }

        private static void CreateDatabaseAssets()
        {
            DoozyUtils.ClearProgressBar();
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.1f);
            UIAnimations.Instance.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.2f);
            SoundySettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.3f);
            UIButtonSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.5f);
            UICanvasSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.7f);
            UIDrawerSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.8f);
            UIPopupSettings.Database.SetDirty(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Creating Database Assets...", 0.9f);
            UIViewSettings.Database.SetDirty(false);
            
            DoozyUtils.DisplayProgressBar("Hold On", "Saving Database Assets...", 0.95f);
            DoozyUtils.SaveAssets();
            DoozyUtils.ClearProgressBar();
        }

        private static void RegenerateDatabaseAssets()
        {
            DoozyUtils.ClearProgressBar();

            //SOUNDY
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.1f);
            SoundySettings.Database.SearchForUnregisteredDatabases(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.2f);
            SoundySettings.Database.RefreshDatabase(false,true);
            
            //UIAnimations
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.3f);
            UIAnimations.Instance.SearchForUnregisteredDatabases(false);
            
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.4f);
            UIButtonSettings.Database.SearchForUnregisteredDatabases(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.6f);
            UICanvasSettings.Database.SearchForUnregisteredDatabases(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.7f);
            UIDrawerSettings.Database.SearchForUnregisteredDatabases(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.8f);
            UIPopupSettings.Database.SearchForUnregisteredLinks(false);
            DoozyUtils.DisplayProgressBar("Hold On", "Regenerating Databases...", 0.9f);
            UIViewSettings.Database.SearchForUnregisteredDatabases(false);
            
            DoozyUtils.DisplayProgressBar("Hold On", "Saving Database Assets...", 0.95f);
            DoozyUtils.SaveAssets();
            DoozyUtils.ClearProgressBar();
        }
    }
}