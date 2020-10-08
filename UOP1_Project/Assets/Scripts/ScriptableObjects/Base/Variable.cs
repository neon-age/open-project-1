using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace ScriptableObjects
{
    public abstract class Variable<T> : ScriptableObject
    {
        [SerializeField] 
        private T value;

        public T Value
        {
            get => value;
            set => this.value = value;
        }

        public static implicit operator T(Variable<T> variable) => variable.value;
        
        protected static TInstance CreateFromValue<TInstance>(T value) where TInstance : Variable<T>
        {
            var instance = CreateInstance<TInstance>();
            instance.value = value;
            
            return instance;
        }
    }
}