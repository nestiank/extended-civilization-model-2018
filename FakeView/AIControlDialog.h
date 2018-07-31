#pragma once

namespace FakeView {

    using namespace System;
    using namespace System::ComponentModel;
    using namespace System::Collections;
    using namespace System::Windows::Forms;
    using namespace System::Data;
    using namespace System::Drawing;

    /// <summary>
    /// AIControlDialog에 대한 요약입니다.
    /// </summary>
    public ref class AIControlDialog : public System::Windows::Forms::Form
    {
    public:
        AIControlDialog(CivModel::Game^ game);

    private:
        CivModel::Game^ m_game;

    protected:
        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        ~AIControlDialog()
        {
            if (components)
            {
                delete components;
            }
        }

    private: System::Windows::Forms::CheckedListBox^  chklistAIControl;
    protected:

    private: System::Windows::Forms::Button^  btnEnableAll;
    private: System::Windows::Forms::Button^  btnDisableAll;
    private: System::Windows::Forms::Button^  btnFlipAll;
    private: System::Windows::Forms::Button^  btnOk;
    private: System::Windows::Forms::Button^  btnCancel;
    protected:






    private:
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        System::ComponentModel::Container ^components;

#pragma region Windows Form Designer generated code
        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        void InitializeComponent(void)
        {
            this->chklistAIControl = (gcnew System::Windows::Forms::CheckedListBox());
            this->btnEnableAll = (gcnew System::Windows::Forms::Button());
            this->btnDisableAll = (gcnew System::Windows::Forms::Button());
            this->btnFlipAll = (gcnew System::Windows::Forms::Button());
            this->btnOk = (gcnew System::Windows::Forms::Button());
            this->btnCancel = (gcnew System::Windows::Forms::Button());
            this->SuspendLayout();
            // 
            // chklistAIControl
            // 
            this->chklistAIControl->FormattingEnabled = true;
            this->chklistAIControl->Location = System::Drawing::Point(14, 11);
            this->chklistAIControl->Name = L"chklistAIControl";
            this->chklistAIControl->Size = System::Drawing::Size(261, 260);
            this->chklistAIControl->TabIndex = 0;
            // 
            // btnEnableAll
            // 
            this->btnEnableAll->Location = System::Drawing::Point(297, 11);
            this->btnEnableAll->Name = L"btnEnableAll";
            this->btnEnableAll->Size = System::Drawing::Size(128, 23);
            this->btnEnableAll->TabIndex = 1;
            this->btnEnableAll->Text = L"모두 활성화";
            this->btnEnableAll->UseVisualStyleBackColor = true;
            this->btnEnableAll->Click += gcnew System::EventHandler(this, &AIControlDialog::btnEnableAll_Click);
            // 
            // btnDisableAll
            // 
            this->btnDisableAll->Location = System::Drawing::Point(297, 49);
            this->btnDisableAll->Name = L"btnDisableAll";
            this->btnDisableAll->Size = System::Drawing::Size(128, 23);
            this->btnDisableAll->TabIndex = 2;
            this->btnDisableAll->Text = L"모두 비활성화";
            this->btnDisableAll->UseVisualStyleBackColor = true;
            this->btnDisableAll->Click += gcnew System::EventHandler(this, &AIControlDialog::btnDisableAll_Click);
            // 
            // btnFlipAll
            // 
            this->btnFlipAll->Location = System::Drawing::Point(297, 87);
            this->btnFlipAll->Name = L"btnFlipAll";
            this->btnFlipAll->Size = System::Drawing::Size(128, 23);
            this->btnFlipAll->TabIndex = 3;
            this->btnFlipAll->Text = L"모두 반전";
            this->btnFlipAll->UseVisualStyleBackColor = true;
            this->btnFlipAll->Click += gcnew System::EventHandler(this, &AIControlDialog::btnFlipAll_Click);
            // 
            // btnOk
            // 
            this->btnOk->DialogResult = System::Windows::Forms::DialogResult::OK;
            this->btnOk->Location = System::Drawing::Point(253, 296);
            this->btnOk->Name = L"btnOk";
            this->btnOk->Size = System::Drawing::Size(75, 23);
            this->btnOk->TabIndex = 4;
            this->btnOk->Text = L"확인";
            this->btnOk->UseVisualStyleBackColor = true;
            this->btnOk->Click += gcnew System::EventHandler(this, &AIControlDialog::btnOk_Click);
            // 
            // btnCancel
            // 
            this->btnCancel->DialogResult = System::Windows::Forms::DialogResult::Cancel;
            this->btnCancel->Location = System::Drawing::Point(350, 296);
            this->btnCancel->Name = L"btnCancel";
            this->btnCancel->Size = System::Drawing::Size(75, 23);
            this->btnCancel->TabIndex = 5;
            this->btnCancel->Text = L"취소";
            this->btnCancel->UseVisualStyleBackColor = true;
            this->btnCancel->Click += gcnew System::EventHandler(this, &AIControlDialog::btnCancel_Click);
            // 
            // AIControlDialog
            // 
            this->AutoScaleDimensions = System::Drawing::SizeF(7, 12);
            this->AutoScaleMode = System::Windows::Forms::AutoScaleMode::Font;
            this->ClientSize = System::Drawing::Size(438, 330);
            this->Controls->Add(this->btnCancel);
            this->Controls->Add(this->btnOk);
            this->Controls->Add(this->btnFlipAll);
            this->Controls->Add(this->btnDisableAll);
            this->Controls->Add(this->btnEnableAll);
            this->Controls->Add(this->chklistAIControl);
            this->FormBorderStyle = System::Windows::Forms::FormBorderStyle::FixedDialog;
            this->MaximizeBox = false;
            this->Name = L"AIControlDialog";
            this->Text = L"AI 컨트롤 변경";
            this->Load += gcnew System::EventHandler(this, &AIControlDialog::AIControlDialog_Load);
            this->ResumeLayout(false);

        }
#pragma endregion

    private:
        System::Void AIControlDialog_Load(System::Object^  sender, System::EventArgs^  e);
        System::Void btnEnableAll_Click(System::Object^  sender, System::EventArgs^  e);
        System::Void btnDisableAll_Click(System::Object^  sender, System::EventArgs^  e);
        System::Void btnFlipAll_Click(System::Object^  sender, System::EventArgs^  e);
        System::Void btnOk_Click(System::Object^  sender, System::EventArgs^  e);
        System::Void btnCancel_Click(System::Object^  sender, System::EventArgs^  e);
};
}
