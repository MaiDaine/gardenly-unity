// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;

namespace Doozy.Engine.Utils
{
    [Serializable]
    public class LanguagePack : ScriptableObject
    {
        private const string CURRENT_LANGUAGE_PREFS_KEY = "Doozy.CurrentLanguage";
        public const Language DEFAULT_LANGUAGE = Language.English;

        private static Language s_currentLanguage = Language.Unknown;

        public static Language CurrentLanguage
        {
            get
            {
                if (s_currentLanguage != Language.Unknown) return s_currentLanguage;
                CurrentLanguage = (Language) PlayerPrefs.GetInt(CURRENT_LANGUAGE_PREFS_KEY, (int) DEFAULT_LANGUAGE);
                return s_currentLanguage;
            }
            set
            {
                SaveLanguagePreference(value);
                s_currentLanguage = value;
            }
        }

        private static void SaveLanguagePreference(Language language) { SaveLanguagePreference(CURRENT_LANGUAGE_PREFS_KEY, language); }

        private static void SaveLanguagePreference(string prefsKey, Language language)
        {
            PlayerPrefs.SetInt(prefsKey, (int) language);
            PlayerPrefs.Save();
        }
    }
}