using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MovementManagerWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private MemoryMappedFile _memoryMappedFile;
        private readonly IConfiguration _configuration;
        private int _currentCompletedStep, _currentTotalStep;


        public Worker(ILogger<Worker> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _memoryMappedFile = CreateMemoryMappedFile();
        }

        private string MemoryMappedFileName => _configuration.GetValue<string>("mapName");

        private MemoryMappedFile CreateMemoryMappedFile()
        {
            if ( string.IsNullOrEmpty( MemoryMappedFileName ) )
            {
                throw new ArgumentNullException("mapName");
            }
            return MemoryMappedFile.OpenExisting( MemoryMappedFileName );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                PrintCompletedStep();
                // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

       public override Task StopAsync(CancellationToken cancellationToken)
       {
           try 
           {
                _memoryMappedFile.Dispose();
           }
           catch (Exception)
           {
               //
           }
           return base.StopAsync(cancellationToken);
       }
        int[] buffer = new int[255];
        private void PrintCompletedStep()
        {
            using (var accessor = _memoryMappedFile.CreateViewAccessor(0, 2000000, MemoryMappedFileAccess.Read))
            {
                accessor.ReadArray<int>(0, buffer, 0, 2);
                int completedStep = buffer[0];
                int totalStep = buffer[1];
                if ( _currentCompletedStep == completedStep && _currentTotalStep == totalStep )
                {
                   // _logger.LogInformation("SKIP");
                   // return;
                }
                _currentCompletedStep = completedStep;
                _currentTotalStep = totalStep;
                _logger.LogInformation($"Progress: { completedStep } / { totalStep } ");
            }
        }
    }
}
