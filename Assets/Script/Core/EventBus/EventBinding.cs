using System;

namespace CityBuilder.EventBus
{
    /// <summary>
    /// Binds one or more callback methods to an event type.
    /// Create in OnEnable, register with <see cref="EventBus{T}.Register"/>,
    /// deregister in OnDisable with <see cref="EventBus{T}.Deregister"/>.
    ///
    /// <example>
    /// <code>
    /// EventBinding&lt;StockChangedEvent&gt; _binding;
    ///
    /// void OnEnable() {
    ///     _binding = new EventBinding&lt;StockChangedEvent&gt;(OnStock);
    ///     EventBus&lt;StockChangedEvent&gt;.Register(_binding);
    /// }
    /// void OnDisable() =&gt; EventBus&lt;StockChangedEvent&gt;.Deregister(_binding);
    ///
    /// void OnStock(StockChangedEvent e) { … }
    /// </code>
    /// </example>
    /// </summary>
    public class EventBinding<T> : IEventBinding<T> where T : IEvent
    {
        Action<T> _onEvent       = delegate { };
        Action    _onEventNoArgs = delegate { };

        Action<T> IEventBinding<T>.OnEvent       { get => _onEvent;       set => _onEvent       = value; }
        Action    IEventBinding<T>.OnEventNoArgs { get => _onEventNoArgs; set => _onEventNoArgs = value; }

        public EventBinding(Action<T> onEvent)       => _onEvent       = onEvent;
        public EventBinding(Action    onEventNoArgs) => _onEventNoArgs = onEventNoArgs;

        public void Add(Action<T> handler)    => _onEvent       += handler;
        public void Remove(Action<T> handler) => _onEvent       -= handler;
        public void Add(Action handler)       => _onEventNoArgs += handler;
        public void Remove(Action handler)    => _onEventNoArgs -= handler;
    }
}
