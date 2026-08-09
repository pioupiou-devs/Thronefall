using System;

namespace CityBuilder.EventBus
{
    /// <summary>
    /// Read-only contract for an event binding, used internally by <see cref="EventBus{T}"/>.
    /// </summary>
    public interface IEventBinding<T> where T : IEvent
    {
        Action<T> OnEvent       { get; set; }
        Action    OnEventNoArgs { get; set; }
    }
}
