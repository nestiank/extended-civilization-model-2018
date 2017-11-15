#include "pch.h"
#include "View.h"

#include "Screen.h"

namespace FakeView
{
    View::View(Screen* screen)
        : m_screen(screen)
    {
        ::MessageBoxW(nullptr, L"move: arrow key\nquit: ESC", L"info", MB_OK);
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

                if (x < 0 || x >= m_game->Terrain->Width)
                    continue;
                if (y < 0 || y >= m_game->Terrain->Height)
                    continue;

                auto point = m_game->Terrain->GetPoint(x, y);

                int px = dx * 3 + 1 - (y % 2);
                int py = dy * 3 + 1;

                auto& c = m_screen->GetChar(px, py);
                switch (point.Type)
                {
                    case CivModel::TerrainType::Flatland:
                        c.ch = 'F';
                        c.color = 0b00000111;
                        break;
                    case CivModel::TerrainType::Grass:
                        c.ch = 'G';
                        c.color = 0b00000010;
                        break;
                    case CivModel::TerrainType::Hill:
                        c.ch = 'H';
                        c.color = 0b00001111;
                        break;
                    case CivModel::TerrainType::Tundra:
                        c.ch = 'T';
                        c.color = 0b00000110;
                        break;
                }
            }
        }
    }

    void View::OnKeyStroke(int ch)
    {
        switch (ch)
        {
            case 0x1b: // ESC
                m_screen->Quit(0);
                break;
            case 0x148: // UP
                --m_sighty;
                break;
            case 0x150: // DOWN
                ++m_sighty;
                break;
            case 0x14b: // LEFT
                --m_sightx;
                break;
            case 0x14d: // RIGHT
                ++m_sightx;
                break;
        }
    }

    void View::OnTick()
    {
    }
}
