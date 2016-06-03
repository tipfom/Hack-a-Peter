using hack_a_peter.Scenes;
using System;
using System.Collections.Generic;

namespace hack_a_peter {
    public class SceneList : List<Scene> {
        private int currentSceneIndex;
        public Scene CurrentScene { get { return this[currentSceneIndex]; } }

        public SceneList(Scene.OnSceneFinished finishedHandleDelegate, params Scene[ ] allScenes) {
            this.AddRange(allScenes);
            SetScene(MainMenu.NAME); // always start at the main menu

            for (int i = 0; i < this.Count; i++) {
                this[i].Finished += finishedHandleDelegate;
            }
        }

        public bool SetScene(string name) {
            Console.WriteLine("setting scene to " + name);
            int nextSceneIndex = this.FindIndex((Scene scene) => scene.Name == name);
            if (nextSceneIndex != -1) {
                currentSceneIndex = nextSceneIndex;
                return true;
            } else {
                Console.WriteLine("scene not found");
                return false;
            }
        }
    }
}
