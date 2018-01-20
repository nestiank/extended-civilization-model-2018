#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        m_presenter = gcnew CivPresenter::Presenter(this);
        
        // test code
        m_presenter->Game->PlayerInTurn->AdditionalAvailableProduction->Add(
            CivModel::Common::JediKnightProductionFactory::Instance);
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
            case CivPresenter::Presenter::States::Victory:
                RenderVictory();
                break;
            case CivPresenter::Presenter::States::Defeated:
                RenderDefeated();
                break;
            default:
                RenderNormal();
                break;
        }
    }

    void View::RenderNormal()
    {
        auto scrsz = m_screen->GetSize();

        m_screen->PrintString(0, 0, 0b0000'1111,
            "Player " + std::to_string(m_presenter->Game->PlayerNumberInTurn));

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

                if (m_presenter->RunningAction != nullptr)
                {
                    if (CivModel::ActorAction::IsActable(m_presenter->RunningAction, point))
                    {
                        auto& c = m_screen->GetChar(px, py);
                        c.color |= 0b0001'0110;
                    }
                }
            }
        }

        if (m_presenter->SelectedActor)
        {
            auto point = m_presenter->SelectedActor->PlacedPoint.Value;
            auto pos = point.Position;
            auto pt = TerrainToScreen(pos.X, pos.Y);
            if (auto pc = m_screen->TryGetChar(pt.first, pt.second))
            {
                pc->color ^= 0b1111'1111;
            }
        }

        auto posCenter = CivModel::Position::FromPhysical(bx + (sx / 2), by + (sy / 2));
        auto ptCenter = TerrainToScreen(posCenter.X, posCenter.Y);
        auto& chCenter = m_screen->GetChar(ptCenter.first, ptCenter.second);

        if (m_presenter->SelectedActor == nullptr ||
            m_presenter->SelectedActor->PlacedPoint.Value.Position != posCenter)
        {
            chCenter.color ^= 0b0111'0111;
        }
        if (m_presenter->RunningAction != nullptr)
        {
            auto pt = m_presenter->Game->Terrain->GetPoint(posCenter);
            if (!CivModel::ActorAction::IsActable(m_presenter->RunningAction, pt))
            {
                chCenter.color ^= 0b0111'0111;
                chCenter.color |= 0b1100'1110;
            }
        }

        switch (m_presenter->State)
        {
            case CivPresenter::Presenter::States::Normal:
            {
                std::string msg = "Turn: " + std::to_string(m_presenter->Game->TurnNumber);
                if (m_presenter->IsThereTodos)
                {
                    msg += " %c\x0f""waiting for command %c\x07(";
                    msg += "%c\x0f""m%c\x07: move ";
                    msg += "%c\x0f""q%c\x07: moving attack ";
                    msg += "%c\x0f""w%c\x07: holding attack ";
                    msg += "%c\x0f""1-9%c\x07 : special acts ";
                    msg += "%c\x0f""p%c\x07: production ";
                    msg += "%c\x0f""z%c\x07: skip)";
                }
                else
                {
                    msg += " %c\x0fpress Enter for the next turn";
                }
                m_screen->PrintStringEx(0, scrsz.height - 1, 0b00000111, msg);
                break;
            }

            case CivPresenter::Presenter::States::Move:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Move");
                break;

            case CivPresenter::Presenter::States::MovingAttack:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Moving Attack");
                break;

            case CivPresenter::Presenter::States::HoldingAttack:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Holding Attack");
                break;

            case CivPresenter::Presenter::States::SpecialAct:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111,
                    "SpecialAct: " + std::to_string(m_presenter->StateParam));
                break;

            case CivPresenter::Presenter::States::Deploy:
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

            std::string msg = GetFactoryDescription(node->Value->Factory);
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

            std::string msg = GetFactoryDescription(node->Value->Factory);
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
            m_screen->PrintStringEx(0, scrsz.height - 1, 0x0f,
                "d%c\x07: cancel production %c\x0f"
                "Up/Down%c\x07: change production order %c\x0f"
                "ESC/Enter%c\x07: cancel selection");
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

            unsigned char color = 0b0000'1111;
            if (m_presenter->SelectedProduction == idx)
                color = ~color;

            m_screen->PrintString(0, y, color, GetFactoryDescription(value));

            ++y;
        }
    }

    void View::RenderVictory()
    {
        for (int y = 10; y <= 20; ++y)
            m_screen->PrintString(10, y, 0b1101'1010, "YOU ARE WINNER");
    }

    void View::RenderDefeated()
    {
        for (int y = 10; y <= 20; ++y)
            m_screen->PrintString(10, y, 0b1000'1111, "YOU ARE LOSER");
    }

    void View::OnKeyStroke(int ch)
    {
        switch (ch)
        {
            case '<':
            case ',':
                m_presenter->Game->Terrain->Save();
                ::MessageBox(nullptr, L"Saved", L"OK", MB_OK);
                break;

            case 'P':
            case 'p':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)0;
                break;
            case 'O':
            case 'o':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)1;
                break;
            case 'M':
            case 'm':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)2;
                break;
            case 'F':
            case 'f':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)3;
                break;
            case 'S':
            case 's':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)4;
                break;
            case 'T':
            case 't':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)5;
                break;
            case 'I':
            case 'i':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)6;
                break;
            case 'H':
            case 'h':
                m_presenter->FocusedPoint.Type = (CivModel::TerrainType)7;
                break;

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
        }
    }

    void View::OnTick()
    {
    }

    void View::PrintTerrain(int px, int py, CivModel::Terrain::Point point)
    {
        auto& c = m_screen->GetChar(px, py);
        c.color = 0b0000'0111;
        c.ch = "POMFSTIH"[(int)point.Type];
    }

    void View::PrintUnit(int px, int py, CivModel::Unit^ unit)
    {
        auto& c = m_screen->GetChar(px, py);
        c.color &= 0xf0;
        c.color |= 0x08 | GetPlayerColor(unit->Owner);

        if (auto u = dynamic_cast<CivModel::Common::Pioneer^>(unit))
        {
            c.ch = 'P';
        }
        else if (auto u = dynamic_cast<CivModel::Common::JediKnight^>(unit))
        {
            c.ch = 'J';
        }
        else
        {
            System::Diagnostics::Debug::WriteLine(L"unqualified unit in PrintUnit()");
            c.ch = 'U';
        }
    }

    void View::PrintTileBuilding(int px, int py, CivModel::TileBuilding^ tileBuilding)
    {
        auto& c = m_screen->GetChar(px, py);
        c.color &= 0x0f;
        c.color |= GetPlayerColor(tileBuilding->Owner) << 4;
        if (auto b = dynamic_cast<CivModel::Common::CityCenter^>(tileBuilding))
        {
            // do nothing
        }
        else
        {
            System::Diagnostics::Debug::WriteLine(L"unqualified tileBuilding in PrintTileBuilding()");
        }
    }

    unsigned char View::GetPlayerColor(CivModel::Player^ player)
    {
        auto players = m_presenter->Game->Players;
        int playerIndex = 0;
        for (; playerIndex < players->Count; ++playerIndex)
        {
            if (players[playerIndex] == player)
                break;
        }
        return static_cast<unsigned char>((playerIndex + 1) % 7);
    }

    std::string View::GetFactoryDescription(CivModel::IProductionFactory^ factory)
    {
        if (auto product = dynamic_cast<CivModel::Common::PioneerProductionFactory^>(factory))
        {
            return "Pioneer";
        }
        if (auto product = dynamic_cast<CivModel::Common::JediKnightProductionFactory^>(factory))
        {
            return "Jedi Knight";
        }
        else
        {
            System::Diagnostics::Debug::WriteLine(L"unqualified production in GetFactoryDescription()");
            return "????";
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
