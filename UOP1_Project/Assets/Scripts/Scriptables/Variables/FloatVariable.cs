using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Variables/Float"), Serializable]
    public class FloatVariable : Variable<float>
    {
        public static implicit operator FloatVariable(float value) => CreateFromValue<FloatVariable>(value);
    }
    
    [Serializable]
    public class FloatReference : Reference<FloatVariable, float>{}
}