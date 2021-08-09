using System;
using System.Threading;
using System.Threading.Tasks;
using Abyss.Parsers;
using Abyss.Persistence.Relational;
using Disqord;
using Disqord.Bot;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qmmands;

namespace Abyss
{
    public class Abyss : DiscordBot
    {
        public Abyss(IOptions<DiscordBotConfiguration> options, ILogger<Abyss> logger, IServiceProvider services,
            DiscordClient client) : base(options, logger, services, client)
        {
            Commands.CommandExecuted += CommandExecutedAsync;
        }

        private async Task CommandExecutedAsync(CommandExecutedEventArgs e)
        {
            var database = Services.GetRequiredService<AbyssPersistenceContext>();
            var record = await database
                .GetUserAccountsAsync((e.Context as DiscordCommandContext).Author.Id);
            record.LatestInteraction = DateTimeOffset.Now;
            record.FirstInteraction ??= DateTimeOffset.Now;
            await database.SaveChangesAsync();
        }

        protected override ValueTask AddTypeParsersAsync(CancellationToken cancellationToken = new())
        {
            Commands.AddTypeParser(new UriTypeParser());
            return base.AddTypeParsersAsync(cancellationToken);
        }

        protected override async ValueTask AddModulesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await Services.GetRequiredService<AbyssPersistenceContext>().Database.MigrateAsync(cancellationToken);
            await base.AddModulesAsync(cancellationToken);
        }
    }
}