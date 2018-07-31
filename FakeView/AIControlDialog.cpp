#include "pch.h"
#include "AIControlDialog.h"

namespace FakeView
{
    AIControlDialog::AIControlDialog(CivModel::Game^ game)
        : m_game(game)
    {
        InitializeComponent();
    }

    System::Void AIControlDialog::AIControlDialog_Load(System::Object^  sender, System::EventArgs^  e)
    {
        int idx = 0;
        for each (auto player in m_game->Players)
        {
            chklistAIControl->Items->Add("플레이어 " + idx, player->IsAIControlled);
            ++idx;
        }
    }

    System::Void AIControlDialog::btnEnableAll_Click(System::Object^  sender, System::EventArgs^  e)
    {
        for (int idx = 0; idx < chklistAIControl->Items->Count; ++idx)
        {
            chklistAIControl->SetItemChecked(idx, true);
        }
    }

    System::Void AIControlDialog::btnDisableAll_Click(System::Object^  sender, System::EventArgs^  e)
    {
        for (int idx = 0; idx < chklistAIControl->Items->Count; ++idx)
        {
            chklistAIControl->SetItemChecked(idx, false);
        }
    }

    System::Void AIControlDialog::btnFlipAll_Click(System::Object^  sender, System::EventArgs^  e)
    {
        for (int idx = 0; idx < chklistAIControl->Items->Count; ++idx)
        {
            bool checked = chklistAIControl->GetItemChecked(idx);
            chklistAIControl->SetItemChecked(idx, !checked);
        }
    }

    System::Void AIControlDialog::btnOk_Click(System::Object^  sender, System::EventArgs^  e)
    {
        for (int idx = 0; idx < chklistAIControl->Items->Count; ++idx)
        {
            bool checked = chklistAIControl->GetItemChecked(idx);
            m_game->Players[idx]->IsAIControlled = checked;
        }
    }

    System::Void AIControlDialog::btnCancel_Click(System::Object^  sender, System::EventArgs^  e)
    {
    }
}
