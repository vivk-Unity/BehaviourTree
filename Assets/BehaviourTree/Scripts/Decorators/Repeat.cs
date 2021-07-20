using System;
using BehaviourTree.Scripts.Tree;
using UnityEngine;

namespace BehaviourTree.Scripts.Decorators
{
    public class Repeat : DecoratorNode
    {
        [SerializeField] private bool _restartOnSuccess = true;
        [SerializeField] private bool _restartOnFailure = false;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate(float deltaTime)
        {
            switch (Child.UpdateCustom(deltaTime))
            {
                case State.Running:
                    break;
                case State.Failure:
                    return _restartOnFailure 
                        ? State.Running 
                        : State.Failure;
                case State.Success:
                    return _restartOnSuccess 
                        ? State.Running 
                        : State.Success;
                default:
                    break;
            }

            return State.Running;
        }
    }
}