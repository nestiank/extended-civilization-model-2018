using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivModel
{
    /// <summary>
    /// Represents a building which must be built in <see cref="CityBase"/>.
    /// </summary>
    /// <seealso cref="TileBuilding"/>
    public abstract class InteriorBuilding : ITurnObserver, IGuidTaggedObject
    {
        /// <summary>
        /// The unique identifier of this class.
        /// </summary>
        public abstract Guid Guid { get; }

        /// <summary>
        /// The <see cref="Player"/> who owns this building.
        /// </summary>
        public Player Owner => _owner;
        private Player _owner;

        /// <summary>
        /// The <see cref="CityBase"/> where this building is.
        /// </summary>
        public CityBase City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    if (value != null && value.Owner != Owner)
                        throw new ArgumentException("the owner of city is different from the owner of building.", "City");

                    if (_city != null)
                    {
                        _city.RemoveBuilding(this);
                        _city = null;
                    }
                    if (value != null)
                    {
                        value.AddBuilding(this);
                        _city = value;
                    }
                }
            }
        }
        private CityBase _city = null;

        /// <summary>
        /// The amount of labor this building provides.
        /// </summary>
        /// <seealso cref="CityBase.Labor"/>
        public virtual double ProvidedLabor => 0;

        /// <summary>
        /// The amount of research this building provides.
        /// </summary>
        /// <seealso cref="CityBase.Research"/>
        public virtual double ProvidedResearch => 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="InteriorBuilding"/> class.
        /// </summary>
        /// <param name="city">The <see cref="CityBase"/> who will own the building.</param>
        /// <exception cref="ArgumentNullException"><paramref name="city"/> is <c>null</c>.</exception>
        public InteriorBuilding(CityBase city)
        {
            _owner = city.Owner;
            City = city ?? throw new ArgumentNullException("city");
        }

        /// <summary>
        /// Process the logic to do at the creation of this actor.
        /// This method should not be called when this <see cref="Actor"/> object is created by loading a save file.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="ProcessCreation"/> has already been called</exception>
        /// <remarks>
        /// If <see cref="Actor"/> is newly created in game logic, such as <see cref="Production"/>, the creator should call this method.
        /// </remarks>
        /// <seealso cref="OnProcessCreation"/>
        public void ProcessCreation()
        {
            if (_processCreationAlreadyCalled)
                throw new InvalidOperationException("ProcessCreation has already been called");

            _processCreationAlreadyCalled = true;
            OnProcessCreation();
        }

        private bool _processCreationAlreadyCalled = false;

        /// <summary>
        /// Called when <see cref="ProcessCreation"/> is called.
        /// This method is not called when this <see cref="Actor"/> object is created by loading a save file.
        /// </summary>
        /// <seealso cref="ProcessCreation"/>
        protected virtual void OnProcessCreation()
        {
        }

        /// <summary>
        /// Destroys this building. <see cref="OnBeforeDestroy"/> is called before the building is destroyed.
        /// </summary>
        /// <remarks>
        /// <strong>postcondition</strong>:
        /// <c><see cref="TileObject.PlacedPoint"/> == null &amp;&amp; <see cref="Owner"/> == null</c>
        /// </remarks>
        public void Destroy()
        {
            OnBeforeDestroy();
            City  = null;
            _owner = null;
        }

        /// <summary>
        /// Called before [destroy], by <see cref="Destroy"/>
        /// </summary>
        protected virtual void OnBeforeDestroy()
        {
        }

        /// <summary>
        /// Called after a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PostPlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called after a turn.
        /// </summary>
        public virtual void PostTurn()
        {
        }

        /// <summary>
        /// Called before a sub turn.
        /// </summary>
        /// <param name="playerInTurn">The player which the sub turn is dedicated to.</param>
        public virtual void PrePlayerSubTurn(Player playerInTurn)
        {
        }

        /// <summary>
        /// Called before a turn.
        /// </summary>
        public virtual void PreTurn()
        {
        }
    }
}
