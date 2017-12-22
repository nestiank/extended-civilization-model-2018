#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
#ifndef _DEBUG
        ::MessageBoxW(nullptr,
            L"move: m\n"
            L"focus: f\n"
            L"scroll screen / select: arrow key\n"
            L"quit / cancel: ESC",
            L"info", MB_OK);
#endif
        m_presenter = gcnew CivPresenter::Presenter(this);
    }

    void View::Refocus()
    {
        auto unit = m_presenter->FocusedUnit;
        if (unit && unit->PlacedPoint.HasValue)
        {
            auto pos = unit->PlacedPoint.Value.Position;
            m_sightx = pos.X;
            m_sighty = pos.Y;
        }
    }

    void View::MoveSight(int dx, int dy)
    {
        m_sightx += dx;
        m_sighty += dy;
    }

    void View::Shutdown()
    {
        m_screen->Quit(0);
    }

    void View::Render()
    {
        auto scrsz = m_screen->GetSize();

        int sx = scrsz.width / 3;
        int sy = scrsz.height / 3;

        int bx = m_sightx - (sx / 2);
        int by = m_sighty - (sy / 2);

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
                if (point.PlacedUnit)
                {
                    PrintUnit(px, py, point.PlacedUnit);
                }
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
                    auto pc = m_screen->TryGetChar(pt.first, pt.second);
                    if (pc)
                    {
                        pc->color = 0b00010110;

                        if (i == m_presenter->MoveSelectedIndex)
                            pc->color |= 0b10001000;
                    }
                }
            }
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
                Refocus();
                break;

            case 'm':
            case 'M':
                m_presenter->CommandMove();
                break;

            case '\r':
                m_presenter->CommandApply();
                break;
        }
    }

    void View::OnTick()
    {
    }

    void View::PrintTerrain(int px, int py, CivModel::Terrain::Point point)
    {
        auto& c = m_screen->GetChar(px, py);

        if (point.Type2 == CivModel::TerrainType2::Mountain)
        {
            c.ch = ' ';
            c.color = 0b0111'0000;
        }
        else
        {
            switch (point.Type1)
            {
                case CivModel::TerrainType1::Flatland:
                    c.ch = ' ';
                    c.color = 0b0011'0000;
                    break;
                case CivModel::TerrainType1::Grass:
                    c.ch = ' ';
                    c.color = 0b0010'0000;
                    break;
                case CivModel::TerrainType1::Swamp:
                    c.ch = ' ';
                    c.color = 0b0110'0000;
                    break;
                case CivModel::TerrainType1::Tundra:
                    c.ch = ' ';
                    c.color = 0b0100'0000;
                    break;
            }
            if (point.Type2 == CivModel::TerrainType2::Hill)
            {
                c.color |= 0b1000'0000;
            }
        }
    }

    void View::PrintUnit(int px, int py, CivModel::Unit^ unit)
    {
        auto& c = m_screen->GetChar(px, py);
        if (auto u = dynamic_cast<CivModel::Units::Pioneer^>(unit))
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

    void View::PrintDistrict(int px, int py, CivModel::District^ district)
    {

    }

    std::pair<int, int> View::TerrainToScreen(int x, int y)
    {
        auto scrsz = m_screen->GetSize();

        int sx = scrsz.width / 3;
        int sy = scrsz.height / 3;

        int bx = m_sightx - (sx / 2);
        int by = m_sighty - (sy / 2);

        int dx = x - bx;
        int dy = y - by;

        return {
            dx * 3 + 1 - (y % 2),
            dy * 3 + 1
        };
    }
}
