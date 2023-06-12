using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

/// <summary>
/// Base class for streamed mesh loaders. Provides a static helper utility class for mesh operations.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class StreamedMeshLoader : UdonSharpBehaviour //, IDownloadable
{
    [HideInInspector] public string MeshData;
    [HideInInspector] public MeshRenderer MeshRenderer;

    [SerializeField] protected MeshFilter meshFilter;
    [SerializeField] protected Material[] materials;
    
    protected UdonBehaviour stringDownloader;
    protected StreamedMeshUtils meshUtils;

    [SerializeField] private GameObject stringDownloaderGameobject;
    [SerializeField] private GameObject _staticMeshUtils;

    protected virtual void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        MeshRenderer = GetComponent<MeshRenderer>();
        stringDownloader = stringDownloaderGameobject.GetComponent<UdonBehaviour>();
        MeshRenderer.materials = materials;
        CreateMeshUtilsSingleton();
    }

    public virtual void GenerateMesh()
    {
        Debug.LogError("This method must be overridden, and assign meshFilter.mesh to the output of this function!");
    }

    /// <summary>
    /// Provides a reusable method for downloading a string from a URL.
    /// </summary>
    /// <param name="uri">A VRCUrl to pull a resource from</param>
    /// <param name="localTarget">A public string to store the return data in</param>
    /// <param name="returnMethodName">A public method that is called when this strings downloads successfully</param>
    protected void DownloadString(VRCUrl uri, string localTarget, string returnMethodName)
    {
        Debug.Log("Url" + stringDownloader.GetProgramVariable("Url"));
        Debug.Log("StreamedMeshLoaderMeshData" + stringDownloader.GetProgramVariable("StreamedMeshLoaderMeshData"));
        Debug.Log("StreamedMeshLoaderOnLoadedEventName" + stringDownloader.GetProgramVariable("StreamedMeshLoaderOnLoadedEventName"));

        stringDownloader.SetProgramVariable("Url", uri);
        stringDownloader.SetProgramVariable("StreamedMeshLoaderMeshData", localTarget);
        stringDownloader.SetProgramVariable("StreamedMeshLoaderOnLoadedEventName", returnMethodName);
        stringDownloader.SendCustomNetworkEvent(NetworkEventTarget.All, "OnCustomMeshRequested");
    }

    private void CreateMeshUtilsSingleton()
    {
        GameObject canidateMeshUtils = GameObject.Find("Static Streamed Mesh Utils(Clone)");

        if (canidateMeshUtils == null)
        {
            meshUtils = Instantiate(_staticMeshUtils).GetComponent<StreamedMeshUtils>();
            meshUtils.transform.SetParent(null);
            meshUtils.transform.SetAsFirstSibling();
            return;
        }

        meshUtils = canidateMeshUtils.GetComponent<StreamedMeshUtils>();
    }
}
