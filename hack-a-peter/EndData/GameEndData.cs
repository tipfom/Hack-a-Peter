
namespace hack_a_peter.EndData {
    public class GameEndData : EndData {
        public int FinishedScore { get; private set; }

        public GameEndData (int finishedScore, string lastScene) : base (lastScene) {
            FinishedScore = finishedScore;
        }
    }
}
