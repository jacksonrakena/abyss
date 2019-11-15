using Disqord;
using System;
using System.Threading.Tasks;

namespace Abyss
{
    public class BadRequestResult : AbyssResult
    {
        public static readonly Color ErrorColor = Color.Red;

        public BadRequestResult(string reason)
        {
            Reason = reason;
        }

        public string Reason { get; set; }

        public override bool IsSuccessful => false;

        public override Task<bool> ExecuteResultAsync(AbyssCommandContext context)
        {
            return context.Channel.TrySendMessageAsync(null, false, new LocalEmbedBuilder()
                .WithTitle("Bad request")
                .WithDescription(Reason)
                .WithColor(ErrorColor)
                .WithTimestamp(DateTimeOffset.Now)
                .WithFooter($"Requested by: {context.Invoker.Format()}", context.Invoker.GetAvatarUrl())
                .Build());
        }

        public override object ToLog()
        {
            return new
            {
                Reason
            };
        }
    }
}