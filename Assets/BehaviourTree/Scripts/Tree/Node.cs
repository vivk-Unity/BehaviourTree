using UnityEngine;

namespace BehaviourTree.Scripts.Tree
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [HideInInspector] public State CurrentState = State.Running;
        [HideInInspector] public bool Started = false;
        [HideInInspector] public string GUID;
        [HideInInspector] public Vector2 Position;
        [TextArea] public string Description;

        public State UpdateCustom(float deltaTime)
        {
            if (!Started)
            {
                OnStart();
                Started = true;
            }

            CurrentState = OnUpdate(deltaTime);

            if (CurrentState != State.Running)
            {
                OnStop();
                Started = false;
            }

            return CurrentState;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        public void Abort()
        {
            BehaviourTree.Traverse(this, (node) =>
            {
                node.Started = false;
                node.CurrentState = State.Running;
                node.OnStop();
            });
        }

        public virtual void OnDrawGizmos()
        {
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate(float deltaTime);
    }
}