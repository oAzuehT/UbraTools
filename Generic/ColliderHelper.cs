using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using DestroyIt;

namespace Ubra.Engine.Generic
{
    public class ColliderHelper : MonoBehaviour
    {

        [System.Serializable]
        public enum COLLIDER_MODE
        {
            Generic,
            Box,
            Sphere,
            Capsule,
            Mesh
        }
        [HideInInspector] public COLLIDER_MODE Mode;
        List<Collider> _colliders = new List<Collider>();

        private void UpdateCollidersList()
        {
            if(_colliders == null)
            {
                _colliders = new List<Collider>();
            }
            _colliders.Clear();
            Transform[] children = gameObject.GetChildren();
            if(children.Length <= 0)
            {
                return;
            }
            switch (Mode)
            {
                case COLLIDER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<Collider>())
                        {
                            _colliders.Add(child.GetComponent<Collider>());
                        }
                    }
                    break;
                case COLLIDER_MODE.Box:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<BoxCollider>())
                        {
                            _colliders.Add(child.GetComponent<BoxCollider>());
                        }
                    }
                    break;
                case COLLIDER_MODE.Sphere:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SphereCollider>())
                        {
                            _colliders.Add(child.GetComponent<SphereCollider>());
                        }
                    }
                    break;
                case COLLIDER_MODE.Capsule:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<CapsuleCollider>())
                        {
                            _colliders.Add(child.GetComponent<CapsuleCollider>());
                        }
                    }
                    break;
                case COLLIDER_MODE.Mesh:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshCollider>())
                        {
                            _colliders.Add(child.GetComponent<MeshCollider>());
                        }
                    }
                    break;
            }

        }

        public void EnableColliders()
        {

            UpdateCollidersList();

            foreach (var collider in _colliders)
            {
                collider.enabled = true;
            }
        }

        public void DisableColliders()
        {

            UpdateCollidersList();

            foreach (var collider in _colliders)
            {
                collider.enabled = false;
            }
        }


        [HideInInspector] public bool RemovalSafetyCheck = false;
        public void RemoveAllColliders()
        {

            if (RemovalSafetyCheck)
            {
                UpdateCollidersList();

                foreach (var collider in _colliders)
                {
                    collider.gameObject.RemoveComponent<Collider>();
                }
            }
        }

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ColliderHelper))]
    public class ColliderHelperEditor : Editor
    {

        SerializedProperty mode;
        SerializedProperty removalCheck;

        private void OnEnable()
        {
            mode = serializedObject.FindProperty("Mode");
            removalCheck = serializedObject.FindProperty("RemovalSafetyCheck");    
        }

        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();
            ColliderHelper CH = (ColliderHelper)target;


            GUILayout.Space(10);

            EditorGUILayout.PropertyField(mode);

            GUILayout.Space(20);

            if (GUILayout.Button("Enable Child Colliders"))
            {
                CH.EnableColliders();
            }

            if (GUILayout.Button("Disable Child Colliders"))
            {
                CH.DisableColliders();
            }

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(removalCheck);
            if (removalCheck.boolValue)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Remove Child Colliders"))
                {
                    CH.RemoveAllColliders();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

    }

#endif
}
