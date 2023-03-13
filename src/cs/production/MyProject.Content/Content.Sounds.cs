using bottlenoselabs.Katabasis;

namespace MyProject;

public static partial class Content
{
    public static class Sounds
    {
        public static SoundEffect Greeting;
        public static SoundEffect GameOver;
        public static SoundEffect Hit;
        public static SoundEffect ObjectiveComplete;

        internal static void LoadSounds()
        {
            Greeting = SoundEffect.FromFile("assets/sounds/greeting_6_alex.wav");
            GameOver = SoundEffect.FromFile("assets/sounds/miscellaneous_1_alex.wav");
            Hit = SoundEffect.FromFile("assets/sounds/BLLTImpt_Hit Marker_07.wav");
            ObjectiveComplete = SoundEffect.FromFile("assets/sounds/completion_8_alex.wav");
        }
    }   
}
