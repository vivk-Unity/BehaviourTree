using System.Linq;
using UnityEngine;

namespace BehaviourTree.Scripts.Blackboard
{
    public static class BlackboardUtils
    {
        public static void ConnectTo(object source, IBlackboard blackboard)
        {
            var sourceType = source.GetType();
            var fieldInfos = sourceType.GetFields();
            var variableInterface = typeof(IVariableCore);
            foreach (var fInfo in fieldInfos)
            {
                var fType = fInfo.FieldType;
                var fInterfaces = fType.GetInterfaces();
                var isVariable = fInterfaces.Contains(variableInterface);
                if (isVariable)
                {
                    if (fInfo.GetValue(source) is IVariableCore variable)
                    {
                        variable.Setup(blackboard);
                        variable.Connect();
                    }
                }
            }
        }
    }
}