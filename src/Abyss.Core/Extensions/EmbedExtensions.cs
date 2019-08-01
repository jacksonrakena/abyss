using Abyss.Entities;
using Discord;

namespace Abyss.Extensions
{
    public static class EmbedExtensions
    {
        public static EmbedBuilder WithRequesterFooter(this EmbedBuilder builder, AbyssRequestContext context)
        {
            return builder.WithFooter($"Requested by {context.Invoker.Format()}",
                context.Invoker.GetEffectiveAvatarUrl());
        }
    }
}