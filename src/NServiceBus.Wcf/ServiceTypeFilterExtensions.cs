namespace NServiceBus.Hosting.Wcf
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    static class ServiceTypeFilterExtensions
    {
        public static IEnumerable<Type> SelectServiceTypes(this IList<Type> availableTypes, Conventions conventions)
        {
            return availableTypes.Where(t => !t.IsAbstract && IsWcfService(t, conventions));
        }

        static bool IsWcfService(Type t, Conventions conventions)
        {
            var args = t.GetGenericArguments();
            if (args.Length == 2)
            {
                if (conventions.IsMessageType(args[0]))
                {
                    var wcfType = typeof(WcfService<,>).MakeGenericType(args[0], args[1]);
                    if (wcfType.IsAssignableFrom(t))
                    {
                        return true;
                    }
                }
            }

            if (t.BaseType != null)
            {
                return IsWcfService(t.BaseType, conventions) && !t.IsAbstract;
            }

            return false;
        }
    }
}