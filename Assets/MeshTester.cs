using UnityEngine;
using System.Collections;

public class MeshTester : MonoBehaviour
{
    public enum MeshType
    {
        Flat,
        Smooth,
        Line
    }
    ;

    public MeshType meshType;

    void BuildAndReplaceMesh (Emgen.IcosphereBuilder builder)
    {
        var meshFilter = GetComponent<MeshFilter> ();

        if (meshFilter.sharedMesh != null) {
            Destroy (meshFilter.sharedMesh);
        }
        
        switch (meshType) {
        case MeshType.Flat:
            meshFilter.sharedMesh = builder.vertexCache.BuildFlatMesh ();
            break;
        case MeshType.Smooth:
            meshFilter.sharedMesh = builder.vertexCache.BuildSmoothMesh ();
            break;
        case MeshType.Line:
            meshFilter.sharedMesh = builder.vertexCache.BuildLineMesh ();
            break;
        }
    }

    IEnumerator Start ()
    {
        GetComponent<MeshFilter> ().sharedMesh = null;
        while (true) {
            var builder = new Emgen.IcosphereBuilder ();
            BuildAndReplaceMesh (builder);
            for (var i = 0; i < 5; i++) {
                yield return new WaitForSeconds (0.5f);
                builder.Subdivide ();
                BuildAndReplaceMesh (builder);
            }
        }
    }
}