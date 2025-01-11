using MTLImporter;
using UnityEngine;
using VRC.SDKBase;

/// <summary>
/// This implementation of StreamedMeshLoader allows for the loading of OBJ files from a URL. Requires a DownloadString component to be attached to the same gameobject.
/// </summary>
public class StreamedOBJLoader : StreamedMeshLoader
{
    /// <summary>
    /// Contains the parsed MTL material, which can be mapped to unity shaders
    /// </summary>
    [HideInInspector] public MTLMaterial ParsedMTLMaterial;

    [SerializeField] private GameObject staticOBJParserPrefab;
    [SerializeField] private GameObject staticMTLParserPrefab;
    [SerializeField] private GameObject shaderMapperUtility;
    [SerializeField] private VRCUrl mtlURL;

    private OBJParser _staticOBJParser;
    private MTLParser _staticMTLParser;
    private ShaderMapper _staticShaderMapper;
    private int _counter = 0;

    protected override void Start()
    {
        base.Start();
        CreateOBJHelperSingleton();
    }

    /* This doesn't seem to find the OBJ Parser in the scene:
     * 
     * var canidateOBJParser = transform.root.GetComponentInChildren<OBJParser>();
     *
     * Moving the gameobject to be the first sibling might break things in the future.
     * Either way, the below works for now:
     */
    private void CreateOBJHelperSingleton()
    {
        GameObject canidateOBJParser = GameObject.Find("Static OBJ Parser(Clone)");
        if (canidateOBJParser == null)
        {
            var go = Instantiate(staticOBJParserPrefab);
            go.transform.SetParent(null);
            go.transform.SetAsFirstSibling(); 
            _staticOBJParser = go.GetComponent<OBJParser>();
        }
        else
        {
            _staticOBJParser = canidateOBJParser.GetComponent<OBJParser>();
        }

        GameObject canidateMTLParser = GameObject.Find("Static MTL Parser(Clone)");
        if (canidateMTLParser == null)
        {
            _staticMTLParser = Instantiate(staticMTLParserPrefab).GetComponent<MTLParser>();
            _staticMTLParser.transform.SetParent(null);
            _staticMTLParser.transform.SetAsFirstSibling();
        }
        else
        {
            _staticMTLParser = canidateOBJParser.GetComponent<MTLParser>();
        }


        // This can be left empty to target a standard shader
        if (shaderMapperUtility == null) return;

        GameObject canidateShaderMapper = GameObject.Find("Shader Mapper(Clone)");
        if (canidateMTLParser == null)
        {
            _staticShaderMapper = Instantiate(staticMTLParserPrefab).GetComponent<ShaderMapper>();
            _staticShaderMapper.transform.SetParent(null);
            _staticShaderMapper.transform.SetAsFirstSibling();
        }
        else
        {
            _staticShaderMapper = canidateShaderMapper.GetComponent<ShaderMapper>();
        }
    }

    public override void GenerateMesh()
    {
        if (_staticOBJParser == null)
        {
            Debug.LogError("Static OBJ Parser not found on root!");
            return;
        }

        _staticOBJParser.ImportOBJ(MeshData);
        _staticOBJParser.StartUdonCoroutine(this);
    }

    public override void OnCompletion(BaseUdonCoroutine sender)
    {
        meshFilter.mesh = _staticOBJParser.Mesh;
        DownloadString(mtlURL, "MeshData", "OnMTLDownloaded");
    }

    public void OnMTLDownloaded()
    {
        Debug.Log("MTL Downloaded, with a material index of " + _counter);
        ParsedMTLMaterial = _staticMTLParser.ImportMTL(MeshData);

        if (_staticShaderMapper == null)
        {
            // In the event no custom shader mapper is provided, we'll just set a standard material
            MeshRenderer.material.color = new Color(
                ParsedMTLMaterial.DiffuseColor.r,
                ParsedMTLMaterial.DiffuseColor.g,
                ParsedMTLMaterial.DiffuseColor.b,
                ParsedMTLMaterial.Alpha);
            MeshRenderer.material.SetColor("_EmissionColor", ParsedMTLMaterial.EmissionColor);
            MeshRenderer.material.SetFloat("_Metallic", ParsedMTLMaterial.Metallic);
            MeshRenderer.material.SetFloat("_Glossiness", ParsedMTLMaterial.Roughness);
            return;
        }

        _staticShaderMapper.MapMTLToShader(ParsedMTLMaterial, MeshRenderer.material);
    }
}
