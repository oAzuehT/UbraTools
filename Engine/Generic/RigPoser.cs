using System;
using System.Collections.Generic;
using Ubra.Engine.Core;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Ubra.Engine.Generic
{

    public class RigPoser : MonoBehaviour
    {

        //[Header(" Make sure this is the ROOT of the Rig ")]
        public GameObject RigParent;

        [Space(20)]
        [SerializeField] private List<BoneTransform> RigTransforms = new List<BoneTransform>();

        [HideInInspector] public bool HasSavedPose = false;

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

    }


#if UNITY_EDITOR

    [CustomEditor(typeof(RigPoser))]
    public class RigPoserEditor : Editor
    {

        SerializedProperty rigParent;
        SerializedProperty hasSavedPose;
        SerializedProperty rigTransforms;
        GUIStyle style;
        
        private void OnEnable()
        {
            rigParent = serializedObject.FindProperty("RigParent");
            hasSavedPose = serializedObject.FindProperty("HasSavedPose");
            rigTransforms = serializedObject.FindProperty("RigTransforms"); 
        }

        public override void OnInspectorGUI()
        {

            Texture2D customTexture = new Texture2D(1, 1);
            customTexture.SetPixel(0, 0, Color.black); // Set the color of the texture
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

                GUILayout.Space(35);
                style.normal.textColor = Color.red;
                if (GUILayout.Button("Write Pose", style))
                {
                    RP.LoadBoneState();
                }

            }

            GUILayout.Space(10);
            style.normal.textColor = Color.green;
            if (GUILayout.Button("Read Pose", style))
            {
                RP.SaveBoneSave();
            }
            GUILayout.Space(10);

            serializedObject.ApplyModifiedProperties();

        }
    }

#endif
}
