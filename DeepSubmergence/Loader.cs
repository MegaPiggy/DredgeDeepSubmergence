using UnityEngine;

namespace DeepSubmergence {
    public class Loader {

        // This method is run by Winch to initialize your mod
        public static void Initialize()
        {
            GameObject.DontDestroyOnLoad(new GameObject(nameof(DeepSubmergence), typeof(DeepSubmergence)));
        }
    }
}