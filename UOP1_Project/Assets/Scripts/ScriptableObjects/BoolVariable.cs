using System;

namespace ScriptableObjects
{
    [Serializable]
    public class BoolVariable : Variable<bool>{}
    
    [Serializable]
    public class BoolReference : Reference<BoolVariable, bool>{}
}