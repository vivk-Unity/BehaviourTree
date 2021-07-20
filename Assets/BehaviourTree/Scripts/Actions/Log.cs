using BehaviourTree.Scripts.Tree;
using UnityEngine;

namespace BehaviourTree.Scripts.Actions
{
    public class Log : ActionNode
    {
        [SerializeField] private string _message;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            Debug.Log($"{_message}");
            return State.Success;
        }
    }
}
