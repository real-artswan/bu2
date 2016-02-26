public class Voting
{
    internal bool inProgress = false;
    internal int yes = 0;
    internal int no = 0;
    internal string what;
    internal byte playerID;

    public void reset() {
        inProgress = false;
        yes = 0;
        no = 0;
        what = "";
        playerID = 255;
    }
}