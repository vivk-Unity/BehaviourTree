using BehaviourTree.Scripts.Tree;
using UnityEngine;

namespace BehaviourTree.Scripts.Actions
{
    public class Random : ActionNode
    {
        [Range(0, 1), SerializeField] private float _odds = 0.5f;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            var value = UnityEngine.Random.value;
            return value < _odds 
                ? State.Success 
                : State.Failure;
        }
    }
}