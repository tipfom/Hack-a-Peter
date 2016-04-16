using System;
using System.Collections.Generic;

namespace hack_a_peter {
    public class SceneList : List<Scene> {
        public const string MAIN_MENU_NAME = "main_menu";

        private int currentSceneIndex;
        public Scene CurrentScene { get { return this[currentSceneIndex]; } }

        public SceneList (params Scene[] allScenes) {
            this.AddRange (allScenes);
            SetScene (MAIN_MENU_NAME); // always start at the main menu
        }

        public bool SetScene (string name) {
            Console.WriteLine ("setting scene to " + name);
            int nextSceneIndex = this.FindIndex ((Scene scene) => scene.Name == name);
            if (nextSceneIndex != -1) {
                currentSceneIndex = nextSceneIndex;
                return true;
            } else {
                Console.WriteLine ("scene not found");
                return false;
            }
        }
    }
}
