public static class l10n
{
    internal static string getRoundStatus(BaboRoundState baboRoundState) {
        switch (baboRoundState) {
            case BaboRoundState.GAME_PLAYING:
                return gamePlaying;
            case BaboRoundState.GAME_BLUE_WIN:
                return string.Format(teamWin, getTeamName(BaboPlayerTeamID.PLAYER_TEAM_BLUE));
            case BaboRoundState.GAME_RED_WIN:
                return string.Format(teamWin, getTeamName(BaboPlayerTeamID.PLAYER_TEAM_BLUE)); ;
            case BaboRoundState.GAME_DRAW:
                return gameDraw;
            case BaboRoundState.GAME_MAP_CHANGE:
                return gameMapChange;
            case BaboRoundState.GAME_DONT_SHOW:
                return roundRestarting;
            default:
                return "";
        }
    }

    public static string getTeamName(BaboPlayerTeamID team) {
        switch (team) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                return blueTeam;
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                return redTeam;
            case BaboPlayerTeamID.PLAYER_TEAM_AUTO_ASSIGN:
                return freeForAll;
            default:
                return specTeam;
        }
    }

    public static string getGameTypeName(BaboGameType gameType) {
        string res = "";
        switch (gameType) {
            case BaboGameType.GAME_TYPE_DM:
                res = deathmatch;
                break;
            case BaboGameType.GAME_TYPE_TDM:
                res = teamDeathmatch;
                break;
            case BaboGameType.GAME_TYPE_CTF:
                res = captureFlag;
                break;
            case BaboGameType.GAME_TYPE_SND:
                res = snd;
                break;
        }
        return res;
    }

    public static string getGameTypeRules(BaboGameType gameType) {
        string res = "";
        switch (gameType) {
            case BaboGameType.GAME_TYPE_DM:
                res = dmRules;
                break;
            case BaboGameType.GAME_TYPE_TDM:
                res = tdmRules;
                break;
            case BaboGameType.GAME_TYPE_CTF:
                res = ctfRules;
                break;
            case BaboGameType.GAME_TYPE_SND:
                res = sndRules;
                break;
        }
        return res;
    }
    public static string dmRules = "The player with the highest scores wins!";
    public static string tdmRules = "The team with the highest scores wins!";
    public static string ctfRules = "Capture the enemy's flag and return it to yours!";
    public static string sndRules = "";

    public static string deathmatch = "DEATHMATCH";
    public static string teamDeathmatch = "TEAM DEATHMATCH";
    public static string captureFlag = "CAPTURE THE FLAG";
    public static string snd = "SND";
    public static string playerDisconnected = "Player {0} disconnected";
    public static string serverDisconnected = "Server disconnected";
    public static string serverMessage = "Server-> team:{0} dest:{1} msg:{2}";
    public static string playerJoinedGame = "{0} joined the game";
    public static string menuGameInfo = "<b>{0}</b>\n{1}\n{2} created by {3}\n{4}";
    public static string playerChangedHisNameFor = "Player {0} has changed his name for {1}";
    public static string votePassed = "Vote passed";
    public static string voteFailed = "Vote failed";
    public static string addingPlayer = "Adding player {0}";
    public static string adminAccepted = "NOW ADMIN - ACCEPTED";
    public static object red = "Red";
    public static object blue = "Blue";
    public static string playerReturnedFlag = "Player {0} returned the {1} flag";
    public static string playerScoresFlag = "Player {0} scores for the {1} team";
    public static string playerTookFlag = "Player {0} took the {1} flag";
    public static string badCheckSumEntity = "{0}) {1}, IP: {1}";
    public static string badCheckSumInfo = ">> {0}";
    public static string redTeam = "RED TEAM";
    public static string blueTeam = "BLUE TEAM";
    public static string specTeam = "SPECTATORS";
    public static string freeForAll = "FREE FOR ALL";
    public static string playerName = "PLAYER NAME";
    public static string kills = "Kills";
    public static string death = "Death";
    public static string damage = "Damage";
    public static string retrns = "Return";
    public static string caps = "Caps";
    public static string ping = "PING";
    public static string playerDead = "(Dead)";
    public static string gamePlaying = "PLAYING";
    public static string teamWin = "{0} WIN";
    public static string gameDraw = "DRAW";
    public static string gameMapChange = "MAP IS CHANGING";
    public static string roundRestarting = "RESTARTING";
}
