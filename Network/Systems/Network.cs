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
        private float elapsedSincePump = 0f;
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

        public bool TryInsert(T value, bool simulate = false)
        {
            foreach (R connection in input)
            {
                if (connection.Container.TryInsert(value, simulate))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryExtract(NetworkFilter<T> filter, out T value, bool simulate = false)
        {
            foreach (R connection in output)
            {
                if (connection.Container.TryExtract(filter, out value, simulate))
                {
                    return true;
                }
            }

            value = default;
            return false;
        }

        public int Count(NetworkFilter<T> filter, PortType from)
        {
            int count = 0;

            if (from.HasFlag(PortType.Input))
            {
                foreach (R connection in input)
                {
                    count += connection.Container.Count(filter);
                }
            }

            if (from.HasFlag(PortType.Output))
            {
                foreach (R connection in output)
                {
                    count += connection.Container.Count(filter);
                }
            }

            return count;
        }

        public int CountRemovable(NetworkFilter<T> filter)
        {
            int count = 0;

            foreach (R connection in output)
            {
                count += connection.Container.CountRemovable(filter);
            }

            return count;
        }

        public void Sync(Network<T, R, W> network)
        {
            elapsedSincePump = network.elapsedSincePump;
        }

        public void Start()
        {
            StartDestroyTimer();
        }

        public void StartDestroyTimer()
        {
            destroyTimer = DestroyTimeout;
        }

        public virtual void Update()
        {
            UpdatePump();
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

        private void UpdatePump()
        {
            elapsedSincePump += DayNightCycle.main.deltaTime;
            if (elapsedSincePump < PumpInterval)
            {
                return;
            }

            elapsedSincePump -= PumpInterval;

            OnPump?.Invoke();
        }

        public const float PumpInterval = 5f;
        public const float DestroyTimeout = 5f;

        public delegate void PumpNotification();
        public event PumpNotification OnPump;

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
