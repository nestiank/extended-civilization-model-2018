using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivModel;

namespace CivPresenter
{
    public enum Direction
    {
        Up, Down, Left, Right
    }

    public interface IView
    {
        void Refocus();
        void MoveSight(int dx, int dy);

        void Shutdown();
    }
}
