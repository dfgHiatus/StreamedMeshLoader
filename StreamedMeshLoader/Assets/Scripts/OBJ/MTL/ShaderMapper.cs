
using MTLImporter;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ShaderMapper : UdonSharpBehaviour
{
    [SerializeField] private string diffuseColor;
    [SerializeField] private string ambientColor;
    [SerializeField] private string emissionColor;
    [SerializeField] private string specularColor;
    [SerializeField] private string alpha;
    [SerializeField] private string metallic;
    [SerializeField] private string roughness;

    public void MapMTLToShader(MTLMaterial mtlMaterial, Material mat)
    {
        mat.SetColor(diffuseColor, mtlMaterial.DiffuseColor);
        mat.SetColor(ambientColor, mtlMaterial.AmbientColor);
        mat.SetColor(emissionColor, mtlMaterial.EmissionColor);
        mat.SetColor(specularColor, mtlMaterial.SpecularColor);
        mat.SetFloat(alpha, mtlMaterial.Alpha);
        mat.SetFloat(metallic, mtlMaterial.Metallic);
        mat.SetFloat(roughness, mtlMaterial.Roughness);
    }
}
