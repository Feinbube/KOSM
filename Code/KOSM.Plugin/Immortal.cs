using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KOSM
{
    public static class Immortal
    {
        private static GameObject gameObject;

        public static T AddImmortal<T>() where T : Component
        {
            if (gameObject == null)
            {
                gameObject = new GameObject("KOSMImmortal", typeof(T));
                UnityEngine.Object.DontDestroyOnLoad(gameObject);
            }
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }
    }
}
