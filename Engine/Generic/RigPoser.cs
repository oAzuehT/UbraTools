using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ubra.Engine.Generic
{

    public class RigPoser : MonoBehaviour
    {

        [HideInInspector] public SkinnedMeshRenderer SkinnedMeshRendererReference;

        //[Header(" Make sure this is the ROOT of the Rig ")]
        public GameObject RigParent;

        [Space(20)]
        [SerializeField] private List<BoneTransform> RigTransforms = new List<BoneTransform>();

        [HideInInspector] public bool HasSavedPose = false;
        [HideInInspector] public string BakedNameOverride = string.Empty;

        public void SaveBoneSave()
        {

            if(RigParent == null)
            {
                Debug.LogError(" Rig Parent NULL! ");
                return;
            }

            if(RigTransforms == null)
            {
                RigTransforms = new List<BoneTransform>();
            }
            RigTransforms.Clear();
            HasSavedPose = false;

            Transform[] children = RigParent.GetChildren();

            if(children.Length > 0)
            {
                foreach (var child in children)
                {

                    BoneTransform bt = new BoneTransform();
                    bt.Name = child.name;
                    bt.Position = child.position;
                    bt.Rotation = child.rotation;

                    RigTransforms.Add(bt);
                }

                HasSavedPose = true;
            }

        }

        public void LoadBoneState()
        {

            if(RigTransforms == null)
            {
                Debug.LogError(" No Rig Data Saved ");
                return;
            }

            if(RigTransforms.Count <= 0)
            {
                Debug.LogError(" No Rig Data Saved ");
                return;
            }

            Transform[] children = RigParent.GetChildren();

            foreach (var child in children)
            {
                foreach (var bone in RigTransforms)
                {

                    if(child.name == bone.Name)
                    {
                        child.position = bone.Position;
                        child.rotation = bone.Rotation;
                    }
                }
            }

        }

        public void BakePose(string nameOverride)
        {

            if(SkinnedMeshRendererReference == null)
            {
                Debug.LogError(" Please assign a valid Skined Mesh Renderer ");
                return;
            }
            GameObject dummy = new GameObject();
            if(string.IsNullOrEmpty(nameOverride) || string.IsNullOrWhiteSpace(nameOverride))
            {
                dummy.name = RigParent.name + " baked pose";
            }
            else
            {
                dummy.name = nameOverride + " baked pose";
            }

            Mesh sourceMesh = SkinnedMeshRendererReference.sharedMesh;
            Transform[] sourceBones = SkinnedMeshRendererReference.bones;
            Matrix4x4[] sourceBindPoses = SkinnedMeshRendererReference.sharedMesh.bindposes;

            SkinnedMeshRenderer targetSkinnedMeshRenderer = dummy.AddComponent<SkinnedMeshRenderer>();
            targetSkinnedMeshRenderer.sharedMesh = sourceMesh;
            targetSkinnedMeshRenderer.sharedMaterials = SkinnedMeshRendererReference.sharedMaterials;
            targetSkinnedMeshRenderer.bones = sourceBones;
            targetSkinnedMeshRenderer.sharedMesh.bindposes = sourceBindPoses;

        }

    }


#if UNITY_EDITOR

    [CustomEditor(typeof(RigPoser))]
    public class RigPoserEditor : Editor
    {

        SerializedProperty rigParent;
        SerializedProperty hasSavedPose;
        SerializedProperty rigTransforms;

        SerializedProperty smr;
        GUIStyle style;
        
        private void OnEnable()
        {
            rigParent = serializedObject.FindProperty("RigParent");
            hasSavedPose = serializedObject.FindProperty("HasSavedPose");
            rigTransforms = serializedObject.FindProperty("RigTransforms");

            smr = serializedObject.FindProperty("SkinnedMeshRendererReference");
        }

        public override void OnInspectorGUI()
        {

            Texture2D customTexture = new Texture2D(1, 1);
            Color buttonCollor = new Color(0.15f, 0.15f, 0.15f, 1f);
            customTexture.SetPixel(0, 0, (buttonCollor)); // Set the color of the texture
            customTexture.Apply(); // Apply changes to the texture    
            style = new GUIStyle(GUI.skin.label);
            style.normal.background = customTexture;
            style.fontSize = 16;
            style.fontStyle = FontStyle.Normal;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.magenta;
            
            RigPoser RP = (RigPoser)target;
            //DrawDefaultInspector();

            GUILayout.Space(20);
            EditorGUILayout.PropertyField(rigParent);

            if (hasSavedPose.boolValue)
            {

                GUILayout.Space(20);
                EditorGUILayout.PropertyField(rigTransforms);

                GUILayout.Space(20);
                style.normal.textColor = Color.red;
                if (GUILayout.Button("Write Pose", style))
                {
                    RP.LoadBoneState();
                }

                GUILayout.Space(30);

                EditorGUILayout.BeginHorizontal();               
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(" Skinned Mesh: ");
                EditorGUILayout.PropertyField(smr, GUIContent.none);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField(" Baked Name: ");
                RP.BakedNameOverride = EditorGUILayout.TextField("", RP.BakedNameOverride);
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(5);
                style.normal.textColor = Color.cyan;
                if (GUILayout.Button("Bake Pose", style))
                {
                    RP.BakePose(RP.BakedNameOverride);
                }
                GUILayout.Space(10);

                serializedObject.ApplyModifiedProperties();
            }

            GUILayout.Space(10);
            style.normal.textColor = Color.green;
            if (GUILayout.Button("Read Again", style))
            {
                RP.SaveBoneSave();
            }
            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();

        }
    }

#endif
}
