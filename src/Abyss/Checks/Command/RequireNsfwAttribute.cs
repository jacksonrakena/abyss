using Qmmands;
using System;
using System.Threading.Tasks;

namespace Abyss
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class RequireNsfwAttribute : CheckAttribute, IAbyssCheck
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            var discordContext = context.ToRequestContext();

            return !discordContext.Channel.IsNsfw
                ? new CheckResult("This command can only be used in an NSFW channel.")
                : CheckResult.Successful;
        }

        public string GetDescription(AbyssRequestContext context) => "We must be in an NSFW channel.";
    }
}