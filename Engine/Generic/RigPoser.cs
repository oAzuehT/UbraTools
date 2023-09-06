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

        [Header(" Make sure this is the Rig ROOT ")]
        public GameObject RigParent;

        public List<BoneTransform> RigTransforms = new List<BoneTransform>(); 

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
           
            Transform[] children = RigParent.GetChildren();

            foreach (var child in children)
            {

                BoneTransform bt = new BoneTransform();
                bt.Name = child.name;
                bt.Position = child.position;
                bt.Rotation = child.rotation;

                RigTransforms.Add(bt);
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
        public override void OnInspectorGUI()
        {

            RigPoser RP = (RigPoser)target;
            DrawDefaultInspector();

            GUILayout.Space(20);

            if (GUILayout.Button("Load Pose"))
            {
                RP.LoadBoneState();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Save Pose"))
            {
                RP.SaveBoneSave();
            }
            GUILayout.Space(10);

        }
    }

#endif
}
