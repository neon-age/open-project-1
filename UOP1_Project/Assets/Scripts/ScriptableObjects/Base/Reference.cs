using System;
using UnityEngine;

namespace ScriptableObjects
{
    [Serializable]
    public abstract class Reference<TVariable, TValue> where TVariable : Variable<TValue>
    {
        [SerializeField] private bool isUnique;
        [SerializeField] private TVariable variable;
        [SerializeField] private TValue uniqueValue;

        public TValue Value
        {
            get => isUnique ? uniqueValue : variable.Value;
            set
            {
                if (isUnique)
                    uniqueValue = value;
                else
                    variable.Value = value;
            }
        }
    }
}