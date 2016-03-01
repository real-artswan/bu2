using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class BaboUtils
{
    public static void Log(string msg, params object[] args) {
        if (!Debug.isDebugBuild)
            return;
        Debug.LogFormat(DateTime.Now.ToString("hh:mm:ss.fff") + ": " + msg, args);
    }
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

    private static string[] TEXT_COLOR_MAP = new string[10]
    {
        "4040ff", "40ff40", "40ffff", "ff4040", "ff40ff", "ffb300", "808080",
        "ffffff", "ffff00", "00ff00"
    };
    private static string COLOR_TEMPLATE_OPEN = "<color=#{0}>";
    private static string COLOR_TEMPLATE_CLOSE = "</color>";
    public static string baboBytesToString(byte[] bytes, bool rtf) {
        string str = Encoding.ASCII.GetString(bytes);
        StringBuilder sb = new StringBuilder(str.Length);
        if (!rtf) {
            foreach (Char ch in str) {
                if ((int)ch > 10)
                    sb.Append(ch);
                else {
                    if ((int)ch == 0)
                        break;
                }
            }
        }
        else {
            string tail = "";
            foreach (Char ch in str) {
                if ((int)ch > 10)
                    sb.Append(ch);
                else {
                    if (tail.Length != 0)
                        sb.Append(tail);
                    if ((int)ch == 0)
                        break;
                    sb.Append(string.Format(COLOR_TEMPLATE_OPEN, TEXT_COLOR_MAP[(int)ch - 1]));
                    tail = COLOR_TEMPLATE_CLOSE;
                }
            }
        }
        return sb.ToString();
    }

    //TODO: support babo colors (convert unity richtext to babo colors)
    public static byte[] stringToBaboBytes(string str, bool rtf) {
        return Encoding.ASCII.GetBytes(str);
    }

    public static Color32 fromBaboColor(byte[] colorArray) {
        return new Color32(colorArray[0], colorArray[1], colorArray[2], 255);
    }

    public static byte[] toBaboColor(Color32 color) {
        byte[] r = new byte[3];
        r[0] = color.r;
        r[1] = color.g;
        r[2] = color.b;

        return r;
    }

    public static Color getTeamColor(BaboPlayerTeamID teamID, float alpha = 1) {
        switch (teamID) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                return new Color(0.3f, 0.3f, 1, alpha);
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                return new Color(1, 0, 0, alpha);
            default:
                return new Color(1, 1, 1);
        }
    }

    public static string getTeamName(BaboPlayerTeamID team) {
        switch (team) {
            case BaboPlayerTeamID.PLAYER_TEAM_BLUE:
                return l10n.blueTeam;
            case BaboPlayerTeamID.PLAYER_TEAM_RED:
                return l10n.redTeam;
            case BaboPlayerTeamID.PLAYER_TEAM_AUTO_ASSIGN:
                return l10n.freeForAll;
            default:
                return l10n.specTeam;
        }
    }

    public static void fixScaledMeshUVs(Mesh mesh, Vector3 scale) {
        float x = scale.x;
        float z = scale.z;
        float y = scale.y;
        Vector2[] newUV = mesh.uv;
        //Back
        newUV[0] = new Vector2(0, 0);
        newUV[1] = new Vector2(x, 0);
        newUV[2] = new Vector2(0, y);
        newUV[3] = new Vector2(x, y);
        //Front
        newUV[7] = new Vector2(0, 0);
        newUV[6] = new Vector2(x, 0);
        newUV[11] = new Vector2(0, y);
        newUV[10] = new Vector2(x, y);
        //Left
        newUV[16] = new Vector2(0, 0);
        newUV[19] = new Vector2(z, 0);
        newUV[17] = new Vector2(0, y);
        newUV[18] = new Vector2(z, y);
        //Right
        newUV[20] = new Vector2(0, 0);
        newUV[23] = new Vector2(z, 0);
        newUV[21] = new Vector2(0, y);
        newUV[22] = new Vector2(z, y);
        //Top
        newUV[5] = new Vector2(0, 0);
        newUV[4] = new Vector2(x, 0);
        newUV[9] = new Vector2(0, z);
        newUV[8] = new Vector2(x, z);
        //Bottom
        newUV[15] = new Vector2(0, 0);
        newUV[12] = new Vector2(x, 0);
        newUV[14] = new Vector2(0, z);
        newUV[13] = new Vector2(x, z);
        //apply new UV map
        mesh.uv = newUV;
    }
}
