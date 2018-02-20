using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a constants storage of <see cref="InteriorBuilding"/>.
    /// </summary>
    /// <seealso cref="InteriorBuilding"/>
    public interface IInteriorBuildingConstants
    {
        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <seealso cref="CityBase.Labor"/>
        double ProvidedLabor { get; }
        /// <summary>
        /// The amount of research this building provides.
        /// </summary>
        /// <seealso cref="CityBase.ResearchIncome"/>
        double ProvidedResearchIncome { get; }
    }

    /// <summary>
    /// Represents a default constants starage of <see cref="InteriorBuilding"/>.
    /// </summary>
    /// <seealso cref="IInteriorBuildingConstants"/>
    /// <seealso cref="InteriorBuilding"/>
    public class InteriorBuildingConstants : IInteriorBuildingConstants
    {
        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <seealso cref="CityBase.Labor"/>
        public virtual double ProvidedLabor => 0;
        /// <summary>
        /// The amount of research this building provides.
        /// </summary>
        /// <seealso cref="CityBase.ResearchIncome"/>
        public virtual double ProvidedResearchIncome => 0;
    }
}
