#include "pch.h"
#include "View.h"

int main()
{
    using namespace FakeView;

    auto screen = Screen { };
    auto view = gcnew View(&screen);

    return screen.Loop(view);
}
