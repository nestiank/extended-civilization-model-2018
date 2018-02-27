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
        /// The amount of gold logistics of this actor.
        /// </summary>
        public double GoldLogistics { get; set; } = 0;
        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        public double ProvidedLabor { get; set; } = 0;
        /// <summary>
        /// The amount of research capacity this building provides.
        /// </summary>
        public double ResearchCapacity { get; set; } = 0;
        /// <summary>
        /// The amount of research income per turn this building provides.
        /// </summary>
        public double ResearchIncome { get; set; } = 0;

        /// <summary>
        /// Create the copy of this object.
        /// </summary>
        /// <returns>The copy of this object.</returns>
        public InteriorBuildingConstants Clone()
        {
            return (InteriorBuildingConstants)MemberwiseClone();
        }
    }
}
