using UnityEditor;
using UnityEngine;

namespace Ubra.Engine.Generic
{
    public class RendererHelper : MonoBehaviour
    {

        [System.Serializable]
        public enum RENDERER_MODE
        {
            Generic,
            MeshRenderer,
            SkinnedRenderer
        }

        [HideInInspector] public RENDERER_MODE Mode;
        [HideInInspector] public Material MaterialOverride;
        [HideInInspector] public bool OverrideMaterial = false;

        ///

        public void EnableRenderers()
        {
            Transform[] children = gameObject.GetChildren();
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().enabled = true;
                        }
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        }
                    }
                    break;
                case RENDERER_MODE.MeshRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                    break;
                case RENDERER_MODE.SkinnedRenderer:                    
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().enabled = true;
                        }
                    }
                    break;
            }

        }

        public void DisableRenderers()
        {
            Transform[] children = gameObject.GetChildren();
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().enabled = false;
                        }
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        }
                    }
                    break;
                case RENDERER_MODE.MeshRenderer:                   
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().enabled = false;
                        }
                    }
                    break;
                case RENDERER_MODE.SkinnedRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().enabled = false;
                        }
                    }
                    break;
            }
        }

        ///

        public void EnableShadows()
        {
            Transform[] children = gameObject.GetChildren();
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                    }
                    break;
                case RENDERER_MODE.MeshRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                    }
                    break;
                case RENDERER_MODE.SkinnedRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                        }
                    }
                    break;
            }
        }

        public void DisableShadows()
        {
            Transform[] children = gameObject.GetChildren();
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }
                    break;
                case RENDERER_MODE.MeshRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            child.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }
                    break;
                case RENDERER_MODE.SkinnedRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            child.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                        }
                    }
                    break;
            }
        }

        ///

        public void OverrideAllMaterials()
        {

            if (!OverrideMaterial || MaterialOverride == null)
            {
                Debug.LogError(" Could not Override materials. Set the override flag to TRUE or check if the material field is not empty.");
                return;
            }

            Transform[] children = gameObject.GetChildren();
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            if (!Application.isPlaying)
                            {
                                child.GetComponent<MeshRenderer>().material = MaterialOverride;
                            }
                            else
                            {
                                child.GetComponent<MeshRenderer>().sharedMaterial = MaterialOverride;
                            }
                        }
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            if (!Application.isPlaying)
                            {
                                child.GetComponent<SkinnedMeshRenderer>().material = MaterialOverride;
                            }
                            else
                            {
                                child.GetComponent<SkinnedMeshRenderer>().sharedMaterial = MaterialOverride;
                            }
                        }
                    }
                    break;
                case RENDERER_MODE.MeshRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<MeshRenderer>() != null)
                        {
                            if (!Application.isPlaying)
                            {
                                child.GetComponent<MeshRenderer>().material = MaterialOverride;
                            }
                            else
                            {
                                child.GetComponent<MeshRenderer>().sharedMaterial = MaterialOverride;
                            }                          
                        }
                    }
                    break;
                case RENDERER_MODE.SkinnedRenderer:
                    foreach (var child in children)
                    {
                        if (child.GetComponent<SkinnedMeshRenderer>() != null)
                        {
                            if (!Application.isPlaying)
                            {
                                child.GetComponent<SkinnedMeshRenderer>().material = MaterialOverride;
                            }
                            else
                            {
                                child.GetComponent<SkinnedMeshRenderer>().sharedMaterial = MaterialOverride;
                            }
                        }
                    }
                    break;
            }            
        }

        ///

        [HideInInspector] public bool RemovalSafetyCheck = false;
        public void RemoveAllRenderers()
        {
            switch (Mode)
            {
                case RENDERER_MODE.Generic:
                    gameObject.RemoveComponentFromChildren<MeshRenderer>();
                    gameObject.RemoveComponentFromChildren<SkinnedMeshRenderer>();
                    break;
                case RENDERER_MODE.MeshRenderer:
                    gameObject.RemoveComponentFromChildren<MeshRenderer>();
                    break;
                case RENDERER_MODE.SkinnedRenderer:
                    gameObject.RemoveComponentFromChildren<SkinnedMeshRenderer>();
                    break;
            }
        }

        ///
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(RendererHelper))]
    public class RendererHelperEditor : Editor
    {

        SerializedProperty mode;
        SerializedProperty overrideMaterial;
        SerializedProperty materialOverride;

        SerializedProperty removalCheck;

        private void OnEnable()
        {
            mode = serializedObject.FindProperty("Mode");
            overrideMaterial = serializedObject.FindProperty("OverrideMaterial");
            materialOverride = serializedObject.FindProperty("MaterialOverride");
            removalCheck = serializedObject.FindProperty("RemovalSafetyCheck");
        }

        public override void OnInspectorGUI()
        {

            DrawDefaultInspector();
            RendererHelper RH = (RendererHelper)target;

            GUILayout.Space(10);

            EditorGUILayout.PropertyField(mode);

            GUILayout.Space(20);

            if (GUILayout.Button("Enable Child Renderers"))
            {
                RH.EnableRenderers();
            }

            if (GUILayout.Button("Disable Child Renderers"))
            {
                RH.DisableRenderers();
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Enable Child Shadows"))
            {
                RH.EnableShadows();
            }

            if (GUILayout.Button("Disable Child Shadows"))
            {
                RH.DisableShadows();
            }

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(overrideMaterial);

            GUILayout.Space(5);
            if (overrideMaterial.boolValue)
            {

                EditorGUILayout.PropertyField(materialOverride);

                if (GUILayout.Button("Override Child Materials"))
                {
                    RH.OverrideAllMaterials();
                }
            }

            GUILayout.Space(10);
            EditorGUILayout.PropertyField(removalCheck);
            if (removalCheck.boolValue)
            {
                GUILayout.Space(10);
                if (GUILayout.Button("Remove Child Renderers"))
                {
                    RH.RemoveAllRenderers();
                }
            }

            serializedObject.ApplyModifiedProperties();

        }
    }

#endif

}
