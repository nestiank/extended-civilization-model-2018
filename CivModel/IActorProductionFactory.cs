using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="Production"/> which products <see cref="Actor"/> objects.
    /// This interface also provides <see cref="ActorConstants"/> of production results.
    /// </summary>
    /// <seealso cref="CivModel.ActorConstants"/>
    /// <seealso cref="ITileObjectProductionFactory"/>
    public interface IActorProductionFactory : ITileObjectProductionFactory
    {
        /// <summary>
        /// The constants of production result <see cref="Actor"/>.
        /// </summary>
        /// <seealso cref="ActorConstants"/>
        ActorConstants ActorConstants { get; }
    }
}
