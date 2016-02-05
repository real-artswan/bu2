using System;
using System.Security.Cryptography;
using System.Text;

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

}
