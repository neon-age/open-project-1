using System;
using Audio;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu]
    public class EnemyTestData : ScriptableObject
    {
        public FloatVariable energy;
        public FloatVariable damage;
        public FloatVariable speed;
        public FloatVariable sightRange;
        public FloatVariable gravityModifier;

        private void Awake()
        {
            
        }
    }
}