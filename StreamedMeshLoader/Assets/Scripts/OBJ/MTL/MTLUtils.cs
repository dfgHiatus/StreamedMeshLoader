using UdonSharp;
using UnityEngine;

namespace MTLImporter
{
    public class MTLUtils : UdonSharpBehaviour
    {
        public int ToInt(string line)
        {
            int canidate = 0;
            if (!int.TryParse(line, out canidate))
            {
                Debug.LogError("Failed to parse int value: " + line);
            }
            return canidate;
        }

        public float ToFloat(string line)
        {
            float canidate = 0;
            if (!float.TryParse(line, out canidate))
            {
                Debug.LogError("Failed to parse float value: " + line);
            }
            return Mathf.Clamp01(canidate);
        }

        public Color ToColor(string[] line)
        {
            float r = 0f;
            float g = 0f;
            float b = 0f;
            if (!float.TryParse(line[0], out r))
            {
                Debug.LogError("Failed to parse color value (R): " + line[0]);
            }
            if (!float.TryParse(line[1], out g))
            {
                Debug.LogError("Failed to parse color value (G): " + line[1]);
            }
            if (!float.TryParse(line[2], out b))
            {
                Debug.LogError("Failed to parse color value (B): " + line[2]);
            }
            return new Color(r, g, b);
        }
    }
}
