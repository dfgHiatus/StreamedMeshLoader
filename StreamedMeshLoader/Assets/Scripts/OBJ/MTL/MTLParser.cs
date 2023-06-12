
using MTLImporter;
using System;
using UdonSharp;
using UnityEngine;

/// <summary>
/// A helper class that parses an MTL file and creates a material for each material in the file
/// This will not import images
/// </summary>
[RequireComponent(typeof(MTLUtils))]
public class MTLParser : UdonSharpBehaviour
{
    [SerializeField] private MTLUtils mtlUtils;
    [SerializeField] private MTLMaterial mtlMat;

    /// <summary>
    /// Converts an MTL file to a material. Supports only 1 MTL at a time
    /// </summary>
    /// <param name="mtlData"></param>
    /// <returns></returns>
    public MTLMaterial ImportMTL(string mtlData)
    {
        // https://stackoverflow.com/questions/65201192/read-from-file-split-content-into-group-when-empty-line
        foreach (var line in mtlData.Split('\n'))
        {
            var lineSplit = line.Split(' ');
            string[] colorArray = new string[lineSplit.Length - 1];
            Array.Copy(lineSplit, 1, colorArray, 0, lineSplit.Length - 1);

            foreach (var item in lineSplit) Debug.Log("LineSplit Item: " + item);

            switch (lineSplit[0])
            {
                case "illum":
                    mtlMat.Illum = mtlUtils.ToInt(lineSplit[1]);
                    break;

                case "d":
                    mtlMat.Alpha = mtlUtils.ToFloat(lineSplit[1]);
                    break;
                case "Tr":
                    mtlMat.NonAlpha = mtlUtils.ToFloat(lineSplit[1]);
                    break;

                case "Ka":
                    mtlMat.AmbientColor = mtlUtils.ToColor(colorArray);
                    break;
                case "map_Ka":
                    // mtlMat.ambientMap = lineSplit[1];
                    break;

                case "Kd":
                    mtlMat.DiffuseColor = mtlUtils.ToColor(colorArray);
                    break;
                case "map_Kd":
                    //mtlMat.diffuseMap = lineSplit[1];
                    break;

                case "norm":
                case "bump":
                case "map_bump":
                    //mtlMat.normalMap = lineSplit[1];
                    break;

                case "disp":
                    // case "height":
                    // case "map_disp":
                    // case "map_height":
                    //mtlMat.heightMap = lineSplit[1];
                    break;

                case "Ke":
                    mtlMat.EmissionColor = mtlUtils.ToColor(colorArray);
                    break;
                case "map_Ke":
                    //mtlMat.emissionMap = lineSplit[1];
                    break;

                case "Ns":
                case "Pr":
                    mtlMat.Roughness = mtlUtils.ToFloat(lineSplit[1]);
                    break;
                case "map_Pr":
                    //mtlMat.roughnessMap = lineSplit[1];
                    break;

                case "Pm":
                    mtlMat.Metallic = mtlUtils.ToFloat(lineSplit[1]);
                    break;
                case "map_Pm":
                    //mtlMat.metallicMap = lineSplit[1];
                    break;

                case "Ks":
                    mtlMat.SpecularColor = mtlUtils.ToColor(colorArray);
                    break;
                case "map_Ks":
                    //mtlMat.specularMap = lineSplit[1];
                    break;

                case "Tf":
                    Debug.LogWarning($"Transparent Color is redundant. MTL importer will not import this.");
                    break;
                case "Ni":
                    Debug.LogWarning($"Index of Refraction imports are not supported. MTL importer will not import this.");
                    break;

                default:
                    Debug.LogWarning($"MTL definition was not found: {lineSplit[0]}");
                    break;
            }
        }

        return mtlMat;
    }
}