#include "pch.h"
#include "View.h"

#include "Screen.h"
#include "AIControlDialog.h"

#ifndef DEBUG_CORE
#define MAP_FILE_PATH L"map.txt"
#else
#define MAP_FILE_PATH L"map-core.txt"
#endif

namespace
{
    std::string cli2str(System::String^ str)
    {
        using namespace System::Runtime::InteropServices;
        auto chars = static_cast<char*>(Marshal::StringToHGlobalAnsi(str).ToPointer());
        std::string ret = chars;
        Marshal::FreeHGlobal(System::IntPtr(chars));
        return ret;
    }
}

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        System::String^ file = MAP_FILE_PATH;
        if (!System::IO::File::Exists(file))
        {
            file = System::IO::Path::Combine(L"..", L"docs", MAP_FILE_PATH);
            if (!System::IO::File::Exists(file))
                file = nullptr;
        }

        m_presenter = nullptr;
        if (file)
        {
            if (MessageBoxW(nullptr, L"Save file is found. Do you want to load it?", L"Save file is found", MB_YESNO)
                == IDYES)
            {
                m_presenter = gcnew CivPresenter::Presenter(this, file);
            }
        }
        if (!m_presenter)
            m_presenter = gcnew CivPresenter::Presenter(this, 15, 12, -1);
    }

    void View::Refocus()
    {
        // do nothing
    }

    void View::Shutdown()
    {
        m_screen->Quit(0);
    }

    void View::Invoke(System::Action^ action)
    {
        m_screen->Invoke(action);
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
            case CivPresenter::Presenter::States::Quest:
                RenderQuest();
                break;
            case CivPresenter::Presenter::States::Ending:
                RenderEnding();
                break;
            case CivPresenter::Presenter::States::CityView:
                RenderCityView();
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

                if (!m_roundEarth && (x < 0 || x >= m_presenter->Game->Terrain->Width))
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
                    if (CivModel::ActorActionExtension::IsActable(m_presenter->RunningAction, point))
                    {
                        auto& c = m_screen->GetChar(px, py);
                        c.color |= 0b0001'0110;
                    }
                }

                if (m_presenter->SelectedActor && m_presenter->SelectedActor == point.Unit)
                {
                    auto& c = m_screen->GetChar(px, py);
                    c.color ^= 0b1111'1111;
                }
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
            if (!CivModel::ActorActionExtension::IsActable(m_presenter->RunningAction, pt))
            {
                chCenter.color ^= 0b0111'0111;
                chCenter.color |= 0b1100'1110;
            }
        }

        if (m_presenter->FocusedPoint.Unit != nullptr && m_presenter->FocusedPoint.Unit != m_presenter->SelectedActor)
        {
            PrintActorInfo(scrsz.height - 3, 0b00000111, "<on cursor>: ", m_presenter->FocusedPoint.Unit);
        }
        else if (m_presenter->FocusedPoint.TileBuilding != nullptr && m_presenter->FocusedPoint.TileBuilding != m_presenter->SelectedActor)
        {
            PrintActorInfo(scrsz.height - 3, 0b00000111, "<on cursor>: ", m_presenter->FocusedPoint.TileBuilding);
        }

        if (m_presenter->SelectedActor != nullptr)
        {
            auto actor = m_presenter->SelectedActor;
            PrintActorInfo(scrsz.height - 2, 0b00000111, "< selected>: ", actor);

            if (actor->MovePath != nullptr)
            {
                PrintMovePath(actor->MovePath);
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

                if (m_autoSkipPlayer != nullptr)
                {
                    if (m_presenter->Game->PlayerInTurn == m_autoSkipPlayer)
                    {
                        if (m_autoSkip != -1)
                        {
                            if (m_autoSkip-- == 0)
                                m_autoSkipPlayer = nullptr;
                        }
                    }

                    if (m_autoSkipPlayer != nullptr)
                        m_presenter->CommandApply();
                }

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

            case CivPresenter::Presenter::States::AIControl:
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "AI is running ...");
                break;

            case CivPresenter::Presenter::States::PathFinding:
                if (m_presenter->MovePath)
                {
                    PrintMovePath(m_presenter->MovePath);
                }
                else
                {
                    auto pt = m_presenter->FocusedPoint;
                    auto spt = TerrainToScreen(pt.Position.X, pt.Position.Y);
                    auto& ch = m_screen->GetChar(spt.first, spt.second);
                    ch.color = 0b1100'1110;
                }
                m_screen->PrintString(0, scrsz.height - 1, 0b00001111, "Move along path");
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
            color = 0b0000'1111;
            m_screen->PrintString(0, y, color, "Player Team  " + std::to_string(player->Team));

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
                "Total Research: " + std::to_string(player->Research)
                + " (+ " + std::to_string(player->ResearchIncome) + ")");

            ++y;
            if (y >= scrsz.height)
                return;

            double popinc = 0;
            for each (auto city in player->Cities)
            {
                popinc += city->PopulationIncome;
            }
            m_screen->PrintString(0, y, color, "Total Population: " + std::to_string(player->Population)
                + " (+" + std::to_string(popinc) + ")");

            y += 2;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 0)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color, "Tax Rate: " + std::to_string(player->TaxRate));

            y += 2;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 1)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Economic Investment: " + std::to_string(player->EconomicInvestmentRatio * 100) + "%"
                + " [ " + std::to_string(player->EconomicInvestment) + " / " + std::to_string(player->BasicEconomicRequire) + " ]");

            ++y;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 2)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Research Investment: " + std::to_string(player->ResearchInvestmentRatio * 100) + "%"
                + " [ " + std::to_string(player->ResearchInvestment) + " / " + std::to_string(player->BasicResearchRequire) + " ]");

            ++y;
            if (y >= scrsz.height)
                return;
            if (m_presenter->SelectedInvestment == 3)
                color = 0b1111'0000;
            else
                color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Repair Investment: " + std::to_string(player->RepairInvestmentRatio * 100) + "%"
                + " [ " + std::to_string(player->RepairInvestment) + " / " + std::to_string(player->BasicLaborForRepair) + " ]");

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
                "[1-9]%c\x07: set the amount %c\x0f"
                "Up/Down%c\x07: move selection %c\x0f"
                "ESC/Enter%c\x07: cancel selection");
        }
        else if (m_presenter->IsProductManipulating)
        {
            m_screen->PrintStringEx(0, scrsz.height - 1, 0x0f,
                "d%c\x07: cancel production %c\x0f"
                "Up/Down%c\x07: change production order %c\x0f"
                "ESC/Enter%c\x07: cancel selection");
        }
        else
        {
            m_screen->PrintStringEx(0, scrsz.height - 1, 0x0f,
                "Enter%c\x07: work with selection %c\x0f"
                "Up/Down%c\x07: move selection %c\x0f"
                "ESC%c\x07: cancel selection");
        }
    }

    void View::RenderProductAdd()
    {
        auto scrsz = m_screen->GetSize();

        int y = scrsz.height - ((scrsz.height / 6) + m_presenter->SelectedProduction);
        if (y > 0)
            y = 0;

        m_screen->PrintString(0, y, 0b00001111, "Add Production");

        unsigned color = 0b0000'0111;
        if (m_presenter->SelectedProduction == -1)
            color = 0b1111'0000;
        y += 2;
        m_screen->PrintString(0, y, color, "Cancel");

        y += 1;
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

    void View::RenderQuest()
    {
        auto scrsz = m_screen->GetSize();

        int y = 0;
        unsigned char color = 0b0000'1111;
        m_screen->PrintString(0, y, color, "Quest List");

        y += 2;
        int count = 0;
        auto arr = gcnew array<System::Collections::Generic::IReadOnlyList<CivModel::Quest^>^>(4);
        arr[0] = m_presenter->AcceptedQuests;
        arr[1] = m_presenter->DeployedQuests;
        arr[2] = m_presenter->CompletedQuests;
        arr[3] = m_presenter->DisabledQuests;
        for each (auto list in arr)
        {
            for (int idx = 0; idx < list->Count; ++idx)
            {
                if (y >= scrsz.height)
                    return;

                CivModel::Quest^ quest = list[idx];
                std::string msg;
                //msg += cli2str(quest->Name); // 한글앙대
                msg += cli2str(quest->GetType()->ToString());
                if (quest->Status == CivModel::QuestStatus::Accepted)
                {
                    msg += " (accepted: " + std::to_string(quest->LeftTurn) + " / " + std::to_string(quest->LimitTurn) + ")";
                }
                else if (quest->Status == CivModel::QuestStatus::Deployed)
                {
                    msg += " (deployed: " + std::to_string(quest->LeftTurn) + " / " + std::to_string(quest->PostingTurn) + ")";
                }
                else if (quest->Status == CivModel::QuestStatus::Completed)
                {
                    msg += " (completed)";
                }
                else if (quest->Status == CivModel::QuestStatus::Disabled)
                {
                    msg += " (disabled)";
                }

                color = 0b0000'1111;
                if (m_presenter->SelectedQuest == count)
                    color = ~color;

                m_screen->PrintString(0, y, color, msg);

                ++y;
                ++count;
            }
        }

        m_screen->PrintStringEx(0, scrsz.height - 1, 0x0f,
            "Enter%c\x07: quest accept/unaccept %c\x0f"
            "Up/Down%c\x07: move selection");
    }

    void View::RenderEnding()
    {
        auto player = m_presenter->Game->PlayerInTurn;
        const char* msg;

        if (player->IsVictoried)
        {
            msg = "HOW DO YOU VICTORIED? (unqualified)";
#ifndef DEBUG_CORE
            if (auto victory = dynamic_cast<CivModel::Quests::FinnoUltimateVictory^>(player->VictoryCondition))
                msg = "YOU ARE C'THULHU SUMMOER";
            else if (auto victory = dynamic_cast<CivModel::Quests::HwanUltimateVictory^>(player->VictoryCondition))
                msg = "YOU ARE CHOSEN PEOPLE";
            else if (auto victory = dynamic_cast<CivModel::Quests::FinnoConquerVictory^>(player->VictoryCondition))
                msg = "YOU ARE NOT SLAVE BUT CONQUERER";
            else if (auto victory = dynamic_cast<CivModel::Quests::HwanConquerVictory^>(player->VictoryCondition))
                msg = "YOU ARE NOT SLAVE BUT CONQUERER";
            else if (auto victory = dynamic_cast<CivModel::Quests::ZapConquerVictory^>(player->VictoryCondition))
                msg = "YOU ARE NOT SLAVE BUT CONQUERER";
#endif
        }
        else if (player->IsDefeated)
        {
            msg = "HOW DO YOU DEFEATED? (unqualified)";
#ifndef DEBUG_CORE
            if (auto defeat = dynamic_cast<CivModel::Quests::GameEndDefeat^>(player->DefeatCondition))
                msg = "YOU ARE DEFEATED BY ULTIMATE FORCE";
            else if (auto defeat = dynamic_cast<CivModel::Quests::EliminationDefeat^>(player->DefeatCondition))
                msg = "YOU ARE ELIMINATED";
#endif
        }
        else if (player->IsDrawed)
        {
            msg = "HOW DO YOU DRAWED? (unqualified)";
#ifndef DEBUG_CORE
            if (auto draw = dynamic_cast<CivModel::Quests::HyperUltimateDraw^>(player->DrawCondition))
                msg = "YOU HAVE SEEN THE END OF HYPERWAR";
#endif
        }
        else
        {
            msg = "HOW HAVE YOU SEEN ENDING? (unqualified)";
        }

        for (int y = 10; y <= 20; ++y)
            m_screen->PrintString(10, y, 0b1101'1010, msg);
    }

    void View::RenderCityView()
    {
        auto scrsz = m_screen->GetSize();

        auto city = m_presenter->SelectedCity;
        int y = 0;
        m_screen->PrintString(0, y, 0b0000'1111, "City View: \"" + cli2str(city->CityName) + "\"");

        {
            unsigned char color = 0b0000'0111;

            int playerNum = 0;
            auto players = m_presenter->Game->Players;
            for (; playerNum < players->Count; ++playerNum)
                if (city->Owner == players[playerNum])
                    break;

            y += 2;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color, "Owner: Player " + std::to_string(playerNum));

            y += 1;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "Population: " + std::to_string(city->Population)
                + "(+ " + std::to_string(city->PopulationIncome) + ")");

            y += 1;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color, "Labor: " + std::to_string(city->ProvidedLabor));

            y += 1;
            if (y >= scrsz.height)
                return;
            color = 0b0000'0111;
            m_screen->PrintString(0, y, color,
                "HP: " + std::to_string(city->RemainHP)
                + " / " + std::to_string(city->MaxHP));

            y += 2;
            for each (auto building in city->InteriorBuildings)
            {
                if (y >= scrsz.height)
                    return;
                color = 0b0000'0111;
                m_screen->PrintString(0, y, color, cli2str(building->GetType()->FullName));

                ++y;
            }
        }
    }

    void View::OnKeyStroke(int ch)
    {
        switch (ch)
        {
            case ',':
            case '<':
                if (!m_presenter->SaveFile)
                    m_presenter->SaveFile = MAP_FILE_PATH;
                m_presenter->CommandSave();
                MessageBoxW(nullptr, L"Saved", L"", MB_OK);
                break;

            case '+':
            case '=':
                if (m_presenter->State == CivPresenter::Presenter::States::Normal)
                {
                    m_roundEarth = !m_roundEarth;
                }
                break;

            case '-':
            case '_':
                if (m_presenter->State == CivPresenter::Presenter::States::Normal)
                {
                    auto msg = L"Press \"Yes\" to make this player controlled by AI\n"
                        L"Press \"No\" to make all but this player controlled by AI\n"
                        L"Press \"Cancel\" to cancel AI Setting";
                    auto rs = MessageBoxW(nullptr, msg, L"AI Setting", MB_YESNOCANCEL);
                    if (rs == IDYES)
                    {
                        auto player = m_presenter->Game->PlayerInTurn;
                        player->IsAIControlled = !player->IsAIControlled;
                    }
                    else if (rs == IDNO)
                    {
                        for (int i = 0; i < m_presenter->Game->Players->Count; ++i)
                        {
                            if (i != m_presenter->Game->PlayerNumberInTurn)
                            {
                                m_presenter->Game->Players[i]->IsAIControlled = true;
                            }
                        }
                    }
                }
                break;

            case '[':
            case '{':
                if (m_presenter->State == CivPresenter::Presenter::States::Normal)
                {
                    auto dialog = gcnew AIControlDialog(m_presenter->Game);
                    dialog->ShowDialog();
                }
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

            case 'n':
            case 'N':
                m_presenter->CommandPathFinding();
                break;

            case 'c':
            case 'C':
                m_presenter->CommandActorCancel();
                break;

            case 'z':
            case 'Z':
                m_presenter->CommandSkip();
                break;

            case 'x':
            case 'X':
                m_presenter->CommandSleep();
                break;

            case 'q':
            case 'Q':
                m_presenter->CommandMovingAttack();
                break;

            case 'w':
            case 'W':
                m_presenter->CommandHoldingAttack();
                break;

            case 'e':
            case 'E':
                m_presenter->CommandPillage();
                break;

            case 'p':
            case 'P':
                m_presenter->CommandProductUI();
                break;

            case 'o':
            case 'O':
                m_presenter->CommandQuest();
                break;

            case 'i':
            case 'I':
                m_presenter->CommandCityView();
                break;

            case '\r':
                m_presenter->CommandApply();
                break;

            case '\'':
            {
                auto str = Microsoft::VisualBasic::Interaction::InputBox(L"input turn count to autoskip", L"", L"", -1, -1);
                int turn;
                if (System::Int32::TryParse(str, turn) && (turn > 0 || turn == -1))
                {
                    m_autoSkip = turn;
                    m_autoSkipPlayer = m_presenter->Game->PlayerInTurn;
                }
                break;
            }

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
        else if (auto u = dynamic_cast<CivModel::Common::FakeKnight^>(unit))
        {
            c.ch = 'F';
        }
        else
        {
            c.ch = cli2str(unit->GetType()->FullName)[15];
        }
    }

    void View::PrintTileBuilding(int px, int py, CivModel::TileBuilding^ tileBuilding)
    {
        auto& c = m_screen->GetChar(px, py);
        c.color &= 0x0f;
        c.color |= GetPlayerColor(tileBuilding->Owner) << 4;
    }

    void View::PrintActorInfo(int line, unsigned char color, const std::string& prefix, CivModel::Actor^ actor)
    {
        std::string msg = prefix;

        if (auto unit = dynamic_cast<CivModel::Unit^>(actor))
            msg += "Unit Name: ";
        else if (auto tb = dynamic_cast<CivModel::TileBuilding^>(actor))
            msg += "TileBuilding: ";
        else
            msg += "(unqualified actor): ";

        msg += cli2str(actor->GetType()->FullName);
        msg += ", HP: " + std::to_string(actor->RemainHP) + " / " + std::to_string(actor->MaxHP);
        msg += ", AP: " + std::to_string(actor->RemainAP) + " / " + std::to_string(actor->MaxAP);

        m_screen->PrintString(0, line, color, msg);
    }

    void View::PrintMovePath(CivModel::IMovePath^ path)
    {
        char num = '*';
        double ap = path->Actor->RemainAP;
        CivModel::Terrain::Point prev;

        for each (auto pt in path->Path)
        {
            auto spt = TerrainToScreen(pt.Position.X, pt.Position.Y);
            if (auto print = m_screen->TryGetChar(spt.first, spt.second))
            {
                char ch;

                if (num == '*')
                {
                    ch = num;
                    num = '1';
                }
                else
                {
                    double required = path->Actor->GetRequiredAPToMoveNearBy(prev, pt).Value;
                    if (ap >= required)
                    {
                        ap -= required;
                    }
                    else
                    {
                        ap = path->Actor->MaxAP - required;
                        if (++num > '9')
                            num = '0';
                    }

                    ch = num;
                }

                print->color = 0b10011111;
                print->ch = ch;
                prev = pt;
            }
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
        return cli2str(factory->GetType()->FullName);
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
