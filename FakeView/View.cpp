#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        ::MessageBoxW(nullptr,
            L"move: m\n"
            L"focus: f\n"
            L"scroll screen / select: arrow key\n"
            L"quit / cancel: ESC",
            L"info", MB_OK);

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

                auto& c = m_screen->GetChar(px, py);
                if (point.PlacedUnit)
                {
                    c.ch = 'U';
                    c.color = 0b00111110;
                }
                else
                {
                    PrintTerrain(c, point);
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

    void View::PrintTerrain(Character& c, CivModel::Terrain::Point point)
    {
        if (point.Type2 == CivModel::TerrainType2::Mountain)
        {
            c.ch = 'M';
            c.color = 0b01111000;
        }
        else
        {
            switch (point.Type1)
            {
                case CivModel::TerrainType1::Flatland:
                    c.ch = 'F';
                    c.color = 0b00000111;
                    break;
                case CivModel::TerrainType1::Grass:
                    c.ch = 'G';
                    c.color = 0b00000011;
                    break;
                case CivModel::TerrainType1::Swamp:
                    c.ch = 'S';
                    c.color = 0b00000010;
                    break;
                case CivModel::TerrainType1::Tundra:
                    c.ch = 'T';
                    c.color = 0b00000110;
                    break;
            }
            if (point.Type2 == CivModel::TerrainType2::Hill)
            {
                c.color |= 0b00001000;
            }
        }
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
