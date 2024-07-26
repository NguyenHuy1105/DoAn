using ComputerManagement.Common.Configs;
using ComputerManagement.Data;
using ComputerManagement.Service.Queue;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Worker
{
    public class CommandOptionJob : BackgroundService
    {
        private SubcribeCommandOption _listener;
        private RabbitMQConfig _rbConfig;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        public CommandOptionJob(IOptionsMonitor<RabbitMQConfig> rbConfig, IDbContextFactory<AppDbContext> contextFactory)
        {
            _rbConfig = rbConfig.CurrentValue;
            _contextFactory = contextFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
                var dbContext = _contextFactory.CreateDbContext();
                var listener = new SubcribeCommandOption(dbContext);
                listener.ReceiveFromQueue(_rbConfig);
        }
    }
}
