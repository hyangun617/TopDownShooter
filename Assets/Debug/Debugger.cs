using UnityEngine;
using System.Diagnostics;

namespace MyGame.Utility
{
    public static class Debugger
    {
        [Conditional("UNITY_EDITOR")]
        public static void Log(string message)
        {
            UnityEngine.Debug.Log(message);   
        }
    }    
}
