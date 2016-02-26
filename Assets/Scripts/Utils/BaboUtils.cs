using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class BaboUtils
{
    public static Vector3 randomInsideBox(Vector3 from, Vector3 to) {
        return new Vector3(
            UnityEngine.Random.Range(from.x, to.x),
            UnityEngine.Random.Range(from.y, to.y),
            UnityEngine.Random.Range(from.z, to.z)
            );
    }
    public static Color randomInsideBox(Color from, Color to) {
        return new Color(
            UnityEngine.Random.Range(from.r, to.r),
            UnityEngine.Random.Range(from.g, to.g),
            UnityEngine.Random.Range(from.b, to.b),
            UnityEngine.Random.Range(from.a, to.a)
            );
    }

    public static float SignedAngle(Vector3 a, Vector3 b) {
        var angle = Vector3.Angle(a, b); // calculate angle
                                         // assume the sign of the cross product's Y component:
        return angle * Mathf.Sign(Vector3.Cross(a, b).y);
    }

    internal static string getRoundStatus(BaboRoundState baboRoundState) {
        switch (baboRoundState) {
            case BaboRoundState.GAME_PLAYING:
                return l10n.gamePlaying;
            case BaboRoundState.GAME_BLUE_WIN:
                return String.Format(l10n.teamWin, getTeamName(BaboPlayerTeamID.PLAYER_TEAM_BLUE));
            case BaboRoundState.GAME_RED_WIN:
                return String.Format(l10n.teamWin, getTeamName(BaboPlayerTeamID.PLAYER_TEAM_BLUE)); ;
            case BaboRoundState.GAME_DRAW:
                return l10n.gameDraw;
            case BaboRoundState.GAME_MAP_CHANGE:
                return l10n.gameMapChange;
            case BaboRoundState.GAME_DONT_SHOW:
                return l10n.roundRestarting;
            default:
                return "";
        }
    }

    public static string GetMd5Hash(string input) {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++) {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    public static string GetMac() {
        string res = "";
        Guid guid = Guid.NewGuid();
        byte[] mac = guid.ToByteArray();
        for (int i = 0; i < 7; i++)
            res += mac[i].ToString("x") + "-";
        res += mac[8].ToString("x");
        return res;
    }

    public static Vector3 fromBaboPosition(short[] baboPos, float wShift, float hShift) {
        return new Vector3((float)baboPos[0] / 100f - wShift, (float)baboPos[2] / 100f, (float)baboPos[1] / 100f - hShift);
    }

    public static Vector3 fromBaboPosition(float[] baboPos, float wShift, float hShift) {
        return new Vector3(baboPos[0] - wShift, baboPos[2], baboPos[1] - hShift);
    }

    public static Vector3 vectorFromArray(short[] array) {
        return new Vector3(array[0], array[2], array[1]);
    }

    public static Vector3 vectorFromArray(float[] array) {
        return new Vector3(array[0], array[2], array[1]);
    }

    public static Vector3 vectorFromArray(byte[] array) {
        return new Vector3(array[0], array[2], array[1]);
    }

    public static Vector3 vectorFromArray(sbyte[] array) {
        return new Vector3(array[0], array[2], array[1]);
    }

    //TODO: support babo colors(convert babo colors to unity richtext)
    public static string baboBytesToString(byte[] bytes, bool rtf) {
        return Encoding.ASCII.GetString(bytes);
    }

    //TODO: support babo colors (convert unity richtext to babo colors)
    public static byte[] stringToBaboBytes(string str, bool rtf) {
        return Encoding.ASCII.GetBytes(str);
    }

    public static Color fromBaboColor(byte[] colorArray) {
        return new Color(
                ((float)colorArray[0]) / 255.0f,
                ((float)colorArray[1]) / 255.0f,
                ((float)colorArray[2]) / 255.0f);
    }

    public static byte[] toBaboColor(Color color) {
        byte[] r = new byte[3];
        r[0] = (byte)(color.r * 255f);
        r[1] = (byte)(color.g * 255f);
        r[2] = (byte)(color.b * 255f);

        return r;
    }

    internal static Color getTeamColor(BaboPlayerTeamID teamID, float alpha = 1) {
        switch (teamID) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                return new Color(0.3f, 0.3f, 1, alpha);
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                return new Color(1, 0, 0, alpha);
            default:
                return new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
    }

    internal static string getTeamName(BaboPlayerTeamID team) {
        switch (team) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                return l10n.blueTeam;
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                return l10n.redTeam;
            default:
                return l10n.specTeam;
        }
    }
}
