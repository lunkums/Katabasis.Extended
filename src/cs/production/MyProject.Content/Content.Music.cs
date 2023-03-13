using bottlenoselabs.Katabasis;

namespace MyProject;

public static partial class Content
{
    public static class Music
    {
        public static Song DnaLoop8Bit = null!;

        internal static void LoadMusic()
        {
            DnaLoop8Bit = Song.FromFile(string.Empty, "assets/music/dna_loop.ogg");
        }
    }   
}
