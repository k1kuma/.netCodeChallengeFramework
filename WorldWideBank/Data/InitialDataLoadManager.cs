using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorldWideBankSample.Services;

namespace WorldWideBankSample.Data
{
    public static class InitialDataLoadManager
    {
        public static IHost LoadInitialData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var loadInitialDataCommand = scope.ServiceProvider.GetRequiredService<ILoadInitialDataCommand>();
                loadInitialDataCommand.Execute();
            }
            return host;
        }
    }
}
