using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// The factory interface of <see cref="Production"/> which products <see cref="Actor"/> objects.
    /// </summary>
    /// <seealso cref="ITileObjectProductionFactory"/>
    public interface IActorProductionFactory : ITileObjectProductionFactory
    {
    }
}
