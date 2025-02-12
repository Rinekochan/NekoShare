using server.Enums;

namespace server.Exceptions;

public class ItemNotFoundException : Exception
{
    public EntityEnum Type { get; set; }

    public ItemNotFoundException()
    {
    }

    public ItemNotFoundException(string message) : base(message)
    {
    }

    public ItemNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public ItemNotFoundException(string message, EntityEnum type) : this(message)
    {
        Type = type;
    }
}