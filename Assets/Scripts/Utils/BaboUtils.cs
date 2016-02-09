using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class BaboUtils
{
    public static string GetMd5Hash(string input)
    {
        // Convert the input string to a byte array and compute the hash.
        byte[] data = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        StringBuilder sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data 
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
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
		return new Vector3 (array [0], array [2], array [1]);
	}

    public static Vector3 vectorFromArray(float[] array)
    {
		return new Vector3(array[0], array[2], array[1]);
    }

    public static Vector3 vectorFromArray(byte[] array) {
		return new Vector3 (array [0], array [2], array [1]);
	}

    public static string bytesToString(byte[] bytes)
    {
        return Encoding.ASCII.GetString(bytes);
    }

    public static byte[] stringToBytes(string str)
    {
        return Encoding.ASCII.GetBytes(str);
    }

    public static Color fromBaboColor(byte[] colorArray)
    {
        return new Color(
                ((float)colorArray[0]) / 255.0f,
                ((float)colorArray[1]) / 255.0f,
                ((float)colorArray[2]) / 255.0f);
    }

    public static byte[] toBaboColor(Color color)
    {
        byte[] r = new byte[3];
        r[0] = (byte)(color.r * 255f);
        r[1] = (byte)(color.g * 255f);
        r[2] = (byte)(color.b * 255f);

        return r;
    }

}
