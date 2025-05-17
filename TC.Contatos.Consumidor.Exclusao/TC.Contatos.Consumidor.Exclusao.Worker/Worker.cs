using Domain.Interfaces;
using UseCase.Interfaces;

namespace Worker
{
    public class WorkerService : BackgroundService
    {
        private readonly IMessageConsumer _messageConsumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public WorkerService(IMessageConsumer messageConsumer,
                      IServiceScopeFactory scopefactory)
        {
            _messageConsumer = messageConsumer;
            _scopeFactory = scopefactory;

            _messageConsumer.OnMessageReceived += ProcessarMensagem;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _messageConsumer.ConsumeAsync();
        }
        private async Task ProcessarMensagem(Guid id)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var removerContatoUseCase = scope.ServiceProvider.GetRequiredService<IRemoverContatoUseCase>();

                removerContatoUseCase.Remover(id);
            }

            await Task.CompletedTask;
        }
    }
}
