using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// This editor script listens for the `playModeStateChanged` event and checks all MonoBehaviour instances for fields marked with the `[NonNullField]` attribute. 
// If any of these fields are null, it logs an error message in the console with the relevant GameObject and field name.
[InitializeOnLoad]
public class NonNullFieldChecker {
    static NonNullFieldChecker() {
        EditorApplication.playModeStateChanged += CheckNonNullFields;
    }

    private static void CheckNonNullFields(PlayModeStateChange state) {
        if (state == PlayModeStateChange.EnteredPlayMode) {
            MonoBehaviour[] allMonoBehaviours = GameObject.FindObjectsOfType<MonoBehaviour>();

            foreach (var monoBehaviour in allMonoBehaviours) {
                IEnumerable<FieldInfo> nonNullFields = GetNonNullFields(monoBehaviour.GetType());

                foreach (var field in nonNullFields) {
                    object fieldValue = field.GetValue(monoBehaviour);
                    if (fieldValue == null) {
                        Debug.LogError($"[NonNullChecker] {monoBehaviour.name}: {field.Name} is not assigned a valid value!", monoBehaviour);
                    }
                }
            }
        }
    }

    private static IEnumerable<FieldInfo> GetNonNullFields(System.Type type) {
        FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        List<FieldInfo> nonNullFields = new List<FieldInfo>();

        foreach (var field in fields) {
            if (field.GetCustomAttribute(typeof(NonNullFieldAttribute)) != null) {
                nonNullFields.Add(field);
            }
        }

        return nonNullFields;
    }
}