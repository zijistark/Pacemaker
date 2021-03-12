using System.Collections.Generic;

using TaleWorlds.Core;

namespace Pacemaker.Extensions
{
    internal static class EnumerableExtensions
    {
        public static T RandomPick<T>(this IEnumerable<T> e) => e.GetRandomElementInefficiently();
    }
}
