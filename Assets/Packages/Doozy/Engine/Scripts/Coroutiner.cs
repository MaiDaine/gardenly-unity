// Copyright (c) 2015 - 2019 Doozy Entertainment / Marlink Trading SRL. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections;
using UnityEngine;

// ReSharper disable UnusedMember.Global
// ReSharper disable Unity.IncorrectMethodSignature
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Engine
{
    /// <inheritdoc />
    /// <summary> Special class used to run Coroutines on. When using any of its public static methods, it will instantiate itself and run any number of coroutines </summary>
    /// <seealso cref="T:UnityEngine.MonoBehaviour" />
    public class Coroutiner : MonoBehaviour
    {
        #region Static Properties

        private static Coroutiner s_instance;

        /// <summary> Returns a reference to the Coroutiner in the Scene. If one does not exist, it gets created </summary>
        public static Coroutiner Instance
        {
            get
            {
                if (s_instance != null) return s_instance;
                s_instance = new GameObject("Coroutiner", typeof(Coroutiner)).GetComponent<Coroutiner>();
                return s_instance;
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Starts a Coroutine and returns a reference to it </summary>
        /// <param name="enumerator"> The enumerator </param>
        public Coroutine StartLocalCoroutine(IEnumerator enumerator) { return StartCoroutine(enumerator); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="coroutine"> The coroutine </param>
        public void StopLocalCoroutine(Coroutine coroutine) { StopCoroutine(coroutine); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="enumerator"> The enumerator </param>
        public void StopLocalCoroutine(IEnumerator enumerator) { StopCoroutine(enumerator); }

        /// <summary> Stops all Coroutines running on this behaviour </summary>
        public void StopAllLocalCoroutines() { StopAllCoroutines(); }

        #endregion

        #region Static Methods

        /// <summary> Starts a Coroutine and returns a reference to it </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static Coroutine Start(IEnumerator enumerator) { return Instance.StartLocalCoroutine(enumerator); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="enumerator"> Target enumerator </param>
        public static void Stop(IEnumerator enumerator) { Instance.StopLocalCoroutine(enumerator); }

        /// <summary> Stops the first Coroutine named methodName, or the Coroutine stored in routine running on this behaviour </summary>
        /// <param name="coroutine"> The coroutine </param>
        public static void Stop(Coroutine coroutine) { Instance.StopLocalCoroutine(coroutine); }

        /// <summary> Stops all Coroutines running on this behaviour </summary>
        public static void StopAll() { Instance.StopAllLocalCoroutines(); }

        #endregion
    }
}