using UnityEngine;
using UnityEngine.UI;

public class IPInputValidation : MonoBehaviour
{

    // Use this for initialization
    void Start() {
        GetComponent<InputField>().onValidateInput += validateIP;
    }

    private bool isInt(char c) {
        int code = (int)c;
        return ((code >= 48) && (code <= 57));
    }

    //TODO
    //255.255.255.255 | 1.0.0.1
    private char validateIP(string input, int charIndex, char addedChar) {
        if ((charIndex == 0) && (addedChar == '0') || ((addedChar != '.') && (!isInt(addedChar))))
            return (char)0;
        return addedChar;
    }
}
