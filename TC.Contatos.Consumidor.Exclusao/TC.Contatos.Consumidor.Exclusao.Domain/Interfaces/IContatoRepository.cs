using Domain.RegionalAggregate;

namespace Domain.Interfaces
{
    public interface IContatoRepository
    {
        void Remover(Contato contato);
        Contato ObterPorId(Guid id);
    }
}
