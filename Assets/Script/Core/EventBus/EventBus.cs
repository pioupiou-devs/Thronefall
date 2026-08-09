using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// Generic, type-safe, static event bus.
    /// One bus is instantiated per <typeparamref name="T"/> event type at runtime.
    ///
    /// Usage:
    ///   Raise  — <c>EventBus&lt;MyEvent&gt;.Raise(new MyEvent { … });</c>
    ///   Listen — Create an <see cref="EventBinding{T}"/>, call <see cref="Register"/>,
    ///            and <see cref="Deregister"/> when done.
    /// </summary>
    public static class EventBus<T> where T : IEvent
    {
        static readonly HashSet<IEventBinding<T>> _bindings = new();

        public static void Register(EventBinding<T> binding)   => _bindings.Add(binding);
        public static void Deregister(EventBinding<T> binding) => _bindings.Remove(binding);

        public static void Raise(T @event)
        {
            // Snapshot prevents mutation-during-iteration when handlers register/deregister
            var snapshot = new HashSet<IEventBinding<T>>(_bindings);
            foreach (var binding in snapshot)
            {
                if (_bindings.Contains(binding))
                {
                    binding.OnEvent.Invoke(@event);
                    binding.OnEventNoArgs.Invoke();
                }
            }
        }

        /// <summary>Used by <see cref="EventBusUtil.ClearAllBuses"/> on play-mode exit.</summary>
        internal static void Clear()
        {
            Debug.Log($"[EventBus] Clearing {typeof(T).Name} bindings");
            _bindings.Clear();
        }
    }
