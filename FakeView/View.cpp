#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        m_presenter = gcnew CivPresenter::Presenter(this);
    }

    void View::Shutdown()
    {
        m_screen->Quit(0);
    }

    void View::Render()
    {
        switch (m_presenter->State)
        {
            case CivPresenter::Presenter::States::ProductUI:
                RenderProductUI();
                break;
            case CivPresenter::Presenter::States::ProductAdd:
                RenderProductAdd();
                break;
            default:
                RenderNormal();
                break;
        }
    }

    void View::RenderNormal()
    {
        auto scrsz = m_screen->GetSize();

        int sx = scrsz.width / 3;
        int sy = scrsz.height / 3;

        int bx = m_presenter->FocusedPoint.Position.X - (sx / 2);
        int by = m_presenter->FocusedPoint.Position.Y - (sy / 2);

        for (int dy = 0; dy < sy; ++dy)
        {
            for (int dx = 0; dx < sx; ++dx)
            {
                int x = bx + dx;
                int y = by + dy;

                if (x < 0 || x >= m_presenter->Game->Terrain->Width)
                    continue;
                if (y < 0 || y >= m_presenter->Game->Terrain->Height)
                    continue;

                auto point = m_presenter->Game->Terrain->GetPoint(x, y);

                int px = dx * 3 + 1 - (y % 2);
                int py = dy * 3 + 1;

                PrintTerrain(px, py, point);
                if (point.TileBuilding)
                {
                    PrintTileBuilding(px, py, point.TileBuilding);
                }
                if (point.Unit)
                {
                    PrintUnit(px, py, point.Unit);
                }
            }
        }

        if (m_presenter->SelectedActor != nullptr)
        {
            auto pos = m_presenter->SelectedActor->PlacedPoint.Value.Position;
            auto pt = TerrainToScreen(pos.X, pos.Y);
            if (auto pc = m_screen->TryGetChar(pt.first, pt.second))
            {
                pc->color |= 0b1000'1000;
            }
        }

        if (m_presenter->MoveAdjcents)
        {
            for (int i = 0; i < m_presenter->MoveAdjcents->Count; ++i)
            {
                if (m_presenter->MoveAdjcents[i].HasValue)
                {
                    auto pos = m_presenter->MoveAdjcents[i].Value.Position;
                    auto pt = TerrainToScreen(pos.X, pos.Y);
                    if (auto pc = m_screen->TryGetChar(pt.first, pt.second))
                    {
                        pc->color = 0b0001'0110;

                        if (i == m_presenter->MoveSelectedIndex)
                            pc->color |= 0b1000'1000;
                    }
                }
            }
        }

        auto ptcenter = TerrainToScreen(bx + (sx / 2), by + (sy / 2));
        switch (m_presenter->State)
        {
            case CivPresenter::Presenter::States::Normal:
                m_screen->GetChar(ptcenter.first, ptcenter.second).color |= 0b1000'1000;
                m_screen->PrintString(0, scrsz.height - 1, 0b00000111,
                    "Turn: " + std::to_string(m_presenter->Game->TurnNumber));
                break;
            case CivPresenter::Presenter::States::Move:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Move");
                break;
            case CivPresenter::Presenter::States::SpecialAct:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111,
                    "SpecialAct: " + std::to_string(m_presenter->StateParam));
                break;
            case CivPresenter::Presenter::States::Deploy:
                m_screen->GetChar(ptcenter.first, ptcenter.second).color |= 0b1000'1000;
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Deploy");
                break;
        }
    }

    void View::RenderProductUI()
    {
        auto scrsz = m_screen->GetSize();

        int y = 0;
        m_screen->PrintString(0, y, 0b0000'1111, "Production UI");

        auto player = m_presenter->Game->PlayerInTurn;
        if ((y = 2) >= scrsz.height)
            return;
        m_screen->PrintString(0, y, 0b0000'0111,
            "Total Labor: " + std::to_string(player->Labor)
            + " (Used: " + std::to_string(player->EstimatedUsedLabor) + ")");

        unsigned color = 0b0000'0111;
        if (m_presenter->SelectedDeploy == -1 && m_presenter->SelectedProduction == -1)
            color = 0b1111'0000;
        if ((y = 4) >= scrsz.height)
            return;
        m_screen->PrintString(0, y, color, "Add Production");

        int idx = 0;
        y = 5;
        for (auto node = player->Deployment->First; node != nullptr; node = node->Next)
        {
            if (y >= scrsz.height)
                return;

            std::string msg;
            if (auto product = dynamic_cast<CivModel::Common::PioneerProductionFactory^>(node->Value->Factory))
            {
                msg = "Pioneer";
            }
            else
            {
                System::Diagnostics::Debug::WriteLine(L"unqualified production in RenderProductUI()");
                msg = "????";
            }

            msg += " (completed)";

            unsigned char color = 0b0000'0111;
            if (m_presenter->SelectedDeploy == idx)
                color = 0b1111'0000;

            m_screen->PrintString(0, y, color, msg);

            ++idx;
            ++y;
        }

        idx = 0;
        for (auto node = player->Production->First; node != nullptr; node = node->Next)
        {
            if (y >= scrsz.height)
                return;

            std::string msg;
            if (auto product = dynamic_cast<CivModel::Common::PioneerProductionFactory^>(node->Value->Factory))
            {
                msg = "Pioneer";
            }
            else
            {
                System::Diagnostics::Debug::WriteLine(L"unqualified production in RenderProductUI()");
                msg = "????";
            }

            msg += " " + std::to_string(node->Value->LaborInputed) + " / " + std::to_string(node->Value->TotalCost);
            msg += " (+" + std::to_string(node->Value->EstimatedLaborInputing);
            msg += " / " + std::to_string(node->Value->CapacityPerTurn) + ")";

            unsigned char color = 0b0000'0111;
            if (m_presenter->SelectedProduction == idx)
                color = 0b1111'0000;

            m_screen->PrintString(0, y, color, msg);

            ++idx;
            ++y;
        }

        if (m_presenter->IsProductManipulating)
        {
            m_screen->PrintString(0, scrsz.height - 1, 0b0000'1111, "press Enter again to cancel production");
        }
    }

    void View::RenderProductAdd()
    {
        auto scrsz = m_screen->GetSize();

        int y = 0;
        m_screen->PrintString(0, y, 0b00001111, "Add Production");

        unsigned color = 0b0000'0111;
        if (m_presenter->SelectedProduction == -1)
            color = 0b1111'0000;
        y = 2;
        m_screen->PrintString(0, y, color, "Cancel");

        y = 3;
        for (int idx = 0; idx < m_presenter->AvailableProduction->Count; ++idx)
        {
            if (y >= scrsz.height)
                return;

            auto value = m_presenter->AvailableProduction[idx];

            std::string msg;
            if (auto product = dynamic_cast<CivModel::Common::PioneerProductionFactory^>(value))
            {
                msg = "Pioneer";
            }
            else
            {
                System::Diagnostics::Debug::WriteLine(L"unqualified production in RenderProductUI()");
                msg = "????";
            }

            unsigned char color = 0b0000'1111;
            if (m_presenter->SelectedProduction == idx)
                color = ~color;

            m_screen->PrintString(0, y, color, msg);

            ++y;
        }
    }

    void View::OnKeyStroke(int ch)
    {
        switch (ch)
        {
            case 0x1b: // ESC
                m_presenter->CommandCancel();
                break;

            case 0x148: // UP
                m_presenter->CommandArrowKey(CivPresenter::Direction::Up);
                break;
            case 0x150: // DOWN
                m_presenter->CommandArrowKey(CivPresenter::Direction::Down);
                break;
            case 0x14b: // LEFT
                m_presenter->CommandArrowKey(CivPresenter::Direction::Left);
                break;
            case 0x14d: // RIGHT
                m_presenter->CommandArrowKey(CivPresenter::Direction::Right);
                break;

            case 'f':
            case 'F':
                m_presenter->CommandRefocus();
                break;

            case 's':
            case 'S':
                m_presenter->CommandSelect();
                break;

            case 'm':
            case 'M':
                m_presenter->CommandMove();
                break;

            case 'p':
            case 'P':
                m_presenter->CommandProductUI();
                break;

            case '\r':
                m_presenter->CommandApply();
                break;

            case '1':
                m_presenter->CommandNumeric(0);
                break;
            case '2':
                m_presenter->CommandNumeric(1);
                break;
            case '3':
                m_presenter->CommandNumeric(2);
                break;
            case '4':
                m_presenter->CommandNumeric(3);
                break;
            case '5':
                m_presenter->CommandNumeric(4);
                break;
            case '6':
                m_presenter->CommandNumeric(5);
                break;
            case '7':
                m_presenter->CommandNumeric(6);
                break;
            case '8':
                m_presenter->CommandNumeric(7);
                break;
            case '9':
                m_presenter->CommandNumeric(8);
                break;
        }
    }

    void View::OnTick()
    {
    }

    void View::PrintTerrain(int px, int py, CivModel::Terrain::Point point)
    {
        auto& c = m_screen->GetChar(px, py);

        c.color = 0b0000'0111;

        if (point.Type2 == CivModel::TerrainType2::Mountain)
        {
            c.ch = '^';
        }
        else
        {
            switch (point.Type1)
            {
                case CivModel::TerrainType1::Flatland:
                    c.ch = '-';
                    break;
                case CivModel::TerrainType1::Grass:
                    c.ch = '*';
                    break;
                case CivModel::TerrainType1::Swamp:
                    c.ch = '@';
                    break;
                case CivModel::TerrainType1::Tundra:
                    c.ch = '#';
                    break;
            }
            if (point.Type2 == CivModel::TerrainType2::Hill)
            {
                c.color |= 0b0000'1000;
            }
        }
    }

    void View::PrintUnit(int px, int py, CivModel::Unit^ unit)
    {
        auto& c = m_screen->GetChar(px, py);
        if (auto u = dynamic_cast<CivModel::Common::Pioneer^>(unit))
        {
            c.ch = 'P';
            c.color &= 0xf0;
            c.color |= 0b0000'1001;
        }
        else
        {
            System::Diagnostics::Debug::WriteLine(L"unqualified unit in PrintUnit()");
            c.ch = 'U';
            c.color &= 0xf0;
            c.color |= 0b0000'1110;
        }
    }

    void View::PrintTileBuilding(int px, int py, CivModel::TileBuilding^ tileBuilding)
    {
        auto& c = m_screen->GetChar(px, py);
        if (auto b = dynamic_cast<CivModel::Common::CityCenter^>(tileBuilding))
        {
            c.color &= 0x0f;
            c.color |= 0b0010'0000;
        }
        else
        {
            System::Diagnostics::Debug::WriteLine(L"unqualified tileBuilding in PrintTileBuilding()");
            c.color &= 0x0f;
            c.color |= 0b0111'0000;
        }
    }

    std::pair<int, int> View::TerrainToScreen(int x, int y)
    {
        auto scrsz = m_screen->GetSize();

        int sx = scrsz.width / 3;
        int sy = scrsz.height / 3;

        int bx = m_presenter->FocusedPoint.Position.X - (sx / 2);
        int by = m_presenter->FocusedPoint.Position.Y - (sy / 2);

        int dx = x - bx;
        int dy = y - by;

        return {
            dx * 3 + 1 - (y % 2),
            dy * 3 + 1
        };
    }
}
