<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class KFJMAST030
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    Public Sub New()
        MyBase.New()

        ' この呼び出しは Windows フォーム デザイナで必要です。
        InitializeComponent()

        ' InitializeComponent() 呼び出しの後に初期化を追加します。

        '検索カナ
        AddHandler cmbKana.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbKana.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler cmbKana.LostFocus, AddressOf CAST.LostFocus

        '検索取引先
        AddHandler cmbToriName.GotFocus, AddressOf CAST.GotFocus
        AddHandler cmbToriName.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler cmbToriName.LostFocus, AddressOf CAST.LostFocus

        '取引先主コード
        AddHandler TORIS_CODE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TORIS_CODE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TORIS_CODE_S.LostFocus, AddressOf CAST.LostFocus

        '取引先副コード
        AddHandler TORIF_CODE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TORIF_CODE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TORIF_CODE_S.LostFocus, AddressOf CAST.LostFocus

        '委託者コード
        AddHandler ITAKU_CODE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler ITAKU_CODE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler ITAKU_CODE_S.LostFocus, AddressOf CAST.LostFocus

        '契約振替日(年)     
        AddHandler KFURI_DATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler KFURI_DATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KFURI_DATE_S.LostFocus, AddressOf CAST.LostFocus

        '契約振替日(月)
        AddHandler KFURI_DATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler KFURI_DATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KFURI_DATE_S1.LostFocus, AddressOf CAST.LostFocus

        '契約振替日(日)
        AddHandler KFURI_DATE_S2.GotFocus, AddressOf CAST.GotFocus
        AddHandler KFURI_DATE_S2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KFURI_DATE_S2.LostFocus, AddressOf CAST.LostFocus

        '振替日(年) 
        AddHandler FURI_DATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURI_DATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_S.LostFocus, AddressOf CAST.LostFocus

        '振替日(月)
        AddHandler FURI_DATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURI_DATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_S1.LostFocus, AddressOf CAST.LostFocus

        '振替日(日)
        AddHandler FURI_DATE_S2.GotFocus, AddressOf CAST.GotFocus
        AddHandler FURI_DATE_S2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler FURI_DATE_S2.LostFocus, AddressOf CAST.LostFocus

        '手数料徴求予定日(年) 
        AddHandler TESUU_YDATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUU_YDATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TESUU_YDATE_S.LostFocus, AddressOf CAST.LostFocus

        '手数料徴求予定日(月)
        AddHandler TESUU_YDATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUU_YDATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TESUU_YDATE_S1.LostFocus, AddressOf CAST.LostFocus

        '手数料徴求予定日(日)
        AddHandler TESUU_YDATE_S2.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUU_YDATE_S2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TESUU_YDATE_S2.LostFocus, AddressOf CAST.LostFocus

        '資金決済予定日(年) 
        AddHandler KESSAI_YDATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler KESSAI_YDATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KESSAI_YDATE_S.LostFocus, AddressOf CAST.LostFocus

        '資金決済予定日(月)
        AddHandler KESSAI_YDATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler KESSAI_YDATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KESSAI_YDATE_S1.LostFocus, AddressOf CAST.LostFocus

        '資金決済予定日(日)
        AddHandler KESSAI_YDATE_S2.GotFocus, AddressOf CAST.GotFocus
        AddHandler KESSAI_YDATE_S2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KESSAI_YDATE_S2.LostFocus, AddressOf CAST.LostFocus

        '再振予定日(年) 
        AddHandler KSAIFURI_DATE_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler KSAIFURI_DATE_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KSAIFURI_DATE_S.LostFocus, AddressOf CAST.LostFocus

        '再振予定日(月)
        AddHandler KSAIFURI_DATE_S1.GotFocus, AddressOf CAST.GotFocus
        AddHandler KSAIFURI_DATE_S1.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KSAIFURI_DATE_S1.LostFocus, AddressOf CAST.LostFocus

        '再振予定日(日)
        AddHandler KSAIFURI_DATE_S2.GotFocus, AddressOf CAST.GotFocus
        AddHandler KSAIFURI_DATE_S2.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler KSAIFURI_DATE_S2.LostFocus, AddressOf CAST.LostFocus

        '受付 UKETUKE_FLG_S
        AddHandler UKETUKE_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler UKETUKE_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler UKETUKE_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '登録 TOUROKU_FLG_S
        AddHandler TOUROKU_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TOUROKU_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler TOUROKU_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '配自 HAISIN_FLG_S
        AddHandler HAISIN_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler HAISIN_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler HAISIN_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '再振 SAIFURI_FLG_S
        AddHandler SAIFURI_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler SAIFURI_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler SAIFURI_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '送信 SOUSIN_FLG_S
        AddHandler SOUSIN_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler SOUSIN_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler SOUSIN_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '不能 FUNOU_FLG_S
        AddHandler FUNOU_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler FUNOU_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler FUNOU_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '手計 TESUUKEI_FLG_S
        AddHandler TESUUKEI_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUUKEI_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler TESUUKEI_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '手徴 TESUUTYO_FLG_S
        AddHandler TESUUTYO_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUUTYO_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler TESUUTYO_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '決済 KESSAI_FLG_S
        AddHandler KESSAI_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler KESSAI_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler KESSAI_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '返還 HENKAN_FLG_S      HENKYAKU_FLG_S
        AddHandler HENKAN_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler HENKAN_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler HENKAN_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '中断 TYUUDAN_FLG_S
        AddHandler TYUUDAN_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TYUUDAN_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler TYUUDAN_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '他行 TAKOU_FLG_S
        AddHandler TAKOU_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TAKOU_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler TAKOU_FLG_S.LostFocus, AddressOf CAST.LostFocus

        '日報 NIPPO_FLG_S
        AddHandler NIPPO_FLG_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler NIPPO_FLG_S.KeyPress, AddressOf CASTx02.KeyPress
        AddHandler NIPPO_FLG_S.LostFocus, AddressOf CAST.LostFocus

        AddHandler TESUU_KIN_S.GotFocus, AddressOf CAST.GotFocus
        AddHandler TESUU_KIN_S.KeyPress, AddressOf CASTx01.KeyPress
        AddHandler TESUU_KIN_S.LostFocus, AddressOf CAST.LostFocus

    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(KFJMAST030))
        Me.lblUser = New System.Windows.Forms.Label()
        Me.lblDate = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label104 = New System.Windows.Forms.Label()
        Me.Label103 = New System.Windows.Forms.Label()
        Me.Label102 = New System.Windows.Forms.Label()
        Me.Label101 = New System.Windows.Forms.Label()
        Me.Label100 = New System.Windows.Forms.Label()
        Me.Label99 = New System.Windows.Forms.Label()
        Me.Label98 = New System.Windows.Forms.Label()
        Me.Label97 = New System.Windows.Forms.Label()
        Me.SAIFURI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.SAIFURI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.SAIFURI_DATE_S = New System.Windows.Forms.TextBox()
        Me.KSAIFURI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.KSAIFURI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.KSAIFURI_DATE_S = New System.Windows.Forms.TextBox()
        Me.cmbToriName = New System.Windows.Forms.ComboBox()
        Me.Label96 = New System.Windows.Forms.Label()
        Me.Label91 = New System.Windows.Forms.Label()
        Me.Label92 = New System.Windows.Forms.Label()
        Me.Label93 = New System.Windows.Forms.Label()
        Me.Label88 = New System.Windows.Forms.Label()
        Me.Label89 = New System.Windows.Forms.Label()
        Me.Label90 = New System.Windows.Forms.Label()
        Me.Label85 = New System.Windows.Forms.Label()
        Me.Label86 = New System.Windows.Forms.Label()
        Me.Label87 = New System.Windows.Forms.Label()
        Me.Label82 = New System.Windows.Forms.Label()
        Me.Label83 = New System.Windows.Forms.Label()
        Me.Label84 = New System.Windows.Forms.Label()
        Me.Label79 = New System.Windows.Forms.Label()
        Me.Label80 = New System.Windows.Forms.Label()
        Me.Label81 = New System.Windows.Forms.Label()
        Me.Label76 = New System.Windows.Forms.Label()
        Me.Label77 = New System.Windows.Forms.Label()
        Me.Label78 = New System.Windows.Forms.Label()
        Me.Label73 = New System.Windows.Forms.Label()
        Me.Label74 = New System.Windows.Forms.Label()
        Me.Label75 = New System.Windows.Forms.Label()
        Me.Label70 = New System.Windows.Forms.Label()
        Me.Label71 = New System.Windows.Forms.Label()
        Me.Label72 = New System.Windows.Forms.Label()
        Me.Label67 = New System.Windows.Forms.Label()
        Me.Label68 = New System.Windows.Forms.Label()
        Me.Label69 = New System.Windows.Forms.Label()
        Me.Label64 = New System.Windows.Forms.Label()
        Me.Label65 = New System.Windows.Forms.Label()
        Me.Label66 = New System.Windows.Forms.Label()
        Me.Label61 = New System.Windows.Forms.Label()
        Me.Label62 = New System.Windows.Forms.Label()
        Me.Label63 = New System.Windows.Forms.Label()
        Me.Label58 = New System.Windows.Forms.Label()
        Me.Label59 = New System.Windows.Forms.Label()
        Me.Label60 = New System.Windows.Forms.Label()
        Me.Label55 = New System.Windows.Forms.Label()
        Me.Label56 = New System.Windows.Forms.Label()
        Me.Label57 = New System.Windows.Forms.Label()
        Me.Label54 = New System.Windows.Forms.Label()
        Me.Label53 = New System.Windows.Forms.Label()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.Label50 = New System.Windows.Forms.Label()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.Label45 = New System.Windows.Forms.Label()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label40 = New System.Windows.Forms.Label()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Label35 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.他行 = New System.Windows.Forms.Label()
        Me.Label31 = New System.Windows.Forms.Label()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label94 = New System.Windows.Forms.Label()
        Me.Label95 = New System.Windows.Forms.Label()
        Me.Label106 = New System.Windows.Forms.Label()
        Me.Label107 = New System.Windows.Forms.Label()
        Me.cmbKana = New System.Windows.Forms.ComboBox()
        Me.BAITAI_CODE_S = New System.Windows.Forms.TextBox()
        Me.ITAKU_CODE_S = New System.Windows.Forms.TextBox()
        Me.CmdBack = New System.Windows.Forms.Button()
        Me.CmdDelete = New System.Windows.Forms.Button()
        Me.CmdSelect = New System.Windows.Forms.Button()
        Me.CmdUpdate = New System.Windows.Forms.Button()
        Me.CmdInsert = New System.Windows.Forms.Button()
        Me.CmdClear = New System.Windows.Forms.Button()
        Me.KESSAI_TIME_STAMP_S = New System.Windows.Forms.TextBox()
        Me.JIFURI_TIME_STAMP_S = New System.Windows.Forms.TextBox()
        Me.SAKUSEI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.SAKUSEI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.SAKUSEI_DATE_S = New System.Windows.Forms.TextBox()
        Me.MOTIKOMI_SEQ_S = New System.Windows.Forms.TextBox()
        Me.SFILE_NAME_S = New System.Windows.Forms.TextBox()
        Me.FILE_SEQ_S = New System.Windows.Forms.TextBox()
        Me.MOTIKOMI_KBN_S = New System.Windows.Forms.TextBox()
        Me.SOUSIN_KBN_S = New System.Windows.Forms.TextBox()
        Me.TESUU_KIN_S = New System.Windows.Forms.TextBox()
        Me.TESUU_KBN_S = New System.Windows.Forms.TextBox()
        Me.FUNOU_KIN_S = New System.Windows.Forms.TextBox()
        Me.FURI_KIN_S = New System.Windows.Forms.TextBox()
        Me.SYORI_KIN_S = New System.Windows.Forms.TextBox()
        Me.FUNOU_KEN_S = New System.Windows.Forms.TextBox()
        Me.FURI_KEN_S = New System.Windows.Forms.TextBox()
        Me.SYORI_KEN_S = New System.Windows.Forms.TextBox()
        Me.TYUUDAN_FLG_S = New System.Windows.Forms.TextBox()
        Me.HENKAN_FLG_S = New System.Windows.Forms.TextBox()
        Me.TESUUTYO_FLG_S = New System.Windows.Forms.TextBox()
        Me.KESSAI_FLG_S = New System.Windows.Forms.TextBox()
        Me.TESUUKEI_FLG_S = New System.Windows.Forms.TextBox()
        Me.NIPPO_FLG_S = New System.Windows.Forms.TextBox()
        Me.FUNOU_FLG_S = New System.Windows.Forms.TextBox()
        Me.SAIFURI_FLG_S = New System.Windows.Forms.TextBox()
        Me.SOUSIN_FLG_S = New System.Windows.Forms.TextBox()
        Me.HAISIN_FLG_S = New System.Windows.Forms.TextBox()
        Me.TOUROKU_FLG_S = New System.Windows.Forms.TextBox()
        Me.UKETUKE_FLG_S = New System.Windows.Forms.TextBox()
        Me.KESSAI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.KESSAI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.KESSAI_DATE_S = New System.Windows.Forms.TextBox()
        Me.HENKAN_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.HENKAN_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.HENKAN_DATE_S = New System.Windows.Forms.TextBox()
        Me.FUNOU_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.FUNOU_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.FUNOU_DATE_S = New System.Windows.Forms.TextBox()
        Me.SOUSIN_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.SOUSIN_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.SOUSIN_DATE_S = New System.Windows.Forms.TextBox()
        Me.HAISIN_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.HAISIN_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.HAISIN_DATE_S = New System.Windows.Forms.TextBox()
        Me.TOUROKU_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.TOUROKU_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.TOUROKU_DATE_S = New System.Windows.Forms.TextBox()
        Me.KESSAI_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.KESSAI_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.KESSAI_YDATE_S = New System.Windows.Forms.TextBox()
        Me.HENKAN_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.HENKAN_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.HENKAN_YDATE_S = New System.Windows.Forms.TextBox()
        Me.FUNOU_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.FUNOU_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.FUNOU_YDATE_S = New System.Windows.Forms.TextBox()
        Me.SOUSIN_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.SOUSIN_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.SOUSIN_YDATE_S = New System.Windows.Forms.TextBox()
        Me.HAISIN_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.HAISIN_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.HAISIN_YDATE_S = New System.Windows.Forms.TextBox()
        Me.UKETUKE_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.UKETUKE_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.UKETUKE_DATE_S = New System.Windows.Forms.TextBox()
        Me.FURI_CODE_S = New System.Windows.Forms.TextBox()
        Me.KIGYO_CODE_S = New System.Windows.Forms.TextBox()
        Me.FURI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.FURI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.FURI_DATE_S = New System.Windows.Forms.TextBox()
        Me.ITAKU_NNAME_T = New System.Windows.Forms.TextBox()
        Me.TORIF_CODE_S = New System.Windows.Forms.TextBox()
        Me.TORIS_CODE_S = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label105 = New System.Windows.Forms.Label()
        Me.KFURI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.KFURI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.KFURI_DATE_S = New System.Windows.Forms.TextBox()
        Me.Label111 = New System.Windows.Forms.Label()
        Me.Label112 = New System.Windows.Forms.Label()
        Me.Label113 = New System.Windows.Forms.Label()
        Me.Label114 = New System.Windows.Forms.Label()
        Me.Label115 = New System.Windows.Forms.Label()
        Me.Label116 = New System.Windows.Forms.Label()
        Me.Label117 = New System.Windows.Forms.Label()
        Me.Label118 = New System.Windows.Forms.Label()
        Me.TESUU_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.TESUU_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.TESUU_DATE_S = New System.Windows.Forms.TextBox()
        Me.TESUU_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.TESUU_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.TESUU_YDATE_S = New System.Windows.Forms.TextBox()
        Me.MEM_FLG = New System.Windows.Forms.TextBox()
        Me.TESUU_TIME_STAMP_S = New System.Windows.Forms.TextBox()
        Me.Label119 = New System.Windows.Forms.Label()
        Me.Label120 = New System.Windows.Forms.Label()
        Me.Label121 = New System.Windows.Forms.Label()
        Me.Label122 = New System.Windows.Forms.Label()
        Me.Label123 = New System.Windows.Forms.Label()
        Me.Label124 = New System.Windows.Forms.Label()
        Me.Label125 = New System.Windows.Forms.Label()
        Me.Label126 = New System.Windows.Forms.Label()
        Me.IRAISYOK_YDATE_S2 = New System.Windows.Forms.TextBox()
        Me.IRAISYOK_YDATE_S1 = New System.Windows.Forms.TextBox()
        Me.IRAISYOK_YDATE_S = New System.Windows.Forms.TextBox()
        Me.IRAISYO_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.IRAISYO_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.IRAISYO_DATE_S = New System.Windows.Forms.TextBox()
        Me.Label131 = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.TAKOU_FLG_S = New System.Windows.Forms.TextBox()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.ERR_KIN_S = New System.Windows.Forms.TextBox()
        Me.ERR_KEN_S = New System.Windows.Forms.TextBox()
        Me.Label127 = New System.Windows.Forms.Label()
        Me.Label128 = New System.Windows.Forms.Label()
        Me.Label129 = New System.Windows.Forms.Label()
        Me.MOTIKOMI_DATE_S2 = New System.Windows.Forms.TextBox()
        Me.MOTIKOMI_DATE_S1 = New System.Windows.Forms.TextBox()
        Me.MOTIKOMI_DATE_S = New System.Windows.Forms.TextBox()
        Me.Label130 = New System.Windows.Forms.Label()
        Me.txtBeforeSaifuri = New System.Windows.Forms.TextBox()
        Me.btnFuriDate = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lblUser
        '
        Me.lblUser.AutoSize = True
        Me.lblUser.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblUser.Location = New System.Drawing.Point(682, 9)
        Me.lblUser.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblUser.Name = "lblUser"
        Me.lblUser.Size = New System.Drawing.Size(35, 12)
        Me.lblUser.TabIndex = 32
        Me.lblUser.Text = "admin"
        '
        'lblDate
        '
        Me.lblDate.AutoSize = True
        Me.lblDate.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.lblDate.Location = New System.Drawing.Point(686, 27)
        Me.lblDate.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.lblDate.Name = "lblDate"
        Me.lblDate.Size = New System.Drawing.Size(89, 12)
        Me.lblDate.TabIndex = 31
        Me.lblDate.Text = "YYYY年MM月DD日"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label3.Location = New System.Drawing.Point(599, 27)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(83, 12)
        Me.Label3.TabIndex = 30
        Me.Label3.Text = "システム日付:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label2.Location = New System.Drawing.Point(615, 9)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(83, 12)
        Me.Label2.TabIndex = 29
        Me.Label2.Text = "ログイン名　:"
        '
        'Label104
        '
        Me.Label104.AutoSize = True
        Me.Label104.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label104.Location = New System.Drawing.Point(555, 341)
        Me.Label104.Name = "Label104"
        Me.Label104.Size = New System.Drawing.Size(21, 13)
        Me.Label104.TabIndex = 399
        Me.Label104.Text = "月"
        '
        'Label103
        '
        Me.Label103.AutoSize = True
        Me.Label103.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label103.Location = New System.Drawing.Point(498, 341)
        Me.Label103.Name = "Label103"
        Me.Label103.Size = New System.Drawing.Size(21, 13)
        Me.Label103.TabIndex = 398
        Me.Label103.Text = "年"
        '
        'Label102
        '
        Me.Label102.AutoSize = True
        Me.Label102.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label102.Location = New System.Drawing.Point(616, 341)
        Me.Label102.Name = "Label102"
        Me.Label102.Size = New System.Drawing.Size(21, 13)
        Me.Label102.TabIndex = 397
        Me.Label102.Text = "日"
        '
        'Label101
        '
        Me.Label101.AutoSize = True
        Me.Label101.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label101.Location = New System.Drawing.Point(284, 341)
        Me.Label101.Name = "Label101"
        Me.Label101.Size = New System.Drawing.Size(21, 13)
        Me.Label101.TabIndex = 396
        Me.Label101.Text = "日"
        '
        'Label100
        '
        Me.Label100.AutoSize = True
        Me.Label100.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label100.Location = New System.Drawing.Point(228, 341)
        Me.Label100.Name = "Label100"
        Me.Label100.Size = New System.Drawing.Size(21, 13)
        Me.Label100.TabIndex = 395
        Me.Label100.Text = "月"
        '
        'Label99
        '
        Me.Label99.AutoSize = True
        Me.Label99.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label99.Location = New System.Drawing.Point(173, 341)
        Me.Label99.Name = "Label99"
        Me.Label99.Size = New System.Drawing.Size(21, 13)
        Me.Label99.TabIndex = 394
        Me.Label99.Text = "年"
        '
        'Label98
        '
        Me.Label98.AutoSize = True
        Me.Label98.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label98.Location = New System.Drawing.Point(337, 341)
        Me.Label98.Name = "Label98"
        Me.Label98.Size = New System.Drawing.Size(77, 13)
        Me.Label98.TabIndex = 393
        Me.Label98.Text = "再　振　日"
        '
        'Label97
        '
        Me.Label97.AutoSize = True
        Me.Label97.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label97.Location = New System.Drawing.Point(27, 341)
        Me.Label97.Name = "Label97"
        Me.Label97.Size = New System.Drawing.Size(77, 13)
        Me.Label97.TabIndex = 392
        Me.Label97.Text = "再振予定日"
        '
        'SAIFURI_DATE_S2
        '
        Me.SAIFURI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.SAIFURI_DATE_S2.Enabled = False
        Me.SAIFURI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAIFURI_DATE_S2.Location = New System.Drawing.Point(584, 337)
        Me.SAIFURI_DATE_S2.MaxLength = 2
        Me.SAIFURI_DATE_S2.Name = "SAIFURI_DATE_S2"
        Me.SAIFURI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.SAIFURI_DATE_S2.TabIndex = 391
        '
        'SAIFURI_DATE_S1
        '
        Me.SAIFURI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.SAIFURI_DATE_S1.Enabled = False
        Me.SAIFURI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAIFURI_DATE_S1.Location = New System.Drawing.Point(523, 337)
        Me.SAIFURI_DATE_S1.MaxLength = 2
        Me.SAIFURI_DATE_S1.Name = "SAIFURI_DATE_S1"
        Me.SAIFURI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.SAIFURI_DATE_S1.TabIndex = 390
        '
        'SAIFURI_DATE_S
        '
        Me.SAIFURI_DATE_S.BackColor = System.Drawing.Color.White
        Me.SAIFURI_DATE_S.Enabled = False
        Me.SAIFURI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAIFURI_DATE_S.Location = New System.Drawing.Point(459, 337)
        Me.SAIFURI_DATE_S.MaxLength = 4
        Me.SAIFURI_DATE_S.Name = "SAIFURI_DATE_S"
        Me.SAIFURI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.SAIFURI_DATE_S.TabIndex = 389
        '
        'KSAIFURI_DATE_S2
        '
        Me.KSAIFURI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.KSAIFURI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KSAIFURI_DATE_S2.Location = New System.Drawing.Point(252, 337)
        Me.KSAIFURI_DATE_S2.MaxLength = 2
        Me.KSAIFURI_DATE_S2.Name = "KSAIFURI_DATE_S2"
        Me.KSAIFURI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.KSAIFURI_DATE_S2.TabIndex = 275
        '
        'KSAIFURI_DATE_S1
        '
        Me.KSAIFURI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.KSAIFURI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KSAIFURI_DATE_S1.Location = New System.Drawing.Point(196, 337)
        Me.KSAIFURI_DATE_S1.MaxLength = 2
        Me.KSAIFURI_DATE_S1.Name = "KSAIFURI_DATE_S1"
        Me.KSAIFURI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.KSAIFURI_DATE_S1.TabIndex = 273
        '
        'KSAIFURI_DATE_S
        '
        Me.KSAIFURI_DATE_S.BackColor = System.Drawing.Color.White
        Me.KSAIFURI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KSAIFURI_DATE_S.Location = New System.Drawing.Point(135, 337)
        Me.KSAIFURI_DATE_S.MaxLength = 4
        Me.KSAIFURI_DATE_S.Name = "KSAIFURI_DATE_S"
        Me.KSAIFURI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.KSAIFURI_DATE_S.TabIndex = 272
        '
        'cmbToriName
        '
        Me.cmbToriName.BackColor = System.Drawing.Color.White
        Me.cmbToriName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbToriName.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbToriName.Location = New System.Drawing.Point(187, 46)
        Me.cmbToriName.Name = "cmbToriName"
        Me.cmbToriName.Size = New System.Drawing.Size(355, 21)
        Me.cmbToriName.TabIndex = 2
        Me.cmbToriName.TabStop = False
        '
        'Label96
        '
        Me.Label96.AutoSize = True
        Me.Label96.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label96.Location = New System.Drawing.Point(27, 49)
        Me.Label96.Name = "Label96"
        Me.Label96.Size = New System.Drawing.Size(77, 13)
        Me.Label96.TabIndex = 386
        Me.Label96.Text = "取引先検索"
        '
        'Label91
        '
        Me.Label91.AutoSize = True
        Me.Label91.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label91.Location = New System.Drawing.Point(754, 496)
        Me.Label91.Name = "Label91"
        Me.Label91.Size = New System.Drawing.Size(21, 13)
        Me.Label91.TabIndex = 375
        Me.Label91.Text = "日"
        '
        'Label92
        '
        Me.Label92.AutoSize = True
        Me.Label92.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label92.Location = New System.Drawing.Point(703, 496)
        Me.Label92.Name = "Label92"
        Me.Label92.Size = New System.Drawing.Size(21, 13)
        Me.Label92.TabIndex = 374
        Me.Label92.Text = "月"
        '
        'Label93
        '
        Me.Label93.AutoSize = True
        Me.Label93.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label93.Location = New System.Drawing.Point(651, 496)
        Me.Label93.Name = "Label93"
        Me.Label93.Size = New System.Drawing.Size(21, 13)
        Me.Label93.TabIndex = 373
        Me.Label93.Text = "年"
        '
        'Label88
        '
        Me.Label88.AutoSize = True
        Me.Label88.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label88.Location = New System.Drawing.Point(616, 297)
        Me.Label88.Name = "Label88"
        Me.Label88.Size = New System.Drawing.Size(21, 13)
        Me.Label88.TabIndex = 343
        Me.Label88.Text = "日"
        '
        'Label89
        '
        Me.Label89.AutoSize = True
        Me.Label89.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label89.Location = New System.Drawing.Point(555, 297)
        Me.Label89.Name = "Label89"
        Me.Label89.Size = New System.Drawing.Size(21, 13)
        Me.Label89.TabIndex = 342
        Me.Label89.Text = "月"
        '
        'Label90
        '
        Me.Label90.AutoSize = True
        Me.Label90.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label90.Location = New System.Drawing.Point(498, 297)
        Me.Label90.Name = "Label90"
        Me.Label90.Size = New System.Drawing.Size(21, 13)
        Me.Label90.TabIndex = 341
        Me.Label90.Text = "年"
        '
        'Label85
        '
        Me.Label85.AutoSize = True
        Me.Label85.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label85.Location = New System.Drawing.Point(616, 275)
        Me.Label85.Name = "Label85"
        Me.Label85.Size = New System.Drawing.Size(21, 13)
        Me.Label85.TabIndex = 337
        Me.Label85.Text = "日"
        '
        'Label86
        '
        Me.Label86.AutoSize = True
        Me.Label86.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label86.Location = New System.Drawing.Point(555, 275)
        Me.Label86.Name = "Label86"
        Me.Label86.Size = New System.Drawing.Size(21, 13)
        Me.Label86.TabIndex = 336
        Me.Label86.Text = "月"
        '
        'Label87
        '
        Me.Label87.AutoSize = True
        Me.Label87.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label87.Location = New System.Drawing.Point(498, 275)
        Me.Label87.Name = "Label87"
        Me.Label87.Size = New System.Drawing.Size(21, 13)
        Me.Label87.TabIndex = 335
        Me.Label87.Text = "年"
        '
        'Label82
        '
        Me.Label82.AutoSize = True
        Me.Label82.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label82.Location = New System.Drawing.Point(616, 253)
        Me.Label82.Name = "Label82"
        Me.Label82.Size = New System.Drawing.Size(21, 13)
        Me.Label82.TabIndex = 331
        Me.Label82.Text = "日"
        '
        'Label83
        '
        Me.Label83.AutoSize = True
        Me.Label83.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label83.Location = New System.Drawing.Point(555, 253)
        Me.Label83.Name = "Label83"
        Me.Label83.Size = New System.Drawing.Size(21, 13)
        Me.Label83.TabIndex = 330
        Me.Label83.Text = "月"
        '
        'Label84
        '
        Me.Label84.AutoSize = True
        Me.Label84.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label84.Location = New System.Drawing.Point(498, 253)
        Me.Label84.Name = "Label84"
        Me.Label84.Size = New System.Drawing.Size(21, 13)
        Me.Label84.TabIndex = 329
        Me.Label84.Text = "年"
        '
        'Label79
        '
        Me.Label79.AutoSize = True
        Me.Label79.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label79.Location = New System.Drawing.Point(616, 231)
        Me.Label79.Name = "Label79"
        Me.Label79.Size = New System.Drawing.Size(21, 13)
        Me.Label79.TabIndex = 325
        Me.Label79.Text = "日"
        '
        'Label80
        '
        Me.Label80.AutoSize = True
        Me.Label80.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label80.Location = New System.Drawing.Point(555, 231)
        Me.Label80.Name = "Label80"
        Me.Label80.Size = New System.Drawing.Size(21, 13)
        Me.Label80.TabIndex = 324
        Me.Label80.Text = "月"
        '
        'Label81
        '
        Me.Label81.AutoSize = True
        Me.Label81.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label81.Location = New System.Drawing.Point(498, 231)
        Me.Label81.Name = "Label81"
        Me.Label81.Size = New System.Drawing.Size(21, 13)
        Me.Label81.TabIndex = 323
        Me.Label81.Text = "年"
        '
        'Label76
        '
        Me.Label76.AutoSize = True
        Me.Label76.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label76.Location = New System.Drawing.Point(616, 209)
        Me.Label76.Name = "Label76"
        Me.Label76.Size = New System.Drawing.Size(21, 13)
        Me.Label76.TabIndex = 319
        Me.Label76.Text = "日"
        '
        'Label77
        '
        Me.Label77.AutoSize = True
        Me.Label77.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label77.Location = New System.Drawing.Point(555, 209)
        Me.Label77.Name = "Label77"
        Me.Label77.Size = New System.Drawing.Size(21, 13)
        Me.Label77.TabIndex = 318
        Me.Label77.Text = "月"
        '
        'Label78
        '
        Me.Label78.AutoSize = True
        Me.Label78.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label78.Location = New System.Drawing.Point(498, 209)
        Me.Label78.Name = "Label78"
        Me.Label78.Size = New System.Drawing.Size(21, 13)
        Me.Label78.TabIndex = 317
        Me.Label78.Text = "年"
        '
        'Label73
        '
        Me.Label73.AutoSize = True
        Me.Label73.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label73.Location = New System.Drawing.Point(616, 187)
        Me.Label73.Name = "Label73"
        Me.Label73.Size = New System.Drawing.Size(21, 13)
        Me.Label73.TabIndex = 313
        Me.Label73.Text = "日"
        '
        'Label74
        '
        Me.Label74.AutoSize = True
        Me.Label74.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label74.Location = New System.Drawing.Point(555, 187)
        Me.Label74.Name = "Label74"
        Me.Label74.Size = New System.Drawing.Size(21, 13)
        Me.Label74.TabIndex = 312
        Me.Label74.Text = "月"
        '
        'Label75
        '
        Me.Label75.AutoSize = True
        Me.Label75.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label75.Location = New System.Drawing.Point(498, 187)
        Me.Label75.Name = "Label75"
        Me.Label75.Size = New System.Drawing.Size(21, 13)
        Me.Label75.TabIndex = 311
        Me.Label75.Text = "年"
        '
        'Label70
        '
        Me.Label70.AutoSize = True
        Me.Label70.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label70.Location = New System.Drawing.Point(284, 297)
        Me.Label70.Name = "Label70"
        Me.Label70.Size = New System.Drawing.Size(21, 13)
        Me.Label70.TabIndex = 307
        Me.Label70.Text = "日"
        '
        'Label71
        '
        Me.Label71.AutoSize = True
        Me.Label71.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label71.Location = New System.Drawing.Point(228, 297)
        Me.Label71.Name = "Label71"
        Me.Label71.Size = New System.Drawing.Size(21, 13)
        Me.Label71.TabIndex = 306
        Me.Label71.Text = "月"
        '
        'Label72
        '
        Me.Label72.AutoSize = True
        Me.Label72.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label72.Location = New System.Drawing.Point(173, 297)
        Me.Label72.Name = "Label72"
        Me.Label72.Size = New System.Drawing.Size(21, 13)
        Me.Label72.TabIndex = 305
        Me.Label72.Text = "年"
        '
        'Label67
        '
        Me.Label67.AutoSize = True
        Me.Label67.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label67.Location = New System.Drawing.Point(284, 275)
        Me.Label67.Name = "Label67"
        Me.Label67.Size = New System.Drawing.Size(21, 13)
        Me.Label67.TabIndex = 304
        Me.Label67.Text = "日"
        '
        'Label68
        '
        Me.Label68.AutoSize = True
        Me.Label68.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label68.Location = New System.Drawing.Point(228, 275)
        Me.Label68.Name = "Label68"
        Me.Label68.Size = New System.Drawing.Size(21, 13)
        Me.Label68.TabIndex = 303
        Me.Label68.Text = "月"
        '
        'Label69
        '
        Me.Label69.AutoSize = True
        Me.Label69.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label69.Location = New System.Drawing.Point(173, 275)
        Me.Label69.Name = "Label69"
        Me.Label69.Size = New System.Drawing.Size(21, 13)
        Me.Label69.TabIndex = 302
        Me.Label69.Text = "年"
        '
        'Label64
        '
        Me.Label64.AutoSize = True
        Me.Label64.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label64.Location = New System.Drawing.Point(284, 253)
        Me.Label64.Name = "Label64"
        Me.Label64.Size = New System.Drawing.Size(21, 13)
        Me.Label64.TabIndex = 298
        Me.Label64.Text = "日"
        '
        'Label65
        '
        Me.Label65.AutoSize = True
        Me.Label65.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label65.Location = New System.Drawing.Point(228, 253)
        Me.Label65.Name = "Label65"
        Me.Label65.Size = New System.Drawing.Size(21, 13)
        Me.Label65.TabIndex = 297
        Me.Label65.Text = "月"
        '
        'Label66
        '
        Me.Label66.AutoSize = True
        Me.Label66.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label66.Location = New System.Drawing.Point(173, 253)
        Me.Label66.Name = "Label66"
        Me.Label66.Size = New System.Drawing.Size(21, 13)
        Me.Label66.TabIndex = 296
        Me.Label66.Text = "年"
        '
        'Label61
        '
        Me.Label61.AutoSize = True
        Me.Label61.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label61.Location = New System.Drawing.Point(284, 231)
        Me.Label61.Name = "Label61"
        Me.Label61.Size = New System.Drawing.Size(21, 13)
        Me.Label61.TabIndex = 292
        Me.Label61.Text = "日"
        '
        'Label62
        '
        Me.Label62.AutoSize = True
        Me.Label62.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label62.Location = New System.Drawing.Point(228, 231)
        Me.Label62.Name = "Label62"
        Me.Label62.Size = New System.Drawing.Size(21, 13)
        Me.Label62.TabIndex = 291
        Me.Label62.Text = "月"
        '
        'Label63
        '
        Me.Label63.AutoSize = True
        Me.Label63.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label63.Location = New System.Drawing.Point(173, 231)
        Me.Label63.Name = "Label63"
        Me.Label63.Size = New System.Drawing.Size(21, 13)
        Me.Label63.TabIndex = 290
        Me.Label63.Text = "年"
        '
        'Label58
        '
        Me.Label58.AutoSize = True
        Me.Label58.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label58.Location = New System.Drawing.Point(284, 209)
        Me.Label58.Name = "Label58"
        Me.Label58.Size = New System.Drawing.Size(21, 13)
        Me.Label58.TabIndex = 286
        Me.Label58.Text = "日"
        '
        'Label59
        '
        Me.Label59.AutoSize = True
        Me.Label59.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label59.Location = New System.Drawing.Point(228, 209)
        Me.Label59.Name = "Label59"
        Me.Label59.Size = New System.Drawing.Size(21, 13)
        Me.Label59.TabIndex = 285
        Me.Label59.Text = "月"
        '
        'Label60
        '
        Me.Label60.AutoSize = True
        Me.Label60.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label60.Location = New System.Drawing.Point(173, 209)
        Me.Label60.Name = "Label60"
        Me.Label60.Size = New System.Drawing.Size(21, 13)
        Me.Label60.TabIndex = 284
        Me.Label60.Text = "年"
        '
        'Label55
        '
        Me.Label55.AutoSize = True
        Me.Label55.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label55.Location = New System.Drawing.Point(616, 165)
        Me.Label55.Name = "Label55"
        Me.Label55.Size = New System.Drawing.Size(21, 13)
        Me.Label55.TabIndex = 280
        Me.Label55.Text = "日"
        '
        'Label56
        '
        Me.Label56.AutoSize = True
        Me.Label56.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label56.Location = New System.Drawing.Point(555, 165)
        Me.Label56.Name = "Label56"
        Me.Label56.Size = New System.Drawing.Size(21, 13)
        Me.Label56.TabIndex = 279
        Me.Label56.Text = "月"
        '
        'Label57
        '
        Me.Label57.AutoSize = True
        Me.Label57.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label57.Location = New System.Drawing.Point(497, 165)
        Me.Label57.Name = "Label57"
        Me.Label57.Size = New System.Drawing.Size(21, 13)
        Me.Label57.TabIndex = 278
        Me.Label57.Text = "年"
        '
        'Label54
        '
        Me.Label54.AutoSize = True
        Me.Label54.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label54.Location = New System.Drawing.Point(284, 118)
        Me.Label54.Name = "Label54"
        Me.Label54.Size = New System.Drawing.Size(21, 13)
        Me.Label54.TabIndex = 272
        Me.Label54.Text = "日"
        '
        'Label53
        '
        Me.Label53.AutoSize = True
        Me.Label53.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label53.Location = New System.Drawing.Point(228, 118)
        Me.Label53.Name = "Label53"
        Me.Label53.Size = New System.Drawing.Size(21, 13)
        Me.Label53.TabIndex = 271
        Me.Label53.Text = "月"
        '
        'Label52
        '
        Me.Label52.AutoSize = True
        Me.Label52.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label52.Location = New System.Drawing.Point(173, 118)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(21, 13)
        Me.Label52.TabIndex = 269
        Me.Label52.Text = "年"
        '
        'Label51
        '
        Me.Label51.AutoSize = True
        Me.Label51.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label51.Location = New System.Drawing.Point(427, 95)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(21, 13)
        Me.Label51.TabIndex = 257
        Me.Label51.Text = "－"
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label50.Location = New System.Drawing.Point(227, 73)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(21, 13)
        Me.Label50.TabIndex = 256
        Me.Label50.Text = "－"
        '
        'Label47
        '
        Me.Label47.AutoSize = True
        Me.Label47.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label47.Location = New System.Drawing.Point(549, 496)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(63, 13)
        Me.Label47.TabIndex = 253
        Me.Label47.Text = "作成日付"
        '
        'Label45
        '
        Me.Label45.AutoSize = True
        Me.Label45.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label45.Location = New System.Drawing.Point(254, 496)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(105, 13)
        Me.Label45.TabIndex = 251
        Me.Label45.Text = "送信ファイル名"
        '
        'Label44
        '
        Me.Label44.AutoSize = True
        Me.Label44.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label44.Location = New System.Drawing.Point(139, 496)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(84, 13)
        Me.Label44.TabIndex = 250
        Me.Label44.Text = "ファイルSEQ"
        '
        'Label43
        '
        Me.Label43.AutoSize = True
        Me.Label43.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label43.Location = New System.Drawing.Point(28, 496)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(56, 13)
        Me.Label43.TabIndex = 249
        Me.Label43.Text = "持込SEQ"
        '
        'Label42
        '
        Me.Label42.AutoSize = True
        Me.Label42.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label42.Location = New System.Drawing.Point(654, 474)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(63, 13)
        Me.Label42.TabIndex = 248
        Me.Label42.Text = "持込区分"
        '
        'Label41
        '
        Me.Label41.AutoSize = True
        Me.Label41.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label41.Location = New System.Drawing.Point(549, 474)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(63, 13)
        Me.Label41.TabIndex = 247
        Me.Label41.Text = "送信区分"
        '
        'Label40
        '
        Me.Label40.AutoSize = True
        Me.Label40.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label40.Location = New System.Drawing.Point(27, 474)
        Me.Label40.Name = "Label40"
        Me.Label40.Size = New System.Drawing.Size(49, 13)
        Me.Label40.TabIndex = 246
        Me.Label40.Text = "手数料"
        '
        'Label39
        '
        Me.Label39.AutoSize = True
        Me.Label39.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label39.Location = New System.Drawing.Point(392, 474)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(105, 13)
        Me.Label39.TabIndex = 245
        Me.Label39.Text = "手数料計算区分"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label38.Location = New System.Drawing.Point(549, 451)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(63, 13)
        Me.Label38.TabIndex = 244
        Me.Label38.Text = "不能金額"
        '
        'Label37
        '
        Me.Label37.AutoSize = True
        Me.Label37.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label37.Location = New System.Drawing.Point(392, 451)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(63, 13)
        Me.Label37.TabIndex = 243
        Me.Label37.Text = "不能件数"
        '
        'Label36
        '
        Me.Label36.AutoSize = True
        Me.Label36.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label36.Location = New System.Drawing.Point(549, 429)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(77, 13)
        Me.Label36.TabIndex = 242
        Me.Label36.Text = "振替済金額"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label35.Location = New System.Drawing.Point(176, 429)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(63, 13)
        Me.Label35.TabIndex = 241
        Me.Label35.Text = "処理金額"
        '
        'Label34
        '
        Me.Label34.AutoSize = True
        Me.Label34.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label34.Location = New System.Drawing.Point(392, 429)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(77, 13)
        Me.Label34.TabIndex = 240
        Me.Label34.Text = "振替済件数"
        '
        'Label33
        '
        Me.Label33.AutoSize = True
        Me.Label33.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label33.Location = New System.Drawing.Point(27, 429)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(63, 13)
        Me.Label33.TabIndex = 239
        Me.Label33.Text = "処理件数"
        '
        '他行
        '
        Me.他行.AutoSize = True
        Me.他行.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.他行.Location = New System.Drawing.Point(676, 381)
        Me.他行.Name = "他行"
        Me.他行.Size = New System.Drawing.Size(35, 13)
        Me.他行.TabIndex = 238
        Me.他行.Text = "他行"
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label31.Location = New System.Drawing.Point(626, 381)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(35, 13)
        Me.Label31.TabIndex = 237
        Me.Label31.Text = "返還"
        '
        'Label30
        '
        Me.Label30.AutoSize = True
        Me.Label30.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label30.Location = New System.Drawing.Point(576, 381)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(35, 13)
        Me.Label30.TabIndex = 236
        Me.Label30.Text = "手徴"
        '
        'Label29
        '
        Me.Label29.AutoSize = True
        Me.Label29.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label29.Location = New System.Drawing.Point(527, 381)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(35, 13)
        Me.Label29.TabIndex = 235
        Me.Label29.Text = "決済"
        '
        'Label28
        '
        Me.Label28.AutoSize = True
        Me.Label28.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label28.Location = New System.Drawing.Point(377, 381)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(35, 13)
        Me.Label28.TabIndex = 234
        Me.Label28.Text = "不能"
        '
        'Label27
        '
        Me.Label27.AutoSize = True
        Me.Label27.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label27.Location = New System.Drawing.Point(327, 381)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(35, 13)
        Me.Label27.TabIndex = 233
        Me.Label27.Text = "再振"
        '
        'Label26
        '
        Me.Label26.AutoSize = True
        Me.Label26.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label26.Location = New System.Drawing.Point(277, 381)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(35, 13)
        Me.Label26.TabIndex = 232
        Me.Label26.Text = "送信"
        '
        'Label25
        '
        Me.Label25.AutoSize = True
        Me.Label25.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label25.Location = New System.Drawing.Point(477, 381)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(35, 13)
        Me.Label25.TabIndex = 231
        Me.Label25.Text = "手計"
        '
        'Label24
        '
        Me.Label24.AutoSize = True
        Me.Label24.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label24.Location = New System.Drawing.Point(427, 381)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(35, 13)
        Me.Label24.TabIndex = 230
        Me.Label24.Text = "日報"
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label23.Location = New System.Drawing.Point(228, 381)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(35, 13)
        Me.Label23.TabIndex = 229
        Me.Label23.Text = "配信"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label22.Location = New System.Drawing.Point(178, 381)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(35, 13)
        Me.Label22.TabIndex = 228
        Me.Label22.Text = "登録"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label21.Location = New System.Drawing.Point(127, 381)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(35, 13)
        Me.Label21.TabIndex = 227
        Me.Label21.Text = "受付"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label20.Location = New System.Drawing.Point(43, 381)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(63, 13)
        Me.Label20.TabIndex = 226
        Me.Label20.Text = "処理状況"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label14.Location = New System.Drawing.Point(337, 297)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(77, 13)
        Me.Label14.TabIndex = 225
        Me.Label14.Text = "資金決済日"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label15.Location = New System.Drawing.Point(337, 275)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(77, 13)
        Me.Label15.TabIndex = 224
        Me.Label15.Text = "返　還　日"
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label16.Location = New System.Drawing.Point(337, 253)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(77, 13)
        Me.Label16.TabIndex = 223
        Me.Label16.Text = "不　能　日"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label17.Location = New System.Drawing.Point(337, 231)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(77, 13)
        Me.Label17.TabIndex = 222
        Me.Label17.Text = "送　信　日"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label18.Location = New System.Drawing.Point(337, 209)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(77, 13)
        Me.Label18.TabIndex = 221
        Me.Label18.Text = "配　信　日"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label19.Location = New System.Drawing.Point(337, 187)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(77, 13)
        Me.Label19.TabIndex = 220
        Me.Label19.Text = "登　録　日"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label13.Location = New System.Drawing.Point(27, 297)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(105, 13)
        Me.Label13.TabIndex = 219
        Me.Label13.Text = "資金決済予定日"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label12.Location = New System.Drawing.Point(27, 275)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(77, 13)
        Me.Label12.TabIndex = 218
        Me.Label12.Text = "返還予定日"
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label11.Location = New System.Drawing.Point(27, 253)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(77, 13)
        Me.Label11.TabIndex = 217
        Me.Label11.Text = "不能予定日"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label10.Location = New System.Drawing.Point(27, 231)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(77, 13)
        Me.Label10.TabIndex = 216
        Me.Label10.Text = "送信予定日"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label9.Location = New System.Drawing.Point(27, 209)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(77, 13)
        Me.Label9.TabIndex = 215
        Me.Label9.Text = "配信予定日"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label8.Location = New System.Drawing.Point(337, 165)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(77, 13)
        Me.Label8.TabIndex = 214
        Me.Label8.Text = "受　付　日"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label7.Location = New System.Drawing.Point(510, 118)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(77, 13)
        Me.Label7.TabIndex = 213
        Me.Label7.Text = "媒体コード"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label6.Location = New System.Drawing.Point(510, 95)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(91, 13)
        Me.Label6.TabIndex = 212
        Me.Label6.Text = "委託者コード"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label1.Location = New System.Drawing.Point(309, 95)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(77, 13)
        Me.Label1.TabIndex = 211
        Me.Label1.Text = "振替－企業"
        '
        'Label94
        '
        Me.Label94.AutoSize = True
        Me.Label94.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label94.Location = New System.Drawing.Point(309, 73)
        Me.Label94.Name = "Label94"
        Me.Label94.Size = New System.Drawing.Size(63, 13)
        Me.Label94.TabIndex = 210
        Me.Label94.Text = "委託者名"
        '
        'Label95
        '
        Me.Label95.AutoSize = True
        Me.Label95.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label95.Location = New System.Drawing.Point(27, 118)
        Me.Label95.Name = "Label95"
        Me.Label95.Size = New System.Drawing.Size(82, 13)
        Me.Label95.TabIndex = 209
        Me.Label95.Text = "振　替　日"
        '
        'Label106
        '
        Me.Label106.AutoSize = True
        Me.Label106.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label106.Location = New System.Drawing.Point(27, 73)
        Me.Label106.Name = "Label106"
        Me.Label106.Size = New System.Drawing.Size(97, 13)
        Me.Label106.TabIndex = 208
        Me.Label106.Text = "取引先コード"
        '
        'Label107
        '
        Me.Label107.Font = New System.Drawing.Font("ＭＳ ゴシック", 14.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label107.Location = New System.Drawing.Point(0, 8)
        Me.Label107.Name = "Label107"
        Me.Label107.Size = New System.Drawing.Size(795, 30)
        Me.Label107.TabIndex = 207
        Me.Label107.Text = "＜スケジュールメンテナンス＞"
        Me.Label107.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbKana
        '
        Me.cmbKana.BackColor = System.Drawing.Color.White
        Me.cmbKana.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbKana.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.cmbKana.Items.AddRange(New Object() {"", "ｱ", "ｲ", "ｳ", "ｴ", "ｵ", "ｶ", "ｷ", "ｸ", "ｹ", "ｺ", "ｻ", "ｼ", "ｽ", "ｾ", "ｿ", "ﾀ", "ﾁ", "ﾂ", "ﾃ", "ﾄ", "ﾅ", "ﾆ", "ﾇ", "ﾈ", "ﾉ", "ﾊ", "ﾋ", "ﾌ", "ﾍ", "ﾎ", "ﾏ", "ﾐ", "ﾑ", "ﾒ", "ﾓ", "ﾔ", "ﾕ", "ﾖ", "ﾗ", "ﾘ", "ﾙ", "ﾚ", "ﾛ", "ﾜ", "ｦ", "ﾝ", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9"})
        Me.cmbKana.Location = New System.Drawing.Point(135, 46)
        Me.cmbKana.Name = "cmbKana"
        Me.cmbKana.Size = New System.Drawing.Size(48, 21)
        Me.cmbKana.TabIndex = 1
        Me.cmbKana.TabStop = False
        '
        'BAITAI_CODE_S
        '
        Me.BAITAI_CODE_S.BackColor = System.Drawing.Color.White
        Me.BAITAI_CODE_S.Enabled = False
        Me.BAITAI_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.BAITAI_CODE_S.Location = New System.Drawing.Point(603, 114)
        Me.BAITAI_CODE_S.MaxLength = 2
        Me.BAITAI_CODE_S.Name = "BAITAI_CODE_S"
        Me.BAITAI_CODE_S.Size = New System.Drawing.Size(32, 20)
        Me.BAITAI_CODE_S.TabIndex = 21
        '
        'ITAKU_CODE_S
        '
        Me.ITAKU_CODE_S.BackColor = System.Drawing.Color.White
        Me.ITAKU_CODE_S.Enabled = False
        Me.ITAKU_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ITAKU_CODE_S.Location = New System.Drawing.Point(603, 91)
        Me.ITAKU_CODE_S.MaxLength = 10
        Me.ITAKU_CODE_S.Name = "ITAKU_CODE_S"
        Me.ITAKU_CODE_S.Size = New System.Drawing.Size(89, 20)
        Me.ITAKU_CODE_S.TabIndex = 20
        '
        'CmdBack
        '
        Me.CmdBack.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdBack.Location = New System.Drawing.Point(660, 520)
        Me.CmdBack.Name = "CmdBack"
        Me.CmdBack.Size = New System.Drawing.Size(120, 40)
        Me.CmdBack.TabIndex = 383
        Me.CmdBack.Text = "終　了"
        '
        'CmdDelete
        '
        Me.CmdDelete.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdDelete.Location = New System.Drawing.Point(400, 520)
        Me.CmdDelete.Name = "CmdDelete"
        Me.CmdDelete.Size = New System.Drawing.Size(120, 40)
        Me.CmdDelete.TabIndex = 381
        Me.CmdDelete.Text = "削　除"
        '
        'CmdSelect
        '
        Me.CmdSelect.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdSelect.Location = New System.Drawing.Point(270, 520)
        Me.CmdSelect.Name = "CmdSelect"
        Me.CmdSelect.Size = New System.Drawing.Size(120, 40)
        Me.CmdSelect.TabIndex = 380
        Me.CmdSelect.Text = "参　照"
        '
        'CmdUpdate
        '
        Me.CmdUpdate.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdUpdate.Location = New System.Drawing.Point(140, 520)
        Me.CmdUpdate.Name = "CmdUpdate"
        Me.CmdUpdate.Size = New System.Drawing.Size(120, 40)
        Me.CmdUpdate.TabIndex = 379
        Me.CmdUpdate.Text = "更　新"
        '
        'CmdInsert
        '
        Me.CmdInsert.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdInsert.Location = New System.Drawing.Point(10, 520)
        Me.CmdInsert.Name = "CmdInsert"
        Me.CmdInsert.Size = New System.Drawing.Size(120, 40)
        Me.CmdInsert.TabIndex = 378
        Me.CmdInsert.Text = "登　録"
        '
        'CmdClear
        '
        Me.CmdClear.Font = New System.Drawing.Font("ＭＳ ゴシック", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.CmdClear.Location = New System.Drawing.Point(530, 520)
        Me.CmdClear.Name = "CmdClear"
        Me.CmdClear.Size = New System.Drawing.Size(120, 40)
        Me.CmdClear.TabIndex = 382
        Me.CmdClear.Text = "取　消"
        '
        'KESSAI_TIME_STAMP_S
        '
        Me.KESSAI_TIME_STAMP_S.BackColor = System.Drawing.Color.White
        Me.KESSAI_TIME_STAMP_S.Enabled = False
        Me.KESSAI_TIME_STAMP_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_TIME_STAMP_S.Location = New System.Drawing.Point(640, 293)
        Me.KESSAI_TIME_STAMP_S.MaxLength = 14
        Me.KESSAI_TIME_STAMP_S.Name = "KESSAI_TIME_STAMP_S"
        Me.KESSAI_TIME_STAMP_S.Size = New System.Drawing.Size(128, 20)
        Me.KESSAI_TIME_STAMP_S.TabIndex = 377
        '
        'JIFURI_TIME_STAMP_S
        '
        Me.JIFURI_TIME_STAMP_S.BackColor = System.Drawing.Color.White
        Me.JIFURI_TIME_STAMP_S.Enabled = False
        Me.JIFURI_TIME_STAMP_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.JIFURI_TIME_STAMP_S.Location = New System.Drawing.Point(640, 205)
        Me.JIFURI_TIME_STAMP_S.MaxLength = 14
        Me.JIFURI_TIME_STAMP_S.Name = "JIFURI_TIME_STAMP_S"
        Me.JIFURI_TIME_STAMP_S.Size = New System.Drawing.Size(128, 20)
        Me.JIFURI_TIME_STAMP_S.TabIndex = 40
        '
        'SAKUSEI_DATE_S2
        '
        Me.SAKUSEI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.SAKUSEI_DATE_S2.Enabled = False
        Me.SAKUSEI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAKUSEI_DATE_S2.Location = New System.Drawing.Point(725, 492)
        Me.SAKUSEI_DATE_S2.MaxLength = 2
        Me.SAKUSEI_DATE_S2.Name = "SAKUSEI_DATE_S2"
        Me.SAKUSEI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.SAKUSEI_DATE_S2.TabIndex = 372
        '
        'SAKUSEI_DATE_S1
        '
        Me.SAKUSEI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.SAKUSEI_DATE_S1.Enabled = False
        Me.SAKUSEI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAKUSEI_DATE_S1.Location = New System.Drawing.Point(673, 492)
        Me.SAKUSEI_DATE_S1.MaxLength = 2
        Me.SAKUSEI_DATE_S1.Name = "SAKUSEI_DATE_S1"
        Me.SAKUSEI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.SAKUSEI_DATE_S1.TabIndex = 371
        '
        'SAKUSEI_DATE_S
        '
        Me.SAKUSEI_DATE_S.BackColor = System.Drawing.Color.White
        Me.SAKUSEI_DATE_S.Enabled = False
        Me.SAKUSEI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAKUSEI_DATE_S.Location = New System.Drawing.Point(614, 492)
        Me.SAKUSEI_DATE_S.MaxLength = 4
        Me.SAKUSEI_DATE_S.Name = "SAKUSEI_DATE_S"
        Me.SAKUSEI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.SAKUSEI_DATE_S.TabIndex = 370
        '
        'MOTIKOMI_SEQ_S
        '
        Me.MOTIKOMI_SEQ_S.BackColor = System.Drawing.Color.White
        Me.MOTIKOMI_SEQ_S.Enabled = False
        Me.MOTIKOMI_SEQ_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MOTIKOMI_SEQ_S.Location = New System.Drawing.Point(106, 492)
        Me.MOTIKOMI_SEQ_S.MaxLength = 4
        Me.MOTIKOMI_SEQ_S.Name = "MOTIKOMI_SEQ_S"
        Me.MOTIKOMI_SEQ_S.Size = New System.Drawing.Size(27, 20)
        Me.MOTIKOMI_SEQ_S.TabIndex = 369
        '
        'SFILE_NAME_S
        '
        Me.SFILE_NAME_S.BackColor = System.Drawing.Color.White
        Me.SFILE_NAME_S.Enabled = False
        Me.SFILE_NAME_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SFILE_NAME_S.Location = New System.Drawing.Point(361, 492)
        Me.SFILE_NAME_S.MaxLength = 15
        Me.SFILE_NAME_S.Name = "SFILE_NAME_S"
        Me.SFILE_NAME_S.Size = New System.Drawing.Size(112, 20)
        Me.SFILE_NAME_S.TabIndex = 367
        '
        'FILE_SEQ_S
        '
        Me.FILE_SEQ_S.BackColor = System.Drawing.Color.White
        Me.FILE_SEQ_S.Enabled = False
        Me.FILE_SEQ_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FILE_SEQ_S.Location = New System.Drawing.Point(225, 492)
        Me.FILE_SEQ_S.MaxLength = 4
        Me.FILE_SEQ_S.Name = "FILE_SEQ_S"
        Me.FILE_SEQ_S.Size = New System.Drawing.Size(28, 20)
        Me.FILE_SEQ_S.TabIndex = 366
        '
        'MOTIKOMI_KBN_S
        '
        Me.MOTIKOMI_KBN_S.BackColor = System.Drawing.Color.White
        Me.MOTIKOMI_KBN_S.Enabled = False
        Me.MOTIKOMI_KBN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MOTIKOMI_KBN_S.Location = New System.Drawing.Point(719, 470)
        Me.MOTIKOMI_KBN_S.MaxLength = 1
        Me.MOTIKOMI_KBN_S.Name = "MOTIKOMI_KBN_S"
        Me.MOTIKOMI_KBN_S.Size = New System.Drawing.Size(24, 20)
        Me.MOTIKOMI_KBN_S.TabIndex = 365
        '
        'SOUSIN_KBN_S
        '
        Me.SOUSIN_KBN_S.BackColor = System.Drawing.Color.White
        Me.SOUSIN_KBN_S.Enabled = False
        Me.SOUSIN_KBN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_KBN_S.Location = New System.Drawing.Point(628, 470)
        Me.SOUSIN_KBN_S.MaxLength = 1
        Me.SOUSIN_KBN_S.Name = "SOUSIN_KBN_S"
        Me.SOUSIN_KBN_S.Size = New System.Drawing.Size(24, 20)
        Me.SOUSIN_KBN_S.TabIndex = 364
        '
        'TESUU_KIN_S
        '
        Me.TESUU_KIN_S.BackColor = System.Drawing.Color.White
        Me.TESUU_KIN_S.Enabled = False
        Me.TESUU_KIN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_KIN_S.Location = New System.Drawing.Point(106, 470)
        Me.TESUU_KIN_S.MaxLength = 15
        Me.TESUU_KIN_S.Name = "TESUU_KIN_S"
        Me.TESUU_KIN_S.Size = New System.Drawing.Size(130, 20)
        Me.TESUU_KIN_S.TabIndex = 363
        Me.TESUU_KIN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TESUU_KBN_S
        '
        Me.TESUU_KBN_S.BackColor = System.Drawing.Color.White
        Me.TESUU_KBN_S.Enabled = False
        Me.TESUU_KBN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_KBN_S.Location = New System.Drawing.Point(505, 470)
        Me.TESUU_KBN_S.MaxLength = 1
        Me.TESUU_KBN_S.Name = "TESUU_KBN_S"
        Me.TESUU_KBN_S.Size = New System.Drawing.Size(24, 20)
        Me.TESUU_KBN_S.TabIndex = 362
        '
        'FUNOU_KIN_S
        '
        Me.FUNOU_KIN_S.BackColor = System.Drawing.Color.White
        Me.FUNOU_KIN_S.Enabled = False
        Me.FUNOU_KIN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_KIN_S.Location = New System.Drawing.Point(628, 447)
        Me.FUNOU_KIN_S.MaxLength = 15
        Me.FUNOU_KIN_S.Name = "FUNOU_KIN_S"
        Me.FUNOU_KIN_S.Size = New System.Drawing.Size(130, 20)
        Me.FUNOU_KIN_S.TabIndex = 361
        Me.FUNOU_KIN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FURI_KIN_S
        '
        Me.FURI_KIN_S.BackColor = System.Drawing.Color.White
        Me.FURI_KIN_S.Enabled = False
        Me.FURI_KIN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_KIN_S.Location = New System.Drawing.Point(628, 425)
        Me.FURI_KIN_S.MaxLength = 15
        Me.FURI_KIN_S.Name = "FURI_KIN_S"
        Me.FURI_KIN_S.Size = New System.Drawing.Size(130, 20)
        Me.FURI_KIN_S.TabIndex = 360
        Me.FURI_KIN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'SYORI_KIN_S
        '
        Me.SYORI_KIN_S.BackColor = System.Drawing.Color.White
        Me.SYORI_KIN_S.Enabled = False
        Me.SYORI_KIN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SYORI_KIN_S.Location = New System.Drawing.Point(255, 425)
        Me.SYORI_KIN_S.MaxLength = 15
        Me.SYORI_KIN_S.Name = "SYORI_KIN_S"
        Me.SYORI_KIN_S.Size = New System.Drawing.Size(130, 20)
        Me.SYORI_KIN_S.TabIndex = 359
        Me.SYORI_KIN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FUNOU_KEN_S
        '
        Me.FUNOU_KEN_S.BackColor = System.Drawing.Color.White
        Me.FUNOU_KEN_S.Enabled = False
        Me.FUNOU_KEN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_KEN_S.Location = New System.Drawing.Point(471, 447)
        Me.FUNOU_KEN_S.MaxLength = 7
        Me.FUNOU_KEN_S.Name = "FUNOU_KEN_S"
        Me.FUNOU_KEN_S.Size = New System.Drawing.Size(58, 20)
        Me.FUNOU_KEN_S.TabIndex = 358
        Me.FUNOU_KEN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'FURI_KEN_S
        '
        Me.FURI_KEN_S.BackColor = System.Drawing.Color.White
        Me.FURI_KEN_S.Enabled = False
        Me.FURI_KEN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_KEN_S.Location = New System.Drawing.Point(471, 425)
        Me.FURI_KEN_S.MaxLength = 7
        Me.FURI_KEN_S.Name = "FURI_KEN_S"
        Me.FURI_KEN_S.Size = New System.Drawing.Size(58, 20)
        Me.FURI_KEN_S.TabIndex = 357
        Me.FURI_KEN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'SYORI_KEN_S
        '
        Me.SYORI_KEN_S.BackColor = System.Drawing.Color.White
        Me.SYORI_KEN_S.Enabled = False
        Me.SYORI_KEN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SYORI_KEN_S.Location = New System.Drawing.Point(106, 425)
        Me.SYORI_KEN_S.MaxLength = 7
        Me.SYORI_KEN_S.Name = "SYORI_KEN_S"
        Me.SYORI_KEN_S.Size = New System.Drawing.Size(58, 20)
        Me.SYORI_KEN_S.TabIndex = 356
        Me.SYORI_KEN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'TYUUDAN_FLG_S
        '
        Me.TYUUDAN_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TYUUDAN_FLG_S.Location = New System.Drawing.Point(730, 398)
        Me.TYUUDAN_FLG_S.MaxLength = 1
        Me.TYUUDAN_FLG_S.Name = "TYUUDAN_FLG_S"
        Me.TYUUDAN_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.TYUUDAN_FLG_S.TabIndex = 356
        '
        'HENKAN_FLG_S
        '
        Me.HENKAN_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_FLG_S.Location = New System.Drawing.Point(632, 398)
        Me.HENKAN_FLG_S.MaxLength = 1
        Me.HENKAN_FLG_S.Name = "HENKAN_FLG_S"
        Me.HENKAN_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.HENKAN_FLG_S.TabIndex = 354
        '
        'TESUUTYO_FLG_S
        '
        Me.TESUUTYO_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUUTYO_FLG_S.Location = New System.Drawing.Point(582, 398)
        Me.TESUUTYO_FLG_S.MaxLength = 1
        Me.TESUUTYO_FLG_S.Name = "TESUUTYO_FLG_S"
        Me.TESUUTYO_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.TESUUTYO_FLG_S.TabIndex = 353
        '
        'KESSAI_FLG_S
        '
        Me.KESSAI_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_FLG_S.Location = New System.Drawing.Point(532, 398)
        Me.KESSAI_FLG_S.MaxLength = 1
        Me.KESSAI_FLG_S.Name = "KESSAI_FLG_S"
        Me.KESSAI_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.KESSAI_FLG_S.TabIndex = 352
        '
        'TESUUKEI_FLG_S
        '
        Me.TESUUKEI_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUUKEI_FLG_S.Location = New System.Drawing.Point(482, 398)
        Me.TESUUKEI_FLG_S.MaxLength = 1
        Me.TESUUKEI_FLG_S.Name = "TESUUKEI_FLG_S"
        Me.TESUUKEI_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.TESUUKEI_FLG_S.TabIndex = 351
        '
        'NIPPO_FLG_S
        '
        Me.NIPPO_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.NIPPO_FLG_S.Location = New System.Drawing.Point(432, 398)
        Me.NIPPO_FLG_S.MaxLength = 1
        Me.NIPPO_FLG_S.Name = "NIPPO_FLG_S"
        Me.NIPPO_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.NIPPO_FLG_S.TabIndex = 350
        '
        'FUNOU_FLG_S
        '
        Me.FUNOU_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_FLG_S.Location = New System.Drawing.Point(382, 398)
        Me.FUNOU_FLG_S.MaxLength = 1
        Me.FUNOU_FLG_S.Name = "FUNOU_FLG_S"
        Me.FUNOU_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.FUNOU_FLG_S.TabIndex = 349
        '
        'SAIFURI_FLG_S
        '
        Me.SAIFURI_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SAIFURI_FLG_S.Location = New System.Drawing.Point(332, 398)
        Me.SAIFURI_FLG_S.MaxLength = 1
        Me.SAIFURI_FLG_S.Name = "SAIFURI_FLG_S"
        Me.SAIFURI_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.SAIFURI_FLG_S.TabIndex = 348
        '
        'SOUSIN_FLG_S
        '
        Me.SOUSIN_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_FLG_S.Location = New System.Drawing.Point(282, 398)
        Me.SOUSIN_FLG_S.MaxLength = 1
        Me.SOUSIN_FLG_S.Name = "SOUSIN_FLG_S"
        Me.SOUSIN_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.SOUSIN_FLG_S.TabIndex = 347
        '
        'HAISIN_FLG_S
        '
        Me.HAISIN_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_FLG_S.Location = New System.Drawing.Point(232, 398)
        Me.HAISIN_FLG_S.MaxLength = 1
        Me.HAISIN_FLG_S.Name = "HAISIN_FLG_S"
        Me.HAISIN_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.HAISIN_FLG_S.TabIndex = 346
        '
        'TOUROKU_FLG_S
        '
        Me.TOUROKU_FLG_S.Enabled = False
        Me.TOUROKU_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TOUROKU_FLG_S.ForeColor = System.Drawing.SystemColors.WindowText
        Me.TOUROKU_FLG_S.Location = New System.Drawing.Point(182, 398)
        Me.TOUROKU_FLG_S.MaxLength = 1
        Me.TOUROKU_FLG_S.Name = "TOUROKU_FLG_S"
        Me.TOUROKU_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.TOUROKU_FLG_S.TabIndex = 345
        '
        'UKETUKE_FLG_S
        '
        Me.UKETUKE_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.UKETUKE_FLG_S.Location = New System.Drawing.Point(132, 398)
        Me.UKETUKE_FLG_S.MaxLength = 1
        Me.UKETUKE_FLG_S.Name = "UKETUKE_FLG_S"
        Me.UKETUKE_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.UKETUKE_FLG_S.TabIndex = 344
        '
        'KESSAI_DATE_S2
        '
        Me.KESSAI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.KESSAI_DATE_S2.Enabled = False
        Me.KESSAI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_DATE_S2.Location = New System.Drawing.Point(584, 293)
        Me.KESSAI_DATE_S2.MaxLength = 2
        Me.KESSAI_DATE_S2.Name = "KESSAI_DATE_S2"
        Me.KESSAI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.KESSAI_DATE_S2.TabIndex = 340
        '
        'KESSAI_DATE_S1
        '
        Me.KESSAI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.KESSAI_DATE_S1.Enabled = False
        Me.KESSAI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_DATE_S1.Location = New System.Drawing.Point(523, 293)
        Me.KESSAI_DATE_S1.MaxLength = 2
        Me.KESSAI_DATE_S1.Name = "KESSAI_DATE_S1"
        Me.KESSAI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.KESSAI_DATE_S1.TabIndex = 339
        '
        'KESSAI_DATE_S
        '
        Me.KESSAI_DATE_S.BackColor = System.Drawing.Color.White
        Me.KESSAI_DATE_S.Enabled = False
        Me.KESSAI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_DATE_S.Location = New System.Drawing.Point(459, 293)
        Me.KESSAI_DATE_S.MaxLength = 4
        Me.KESSAI_DATE_S.Name = "KESSAI_DATE_S"
        Me.KESSAI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.KESSAI_DATE_S.TabIndex = 338
        '
        'HENKAN_DATE_S2
        '
        Me.HENKAN_DATE_S2.BackColor = System.Drawing.Color.White
        Me.HENKAN_DATE_S2.Enabled = False
        Me.HENKAN_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_DATE_S2.Location = New System.Drawing.Point(584, 271)
        Me.HENKAN_DATE_S2.MaxLength = 2
        Me.HENKAN_DATE_S2.Name = "HENKAN_DATE_S2"
        Me.HENKAN_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.HENKAN_DATE_S2.TabIndex = 334
        '
        'HENKAN_DATE_S1
        '
        Me.HENKAN_DATE_S1.BackColor = System.Drawing.Color.White
        Me.HENKAN_DATE_S1.Enabled = False
        Me.HENKAN_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_DATE_S1.Location = New System.Drawing.Point(523, 271)
        Me.HENKAN_DATE_S1.MaxLength = 2
        Me.HENKAN_DATE_S1.Name = "HENKAN_DATE_S1"
        Me.HENKAN_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.HENKAN_DATE_S1.TabIndex = 333
        '
        'HENKAN_DATE_S
        '
        Me.HENKAN_DATE_S.BackColor = System.Drawing.Color.White
        Me.HENKAN_DATE_S.Enabled = False
        Me.HENKAN_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_DATE_S.Location = New System.Drawing.Point(459, 271)
        Me.HENKAN_DATE_S.MaxLength = 4
        Me.HENKAN_DATE_S.Name = "HENKAN_DATE_S"
        Me.HENKAN_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.HENKAN_DATE_S.TabIndex = 332
        '
        'FUNOU_DATE_S2
        '
        Me.FUNOU_DATE_S2.BackColor = System.Drawing.Color.White
        Me.FUNOU_DATE_S2.Enabled = False
        Me.FUNOU_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_DATE_S2.Location = New System.Drawing.Point(584, 249)
        Me.FUNOU_DATE_S2.MaxLength = 2
        Me.FUNOU_DATE_S2.Name = "FUNOU_DATE_S2"
        Me.FUNOU_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.FUNOU_DATE_S2.TabIndex = 328
        '
        'FUNOU_DATE_S1
        '
        Me.FUNOU_DATE_S1.BackColor = System.Drawing.Color.White
        Me.FUNOU_DATE_S1.Enabled = False
        Me.FUNOU_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_DATE_S1.Location = New System.Drawing.Point(523, 249)
        Me.FUNOU_DATE_S1.MaxLength = 2
        Me.FUNOU_DATE_S1.Name = "FUNOU_DATE_S1"
        Me.FUNOU_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.FUNOU_DATE_S1.TabIndex = 327
        '
        'FUNOU_DATE_S
        '
        Me.FUNOU_DATE_S.BackColor = System.Drawing.Color.White
        Me.FUNOU_DATE_S.Enabled = False
        Me.FUNOU_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_DATE_S.Location = New System.Drawing.Point(459, 249)
        Me.FUNOU_DATE_S.MaxLength = 4
        Me.FUNOU_DATE_S.Name = "FUNOU_DATE_S"
        Me.FUNOU_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.FUNOU_DATE_S.TabIndex = 326
        '
        'SOUSIN_DATE_S2
        '
        Me.SOUSIN_DATE_S2.BackColor = System.Drawing.Color.White
        Me.SOUSIN_DATE_S2.Enabled = False
        Me.SOUSIN_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_DATE_S2.Location = New System.Drawing.Point(584, 227)
        Me.SOUSIN_DATE_S2.MaxLength = 2
        Me.SOUSIN_DATE_S2.Name = "SOUSIN_DATE_S2"
        Me.SOUSIN_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.SOUSIN_DATE_S2.TabIndex = 322
        '
        'SOUSIN_DATE_S1
        '
        Me.SOUSIN_DATE_S1.BackColor = System.Drawing.Color.White
        Me.SOUSIN_DATE_S1.Enabled = False
        Me.SOUSIN_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_DATE_S1.Location = New System.Drawing.Point(523, 227)
        Me.SOUSIN_DATE_S1.MaxLength = 2
        Me.SOUSIN_DATE_S1.Name = "SOUSIN_DATE_S1"
        Me.SOUSIN_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.SOUSIN_DATE_S1.TabIndex = 321
        '
        'SOUSIN_DATE_S
        '
        Me.SOUSIN_DATE_S.BackColor = System.Drawing.Color.White
        Me.SOUSIN_DATE_S.Enabled = False
        Me.SOUSIN_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_DATE_S.Location = New System.Drawing.Point(459, 227)
        Me.SOUSIN_DATE_S.MaxLength = 4
        Me.SOUSIN_DATE_S.Name = "SOUSIN_DATE_S"
        Me.SOUSIN_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.SOUSIN_DATE_S.TabIndex = 320
        '
        'HAISIN_DATE_S2
        '
        Me.HAISIN_DATE_S2.BackColor = System.Drawing.Color.White
        Me.HAISIN_DATE_S2.Enabled = False
        Me.HAISIN_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_DATE_S2.Location = New System.Drawing.Point(584, 205)
        Me.HAISIN_DATE_S2.MaxLength = 2
        Me.HAISIN_DATE_S2.Name = "HAISIN_DATE_S2"
        Me.HAISIN_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.HAISIN_DATE_S2.TabIndex = 39
        '
        'HAISIN_DATE_S1
        '
        Me.HAISIN_DATE_S1.BackColor = System.Drawing.Color.White
        Me.HAISIN_DATE_S1.Enabled = False
        Me.HAISIN_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_DATE_S1.Location = New System.Drawing.Point(523, 205)
        Me.HAISIN_DATE_S1.MaxLength = 2
        Me.HAISIN_DATE_S1.Name = "HAISIN_DATE_S1"
        Me.HAISIN_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.HAISIN_DATE_S1.TabIndex = 38
        '
        'HAISIN_DATE_S
        '
        Me.HAISIN_DATE_S.BackColor = System.Drawing.Color.White
        Me.HAISIN_DATE_S.Enabled = False
        Me.HAISIN_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_DATE_S.Location = New System.Drawing.Point(459, 205)
        Me.HAISIN_DATE_S.MaxLength = 4
        Me.HAISIN_DATE_S.Name = "HAISIN_DATE_S"
        Me.HAISIN_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.HAISIN_DATE_S.TabIndex = 37
        '
        'TOUROKU_DATE_S2
        '
        Me.TOUROKU_DATE_S2.BackColor = System.Drawing.Color.White
        Me.TOUROKU_DATE_S2.Enabled = False
        Me.TOUROKU_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TOUROKU_DATE_S2.Location = New System.Drawing.Point(584, 183)
        Me.TOUROKU_DATE_S2.MaxLength = 2
        Me.TOUROKU_DATE_S2.Name = "TOUROKU_DATE_S2"
        Me.TOUROKU_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.TOUROKU_DATE_S2.TabIndex = 33
        '
        'TOUROKU_DATE_S1
        '
        Me.TOUROKU_DATE_S1.BackColor = System.Drawing.Color.White
        Me.TOUROKU_DATE_S1.Enabled = False
        Me.TOUROKU_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TOUROKU_DATE_S1.Location = New System.Drawing.Point(523, 183)
        Me.TOUROKU_DATE_S1.MaxLength = 2
        Me.TOUROKU_DATE_S1.Name = "TOUROKU_DATE_S1"
        Me.TOUROKU_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.TOUROKU_DATE_S1.TabIndex = 32
        '
        'TOUROKU_DATE_S
        '
        Me.TOUROKU_DATE_S.BackColor = System.Drawing.Color.White
        Me.TOUROKU_DATE_S.Enabled = False
        Me.TOUROKU_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TOUROKU_DATE_S.Location = New System.Drawing.Point(459, 183)
        Me.TOUROKU_DATE_S.MaxLength = 4
        Me.TOUROKU_DATE_S.Name = "TOUROKU_DATE_S"
        Me.TOUROKU_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.TOUROKU_DATE_S.TabIndex = 31
        '
        'KESSAI_YDATE_S2
        '
        Me.KESSAI_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.KESSAI_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_YDATE_S2.Location = New System.Drawing.Point(252, 293)
        Me.KESSAI_YDATE_S2.MaxLength = 2
        Me.KESSAI_YDATE_S2.Name = "KESSAI_YDATE_S2"
        Me.KESSAI_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.KESSAI_YDATE_S2.TabIndex = 268
        '
        'KESSAI_YDATE_S1
        '
        Me.KESSAI_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.KESSAI_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_YDATE_S1.Location = New System.Drawing.Point(196, 293)
        Me.KESSAI_YDATE_S1.MaxLength = 2
        Me.KESSAI_YDATE_S1.Name = "KESSAI_YDATE_S1"
        Me.KESSAI_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.KESSAI_YDATE_S1.TabIndex = 267
        '
        'KESSAI_YDATE_S
        '
        Me.KESSAI_YDATE_S.BackColor = System.Drawing.Color.White
        Me.KESSAI_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KESSAI_YDATE_S.Location = New System.Drawing.Point(135, 293)
        Me.KESSAI_YDATE_S.MaxLength = 4
        Me.KESSAI_YDATE_S.Name = "KESSAI_YDATE_S"
        Me.KESSAI_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.KESSAI_YDATE_S.TabIndex = 265
        '
        'HENKAN_YDATE_S2
        '
        Me.HENKAN_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.HENKAN_YDATE_S2.Enabled = False
        Me.HENKAN_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_YDATE_S2.Location = New System.Drawing.Point(252, 271)
        Me.HENKAN_YDATE_S2.MaxLength = 2
        Me.HENKAN_YDATE_S2.Name = "HENKAN_YDATE_S2"
        Me.HENKAN_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.HENKAN_YDATE_S2.TabIndex = 301
        '
        'HENKAN_YDATE_S1
        '
        Me.HENKAN_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.HENKAN_YDATE_S1.Enabled = False
        Me.HENKAN_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_YDATE_S1.Location = New System.Drawing.Point(196, 271)
        Me.HENKAN_YDATE_S1.MaxLength = 2
        Me.HENKAN_YDATE_S1.Name = "HENKAN_YDATE_S1"
        Me.HENKAN_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.HENKAN_YDATE_S1.TabIndex = 300
        '
        'HENKAN_YDATE_S
        '
        Me.HENKAN_YDATE_S.BackColor = System.Drawing.Color.White
        Me.HENKAN_YDATE_S.Enabled = False
        Me.HENKAN_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HENKAN_YDATE_S.Location = New System.Drawing.Point(135, 271)
        Me.HENKAN_YDATE_S.MaxLength = 4
        Me.HENKAN_YDATE_S.Name = "HENKAN_YDATE_S"
        Me.HENKAN_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.HENKAN_YDATE_S.TabIndex = 299
        '
        'FUNOU_YDATE_S2
        '
        Me.FUNOU_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.FUNOU_YDATE_S2.Enabled = False
        Me.FUNOU_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_YDATE_S2.Location = New System.Drawing.Point(252, 249)
        Me.FUNOU_YDATE_S2.MaxLength = 2
        Me.FUNOU_YDATE_S2.Name = "FUNOU_YDATE_S2"
        Me.FUNOU_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.FUNOU_YDATE_S2.TabIndex = 295
        '
        'FUNOU_YDATE_S1
        '
        Me.FUNOU_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.FUNOU_YDATE_S1.Enabled = False
        Me.FUNOU_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_YDATE_S1.Location = New System.Drawing.Point(196, 249)
        Me.FUNOU_YDATE_S1.MaxLength = 2
        Me.FUNOU_YDATE_S1.Name = "FUNOU_YDATE_S1"
        Me.FUNOU_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.FUNOU_YDATE_S1.TabIndex = 294
        '
        'FUNOU_YDATE_S
        '
        Me.FUNOU_YDATE_S.BackColor = System.Drawing.Color.White
        Me.FUNOU_YDATE_S.Enabled = False
        Me.FUNOU_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FUNOU_YDATE_S.Location = New System.Drawing.Point(135, 249)
        Me.FUNOU_YDATE_S.MaxLength = 4
        Me.FUNOU_YDATE_S.Name = "FUNOU_YDATE_S"
        Me.FUNOU_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.FUNOU_YDATE_S.TabIndex = 293
        '
        'SOUSIN_YDATE_S2
        '
        Me.SOUSIN_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.SOUSIN_YDATE_S2.Enabled = False
        Me.SOUSIN_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_YDATE_S2.Location = New System.Drawing.Point(252, 227)
        Me.SOUSIN_YDATE_S2.MaxLength = 2
        Me.SOUSIN_YDATE_S2.Name = "SOUSIN_YDATE_S2"
        Me.SOUSIN_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.SOUSIN_YDATE_S2.TabIndex = 289
        '
        'SOUSIN_YDATE_S1
        '
        Me.SOUSIN_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.SOUSIN_YDATE_S1.Enabled = False
        Me.SOUSIN_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_YDATE_S1.Location = New System.Drawing.Point(196, 227)
        Me.SOUSIN_YDATE_S1.MaxLength = 2
        Me.SOUSIN_YDATE_S1.Name = "SOUSIN_YDATE_S1"
        Me.SOUSIN_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.SOUSIN_YDATE_S1.TabIndex = 288
        '
        'SOUSIN_YDATE_S
        '
        Me.SOUSIN_YDATE_S.BackColor = System.Drawing.Color.White
        Me.SOUSIN_YDATE_S.Enabled = False
        Me.SOUSIN_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.SOUSIN_YDATE_S.Location = New System.Drawing.Point(135, 227)
        Me.SOUSIN_YDATE_S.MaxLength = 4
        Me.SOUSIN_YDATE_S.Name = "SOUSIN_YDATE_S"
        Me.SOUSIN_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.SOUSIN_YDATE_S.TabIndex = 41
        '
        'HAISIN_YDATE_S2
        '
        Me.HAISIN_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.HAISIN_YDATE_S2.Enabled = False
        Me.HAISIN_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_YDATE_S2.Location = New System.Drawing.Point(252, 205)
        Me.HAISIN_YDATE_S2.MaxLength = 2
        Me.HAISIN_YDATE_S2.Name = "HAISIN_YDATE_S2"
        Me.HAISIN_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.HAISIN_YDATE_S2.TabIndex = 36
        '
        'HAISIN_YDATE_S1
        '
        Me.HAISIN_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.HAISIN_YDATE_S1.Enabled = False
        Me.HAISIN_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_YDATE_S1.Location = New System.Drawing.Point(196, 205)
        Me.HAISIN_YDATE_S1.MaxLength = 2
        Me.HAISIN_YDATE_S1.Name = "HAISIN_YDATE_S1"
        Me.HAISIN_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.HAISIN_YDATE_S1.TabIndex = 35
        '
        'HAISIN_YDATE_S
        '
        Me.HAISIN_YDATE_S.BackColor = System.Drawing.Color.White
        Me.HAISIN_YDATE_S.Enabled = False
        Me.HAISIN_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.HAISIN_YDATE_S.Location = New System.Drawing.Point(135, 205)
        Me.HAISIN_YDATE_S.MaxLength = 4
        Me.HAISIN_YDATE_S.Name = "HAISIN_YDATE_S"
        Me.HAISIN_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.HAISIN_YDATE_S.TabIndex = 34
        '
        'UKETUKE_DATE_S2
        '
        Me.UKETUKE_DATE_S2.BackColor = System.Drawing.Color.White
        Me.UKETUKE_DATE_S2.Enabled = False
        Me.UKETUKE_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.UKETUKE_DATE_S2.Location = New System.Drawing.Point(584, 161)
        Me.UKETUKE_DATE_S2.MaxLength = 2
        Me.UKETUKE_DATE_S2.Name = "UKETUKE_DATE_S2"
        Me.UKETUKE_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.UKETUKE_DATE_S2.TabIndex = 30
        '
        'UKETUKE_DATE_S1
        '
        Me.UKETUKE_DATE_S1.BackColor = System.Drawing.Color.White
        Me.UKETUKE_DATE_S1.Enabled = False
        Me.UKETUKE_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.UKETUKE_DATE_S1.Location = New System.Drawing.Point(523, 161)
        Me.UKETUKE_DATE_S1.MaxLength = 2
        Me.UKETUKE_DATE_S1.Name = "UKETUKE_DATE_S1"
        Me.UKETUKE_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.UKETUKE_DATE_S1.TabIndex = 29
        '
        'UKETUKE_DATE_S
        '
        Me.UKETUKE_DATE_S.BackColor = System.Drawing.Color.White
        Me.UKETUKE_DATE_S.Enabled = False
        Me.UKETUKE_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.UKETUKE_DATE_S.Location = New System.Drawing.Point(459, 161)
        Me.UKETUKE_DATE_S.MaxLength = 4
        Me.UKETUKE_DATE_S.Name = "UKETUKE_DATE_S"
        Me.UKETUKE_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.UKETUKE_DATE_S.TabIndex = 28
        '
        'FURI_CODE_S
        '
        Me.FURI_CODE_S.BackColor = System.Drawing.Color.White
        Me.FURI_CODE_S.Enabled = False
        Me.FURI_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_CODE_S.Location = New System.Drawing.Point(389, 91)
        Me.FURI_CODE_S.MaxLength = 3
        Me.FURI_CODE_S.Name = "FURI_CODE_S"
        Me.FURI_CODE_S.Size = New System.Drawing.Size(35, 20)
        Me.FURI_CODE_S.TabIndex = 15
        '
        'KIGYO_CODE_S
        '
        Me.KIGYO_CODE_S.BackColor = System.Drawing.Color.White
        Me.KIGYO_CODE_S.Enabled = False
        Me.KIGYO_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KIGYO_CODE_S.Location = New System.Drawing.Point(450, 91)
        Me.KIGYO_CODE_S.MaxLength = 5
        Me.KIGYO_CODE_S.Name = "KIGYO_CODE_S"
        Me.KIGYO_CODE_S.Size = New System.Drawing.Size(51, 20)
        Me.KIGYO_CODE_S.TabIndex = 16
        '
        'FURI_DATE_S2
        '
        Me.FURI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.FURI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_S2.Location = New System.Drawing.Point(252, 114)
        Me.FURI_DATE_S2.MaxLength = 2
        Me.FURI_DATE_S2.Name = "FURI_DATE_S2"
        Me.FURI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.FURI_DATE_S2.TabIndex = 11
        '
        'FURI_DATE_S1
        '
        Me.FURI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.FURI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_S1.Location = New System.Drawing.Point(196, 114)
        Me.FURI_DATE_S1.MaxLength = 2
        Me.FURI_DATE_S1.Name = "FURI_DATE_S1"
        Me.FURI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.FURI_DATE_S1.TabIndex = 10
        '
        'FURI_DATE_S
        '
        Me.FURI_DATE_S.BackColor = System.Drawing.Color.White
        Me.FURI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FURI_DATE_S.Location = New System.Drawing.Point(135, 114)
        Me.FURI_DATE_S.MaxLength = 4
        Me.FURI_DATE_S.Name = "FURI_DATE_S"
        Me.FURI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.FURI_DATE_S.TabIndex = 9
        '
        'ITAKU_NNAME_T
        '
        Me.ITAKU_NNAME_T.BackColor = System.Drawing.Color.White
        Me.ITAKU_NNAME_T.Enabled = False
        Me.ITAKU_NNAME_T.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ITAKU_NNAME_T.ImeMode = System.Windows.Forms.ImeMode.Hiragana
        Me.ITAKU_NNAME_T.Location = New System.Drawing.Point(389, 69)
        Me.ITAKU_NNAME_T.MaxLength = 25
        Me.ITAKU_NNAME_T.Name = "ITAKU_NNAME_T"
        Me.ITAKU_NNAME_T.Size = New System.Drawing.Size(379, 20)
        Me.ITAKU_NNAME_T.TabIndex = 5
        '
        'TORIF_CODE_S
        '
        Me.TORIF_CODE_S.BackColor = System.Drawing.Color.White
        Me.TORIF_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TORIF_CODE_S.Location = New System.Drawing.Point(252, 69)
        Me.TORIF_CODE_S.MaxLength = 2
        Me.TORIF_CODE_S.Name = "TORIF_CODE_S"
        Me.TORIF_CODE_S.Size = New System.Drawing.Size(28, 20)
        Me.TORIF_CODE_S.TabIndex = 4
        '
        'TORIS_CODE_S
        '
        Me.TORIS_CODE_S.BackColor = System.Drawing.Color.White
        Me.TORIS_CODE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TORIS_CODE_S.Location = New System.Drawing.Point(135, 69)
        Me.TORIS_CODE_S.MaxLength = 10
        Me.TORIS_CODE_S.Name = "TORIS_CODE_S"
        Me.TORIS_CODE_S.Size = New System.Drawing.Size(89, 20)
        Me.TORIS_CODE_S.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label4.Location = New System.Drawing.Point(284, 95)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(21, 13)
        Me.Label4.TabIndex = 405
        Me.Label4.Text = "日"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label5.Location = New System.Drawing.Point(228, 95)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(21, 13)
        Me.Label5.TabIndex = 404
        Me.Label5.Text = "月"
        '
        'Label105
        '
        Me.Label105.AutoSize = True
        Me.Label105.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label105.Location = New System.Drawing.Point(173, 95)
        Me.Label105.Name = "Label105"
        Me.Label105.Size = New System.Drawing.Size(21, 13)
        Me.Label105.TabIndex = 403
        Me.Label105.Text = "年"
        '
        'KFURI_DATE_S2
        '
        Me.KFURI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.KFURI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KFURI_DATE_S2.Location = New System.Drawing.Point(252, 91)
        Me.KFURI_DATE_S2.MaxLength = 2
        Me.KFURI_DATE_S2.Name = "KFURI_DATE_S2"
        Me.KFURI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.KFURI_DATE_S2.TabIndex = 8
        '
        'KFURI_DATE_S1
        '
        Me.KFURI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.KFURI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KFURI_DATE_S1.Location = New System.Drawing.Point(196, 91)
        Me.KFURI_DATE_S1.MaxLength = 2
        Me.KFURI_DATE_S1.Name = "KFURI_DATE_S1"
        Me.KFURI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.KFURI_DATE_S1.TabIndex = 7
        '
        'KFURI_DATE_S
        '
        Me.KFURI_DATE_S.BackColor = System.Drawing.Color.White
        Me.KFURI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.KFURI_DATE_S.Location = New System.Drawing.Point(135, 91)
        Me.KFURI_DATE_S.MaxLength = 4
        Me.KFURI_DATE_S.Name = "KFURI_DATE_S"
        Me.KFURI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.KFURI_DATE_S.TabIndex = 6
        '
        'Label111
        '
        Me.Label111.AutoSize = True
        Me.Label111.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label111.Location = New System.Drawing.Point(616, 319)
        Me.Label111.Name = "Label111"
        Me.Label111.Size = New System.Drawing.Size(21, 13)
        Me.Label111.TabIndex = 425
        Me.Label111.Text = "日"
        '
        'Label112
        '
        Me.Label112.AutoSize = True
        Me.Label112.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label112.Location = New System.Drawing.Point(555, 319)
        Me.Label112.Name = "Label112"
        Me.Label112.Size = New System.Drawing.Size(21, 13)
        Me.Label112.TabIndex = 424
        Me.Label112.Text = "月"
        '
        'Label113
        '
        Me.Label113.AutoSize = True
        Me.Label113.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label113.Location = New System.Drawing.Point(498, 319)
        Me.Label113.Name = "Label113"
        Me.Label113.Size = New System.Drawing.Size(21, 13)
        Me.Label113.TabIndex = 423
        Me.Label113.Text = "年"
        '
        'Label114
        '
        Me.Label114.AutoSize = True
        Me.Label114.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label114.Location = New System.Drawing.Point(284, 319)
        Me.Label114.Name = "Label114"
        Me.Label114.Size = New System.Drawing.Size(21, 13)
        Me.Label114.TabIndex = 419
        Me.Label114.Text = "日"
        '
        'Label115
        '
        Me.Label115.AutoSize = True
        Me.Label115.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label115.Location = New System.Drawing.Point(228, 319)
        Me.Label115.Name = "Label115"
        Me.Label115.Size = New System.Drawing.Size(21, 13)
        Me.Label115.TabIndex = 418
        Me.Label115.Text = "月"
        '
        'Label116
        '
        Me.Label116.AutoSize = True
        Me.Label116.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label116.Location = New System.Drawing.Point(173, 319)
        Me.Label116.Name = "Label116"
        Me.Label116.Size = New System.Drawing.Size(21, 13)
        Me.Label116.TabIndex = 417
        Me.Label116.Text = "年"
        '
        'Label117
        '
        Me.Label117.AutoSize = True
        Me.Label117.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label117.Location = New System.Drawing.Point(337, 319)
        Me.Label117.Name = "Label117"
        Me.Label117.Size = New System.Drawing.Size(77, 13)
        Me.Label117.TabIndex = 413
        Me.Label117.Text = "手数徴求日"
        '
        'Label118
        '
        Me.Label118.AutoSize = True
        Me.Label118.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label118.Location = New System.Drawing.Point(27, 319)
        Me.Label118.Name = "Label118"
        Me.Label118.Size = New System.Drawing.Size(105, 13)
        Me.Label118.TabIndex = 412
        Me.Label118.Text = "手数徴求予定日"
        '
        'TESUU_DATE_S2
        '
        Me.TESUU_DATE_S2.BackColor = System.Drawing.Color.White
        Me.TESUU_DATE_S2.Enabled = False
        Me.TESUU_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_DATE_S2.Location = New System.Drawing.Point(584, 315)
        Me.TESUU_DATE_S2.MaxLength = 4
        Me.TESUU_DATE_S2.Name = "TESUU_DATE_S2"
        Me.TESUU_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.TESUU_DATE_S2.TabIndex = 422
        '
        'TESUU_DATE_S1
        '
        Me.TESUU_DATE_S1.BackColor = System.Drawing.Color.White
        Me.TESUU_DATE_S1.Enabled = False
        Me.TESUU_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_DATE_S1.Location = New System.Drawing.Point(523, 315)
        Me.TESUU_DATE_S1.MaxLength = 4
        Me.TESUU_DATE_S1.Name = "TESUU_DATE_S1"
        Me.TESUU_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.TESUU_DATE_S1.TabIndex = 421
        '
        'TESUU_DATE_S
        '
        Me.TESUU_DATE_S.BackColor = System.Drawing.Color.White
        Me.TESUU_DATE_S.Enabled = False
        Me.TESUU_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_DATE_S.Location = New System.Drawing.Point(459, 315)
        Me.TESUU_DATE_S.MaxLength = 4
        Me.TESUU_DATE_S.Name = "TESUU_DATE_S"
        Me.TESUU_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.TESUU_DATE_S.TabIndex = 420
        '
        'TESUU_YDATE_S2
        '
        Me.TESUU_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.TESUU_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_YDATE_S2.Location = New System.Drawing.Point(252, 315)
        Me.TESUU_YDATE_S2.MaxLength = 2
        Me.TESUU_YDATE_S2.Name = "TESUU_YDATE_S2"
        Me.TESUU_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.TESUU_YDATE_S2.TabIndex = 271
        '
        'TESUU_YDATE_S1
        '
        Me.TESUU_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.TESUU_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_YDATE_S1.Location = New System.Drawing.Point(196, 315)
        Me.TESUU_YDATE_S1.MaxLength = 2
        Me.TESUU_YDATE_S1.Name = "TESUU_YDATE_S1"
        Me.TESUU_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.TESUU_YDATE_S1.TabIndex = 270
        '
        'TESUU_YDATE_S
        '
        Me.TESUU_YDATE_S.BackColor = System.Drawing.Color.White
        Me.TESUU_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_YDATE_S.Location = New System.Drawing.Point(135, 315)
        Me.TESUU_YDATE_S.MaxLength = 4
        Me.TESUU_YDATE_S.Name = "TESUU_YDATE_S"
        Me.TESUU_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.TESUU_YDATE_S.TabIndex = 269
        '
        'MEM_FLG
        '
        Me.MEM_FLG.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MEM_FLG.Location = New System.Drawing.Point(59, 398)
        Me.MEM_FLG.MaxLength = 1
        Me.MEM_FLG.Name = "MEM_FLG"
        Me.MEM_FLG.Size = New System.Drawing.Size(20, 20)
        Me.MEM_FLG.TabIndex = 426
        '
        'TESUU_TIME_STAMP_S
        '
        Me.TESUU_TIME_STAMP_S.BackColor = System.Drawing.Color.White
        Me.TESUU_TIME_STAMP_S.Enabled = False
        Me.TESUU_TIME_STAMP_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TESUU_TIME_STAMP_S.Location = New System.Drawing.Point(640, 315)
        Me.TESUU_TIME_STAMP_S.MaxLength = 14
        Me.TESUU_TIME_STAMP_S.Name = "TESUU_TIME_STAMP_S"
        Me.TESUU_TIME_STAMP_S.Size = New System.Drawing.Size(128, 20)
        Me.TESUU_TIME_STAMP_S.TabIndex = 428
        '
        'Label119
        '
        Me.Label119.AutoSize = True
        Me.Label119.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label119.Location = New System.Drawing.Point(616, 141)
        Me.Label119.Name = "Label119"
        Me.Label119.Size = New System.Drawing.Size(21, 13)
        Me.Label119.TabIndex = 442
        Me.Label119.Text = "日"
        '
        'Label120
        '
        Me.Label120.AutoSize = True
        Me.Label120.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label120.Location = New System.Drawing.Point(555, 142)
        Me.Label120.Name = "Label120"
        Me.Label120.Size = New System.Drawing.Size(21, 13)
        Me.Label120.TabIndex = 441
        Me.Label120.Text = "月"
        '
        'Label121
        '
        Me.Label121.AutoSize = True
        Me.Label121.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label121.Location = New System.Drawing.Point(498, 142)
        Me.Label121.Name = "Label121"
        Me.Label121.Size = New System.Drawing.Size(21, 13)
        Me.Label121.TabIndex = 440
        Me.Label121.Text = "年"
        '
        'Label122
        '
        Me.Label122.AutoSize = True
        Me.Label122.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label122.Location = New System.Drawing.Point(284, 142)
        Me.Label122.Name = "Label122"
        Me.Label122.Size = New System.Drawing.Size(21, 13)
        Me.Label122.TabIndex = 436
        Me.Label122.Text = "日"
        '
        'Label123
        '
        Me.Label123.AutoSize = True
        Me.Label123.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label123.Location = New System.Drawing.Point(228, 142)
        Me.Label123.Name = "Label123"
        Me.Label123.Size = New System.Drawing.Size(21, 13)
        Me.Label123.TabIndex = 435
        Me.Label123.Text = "月"
        '
        'Label124
        '
        Me.Label124.AutoSize = True
        Me.Label124.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label124.Location = New System.Drawing.Point(173, 142)
        Me.Label124.Name = "Label124"
        Me.Label124.Size = New System.Drawing.Size(21, 13)
        Me.Label124.TabIndex = 434
        Me.Label124.Text = "年"
        '
        'Label125
        '
        Me.Label125.AutoSize = True
        Me.Label125.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label125.Location = New System.Drawing.Point(337, 141)
        Me.Label125.Name = "Label125"
        Me.Label125.Size = New System.Drawing.Size(119, 13)
        Me.Label125.TabIndex = 430
        Me.Label125.Text = "依頼書回収予定日"
        '
        'Label126
        '
        Me.Label126.AutoSize = True
        Me.Label126.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label126.Location = New System.Drawing.Point(27, 142)
        Me.Label126.Name = "Label126"
        Me.Label126.Size = New System.Drawing.Size(91, 13)
        Me.Label126.TabIndex = 429
        Me.Label126.Text = "依頼書作成日"
        '
        'IRAISYOK_YDATE_S2
        '
        Me.IRAISYOK_YDATE_S2.BackColor = System.Drawing.Color.White
        Me.IRAISYOK_YDATE_S2.Enabled = False
        Me.IRAISYOK_YDATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYOK_YDATE_S2.Location = New System.Drawing.Point(584, 138)
        Me.IRAISYOK_YDATE_S2.MaxLength = 2
        Me.IRAISYOK_YDATE_S2.Name = "IRAISYOK_YDATE_S2"
        Me.IRAISYOK_YDATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.IRAISYOK_YDATE_S2.TabIndex = 27
        '
        'IRAISYOK_YDATE_S1
        '
        Me.IRAISYOK_YDATE_S1.BackColor = System.Drawing.Color.White
        Me.IRAISYOK_YDATE_S1.Enabled = False
        Me.IRAISYOK_YDATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYOK_YDATE_S1.Location = New System.Drawing.Point(523, 138)
        Me.IRAISYOK_YDATE_S1.MaxLength = 2
        Me.IRAISYOK_YDATE_S1.Name = "IRAISYOK_YDATE_S1"
        Me.IRAISYOK_YDATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.IRAISYOK_YDATE_S1.TabIndex = 26
        '
        'IRAISYOK_YDATE_S
        '
        Me.IRAISYOK_YDATE_S.BackColor = System.Drawing.Color.White
        Me.IRAISYOK_YDATE_S.Enabled = False
        Me.IRAISYOK_YDATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYOK_YDATE_S.Location = New System.Drawing.Point(459, 138)
        Me.IRAISYOK_YDATE_S.MaxLength = 4
        Me.IRAISYOK_YDATE_S.Name = "IRAISYOK_YDATE_S"
        Me.IRAISYOK_YDATE_S.Size = New System.Drawing.Size(35, 20)
        Me.IRAISYOK_YDATE_S.TabIndex = 25
        '
        'IRAISYO_DATE_S2
        '
        Me.IRAISYO_DATE_S2.BackColor = System.Drawing.Color.White
        Me.IRAISYO_DATE_S2.Enabled = False
        Me.IRAISYO_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYO_DATE_S2.Location = New System.Drawing.Point(252, 138)
        Me.IRAISYO_DATE_S2.MaxLength = 2
        Me.IRAISYO_DATE_S2.Name = "IRAISYO_DATE_S2"
        Me.IRAISYO_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.IRAISYO_DATE_S2.TabIndex = 24
        '
        'IRAISYO_DATE_S1
        '
        Me.IRAISYO_DATE_S1.BackColor = System.Drawing.Color.White
        Me.IRAISYO_DATE_S1.Enabled = False
        Me.IRAISYO_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYO_DATE_S1.Location = New System.Drawing.Point(196, 138)
        Me.IRAISYO_DATE_S1.MaxLength = 2
        Me.IRAISYO_DATE_S1.Name = "IRAISYO_DATE_S1"
        Me.IRAISYO_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.IRAISYO_DATE_S1.TabIndex = 23
        '
        'IRAISYO_DATE_S
        '
        Me.IRAISYO_DATE_S.BackColor = System.Drawing.Color.White
        Me.IRAISYO_DATE_S.Enabled = False
        Me.IRAISYO_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.IRAISYO_DATE_S.Location = New System.Drawing.Point(135, 138)
        Me.IRAISYO_DATE_S.MaxLength = 4
        Me.IRAISYO_DATE_S.Name = "IRAISYO_DATE_S"
        Me.IRAISYO_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.IRAISYO_DATE_S.TabIndex = 22
        '
        'Label131
        '
        Me.Label131.AutoSize = True
        Me.Label131.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label131.Location = New System.Drawing.Point(27, 95)
        Me.Label131.Name = "Label131"
        Me.Label131.Size = New System.Drawing.Size(77, 13)
        Me.Label131.TabIndex = 456
        Me.Label131.Text = "契約振替日"
        '
        'Label48
        '
        Me.Label48.AutoSize = True
        Me.Label48.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label48.Location = New System.Drawing.Point(726, 381)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(35, 13)
        Me.Label48.TabIndex = 458
        Me.Label48.Text = "中断"
        '
        'TAKOU_FLG_S
        '
        Me.TAKOU_FLG_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.TAKOU_FLG_S.Location = New System.Drawing.Point(682, 398)
        Me.TAKOU_FLG_S.MaxLength = 1
        Me.TAKOU_FLG_S.Name = "TAKOU_FLG_S"
        Me.TAKOU_FLG_S.Size = New System.Drawing.Size(20, 20)
        Me.TAKOU_FLG_S.TabIndex = 355
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label32.Location = New System.Drawing.Point(176, 452)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(77, 13)
        Me.Label32.TabIndex = 461
        Me.Label32.Text = "エラー金額"
        '
        'Label49
        '
        Me.Label49.AutoSize = True
        Me.Label49.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label49.Location = New System.Drawing.Point(27, 452)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(77, 13)
        Me.Label49.TabIndex = 460
        Me.Label49.Text = "エラー件数"
        '
        'ERR_KIN_S
        '
        Me.ERR_KIN_S.BackColor = System.Drawing.Color.White
        Me.ERR_KIN_S.Enabled = False
        Me.ERR_KIN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ERR_KIN_S.Location = New System.Drawing.Point(255, 448)
        Me.ERR_KIN_S.MaxLength = 15
        Me.ERR_KIN_S.Name = "ERR_KIN_S"
        Me.ERR_KIN_S.Size = New System.Drawing.Size(130, 20)
        Me.ERR_KIN_S.TabIndex = 463
        Me.ERR_KIN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'ERR_KEN_S
        '
        Me.ERR_KEN_S.BackColor = System.Drawing.Color.White
        Me.ERR_KEN_S.Enabled = False
        Me.ERR_KEN_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.ERR_KEN_S.Location = New System.Drawing.Point(106, 448)
        Me.ERR_KEN_S.MaxLength = 7
        Me.ERR_KEN_S.Name = "ERR_KEN_S"
        Me.ERR_KEN_S.Size = New System.Drawing.Size(58, 20)
        Me.ERR_KEN_S.TabIndex = 462
        Me.ERR_KEN_S.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        '
        'Label127
        '
        Me.Label127.AutoSize = True
        Me.Label127.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label127.Location = New System.Drawing.Point(284, 165)
        Me.Label127.Name = "Label127"
        Me.Label127.Size = New System.Drawing.Size(21, 13)
        Me.Label127.TabIndex = 470
        Me.Label127.Text = "日"
        '
        'Label128
        '
        Me.Label128.AutoSize = True
        Me.Label128.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label128.Location = New System.Drawing.Point(228, 165)
        Me.Label128.Name = "Label128"
        Me.Label128.Size = New System.Drawing.Size(21, 13)
        Me.Label128.TabIndex = 469
        Me.Label128.Text = "月"
        '
        'Label129
        '
        Me.Label129.AutoSize = True
        Me.Label129.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label129.Location = New System.Drawing.Point(173, 165)
        Me.Label129.Name = "Label129"
        Me.Label129.Size = New System.Drawing.Size(21, 13)
        Me.Label129.TabIndex = 468
        Me.Label129.Text = "年"
        '
        'MOTIKOMI_DATE_S2
        '
        Me.MOTIKOMI_DATE_S2.BackColor = System.Drawing.Color.White
        Me.MOTIKOMI_DATE_S2.Enabled = False
        Me.MOTIKOMI_DATE_S2.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MOTIKOMI_DATE_S2.Location = New System.Drawing.Point(252, 161)
        Me.MOTIKOMI_DATE_S2.MaxLength = 2
        Me.MOTIKOMI_DATE_S2.Name = "MOTIKOMI_DATE_S2"
        Me.MOTIKOMI_DATE_S2.Size = New System.Drawing.Size(28, 20)
        Me.MOTIKOMI_DATE_S2.TabIndex = 19
        '
        'MOTIKOMI_DATE_S1
        '
        Me.MOTIKOMI_DATE_S1.BackColor = System.Drawing.Color.White
        Me.MOTIKOMI_DATE_S1.Enabled = False
        Me.MOTIKOMI_DATE_S1.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MOTIKOMI_DATE_S1.Location = New System.Drawing.Point(196, 161)
        Me.MOTIKOMI_DATE_S1.MaxLength = 2
        Me.MOTIKOMI_DATE_S1.Name = "MOTIKOMI_DATE_S1"
        Me.MOTIKOMI_DATE_S1.Size = New System.Drawing.Size(28, 20)
        Me.MOTIKOMI_DATE_S1.TabIndex = 18
        '
        'MOTIKOMI_DATE_S
        '
        Me.MOTIKOMI_DATE_S.BackColor = System.Drawing.Color.White
        Me.MOTIKOMI_DATE_S.Enabled = False
        Me.MOTIKOMI_DATE_S.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.MOTIKOMI_DATE_S.Location = New System.Drawing.Point(135, 161)
        Me.MOTIKOMI_DATE_S.MaxLength = 4
        Me.MOTIKOMI_DATE_S.Name = "MOTIKOMI_DATE_S"
        Me.MOTIKOMI_DATE_S.Size = New System.Drawing.Size(35, 20)
        Me.MOTIKOMI_DATE_S.TabIndex = 17
        '
        'Label130
        '
        Me.Label130.AutoSize = True
        Me.Label130.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label130.Location = New System.Drawing.Point(27, 165)
        Me.Label130.Name = "Label130"
        Me.Label130.Size = New System.Drawing.Size(63, 13)
        Me.Label130.TabIndex = 471
        Me.Label130.Text = "持込期日"
        '
        'txtBeforeSaifuri
        '
        Me.txtBeforeSaifuri.Enabled = False
        Me.txtBeforeSaifuri.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.txtBeforeSaifuri.Location = New System.Drawing.Point(20, 359)
        Me.txtBeforeSaifuri.Name = "txtBeforeSaifuri"
        Me.txtBeforeSaifuri.Size = New System.Drawing.Size(100, 20)
        Me.txtBeforeSaifuri.TabIndex = 472
        Me.txtBeforeSaifuri.TabStop = False
        Me.txtBeforeSaifuri.Visible = False
        '
        'btnFuriDate
        '
        Me.btnFuriDate.Font = New System.Drawing.Font("ＭＳ Ｐゴシック", 9.75!)
        Me.btnFuriDate.Location = New System.Drawing.Point(306, 112)
        Me.btnFuriDate.Name = "btnFuriDate"
        Me.btnFuriDate.Size = New System.Drawing.Size(80, 25)
        Me.btnFuriDate.TabIndex = 380
        Me.btnFuriDate.TabStop = False
        Me.btnFuriDate.Text = "振替日変更"
        '
        'KFJMAST030
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.btnFuriDate)
        Me.Controls.Add(Me.txtBeforeSaifuri)
        Me.Controls.Add(Me.Label130)
        Me.Controls.Add(Me.Label127)
        Me.Controls.Add(Me.Label128)
        Me.Controls.Add(Me.Label129)
        Me.Controls.Add(Me.MOTIKOMI_DATE_S2)
        Me.Controls.Add(Me.MOTIKOMI_DATE_S1)
        Me.Controls.Add(Me.MOTIKOMI_DATE_S)
        Me.Controls.Add(Me.Label32)
        Me.Controls.Add(Me.Label49)
        Me.Controls.Add(Me.ERR_KIN_S)
        Me.Controls.Add(Me.ERR_KEN_S)
        Me.Controls.Add(Me.Label48)
        Me.Controls.Add(Me.TAKOU_FLG_S)
        Me.Controls.Add(Me.Label131)
        Me.Controls.Add(Me.Label119)
        Me.Controls.Add(Me.Label120)
        Me.Controls.Add(Me.Label121)
        Me.Controls.Add(Me.Label122)
        Me.Controls.Add(Me.Label123)
        Me.Controls.Add(Me.Label124)
        Me.Controls.Add(Me.Label125)
        Me.Controls.Add(Me.Label126)
        Me.Controls.Add(Me.IRAISYOK_YDATE_S2)
        Me.Controls.Add(Me.IRAISYOK_YDATE_S1)
        Me.Controls.Add(Me.IRAISYOK_YDATE_S)
        Me.Controls.Add(Me.IRAISYO_DATE_S2)
        Me.Controls.Add(Me.IRAISYO_DATE_S1)
        Me.Controls.Add(Me.IRAISYO_DATE_S)
        Me.Controls.Add(Me.TESUU_TIME_STAMP_S)
        Me.Controls.Add(Me.MEM_FLG)
        Me.Controls.Add(Me.Label111)
        Me.Controls.Add(Me.Label112)
        Me.Controls.Add(Me.Label113)
        Me.Controls.Add(Me.Label114)
        Me.Controls.Add(Me.Label115)
        Me.Controls.Add(Me.Label116)
        Me.Controls.Add(Me.Label117)
        Me.Controls.Add(Me.Label118)
        Me.Controls.Add(Me.TESUU_DATE_S2)
        Me.Controls.Add(Me.TESUU_DATE_S1)
        Me.Controls.Add(Me.TESUU_DATE_S)
        Me.Controls.Add(Me.TESUU_YDATE_S2)
        Me.Controls.Add(Me.TESUU_YDATE_S1)
        Me.Controls.Add(Me.TESUU_YDATE_S)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label105)
        Me.Controls.Add(Me.KFURI_DATE_S2)
        Me.Controls.Add(Me.KFURI_DATE_S1)
        Me.Controls.Add(Me.KFURI_DATE_S)
        Me.Controls.Add(Me.Label104)
        Me.Controls.Add(Me.Label103)
        Me.Controls.Add(Me.Label102)
        Me.Controls.Add(Me.Label101)
        Me.Controls.Add(Me.Label100)
        Me.Controls.Add(Me.Label99)
        Me.Controls.Add(Me.Label98)
        Me.Controls.Add(Me.Label97)
        Me.Controls.Add(Me.SAIFURI_DATE_S2)
        Me.Controls.Add(Me.SAIFURI_DATE_S1)
        Me.Controls.Add(Me.SAIFURI_DATE_S)
        Me.Controls.Add(Me.KSAIFURI_DATE_S2)
        Me.Controls.Add(Me.KSAIFURI_DATE_S1)
        Me.Controls.Add(Me.KSAIFURI_DATE_S)
        Me.Controls.Add(Me.cmbToriName)
        Me.Controls.Add(Me.Label96)
        Me.Controls.Add(Me.Label91)
        Me.Controls.Add(Me.Label92)
        Me.Controls.Add(Me.Label93)
        Me.Controls.Add(Me.Label88)
        Me.Controls.Add(Me.Label89)
        Me.Controls.Add(Me.Label90)
        Me.Controls.Add(Me.Label85)
        Me.Controls.Add(Me.Label86)
        Me.Controls.Add(Me.Label87)
        Me.Controls.Add(Me.Label82)
        Me.Controls.Add(Me.Label83)
        Me.Controls.Add(Me.Label84)
        Me.Controls.Add(Me.Label79)
        Me.Controls.Add(Me.Label80)
        Me.Controls.Add(Me.Label81)
        Me.Controls.Add(Me.Label76)
        Me.Controls.Add(Me.Label77)
        Me.Controls.Add(Me.Label78)
        Me.Controls.Add(Me.Label73)
        Me.Controls.Add(Me.Label74)
        Me.Controls.Add(Me.Label75)
        Me.Controls.Add(Me.Label70)
        Me.Controls.Add(Me.Label71)
        Me.Controls.Add(Me.Label72)
        Me.Controls.Add(Me.Label67)
        Me.Controls.Add(Me.Label68)
        Me.Controls.Add(Me.Label69)
        Me.Controls.Add(Me.Label64)
        Me.Controls.Add(Me.Label65)
        Me.Controls.Add(Me.Label66)
        Me.Controls.Add(Me.Label61)
        Me.Controls.Add(Me.Label62)
        Me.Controls.Add(Me.Label63)
        Me.Controls.Add(Me.Label58)
        Me.Controls.Add(Me.Label59)
        Me.Controls.Add(Me.Label60)
        Me.Controls.Add(Me.Label55)
        Me.Controls.Add(Me.Label56)
        Me.Controls.Add(Me.Label57)
        Me.Controls.Add(Me.Label54)
        Me.Controls.Add(Me.Label53)
        Me.Controls.Add(Me.Label52)
        Me.Controls.Add(Me.Label51)
        Me.Controls.Add(Me.Label50)
        Me.Controls.Add(Me.Label47)
        Me.Controls.Add(Me.Label42)
        Me.Controls.Add(Me.Label41)
        Me.Controls.Add(Me.Label40)
        Me.Controls.Add(Me.Label39)
        Me.Controls.Add(Me.Label38)
        Me.Controls.Add(Me.Label37)
        Me.Controls.Add(Me.Label36)
        Me.Controls.Add(Me.Label35)
        Me.Controls.Add(Me.Label34)
        Me.Controls.Add(Me.Label33)
        Me.Controls.Add(Me.他行)
        Me.Controls.Add(Me.Label31)
        Me.Controls.Add(Me.Label30)
        Me.Controls.Add(Me.Label29)
        Me.Controls.Add(Me.Label28)
        Me.Controls.Add(Me.Label27)
        Me.Controls.Add(Me.Label26)
        Me.Controls.Add(Me.Label25)
        Me.Controls.Add(Me.Label24)
        Me.Controls.Add(Me.Label23)
        Me.Controls.Add(Me.Label22)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.Label20)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.Label16)
        Me.Controls.Add(Me.Label17)
        Me.Controls.Add(Me.Label18)
        Me.Controls.Add(Me.Label19)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Label94)
        Me.Controls.Add(Me.Label95)
        Me.Controls.Add(Me.Label106)
        Me.Controls.Add(Me.cmbKana)
        Me.Controls.Add(Me.BAITAI_CODE_S)
        Me.Controls.Add(Me.ITAKU_CODE_S)
        Me.Controls.Add(Me.CmdBack)
        Me.Controls.Add(Me.CmdDelete)
        Me.Controls.Add(Me.CmdSelect)
        Me.Controls.Add(Me.CmdUpdate)
        Me.Controls.Add(Me.CmdInsert)
        Me.Controls.Add(Me.CmdClear)
        Me.Controls.Add(Me.KESSAI_TIME_STAMP_S)
        Me.Controls.Add(Me.JIFURI_TIME_STAMP_S)
        Me.Controls.Add(Me.SAKUSEI_DATE_S2)
        Me.Controls.Add(Me.SAKUSEI_DATE_S1)
        Me.Controls.Add(Me.SAKUSEI_DATE_S)
        Me.Controls.Add(Me.MOTIKOMI_SEQ_S)
        Me.Controls.Add(Me.SFILE_NAME_S)
        Me.Controls.Add(Me.FILE_SEQ_S)
        Me.Controls.Add(Me.MOTIKOMI_KBN_S)
        Me.Controls.Add(Me.SOUSIN_KBN_S)
        Me.Controls.Add(Me.TESUU_KIN_S)
        Me.Controls.Add(Me.TESUU_KBN_S)
        Me.Controls.Add(Me.FUNOU_KIN_S)
        Me.Controls.Add(Me.FURI_KIN_S)
        Me.Controls.Add(Me.SYORI_KIN_S)
        Me.Controls.Add(Me.FUNOU_KEN_S)
        Me.Controls.Add(Me.FURI_KEN_S)
        Me.Controls.Add(Me.SYORI_KEN_S)
        Me.Controls.Add(Me.TYUUDAN_FLG_S)
        Me.Controls.Add(Me.HENKAN_FLG_S)
        Me.Controls.Add(Me.TESUUTYO_FLG_S)
        Me.Controls.Add(Me.KESSAI_FLG_S)
        Me.Controls.Add(Me.TESUUKEI_FLG_S)
        Me.Controls.Add(Me.NIPPO_FLG_S)
        Me.Controls.Add(Me.FUNOU_FLG_S)
        Me.Controls.Add(Me.SAIFURI_FLG_S)
        Me.Controls.Add(Me.SOUSIN_FLG_S)
        Me.Controls.Add(Me.HAISIN_FLG_S)
        Me.Controls.Add(Me.TOUROKU_FLG_S)
        Me.Controls.Add(Me.UKETUKE_FLG_S)
        Me.Controls.Add(Me.KESSAI_DATE_S2)
        Me.Controls.Add(Me.KESSAI_DATE_S1)
        Me.Controls.Add(Me.KESSAI_DATE_S)
        Me.Controls.Add(Me.HENKAN_DATE_S2)
        Me.Controls.Add(Me.HENKAN_DATE_S1)
        Me.Controls.Add(Me.HENKAN_DATE_S)
        Me.Controls.Add(Me.FUNOU_DATE_S2)
        Me.Controls.Add(Me.FUNOU_DATE_S1)
        Me.Controls.Add(Me.FUNOU_DATE_S)
        Me.Controls.Add(Me.SOUSIN_DATE_S2)
        Me.Controls.Add(Me.SOUSIN_DATE_S1)
        Me.Controls.Add(Me.SOUSIN_DATE_S)
        Me.Controls.Add(Me.HAISIN_DATE_S2)
        Me.Controls.Add(Me.HAISIN_DATE_S1)
        Me.Controls.Add(Me.HAISIN_DATE_S)
        Me.Controls.Add(Me.TOUROKU_DATE_S2)
        Me.Controls.Add(Me.TOUROKU_DATE_S1)
        Me.Controls.Add(Me.TOUROKU_DATE_S)
        Me.Controls.Add(Me.KESSAI_YDATE_S2)
        Me.Controls.Add(Me.KESSAI_YDATE_S1)
        Me.Controls.Add(Me.KESSAI_YDATE_S)
        Me.Controls.Add(Me.HENKAN_YDATE_S2)
        Me.Controls.Add(Me.HENKAN_YDATE_S1)
        Me.Controls.Add(Me.HENKAN_YDATE_S)
        Me.Controls.Add(Me.FUNOU_YDATE_S2)
        Me.Controls.Add(Me.FUNOU_YDATE_S1)
        Me.Controls.Add(Me.FUNOU_YDATE_S)
        Me.Controls.Add(Me.SOUSIN_YDATE_S2)
        Me.Controls.Add(Me.SOUSIN_YDATE_S1)
        Me.Controls.Add(Me.SOUSIN_YDATE_S)
        Me.Controls.Add(Me.HAISIN_YDATE_S2)
        Me.Controls.Add(Me.HAISIN_YDATE_S1)
        Me.Controls.Add(Me.HAISIN_YDATE_S)
        Me.Controls.Add(Me.UKETUKE_DATE_S2)
        Me.Controls.Add(Me.UKETUKE_DATE_S1)
        Me.Controls.Add(Me.UKETUKE_DATE_S)
        Me.Controls.Add(Me.FURI_CODE_S)
        Me.Controls.Add(Me.KIGYO_CODE_S)
        Me.Controls.Add(Me.FURI_DATE_S2)
        Me.Controls.Add(Me.FURI_DATE_S1)
        Me.Controls.Add(Me.FURI_DATE_S)
        Me.Controls.Add(Me.ITAKU_NNAME_T)
        Me.Controls.Add(Me.TORIF_CODE_S)
        Me.Controls.Add(Me.TORIS_CODE_S)
        Me.Controls.Add(Me.lblUser)
        Me.Controls.Add(Me.lblDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label107)
        Me.Controls.Add(Me.Label44)
        Me.Controls.Add(Me.Label45)
        Me.Controls.Add(Me.Label43)
        Me.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MaximumSize = New System.Drawing.Size(800, 600)
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(800, 600)
        Me.Name = "KFJMAST030"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "KFJMAST030"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents lblUser As System.Windows.Forms.Label
    Friend WithEvents lblDate As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label104 As System.Windows.Forms.Label
    Friend WithEvents Label103 As System.Windows.Forms.Label
    Friend WithEvents Label102 As System.Windows.Forms.Label
    Friend WithEvents Label101 As System.Windows.Forms.Label
    Friend WithEvents Label100 As System.Windows.Forms.Label
    Friend WithEvents Label99 As System.Windows.Forms.Label
    Friend WithEvents Label98 As System.Windows.Forms.Label
    Friend WithEvents Label97 As System.Windows.Forms.Label
    Friend WithEvents SAIFURI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents SAIFURI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents SAIFURI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents KSAIFURI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents KSAIFURI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents KSAIFURI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents cmbToriName As System.Windows.Forms.ComboBox
    Friend WithEvents Label96 As System.Windows.Forms.Label
    Friend WithEvents Label91 As System.Windows.Forms.Label
    Friend WithEvents Label92 As System.Windows.Forms.Label
    Friend WithEvents Label93 As System.Windows.Forms.Label
    Friend WithEvents Label88 As System.Windows.Forms.Label
    Friend WithEvents Label89 As System.Windows.Forms.Label
    Friend WithEvents Label90 As System.Windows.Forms.Label
    Friend WithEvents Label85 As System.Windows.Forms.Label
    Friend WithEvents Label86 As System.Windows.Forms.Label
    Friend WithEvents Label87 As System.Windows.Forms.Label
    Friend WithEvents Label82 As System.Windows.Forms.Label
    Friend WithEvents Label83 As System.Windows.Forms.Label
    Friend WithEvents Label84 As System.Windows.Forms.Label
    Friend WithEvents Label79 As System.Windows.Forms.Label
    Friend WithEvents Label80 As System.Windows.Forms.Label
    Friend WithEvents Label81 As System.Windows.Forms.Label
    Friend WithEvents Label76 As System.Windows.Forms.Label
    Friend WithEvents Label77 As System.Windows.Forms.Label
    Friend WithEvents Label78 As System.Windows.Forms.Label
    Friend WithEvents Label73 As System.Windows.Forms.Label
    Friend WithEvents Label74 As System.Windows.Forms.Label
    Friend WithEvents Label75 As System.Windows.Forms.Label
    Friend WithEvents Label70 As System.Windows.Forms.Label
    Friend WithEvents Label71 As System.Windows.Forms.Label
    Friend WithEvents Label72 As System.Windows.Forms.Label
    Friend WithEvents Label67 As System.Windows.Forms.Label
    Friend WithEvents Label68 As System.Windows.Forms.Label
    Friend WithEvents Label69 As System.Windows.Forms.Label
    Friend WithEvents Label64 As System.Windows.Forms.Label
    Friend WithEvents Label65 As System.Windows.Forms.Label
    Friend WithEvents Label66 As System.Windows.Forms.Label
    Friend WithEvents Label61 As System.Windows.Forms.Label
    Friend WithEvents Label62 As System.Windows.Forms.Label
    Friend WithEvents Label63 As System.Windows.Forms.Label
    Friend WithEvents Label58 As System.Windows.Forms.Label
    Friend WithEvents Label59 As System.Windows.Forms.Label
    Friend WithEvents Label60 As System.Windows.Forms.Label
    Friend WithEvents Label55 As System.Windows.Forms.Label
    Friend WithEvents Label56 As System.Windows.Forms.Label
    Friend WithEvents Label57 As System.Windows.Forms.Label
    Friend WithEvents Label54 As System.Windows.Forms.Label
    Friend WithEvents Label53 As System.Windows.Forms.Label
    Friend WithEvents Label52 As System.Windows.Forms.Label
    Friend WithEvents Label51 As System.Windows.Forms.Label
    Friend WithEvents Label50 As System.Windows.Forms.Label
    Friend WithEvents Label47 As System.Windows.Forms.Label
    Friend WithEvents Label45 As System.Windows.Forms.Label
    Friend WithEvents Label44 As System.Windows.Forms.Label
    Friend WithEvents Label43 As System.Windows.Forms.Label
    Friend WithEvents Label42 As System.Windows.Forms.Label
    Friend WithEvents Label41 As System.Windows.Forms.Label
    Friend WithEvents Label40 As System.Windows.Forms.Label
    Friend WithEvents Label39 As System.Windows.Forms.Label
    Friend WithEvents Label38 As System.Windows.Forms.Label
    Friend WithEvents Label37 As System.Windows.Forms.Label
    Friend WithEvents Label36 As System.Windows.Forms.Label
    Friend WithEvents Label35 As System.Windows.Forms.Label
    Friend WithEvents Label34 As System.Windows.Forms.Label
    Friend WithEvents Label33 As System.Windows.Forms.Label
    Friend WithEvents 他行 As System.Windows.Forms.Label
    Friend WithEvents Label31 As System.Windows.Forms.Label
    Friend WithEvents Label30 As System.Windows.Forms.Label
    Friend WithEvents Label29 As System.Windows.Forms.Label
    Friend WithEvents Label28 As System.Windows.Forms.Label
    Friend WithEvents Label27 As System.Windows.Forms.Label
    Friend WithEvents Label26 As System.Windows.Forms.Label
    Friend WithEvents Label25 As System.Windows.Forms.Label
    Friend WithEvents Label24 As System.Windows.Forms.Label
    Friend WithEvents Label23 As System.Windows.Forms.Label
    Friend WithEvents Label22 As System.Windows.Forms.Label
    Friend WithEvents Label21 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label14 As System.Windows.Forms.Label
    Friend WithEvents Label15 As System.Windows.Forms.Label
    Friend WithEvents Label16 As System.Windows.Forms.Label
    Friend WithEvents Label17 As System.Windows.Forms.Label
    Friend WithEvents Label18 As System.Windows.Forms.Label
    Friend WithEvents Label19 As System.Windows.Forms.Label
    Friend WithEvents Label13 As System.Windows.Forms.Label
    Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label94 As System.Windows.Forms.Label
    Friend WithEvents Label95 As System.Windows.Forms.Label
    Friend WithEvents Label106 As System.Windows.Forms.Label
    Friend WithEvents Label107 As System.Windows.Forms.Label
    Friend WithEvents cmbKana As System.Windows.Forms.ComboBox
    Friend WithEvents BAITAI_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents ITAKU_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents CmdBack As System.Windows.Forms.Button
    Friend WithEvents CmdDelete As System.Windows.Forms.Button
    Friend WithEvents CmdSelect As System.Windows.Forms.Button
    Friend WithEvents CmdUpdate As System.Windows.Forms.Button
    Friend WithEvents CmdInsert As System.Windows.Forms.Button
    Friend WithEvents CmdClear As System.Windows.Forms.Button
    Friend WithEvents KESSAI_TIME_STAMP_S As System.Windows.Forms.TextBox
    Friend WithEvents JIFURI_TIME_STAMP_S As System.Windows.Forms.TextBox
    Friend WithEvents SAKUSEI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents SAKUSEI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents SAKUSEI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents MOTIKOMI_SEQ_S As System.Windows.Forms.TextBox
    Friend WithEvents SFILE_NAME_S As System.Windows.Forms.TextBox
    Friend WithEvents FILE_SEQ_S As System.Windows.Forms.TextBox
    Friend WithEvents MOTIKOMI_KBN_S As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_KBN_S As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_KIN_S As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_KBN_S As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_KIN_S As System.Windows.Forms.TextBox
    Friend WithEvents FURI_KIN_S As System.Windows.Forms.TextBox
    Friend WithEvents SYORI_KIN_S As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_KEN_S As System.Windows.Forms.TextBox
    Friend WithEvents FURI_KEN_S As System.Windows.Forms.TextBox
    Friend WithEvents SYORI_KEN_S As System.Windows.Forms.TextBox
    Friend WithEvents TYUUDAN_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents TESUUTYO_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents TESUUKEI_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents NIPPO_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents SAIFURI_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents TOUROKU_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents UKETUKE_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents TOUROKU_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents TOUROKU_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents TOUROKU_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents KESSAI_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents HENKAN_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents FUNOU_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents SOUSIN_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents HAISIN_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents UKETUKE_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents UKETUKE_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents UKETUKE_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents FURI_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents KIGYO_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents FURI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents ITAKU_NNAME_T As System.Windows.Forms.TextBox
    Friend WithEvents TORIF_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents TORIS_CODE_S As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label105 As System.Windows.Forms.Label
    Friend WithEvents KFURI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents KFURI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents KFURI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents Label111 As System.Windows.Forms.Label
    Friend WithEvents Label112 As System.Windows.Forms.Label
    Friend WithEvents Label113 As System.Windows.Forms.Label
    Friend WithEvents Label114 As System.Windows.Forms.Label
    Friend WithEvents Label115 As System.Windows.Forms.Label
    Friend WithEvents Label116 As System.Windows.Forms.Label
    Friend WithEvents Label117 As System.Windows.Forms.Label
    Friend WithEvents Label118 As System.Windows.Forms.Label
    Friend WithEvents TESUU_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents MEM_FLG As System.Windows.Forms.TextBox
    Friend WithEvents TESUU_TIME_STAMP_S As System.Windows.Forms.TextBox
    Friend WithEvents Label119 As System.Windows.Forms.Label
    Friend WithEvents Label120 As System.Windows.Forms.Label
    Friend WithEvents Label121 As System.Windows.Forms.Label
    Friend WithEvents Label122 As System.Windows.Forms.Label
    Friend WithEvents Label123 As System.Windows.Forms.Label
    Friend WithEvents Label124 As System.Windows.Forms.Label
    Friend WithEvents Label125 As System.Windows.Forms.Label
    Friend WithEvents Label126 As System.Windows.Forms.Label
    Friend WithEvents IRAISYOK_YDATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents IRAISYOK_YDATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents IRAISYOK_YDATE_S As System.Windows.Forms.TextBox
    Friend WithEvents IRAISYO_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents IRAISYO_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents IRAISYO_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents Label131 As System.Windows.Forms.Label
    Friend WithEvents Label48 As System.Windows.Forms.Label
    Friend WithEvents TAKOU_FLG_S As System.Windows.Forms.TextBox
    Friend WithEvents Label32 As System.Windows.Forms.Label
    Friend WithEvents Label49 As System.Windows.Forms.Label
    Friend WithEvents ERR_KIN_S As System.Windows.Forms.TextBox
    Friend WithEvents ERR_KEN_S As System.Windows.Forms.TextBox
    Friend WithEvents Label127 As System.Windows.Forms.Label
    Friend WithEvents Label128 As System.Windows.Forms.Label
    Friend WithEvents Label129 As System.Windows.Forms.Label
    Friend WithEvents MOTIKOMI_DATE_S2 As System.Windows.Forms.TextBox
    Friend WithEvents MOTIKOMI_DATE_S1 As System.Windows.Forms.TextBox
    Friend WithEvents MOTIKOMI_DATE_S As System.Windows.Forms.TextBox
    Friend WithEvents Label130 As System.Windows.Forms.Label
    Friend WithEvents txtBeforeSaifuri As System.Windows.Forms.TextBox
    Friend WithEvents btnFuriDate As System.Windows.Forms.Button
End Class
