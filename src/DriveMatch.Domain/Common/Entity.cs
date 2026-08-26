using DriveMatch.Domain.Exceptions;

namespace DriveMatch.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; protected set; }

    protected Entity(Guid id)
    {
        if (id == Guid.Empty)
            throw new DomainException("O identificador da entidade não pode ser vazio.");

        Id = id;
    }

    protected Entity()
    {
    }
}