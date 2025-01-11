using UdonSharp;
using UnityEngine;

namespace MTLImporter
{
    // https://people.sc.fsu.edu/~jburkardt/data/mtl/mtl.html
    // https://steamcommunity.com/sharedfiles/filedetails/?id=2005695630
    public class MTLMaterial : UdonSharpBehaviour
    {
        public Color DiffuseColor = new Color(0.8f, 0.8f, 0.8f);
        public Color AmbientColor = new Color(0.2f, 0.2f, 0.2f);
        public Color EmissionColor = new Color(0.0f, 0.0f, 0.0f);
        public Color SpecularColor = new Color(0.0f, 0.0f, 0.0f, 0.25f);
        public float Alpha = 1.0f;
        public float Metallic = 0.0f;
        private float _roughness = 0.75f;
        public bool IsMetallic = true;

        //public string diffuseMap = string.Empty;
        //public string ambientMap = string.Empty;
        //public string emissionMap = string.Empty;
        //public string normalMap = string.Empty;
        //public string heightMap = string.Empty;
        //public string metallicMap = string.Empty;
        //public string specularMap = string.Empty;
        //public string roughnessMap = string.Empty;

        public float NonAlpha { set => Alpha = Mathf.Max(Alpha, 1 - value); }

        public float Roughness
        {
            set => _roughness = (100.0f - value) / 100.0f;
            get => _roughness;
        }

        public int Illum
        {
            set
            {
                IsMetallic = ConvertFromMTL(value);
            }
        }

        public bool ConvertFromMTL(int index)
        {
            switch (index)
            {
                case 0:
                case 1:
                case 2:
                case 4:
                case 6:
                case 7:
                case 9:
                    return false;
                case 3:
                case 5:
                case 8:
                    return true;
                default:
                    return false;
            }
        }
    }
}