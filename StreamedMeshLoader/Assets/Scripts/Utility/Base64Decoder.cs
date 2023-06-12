
using System;
using System.Text;
using UdonSharp;
using UnityEngine;

public class Base64Decoder : UdonSharpBehaviour
{
    /// <summary>
    /// Decodes a base64 string into a UTF-8/16 string
    /// </summary>
    /// <param name="payload">The base64 to parse</param>
    /// <param name="UTF16">Use UTF-16 encoding instead if UTf-8</param>
    /// <returns></returns>
    public string DecodeBase64(string payload, bool UTF16 = false)
    {
        byte[] data = Convert.FromBase64String(payload);
        // return Encoding.UTF8.GetString(data); // Why is this not supported?

        if (UTF16)
        {
            return DecodeUtf16String(data);
        }
        else
        {
            return DecodeUtf8ByteArray(data);
        }
    }

    public string DecodeUtf8ByteArray(byte[] byteArray)
    {
        int byteCount = byteArray.Length;
        char[] charArray = new char[byteCount];
        int charIndex = 0;

        int byteIndex = 0;
        while (byteIndex < byteCount)
        {
            byte currentByte = byteArray[byteIndex];

            if ((currentByte & 0x80) == 0)
            {
                // Single-byte character
                charArray[charIndex++] = (char)currentByte;
                byteIndex++;
            }
            else if ((currentByte & 0xE0) == 0xC0)
            {
                // Two-byte character
                if (byteIndex + 1 >= byteCount)
                {
                    Debug.LogError("Invalid UTF-8 byte sequence.");
                    return string.Empty;
                }

                byte nextByte = byteArray[byteIndex + 1];
                if ((nextByte & 0xC0) != 0x80)
                {
                    Debug.LogError("Invalid UTF-8 byte sequence.");
                    return string.Empty;
                }

                charArray[charIndex++] = (char)(((currentByte & 0x1F) << 6) | (nextByte & 0x3F));
                byteIndex += 2;
            }
            else if ((currentByte & 0xF0) == 0xE0)
            {
                // Three-byte character
                if (byteIndex + 2 >= byteCount)
                {
                    Debug.LogError("Invalid UTF-8 byte sequence.");
                    return string.Empty;
                }

                byte nextByte1 = byteArray[byteIndex + 1];
                byte nextByte2 = byteArray[byteIndex + 2];
                if ((nextByte1 & 0xC0) != 0x80 || (nextByte2 & 0xC0) != 0x80)
                    Debug.LogError("Invalid UTF-8 byte sequence.");

                charArray[charIndex++] = (char)(((currentByte & 0x0F) << 12) | ((nextByte1 & 0x3F) << 6) | (nextByte2 & 0x3F));
                byteIndex += 3;
            }
            else
            {
                Debug.LogError("Invalid UTF-8 byte sequence.");
            }
        }

        return new string(charArray, 0, charIndex);
    }

    public string DecodeUtf16String(byte[] byteArray)
    {
        if (byteArray.Length % 2 != 0)
        {
            Debug.LogError("Invalid UTF-16 byte array length.");
            return string.Empty;
        }

        int charCount = byteArray.Length / 2;
        char[] charArray = new char[charCount];
        int charIndex = 0;

        for (int i = 0; i < byteArray.Length; i += 2)
        {
            byte lowByte = byteArray[i];
            byte highByte = byteArray[i + 1];
            charArray[charIndex++] = (char)((highByte << 8) | lowByte);
        }

        return new string(charArray);
    }
}
