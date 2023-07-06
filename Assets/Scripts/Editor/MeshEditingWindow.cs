using System.Collections;
using Unity.EditorCoroutines.Editor;
using UnityEngine;
using UnityEditor;

// Authored by Francis Ge: https://github.com/kumorikuma
// UnityEditor window with options for editing mesh.
// Accessed from toolbar: Custom -> Mesh Editing
public class MeshEditingWindow : EditorWindow {
    MeshFilter SourceMesh; // Mesh to edit and perform changes onto
    MeshFilter TargetMesh; // Mesh to use as data

    [MenuItem("Custom/Mesh Editing")]
    public static void OpenWindow() {
        GetWindow<MeshEditingWindow>();
    }

    void OnGUI() {
        SourceMesh = EditorGUILayout.ObjectField("Mesh", SourceMesh, typeof(MeshFilter), true) as MeshFilter;

        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("Blendshape");
        GUILayout.BeginVertical("GroupBox");
        TargetMesh = EditorGUILayout.ObjectField("Target Mesh", TargetMesh, typeof(MeshFilter), true) as MeshFilter;
        if (GUILayout.Button("Add target mesh as blendshape")) {
            this.StartCoroutine(AddMeshAsBlendshape(TargetMesh, SourceMesh));
        }
        GUILayout.EndVertical();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("HelpBox");
        GUILayout.Label("Mesh Properties");
        GUILayout.BeginVertical("GroupBox");
        if (GUILayout.Button("Recompute Normals")) {
            this.StartCoroutine(RecalculateNormals(SourceMesh));
        }
        if (GUILayout.Button("Add barycentric coords to verts")) {
            this.StartCoroutine(GenerateBarycentricCoordinates(SourceMesh));
        }
        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    IEnumerator RecalculateNormals(MeshFilter sourceMesh) {
        sourceMesh.sharedMesh.RecalculateNormals();
        yield return null;
    }

    // For each triangle, generates duplicates vertices that have barycentric coordinates set
    // as vertex attributes for interpolation in shader.
    IEnumerator GenerateBarycentricCoordinates(MeshFilter sourceMesh) {
        Vector3[] sourceVerts = sourceMesh.sharedMesh.vertices;
        Vector2[] sourceUvs = sourceMesh.sharedMesh.uv;
        int[] sourceTriangles = sourceMesh.sharedMesh.triangles;

        Debug.Log($"UVS {sourceUvs.Length}");
        Debug.Log($"Verts {sourceVerts.Length}");

        Vector3[] newVerts = new Vector3[sourceVerts.Length * 3];
        Vector3[] newUvs = new Vector3[sourceVerts.Length * 3];
        Vector3[] barycentricCoordinates = new Vector3[sourceVerts.Length * 3];
        int[] newTriangles = new int[sourceTriangles.Length];

        int vertIdx = 0;
        int triIdx = 0;
        // For each triangle, generate three vertices
        for (int i = 0; i < sourceTriangles.Length; i += 3) {
            int vertA = sourceTriangles[i];
            int vertB = sourceTriangles[i + 1];
            int vertC = sourceTriangles[i + 2];

            newTriangles[triIdx++] = vertIdx;
            newVerts[vertIdx] = sourceVerts[vertA];
            if (sourceUvs.Length > 0) {
                newUvs[vertIdx] = sourceUvs[vertA];
            }
            barycentricCoordinates[vertIdx] = new Vector3(1, 0, 0);
            vertIdx++;

            newTriangles[triIdx++] = vertIdx;
            newVerts[vertIdx] = sourceVerts[vertB];
            if (sourceUvs.Length > 0) {
                newUvs[vertIdx] = sourceUvs[vertB];
            }
            barycentricCoordinates[vertIdx] = new Vector3(0, 1, 0);
            vertIdx++;

            newTriangles[triIdx++] = vertIdx;
            newVerts[vertIdx] = sourceVerts[vertC];
            if (sourceUvs.Length > 0) {
                newUvs[vertIdx] = sourceUvs[vertC];
            }
            barycentricCoordinates[vertIdx] = new Vector3(0, 0, 1);
            vertIdx++;
        }

        sourceMesh.sharedMesh.vertices = newVerts;
        if (sourceUvs.Length > 0) {
            sourceMesh.sharedMesh.SetUVs(0, newUvs);
        }
        sourceMesh.sharedMesh.SetUVs(1, barycentricCoordinates);
        sourceMesh.sharedMesh.triangles = newTriangles;
        sourceMesh.sharedMesh.RecalculateNormals();
        sourceMesh.sharedMesh.RecalculateBounds();

        yield return null;
    }

    // Adds the selected mesh as a blendshape to the source mesh.
    // Only works if they have the same number of vertices.
    // Note: MeshRenderer using the Mesh needs to be changed to SkinnedMeshRenderer after.
    IEnumerator AddMeshAsBlendshape(MeshFilter sourceMesh, MeshFilter targetMesh) {
        Vector3[] sourceVerts = sourceMesh.mesh.vertices;
        Vector3[] targetVerts = targetMesh.mesh.vertices;
        if (sourceVerts.Length != targetVerts.Length) {
            Debug.LogError("Failed to add blendshape. Source mesh has different vertex count than target mesh.");
            yield break;
        }

        Vector3[] deltaVertices = new Vector3[sourceVerts.Length];
        for (int i = 0; i < deltaVertices.Length; i++) {
            deltaVertices[i] = sourceVerts[i] - targetVerts[i];
        }
        targetMesh.mesh.AddBlendShapeFrame("Blendshape", 100, deltaVertices, null, null);
        // Need to do this after adding blendshape.
        // See: https://forum.unity.com/threads/adding-new-blendshape-from-script-buggy-deformation-result-fixed.827187/ 
        targetMesh.mesh.RecalculateNormals();
        targetMesh.mesh.RecalculateTangents();
        yield return null;
    }
}