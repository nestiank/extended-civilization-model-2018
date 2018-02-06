#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        m_presenter = nullptr;
        if (System::IO::File::Exists(L"map.txt"))
        {
            if (MessageBox(nullptr, L"Save file is found. Do you want to load it?", L"Save file is found", MB_YESNO)
                == IDYES)
            {
                m_presenter = gcnew CivPresenter::Presenter(this, L"map.txt");
            }
        }
        if (!m_presenter)
            m_presenter = gcnew CivPresenter::Presenter(this, 10, 8, 2);
    }

    void View::Refocus()
    {
        // do nothing
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
                if (point.TileBuilding)
                {
                    PrintTileBuilding(px, py, point.TileBuilding);
                }
                if (point.Unit)
                {
                    PrintUnit(px, py, point.Unit);
                }

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

        auto player = m_presenter->Game->PlayerInTurn;
        int y = 0;
        m_screen->PrintString(0, y, 0b0000'1111, "Production UI");

        {
            unsigned char color = 0b0000'0111;

            y += 2;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Total Gold: " + std::to_string(player->Gold)
                + " (+ " + std::to_string(player->GoldNetIncome) + ")");

            ++y;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Total Happiness: " + std::to_string(player->Happiness)
                + " (+ " + std::to_string(player->HappinessIncome) + ")");

            ++y;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Total Labor: " + std::to_string(player->Labor)
                + " (Used: " + std::to_string(player->EstimatedUsedLabor) + ")");

            ++y;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Total Research: " + std::to_string(player->Research));

            ++y;
            if (y >= scrsz.height)
                return;
            m_screen->PrintString(0, y, color,
                "Total Population: " + std::to_string(player->Population));

            y += 2;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 0)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Economic Investment: " + std::to_string(player->EconomicInvestment)
                + " (basic requirement: " + std::to_string(player->BasicEconomicRequire) + ")");

            ++y;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 1)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Research Investment: " + std::to_string(player->ResearchInvestment)
                + " (basic requirement: " + std::to_string(player->BasicResearchRequire) + ")");

            y += 2;
            if (m_presenter->SelectedInvestment == -1 && m_presenter->SelectedDeploy == -1 && m_presenter->SelectedProduction == -1)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            if (y >= scrsz.height)
                return;
            m_screen->PrintString(0, y, color, "Add Production");
        }

        ++y;
        int idx = 0;
        for (auto node = player->Deployment->First; node != nullptr; node = node->Next)
        {
            if (y >= scrsz.height)
                return;

            unsigned char color = 0b0000'0111;
            if (m_presenter->SelectedDeploy == idx)
                color = 0b1111'0000;

            std::string msg = GetFactoryDescription(node->Value->Factory);
            msg += " (completed)";

            m_screen->PrintString(0, y, color, msg);

            ++idx;
            ++y;
        }

        idx = 0;
        for (auto node = player->Production->First; node != nullptr; node = node->Next)
        {
            if (y + 2 >= scrsz.height)
                return;

            unsigned char color = 0b0000'0111;
            if (m_presenter->SelectedProduction == idx)
                color = 0b1111'0000;

            std::string msg1, msg2, msg3;
            msg1 = GetFactoryDescription(node->Value->Factory);
            msg2 = "    Labor: " + std::to_string(node->Value->LaborInputed) + " / " + std::to_string(node->Value->TotalLaborCost);
            msg2 += " (+" + std::to_string(node->Value->EstimatedLaborInputing) + " / " + std::to_string(node->Value->LaborCapacityPerTurn) + ")";
            msg3 = "     Gold: " + std::to_string(node->Value->GoldInputed) + " / " + std::to_string(node->Value->TotalGoldCost);
            msg3 += " (+" + std::to_string(node->Value->EstimatedGoldInputing) + " / " + std::to_string(node->Value->GoldCapacityPerTurn) + ")";

            m_screen->PrintString(0, y++, color, msg1);
            m_screen->PrintString(0, y++, color, msg2);
            m_screen->PrintString(0, y++, color, msg3);

            ++idx;
        }

        if (m_presenter->SelectedInvestment != -1)
        {
            m_screen->PrintStringEx(0, scrsz.height - 1, 0x0f,
                "[1-4]%c\x07: set investments (below basic requirement) %c\x0f"
                "[5]%c\x07: set investments to basic requirement %c\x0f"
                "[6-0]%c\x07: set investments (above basic requirement) %c\x0f");
        }
        else if (m_presenter->IsProductManipulating)
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

            unsigned char color = 0b0000'1111;
            if (m_presenter->SelectedProduction == idx)
                color = ~color;

            auto value = m_presenter->AvailableProduction[idx];

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
            case ',':
            case '<':
                if (!m_presenter->SaveFile)
                    m_presenter->SaveFile = L"map.txt";
                m_presenter->CommandSave();
                MessageBox(nullptr, L"Saved", L"", MB_OK);
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

            case 'f':
            case 'F':
                m_presenter->CommandRefocus();
                break;

            case 's':
            case 'S':
                m_presenter->CommandSelect();
                break;

            case 'd':
            case 'D':
                m_presenter->CommandRemove();
                break;

            case 'm':
            case 'M':
                m_presenter->CommandMove();
                break;

            case 'z':
            case 'Z':
                m_presenter->CommandSkip();
                break;

            case 'q':
            case 'Q':
                m_presenter->CommandMovingAttack();
                break;

            case 'w':
            case 'W':
                m_presenter->CommandHoldingAttack();
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
        if (point.TileOwner)
            c.color = GetPlayerColor(point.TileOwner);
        else
            c.color = 0b0000'0111;

        switch (point.Type)
        {
            case CivModel::TerrainType::Plain:
                c.ch = '-';
                break;
            case CivModel::TerrainType::Ocean:
                c.ch = '*';
                break;
            case CivModel::TerrainType::Mount:
                c.ch = '^';
                break;
            case CivModel::TerrainType::Forest:
                c.ch = '#';
                break;
            case CivModel::TerrainType::Swamp:
                c.ch = '@';
                break;
            case CivModel::TerrainType::Tundra:
                c.ch = '%';
                break;
            case CivModel::TerrainType::Ice:
                c.ch = '$';
                break;
            case CivModel::TerrainType::Hill:
                c.ch = '&';
                break;
        }
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
        return static_cast<unsigned char>(playerIndex % 6) + 1;
    }

    std::string View::GetFactoryDescription(CivModel::IProductionFactory^ factory)
    {
        if (auto product = dynamic_cast<CivModel::Common::PioneerProductionFactory^>(factory))
        {
            return "Pioneer";
        }
        else if (auto product = dynamic_cast<CivModel::Common::JediKnightProductionFactory^>(factory))
        {
            return "Jedi Knight";
        }
        else if (auto product = dynamic_cast<CivModel::Common::FactoryBuildingProductionFactory^>(factory))
        {
            return "Factory";
        }
        else if (auto product = dynamic_cast<CivModel::Common::LaboratoryBuildingProductionFactory^>(factory))
        {
            return "Laboratory";
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
