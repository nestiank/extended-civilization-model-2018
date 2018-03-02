using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Provides <see cref="IGameScheme"/> management.
    /// </summary>
    public sealed class SchemeLoader
    {
        /// <summary>
        /// The schema tree. The item whose index is smaller is ancestor.
        /// </summary>
        /// <remarks>
        /// Although the item whose index is smaller is ancestor, the index of <see cref="RootScheme"/> can be not zero.
        /// </remarks>
        public IReadOnlyList<IGameScheme> SchemaTree => _schemaTree;
        private List<IGameScheme> _schemaTree = new List<IGameScheme>();

        /// <summary>
        /// The root scheme of <see cref="SchemaTree"/>.
        /// </summary>
        /// <seealso cref="SchemaTree"/>
        public IGameScheme RootScheme => SchemaTree[1];

        private DefaultScheme _defaultScheme = new DefaultScheme();

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemeLoader"/> class.
        /// </summary>
        /// <param name="rootFactory">The root factory.</param>
        /// <param name="knownSchemes">
        /// The known schemes.
        /// If <c>null</c>, use only <paramref name="rootFactory"/> and those <paramref name="rootFactory"/> provides.
        /// </param>
        public SchemeLoader(IGameSchemeFactory rootFactory, IEnumerable<IGameSchemeFactory> knownSchemes = null)
        {
            Load(rootFactory, knownSchemes);
        }

        /// <summary>
        /// Loads the specified scheme factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="knownSchemes">
        /// The known schemes.
        /// If <c>null</c>, use only <paramref name="factory"/> and those <paramref name="factory"/> provides.
        /// </param>
        public void Load(IGameSchemeFactory factory, IEnumerable<IGameSchemeFactory> knownSchemes = null)
        {
            var set = knownSchemes != null ? new HashSet<IGameSchemeFactory>(knownSchemes) : null;
            RecursiveLoad(factory, set, new HashSet<Guid>());
        }

        private void RecursiveLoad(IGameSchemeFactory factory, HashSet<IGameSchemeFactory> knownSchemes, HashSet<Guid> loadingSet)
        {
            loadingSet.Add(factory.Guid);

            foreach (var known in factory.KnownSchemeFactories)
            {
                knownSchemes.Add(known);
            }

            foreach (var dep in factory.Dependencies)
            {
                if (loadingSet.Contains(dep))
                    throw new InvalidOperationException("circular scheme dependency is detected");
                if (_schemaTree.Any(s => s.Factory?.Guid == dep))
                    continue;

                var depfac = knownSchemes.Where(f => f.Guid == dep).First();
                RecursiveLoad(depfac, knownSchemes, loadingSet);
            }

            _schemaTree.Add(factory.Create());
            loadingSet.Remove(factory.Guid);
        }

        /// <summary>
        /// Gets the applied exclusive scheme.
        /// </summary>
        /// <typeparam name="T">The type of exclusive scheme.</typeparam>
        /// <returns>The applied scheme. <c>null</c> if there is no scheme of given type.</returns>
        public T GetExclusiveScheme<T>() where T : class, IGameScheme
        {
            var ret = SchemaTree.Reverse().OfType<T>().FirstOrDefault();
            if (ret == null)
            {
                if (_defaultScheme is T)
                    return (T)(IGameScheme)_defaultScheme;
                else
                    return null;
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// Gets the list of overlappable schemes.
        /// </summary>
        /// <typeparam name="T">The type of overlappable scheme.</typeparam>
        /// <returns>The list of overlappable schemes</returns>
        public IEnumerable<T> GetOverlappableScheme<T>() where T : class, IGameScheme
        {
            return SchemaTree.OfType<T>();
        }
    }

    sealed class DefaultScheme : IGameConstantScheme, IGameStartupScheme
    {
        // not used.
        IGameSchemeFactory IGameScheme.Factory => null;
        void IGameScheme.OnAfterInitialized(Game game) => throw new NotImplementedException();

        public bool OnlyDefaultPlayers => false;
        public int DefaultNumberOfPlayers => 2;

        public bool OnlyDefaultTerrain => false;
        public int DefaultTerrainWidth => 128;
        public int DefaultTerrainHeight => 80;

        public double GoldCoefficient => 1;

        public double PopulationConstant => 0.1;
        public double PopulationHappinessCoefficient => 0.01;

        public double HappinessCoefficient => 1;

        public double LaborHappinessCoefficient => 0.008;
        public double ResearchHappinessCoefficient => 0.005;

        public double EconomicRequireCoefficient => 0.2;
        public double EconomicRequireTaxRateConstant => 0.2;

        public double ResearchRequireCoefficient => 0.2;

        public IEnumerable<IProductionFactory> AdditionalProductionFactory => null;

        public void InitializeGame(Game game, bool isNewGame) { }
    }
}
