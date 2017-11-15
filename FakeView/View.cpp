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

                m_screen->GotoXY(px, py);
                switch (point.Type)
                {
                    case CivModel::TerrainType::Flatland:
                        _putch('F');
                        break;
                    case CivModel::TerrainType::Grass:
                        _putch('G');
                        break;
                    case CivModel::TerrainType::Hill:
                        _putch('H');
                        break;
                    case CivModel::TerrainType::Tundra:
                        _putch('T');
                        break;
                }
            }
        }
    }

    void View::OnKeyStroke(int ch)
    {
        switch (ch)
        {
            case 27: // ESC
                m_screen->Quit(0);
                break;
        }
    }

    void View::OnTick()
    {
        if (::GetAsyncKeyState(VK_UP))
        {
            --m_sighty;
            m_screen->Invalidate();
        }
        if (::GetAsyncKeyState(VK_DOWN))
        {
            ++m_sighty;
            m_screen->Invalidate();
        }
        if (::GetAsyncKeyState(VK_LEFT))
        {
            --m_sightx;
            m_screen->Invalidate();
        }
        if (::GetAsyncKeyState(VK_RIGHT))
        {
            ++m_sightx;
            m_screen->Invalidate();
        }
    }
}
