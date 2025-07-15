using Industrica.Network.Container;
using Industrica.Network.Filter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Network.Systems
{
    public abstract class Network<T, R, W> : MonoBehaviour where R : Network<T, R, W>.NetworkConnection where W : Network<T, R, W>.ContainerWrapper<R>, new()
    {
        private float destroyTimer = 0f;
        private bool lockDestruction = false;
        protected readonly W input = new();
        protected readonly W output = new();
        private bool IsEmpty => input.IsEmpty && output.IsEmpty;

        public bool LockDestruction { set => lockDestruction = value; }

        protected R Register(PortType port, R connection)
        {
            switch (port)
            {
                case PortType.Input:
                    input.Add(connection);
                    break;
                case PortType.Output:
                    output.Add(connection);
                    break;
            }

            return connection;
        }

        protected void Deregister(PortType port, R connection)
        {
            switch (port)
            {
                case PortType.Input:
                    input.Remove(connection);
                    break;
                case PortType.Output:
                    output.Remove(connection);
                    break;
            }

            StartDestroyTimer();
        }

        public bool TryInsert(T value)
        {
            foreach (R connection in input)
            {
                if (connection.Container.TryInsert(value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryExtract(NetworkFilter<T> filter, out T value)
        {
            foreach (R connection in output)
            {
                if (connection.Container.TryExtract(filter, out value))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

        public bool Contains(NetworkFilter<T> filter, out bool canExtract)
        {
            foreach (R connection in output)
            {
                if (connection.Container.Contains(filter, out canExtract))
                {
                    return true;
                }
            }

            canExtract = default;
            return false;
        }

        public void StartDestroyTimer()
        {
            destroyTimer = DestroyTimeout;
        }

        public virtual void Update()
        {
            UpdateDestroyTimer();
        }

        private void UpdateDestroyTimer()
        {
            if (destroyTimer <= 0f || !IsEmpty)
            {
                destroyTimer = 0f;
                return;
            }

            if (lockDestruction)
            {
                return;
            }

            // If the network was empty for long enough, it's probably not going to be used anymore and can be cleaned up
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0f)
            {
                GameObject.Destroy(gameObject);
            }
        }

        public static float DestroyTimeout = 5f;

        public static IEnumerator CreateNetwork<N>(TechType tech, Action<N> postLoadAction) where N : Network<T, R, W>
        {
            CoroutineTask<GameObject> request = CraftData.GetPrefabForTechTypeAsync(tech, false);
            yield return request;

            GameObject result = request.GetResult();
            if (result == null)
            {
                ErrorMessage.AddMessage($"Could not spawn {nameof(ItemPhysicalNetwork)}. Discarding");
                yield break;
            }

            GameObject network = GameObject.Instantiate(result);
            LargeWorldEntity.Register(network);
            network.transform.position = Vector3.one; // make it save, position does not matter but has to not be (0, 0, 0) (thank you subnautica)

            postLoadAction.Invoke(network.GetComponent<N>());
        }

        public abstract record NetworkConnection(
            Network<T, R, W> Network,
            PortType Type,
            Container<T> Container)
        {
            public void Deregister()
            {
                Network.Deregister(Type, this as R);
            }
        }

        public abstract class ContainerWrapper<C> where C : NetworkConnection
        {
            public abstract void Add(C c);
            public abstract void Remove(C c);
            public abstract bool IsEmpty { get; }
            public abstract IEnumerator<C> GetEnumerator();
        }
    }
}
