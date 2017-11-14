#pragma once

#include "Screen.h"

namespace FakeView
{
    ref class View : public IView
    {
    public:
        explicit View(Screen* screen);

        virtual void Render();
        virtual void OnKeyStroke(int ch);
        virtual void OnTick();

    private:
        CivModel::Game^ m_game = gcnew CivModel::Game(100, 100);

        int m_sightx = 0;
        int m_sighty = 0;

        Screen* m_screen;
    };
}
