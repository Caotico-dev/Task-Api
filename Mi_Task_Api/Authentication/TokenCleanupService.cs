
namespace Mi_Task_Api.Authentication
{
    public class TokenCleanupService : BackgroundService
    {
        private readonly IClearBlackList clearBlackList;
        public TokenCleanupService(IClearBlackList clearBlackList)
        {
            this.clearBlackList = clearBlackList;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (clearBlackList.CountBlackList > 1000)
                {
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);

                    Console.WriteLine("Iniciando proceso de eliminación...");

                    clearBlackList.RemoveThousandItems();

                }
                else
                {
                    Console.WriteLine("Verificando BlackList");
                    await Task.Delay(TimeSpan.FromMinutes(4), stoppingToken);

                }
            }
        }

    }
}
