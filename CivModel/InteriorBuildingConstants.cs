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
    public class InteriorBuildingConstants
    {
        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <seealso cref="CityBase.Labor"/>
        public double ProvidedLabor { get; set; } = 0;
        /// <summary>
        /// The amount of research this building provides.
        /// </summary>
        /// <seealso cref="CityBase.ResearchIncome"/>
        public double ProvidedResearchIncome { get; set; } = 0;
    }
}
