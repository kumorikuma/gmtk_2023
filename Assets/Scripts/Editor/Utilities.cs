using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public struct Setting<ValueType> {
    public ValueType value;
    public string tooltip;

    public Setting(ValueType _value, string _tooltip) {
        value = _value;
        tooltip = _tooltip;
    }
}

// Misc utility functions
public class Utilities {
    // Creates a mesh from mesh data, and spawns it in the world
    public static GameObject SpawnMesh(string name, Vector3[] vertices, Vector2[] uvs, int[] triangles, Transform parent = null, Material material = null) {
        GameObject meshObject = new GameObject(name);
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>() as MeshFilter;
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>() as MeshRenderer;
        Mesh mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        // mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
        meshRenderer.material = material;

        if (parent != null) {
            meshObject.transform.SetParent(parent);
        }

        return meshObject;
    }
}
