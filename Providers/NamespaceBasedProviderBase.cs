using System;
using System.Linq;
using System.Reflection;

namespace SteamGuard.Providers
{
    /// <summary>
    /// Implements the reflection wrapper for use classes simplify.
    /// </summary>
    /// <typeparam name="T">Interface with namespace in request behavior</typeparam>
    public abstract class NamespaceBasedProviderBase<T> where T : class
    {
        private static Type[] _types = null;

        /// <summary>
        /// Types which was found and filter in namespace.
        /// </summary>
        public static Type[] Types
        {
            get
            {
                if (_types == null)
                    _types = ExtractTypes();

                return _types;
            }
        }

        private static T[] _instances = null;

        /// <summary>
        /// Instances of defined <c>types</c>.
        /// </summary>
        public static T[] Instances
        {
            get
            {
                if (_instances == null)
                    _instances = CreateInstances(Types);

                return _instances;
            }
        }


        /// <summary>
        /// Finds an instance with type <typeparamref name="G"/>.
        /// </summary>
        /// <typeparam name="G">Required type.</typeparam>
        /// <returns>The instance object.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static G Get<G>() => Instances.OfType<G>().First();

        /// <summary>
        /// Finds an instance with specific <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Required type.</param>
        /// <returns>The instance object.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static object Get(Type type) => Instances.First(t => t.GetType().Equals(type));


        private protected static Type[] ExtractTypes()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(t => string.Equals(t.Namespace, typeof(T).Namespace, StringComparison.Ordinal));
            return types.Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(typeof(T))).ToArray();
        }

        private protected static T[] CreateInstances(params Type[] types)
        {
            return types.Select(t => (T)Activator.CreateInstance(t)).ToArray();
        }
    }
}
