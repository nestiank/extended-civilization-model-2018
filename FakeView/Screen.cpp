#include "pch.h"
#include "Screen.h"

namespace FakeView
{
    Screen::Screen()
    {
    }

    int Screen::Loop(IView^ view)
    {
        std::system("mode con cols=120 lines=36");
        EnableCursor(false);

        DWORD prevTick = GetTickCount();

        while (!m_quit)
        {
            std::system("cls");
            view->Render();

            while (!_kbhit())
            {
                DWORD nowTick = GetTickCount();
                if (nowTick - prevTick >= 100)
                {
                    prevTick += 100;

                    view->OnTick();
                    if (m_invalidated)
                    {
                        view->Render();
                    }
                }
                else
                {
                    ::Sleep(1);
                }
            }

            int ch = _getch();
            if (ch == 0 || ch == 0xe0)
            {
                ch = _getch() | 0x0100;
            }

            view->OnKeyStroke(ch);
        }

        return m_exitcode;
    }

    void Screen::Quit(int exitcode)
    {
        m_exitcode = exitcode;
        m_quit = true;
    }

    void Screen::Invalidate()
    {
        m_invalidated = true;
    }

    void Screen::GotoXY(int x, int y)
    {
        COORD Cur = { x, y };
        ::SetConsoleCursorPosition(::GetStdHandle(STD_OUTPUT_HANDLE), Cur);
    }

    void Screen::EnableCursor(bool enable)
    {
        CONSOLE_CURSOR_INFO CurInfo;
        if (enable)
        {
            CurInfo.dwSize = 20;
            CurInfo.bVisible = TRUE;
        }
        else
        {
            CurInfo.dwSize = 1;
            CurInfo.bVisible = FALSE;
        }
        ::SetConsoleCursorInfo(::GetStdHandle(STD_OUTPUT_HANDLE), &CurInfo);
    }

    Size Screen::GetSize()
    {
        CONSOLE_SCREEN_BUFFER_INFO csbi;
        ::GetConsoleScreenBufferInfo(::GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
        return {
            csbi.srWindow.Right - csbi.srWindow.Left + 1,
            csbi.srWindow.Bottom - csbi.srWindow.Top + 1
        };
    }
}
