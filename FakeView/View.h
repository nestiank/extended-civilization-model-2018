#pragma once

#include "Screen.h"

namespace FakeView
{
    ref class View : public CivPresenter::IView, public IScreenClient
    {
    public:
        explicit View(Screen* screen);

        virtual void Refocus();
        virtual void MoveSight(int dx, int dy);
        virtual void Shutdown();

        virtual void Render();
        virtual void OnKeyStroke(int ch);
        virtual void OnTick();

    private:
        void PrintTerrain(Character& c, CivModel::Terrain::Point point);

        std::pair<int, int> TerrainToScreen(int x, int y);

        CivPresenter::Presenter^ m_presenter;

        int m_sightx = 0;
        int m_sighty = 0;

        Screen* m_screen;
    };
}
