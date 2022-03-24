﻿using System;
using System.Linq;
using Disqord;
using Disqord.Gateway;

namespace Abyss.Extensions;

/// <summary>
///     Extensions for dealing with Discord users.
/// </summary>
public static class UserExtensions
{
    /// <summary>
    ///     Returns the highest role colour of the specified user, or the default embed colour.
    /// </summary>
    /// <param name="normalUser">The user to retrieve role colours from.</param>
    /// <returns>The highest role colour of the specified user, or the default embed colour.</returns>
    public static Color GetHighestRoleColourOrDefault(this IUser normalUser)
    {
        return GetHighestRoleColour(normalUser) ?? Constants.Theme;
    }

    /// <summary>
    ///     Returns the highest role colour of the specified user, or null if the user has no colour.
    /// </summary>
    /// <param name="normalUser"></param>
    /// <returns>The colour of the user, or null.</returns>
    public static Color? GetHighestRoleColour(this IUser normalUser)
    {
        if (normalUser is not CachedMember user) return null;
        var orderedRoles = user.GetHighestRoleOrDefault(r => r.Color != null && r.Color.Value.RawValue != 0);
        return orderedRoles?.Color;
    }

    /// <summary>
    ///     Gets the highest role of the user that matches a predicate.
    /// </summary>
    /// <param name="user">The user to check.</param>
    /// <param name="predicate">The predicate of which to use to filter the roles of the user.</param>
    /// <returns>The highest role of the user that matches a predicate.</returns>
    public static IRole GetHighestRoleOrDefault(this IMember user, Func<IRole, bool>? predicate = null)
    {
        return user.GetRoles().Values.OrderByDescending(r => r.Position).FirstOrDefault(predicate ?? (d => true));
    }
}