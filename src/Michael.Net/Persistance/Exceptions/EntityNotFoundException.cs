using Michael.Net.Domain;
using Michael.Net.Exceptions;

namespace Michael.Net.Persistence.Exceptions
{
    public class EntityNotFoundException<T> : MichaelNetException where T : IEntity
    {
        public EntityNotFoundException(int id)
            : base($"Entity '{nameof(T)}' with id '{id}' was not found.") { }
    }
}
