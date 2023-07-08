using UnityEditor;
using UnityEngine;

namespace Kotorman
{
    namespace Rope
    {
#if UNITY_EDITOR
        [CustomEditor(typeof(Rope))]
        public class RopeEditor : Editor
        {
            SerializedProperty RopeNode;
            SerializedProperty AttachTo;
            SerializedProperty EndAttachTo;
            SerializedProperty NodeDistance;
            SerializedProperty BonesVisible;
            SerializedProperty Correction;
            //int collisionLayers = 0;
            SerializedProperty collisionLayers;

            [Min(1)]
            SerializedProperty NodesCount;
            SerializedProperty Collision;
            SerializedProperty SimulationIterations;
            SerializedProperty RotateNodes;
            SerializedProperty Gravity;
            SerializedProperty GravityForce;

            //foldouts
            bool physicsGroup = false;
            bool gravityGroup = false;

            //buttons
            bool buttonGenerate;

            //layersList
            string[] layersList;


            private void OnEnable()
            {
                InitLayers();

                RopeNode = serializedObject.FindProperty("ropeNode");
                AttachTo = serializedObject.FindProperty("attachTo");
                EndAttachTo = serializedObject.FindProperty("endAttachTo");
                NodeDistance = serializedObject.FindProperty("_nodeDistance");
                Correction = serializedObject.FindProperty("_correction");
                NodesCount = serializedObject.FindProperty("_totalNodes");
                collisionLayers = serializedObject.FindProperty("_collisionLayers");

                BonesVisible = serializedObject.FindProperty("_nodesVisible");
                Collision = serializedObject.FindProperty("_collision");
                RotateNodes = serializedObject.FindProperty("_rotateNodes");
                SimulationIterations = serializedObject.FindProperty("_simulationIterations");
                Gravity = serializedObject.FindProperty("_gravity");
                GravityForce = serializedObject.FindProperty("_gravityForce");
            }

            void InitLayers()
            {
                layersList = new string[32];
                for (int i = 0; i <= 31; i++) //user defined layers start with layer 8 and unity supports 31 layers
                {
                    string layerN = LayerMask.LayerToName(i); //get the name of the layer
                    if (layerN.Length > 0) //only add the layer if it has been named
                        layersList[i] = layerN;
                    else
                        layersList[i] = "<undefined>";
                }
            }


            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                Render();

                serializedObject.ApplyModifiedProperties();
            }

            private void Render()
            {
                //InitLayers();
                Rope ropeBehaviour = (Rope)target;


                EditorGUILayout.PropertyField(RopeNode);
                EditorGUILayout.PropertyField(AttachTo);
                EditorGUILayout.PropertyField(EndAttachTo);
                EditorGUILayout.PropertyField(NodeDistance);
                if (NodeDistance.floatValue < 0.001f) { NodeDistance.floatValue = 0.001f; } //(!&?)

                EditorGUI.BeginDisabledGroup(Application.isPlaying); 
                    EditorGUILayout.PropertyField(NodesCount);
                    if (NodesCount.intValue < 1) { NodesCount.intValue = 1; } //(!&?)
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(BonesVisible);
                ropeBehaviour.SetNodesVisible(BonesVisible.boolValue);

                //layerField.intValue = EditorGUILayout.LayerField(layerField.intValue);

                //____________________\\
                EditorGUILayout.Space(3);

                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                    buttonGenerate = GUILayout.Button("Preview");
                    if (buttonGenerate)
                    {
                        ropeBehaviour.Clear(true);
                        ropeBehaviour.Generate();
                    }
                EditorGUI.EndDisabledGroup();

                //____________________\\
                EditorGUILayout.Space(5);

                #region PhysicsGroup
                physicsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(physicsGroup, "Physics settings");
                if (physicsGroup)
                {
                    EditorGUILayout.PropertyField(Collision);
                    if (Collision.boolValue)
                    {
                        collisionLayers.intValue = EditorGUILayout.MaskField("Collision Layers", collisionLayers.intValue, layersList/*InternalEditorUtility.layers*/);
                        //Debug.Log(collisionLayers.intValue);
                    }

                    //____________________\\
                    EditorGUILayout.Space(3);

                    EditorGUILayout.PropertyField(SimulationIterations);
                    EditorGUILayout.HelpBox("More iterations - more smooth physics and more resources required to provide it", MessageType.None);

                    //____________________\\
                    EditorGUILayout.Space(3);

                    EditorGUILayout.Slider(Correction, 0, 0.9f);
                    EditorGUILayout.HelpBox("0 - very stretchy rope, 0.9 - tight rope", MessageType.None);

                    EditorGUILayout.PropertyField(RotateNodes);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                #endregion

                //____________________\\
                EditorGUILayout.Space(5);

                #region GravityGroup
                gravityGroup = EditorGUILayout.BeginFoldoutHeaderGroup(gravityGroup, "Gravity settings");
                if (gravityGroup)
                {
                    EditorGUILayout.PropertyField(Gravity);
                    if (ropeBehaviour.Gravity)
                    {
                        EditorGUILayout.PropertyField(GravityForce);
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                #endregion

                //____________________\\
                EditorGUILayout.Space(5);
            }

        }
#endif
    }
}