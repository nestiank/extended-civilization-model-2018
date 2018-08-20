#pragma once

#include "Screen.h"

namespace FakeView
{
    ref class View : public CivPresenter::IView, public IScreenClient
    {
    public:
        explicit View(Screen* screen);

        virtual void Refocus();
        virtual void Shutdown();
        virtual void Invoke(System::Action^ action);

        virtual void Render();
        virtual void OnKeyStroke(int ch);
        virtual void OnTick();

    private:
        void RenderNormal();
        void RenderProductUI();
        void RenderProductAdd();
        void RenderQuest();
        void RenderEnding();
        void RenderCityView();

        void PrintTerrain(int px, int py, CivModel::Terrain::Point point);
        void PrintUnit(int px, int py, CivModel::Unit^ unit);
        void PrintTileBuilding(int px, int py, CivModel::TileBuilding^ tileBuilding);

        void PrintActorInfo(int line, unsigned char color, const std::string& prefix, CivModel::Actor^ actor);
        void PrintMovePath(CivModel::IMovePath^ path);

        unsigned char GetPlayerColor(CivModel::Player^ player);

        std::string GetFactoryDescription(CivModel::IProductionFactory^ factory);

        std::pair<int, int> TerrainToScreen(int x, int y);

        CivPresenter::Presenter^ m_presenter;
        bool m_roundEarth = false;

        Screen* m_screen;

        int m_autoSkip = 0;
        CivModel::Player^ m_autoSkipPlayer;

        System::Nullable<CivModel::Terrain::Point> m_fixedCenter;

        bool m_bTeamColor = false;
    };
}
