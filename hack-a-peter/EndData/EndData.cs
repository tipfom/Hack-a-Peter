﻿namespace hack_a_peter.EndData {
    public class EndData {
        public string LastScene { get; private set; }
        public int TimePlayed { get; set; }

        public EndData (string lastScene) {
            LastScene = lastScene;
        }
    }
}
