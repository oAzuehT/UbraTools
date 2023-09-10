using System.Collections;
using UnityEngine;
using System;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using Ubra.Engine.Core;

public struct IntWrapper
{
    public int Value;
}
public struct FloatWrapper
{
    public float Value;
}
public struct BoolWrapper
{
    public bool Value;
}
public struct StringWrapper
{
    public string Value;
}

[System.Serializable]
public struct TransformDistance
{
    public Transform Transform;
    public float Distance;

    public TransformDistance(Transform transform, float distance)
    {
        Transform = transform;
        Distance = distance;
    }
}

[System.Serializable]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[System.Serializable]
public struct BoneTransform
{
    public string Name;
    public Vector3 Position;
    public Quaternion Rotation;

    public Transform Target; // animation target points to Ragdoll Transform --- Ragdoll target points to Animation Transform
}



public static class UbraUtils
{

    /// To use these Methods add:
    // using static UbraUtils;
    /// to your header


    #region DEBUG Methods

    /// Read Me!
    // DEBUG GENERATES GARBAGE!!! - Be mindful of that when profiling!
    // Use conditional debugs to reduce garbage!

    // You can customize the debug flags:
    public static string DEBUG_WARNING = "@@";
    public static string DEBUG_ERROR = "!!";

    ///Example:
    // ConsoleDebug("!!This is an Error", "This is an Warn@@ing");
    // The console will spit two console messages, one Warning and one Error message with their respective input messages.
    
    // Use ConsoleDebug("NormalDebug"); for normal debugs 
    // Use ConsoleDebug((Boolean result), "true message", "optional false message") for conditional debugs
    
    public static void ConsoleDebug(params string[] values)
    {
#if UNITY_EDITOR

        if (values != null && values.Length > 0)
        {
            ///Debug.Log(string.Join(" ", values));
            foreach (string value in values)
            {
                DebugValue(value);
            }
        }
#endif
    }

    //Conditional Debug
    public static void ConsoleDebug(Type condition, string valueIfTrue, string valueIfFalse = "")
    {
#if UNITY_EDITOR
        if ((condition.IsValueType.Equals(true)))
        {
            DebugValue(valueIfTrue);
        }
        else
        {
            DebugValue(valueIfFalse);
        }
#endif
    }
    public static void ConsoleDebug(bool condition, string valueIfTrue, string valueIfFalse = "")
    {
#if UNITY_EDITOR
        if (condition)
        {
            DebugValue(valueIfTrue);
        }
        else
        {
            DebugValue(valueIfFalse);
        }
#endif
    }
    public static void ConsoleDebug(bool condition, string valueIfTrue, System.Action doIfFalse)
    {
#if UNITY_EDITOR
        if (condition)
        {
            DebugValue(valueIfTrue);
        }
        else
        {
            doIfFalse?.Invoke();
        }
#endif
    }
    public static void ConsoleDebug(bool condition, System.Action doIfTrue, string valueIfFalse = "")
    {
#if UNITY_EDITOR
        if (condition)
        {
            doIfTrue?.Invoke();
        }
        else
        {
            DebugValue(valueIfFalse);
        }
#endif
    }
    public static void ConsoleDebug(bool condition, System.Action doIfTrue, System.Action doIfFalse)
    {
#if UNITY_EDITOR
        if (condition)
        {
            doIfTrue?.Invoke();
        }
        else
        {
            doIfFalse?.Invoke();
        }
#endif
    }

    private static void DebugValue(string value)
    {

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        if (value.Contains(DEBUG_ERROR))
        {
            value = value.RemoveCharacters(DEBUG_ERROR);         
            Debug.LogError(value);
        }
        else if (value.Contains(DEBUG_WARNING))
        {
            value = value.RemoveCharacters(DEBUG_WARNING);
            Debug.LogWarning(value);
        }
        else
        {
            Debug.Log(value);
        }
    }
    
    #endregion


    //Returns a variable NAME.
    ///Example: string variableName = GetPropertyName(() => SomeClass.SomeProperty);
    public static string GetPropertyName<T>(Expression<Func<T>> propertyLambda) //returns variable name 
    {
        var me = propertyLambda.Body as MemberExpression;

        if (me == null)
        {
            throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
        }

        return me.Member.Name;
    }


    //Consider using LayerMask.NameToLayer instead
    public static int GetLayerIndex(string layerName)
    {
        return (int)Mathf.Log(LayerMask.GetMask(layerName), 2);
    }

}


public static class GameObjectExtensions
{    

    /// Hello, Chat GPT!
    public static void RemoveComponent<T>(this GameObject instance) where T : Component
    {

        T obj =  instance.GetComponent<T>();
        if(obj != null)
        {
            if (Application.isPlaying)
            {
                UnityEngine.Object.Destroy(instance.GetComponent<T>());
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(instance.GetComponent<T>());
            }
        }
        
    }
    public static void RemoveComponentFromChildren<T>(this GameObject instance) where T : Component
    {

        T[] children = instance.GetComponentsInChildren<T>();
        if(children.Length > 0)
        {
            for (int i = 0; i < children.Length; i++)
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(children[i]);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(children[i]);
                }              
            }
        }

    }

    public static Vector3[] GetBoxColliderVertices(this BoxCollider box)
    {
        var size = box.size * 0.5f;

        var mtx = Matrix4x4.TRS(box.bounds.center, box.transform.localRotation, box.transform.localScale);

        Vector3[] points = new Vector3[8];

        points[0] = mtx.MultiplyPoint3x4(new Vector3(-size.x, size.y, size.z));
        points[1] = mtx.MultiplyPoint3x4(new Vector3(-size.x, -size.y, size.z));
        points[2] = mtx.MultiplyPoint3x4(new Vector3(size.x, -size.y, size.z));
        points[3] = mtx.MultiplyPoint3x4(new Vector3(size.x, size.y, size.z));
        points[4] = mtx.MultiplyPoint3x4(new Vector3(-size.x, size.y, -size.z));
        points[5] = mtx.MultiplyPoint3x4(new Vector3(-size.x, -size.y, -size.z));
        points[6] = mtx.MultiplyPoint3x4(new Vector3(size.x, -size.y, -size.z));
        points[7] = mtx.MultiplyPoint3x4(new Vector3(size.x, size.y, -size.z));

        return points;
    }

    public static bool HasLayer(this LayerMask layerMask, int layerIndex)
    {
        return ((layerMask.value & (1 << layerIndex)) > 0);

        // Check for Layer Method 2
        ///return layerMask == (layerMask | (1 << layerIndex));
    }
    public static bool HasLayer(this LayerMask layerMask, string layerName)
    {
        int layerIndex = LayerMask.NameToLayer(layerName);
        if (layerIndex == -1)
        {
            // The layerName doesn't exist, so it's not present in the LayerMask.
            return false;
        }

        return layerMask.HasLayer(layerIndex);
    }

    public static bool[] HasLayers(this LayerMask layerMask)
    {
        var hasLayers = new bool[32];

        for (int i = 0; i < 32; i++)
        {
            if (layerMask == (layerMask | (1 << i)))
            {
                hasLayers[i] = true;
            }
        }

        return hasLayers;
    }


    #region Material and Renderer Handling Methods

    /// Code from JohnStairs
    public static void EnableZWrite(this Renderer r, string shaderName = "HDRP/Lit")
    {
        Shader targetShader = Shader.Find(shaderName);

        if (targetShader == null)
        {
            return;
        }

        foreach (Material m in r.materials)
        {
            if (m.HasProperty("_Color"))
            {
                m.SetInt("_ZWrite", 1);
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                switch (targetShader.name)
                {

                    case ("HDRP/Lit"):
                        m.SetInt("_ZWrite", 1);
                        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
                        break;

                    case ("someOtherShader"):
                        m.SetFloat("_N_F_O", 1);
                        m.SetInt("_ZWrite", 1);
                        m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
                        break;
                }
            }
        }
    }

    public static void DisableZWrite(this Renderer r, string shaderName = "HDRP/Lit")
    {

        Shader targetShader = Shader.Find(shaderName);

        if (targetShader == null)
        {
            return;
        }

        foreach (Material m in r.materials)
        {
            if (m.HasProperty("_Color"))
            {
                m.SetInt("_ZWrite", 0);
                m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 100;
            }
            else
            {
                if (m.shader == targetShader)
                {
                    switch (targetShader.name)
                    {

                        case ("HDRP/Lit"):
                            m.SetInt("_ZWrite", 0);
                            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 100;
                            break;

                        case ("someOtherShader"):
                            m.SetFloat("_N_F_O", 0);
                            m.SetInt("_ZWrite", 0);
                            m.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent + 100;
                            break;
                    }
                }
            }
        }
    }
        

    #endregion


}

public static class AnimatorExtensions
{

    public static void SetLength(this AnimationCurve curve, float lenght)
    {
        Keyframe[] keys = curve.keys;
        for (int i = 0; i < keys.Length; i++)
        {
            keys[i].time *= lenght;
        }
        curve.keys = keys;
    }

    public static bool IsAnimationPlaying(this Animator animator, string stateName, int layerIndex, string stateMachineName = "")
    {

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        int currentAnimationHash = stateInfo.fullPathHash;

        //string currentAnimationName = controller.GetChildStateMachine(currentAnimationHash, layerIndex).state.name;
        string currentAnimationName = animator.GetActiveAnimatorState(layerIndex).state.name;

        AnimatorState resultState = animator.GetAnimatorState(layerIndex, stateName, stateMachineName);
        if (resultState != null)
        {
            //Debug.Log(" Trying to compare ::: " + resultState.name.ToString() + " to ::::" + currentAnimationName);

            Debug.Log(" Trying to compare ::: " + resultState.name.ToString() + " to ::::" + currentAnimationName);

            return resultState.nameHash == currentAnimationHash;
            //return currentState.name == stateName;
        }

        return false;
    }


    public static float GetLength(this Animator animator, string stateName, int layerIndex = 0)
    {

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        // Check if the current state matches the desired state by name
        if (stateInfo.IsName(stateName))
        {
            // Get the length (duration) of the current state
            float stateLength = stateInfo.length;
            return stateLength;
        }
        else
        {
            // The desired state is not currently playing; find it by name
            AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
            foreach (ChildAnimatorState state in controller.layers[layerIndex].stateMachine.states)
            {
                if (state.state.name == stateName)
                {
                    // Found the desired state by name; return its length
                    return state.state.motion.averageDuration;
                }
            }

            // State with the given name not found; return a default value or handle it accordingly
            return 0f; // You can return 0 or another default value here
        }
    }
    public static ChildAnimatorState GetActiveAnimatorState(this Animator animator, int layerIndex)
    {

        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);

        // Store a reference to the current state
        ChildAnimatorState currentPlayingState = controller.layers[layerIndex].stateMachine.states[0];
        bool stateFound = false;

        foreach (ChildAnimatorState state in controller.layers[layerIndex].stateMachine.states)
        {
            if (animator.IsInTransition(layerIndex))
            {
                // The Animator is in transition, so use the next state
                if (stateInfo.shortNameHash == state.state.nameHash)
                {
                    currentPlayingState = state;
                    stateFound = true;
                    break;
                }
            }
            else
            {
                // The Animator is not in transition, so use the current state
                if (stateInfo.fullPathHash == state.state.nameHash)
                {
                    currentPlayingState = state;
                    stateFound = true;
                    break;
                }
            }
        }

        if (stateFound)
        {
            string currentStateName = currentPlayingState.state.name;
            Debug.Log("Current Active State: " + currentStateName);

            return currentPlayingState;
        }
        else
        {
            Debug.Log("FORBIDDEN RETURN!");
            return controller.layers[layerIndex].stateMachine.states[0];
        }

    }


    //

    private static AnimatorState GetAnimatorState(this AnimatorStateMachine stateMachine, int nameHash)
    {
        foreach (ChildAnimatorState state in stateMachine.states)
        {
            // Compare the fullPathHash of each state with the current state hash code
            if (state.state.nameHash == nameHash)
            {
                // Return the matching state
                return state.state;
            }
        }

        return null;
    }
    private static AnimatorState GetAnimatorState(this AnimatorStateMachine stateMachine, string stateName)
    {

        foreach (ChildAnimatorState state in stateMachine.states)
        {
            // Compare the fullPathHash of each state with the current state hash code
            if (state.state.name == stateName)
            {
                // Return the matching state
                return state.state;
            }
        }

        return null;
    }

    //

    public static ChildAnimatorState GetChildStateMachine(this AnimatorController animatorController, string stateName, int layerIndex)
    {

        ChildAnimatorState result = animatorController.layers[layerIndex].stateMachine.states[0];

        foreach (var machine in animatorController.layers[layerIndex].stateMachine.states)
        {
            if (machine.state.name == stateName)
            {
                return machine;
            }
        }

        return result;
    }
    public static ChildAnimatorState GetChildStateMachine(this AnimatorController animatorController, int stateNameHash, int layerIndex)
    {

        ChildAnimatorState result = animatorController.layers[layerIndex].stateMachine.states[0];

        foreach (var machine in animatorController.layers[layerIndex].stateMachine.states)
        {
            if (machine.state.nameHash == stateNameHash)
            {
                return machine;
            }
        }

        return result;
    }

    public static ChildAnimatorStateMachine[] GetStateMachines(this AnimatorController animatorController, int layerIndex)
    {
        return animatorController.layers[layerIndex].stateMachine.stateMachines;
    }

    public static AnimatorState GetAnimatorState(this Animator animator, int layerIndex, string desiredStateName, string stateMachineName)
    {

        // Get all states in the Animator Controller
        AnimatorController controller = animator.runtimeAnimatorController as AnimatorController;
        ///AnimatorStateMachine rootStateMachine = controller.layers[layerIndex].stateMachine;
        ChildAnimatorStateMachine[] rootMachines = controller.GetStateMachines(0);
        AnimatorState desiredResult = null;

        /// ROOT LEVEL

        foreach (ChildAnimatorStateMachine subStateMachine in rootMachines)
        {

            // Check if Desired StateMachine is at Root level

            if (subStateMachine.stateMachine.name.Equals(stateMachineName))
            {

                // If it is, then get State from this State Machine

                desiredResult = subStateMachine.stateMachine.GetAnimatorState(desiredStateName);
                if (desiredResult != null)
                {

                    Debug.Log(" State :: " + desiredResult.name + " found in Child State Machine::" + subStateMachine.stateMachine.name);

                    return desiredResult;
                }
            }

        }

        /// Branch LEVEL

        foreach (ChildAnimatorStateMachine subStateMachine in rootMachines)
        {

            if (subStateMachine.stateMachine.name.Equals(stateMachineName))
            {

                ChildAnimatorState branch = controller.GetChildStateMachine(desiredStateName, layerIndex);
                if (branch.state.name.Equals(stateMachineName))
                {

                    Debug.Log(" State :: " + branch.state.name + " found in Child State Machine::" + subStateMachine.stateMachine.name);

                    return branch.state;
                }

            }

        }

        Debug.Log("State not Found!");
        return null;

    }


    public static string GetAnimatorLayerName(this Animator animator, int layerIndex)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return string.Empty;
        }

        if (layerIndex < 0 || layerIndex >= animator.layerCount)
        {
            Debug.LogError("Invalid layer index: " + layerIndex);
            return string.Empty;
        }

        return animator.GetLayerName(layerIndex);
    }

    public static int GetAnimatorIndex(this Animator animator, string layerName)
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null!");
            return -1;
        }

        for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
        {
            if (animator.GetLayerName(layerIndex) == layerName)
            {
                return layerIndex;
            }
        }

        Debug.LogError("Layer name not found: " + layerName);
        return -1;
    }

}

public static class TransformExtensions
{

    //Instantly 'Look' at other transform
    public static void LookTo(this Transform value, Transform target)
    {
        value.rotation = Quaternion.LookRotation(target.position - value.position);
    }
    //Smooth the Look operation using Coroutines.
    // If you don't have a Singleton instance you can instead directly call the 'LookAtCoroutine' Coroutine.
    public static void LookTo(this Transform value, Transform target, float speed, float maxTimeSpan, ref bool isFinished)
    {

        isFinished = false;

        BoolWrapper statusWrapper = new BoolWrapper();
        statusWrapper.Value = false;
        isFinished = statusWrapper.Value;

        System.Action result = () =>
        {
            //Swap this with any Singleton inheriting from MonoBehaviour
            /// Singleton.Instance.StartCoroutine
            Ubra.Ubra.Instance.StartCoroutine(LookAtCoroutine(value, target, speed, maxTimeSpan, statusWrapper.Value));
        };

        result();
    }

    //You can retrieve the operation status by using a bool wrapper
    public static IEnumerator LookAtCoroutine(Transform value, Transform target, float timeToLookAt, float maxTimeSpan, bool status)
    {

        Quaternion startRotation = value.rotation;
        Quaternion endRotation = Quaternion.LookRotation(target.position - value.position);

        float elapsedTime = 0f;

        // If the loop finishes before reaching maxTimeSpan, we continue the lerp until maxTimeSpan is reached.
        while (elapsedTime < maxTimeSpan)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / maxTimeSpan); // Normalize time from 0 to 1

            // Interpolate between start and end rotation using Lerp
            value.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }

        // Ensure the final rotation is precisely the target's forward direction
        value.rotation = endRotation;

        status = true;
    }


    //Gets all children of a gameObject, making sure the referenced gameObject is not included
    public static Transform[] GetChildren(this GameObject gameObject, bool includeInactive = false)
    {

        // Set includeInactive to false if you want to exclude inactive objects
        Transform[] childTransforms = gameObject.transform.GetComponentsInChildren<Transform>(includeInactive: includeInactive);

        // Exclude the root transform itself from the list
        // You can use LINQ to create a new array excluding the root transform
        return childTransforms.Where(child => child != gameObject.transform).ToArray();

    }

    //Gets all children of a transform, making sure the referenced transform is not included
    public static Transform[] GetChildren(this Transform transform, bool includeInactive = false)
    {

        // Set includeInactive to false if you want to exclude inactive objects
        Transform[] childTransforms = transform.GetComponentsInChildren<Transform>(includeInactive: includeInactive);

        // Exclude the root transform itself from the list
        // You can use LINQ to create a new array excluding the root transform
        return childTransforms.Where(child => child != transform).ToArray();
    }


    //Gets the nearest transform TO a position FROM the comparison list
    public static Transform FindClosestTransform(this Vector3 referencePosition, List<Transform> transformList)
    {

        Vector3 targetPosition = referencePosition;

        // Calculate distances for each transform
        List<TransformDistance> transformDistances = new List<TransformDistance>();
        foreach (var transform in transformList)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            transformDistances.Add(new TransformDistance(transform, distance));
        }

        // Sort the transformDistances list based on distance in ascending order
        transformDistances.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // Take the top N transforms based on the number ofClosestObjects
        List<Transform> closestTransforms = transformDistances
            .Take(1)
            .Select(td => td.Transform)
            .ToList();

        return closestTransforms[0];

    }

    //Gets the selected number of closest transforms TO a position FROM the comparison list
    public static Transform[] FindClosestTransform(this Vector3 referencePosition, List<Transform> transformList, int numberOfClosestObjects)
    {

        Vector3 targetPosition = referencePosition;

        // Calculate distances for each transform
        TransformDistance[] transformDistances = new TransformDistance[transformList.Count];
        for (int i = 0; i < transformDistances.Length; i++)
        {
            float distance = Vector3.Distance(transformList[i].position, targetPosition);
            transformDistances[i] = new TransformDistance(transformList[i].transform, distance);
        }

        // Sort the transformDistances list based on distance in ascending order
        ///transformDistances.Sort((a, b) => a.Distance.CompareTo(b.Distance)); - list
        // Take the top N transforms based on the number ofClosestObjects
        //List<Transform> closestTransforms = transformDistances
        //    .Take(numberOfClosestObjects)
        //    .Select(td => td.Transform)
        //    .ToList();

        Array.Sort(transformDistances, (a, b) => a.Distance.CompareTo(b.Distance));
        Transform[] closestTransforms = transformDistances
            .Take(numberOfClosestObjects)
            .Select(td => td.Transform)
            .ToArray();

        return closestTransforms;

    }

    //Gets the nearest transform TO the referenced transform FROM the comparison list
    public static Transform FindClosestTransform(this Transform referenceTransform, List<Transform> transformList)
    {

        Vector3 targetPosition = referenceTransform.position;

        // Calculate distances for each transform
        List<TransformDistance> transformDistances = new List<TransformDistance>();
        foreach (var transform in transformList)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            transformDistances.Add(new TransformDistance(transform, distance));
        }

        // Sort the transformDistances list based on distance in ascending order
        transformDistances.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // Take the top N transforms based on the number ofClosestObjects
        List<Transform> closestTransforms = transformDistances
            .Take(1)
            .Select(td => td.Transform)
            .ToList();

        return closestTransforms[0];

    }

    //Gets the selected number of closest transforms TO the referenced transform FROM the comparison list
    public static List<Transform> FindClosestTransform(this Transform referenceTransform, List<Transform> transformList, int numberOfClosestObjects)
    {

        Vector3 targetPosition = referenceTransform.position;

        // Calculate distances for each transform
        List<TransformDistance> transformDistances = new List<TransformDistance>();
        foreach (var transform in transformList)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            transformDistances.Add(new TransformDistance(transform, distance));
        }

        // Sort the transformDistances list based on distance in ascending order
        transformDistances.Sort((a, b) => a.Distance.CompareTo(b.Distance));

        // Take the top N transforms based on the number ofClosestObjects
        List<Transform> closestTransforms = transformDistances
            .Take(numberOfClosestObjects)
            .Select(td => td.Transform)
            .ToList();

        return closestTransforms;

    }

}

public static class GenericTypeExtensions
{    

    private static System.Random RANDOM = new System.Random();
    public static int RANDOM_SEED = 13796240;


    #region INTEGER

    public static int Random(this int value, int min, int max)
    {
        return RANDOM.Next(min, max + 1);
    }
    public static int RandomCheat(this int value, int min, int max)
    {
        return (new System.Random(RANDOM_SEED)).Next(min, max + 1);
    }
    public static int RandomSeed(this int value, int min, int max, int seed)
    {
        return (new System.Random(seed)).Next(min, max + 1);
    }

    //gets the 0-1 representation between value and target
    public static int Normalize(this int value, int target)
    {
        // Ensure that value and target are positive.
        int absValue = Mathf.Abs(value);
        int absTarget = Mathf.Abs(target);

        // Calculate the normalization factor based on the magnitude of value and target.
        int factor = absValue < absTarget ? absValue / absTarget : absTarget / absValue;

        // Ensure the result is within the range [0, 1].
        return Mathf.Clamp(factor, 0, 1);
    }
    //gets the 0-1 representation between value's current stance on MIN-MAX range
    public static int Normalize(this int value, int minValue, int maxValue)
    {
        // Ensure that value is within the specified range.
        int result = Mathf.Clamp(value, minValue, maxValue);

        // Calculate the normalized position of value within the range.
        int normalizedValue = Mathf.RoundToInt(Mathf.InverseLerp(minValue, maxValue, result));

        return normalizedValue;
    }


    #endregion


    #region FLOAT

    public static float Random(this float value, float min, float max)
    {
        return (float)RANDOM.NextDouble() * (max - min) + min;
    }
    public static float RandomCheat(this float value, float min, float max)
    {
        return (float)(new System.Random(RANDOM_SEED)).NextDouble() * (max - min) + min;
    }
    public static float RandomSeed(this float value, float min, float max, int seed)
    {
        return (float)(new System.Random(seed)).NextDouble() * (max - min) + min;
    }

    //Faster, but Percentage HAS to be in 0 - 100 range
    public static float PercentageHipster(this float value, float percentage)
    {
        return value * (percentage * 0.01f);
    }
    // Default Percentage
    public static float Percentage(this float value, float percentage)
    {
        return value * (percentage / 100.0f); 
    }

    //gets the 0-1 representation between value and target
    public static float Normalize(this float value, float target, float tolerance = 1e-6f, float minValue = 0.0f)
    {

        // Ensure that value and target are positive.
        float absValue = Mathf.Abs(value);
        float absTarget = Mathf.Abs(target);


        // Handle the case where both values are close to zero.
        if (absValue < tolerance && absTarget < tolerance)
        {
            return minValue; // You can customize this.
        }

        // Calculate the normalization factor based on the magnitude of value and target.
        float factor = absValue < absTarget ? absValue / absTarget : absTarget / absValue;
        return Mathf.Clamp(factor, minValue, 1f);

    }
    //gets the 0-1 representation between value's current stance on MIN-MAX range
    public static float Normalize(this float value, float minValue, float maxValue)
    {
        return Mathf.InverseLerp(minValue, maxValue, value);
    }

    //Smoothly lerps from current value to target value
    ///Example:
    /*
    StartCoroutine(value.Lerp(value, speed, result =>
    {

        // use the result value here
        value = result;

        transform.localScale = new Vector2(_currentScale, _currentScale);

    }));
    */
    public static IEnumerator Lerp(this float value, float targetValue, float speed, Action<float> callback)
    {
        float result = value;
        float time = 0;
        while (time < 1)
        {
            result = Mathf.Lerp(value, targetValue, time);
            time += Time.deltaTime * speed;
            callback(result);
            yield return null;
        }
    }

    //constrain a value within a specified range
    public static float Clamp(this float value, float minValue, float maxValue)
    {
        return Mathf.Clamp(value, minValue, maxValue);
    }
    
    //ensure that the resulting value has at most two decimal places
    public static float Truncate(this float value)
    {
        return (float)(Math.Truncate((double)value * 100.0) / 100.0);
    }
    
    //ensure that the resulting value has at most 'decimal' places
    public static float Truncate(this float value, int decimals)
    {
        double mult = Math.Pow(10.0, decimals);
        double result = Math.Truncate(mult * value) / mult;
        return (float)result;
    }
   
    //rounds the value to the specified number of decimal places
    public static float Round(this float value, int digits)
    {
        return (float)(Math.Round((double)value, digits));
    }

    #endregion


    #region STRING

    public static char GetRandomCharacter(this string value)
    {
        int index = RANDOM.Next(0, value.Length);
        ///int index = UnityEngine.Random.Range(0, reference.Length);
        return value[index];
    }

    public static char[] GetRandomCharacterArray(this string value, int arrayLenght)
    {
        char[] characterArray = new char[arrayLenght];

        for (int i = 0; i < arrayLenght; i++)
        {
            //int index = range.Next(reference.Length);
            ///int index = UnityEngine.Random.Range(0, value.Length);
            int index = RANDOM.Next(0, value.Length);

            characterArray[i] = value[index];
        }
        //return reference[index];

        return characterArray;
    }
    
    public static string Shuffle(this string value)
    {
        string characterArray = "";

        for (int i = 0; i < value.Length; i++)
        {
            //int index = range.Next(reference.Length);
            ///int index = UnityEngine.Random.Range(0, value.Length);
            int index = RANDOM.Next(0, value.Length);

            characterArray = characterArray + value[index];
        }
        //return reference[index];

        return characterArray;
    }

    public static string GetRandomString(int length, bool alphabeticOrder = false)
    {

        if (length <= 0)
        {
            throw new ArgumentException("Length must be greater than zero.");
        }

        char[] letters = Enumerable.Range(0, length)
                                    .Select(_ => GetRandomLetter())
                                    .ToArray();

        if (alphabeticOrder)
        {
            Array.Sort(letters);
        }

        return new string(letters);
    }

    private static char GetRandomLetter()
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        return alphabet[RANDOM.Next(alphabet.Length)];
    }


    public static string RemoveCharacters(this string value, string charactersToRemove)
    {
        return value.Replace(charactersToRemove, "");
    }

    public static string RemoveWhiteSpaces(this string value)
    {
        StringBuilder sb = new StringBuilder();
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    #endregion


    //Guid - Guid merge
    public static Guid MergeGuid(this Guid value, Guid mergeTarget)
    {
        if (mergeTarget != null)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];
            byte[] guid1Byte = value.ToByteArray();
            byte[] guid2Byte = mergeTarget.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }
        else
        {
            return value;
        }
    }
    
    //Guid - string merge
    public static Guid MergeString(this Guid value, string mergeTarget = "")
    {
        if (mergeTarget.Length > 0)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];

            byte[] guid1Byte = value.ToByteArray();
            byte[] guid2Byte = Encoding.ASCII.GetBytes(mergeTarget);

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }
        else
        {
            return value;
        }

    }

    //string - Guid merge
    public static Guid MergeGuid(this string value, Guid mergeTarget)
    {

        if (mergeTarget != null)
        {
            const int BYTECOUNT = 16;
            byte[] destByte = new byte[BYTECOUNT];
            byte[] guid1Byte = Encoding.ASCII.GetBytes(value);
            byte[] guid2Byte = mergeTarget.ToByteArray();

            for (int i = 0; i < BYTECOUNT; i++)
            {
                destByte[i] = (byte)(guid1Byte[i] ^ guid2Byte[i]);
            }
            return new Guid(destByte);
        }
        else
        {
            return mergeTarget;
        }

    }

}

public static class MathExtensions
{

    public static Vector3 GetCentroid(this Vector3[] value)
    {
        Vector3 centroid = new Vector3(0, 0, 0);

        foreach (var point in value)
        {
            centroid += point;
        }

        centroid /= value.Length;

        return centroid;
    }

    public static float GetDiagonalLenght(this Vector3 value, float scale = 1f)
    {
        float diagonalLength = Mathf.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
        return diagonalLength * scale;
    }

    public static float GetCommonDivisor(float valueA, float valueB, int iterations = 10)
    {
        return ApproximateGCD(valueA, valueB, iterations);
    }

    // Calculate Greatest Common Divisor
    public static float ForbiddenGCD(float a, float b, float tolerance = 1e-6f)
    {

        // Ensure that a is greater than or equal to b
        if (a < b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        a = Mathf.Abs(a);
        b = Mathf.Abs(b);

        // Make sure no value is near 0
        if (a < tolerance || b < tolerance)
        {
            return Mathf.Max(a, b);
        }

        // Calculate the GCD using Euclid's algorithm... Accurate, but may cause numerical instability.
        while (b > tolerance)
        {
            float remainder = a % b;
            a = b;
            b = remainder;
        }

        return a;
    }
    // 'Calculate' Greatest Common Divisor
    public static float ApproximateGCD(float a, float b, int maxIterations = 100, float tolerance = 1e-6f)
    {

        // Ensure that a is greater than or equal to b
        if (a < b)
        {
            float temp = a;
            a = b;
            b = temp;
        }

        a = Mathf.Abs(a);
        b = Mathf.Abs(b);

        // Make sure no value is near 0
        if (a < tolerance || b < tolerance)
        {
            return Mathf.Max(a, b);
        }

        // Innacurately calculate the GCD using Euclid's algorithm untill -> maxIterations
        int iterations = 0;
        while (b > tolerance && iterations < maxIterations)
        {
            float remainder = a % b;
            a = b;
            b = remainder;
            iterations++;
        }

        return a;
    }


    public static float ToRadius(this Vector3 value, float baseRadius)
    {
        Vector3 cubeSize = value;//cubeTransform.localScale;
        float cubeVolume = cubeSize.x * cubeSize.y * cubeSize.z;

        float sphereRadius = baseRadius;//sphereTransform.localScale.x; // Assuming the sphere is uniformly scaled
        float sphereVolume = (4f / 3f) * Mathf.PI * sphereRadius * sphereRadius * sphereRadius;

        return Mathf.Pow(cubeVolume / sphereVolume, 1f / 3f) * 0.1f;

        // Set the scale of the sphere
        // sphereTransform.localScale = new Vector3(scale, scale, scale);
    }
    public static Vector3 ToScale(this float value)
    {
        //float sphereVolume = (4f / 3f) * Mathf.PI * radius * radius * radius;
        float sphereVolume = ((4f / 3f) * Mathf.PI * value * value * value);
        float cubeSide = Mathf.Pow(sphereVolume, 1f / 3f);
        Vector3 scale = new Vector3(cubeSide, cubeSide, cubeSide);

        return scale;
    }


    public static float GetTargetDistance(this Transform value, Transform target)
    {
        return Vector3.Distance(value.position, target.position);
        //var closest = array.OrderBy(t => (t.transform.position - gameObject.transform.position).sqrMagnitude).FirstOrDefault();
    }
    public static float GetTargetDistance(this Vector3 value, Vector3 target)
    {
        return Vector3.Distance(value, target);
        //var closest = array.OrderBy(t => (t.transform.position - gameObject.transform.position).sqrMagnitude).FirstOrDefault();
    }
    public static Vector3 Direction(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
    public static Vector3 Middle(this Vector3 firstPosition, Vector3 secondPosition)
    {
        return ((firstPosition + secondPosition) / 2);
    }

    public static Vector3 PredictPositionChange(this Rigidbody value, Vector3 force, float timeInterval)
    {
        Vector3 acceleration = force / value.mass;  // Calculate acceleration
        Vector3 predictedPositionChange = 0.5f * acceleration * (timeInterval * timeInterval);

        return value.position + predictedPositionChange;
    }

    #region External Sources

    public static float GetAngleBetweenPositions(Vector3 firstPosition, Vector3 secondPosition)
    {
        return Vector3.Angle(firstPosition, secondPosition);
    }
    public static float WrapAngle(float angle)
    {

        float result = angle;

        //WrapAngle will convert 270 to -90
        result %= 360;
        if (result > 180)
            return result - 360;

        return result;
    }
    public static float UnwrapAngle(float angle)
    {
        float result = angle;

        //Will convert clamped angle to 270
        if (result >= 0)
            return result;

        result = -result % 360;

        return 360 - result;
    }

    ///Code from JohnStairs.Utils

    //the angle of the rotation from vector A to vector B
    public static float SignedAngle(Vector3 a, Vector3 b, Vector3 normal)
    {
        // Project a and b onto the plane with normal normal
        a = Vector3.ProjectOnPlane(a, normal);
        b = Vector3.ProjectOnPlane(b, normal);
        // Calculate the signed angle between them
        return Vector3.SignedAngle(a, b, normal);
    }

    //Check if angle between vectors is smaller than 1 degree
    public static bool IsAlmostEqual(Vector3 a, Vector3 b)
    {
        return Vector3.Angle(a, b) < 1.0f;
    }
    public static bool IsAlmostEqual(float a, float b, float epsilon)
    {
        return Mathf.Abs(a - b) < epsilon;
    }

    #endregion

}
