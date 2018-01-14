#pragma once

#include "Screen.h"

namespace FakeView
{
    ref class View : public CivPresenter::IView, public IScreenClient
    {
    public:
        explicit View(Screen* screen);

        virtual void Shutdown();

        virtual void Render();
        virtual void OnKeyStroke(int ch);
        virtual void OnTick();

    private:
        void RenderNormal();
        void RenderProductUI();
        void RenderProductAdd();

        void PrintTerrain(int px, int py, CivModel::Terrain::Point point);
        void PrintUnit(int px, int py, CivModel::Unit^ unit);
        void PrintTileBuilding(int px, int py, CivModel::TileBuilding^ tileBuilding);

        std::pair<int, int> TerrainToScreen(int x, int y);

        CivPresenter::Presenter^ m_presenter;

        Screen* m_screen;
    };
}
