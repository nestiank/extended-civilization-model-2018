#include "pch.h"
#include "Screen.h"

namespace FakeView
{
    Screen::Screen()
    {
        ClearBuffer();
    }

    int Screen::Loop(IScreenClient^ view)
    {
        std::system("mode con cols=120 lines=36");
        std::system("cls");
        EnableCursor(false);

        DWORD prevTick = GetTickCount();

        while (!m_quit)
        {
            ClearBuffer();
            view->Render();
            DrawBuffer();

            while (!_kbhit())
            {
                DWORD nowTick = GetTickCount();
                if (nowTick - prevTick >= 100)
                {
                    prevTick += 100;

                    view->OnTick();

                    ClearBuffer();
                    view->Render();
                    DrawBuffer();
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

    void Screen::ClearBuffer()
    {
        for (auto& c : m_buffer)
        {
            c.ch = ' ';
            c.color = FOREGROUND_RED | FOREGROUND_GREEN | FOREGROUND_BLUE;
        }
    }

    void Screen::DrawBuffer()
    {
        GotoXY(0, 0);
        for (const auto& c : m_buffer)
        {
            ::SetConsoleTextAttribute(::GetStdHandle(STD_OUTPUT_HANDLE), c.color);
            _putch(c.ch);
        }
    }

    void Screen::Quit(int exitcode)
    {
        m_exitcode = exitcode;
        m_quit = true;
    }

    Character& Screen::GetChar(int x, int y)
    {
        auto sz = GetSize();
        return m_buffer.at(y * sz.width + x);
    }

    Character* Screen::TryGetChar(int x, int y)
    {
        auto sz = GetSize();
        int idx = y * sz.width + x;
        if (static_cast<std::size_t>(idx) >= m_buffer.size())
            return nullptr;

        return &m_buffer[idx];
    }

    void Screen::GotoXY(int x, int y)
    {
        COORD Cur = { static_cast<SHORT>(x), static_cast<SHORT>(y) };
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
        /*CONSOLE_SCREEN_BUFFER_INFO csbi;
        ::GetConsoleScreenBufferInfo(::GetStdHandle(STD_OUTPUT_HANDLE), &csbi);
        return {
            csbi.srWindow.Right - csbi.srWindow.Left + 1,
            csbi.srWindow.Bottom - csbi.srWindow.Top + 1
        };*/
        return { 120, 36 };
    }
}
