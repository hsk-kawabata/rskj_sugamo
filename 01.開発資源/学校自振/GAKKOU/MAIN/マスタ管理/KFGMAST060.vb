Option Explicit On 
Option Strict Off

Imports System.Text
Imports CASTCommon

Public Class KFGMAST060

    Private Enum gintPG_KBN As Integer
        KOBETU = 1
        IKKATU = 2
    End Enum
    Private Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum

    Private gstrTORIS_CODE As String
    Private gstrFURI_DATE As String

    Private gastrTORIS_CODE_T As String
    Private gastrTORIF_CODE_T As String
    Private gastrITAKU_KNAME_T As String
    Private gastrITAKU_NNAME_T As String
    Private gastrFILE_NAME_T As String
    Private gastrKIGYO_CODE_T As String
    Private gastrFURI_CODE_T As String
    Private gastrBAITAI_CODE_T As String
    Private gastrFMT_KBN_T As String
    Private gastrTAKOU_KBN_T As String
    Private gastrITAKU_CODE_T As String
    Private gastrNS_KBN_T As String
    Private gastrLABEL_KBN As String
    Private gastrITAKU_KIN As String
    Private gastrITAKU_SIT As String
    Private gastrITAKU_KAMOKU As String
    Private gastrITAKU_KOUZA As String
    Private gastrTEKIYO_KBN As String
    Private gastrKTEKIYO As String
    Private gastrNTEKIYO As String
    Private gastrMULTI_KBN As String
    Private gastrNS_KBN As String
    Private gastrCODE_KBN_T As String

    'SCHMAST梡僨乕僞僙僢僩
    Private gstrKYUJITU As String
    Private gstrWORK_DATE As String
    Private gSCH_DATA(71) As String


#Region " 嫟捠曄悢愰尵 "

    Private STR惪媮擭寧 As String
    Private STR怳懼擔 As String
    Private STR嵞怳懼擔 As String
    '2010/10/21 宊栺怳懼擔捛壛
    Private STR宊栺怳懼擔 As String

    Private STR僗働嬫暘 As String
    Private STR怳懼嬫暘 As String
    Private STR妛擭侾 As String
    Private STR妛擭俀 As String
    Private STR妛擭俁 As String
    Private STR妛擭係 As String
    Private STR妛擭俆 As String
    Private STR妛擭俇 As String
    Private STR妛擭俈 As String
    Private STR妛擭俉 As String
    Private STR妛擭俋 As String
    Private STR侾妛擭 As String
    Private STR俀妛擭 As String
    Private STR俁妛擭 As String
    Private STR係妛擭 As String
    Private STR俆妛擭 As String
    Private STR俇妛擭 As String
    Private STR俈妛擭 As String
    Private STR俉妛擭 As String
    Private STR俋妛擭 As String

    Private STR擭娫擖椡怳懼擔 As String

    'Private STR柧嵶嶌惉梊掕擔 As String
    'Private STR僠僃僢僋梊掕擔 As String
    'Private STR怳懼僨乕僞嶌惉梊掕擔 As String
    'Private STR晄擻寢壥峏怴梊掕擔 As String
    'Private STR寛嵪梊掕擔 As String
    Private STRW嵞怳懼擭 As String
    Private STRW嵞怳懼寧 As String
    Private STRW嵞怳懼擔 As String
    Private STR張棟柤 As String
    Private STRYasumi_List(0) As String

    Private str媽怳懼擔(6) As String '2006/11/22
    Private str媽嵞怳擔(6) As String '2006/11/22
    Private int媽怳懼俬俢 As Integer '2006/11/22
    Private str捠忢怳懼擔(12) As String '2006/11/22
    Private str捠忢嵞怳擔(12) As String '2006/11/22

    Private str捠忢嵞乆怳擔(12) As String '2006/11/30
    Private str摿暿嵞乆怳擔(6) As String '2006/11/30
    Private bln擭娫峏怴(12) As Boolean '2006/11/30
    Private bln摿暿峏怴(6) As Boolean '2006/11/30
    Private bln悘帪峏怴(6) As Boolean '2006/11/30

    Private Int_Zengo_Kbn(1) As String

    '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
    Private Sai_Zengo_Kbn As String       '嵞怳媥擔僔僼僩
    '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END

    Private Structure NenkanData
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
        <VBFixedStringAttribute(10)> Public Furikae_Day As String
        <VBFixedStringAttribute(10)> Public SaiFurikae_Day As String
        Public Furikae_Check As Boolean
        Public SaiFurikae_Check As Boolean
        Public Furikae_Enabled As Boolean
        Public SaiFurikae_Enabled As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
    End Structure
    Private NENKAN_SCHINFO(12) As NenkanData
    Private SYOKI_NENKAN_SCHINFO(12) As NenkanData '2006/11/30

    Private Structure TokubetuData
        <VBFixedStringAttribute(2)> Public Seikyu_Tuki As String
        Public SyoriFurikae_Flag As Boolean
        Public CheckFurikae_Flag As Boolean '2006/11/30
        Public FunouFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public SyoriSaiFurikae_Flag As Boolean
        Public CheckSaiFurikae_Flag As Boolean '2006/11/30
        <VBFixedStringAttribute(2)> Public SaiFurikae_Tuki As String
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean
    End Structure
    Private TOKUBETU_SCHINFO(6) As TokubetuData
    Private SYOKI_TOKUBETU_SCHINFO(6) As TokubetuData

    Private Structure ZuijiData
        <VBFixedStringAttribute(2)> Public Nyusyutu_Kbn As String
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String
        <VBFixedStringAttribute(2)> Public Furikae_Date As String
        Public Syori_Flag As Boolean
        Public SiyouGakunenALL_Check As Boolean
        Public SiyouGakunen1_Check As Boolean
        Public SiyouGakunen2_Check As Boolean
        Public SiyouGakunen3_Check As Boolean
        Public SiyouGakunen4_Check As Boolean
        Public SiyouGakunen5_Check As Boolean
        Public SiyouGakunen6_Check As Boolean
        Public SiyouGakunen7_Check As Boolean
        Public SiyouGakunen8_Check As Boolean
        Public SiyouGakunen9_Check As Boolean
    End Structure
    Private ZUIJI_SCHINFO(6) As ZuijiData
    Private SYOKI_ZUIJI_SCHINFO(6) As ZuijiData

    Private Structure GakData
        <VBFixedStringAttribute(7)> Public GAKKOU_CODE As String
        <VBFixedStringAttribute(50)> Public GAKKOU_NNAME As String
        Public SIYOU_GAKUNEN As Integer
        <VBFixedStringAttribute(2)> Public FURI_DATE As String
        <VBFixedStringAttribute(2)> Public SFURI_DATE As String
        <VBFixedStringAttribute(1)> Public BAITAI_CODE As String
        <VBFixedStringAttribute(10)> Public ITAKU_CODE As String
        <VBFixedStringAttribute(4)> Public TKIN_CODE As String
        <VBFixedStringAttribute(3)> Public TSIT_CODE As String
        <VBFixedStringAttribute(1)> Public SFURI_SYUBETU As String
        <VBFixedStringAttribute(6)> Public KAISI_DATE As String
        <VBFixedStringAttribute(6)> Public SYURYOU_DATE As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KBN As String
        <VBFixedStringAttribute(1)> Public TESUUTYO_KIJITSU As String
        Public TESUUTYO_NO As Integer
        <VBFixedStringAttribute(1)> Public TESUU_KYU_CODE As String
        <VBFixedStringAttribute(6)> Public TAISYOU_START_NENDO As String
        <VBFixedStringAttribute(6)> Public TAISYOU_END_NENDO As String
    End Structure
    Private GAKKOU_INFO As GakData

    Private Str_SyoriDate(1) As String

    '張棟忬嫷(0:擭娫1:摿暿2:悘帪)
    '0:僗働僕儏乕儖枹嶌惉
    '1:僗働僕儏乕儖嶌惉惉岟
    '2:僗働僕儏乕儖嶌惉幐攕
    Private Int_Syori_Flag(2) As Integer

    Private Int_Zuiji_Flag As Integer
    Private Int_Tokubetu_Flag As Integer


    Private Str_FURI_DATE As String
    Private Str_SFURI_DATE As String

    Private strFURI_DT As String '妛峑儅僗僞俀偺怳懼擔
    Private strSFURI_DT As String '妛峑儅僗僞俀偺嵞怳懼擔

    '2006/10/24
    Private strENTRI_FLG As String = "0"
    Private strCHECK_FLG As String = "0"
    Private strDATA_FLG As String = "0"
    Private strFUNOU_FLG As String = "0"
    Private strHENKAN_FLG As String = "0"
    Private strSAIFURI_FLG As String = "0"
    Private strKESSAI_FLG As String = "0"
    Private strTYUUDAN_FLG As String = "0"
    Private strENTRI_FLG_SAI As String = "0"
    Private strCHECK_FLG_SAI As String = "0"
    Private strDATA_FLG_SAI As String = "0"
    Private strFUNOU_FLG_SAI As String = "0"
    Private strSAIFURI_FLG_SAI As String = "0"
    Private strKESSAI_FLG_SAI As String = "0"
    Private strTYUUDAN_FLG_SAI As String = "0"

    Private strSAIFURI_DEF As String = "00000000" '捠忢僗働僕儏乕儖偺嵞怳擔

    Private lngSYORI_KEN As Long = 0
    Private dblSYORI_KIN As Double = 0
    Private lngFURI_KEN As Long = 0
    Private dblFURI_KIN As Double = 0
    Private lngFUNOU_KEN As Long = 0
    Private dblFUNOU_KIN As Double = 0

    '婇嬈帺怳僗働僕儏乕儖楢実梡丂2006/12/01
    Private strSYOFURI_NENKAN(12) As String
    Private strSAIFURI_NENKAN(12) As String
    Private strSYOFURI_TOKUBETU(6) As String
    Private strSAIFURI_TOKUBETU(6) As String
    Private strFURI_ZUIJI(6) As String
    Private strFURIKBN_ZUIJI(6) As String
    Private strSYOFURI_NENKAN_AFTER(12) As String '峏怴屻偺僗働僕儏乕儖
    Private strSAIFURI_NENKAN_AFTER(12) As String '峏怴屻偺僗働僕儏乕儖
    Private strSYOFURI_TOKUBETU_AFTER(6) As String '峏怴屻偺僗働僕儏乕儖
    Private strSAIFURI_TOKUBETU_AFTER(6) As String '峏怴屻偺僗働僕儏乕儖
    Private strFURI_ZUIJI_AFTER(6) As String '峏怴屻偺僗働僕儏乕儖
    Private strFURIKBN_ZUIJI_AFTER(6) As String '峏怴屻偺僗働僕儏乕儖

    Private intPUSH_BTN As Integer '0:嶌惉丂1:嶲徠 2:峏怴 3:庢徚
#End Region

    '2010.02.27 曄悢惍棟偺偨傔怴婯嶌惉 伀************
    Private strGakkouCode As String

    Private Structure LogWrite
        Dim UserID As String            '儐乕僓ID
        Dim ToriCode As String          '庢堷愭庡暃僐乕僪
        Dim FuriDate As String          '怳懼擔
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST060", "擭娫僗働僕儏乕儖嶌惉夋柺")
    Private Const msgTitle As String = "擭娫僗働僕儏乕儖嶌惉夋柺(KFGMAST060)"
    Private MainDB As MyOracle

#Region " Form_Load "
    Private Sub KFGMAST060_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try

            '儘僌梡
            LW.UserID = GCom.GetUserID
            LW.ToriCode = "000000000000"
            LW.FuriDate = "00000000"

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)奐巒", "惉岟", "")

            '2016/10/07 saitou RSV2 ADD 妛峑彅夛旓儊儞僥僫儞僗 ---------------------------------------- START
            Call GSUB_CONNECT()
            '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

            '妛峑僐儞儃愝掕乮慡妛峑乯
            If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmbGAKKOUNAME)")
                MessageBox.Show("妛峑柤僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

            '僥僉僗僩僼傽僀儖偐傜僐儞儃儃僢僋僗愝掕
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘侾) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘侾)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俀) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘俀)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俁) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘俁)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘係) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘係)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俆) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘俆)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俇) = False Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)", "幐攕", "僐儞儃儃僢僋僗愝掕(cmb擖弌嬫暘俇)")
                MessageBox.Show("擖弌嬫暘僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            '弶婜夋柺昞帵
            Call PSUB_FORMAT_ALL()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)椺奜僄儔乕", "幐攕", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(儘乕僪)廔椆", "惉岟", "")
        End Try

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)奐巒", "惉岟", "")

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 0

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 寧偑0傑偨偼12傛傝傕戝偒偔愝掕偝傟偨応崌偼僄儔乕 偙偙偐傜
            For Each txt摿暿寧 As Control In TabPage2.Controls
                If Mid(txt摿暿寧.Name, 1, 8) = "txt摿暿惪媮寧" OrElse Mid(txt摿暿寧.Name, 1, 8) = "txt摿暿怳懼寧" _
                    OrElse Mid(txt摿暿寧.Name, 1, 9) = "txt摿暿嵞怳懼寧" Then
                    If txt摿暿寧.Text <> "" Then
                        If CInt(txt摿暿寧.Text) > 12 OrElse CInt(txt摿暿寧.Text) = 0 Then
                            MessageBox.Show("寧偼侾乣侾俀傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt摿暿寧.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            For Each txt悘帪怳懼寧 As Control In TabPage3.Controls
                If Mid(txt悘帪怳懼寧.Name, 1, 8) = "txt悘帪怳懼寧" Then
                    If txt悘帪怳懼寧.Text <> "" Then
                        If CInt(txt悘帪怳懼寧.Text) > 12 OrElse CInt(txt悘帪怳懼寧.Text) = 0 Then
                            MessageBox.Show("寧偼侾乣侾俀傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt悘帪怳懼寧.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 寧偑0傑偨偼12傛傝傕戝偒偔愝掕偝傟偨応崌偼僄儔乕 偙偙傑偱

            Call sb_HENSU_CLEAR()

            '2006/12/08丂乽嶌惉偡傞乿偲偄偆僼儔僌傪棫偰傞
            Call PSUB_Kousin_Check()

            If PFUNC_SCH_INSERT_ALL() = False Then
                Return
            End If

            '擖椡崁栚惂屼
            txt懳徾擭搙.Enabled = False
            txtGAKKOU_CODE.Enabled = False

            If Int_Syori_Flag(0) = 2 Then '捛壛 2005/06/15
                '擖椡儃僞儞惂屼
                Call PSUB_BUTTON_Enable(0)
            Else
                '擖椡儃僞儞惂屼
                Call PSUB_BUTTON_Enable(1)
            End If

            Call sb_SANSYOU_SET()

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)椺奜僄儔乕", "幐攕", ex.Message)
        Finally
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)廔椆", "惉岟", "")
        End Try
        

    End Sub
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click

        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(嶲徠)奐巒", "惉岟", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 1

            '嶲徠儃僞儞
            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            If PFUNC_SCH_GET_ALL() = False Then
                Exit Sub
            End If

            '2006/10/11丂嵟崅妛擭埲忋偺妛擭偺巊梡晄壜
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '擖椡儃僞儞惂屼
            Call PSUB_BUTTON_Enable(1)

            '婇嬈楢実岦偗 2006/12/04
            Call sb_SANSYOU_SET()

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(嶲徠)椺奜僄儔乕", "幐攕", ex.Message)

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(嶲徠)廔椆", "惉岟", "")
            MainDB.Close()
        End Try

    End Sub
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click


        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(峏怴)奐巒", "惉岟", "")
            MainDB = New MyOracle

            Cursor.Current = Cursors.WaitCursor()
            intPUSH_BTN = 2

            strGakkouCode = Trim(txtGAKKOU_CODE.Text)

            '2010/10/21 寧偑0傑偨偼12傛傝傕戝偒偔愝掕偝傟偨応崌偼僄儔乕 偙偙偐傜
            For Each txt摿暿寧 As Control In TabPage2.Controls
                If Mid(txt摿暿寧.Name, 1, 8) = "txt摿暿惪媮寧" OrElse Mid(txt摿暿寧.Name, 1, 8) = "txt摿暿怳懼寧" _
                    OrElse Mid(txt摿暿寧.Name, 1, 9) = "txt摿暿嵞怳懼寧" Then
                    If txt摿暿寧.Text <> "" Then
                        If CInt(txt摿暿寧.Text) > 12 OrElse CInt(txt摿暿寧.Text) = 0 Then
                            MessageBox.Show("寧偼侾乣侾俀傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt摿暿寧.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next

            '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------START
            If PFUNC_Nyuryoku_Check() = False Then
                Exit Sub
            End If
            '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------END

            For Each txt悘帪怳懼寧 As Control In TabPage3.Controls
                If Mid(txt悘帪怳懼寧.Name, 1, 8) = "txt悘帪怳懼寧" Then
                    If txt悘帪怳懼寧.Text <> "" Then
                        If CInt(txt悘帪怳懼寧.Text) > 12 OrElse CInt(txt悘帪怳懼寧.Text) = 0 Then
                            MessageBox.Show("寧偼侾乣侾俀傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txt悘帪怳懼寧.Focus()
                            Exit Sub
                        End If
                    End If
                End If
            Next
            '2010/10/21 寧偑0傑偨偼12傛傝傕戝偒偔愝掕偝傟偨応崌偼僄儔乕 偙偙傑偱

            Call sb_HENSU_CLEAR()

            If PFUNC_SCH_DELETE_INSERT_ALL() = False Then

                MainDB.Rollback()
                Return

            End If

            MainDB.Commit()

            '擖椡崁栚惂屼
            txt懳徾擭搙.Enabled = True
            txtGAKKOU_CODE.Enabled = True
            '2006/10/11丂嵟崅妛擭埲忋偺妛擭偺巊梡晄壜
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            '擖椡儃僞儞惂屼
            Call PSUB_BUTTON_Enable(2)

        Catch ex As Exception

            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(峏怴)椺奜僄儔乕", "幐攕", ex.Message)
            MainDB.Rollback()

        Finally

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(峏怴)廔椆", "惉岟", "")
            MainDB.Close()

        End Try

    End Sub
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click
        intPUSH_BTN = 3

        '庢徚儃僞儞

        '夋柺弶婜忬懺
        Call PSUB_FORMAT_ALL()

        '捛壛 2006/12/27
        ReDim SYOKI_NENKAN_SCHINFO(12)
        ReDim SYOKI_TOKUBETU_SCHINFO(6)
        ReDim SYOKI_ZUIJI_SCHINFO(6)
        ReDim NENKAN_SCHINFO(12)
        ReDim TOKUBETU_SCHINFO(6)
        ReDim ZUIJI_SCHINFO(6)

        txt懳徾擭搙.Focus()

    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        Try
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(僋儘乕僘)奐巒", "惉岟", "")
            Me.Close()
        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(僋儘乕僘)椺奜僄儔乕", "幐攕", ex.Message)
        Finally
            If Not MainDB Is Nothing Then MainDB.Close()
        End Try
    End Sub
#End Region

#Region " GotFocus "
    '妛峑忣曬
    Private Sub txt懳徾擭搙_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt懳徾擭搙.GotFocus
        Me.txt懳徾擭搙.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt懳徾擭搙)

    End Sub
    Private Sub txtGAKKOU_CODE_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.GotFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txtGAKKOU_CODE)

    End Sub
    '擭娫僗働僕儏乕儖
    Private Sub txt係寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt係寧怳懼擔.GotFocus
        Me.txt係寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt係寧怳懼擔)

    End Sub
    Private Sub txt俆寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俆寧怳懼擔.GotFocus
        Me.txt俆寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俆寧怳懼擔)

    End Sub
    Private Sub txt俇寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俇寧怳懼擔.GotFocus
        Me.txt俇寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俇寧怳懼擔)

    End Sub
    Private Sub txt俈寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俈寧怳懼擔.GotFocus
        Me.txt俈寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俈寧怳懼擔)

    End Sub
    Private Sub txt俉寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俉寧怳懼擔.GotFocus
        Me.txt俉寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俉寧怳懼擔)

    End Sub
    Private Sub txt俋寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俋寧怳懼擔.GotFocus
        Me.txt俋寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俋寧怳懼擔)

    End Sub
    Private Sub txt侾侽寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侽寧怳懼擔.GotFocus
        Me.txt侾侽寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾侽寧怳懼擔)

    End Sub
    Private Sub txt侾侾寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侾寧怳懼擔.GotFocus
        Me.txt侾侾寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾侾寧怳懼擔)

    End Sub
    Private Sub txt侾俀寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾俀寧怳懼擔.GotFocus
        Me.txt侾俀寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾俀寧怳懼擔)

    End Sub
    Private Sub txt侾寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾寧怳懼擔.GotFocus
        Me.txt侾寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾寧怳懼擔)

    End Sub
    Private Sub txt俀寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俀寧怳懼擔.GotFocus
        Me.txt俀寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俀寧怳懼擔)

    End Sub
    Private Sub txt俁寧怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俁寧怳懼擔.GotFocus
        Me.txt俁寧怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俁寧怳懼擔)

    End Sub
    Private Sub txt係寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt係寧嵞怳懼擔.GotFocus
        Me.txt係寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt係寧嵞怳懼擔)

    End Sub
    Private Sub txt俆寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俆寧嵞怳懼擔.GotFocus
        Me.txt俆寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俆寧嵞怳懼擔)

    End Sub
    Private Sub txt俇寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俇寧嵞怳懼擔.GotFocus
        Me.txt俇寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俇寧嵞怳懼擔)

    End Sub
    Private Sub txt俈寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俈寧嵞怳懼擔.GotFocus
        Me.txt俈寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俈寧嵞怳懼擔)

    End Sub
    Private Sub txt俉寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俉寧嵞怳懼擔.GotFocus
        Me.txt俉寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俉寧嵞怳懼擔)

    End Sub
    Private Sub txt俋寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俋寧嵞怳懼擔.GotFocus
        Me.txt俋寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俋寧嵞怳懼擔)

    End Sub
    Private Sub txt侾侽寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侽寧嵞怳懼擔.GotFocus
        Me.txt侾侽寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾侽寧嵞怳懼擔)

    End Sub
    Private Sub txt侾侾寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侾寧嵞怳懼擔.GotFocus
        Me.txt侾侾寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾侾寧嵞怳懼擔)

    End Sub
    Private Sub txt侾俀寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾俀寧嵞怳懼擔.GotFocus
        Me.txt侾俀寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾俀寧嵞怳懼擔)

    End Sub
    Private Sub txt侾寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾寧嵞怳懼擔.GotFocus
        Me.txt侾寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt侾寧嵞怳懼擔)

    End Sub
    Private Sub txt俀寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俀寧嵞怳懼擔.GotFocus
        Me.txt俀寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俀寧嵞怳懼擔)

    End Sub
    Private Sub txt俁寧嵞怳懼擔_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俁寧嵞怳懼擔.GotFocus
        Me.txt俁寧嵞怳懼擔.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt俁寧嵞怳懼擔)

    End Sub
    '摿暿僗働僕儏乕儖
    Private Sub txt摿暿惪媮寧侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧侾.GotFocus
        Me.txt摿暿惪媮寧侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧侾)

    End Sub
    Private Sub txt摿暿惪媮寧俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俀.GotFocus
        Me.txt摿暿惪媮寧俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧俀)

    End Sub
    Private Sub txt摿暿惪媮寧俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俁.GotFocus
        Me.txt摿暿惪媮寧俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧俁)

    End Sub
    Private Sub txt摿暿惪媮寧係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧係.GotFocus
        Me.txt摿暿惪媮寧係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧係)

    End Sub
    Private Sub txt摿暿惪媮寧俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俆.GotFocus
        Me.txt摿暿惪媮寧俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧俆)

    End Sub
    Private Sub txt摿暿惪媮寧俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俇.GotFocus
        Me.txt摿暿惪媮寧俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿惪媮寧俇)

    End Sub
    Private Sub txt摿暿怳懼寧侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧侾.GotFocus
        Me.txt摿暿怳懼寧侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧侾)

    End Sub
    Private Sub txt摿暿怳懼寧俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俀.GotFocus
        Me.txt摿暿怳懼寧俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧俀)

    End Sub
    Private Sub txt摿暿怳懼寧俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俁.GotFocus
        Me.txt摿暿怳懼寧俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧俁)

    End Sub
    Private Sub txt摿暿怳懼寧係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧係.GotFocus
        Me.txt摿暿怳懼寧係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧係)

    End Sub
    Private Sub txt摿暿怳懼寧俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俆.GotFocus
        Me.txt摿暿怳懼寧俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧俆)

    End Sub
    Private Sub txt摿暿怳懼寧俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俇.GotFocus
        Me.txt摿暿怳懼寧俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼寧俇)

    End Sub
    Private Sub txt摿暿怳懼擔侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔侾.GotFocus
        Me.txt摿暿怳懼擔侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔侾)

    End Sub
    Private Sub txt摿暿怳懼擔俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俀.GotFocus
        Me.txt摿暿怳懼擔俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔俀)

    End Sub
    Private Sub txt摿暿怳懼擔俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俁.GotFocus
        Me.txt摿暿怳懼擔俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔俁)

    End Sub
    Private Sub txt摿暿怳懼擔係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔係.GotFocus
        Me.txt摿暿怳懼擔係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔係)

    End Sub
    Private Sub txt摿暿怳懼擔俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俆.GotFocus
        Me.txt摿暿怳懼擔俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔俆)

    End Sub
    Private Sub txt摿暿怳懼擔俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俇.GotFocus
        Me.txt摿暿怳懼擔俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿怳懼擔俇)

    End Sub
    Private Sub txt摿暿嵞怳懼寧侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧侾.GotFocus
        Me.txt摿暿嵞怳懼寧侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧侾)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俀.GotFocus
        Me.txt摿暿嵞怳懼寧俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧俀)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俁.GotFocus
        Me.txt摿暿嵞怳懼寧俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧俁)

    End Sub
    Private Sub txt摿暿嵞怳懼寧係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧係.GotFocus
        Me.txt摿暿嵞怳懼寧係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧係)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俆.GotFocus
        Me.txt摿暿嵞怳懼寧俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧俆)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俇.GotFocus
        Me.txt摿暿嵞怳懼寧俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼寧俇)

    End Sub
    Private Sub txt摿暿嵞怳懼擔侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔侾.GotFocus
        Me.txt摿暿嵞怳懼擔侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔侾)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俀.GotFocus
        Me.txt摿暿嵞怳懼擔俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔俀)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俁.GotFocus
        Me.txt摿暿嵞怳懼擔俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔俁)

    End Sub
    Private Sub txt摿暿嵞怳懼擔係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔係.GotFocus
        Me.txt摿暿嵞怳懼擔係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔係)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俆.GotFocus
        Me.txt摿暿嵞怳懼擔俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔俆)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俇.GotFocus
        Me.txt摿暿嵞怳懼擔俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt摿暿嵞怳懼擔俇)

    End Sub
    '悘帪僗働僕儏乕儖
    Private Sub txt悘帪怳懼寧侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧侾.GotFocus
        Me.txt悘帪怳懼寧侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧侾)

    End Sub
    Private Sub txt悘帪怳懼寧俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俀.GotFocus
        Me.txt悘帪怳懼寧俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧俀)

    End Sub
    Private Sub txt悘帪怳懼寧俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俁.GotFocus
        Me.txt悘帪怳懼寧俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧俁)

    End Sub
    Private Sub txt悘帪怳懼寧係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧係.GotFocus
        Me.txt悘帪怳懼寧係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧係)

    End Sub
    Private Sub txt悘帪怳懼寧俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俆.GotFocus
        Me.txt悘帪怳懼寧俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧俆)

    End Sub
    Private Sub txt悘帪怳懼寧俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俇.GotFocus
        Me.txt悘帪怳懼寧俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼寧俇)

    End Sub
    Private Sub txt悘帪怳懼擔侾_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔侾.GotFocus
        Me.txt悘帪怳懼擔侾.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔侾)

    End Sub
    Private Sub txt悘帪怳懼擔俀_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俀.GotFocus
        Me.txt悘帪怳懼擔俀.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔俀)

    End Sub
    Private Sub txt悘帪怳懼擔俁_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俁.GotFocus
        Me.txt悘帪怳懼擔俁.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔俁)

    End Sub
    Private Sub txt悘帪怳懼擔係_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔係.GotFocus
        Me.txt悘帪怳懼擔係.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔係)

    End Sub
    Private Sub txt悘帪怳懼擔俆_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俆.GotFocus
        Me.txt悘帪怳懼擔俆.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔俆)

    End Sub
    Private Sub txt悘帪怳懼擔俇_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俇.GotFocus
        Me.txt悘帪怳懼擔俇.BackColor = System.Drawing.Color.LightCyan
        Call GSUB_PRESEL(txt悘帪怳懼擔俇)

    End Sub
#End Region

#Region " LostFocus "
    '婎杮
    Private Sub txt懳徾擭搙_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt懳徾擭搙.LostFocus
        Me.txt懳徾擭搙.BackColor = System.Drawing.Color.White
        '媥擔忣曬偺昞帵
        If PFUNC_KYUJITULIST_SET() = False Then
            Exit Sub
        End If

        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt懳徾擭搙.Text) <> "" Then
            '懳徾擭搙傕擖椡偝傟偰偄傞応崌丄僗働僕儏乕儖懚嵼僠僃僢僋傪偐偗
            '僗働僕儏乕儖偑懚嵼偡傞応崌偼嶲徠儃僞儞偵僼僅乕僇僗堏摦
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    Private Sub txtGAKKOU_CODE_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtGAKKOU_CODE.LostFocus
        Me.txtGAKKOU_CODE.BackColor = System.Drawing.Color.White
        '妛峑柤偺庢摼
        If Trim(txtGAKKOU_CODE.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txtGAKKOU_CODE, 10)

            '妛峑柤偺庢摼(妛峑忣曬傕曄悢偵奿擺偝傟傞)
            If PFUNC_GAKINFO_GET() = False Then
                Exit Sub
            End If

            '擭娫僗働僕儏乕儖夋柺弶婜壔
            Call PSUB_NENKAN_FORMAT()

            '摿暿僗働僕儏乕儖夋柺弶婜壔
            Call PSUB_TOKUBETU_FORMAT()

            '悘帪僗働僕儏乕儖夋柺弶婜壔
            Call PSUB_ZUIJI_FORMAT()

            '嵞怳懼擔偺僾儘僥僋僩True
            Call PSUB_SAIFURI_PROTECT(True)

            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "0", "3"
                    Call PSUB_SAIFURI_PROTECT(False)
            End Select

            '2006/10/12丂嵟崅妛擭埲忋偺妛擭偺僠僃僢僋儃僢僋僗巊梡晄壜
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()

            If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt懳徾擭搙.Text) <> "" Then
                '懳徾擭搙傕擖椡偝傟偰偄傞応崌丄僗働僕儏乕儖懚嵼僠僃僢僋傪偐偗
                '僗働僕儏乕儖偑懚嵼偡傞応崌偼嶲徠儃僞儞偵僼僅乕僇僗堏摦
                Call PSUB_SANSYOU_FOCUS()
            End If
        Else
            '2006/10/12丂妛峑僐乕僪偑嬻敀偺偲偒丄妛峑柤儔儀儖傪嬻敀偵偡傞
            lab妛峑柤.Text = ""
        End If

    End Sub
    '擭娫
    Private Sub txt摿暿惪媮寧侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧侾.LostFocus
        Me.txt摿暿惪媮寧侾.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧侾, 2)
        End If

    End Sub
    Private Sub txt摿暿惪媮寧俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俀.LostFocus
        Me.txt摿暿惪媮寧俀.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧俀, 2)
        End If

    End Sub
    Private Sub txt摿暿惪媮寧俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俁.LostFocus
        Me.txt摿暿惪媮寧俁.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧俁, 2)
        End If

    End Sub
    Private Sub txt摿暿惪媮寧係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧係.LostFocus
        Me.txt摿暿惪媮寧係.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧係, 2)
        End If

    End Sub
    Private Sub txt摿暿惪媮寧俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俆.LostFocus
        Me.txt摿暿惪媮寧俆.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧俆, 2)
        End If

    End Sub
    Private Sub txt摿暿惪媮寧俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿惪媮寧俇.LostFocus
        Me.txt摿暿惪媮寧俇.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿惪媮寧俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿惪媮寧俇, 2)
        End If

    End Sub
    Private Sub txt係寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt係寧怳懼擔.LostFocus
        Me.txt係寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt係寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt係寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俆寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俆寧怳懼擔.LostFocus
        Me.txt俆寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俆寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俆寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俇寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俇寧怳懼擔.LostFocus
        Me.txt俇寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俇寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俇寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俈寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俈寧怳懼擔.LostFocus
        Me.txt俈寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俈寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俈寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俉寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俉寧怳懼擔.LostFocus
        Me.txt俉寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俉寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俉寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俋寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俋寧怳懼擔.LostFocus
        Me.txt俋寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俋寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俋寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾侽寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侽寧怳懼擔.LostFocus
        Me.txt侾侽寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾侽寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾侽寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾侾寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侾寧怳懼擔.LostFocus
        Me.txt侾侾寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾侾寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾侾寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾俀寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾俀寧怳懼擔.LostFocus
        Me.txt侾俀寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾俀寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾俀寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾寧怳懼擔.LostFocus
        Me.txt侾寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俀寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俀寧怳懼擔.LostFocus
        Me.txt俀寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俀寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俀寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俁寧怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俁寧怳懼擔.LostFocus
        Me.txt俁寧怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俁寧怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俁寧怳懼擔, 2)
        End If

    End Sub
    Private Sub txt係寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt係寧嵞怳懼擔.LostFocus
        Me.txt係寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt係寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt係寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俆寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俆寧嵞怳懼擔.LostFocus
        Me.txt俆寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俆寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俆寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俇寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俇寧嵞怳懼擔.LostFocus
        Me.txt俇寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俇寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俇寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俈寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俈寧嵞怳懼擔.LostFocus
        Me.txt俈寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俈寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俈寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俉寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俉寧嵞怳懼擔.LostFocus
        Me.txt俉寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俉寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俉寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俋寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俋寧嵞怳懼擔.LostFocus
        Me.txt俋寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俋寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俋寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾侽寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侽寧嵞怳懼擔.LostFocus
        Me.txt侾侽寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾侽寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾侽寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾侾寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾侾寧嵞怳懼擔.LostFocus
        Me.txt侾侾寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾侾寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾侾寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾俀寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾俀寧嵞怳懼擔.LostFocus
        Me.txt侾俀寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾俀寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾俀寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt侾寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt侾寧嵞怳懼擔.LostFocus
        Me.txt侾寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt侾寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt侾寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俀寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俀寧嵞怳懼擔.LostFocus
        Me.txt俀寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俀寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俀寧嵞怳懼擔, 2)
        End If

    End Sub
    Private Sub txt俁寧嵞怳懼擔_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt俁寧嵞怳懼擔.LostFocus
        Me.txt俁寧嵞怳懼擔.BackColor = System.Drawing.Color.White
        If Trim(txt俁寧嵞怳懼擔.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt俁寧嵞怳懼擔, 2)
        End If

    End Sub
    '摿暿
    Private Sub txt摿暿怳懼寧侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧侾.LostFocus
        Me.txt摿暿怳懼寧侾.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧侾, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼寧俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俀.LostFocus
        Me.txt摿暿怳懼寧俀.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧俀, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼寧俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俁.LostFocus
        Me.txt摿暿怳懼寧俁.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧俁, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼寧係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧係.LostFocus
        Me.txt摿暿怳懼寧係.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧係, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼寧俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俆.LostFocus
        Me.txt摿暿怳懼寧俆.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧俆, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼寧俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼寧俇.LostFocus
        Me.txt摿暿怳懼寧俇.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼寧俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼寧俇, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔侾.LostFocus
        Me.txt摿暿怳懼擔侾.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔侾, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俀.LostFocus
        Me.txt摿暿怳懼擔俀.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔俀, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俁.LostFocus
        Me.txt摿暿怳懼擔俁.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔俁, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔係.LostFocus
        Me.txt摿暿怳懼擔係.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔係, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俆.LostFocus
        Me.txt摿暿怳懼擔俆.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔俆, 2)
        End If

    End Sub
    Private Sub txt摿暿怳懼擔俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿怳懼擔俇.LostFocus
        Me.txt摿暿怳懼擔俇.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿怳懼擔俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿怳懼擔俇, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧侾.LostFocus
        Me.txt摿暿嵞怳懼寧侾.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧侾, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俀.LostFocus
        Me.txt摿暿嵞怳懼寧俀.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧俀, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俁.LostFocus
        Me.txt摿暿嵞怳懼寧俁.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧俁, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧係.LostFocus
        Me.txt摿暿嵞怳懼寧係.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧係, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俆.LostFocus
        Me.txt摿暿嵞怳懼寧俆.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧俆, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼寧俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼寧俇.LostFocus
        Me.txt摿暿嵞怳懼寧俇.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼寧俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼寧俇, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔侾.LostFocus
        Me.txt摿暿嵞怳懼擔侾.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔侾, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俀.LostFocus
        Me.txt摿暿嵞怳懼擔俀.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔俀, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俁.LostFocus
        Me.txt摿暿嵞怳懼擔俁.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔俁, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔係.LostFocus
        Me.txt摿暿嵞怳懼擔係.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔係, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俆.LostFocus
        Me.txt摿暿嵞怳懼擔俆.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔俆, 2)
        End If

    End Sub
    Private Sub txt摿暿嵞怳懼擔俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt摿暿嵞怳懼擔俇.LostFocus
        Me.txt摿暿嵞怳懼擔俇.BackColor = System.Drawing.Color.White
        If Trim(txt摿暿嵞怳懼擔俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt摿暿嵞怳懼擔俇, 2)
        End If

    End Sub
    '悘帪
    Private Sub txt悘帪怳懼寧侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧侾.LostFocus
        Me.txt悘帪怳懼寧侾.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧侾, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼寧俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俀.LostFocus
        Me.txt悘帪怳懼寧俀.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧俀, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼寧俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俁.LostFocus
        Me.txt悘帪怳懼寧俁.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧俁, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼寧係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧係.LostFocus
        Me.txt悘帪怳懼寧係.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧係, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼寧俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俆.LostFocus
        Me.txt悘帪怳懼寧俆.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧俆, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼寧俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼寧俇.LostFocus
        Me.txt悘帪怳懼寧俇.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼寧俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼寧俇, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔侾_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔侾.LostFocus
        Me.txt悘帪怳懼擔侾.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔侾.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔侾, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔俀_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俀.LostFocus
        Me.txt悘帪怳懼擔俀.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔俀.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔俀, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔俁_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俁.LostFocus
        Me.txt悘帪怳懼擔俁.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔俁.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔俁, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔係_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔係.LostFocus
        Me.txt悘帪怳懼擔係.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔係.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔係, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔俆_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俆.LostFocus
        Me.txt悘帪怳懼擔俆.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔俆.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔俆, 2)
        End If

    End Sub
    Private Sub txt悘帪怳懼擔俇_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txt悘帪怳懼擔俇.LostFocus
        Me.txt悘帪怳懼擔俇.BackColor = System.Drawing.Color.White
        If Trim(txt悘帪怳懼擔俇.Text) <> "" Then
            '侽晅壛
            Call GFUNC_ZERO_ADD(txt悘帪怳懼擔俇, 2)
        End If

    End Sub
#End Region

#Region " KeyPress "
    '婎杮
    Private Sub txt懳徾擭搙_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt懳徾擭搙.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txtGAKKOU_CODE_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txtGAKKOU_CODE.KeyPress
        '妛峑僐乕僪偺KEY擖椡惂屼
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '擭娫
    Private Sub txt係寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt係寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俆寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俆寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俇寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俇寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俈寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俈寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俉寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俉寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俋寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俋寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾侽寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾侽寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾侾寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾侾寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾俀寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾俀寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俀寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俀寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俁寧怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俁寧怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt係寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt係寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俆寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俆寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俇寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俇寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俈寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俈寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俉寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俉寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俋寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俋寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾侽寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾侽寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾侾寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾侾寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾俀寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾俀寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt侾寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt侾寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俀寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俀寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt俁寧嵞怳懼擔_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt俁寧嵞怳懼擔.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '摿暿
    Private Sub txt摿暿惪媮寧侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿惪媮寧俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿惪媮寧俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿惪媮寧係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿惪媮寧俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿惪媮寧俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿惪媮寧俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)
    End Sub
    Private Sub txt摿暿怳懼寧侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼寧俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼寧俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼寧係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼寧俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼寧俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼寧俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿怳懼擔俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿怳懼擔俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼寧俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt摿暿嵞怳懼擔俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    '悘帪
    Private Sub txt悘帪怳懼寧侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼寧俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼寧俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼寧係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼寧俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼寧俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼寧俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔侾_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔侾.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔俀_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔俀.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔俁_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔俁.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔係_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔係.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔俆_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔俆.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
    Private Sub txt悘帪怳懼擔俇_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles txt悘帪怳懼擔俇.KeyPress
        '擖椡悢抣僠僃僢僋
        Call GFUNC_KEYCHECK(Me, e, 1)

    End Sub
#End Region

#Region " KeyUp "
    '妛峑忣曬
    Private Sub txt懳徾擭搙_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt懳徾擭搙.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt懳徾擭搙)

    End Sub
    Private Sub txtGAKKOU_CODE_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtGAKKOU_CODE.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txtGAKKOU_CODE)

    End Sub
    '擭娫僗働僕儏乕儖
    Private Sub txt係寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt係寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt係寧怳懼擔)

    End Sub
    Private Sub txt俆寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俆寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俆寧怳懼擔)

    End Sub
    Private Sub txt俇寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俇寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俇寧怳懼擔)

    End Sub
    Private Sub txt俈寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俈寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俈寧怳懼擔)

    End Sub
    Private Sub txt俉寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俉寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俉寧怳懼擔)

    End Sub
    Private Sub txt俋寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俋寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俋寧怳懼擔)

    End Sub
    Private Sub txt侾侽寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾侽寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾侽寧怳懼擔)

    End Sub
    Private Sub txt侾侾寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾侾寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾侾寧怳懼擔)

    End Sub
    Private Sub txt侾俀寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾俀寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾俀寧怳懼擔)

    End Sub
    Private Sub txt侾寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾寧怳懼擔)

    End Sub
    Private Sub txt俀寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俀寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俀寧怳懼擔)

    End Sub
    Private Sub txt俁寧怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俁寧怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俁寧怳懼擔)

    End Sub
    Private Sub txt係寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt係寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt係寧嵞怳懼擔)

    End Sub
    Private Sub txt俆寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俆寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俆寧嵞怳懼擔)

    End Sub
    Private Sub txt俇寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俇寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俇寧嵞怳懼擔)

    End Sub
    Private Sub txt俈寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俈寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俈寧嵞怳懼擔)

    End Sub
    Private Sub txt俉寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俉寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俉寧嵞怳懼擔)

    End Sub
    Private Sub txt俋寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俋寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俋寧嵞怳懼擔)

    End Sub
    Private Sub txt侾侽寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾侽寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾侽寧嵞怳懼擔)

    End Sub
    Private Sub txt侾侾寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾侾寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾侾寧嵞怳懼擔)

    End Sub
    Private Sub txt侾俀寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾俀寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾俀寧嵞怳懼擔)

    End Sub
    Private Sub txt侾寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt侾寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt侾寧嵞怳懼擔)

    End Sub
    Private Sub txt俀寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俀寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俀寧嵞怳懼擔)

    End Sub
    Private Sub txt俁寧嵞怳懼擔_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt俁寧嵞怳懼擔.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt俁寧嵞怳懼擔)

    End Sub
    '摿暿僗働僕儏乕儖
    Private Sub txt摿暿惪媮寧侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧侾)

    End Sub
    Private Sub txt摿暿惪媮寧俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧俀)

    End Sub
    Private Sub txt摿暿惪媮寧俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧俁)

    End Sub
    Private Sub txt摿暿惪媮寧係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧係)

    End Sub
    Private Sub txt摿暿惪媮寧俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧俆)

    End Sub
    Private Sub txt摿暿惪媮寧俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿惪媮寧俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿惪媮寧俇)

    End Sub
    Private Sub txt摿暿怳懼寧侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧侾)

    End Sub
    Private Sub txt摿暿怳懼寧俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧俀)

    End Sub
    Private Sub txt摿暿怳懼寧俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧俁)

    End Sub
    Private Sub txt摿暿怳懼寧係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧係)

    End Sub
    Private Sub txt摿暿怳懼寧俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧俆)

    End Sub
    Private Sub txt摿暿怳懼寧俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼寧俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼寧俇)

    End Sub
    Private Sub txt摿暿怳懼擔侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔侾)

    End Sub
    Private Sub txt摿暿怳懼擔俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔俀)

    End Sub
    Private Sub txt摿暿怳懼擔俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔俁)

    End Sub
    Private Sub txt摿暿怳懼擔係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔係)

    End Sub
    Private Sub txt摿暿怳懼擔俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔俆)

    End Sub
    Private Sub txt摿暿怳懼擔俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿怳懼擔俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿怳懼擔俇)

    End Sub
    Private Sub txt摿暿嵞怳懼寧侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧侾)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧俀)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧俁)

    End Sub
    Private Sub txt摿暿嵞怳懼寧係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧係)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧俆)

    End Sub
    Private Sub txt摿暿嵞怳懼寧俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼寧俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼寧俇)

    End Sub
    Private Sub txt摿暿嵞怳懼擔侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔侾)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔俀)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔俁)

    End Sub
    Private Sub txt摿暿嵞怳懼擔係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔係)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔俆)

    End Sub
    Private Sub txt摿暿嵞怳懼擔俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt摿暿嵞怳懼擔俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt摿暿嵞怳懼擔俇)

    End Sub
    '悘帪僗働僕儏乕儖
    Private Sub txt悘帪怳懼寧侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧侾)

    End Sub
    Private Sub txt悘帪怳懼寧俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧俀)

    End Sub
    Private Sub txt悘帪怳懼寧俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧俁)

    End Sub
    Private Sub txt悘帪怳懼寧係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧係)

    End Sub
    Private Sub txt悘帪怳懼寧俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧俆)

    End Sub
    Private Sub txt悘帪怳懼寧俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼寧俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼寧俇)

    End Sub
    Private Sub txt悘帪怳懼擔侾_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔侾.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔侾)

    End Sub
    Private Sub txt悘帪怳懼擔俀_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔俀.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔俀)

    End Sub
    Private Sub txt悘帪怳懼擔俁_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔俁.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔俁)

    End Sub
    Private Sub txt悘帪怳懼擔係_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔係.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔係)

    End Sub
    Private Sub txt悘帪怳懼擔俆_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔俆.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔俆)

    End Sub
    Private Sub txt悘帪怳懼擔俇_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txt悘帪怳懼擔俇.KeyUp

        Call GSUB_NEXTFOCUS(Me, e, txt悘帪怳懼擔俇)

    End Sub
#End Region

#Region " CheckedChanged(CheckBox) "
    '摿暿僗働僕儏乕儖
    Private Sub chk侾_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk侾_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk侾_慡妛擭, _
                                           chk侾_侾妛擭, _
                                           chk侾_俀妛擭, _
                                           chk侾_俁妛擭, _
                                           chk侾_係妛擭, _
                                           chk侾_俆妛擭, _
                                           chk侾_俇妛擭, _
                                           chk侾_俈妛擭, _
                                           chk侾_俉妛擭, _
                                           chk侾_俋妛擭)

    End Sub
    Private Sub chk俀_慡妛擭_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俀_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk俀_慡妛擭, _
                                           chk俀_侾妛擭, _
                                           chk俀_俀妛擭, _
                                           chk俀_俁妛擭, _
                                           chk俀_係妛擭, _
                                           chk俀_俆妛擭, _
                                           chk俀_俇妛擭, _
                                           chk俀_俈妛擭, _
                                           chk俀_俉妛擭, _
                                           chk俀_俋妛擭)

    End Sub
    Private Sub chk俁_慡妛擭_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俁_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk俁_慡妛擭, _
                                           chk俁_侾妛擭, _
                                           chk俁_俀妛擭, _
                                           chk俁_俁妛擭, _
                                           chk俁_係妛擭, _
                                           chk俁_俆妛擭, _
                                           chk俁_俇妛擭, _
                                           chk俁_俈妛擭, _
                                           chk俁_俉妛擭, _
                                           chk俁_俋妛擭)

    End Sub
    Private Sub chk係_慡妛擭_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk係_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk係_慡妛擭, _
                                           chk係_侾妛擭, _
                                           chk係_俀妛擭, _
                                           chk係_俁妛擭, _
                                           chk係_係妛擭, _
                                           chk係_俆妛擭, _
                                           chk係_俇妛擭, _
                                           chk係_俈妛擭, _
                                           chk係_俉妛擭, _
                                           chk係_俋妛擭)

    End Sub
    Private Sub chk俆_慡妛擭_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俆_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk俆_慡妛擭, _
                                           chk俆_侾妛擭, _
                                           chk俆_俀妛擭, _
                                           chk俆_俁妛擭, _
                                           chk俆_係妛擭, _
                                           chk俆_俆妛擭, _
                                           chk俆_俇妛擭, _
                                           chk俆_俈妛擭, _
                                           chk俆_俉妛擭, _
                                           chk俆_俋妛擭)

    End Sub
    Private Sub chk俇_慡妛擭_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俇_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk俇_慡妛擭, _
                                           chk俇_侾妛擭, _
                                           chk俇_俀妛擭, _
                                           chk俇_俁妛擭, _
                                           chk俇_係妛擭, _
                                           chk俇_俆妛擭, _
                                           chk俇_俇妛擭, _
                                           chk俇_俈妛擭, _
                                           chk俇_俉妛擭, _
                                           chk俇_俋妛擭)

    End Sub
    '悘帪僗働僕儏乕儖
    Private Sub chk悘帪侾_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪侾_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪侾_慡妛擭, _
                                           chk悘帪侾_侾妛擭, _
                                           chk悘帪侾_俀妛擭, _
                                           chk悘帪侾_俁妛擭, _
                                           chk悘帪侾_係妛擭, _
                                           chk悘帪侾_俆妛擭, _
                                           chk悘帪侾_俇妛擭, _
                                           chk悘帪侾_俈妛擭, _
                                           chk悘帪侾_俉妛擭, _
                                           chk悘帪侾_俋妛擭)

    End Sub
    Private Sub chk悘帪俀_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪俀_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪俀_慡妛擭, _
                                           chk悘帪俀_侾妛擭, _
                                           chk悘帪俀_俀妛擭, _
                                           chk悘帪俀_俁妛擭, _
                                           chk悘帪俀_係妛擭, _
                                           chk悘帪俀_俆妛擭, _
                                           chk悘帪俀_俇妛擭, _
                                           chk悘帪俀_俈妛擭, _
                                           chk悘帪俀_俉妛擭, _
                                           chk悘帪俀_俋妛擭)

    End Sub
    Private Sub chk悘帪俁_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪俁_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪俁_慡妛擭, _
                                           chk悘帪俁_侾妛擭, _
                                           chk悘帪俁_俀妛擭, _
                                           chk悘帪俁_俁妛擭, _
                                           chk悘帪俁_係妛擭, _
                                           chk悘帪俁_俆妛擭, _
                                           chk悘帪俁_俇妛擭, _
                                           chk悘帪俁_俈妛擭, _
                                           chk悘帪俁_俉妛擭, _
                                           chk悘帪俁_俋妛擭)

    End Sub
    Private Sub chk悘帪係_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪係_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪係_慡妛擭, _
                                           chk悘帪係_侾妛擭, _
                                           chk悘帪係_俀妛擭, _
                                           chk悘帪係_俁妛擭, _
                                           chk悘帪係_係妛擭, _
                                           chk悘帪係_俆妛擭, _
                                           chk悘帪係_俇妛擭, _
                                           chk悘帪係_俈妛擭, _
                                           chk悘帪係_俉妛擭, _
                                           chk悘帪係_俋妛擭)

    End Sub
    Private Sub chk悘帪俆_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪俆_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪俆_慡妛擭, _
                                           chk悘帪俆_侾妛擭, _
                                           chk悘帪俆_俀妛擭, _
                                           chk悘帪俆_俁妛擭, _
                                           chk悘帪俆_係妛擭, _
                                           chk悘帪俆_俆妛擭, _
                                           chk悘帪俆_俇妛擭, _
                                           chk悘帪俆_俈妛擭, _
                                           chk悘帪俆_俉妛擭, _
                                           chk悘帪俆_俋妛擭)

    End Sub
    Private Sub chk悘帪俇_慡妛擭_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chk悘帪俇_慡妛擭.CheckedChanged

        Call PSUB_ZENGAKUNEN_CHKBOX_CNTROL(chk悘帪俇_慡妛擭, _
                                           chk悘帪俇_侾妛擭, _
                                           chk悘帪俇_俀妛擭, _
                                           chk悘帪俇_俁妛擭, _
                                           chk悘帪俇_係妛擭, _
                                           chk悘帪俇_俆妛擭, _
                                           chk悘帪俇_俇妛擭, _
                                           chk悘帪俇_俈妛擭, _
                                           chk悘帪俇_俉妛擭, _
                                           chk悘帪俇_俋妛擭)

    End Sub
#End Region

#Region " CheckedChanged "

    Private Sub chk係寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk係寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk係寧怳懼擔.Checked = False Then
            chk係寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk係寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk係寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk係寧嵞怳懼擔.Checked = True Then
            chk係寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俆寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俆寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俆寧怳懼擔.Checked = False Then
            chk俆寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俆寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俆寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俆寧嵞怳懼擔.Checked = True Then
            chk俆寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俇寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俇寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俇寧怳懼擔.Checked = False Then
            chk俇寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俇寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俇寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俇寧嵞怳懼擔.Checked = True Then
            chk俇寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俈寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俈寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俈寧怳懼擔.Checked = False Then
            chk俈寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俈寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俈寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俈寧嵞怳懼擔.Checked = True Then
            chk俈寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俉寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俉寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俉寧怳懼擔.Checked = False Then
            chk俉寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俉寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俉寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俉寧嵞怳懼擔.Checked = True Then
            chk俉寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俋寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俋寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俋寧怳懼擔.Checked = False Then
            chk俋寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俋寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俋寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俋寧嵞怳懼擔.Checked = True Then
            chk俋寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk侾侽寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾侽寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾侽寧怳懼擔.Checked = False Then
            chk侾侽寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk侾侽寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾侽寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾侽寧嵞怳懼擔.Checked = True Then
            chk侾侽寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk侾侾寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾侾寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾侾寧怳懼擔.Checked = False Then
            chk侾侾寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk侾侾寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾侾寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾侾寧嵞怳懼擔.Checked = True Then
            chk侾侾寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk侾俀寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾俀寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾俀寧怳懼擔.Checked = False Then
            chk侾俀寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk侾俀寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾俀寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾俀寧嵞怳懼擔.Checked = True Then
            chk侾俀寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk侾寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾寧怳懼擔.Checked = False Then
            chk侾寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk侾寧嵞怳懼擔_CheckStateChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk侾寧嵞怳懼擔.CheckStateChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk侾寧嵞怳懼擔.Checked = True Then
            chk侾寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俀寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俀寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俀寧怳懼擔.Checked = False Then
            chk俀寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俀寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俀寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俀寧嵞怳懼擔.Checked = True Then
            chk俀寧怳懼擔.Checked = True
        End If
    End Sub

    Private Sub chk俁寧怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俁寧怳懼擔.CheckedChanged
        '2006/11/22丂弶怳僠僃僢僋傪奜偟偨偲偒丄嵞怳僠僃僢僋傕奜偡乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俁寧怳懼擔.Checked = False Then
            chk俁寧嵞怳懼擔.Checked = False
        End If
    End Sub

    Private Sub chk俁寧嵞怳懼擔_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chk俁寧嵞怳懼擔.CheckedChanged
        '2006/11/22丂嵞怳僠僃僢僋傪擖傟偨偲偒丄弶怳僠僃僢僋傕擖傟傞乮嵞怳偺傒偺搊榐傪杊偖偨傔乯
        If chk俁寧嵞怳懼擔.Checked = True Then
            chk俁寧怳懼擔.Checked = True
        End If
    End Sub

#End Region

#Region " Private Sub(嫟捠)"
    Private Sub PSUB_FORMAT_ALL()

        '婎杮忣曬晹弶婜壔
        Call PSUB_KIHON_FORMAT()

        '擭娫僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_NENKAN_FORMAT()

        '摿暿僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_TOKUBETU_FORMAT()

        '悘帪僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_ZUIJI_FORMAT()

        '儃僞儞Enabled弶婜忬懺
        Call PSUB_BUTTON_Enable()

    End Sub

    Private Sub PSUB_BUTTON_Enable(Optional ByVal pIndex As Integer = 0)

        Select Case pIndex
            Case 0
                btnAction.Enabled = True
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt懳徾擭搙.Enabled = True
            Case 1
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUpdate.Enabled = True
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = False
                cmbGakkouName.Enabled = False
                cmbKana.Enabled = False
                txt懳徾擭搙.Enabled = False
            Case 2
                btnAction.Enabled = False '2007/02/15
                btnFind.Enabled = True
                btnUpdate.Enabled = False
                btnEraser.Enabled = True
                txtGAKKOU_CODE.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt懳徾擭搙.Enabled = True
        End Select

    End Sub

    Private Sub PSUB_KIHON_FORMAT()

        txt懳徾擭搙.Enabled = True
        'txt懳徾擭搙.Text = ""

        txtGAKKOU_CODE.Enabled = True
        txtGAKKOU_CODE.Text = ""

        lab妛峑柤.Text = ""

        '媥擔儕僗僩儃僢僋僗弶婜壔
        lst媥擔.Items.Clear()

        '妛峑専嶕乮僇僫乯
        cmbKana.SelectedIndex = -1

        '捛壛 2007/02/15
        '妛峑僐儞儃愝掕乮慡妛峑乯
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "妛峑僐儞儃愝掕", "幐攕", "僐儞儃儃僢僋僗愝掕(cmbGAKKOUNAME)")
            MessageBox.Show("妛峑柤僐儞儃儃僢僋僗愝掕偱僄儔乕偑敪惗偟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        '妛峑専嶕乮妛峑柤乯
        cmbGakkouName.SelectedIndex = -1

    End Sub

    '========================================
    '僗働僕儏乕儖儅僗僞搊榐
    '2006/11/30丂妛擭僼儔僌傪曄峏
    '========================================
    Private Function PSUB_INSERT_G_SCHMAST_SQL() As String

        Dim sql As String = ""

        '奺庬梊掕擔偺嶼弌
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        '僗働僕儏乕儖嶌惉懳徾偺庢堷愭僐乕僪傪拪弌
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR怳懼擔), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR怳懼擔)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE '僨僶僢僌梡丠

        '2010/10/21 宊栺怳懼擔懳墳 偙偙偐傜
        If STR宊栺怳懼擔 = "" OrElse STR宊栺怳懼擔.Length <> 8 Then
            '堷悢偑側偄応崌偼幚怳懼擔傪愝掕
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR宊栺怳懼擔
        End If
        '2010/10/21 宊栺怳懼擔懳墳 偙偙傑偱

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '柧嵶嶌惉梊掕擔嶼弌
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR怳懼擔, STR_JIFURI_CHECK, "-")           '僠僃僢僋梊掕擔嶼弌
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR怳懼擔, STR_JIFURI_HAISIN, "-")       '怳懼僨乕僞嶌惉梊掕擔嶼弌
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '晄擻寢壥峏怴梊掕擔嶼弌
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '帒嬥寛嵪梊掕擔嶼弌
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '曉娨梊掕擔

        'INSERT暥嶌惉
        sql += "INSERT INTO G_SCHMAST "
        sql += " VALUES ( "
        '妛峑僐乕僪
        sql += "'" & GAKKOU_INFO.GAKKOU_CODE & "'"
        '惪媮擭寧
        sql += ",'" & STR惪媮擭寧 & "'"
        '僗働僕儏乕儖嬫暘
        sql += ",'" & STR僗働嬫暘 & "'"
        '怳懼嬫暘
        sql += ",'" & STR怳懼嬫暘 & "'"
        '怳懼擔
        sql += ",'" & STR怳懼擔 & "'"
        '嵞怳懼擔
        sql += ",'" & STR嵞怳懼擔 & "'"
        '妛擭侾
        sql += ",'" & STR侾妛擭 & "'"
        '妛擭俀
        sql += ",'" & STR俀妛擭 & "'"
        '妛擭俁
        sql += ",'" & STR俁妛擭 & "'"
        '妛擭係
        sql += ",'" & STR係妛擭 & "'"
        '妛擭俆
        sql += ",'" & STR俆妛擭 & "'"
        '妛擭俇
        sql += ",'" & STR俇妛擭 & "'"
        '妛擭俈
        sql += ",'" & STR俈妛擭 & "'"
        '妛擭俉
        sql += ",'" & STR俉妛擭 & "'"
        '妛擭俋
        sql += ",'" & STR俋妛擭 & "'"
        '埾戸幰僐乕僪
       
        '2011/06/16 昗弨斉廋惓 埾戸幰僐乕僪偺壓侾寘曄峏傪峴傢側偄------------------START
        sql += ",'" & GAKKOU_INFO.ITAKU_CODE & "'"
        'Select Case STR怳懼嬫暘
        '    Case "0"
        '        sql += ",'" & "0" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "1"
        '        sql += ",'" & "1" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "2"
        '        sql += ",'" & "2" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        '    Case "3"
        '        sql += ",'" & "3" + GAKKOU_INFO.ITAKU_CODE.Substring(1, 9) & "'"
        'End Select
        '2011/06/16 昗弨斉廋惓 埾戸幰僐乕僪偺壓侾寘曄峏傪峴傢側偄------------------END
        '庢埖嬥梈婡娭
        sql += ",'" & GAKKOU_INFO.TKIN_CODE & "'"
        '庢埖巟揦
        sql += ",'" & GAKKOU_INFO.TSIT_CODE & "'"
        '攠懱僐乕僪 
        sql += ",'" & GAKKOU_INFO.BAITAI_CODE & "'"
        '庤悢椏嬫暘 
        sql += ",'" & GAKKOU_INFO.TESUUTYO_KBN & "'"
        '柧嵶嶌惉梊掕擔
        sql += "," & "'" & ENTRY_Y_DATE & "'"
        '柧嵶嶌惉擔
        sql += "," & "'00000000'"
        '僠僃僢僋梊掕擔
        sql += "," & "'" & CHECK_Y_DATE & "'"
        '僠僃僢僋擔
        sql += "," & "'00000000'"
        '怳懼僨乕僞嶌惉梊掕擔
        sql += "," & "'" & DATA_Y_DATE & "'"
        '怳懼僨乕僞嶌惉擔
        sql += "," & "'00000000'"
        '晄擻寢壥峏怴梊掕擔
        sql += "," & "'" & FUNOU_Y_DATE & "'"
        '晄擻寢壥峏怴擔
        sql += "," & "'00000000'"
        '曉娨梊掕擔
        sql += "," & "'" & HENKAN_Y_DATE & "'"
        '曉娨擔
        sql += "," & "'00000000'"
        '寛嵪梊掕擔
        sql += "," & "'" & KESSAI_Y_DATE & "'"
        '寛嵪擔
        sql += "," & "'00000000'"
        '柧嵶嶌惉嵪僼儔僌
        sql += "," & "'" & strENTRI_FLG & "'"
        '嬥妟妋擣嵪僼儔僌
        sql += "," & "'" & strCHECK_FLG & "'"
        '怳懼僨乕僞嶌惉嵪僼儔僌
        sql += "," & "'" & strDATA_FLG & "'"
        '晄擻寢壥峏怴嵪僼儔僌
        sql += "," & "'" & strFUNOU_FLG & "'"
        '曉娨嵪僼儔僌
        sql += "," & "'" & strHENKAN_FLG & "'"
        '嵞怳僨乕僞嶌惉嵪僼儔僌
        sql += "," & "'" & strSAIFURI_FLG & "'"
        '寛嵪嵪僼儔僌
        sql += "," & "'" & strKESSAI_FLG & "'"
        '拞抐僼儔僌
        sql += "," & "'" & strTYUUDAN_FLG & "'"
        '張棟審悢
        sql += "," & lngSYORI_KEN
        '張棟嬥妟
        sql += "," & dblSYORI_KIN
        '庤悢椏
        sql += "," & 0
        '庤悢椏侾
        sql += "," & 0
        '庤悢椏俀
        sql += "," & 0
        '庤悢椏俁
        sql += "," & 0
        '怳懼嵪審悢
        sql += "," & lngFURI_KEN
        '怳懼嵪嬥妟
        sql += "," & dblFURI_KIN
        '晄擻審悢
        sql += "," & lngFUNOU_KEN
        '晄擻嬥妟
        sql += "," & dblFUNOU_KIN
        '嶌惉擔晅
        sql += "," & "'" & Str_SyoriDate(0) & "'"
        '僞僀儉僗僞儞僾
        sql += "," & "'" & Str_SyoriDate(1) & "'"
        '梊旛侾
        sql += "," & "'" & STR擭娫擖椡怳懼擔 & "'"
        '梊旛俀
        sql += "," & "'" & Space(30) & "'"
        '梊旛俁
        sql += "," & "'" & Space(30) & "'"
        '梊旛係
        sql += "," & "'" & Space(30) & "'"
        '梊旛俆
        sql += "," & "'" & Space(30) & "'"
        '梊旛俇
        sql += "," & "'" & Space(30) & "'"
        '梊旛俈
        sql += "," & "'" & Space(30) & "'"
        '梊旛俉
        sql += "," & "'" & Space(30) & "'"
        '梊旛俋
        sql += "," & "'" & Space(30) & "'"
        '梊旛侾侽
        sql += "," & "'" & Space(30) & "')"

        Return sql

    End Function

    '===================================================
    'PSUB_UPDATE_G_SCHMAST_SQL
    'UPDATE 2006/11/30丂擭娫丒摿暿丒悘帪偦傟偧傟偵懳墳
    '===================================================
    Private Function PSUB_UPDATE_G_SCHMAST_SQL(ByVal strJoken_Furi_Date As String, ByVal strJoken_SFuri_Date As String) As String
        'strJoken_Furi_Date 丗峏怴慜怳懼擔
        'strJoken_SFuri_Date丗峏怴慜嵞怳擔

        Dim sql As String = ""

        '峏怴慜嵞怳擔偑嬻敀偺応崌偼0杽傔偡傞
        If Trim(strJoken_SFuri_Date) = "" Then
            strJoken_SFuri_Date = "00000000"
        End If

        '奺庬梊掕擔偺嶼弌
        Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
        Call CLS.SetKyuzituInformation()

        CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

        '僗働僕儏乕儖嶌惉懳徾偺庢堷愭僐乕僪傪拪弌
        CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(STR怳懼擔), strGakkouCode, "01")

        CLS.SCH.FURI_DATE = GCom.SET_DATE(STR怳懼擔)
        If CLS.SCH.FURI_DATE = "00000000" Then
        Else
            CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
        End If

        Dim strFURI_DATE As String = CLS.SCH.FURI_DATE '僨僶僢僌梡丠

        '2010/10/21 宊栺怳懼擔懳墳 偙偙偐傜
        If STR宊栺怳懼擔 = "" OrElse STR宊栺怳懼擔.Length <> 8 Then
            '堷悢偑側偄応崌偼幚怳懼擔傪愝掕
            CLS.SCH.KFURI_DATE = CLS.SCH.FURI_DATE
        Else
            CLS.SCH.KFURI_DATE = STR宊栺怳懼擔
        End If
        '2010/10/21 宊栺怳懼擔懳墳 偙偙傑偱

        Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

        Dim ENTRY_Y_DATE As String = "00000000"                                                   '柧嵶嶌惉梊掕擔嶼弌
        Dim CHECK_Y_DATE As String = fn_GetEigyoubi(STR怳懼擔, STR_JIFURI_CHECK, "-")           '僠僃僢僋梊掕擔嶼弌
        Dim DATA_Y_DATE As String = fn_GetEigyoubi(STR怳懼擔, STR_JIFURI_HAISIN, "-")       '怳懼僨乕僞嶌惉梊掕擔嶼弌
        Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '晄擻寢壥峏怴梊掕擔嶼弌
        Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '帒嬥寛嵪梊掕擔嶼弌
        Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '曉娨梊掕擔

        'UPDATE暥嶌惉
        sql = " UPDATE  G_SCHMAST"
        sql += " SET "
        sql += " FURI_DATE_S = '" & STR怳懼擔 & "'," '   2006/11/22丂怳懼擔
        sql += " SFURI_DATE_S = '" & STR嵞怳懼擔 & "'," '2006/11/22丂嵞怳擔
        sql += " GAKUNEN1_FLG_S  ='" & STR侾妛擭 & "',"
        sql += " GAKUNEN2_FLG_S  ='" & STR俀妛擭 & "',"
        sql += " GAKUNEN3_FLG_S  ='" & STR俁妛擭 & "',"
        sql += " GAKUNEN4_FLG_S  ='" & STR係妛擭 & "',"
        sql += " GAKUNEN5_FLG_S  ='" & STR俆妛擭 & "',"
        sql += " GAKUNEN6_FLG_S  ='" & STR俇妛擭 & "',"
        sql += " GAKUNEN7_FLG_S  ='" & STR俈妛擭 & "',"
        sql += " GAKUNEN8_FLG_S  ='" & STR俉妛擭 & "',"
        sql += " GAKUNEN9_FLG_S  ='" & STR俋妛擭 & "',"
        sql += " SYORI_KEN_S =" & lngSYORI_KEN & ","
        sql += " SYORI_KIN_S =" & dblSYORI_KIN & ","
        sql += " FURI_KEN_S =" & lngFURI_KEN & ","
        sql += " FURI_KIN_S =" & dblFURI_KIN & ","
        sql += " FUNOU_KEN_S =" & lngFUNOU_KEN & ","
        sql += " FUNOU_KIN_S =" & dblFUNOU_KIN & ","
        sql += " YOBI1_S = '" & STR擭娫擖椡怳懼擔 & "',"
        '奺梊掕擔峏怴 2007/12/13
        sql += " ENTRI_YDATE_S ='" & ENTRY_Y_DATE & "',"
        sql += " CHECK_YDATE_S ='" & CHECK_Y_DATE & "',"
        sql += " DATA_YDATE_S ='" & DATA_Y_DATE & "',"
        sql += " FUNOU_YDATE_S ='" & FUNOU_Y_DATE & "',"
        sql += " HENKAN_YDATE_S ='" & HENKAN_Y_DATE & "',"
        sql += " KESSAI_YDATE_S ='" & KESSAI_Y_DATE & "'"
        sql += " WHERE"
        sql += " GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'"
        sql += " AND"
        sql += " NENGETUDO_S ='" & STR惪媮擭寧 & "'"
        sql += " AND"
        sql += " SCH_KBN_S ='" & STR僗働嬫暘 & "'"
        sql += " AND"
        sql += " FURI_KBN_S ='" & STR怳懼嬫暘 & "'"
        sql += " AND"

        '2006/11/22丂忦審傪媽僨乕僞偵廋惓
        'sql += " FURI_DATE_S ='" & STR怳懼擔 & "'"
        'sql += " FURI_DATE_S ='" & str媽怳懼擔(int媽怳懼俬俢) & "'"
        sql += " FURI_DATE_S = '" & strJoken_Furi_Date & "'"
        sql += " AND"
        'sql += " SFURI_DATE_S ='" & STR嵞怳懼擔 & "'"
        'sql += " SFURI_DATE_S ='" & str媽嵞怳擔(int媽怳懼俬俢) & "'"
        sql += " SFURI_DATE_S = '" & strJoken_SFuri_Date & "'"

        Return sql

    End Function

    Private Sub PSUB_ZENGAKUNEN_CHKBOX_CNTROL(ByVal chkBOXALL As CheckBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox)

        If chkBOXALL.Checked = True Then
            '奺妛擭偺僠僃僢僋倧倖倖
            chkBOX1.Checked = False
            chkBOX2.Checked = False
            chkBOX3.Checked = False
            chkBOX4.Checked = False
            chkBOX5.Checked = False
            chkBOX6.Checked = False
            chkBOX7.Checked = False
            chkBOX8.Checked = False
            chkBOX9.Checked = False
            '奺妛擭偺僠僃僢僋儃僢僋僗巊梡晄壜 
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            '奺妛擭偺僠僃僢僋儃僢僋僗巊梡壜 
            chkBOX1.Enabled = True
            chkBOX2.Enabled = True
            chkBOX3.Enabled = True
            chkBOX4.Enabled = True
            chkBOX5.Enabled = True
            chkBOX6.Enabled = True
            chkBOX7.Enabled = True
            chkBOX8.Enabled = True
            chkBOX9.Enabled = True
            '2006/10/12丂嵟崅妛擭僠僃僢僋
            PSUB_TGAKUNEN_CHK()
            PSUB_ZGAKUNEN_CHK()
        End If
    End Sub

    Private Sub PSUB_SANSYOU_FOCUS()

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt懳徾擭搙.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt懳徾擭搙.Text)) + 1) & "03'")

        If oraReader.DataReader(sql) = True Then
            btnAction.Enabled = False
            btnFind.Enabled = True
            btnFind.Focus()
        Else '捛壛 2007/02/15
            btnFind.Enabled = False
            btnAction.Enabled = True
            btnAction.Focus()
        End If

        oraReader.Close()

    End Sub
    '2006/10/25 曄悢僋儕傾
    Public Sub sb_HENSU_CLEAR()
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        strSAIFURI_DEF = "00000000" '捠忢僗働僕儏乕儖偺嵞怳擔

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0
    End Sub

    '==========================================
    '曄峏偝傟偨崁栚傪僠僃僢僋  2006/11/30
    '==========================================
    Private Sub PSUB_Kousin_Check()

        '--------------------------------------
        '奺棑偺抣傪峔憿懱偵擖椡乮峏怴帪偺傕偺乯
        '--------------------------------------
        Call PSUB_NENKAN_GET(NENKAN_SCHINFO)
        Call PSUB_TOKUBETU_GET(TOKUBETU_SCHINFO)
        Call PSUB_ZUIJI_GET(ZUIJI_SCHINFO)

        '嶲徠帪偲峏怴帪偺崁栚傪斾傋丄曄峏偑偁偭偨傕偺偺峏怴僼儔僌傪棫偰傞

        For i As Integer = 1 To 12
            '--------------------------------------
            '擭娫僗働僕儏乕儖僠僃僢僋
            '--------------------------------------
            If NENKAN_SCHINFO(i).Furikae_Check = SYOKI_NENKAN_SCHINFO(i).Furikae_Check And _
               NENKAN_SCHINFO(i).Furikae_Date = SYOKI_NENKAN_SCHINFO(i).Furikae_Date And _
               NENKAN_SCHINFO(i).Furikae_Day = SYOKI_NENKAN_SCHINFO(i).Furikae_Day And _
               NENKAN_SCHINFO(i).Furikae_Enabled = SYOKI_NENKAN_SCHINFO(i).Furikae_Enabled And _
               NENKAN_SCHINFO(i).SaiFurikae_Check = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Check And _
               NENKAN_SCHINFO(i).SaiFurikae_Date = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Date And _
               NENKAN_SCHINFO(i).SaiFurikae_Day = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day And _
               NENKAN_SCHINFO(i).SaiFurikae_Enabled = SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Enabled Then

                bln擭娫峏怴(i) = False '曄峏側偟
            Else
                bln擭娫峏怴(i) = True ' 曄峏偁傝
            End If
        Next

        For i As Integer = 1 To 6
            '--------------------------------------
            '摿暿僗働僕儏乕儖僠僃僢僋
            '--------------------------------------
            '2006/12/12丂堦晹捛壛丗擖椡偑晄懌偟偰偄偨応崌丄峏怴偟側偄
            If (TOKUBETU_SCHINFO(i).Seikyu_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki And _
               TOKUBETU_SCHINFO(i).Furikae_Date = SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki And _
               TOKUBETU_SCHINFO(i).SaiFurikae_Date = SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date And _
               TOKUBETU_SCHINFO(i).SiyouGakunen1_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen2_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen3_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen4_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen5_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen6_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen7_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen8_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunen9_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check And _
               TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((TOKUBETU_SCHINFO(i).Furikae_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date = "")) Or _
               ((TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "") Or _
               (TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date = "")) Then

                bln摿暿峏怴(i) = False '曄峏側偟
            Else
                bln摿暿峏怴(i) = True ' 曄峏偁傝
            End If

            '--------------------------------------
            '悘帪僗働僕儏乕儖僠僃僢僋
            '--------------------------------------
            '2006/12/12丂堦晹捛壛丗擖椡偑晄懌偟偰偄偨応崌丄峏怴偟側偄
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln悘帪峏怴(i) = False '曄峏側偟
            Else
                bln悘帪峏怴(i) = True ' 曄峏偁傝
            End If
        Next

    End Sub

    '夋柺昞帵帪戅旔丂2006/12/04
    Public Sub sb_SANSYOU_SET()
        '擭娫弶怳
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾寧怳懼擔.Text.Trim = "" Then
        If lab侾寧怳懼擔.Text.Trim = "" Or chk侾寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(1) = ""
        Else
            strSYOFURI_NENKAN(1) = Replace(lab侾寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俀寧怳懼擔.Text.Trim = "" Then
        If lab俀寧怳懼擔.Text.Trim = "" Or chk俀寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(2) = ""
        Else
            strSYOFURI_NENKAN(2) = Replace(lab俀寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俁寧怳懼擔.Text.Trim = "" Then
        If lab俁寧怳懼擔.Text.Trim = "" Or chk俁寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(3) = ""
        Else
            strSYOFURI_NENKAN(3) = Replace(lab俁寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab係寧怳懼擔.Text.Trim = "" Then
        If lab係寧怳懼擔.Text.Trim = "" Or chk係寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(4) = ""
        Else
            strSYOFURI_NENKAN(4) = Replace(lab係寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俆寧怳懼擔.Text.Trim = "" Then
        If lab俆寧怳懼擔.Text.Trim = "" Or chk俆寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(5) = ""
        Else
            strSYOFURI_NENKAN(5) = Replace(lab俆寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俇寧怳懼擔.Text.Trim = "" Then
        If lab俇寧怳懼擔.Text.Trim = "" Or chk俇寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(6) = ""
        Else
            strSYOFURI_NENKAN(6) = Replace(lab俇寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俈寧怳懼擔.Text.Trim = "" Then
        If lab俈寧怳懼擔.Text.Trim = "" Or chk俈寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(7) = ""
        Else
            strSYOFURI_NENKAN(7) = Replace(lab俈寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俉寧怳懼擔.Text.Trim = "" Then
        If lab俉寧怳懼擔.Text.Trim = "" Or chk俉寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(8) = ""
        Else
            strSYOFURI_NENKAN(8) = Replace(lab俉寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俋寧怳懼擔.Text.Trim = "" Then
        If lab俋寧怳懼擔.Text.Trim = "" Or chk俋寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(9) = ""
        Else
            strSYOFURI_NENKAN(9) = Replace(lab俋寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侽寧怳懼擔.Text.Trim = "" Then
        If lab侾侽寧怳懼擔.Text.Trim = "" Or chk侾侽寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(10) = ""
        Else
            strSYOFURI_NENKAN(10) = Replace(lab侾侽寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侾寧怳懼擔.Text.Trim = "" Then
        If lab侾侾寧怳懼擔.Text.Trim = "" Or chk侾侾寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(11) = ""
        Else
            strSYOFURI_NENKAN(11) = Replace(lab侾侾寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾俀寧怳懼擔.Text.Trim = "" Then
        If lab侾俀寧怳懼擔.Text.Trim = "" Or chk侾俀寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN(12) = ""
        Else
            strSYOFURI_NENKAN(12) = Replace(lab侾俀寧怳懼擔.Text, "/", "")
        End If
        '擭娫嵞怳
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾寧嵞怳懼擔.Text.Trim = "" Or chk侾寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(1) = ""
        Else
            strSAIFURI_NENKAN(1) = Replace(lab侾寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俀寧嵞怳懼擔.Text.Trim = "" Then
        If lab俀寧嵞怳懼擔.Text.Trim = "" Or chk俀寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(2) = ""
        Else
            strSAIFURI_NENKAN(2) = Replace(lab俀寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俁寧嵞怳懼擔.Text.Trim = "" Then
        If lab俁寧嵞怳懼擔.Text.Trim = "" Or chk俁寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(3) = ""
        Else
            strSAIFURI_NENKAN(3) = Replace(lab俁寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab係寧嵞怳懼擔.Text.Trim = "" Then
        If lab係寧嵞怳懼擔.Text.Trim = "" Or chk係寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(4) = ""
        Else
            strSAIFURI_NENKAN(4) = Replace(lab係寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俆寧嵞怳懼擔.Text.Trim = "" Then
        If lab俆寧嵞怳懼擔.Text.Trim = "" Or chk俆寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(5) = ""
        Else
            strSAIFURI_NENKAN(5) = Replace(lab俆寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俇寧嵞怳懼擔.Text.Trim = "" Then
        If lab俇寧嵞怳懼擔.Text.Trim = "" Or chk俇寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(6) = ""
        Else
            strSAIFURI_NENKAN(6) = Replace(lab俇寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俈寧嵞怳懼擔.Text.Trim = "" Then
        If lab俈寧嵞怳懼擔.Text.Trim = "" Or chk俈寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(7) = ""
        Else
            strSAIFURI_NENKAN(7) = Replace(lab俈寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俉寧嵞怳懼擔.Text.Trim = "" Then
        If lab俉寧嵞怳懼擔.Text.Trim = "" Or chk俉寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(8) = ""
        Else
            strSAIFURI_NENKAN(8) = Replace(lab俉寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俋寧嵞怳懼擔.Text.Trim = "" Then
        If lab俋寧嵞怳懼擔.Text.Trim = "" Or chk俋寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(9) = ""
        Else
            strSAIFURI_NENKAN(9) = Replace(lab俋寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侽寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾侽寧嵞怳懼擔.Text.Trim = "" Or chk侾侽寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(10) = ""
        Else
            strSAIFURI_NENKAN(10) = Replace(lab侾侽寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侾寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾侾寧嵞怳懼擔.Text.Trim = "" Or chk侾侾寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(11) = ""
        Else
            strSAIFURI_NENKAN(11) = Replace(lab侾侾寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾俀寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾俀寧嵞怳懼擔.Text.Trim = "" Or chk侾俀寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN(12) = ""
        Else
            strSAIFURI_NENKAN(12) = Replace(lab侾俀寧嵞怳懼擔.Text, "/", "")
        End If
        '摿暿弶怳
        strSYOFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧侾.Text) & txt摿暿怳懼擔侾.Text
        strSYOFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俀.Text) & txt摿暿怳懼擔俀.Text
        strSYOFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俁.Text) & txt摿暿怳懼擔俁.Text
        strSYOFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧係.Text) & txt摿暿怳懼擔係.Text
        strSYOFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俆.Text) & txt摿暿怳懼擔俆.Text
        strSYOFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俇.Text) & txt摿暿怳懼擔俇.Text
        '摿暿嵞怳
        strSAIFURI_TOKUBETU(1) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧侾.Text) & txt摿暿嵞怳懼擔侾.Text
        strSAIFURI_TOKUBETU(2) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俀.Text) & txt摿暿嵞怳懼擔俀.Text
        strSAIFURI_TOKUBETU(3) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俁.Text) & txt摿暿嵞怳懼擔俁.Text
        strSAIFURI_TOKUBETU(4) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧係.Text) & txt摿暿嵞怳懼擔係.Text
        strSAIFURI_TOKUBETU(5) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俆.Text) & txt摿暿嵞怳懼擔俆.Text
        strSAIFURI_TOKUBETU(6) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俇.Text) & txt摿暿嵞怳懼擔俇.Text
        '悘帪怳懼擔
        strFURI_ZUIJI(1) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧侾.Text) & txt悘帪怳懼擔侾.Text
        strFURI_ZUIJI(2) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俀.Text) & txt悘帪怳懼擔俀.Text
        strFURI_ZUIJI(3) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俁.Text) & txt悘帪怳懼擔俁.Text
        strFURI_ZUIJI(4) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧係.Text) & txt悘帪怳懼擔係.Text
        strFURI_ZUIJI(5) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俆.Text) & txt悘帪怳懼擔俆.Text
        strFURI_ZUIJI(6) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俇.Text) & txt悘帪怳懼擔俇.Text
        '悘帪怳懼嬫暘
        strFURIKBN_ZUIJI(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘侾)
        strFURIKBN_ZUIJI(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俀)
        strFURIKBN_ZUIJI(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俁)
        strFURIKBN_ZUIJI(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘係)
        strFURIKBN_ZUIJI(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俆)
        strFURIKBN_ZUIJI(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俇)
    End Sub

    '峏怴屻偺忬懺戅旔丂2006/12/04
    Public Sub sb_KOUSIN_SET()
        '擭娫弶怳
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾寧怳懼擔.Text.Trim = "" Then
        If lab侾寧怳懼擔.Text.Trim = "" Or chk侾寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(1) = ""
        Else
            strSYOFURI_NENKAN_AFTER(1) = Replace(lab侾寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俀寧怳懼擔.Text.Trim = "" Then
        If lab俀寧怳懼擔.Text.Trim = "" Or chk俀寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(2) = ""
        Else
            strSYOFURI_NENKAN_AFTER(2) = Replace(lab俀寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俁寧怳懼擔.Text.Trim = "" Then
        If lab俁寧怳懼擔.Text.Trim = "" Or chk俁寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(3) = ""
        Else
            strSYOFURI_NENKAN_AFTER(3) = Replace(lab俁寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab係寧怳懼擔.Text.Trim = "" Then
        If lab係寧怳懼擔.Text.Trim = "" Or chk係寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(4) = ""
        Else
            strSYOFURI_NENKAN_AFTER(4) = Replace(lab係寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俆寧怳懼擔.Text.Trim = "" Then
        If lab俆寧怳懼擔.Text.Trim = "" Or chk俆寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(5) = ""
        Else
            strSYOFURI_NENKAN_AFTER(5) = Replace(lab俆寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俇寧怳懼擔.Text.Trim = "" Then
        If lab俇寧怳懼擔.Text.Trim = "" Or chk俇寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(6) = ""
        Else
            strSYOFURI_NENKAN_AFTER(6) = Replace(lab俇寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俈寧怳懼擔.Text.Trim = "" Then
        If lab俈寧怳懼擔.Text.Trim = "" Or chk俈寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(7) = ""
        Else
            strSYOFURI_NENKAN_AFTER(7) = Replace(lab俈寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俉寧怳懼擔.Text.Trim = "" Then
        If lab俉寧怳懼擔.Text.Trim = "" Or chk俉寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(8) = ""
        Else
            strSYOFURI_NENKAN_AFTER(8) = Replace(lab俉寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俋寧怳懼擔.Text.Trim = "" Then
        If lab俋寧怳懼擔.Text.Trim = "" Or chk俋寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(9) = ""
        Else
            strSYOFURI_NENKAN_AFTER(9) = Replace(lab俋寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侽寧怳懼擔.Text.Trim = "" Then
        If lab侾侽寧怳懼擔.Text.Trim = "" Or chk侾侽寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(10) = ""
        Else
            strSYOFURI_NENKAN_AFTER(10) = Replace(lab侾侽寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侾寧怳懼擔.Text.Trim = "" Then
        If lab侾侾寧怳懼擔.Text.Trim = "" Or chk侾侾寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(11) = ""
        Else
            strSYOFURI_NENKAN_AFTER(11) = Replace(lab侾侾寧怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾俀寧怳懼擔.Text.Trim = "" Then
        If lab侾俀寧怳懼擔.Text.Trim = "" Or chk侾俀寧怳懼擔.Checked = False Then
            strSYOFURI_NENKAN_AFTER(12) = ""
        Else
            strSYOFURI_NENKAN_AFTER(12) = Replace(lab侾俀寧怳懼擔.Text, "/", "")
        End If
        '擭娫嵞怳
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾寧嵞怳懼擔.Text.Trim = "" Or chk侾寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(1) = ""
        Else
            strSAIFURI_NENKAN_AFTER(1) = Replace(lab侾寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俀寧嵞怳懼擔.Text.Trim = "" Then
        If lab俀寧嵞怳懼擔.Text.Trim = "" Or chk俀寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(2) = ""
        Else
            strSAIFURI_NENKAN_AFTER(2) = Replace(lab俀寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俁寧嵞怳懼擔.Text.Trim = "" Then
        If lab俁寧嵞怳懼擔.Text.Trim = "" Or chk俁寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(3) = ""
        Else
            strSAIFURI_NENKAN_AFTER(3) = Replace(lab俁寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab係寧嵞怳懼擔.Text.Trim = "" Then
        If lab係寧嵞怳懼擔.Text.Trim = "" Or chk係寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(4) = ""
        Else
            strSAIFURI_NENKAN_AFTER(4) = Replace(lab係寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俆寧嵞怳懼擔.Text.Trim = "" Then
        If lab俆寧嵞怳懼擔.Text.Trim = "" Or chk俆寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(5) = ""
        Else
            strSAIFURI_NENKAN_AFTER(5) = Replace(lab俆寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俇寧嵞怳懼擔.Text.Trim = "" Then
        If lab俇寧嵞怳懼擔.Text.Trim = "" Or chk俇寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(6) = ""
        Else
            strSAIFURI_NENKAN_AFTER(6) = Replace(lab俇寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俈寧嵞怳懼擔.Text.Trim = "" Then
        If lab俈寧嵞怳懼擔.Text.Trim = "" Or chk俈寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(7) = ""
        Else
            strSAIFURI_NENKAN_AFTER(7) = Replace(lab俈寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俉寧嵞怳懼擔.Text.Trim = "" Then
        If lab俉寧嵞怳懼擔.Text.Trim = "" Or chk俉寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(8) = ""
        Else
            strSAIFURI_NENKAN_AFTER(8) = Replace(lab俉寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab俋寧嵞怳懼擔.Text.Trim = "" Then
        If lab俋寧嵞怳懼擔.Text.Trim = "" Or chk俋寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(9) = ""
        Else
            strSAIFURI_NENKAN_AFTER(9) = Replace(lab俋寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侽寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾侽寧嵞怳懼擔.Text.Trim = "" Or chk侾侽寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(10) = ""
        Else
            strSAIFURI_NENKAN_AFTER(10) = Replace(lab侾侽寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾侾寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾侾寧嵞怳懼擔.Text.Trim = "" Or chk侾侾寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(11) = ""
        Else
            strSAIFURI_NENKAN_AFTER(11) = Replace(lab侾侾寧嵞怳懼擔.Text, "/", "")
        End If
        '2010/10/21 僠僃僢僋儃僢僋僗偺忬懺傪尒傞
        'If lab侾俀寧嵞怳懼擔.Text.Trim = "" Then
        If lab侾俀寧嵞怳懼擔.Text.Trim = "" Or chk侾俀寧嵞怳懼擔.Checked = False Then
            strSAIFURI_NENKAN_AFTER(12) = ""
        Else
            strSAIFURI_NENKAN_AFTER(12) = Replace(lab侾俀寧嵞怳懼擔.Text, "/", "")
        End If
        '摿暿弶怳
        strSYOFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧侾.Text) & txt摿暿怳懼擔侾.Text
        strSYOFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俀.Text) & txt摿暿怳懼擔俀.Text
        strSYOFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俁.Text) & txt摿暿怳懼擔俁.Text
        strSYOFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧係.Text) & txt摿暿怳懼擔係.Text
        strSYOFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俆.Text) & txt摿暿怳懼擔俆.Text
        strSYOFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt摿暿怳懼寧俇.Text) & txt摿暿怳懼擔俇.Text
        '摿暿嵞怳
        strSAIFURI_TOKUBETU_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧侾.Text) & txt摿暿嵞怳懼擔侾.Text
        strSAIFURI_TOKUBETU_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俀.Text) & txt摿暿嵞怳懼擔俀.Text
        strSAIFURI_TOKUBETU_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俁.Text) & txt摿暿嵞怳懼擔俁.Text
        strSAIFURI_TOKUBETU_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧係.Text) & txt摿暿嵞怳懼擔係.Text
        strSAIFURI_TOKUBETU_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俆.Text) & txt摿暿嵞怳懼擔俆.Text
        strSAIFURI_TOKUBETU_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt摿暿嵞怳懼寧俇.Text) & txt摿暿嵞怳懼擔俇.Text
        '悘帪怳懼擔
        strFURI_ZUIJI_AFTER(1) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧侾.Text) & txt悘帪怳懼擔侾.Text
        strFURI_ZUIJI_AFTER(2) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俀.Text) & txt悘帪怳懼擔俀.Text
        strFURI_ZUIJI_AFTER(3) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俁.Text) & txt悘帪怳懼擔俁.Text
        strFURI_ZUIJI_AFTER(4) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧係.Text) & txt悘帪怳懼擔係.Text
        strFURI_ZUIJI_AFTER(5) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俆.Text) & txt悘帪怳懼擔俆.Text
        strFURI_ZUIJI_AFTER(6) = PFUNC_SEIKYUTUKIHI(txt悘帪怳懼寧俇.Text) & txt悘帪怳懼擔俇.Text
        '悘帪怳懼嬫暘
        strFURIKBN_ZUIJI_AFTER(1) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘侾)
        strFURIKBN_ZUIJI_AFTER(2) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俀)
        strFURIKBN_ZUIJI_AFTER(3) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俁)
        strFURIKBN_ZUIJI_AFTER(4) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘係)
        strFURIKBN_ZUIJI_AFTER(5) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俆)
        strFURIKBN_ZUIJI_AFTER(6) = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俇)
    End Sub

#End Region

#Region " Private Function(嫟捠)"
    Private Function PFUNC_COMMON_CHECK() As Boolean

        Dim sStart As String
        Dim sEnd As String

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            If Trim(txtGAKKOU_CODE.Text) = "" Then
                MessageBox.Show("妛峑僐乕僪偑擖椡偝傟偰偄傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGAKKOU_CODE.Focus()
                Return False
            Else
                '妛峑儅僗僞懚嵼僠僃僢僋
                sql.Append("SELECT *")
                sql.Append(" FROM GAKMAST2")
                sql.Append(" WHERE GAKKOU_CODE_T = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")

                If oraReader.DataReader(sql) = True Then

                    Int_Zengo_Kbn(0) = oraReader.GetString("NKYU_CODE_T")
                    Int_Zengo_Kbn(1) = oraReader.GetString("SKYU_CODE_T")
                    '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
                    Sai_Zengo_Kbn = oraReader.GetString("SFURI_KYU_CODE_T")
                    '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END

                    sStart = Mid(oraReader.GetString("KAISI_DATE_T"), 1, 4)
                    sEnd = Mid(oraReader.GetString("SYURYOU_DATE_T"), 1, 4)

                    strFURI_DT = oraReader.GetString("FURI_DATE_T") '2005/12/09
                    strSFURI_DT = ConvNullToString(oraReader.GetString("SFURI_DATE_T"), "") '2005/12/09

                Else
                    MessageBox.Show("擖椡偝傟偨妛峑僐乕僪偑懚嵼偟傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGAKKOU_CODE.Focus()
                    Return False
                End If

                oraReader.Close()

            End If

            If (Trim(txt懳徾擭搙.Text)) = "" Then
                MessageBox.Show("懳徾擭搙傪擖椡偟偰偔偩偝偄", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txt懳徾擭搙.Focus()
                Return False
            Else
                Select Case (sStart <= txt懳徾擭搙.Text >= sEnd)
                    Case False
                        MessageBox.Show("懳徾擭搙偑擖椡斖埻奜偱偡(" & sStart & "乣" & sEnd & ")", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt懳徾擭搙.Focus()
                        Return False
                End Select
            End If

            GAKKOU_INFO.TAISYOU_START_NENDO = txt懳徾擭搙.Text & "04"
            GAKKOU_INFO.TAISYOU_END_NENDO = CStr(CInt(txt懳徾擭搙.Text) + 1) & "03"

            Return True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    '==============================================================
    '僠僃僢僋儃僢僋僗忬懺僠僃僢僋丒妛擭僼儔僌曄悢庢摼丂2006/11/30
    '==============================================================
    Private Function PFUNC_GAKUNENFLG_CHECK(ByVal blnCheck_FLG1 As Boolean, ByVal blnCheck_FLG2 As Boolean, ByVal blnCheck_FLG3 As Boolean, ByVal blnCheck_FLG4 As Boolean, ByVal blnCheck_FLG5 As Boolean, ByVal blnCheck_FLG6 As Boolean, ByVal blnCheck_FLG7 As Boolean, ByVal blnCheck_FLG8 As Boolean, ByVal blnCheck_FLG9 As Boolean, ByVal blnCheck_FLGALL As Boolean) As Boolean

        '僠僃僢僋儃僢僋僗忬懺僠僃僢僋
        PFUNC_GAKUNENFLG_CHECK = False

        If blnCheck_FLG1 = False And _
           blnCheck_FLG2 = False And _
           blnCheck_FLG3 = False And _
           blnCheck_FLG4 = False And _
           blnCheck_FLG5 = False And _
           blnCheck_FLG6 = False And _
           blnCheck_FLG7 = False And _
           blnCheck_FLG8 = False And _
           blnCheck_FLG9 = False And _
           blnCheck_FLGALL = False Then

            Call MessageBox.Show("張棟懳徾妛擭巜掕偑偝傟偰偄傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '僠僃僢僋儃僢僋僗忬懺傪嫟捠曄悢偵愝掕()
        If blnCheck_FLGALL = True Then
            STR侾妛擭 = "1"
            STR俀妛擭 = "1"
            STR俁妛擭 = "1"
            STR係妛擭 = "1"
            STR俆妛擭 = "1"
            STR俇妛擭 = "1"
            STR俈妛擭 = "1"
            STR俉妛擭 = "1"
            STR俋妛擭 = "1"
        Else
            If blnCheck_FLG1 = True Then
                STR侾妛擭 = "1"
            Else
                STR侾妛擭 = "0"
            End If
            If blnCheck_FLG2 = True Then
                STR俀妛擭 = "1"
            Else
                STR俀妛擭 = "0"
            End If
            If blnCheck_FLG3 = True Then
                STR俁妛擭 = "1"
            Else
                STR俁妛擭 = "0"
            End If
            If blnCheck_FLG4 = True Then
                STR係妛擭 = "1"
            Else
                STR係妛擭 = "0"
            End If
            If blnCheck_FLG5 = True Then
                STR俆妛擭 = "1"
            Else
                STR俆妛擭 = "0"
            End If
            If blnCheck_FLG6 = True Then
                STR俇妛擭 = "1"
            Else
                STR俇妛擭 = "0"
            End If
            If blnCheck_FLG7 = True Then
                STR俈妛擭 = "1"
            Else
                STR俈妛擭 = "0"
            End If
            If blnCheck_FLG8 = True Then
                STR俉妛擭 = "1"
            Else
                STR俉妛擭 = "0"
            End If
            If blnCheck_FLG9 = True Then
                STR俋妛擭 = "1"
            Else
                STR俋妛擭 = "0"
            End If
        End If

        PFUNC_GAKUNENFLG_CHECK = True

    End Function

    Private Function PFUNC_KYUJITULIST_SET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        '媥擔忣曬偺昞帵
        Dim sTuki As String
        Dim sDay As String
        Dim sYName As String

        lst媥擔.Items.Clear()

        If Trim(txt懳徾擭搙.Text) <> "" Then
            Select Case CInt(txt懳徾擭搙.Text)
                Case Is > 1900
                    sql.Append(" SELECT ")
                    sql.Append(" YASUMI_DATE_Y")
                    sql.Append(",YASUMI_NAME_Y")
                    sql.Append(" FROM YASUMIMAST")
                    sql.Append(" WHERE")
                    sql.Append(" YASUMI_DATE_Y > '" & txt懳徾擭搙.Text & "0400'")
                    sql.Append(" AND")
                    sql.Append(" YASUMI_DATE_Y < '" & CStr(CInt(txt懳徾擭搙.Text) + 1) & "0399'")
                    sql.Append(" ORDER BY YASUMI_DATE_Y ASC")

                    If oraReader.DataReader(sql) = True Then

                        Do Until oraReader.EOF

                            sTuki = Mid(oraReader.GetString("YASUMI_DATE_Y"), 5, 2)
                            sDay = Mid(oraReader.GetString("YASUMI_DATE_Y"), 7, 2)
                            sYName = Trim(oraReader.GetString("YASUMI_NAME_Y"))

                            lst媥擔.Items.Add(sTuki & "寧" & sDay & "擔" & Space(1) & sYName)

                            '2006/10/23丂媥擔堦棗傪庢摼
                            STRYasumi_List(STRYasumi_List.Length - 1) = txt懳徾擭搙.Text & sTuki & sDay
                            ReDim Preserve STRYasumi_List(STRYasumi_List.Length)

                            oraReader.NextRead()

                        Loop

                    End If
                    oraReader.Close()

                Case Else
                    MessageBox.Show("懳徾擭搙偼侾俋侽侽擭埲崀傪擖椡偟偰偔偩偝偄", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt懳徾擭搙.Focus()
                    Return False
            End Select
        End If

        Return True

    End Function

    Private Function PFUNC_GAKINFO_GET() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader

        sql.Append(" SELECT ")
        sql.Append(" GAKKOU_NNAME_G")
        sql.Append(",SIYOU_GAKUNEN_T")
        sql.Append(",FURI_DATE_T")
        sql.Append(",SFURI_DATE_T")
        sql.Append(",BAITAI_CODE_T")
        sql.Append(",ITAKU_CODE_T")
        sql.Append(",TKIN_NO_T")
        sql.Append(",TSIT_NO_T")
        sql.Append(",SFURI_SYUBETU_T")
        sql.Append(",KAISI_DATE_T")
        sql.Append(",SYURYOU_DATE_T")
        sql.Append(",TESUUTYO_KBN_T")
        sql.Append(",TESUUTYO_KIJITSU_T")
        sql.Append(",TESUUTYO_DAY_T")
        sql.Append(",TESUU_KYU_CODE_T")
        sql.Append(" FROM ")
        sql.Append(" GAKMAST1")
        sql.Append(",GAKMAST2")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_G = GAKKOU_CODE_T")
        sql.Append(" AND")
        sql.Append(" GAKUNEN_CODE_G = 1")
        sql.Append(" AND")
        sql.Append(" GAKKOU_CODE_G ='" & Trim(txtGAKKOU_CODE.Text) & "'")


        If oraReader.DataReader(sql) = True Then

            GAKKOU_INFO.GAKKOU_CODE = Trim(txtGAKKOU_CODE.Text)
            GAKKOU_INFO.GAKKOU_NNAME = Trim(oraReader.GetString("GAKKOU_NNAME_G"))

            '巊梡妛擭悢
            If IsDBNull(oraReader.GetString("SIYOU_GAKUNEN_T")) = False Then
                GAKKOU_INFO.SIYOU_GAKUNEN = CInt(oraReader.GetString("SIYOU_GAKUNEN_T"))
            Else
                GAKKOU_INFO.SIYOU_GAKUNEN = 0
            End If

            '怳懼擔
            If IsDBNull(oraReader.GetString("FURI_DATE_T")) = False Then
                GAKKOU_INFO.FURI_DATE = oraReader.GetString("FURI_DATE_T")
            Else
                GAKKOU_INFO.FURI_DATE = ""
            End If

            '嵞怳擔
            If IsDBNull(oraReader.GetString("SFURI_DATE_T")) = False Then
                GAKKOU_INFO.SFURI_DATE = oraReader.GetString("SFURI_DATE_T")
            Else
                GAKKOU_INFO.SFURI_DATE = ""
            End If

            '攠懱僐乕僪
            If IsDBNull(oraReader.GetString("BAITAI_CODE_T")) = False Then
                GAKKOU_INFO.BAITAI_CODE = oraReader.GetString("BAITAI_CODE_T")
            Else
                GAKKOU_INFO.BAITAI_CODE = ""
            End If

            '埾戸幰僐乕僪
            If IsDBNull(oraReader.GetString("ITAKU_CODE_T")) = False Then
                GAKKOU_INFO.ITAKU_CODE = oraReader.GetString("ITAKU_CODE_T")
            Else
                GAKKOU_INFO.ITAKU_CODE = ""
            End If

            '庢埖嬥梈婡娭僐乕僪
            GAKKOU_INFO.TKIN_CODE = oraReader.GetString("TKIN_NO_T")

            '庢埖巟揦僐乕僪
            GAKKOU_INFO.TSIT_CODE = oraReader.GetString("TSIT_NO_T")

            '嵞怳庬暿
            If IsDBNull(oraReader.GetString("SFURI_SYUBETU_T")) = False Then
                GAKKOU_INFO.SFURI_SYUBETU = oraReader.GetString("SFURI_SYUBETU_T")
            Else
                GAKKOU_INFO.SFURI_SYUBETU = ""
            End If

            '帺怳奐巒擭寧
            GAKKOU_INFO.KAISI_DATE = oraReader.GetString("KAISI_DATE_T")

            '帺怳廔椆擭寧
            GAKKOU_INFO.SYURYOU_DATE = oraReader.GetString("SYURYOU_DATE_T")

            '庤悢椏挜媮婜擔嬫暘
            If IsDBNull(oraReader.GetString("TESUUTYO_KIJITSU_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KIJITSU = oraReader.GetString("TESUUTYO_KIJITSU_T")
            Else
                GAKKOU_INFO.TESUUTYO_KIJITSU = ""
            End If

            '庤悢椏挜媮擔悢
            If IsDBNull(oraReader.GetString("TESUUTYO_DAY_T")) = False Then
                GAKKOU_INFO.TESUUTYO_NO = CInt(oraReader.GetString("TESUUTYO_DAY_T"))
            Else
                GAKKOU_INFO.TESUUTYO_NO = 0
            End If

            '庤悢椏挜媮嬫暘
            If IsDBNull(oraReader.GetString("TESUUTYO_KBN_T")) = False Then
                GAKKOU_INFO.TESUUTYO_KBN = oraReader.GetString("TESUUTYO_KBN_T")
            Else
                GAKKOU_INFO.TESUUTYO_KBN = ""
            End If

            '寛嵪媥擔僐乕僪
            If IsDBNull(oraReader.GetString("TESUU_KYU_CODE_T")) = False Then
                GAKKOU_INFO.TESUU_KYU_CODE = oraReader.GetString("TESUU_KYU_CODE_T")
            Else
                GAKKOU_INFO.TESUU_KYU_CODE = ""
            End If

        Else

            MessageBox.Show("妛峑儅僗僞偵搊榐偝傟偰偄傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

            lab妛峑柤.Text = ""

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        lab妛峑柤.Text = GAKKOU_INFO.GAKKOU_NNAME

        Return True

    End Function

    Private Function PFUNC_SCH_GET_ALL() As Boolean

        PFUNC_SCH_GET_ALL = False

        '嫟捠擖椡僠僃僢僋
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        '僗働僕儏乕儖儅僗僞懚嵼僠僃僢僋
        If PFUNC_SCHMAST_SERCH() = False Then
            Call MessageBox.Show("巜掕偟偨擭搙偺妛峑僗働僕儏乕儖偼懚嵼偟傑偣傫偱偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '擭娫僗働僕儏乕儖嶲徠
        If PFUNC_SCH_GET_NENKAN() = False Then
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
            MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
            'MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
        Else
            MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "")
        End If

        '摿暿僗働僕儏乕儖嶲徠
        If PFUNC_SCH_GET_TOKUBETU() = False Then
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
            MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
            'MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
        Else
            MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "")
        End If

        '悘帪僗働僕儏乕儖嶲徠
        If PFUNC_SCH_GET_ZUIJI() = False Then
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
            MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
            'MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
            ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
        Else
            MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "")
        End If

        '2006/11/30丂奺棑偺抣傪峔憿懱偵擖椡
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    擭娫僗働僕儏乕儖暘
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '摿暿僗働僕儏乕儖暘
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      悘帪僗働僕儏乕儖暘

        PFUNC_SCH_GET_ALL = True

    End Function

    Private Function PFUNC_SCH_INSERT_ALL() As Boolean

        PFUNC_SCH_INSERT_ALL = False

        Try
            MainDB = New MyOracle

            '嫟捠擖椡僠僃僢僋
            If PFUNC_COMMON_CHECK() = False Then
                Exit Function
            End If

            '2006/10/12丂弶怳偲嵞怳偑摨偠擔偱偼側偄偐僠僃僢僋
            If PFUNC_CHECK_SFURI() = False Then
                Exit Function
            End If

            '2006/11/22丂僗働僕儏乕儖摨堦擔僠僃僢僋
            If PFUNC_CHECK_TOKUBETSU() = False Then
                Exit Function
            End If

            '2006/11/30丂摨惪媮寧偐偮摨妛擭僼儔僌偑側偄偐僠僃僢僋
            If PFUNC_GAKNENFLG_CHECK() = False Then
                Exit Function
            End If

            '2010/10/21 悘帪偺摨堦僗働僕儏乕儖僠僃僢僋
            If PFUNC_CHECK_ZUIJI() = False Then
                Exit Function
            End If

            Int_Syori_Flag(0) = 0
            Int_Syori_Flag(1) = 0
            Int_Syori_Flag(2) = 0

            Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
            Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            MainDB.BeginTrans()

            '擭娫僗働僕儏乕儖嶌惉
            If PFUNC_NENKAN_SAKUSEI() = False Then
                MainDB.Rollback()
                '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
                Int_Syori_Flag(0) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "擭娫僗働僕儏乕儖嶌惉偱僄儔乕")
                Return False
            End If

            '摿暿僗働僕儏乕儖嶌惉
            If PFUNC_TOKUBETU_SAKUSEI("嶌惉") = False Then
                MainDB.Rollback()
                '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
                Int_Syori_Flag(1) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "摿暿僗働僕儏乕儖嶌惉偱僄儔乕")
                Return False
            End If

            '悘帪僗働僕儏乕儖嶌惉
            If PFUNC_ZUIJI_SAKUSEI("嶌惉") = False Then
                MainDB.Rollback()
                '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
                Int_Syori_Flag(2) = 2
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "悘帪僗働僕儏乕儖嶌惉偱僄儔乕")
                Return False
            End If

            '晄梫擭娫僗働僕儏乕儖嶍彍張棟
            If PFUNC_DELETE_GSCHMAST() = False Then
                MainDB.Rollback()
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "晄梫擭娫僗働僕儏乕儖嶍彍偱僄儔乕")
                Return False
            End If

            'Select Case True
            '    Case (Int_Syori_Flag(0) = 0 And Int_Syori_Flag(1) = 0 And Int_Syori_Flag(2) = 0)
            '        '張棟審悢側偟
            '        Exit Function
            'End Select

            If Int_Syori_Flag(0) = 1 Then
                '擭娫僗働僕儏乕儖嶲徠
                If PFUNC_SCH_GET_NENKAN() = False Then
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                    MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                    'MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
                Else
                    MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "")
                End If
            End If

            If Int_Syori_Flag(1) = 1 Then
                '摿暿僗働僕儏乕儖嶲徠
                If PFUNC_SCH_GET_TOKUBETU() = False Then
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                    MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                    'MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
                Else
                    MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "")
                End If
            End If

            If Int_Syori_Flag(2) = 1 Then
                '悘帪僗働僕儏乕儖嶲徠
                If PFUNC_SCH_GET_ZUIJI() = False Then
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                    MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                    'MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                    ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
                Else
                    MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "")
                End If
            End If

            '2006/11/30丂奺棑偺抣傪峔憿懱偵擖椡
            Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    擭娫僗働僕儏乕儖暘
            Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '摿暿僗働僕儏乕儖暘
            Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      悘帪僗働僕儏乕儖暘

            MainDB.Commit()

            If Int_Syori_Flag(0) <> 2 Then '捛壛 2005/06/15
                MessageBox.Show("僗働僕儏乕儖偑嶌惉偝傟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If

            PFUNC_SCH_INSERT_ALL = True

        Catch ex As Exception
            MainDB.Rollback()
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", ex.Message)
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error) '2010/10/21 椺奜帪偺僄儔乕儊僢僙乕僕捛壛
            Return False
        Finally
            MainDB.Close()
        End Try

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ALL() As Boolean

        PFUNC_SCH_DELETE_INSERT_ALL = False

        '嫟捠擖椡僠僃僢僋
        If PFUNC_COMMON_CHECK() = False Then
            Exit Function
        End If

        '2006/10/12丂弶怳偲嵞怳偑摨偠擔偱偼側偄偐僠僃僢僋
        If PFUNC_CHECK_SFURI() = False Then
            Exit Function
        End If

        '2006/11/22丂僗働僕儏乕儖摨堦擔僠僃僢僋
        If PFUNC_CHECK_TOKUBETSU() = False Then
            Exit Function
        End If

        '2006/11/30丂摨惪媮寧偐偮摨妛擭僼儔僌偑側偄偐僠僃僢僋
        If PFUNC_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        '2010/10/21 悘帪偺摨堦僗働僕儏乕儖僠僃僢僋
        If PFUNC_CHECK_ZUIJI() = False Then
            Exit Function
        End If

        If MessageBox.Show("尰嵼偺僗働僕儏乕儖偺撪梕偼堦怴偝傟傑偡", msgTitle, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) <> vbOK Then
            Return False
        End If

        Int_Syori_Flag(0) = 0
        Int_Syori_Flag(1) = 0
        Int_Syori_Flag(2) = 0

        Str_SyoriDate(0) = Format(Now, "yyyyMMdd")
        Str_SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

        '2006/11/30丂嶲徠帪偺僨乕僞偲斾傋丄峏怴偑昁梫側僨乕僞傪僠僃僢僋偡傞
        Call PSUB_Kousin_Check()

        '擭娫僗働僕儏乕儖嶌惉
        If PFUNC_SCH_DELETE_INSERT_NENKAN() = False Then
            MainLOG.Write("擭娫僗働僕儏乕儖峏怴張棟", "幐攕", "")
            Exit Function
        Else
            MainLOG.Write("擭娫僗働僕儏乕儖峏怴張棟", "惉岟", "")
        End If

        '摿暿僗働僕儏乕儖嶌惉
        If PFUNC_SCH_DELETE_INSERT_TOKUBETU() = False Then
            MainLOG.Write("摿暿僗働僕儏乕儖峏怴張棟", "幐攕", "")
            Exit Function
        Else
            MainLOG.Write("摿暿僗働僕儏乕儖峏怴張棟", "惉岟", "")
        End If

        '悘帪僗働僕儏乕儖嶌惉
        If PFUNC_SCH_DELETE_INSERT_ZUIJI() = False Then
            MainLOG.Write("悘帪僗働僕儏乕儖峏怴張棟", "幐攕", "")
            Exit Function
        Else
            MainLOG.Write("悘帪僗働僕儏乕儖峏怴張棟", "惉岟", "")
        End If

        'Select case True
        '    Case (Int_Syori_Flag(0) = 0 AND Int_Syori_Flag(1) = 0 AND Int_Syori_Flag(2) = 0)
        '        '張棟審悢側偟
        '        Exit Function
        'End Select

        If Int_Syori_Flag(0) = 1 Then
            '擭娫僗働僕儏乕儖嶲徠
            If PFUNC_SCH_GET_NENKAN() = False Then
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                'MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
            Else
                MainLOG.Write("擭娫僗働僕儏乕儖嶲徠", "惉岟", "")
            End If
        End If

        If Int_Syori_Flag(1) = 1 Then
            '摿暿僗働僕儏乕儖嶲徠
            If PFUNC_SCH_GET_TOKUBETU() = False Then
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                'MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
            Else
                MainLOG.Write("摿暿僗働僕儏乕儖嶲徠", "惉岟", "")
            End If
        End If

        If Int_Syori_Flag(2) = 1 Then
            '悘帪僗働僕儏乕儖嶲徠
            If PFUNC_SCH_GET_ZUIJI() = False Then
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- START
                MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "乮懳徾審悢侽審乯")
                'MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "幐攕", "乮懳徾審悢侽審乯")
                ' 2016/02/08 僞僗僋乯娾忛 CHG 亂IT亃UI_B-14-99(RSV2懳墳(婛懚僶僌廋惓)) -------------------- END
            Else
                MainLOG.Write("悘帪僗働僕儏乕儖嶲徠", "惉岟", "")
            End If
        End If

        '婇嬈帺怳楢実 2006/12/04
        Call sb_KOUSIN_SET()
        If fn_CHECK_CHANGE() = False Then
            Exit Function
        End If

        '2006/11/30丂奺棑偺抣傪峔憿懱偵擖椡
        Call PSUB_NENKAN_GET(SYOKI_NENKAN_SCHINFO) '    擭娫僗働僕儏乕儖暘
        Call PSUB_TOKUBETU_GET(SYOKI_TOKUBETU_SCHINFO) '摿暿僗働僕儏乕儖暘
        Call PSUB_ZUIJI_GET(SYOKI_ZUIJI_SCHINFO) '      悘帪僗働僕儏乕儖暘

        MessageBox.Show("僗働僕儏乕儖偑峏怴偝傟傑偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Information)

        PFUNC_SCH_DELETE_INSERT_ALL = True

    End Function

    Private Function PFUNC_SEIKYUTUKIHI(ByVal strTUKI As String) As String

        '擖椡懳徾擭搙偲桳岠寧傪傕偲偵惪媮擭寧傪寁嶼
        If strTUKI = "01" Or strTUKI = "02" Or strTUKI = "03" Then
            PFUNC_SEIKYUTUKIHI = CStr(CInt(txt懳徾擭搙.Text) + 1) & strTUKI
        Else
            PFUNC_SEIKYUTUKIHI = txt懳徾擭搙.Text & strTUKI
        End If

    End Function

    Private Function PFUNC_FURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '怳懼擔偺嶌惉
        PFUNC_FURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '捠忢
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '弶怳
                            PFUNC_FURIHI_MAKE = STR惪媮擭寧 & GAKKOU_INFO.FURI_DATE
                        Case "1"   '嵞怳
                            PFUNC_FURIHI_MAKE = STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE
                    End Select
                Else
                    PFUNC_FURIHI_MAKE = STR惪媮擭寧 & strFURIHI
                End If
            Case "1"     '摿暿
                '擖椡懳徾擭搙偲擖椡怳懼寧丄擔傪傕偲偵怳懼擭寧擔傪寁嶼
                If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                    PFUNC_FURIHI_MAKE = CStr(CInt(txt懳徾擭搙.Text) + 1) & strFURITUKI & strFURIHI
                Else
                    PFUNC_FURIHI_MAKE = txt懳徾擭搙.Text & strFURITUKI & strFURIHI
                End If
            Case "2"     '悘帪
                PFUNC_FURIHI_MAKE = STR惪媮擭寧 & strFURIHI
        End Select

        '塩嬈擔嶼弌
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '梻塩嬈擔
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "+")
            Case 1
                '慜塩嬈擔
                PFUNC_FURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_FURIHI_MAKE, "0", "-")
        End Select

    End Function

    Private Function PFUNC_SAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String

        '嵞怳懼擔偺嶌惉
        PFUNC_SAIFURIHI_MAKE = ""

        '嵞怳懼擔偺弶婜抣愝掕
        PFUNC_SAIFURIHI_MAKE = STRW嵞怳懼擭 & strFURITUKI & strFURIHI

        '嵞怳庬暿偑乽侽乿丄乽俁乿偺応崌偼嵞怳懼擔偺愝掕偼晄梫
        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case "3"
                PFUNC_SAIFURIHI_MAKE = "00000000"
            Case Else
                '塩嬈擔嶼弌
                '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
                Select Case Sai_Zengo_Kbn
                    'Select Case Int_Zengo_Kbn(1)
                    '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END
                    Case 0
                        '梻塩嬈擔
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "+")
                    Case 1
                        '慜塩嬈擔
                        PFUNC_SAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(PFUNC_SAIFURIHI_MAKE, "0", "-")
                End Select
        End Select

    End Function

    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String) As String
        '嵞怳儗僐乕僪偺嵞怳懼擔偺嶌惉乮師夞偺弶怳擔乯
        Dim str擭 As String
        Dim str寧 As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str擭 = Mid(STR惪媮擭寧, 1, 4)

        If strFURIHI <= GAKKOU_INFO.FURI_DATE Then
            str寧 = strFURITUKI
        Else
            If strFURITUKI = "12" Then
                str寧 = "01"
                str擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)

            Else
                str寧 = Format((CInt(strFURITUKI) + 1), "00")
            End If
        End If

        '塩嬈擔嶼弌
        PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str擭 & str寧 & GAKKOU_INFO.FURI_DATE, "0", "+")

    End Function

    '2011/06/15 昗弨斉廋惓 嵞乆怳懼擔嶼弌梡娭悢捛壛 -------------START
    Private Function PFUNC_SAISAIFURIHI_MAKE(ByVal strFURIDATE As String) As String
        '嵞怳儗僐乕僪偺嵞怳懼擔偺嶌惉乮師夞偺弶怳擔乯
        Dim str擭 As String
        Dim str寧 As String

        PFUNC_SAISAIFURIHI_MAKE = ""

        str擭 = strFURIDATE.Substring(0, 4)
        str寧 = strFURIDATE.Substring(4, 2)

        '嵞怳擔 >= 摨堦寧偺弶怳擔偲側傞応崌偼丄棃寧偺弶怳擔傪師夞偺弶怳擔偲偡傞
        If strFURIDATE >= str擭 & str寧 & GAKKOU_INFO.FURI_DATE Then
            If str寧 = "12" Then
                str擭 = (CInt(str擭) + 1).ToString("0000")
                str寧 = "01"
            Else
                str寧 = (CInt(str寧) + 1).ToString("00")
            End If
        End If

        '塩嬈擔嶼弌
        Select Case Int_Zengo_Kbn(1)
            Case 0
                '梻塩嬈擔
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str擭 & str寧 & GAKKOU_INFO.FURI_DATE, "0", "+")
            Case 1
                '慜塩嬈擔
                PFUNC_SAISAIFURIHI_MAKE = PFUNC_EIGYOUBI_GET(str擭 & str寧 & GAKKOU_INFO.FURI_DATE, "0", "-")
        End Select

    End Function
    '2011/06/15 昗弨斉廋惓 嵞乆怳懼擔嶼弌梡娭悢捛壛 -------------END
    '2011/06/15 昗弨斉廋惓 嵞乆怳懼擔嶼弌梡娭悢捛壛 -------------START
    Private Function PFUNC_KFURIHI_MAKE(ByVal strFURITUKI As String, ByVal strFURIHI As String, ByVal strSCHKUBUN As String, ByVal strFURIKUBUN As String) As String

        '怳懼擔偺嶌惉
        PFUNC_KFURIHI_MAKE = ""

        Select Case strSCHKUBUN
            Case "0"     '捠忢
                If strFURIHI = "" Then
                    Select Case strFURIKUBUN
                        Case "0"   '弶怳
                            PFUNC_KFURIHI_MAKE = STR惪媮擭寧 & GAKKOU_INFO.FURI_DATE
                        Case "1"   '嵞怳
                            '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------START
                            'PFUNC_KFURIHI_MAKE = STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE
                            If GAKKOU_INFO.FURI_DATE < GAKKOU_INFO.SFURI_DATE Then
                                PFUNC_KFURIHI_MAKE = STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE
                            Else
                                If STR惪媮擭寧.Substring(4, 2) = "12" Then
                                    PFUNC_KFURIHI_MAKE = (CInt(STR惪媮擭寧.Substring(0, 4)) + 1).ToString("0000") & "01" & GAKKOU_INFO.SFURI_DATE
                                Else
                                    PFUNC_KFURIHI_MAKE = (CInt(STR惪媮擭寧) + 1).ToString("000000") & GAKKOU_INFO.SFURI_DATE
                                End If
                            End If
                            '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------E
                    End Select
                Else
                    ''擖椡擔晅傪宊栺怳懼擔偵偡傞応崌
                    'PFUNC_KFURIHI_MAKE = STR惪媮擭寧 & strFURIHI

                    '幚怳懼擔傪宊栺怳懼擔偵偡傞応崌
                    PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
                End If
            Case "1"     '摿暿
                ''擖椡擔晅傪宊栺怳懼擔偵偡傞応崌
                ''擖椡懳徾擭搙偲擖椡怳懼寧丄擔傪傕偲偵怳懼擭寧擔傪寁嶼
                'If strFURITUKI = "01" Or strFURITUKI = "02" Or strFURITUKI = "03" Then
                '    PFUNC_KFURIHI_MAKE = CStr(CInt(txt懳徾擭搙.Text) + 1) & strFURITUKI & strFURIHI
                'Else
                '    PFUNC_KFURIHI_MAKE = txt懳徾擭搙.Text & strFURITUKI & strFURIHI
                'End If

                '幚怳懼擔傪宊栺怳懼擔偵偡傞応崌
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
            Case "2"     '悘帪
                ''擖椡擔晅傪宊栺怳懼擔偵偡傞応崌
                'PFUNC_KFURIHI_MAKE = STR惪媮擭寧 & strFURIHI

                '幚怳懼擔傪宊栺怳懼擔偵偡傞応崌
                PFUNC_KFURIHI_MAKE = PFUNC_FURIHI_MAKE(strFURITUKI, strFURIHI, strSCHKUBUN, strFURIKUBUN)
        End Select

        '寧枛曗惓(寧枛巜掕偺応崌幚擔偵曄姺偡傞)
        Dim strFURINEN As String = PFUNC_KFURIHI_MAKE.Substring(0, 4)
        strFURITUKI = PFUNC_KFURIHI_MAKE.Substring(4, 2)
        strFURIHI = PFUNC_KFURIHI_MAKE.Substring(6, 2)

        Dim intGETUMATU As Integer = Date.DaysInMonth(CInt(strFURINEN), CInt(strFURITUKI))
        If CInt(strFURIHI) > intGETUMATU Then
            PFUNC_KFURIHI_MAKE = strFURINEN & strFURITUKI & intGETUMATU.ToString("00")
        End If

    End Function
    '2011/06/15 昗弨斉廋惓 嵞乆怳懼擔嶼弌梡娭悢捛壛 -------------END

    Private Function PFUNC_EIGYOUBI_GET(ByVal str擭寧擔 As String, ByVal str擔悢 As String, ByVal str慜屻塩嬈擔嬫暘 As String) As String

        '塩嬈擔嶼弌
        Dim WORK_DATE As Date
        Dim YOUBI As Long
        Dim HOSEI As Integer

        Dim int擔悢 As Integer

        PFUNC_EIGYOUBI_GET = ""

        int擔悢 = CInt(str擔悢)

        '-------------------------------------
        '寧枛曗惓乮寧枛巜掕偺応崌幚擔偵曄姺偡傞乯
        '-------------------------------------
        Select Case Mid(str擭寧擔, 5, 2)
            Case "01", "03", "05", "07", "08", "10", "12"
                If Mid(str擭寧擔, 7, 2) < "01" Then
                    Mid(str擭寧擔, 7, 2) = "01"
                End If
                If Mid(str擭寧擔, 7, 2) > "31" Then
                    Mid(str擭寧擔, 7, 2) = "31"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str擭寧擔, 1, 4)), CInt(Mid(str擭寧擔, 5, 2)), CInt(Mid(str擭寧擔, 7, 2)))
            Case "04", "06", "09", "11"
                If Mid(str擭寧擔, 7, 2) < "01" Then
                    Mid(str擭寧擔, 7, 2) = "01"
                End If
                If Mid(str擭寧擔, 7, 2) > "30" Then
                    Mid(str擭寧擔, 7, 2) = "30"
                End If
                WORK_DATE = DateSerial(CInt(Mid(str擭寧擔, 1, 4)), CInt(Mid(str擭寧擔, 5, 2)), CInt(Mid(str擭寧擔, 7, 2)))
            Case "02"
                If Mid(str擭寧擔, 7, 2) < "01" Then
                    Mid(str擭寧擔, 7, 2) = "01"
                End If
                '俀寧俀俋擔,俀寧俁侽擔,俀寧俁侾擔偼俀寧枛擔巜掕埖偄偱俀寧枛擔乮幚擔偵曄姺乯
                If Mid(str擭寧擔, 7, 2) > "28" Then
                    '侾寧枛偺幚擔偱擔晅宆僨乕僞曄姺
                    WORK_DATE = Mid(str擭寧擔, 1, 4) & "/" & "01" & "/" & "31"
                    '俀寧枛偺幚擔傪嶼弌
                    WORK_DATE = DateAdd(DateInterval.Month, 1, WORK_DATE)
                Else
                    '俀寧枛擔埲奜偺擔晅曄姺
                    WORK_DATE = DateSerial(CInt(Mid(str擭寧擔, 1, 4)), CInt(Mid(str擭寧擔, 5, 2)), CInt(Mid(str擭寧擔, 7, 2)))
                End If
        End Select

        '------------
        '侽塩嬈擔懳墳
        '------------
        If int擔悢 = 0 Then
            YOUBI = Weekday(WORK_DATE)

            '梛擔敾掕(Sun = 1:Sat = 7)
            If YOUBI = 1 Or _
               YOUBI = 7 Or _
               PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = False Then
                HOSEI = 1
            Else
                HOSEI = 0
            End If

            Do Until HOSEI = 0
                If str慜屻塩嬈擔嬫暘 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str慜屻塩嬈擔嬫暘 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If
                YOUBI = Weekday(WORK_DATE)

                '梛擔敾掕(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        HOSEI = HOSEI - 1
                    End If
                End If
            Loop
        Else
            '-----------------
            '侽塩嬈擔埲奜偺張棟
            '-----------------
            Do Until int擔悢 = 0
                If str慜屻塩嬈擔嬫暘 = "+" Then
                    WORK_DATE = DateAdd(DateInterval.Day, 1, WORK_DATE)
                End If
                If str慜屻塩嬈擔嬫暘 = "-" Then
                    WORK_DATE = DateAdd(DateInterval.Day, -1, WORK_DATE)
                End If

                YOUBI = Weekday(WORK_DATE)

                '梛擔敾掕(Sun = 1:Sat = 7)
                If (YOUBI <> 1) And (YOUBI <> 7) Then
                    If PFUNC_COMMON_YASUMIGET(Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")) = True Then
                        int擔悢 = int擔悢 - 1
                    End If
                End If
            Loop
        End If

        PFUNC_EIGYOUBI_GET = Format(WORK_DATE, "yyyy") & Format(WORK_DATE, "MM") & Format(WORK_DATE, "dd")

    End Function

    Private Function PFUNC_COMMON_YASUMIGET(ByVal str擭寧擔 As String) As Boolean

        '媥擔儅僗僞懚嵼僠僃僢僋
        PFUNC_COMMON_YASUMIGET = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            sql.Append(" SELECT * FROM YASUMIMAST")
            sql.Append(" WHERE")
            sql.Append(" YASUMI_DATE_Y ='" & str擭寧擔 & "'")

            If oraReader.DataReader(sql) = True Then
                Return False
            End If

            PFUNC_COMMON_YASUMIGET = True

        Catch ex As Exception

            Throw

        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_SCHMAST_GET(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String, ByVal strSAIFURIHI As String) As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        '僗働僕儏乕儖儅僗僞懚嵼僠僃僢僋 
        '僉乕偼丄妛峑僐乕僪丄僗働僕儏乕儖嬫暘丄怳懼嬫暘丄怳懼擔,嵞怳懼擔
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")
        '2006/11/30丂擭娫僗働僕儏乕儖偼嵞怳擔傪僠僃僢僋偟側偄
        If strSCHKBN <> "0" Then
            sql.Append(" AND")
            sql.Append(" SFURI_DATE_S ='" & strSAIFURIHI & "'")
        End If

        If oraReader.DataReader(sql) = True Then
            bret = True
        End If
        oraReader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_GET_FLG(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '捠忢偺僗働僕儏乕儖偺張棟僼儔僌庢摼 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '僗働僕儏乕儖儅僗僞懚嵼僠僃僢僋 
        '僉乕偼丄妛峑僐乕僪丄僗働僕儏乕儖嬫暘丄怳懼嬫暘丄怳懼擔
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '弶婜壔
        strENTRI_FLG = "0"
        strCHECK_FLG = "0"
        strDATA_FLG = "0"
        strFUNOU_FLG = "0"
        strSAIFURI_FLG = "0"
        strKESSAI_FLG = "0"
        strTYUUDAN_FLG = "0"
        strSAIFURI_DEF = "00000000"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG = oraReader.GetString("TYUUDAN_FLG_S")
                strSAIFURI_DEF = oraReader.GetString("SFURI_DATE_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_SCHMAST_GET_FLG_SAI(ByVal strSCHKBN As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '捠忢偺僗働僕儏乕儖偺張棟僼儔僌(嵞怳暘)庢摼 2006/10/24

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '僗働僕儏乕儖儅僗僞懚嵼僠僃僢僋 
        '僉乕偼丄妛峑僐乕僪丄僗働僕儏乕儖嬫暘丄怳懼嬫暘丄怳懼擔
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = '" & strSCHKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

        '弶婜壔
        strENTRI_FLG_SAI = "0"
        strCHECK_FLG_SAI = "0"
        strDATA_FLG_SAI = "0"
        strFUNOU_FLG_SAI = "0"
        strSAIFURI_FLG_SAI = "0"
        strKESSAI_FLG_SAI = "0"
        strTYUUDAN_FLG_SAI = "0"

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG_SAI = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG_SAI = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG_SAI = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG_SAI = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG_SAI = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG_SAI = oraReader.GetString("KESSAI_FLG_S")
                strTYUUDAN_FLG_SAI = oraReader.GetString("TYUUDAN_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT_MOTO(ByVal strNENGETUDO As String, ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '婛懚偺僗働僕儏乕儖暘偺張棟寢壥悢傪嵞僇僂儞僩仌峏怴
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '捠忢儗僐乕僪偺懚嵼僠僃僢僋
        PFUNC_G_MEIMAST_COUNT_MOTO = True

        '僉乕偼丄妛峑僐乕僪丄怳懼嬫暘丄怳懼擔
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()


        PFUNC_G_MEIMAST_COUNT_MOTO = False
        bFlg = False

        sql = New StringBuilder(128)

        '妛擭巜掕偑側偄応崌偼張棟傪偟側偄
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '摿暿儗僐乕僪偺懳徾妛擭僼儔僌偺忬懺傪尦偵捠忢儗僐乕僪偺懳徾妛擭僼儔僌傪俷俥俥偵偡傞
            '俷俶偵偡傞婡擻傪帩偨偣偨応崌丄摿暿儗僐乕僪偑暋悢審懚嵼偟偨応崌偵慜儗僐乕僪偱偺張棟偑柍懯偵側傞
            '摿暿儗僐乕僪偺懳徾妛擭侾僼儔僌偑乽侾乿偺応崌
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            sql.Append(" SYORI_KEN_S =" & lngSYORI_KEN & ",")
            sql.Append(" SYORI_KIN_S =" & dblSYORI_KIN & ",")
            sql.Append(" FURI_KEN_S =" & lngFURI_KEN & ",")
            sql.Append(" FURI_KIN_S =" & dblFURI_KIN & ",")
            sql.Append(" FUNOU_KEN_S =" & lngFUNOU_KEN & ",")
            sql.Append(" FUNOU_KIN_S =" & dblFUNOU_KIN)
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKBN & "'")
            sql.Append(" AND")
            sql.Append(" FURI_DATE_S ='" & strFURIHI & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '峏怴張棟僄儔乕
                MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    Private Function PFUNC_G_MEIMAST_COUNT(ByVal strFURIKBN As String, ByVal strFURIHI As String) As Boolean
        '僨乕僞僼儔僌=1偺応崌偼柧嵶儅僗僞偐傜張棟審悢丒嬥妟傪僇僂儞僩
        '晄擻僼儔僌=1偺応崌偼柧嵶儅僗僞偐傜怳懼嵪傒審悢丒嬥妟丄晄擻審悢丒嬥妟傪僇僂儞僩
        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_G_MEIMAST_COUNT = False

        '僉乕偼丄妛峑僐乕僪丄怳懼嬫暘丄怳懼擔
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '" & strFURIKBN & "'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURIHI & "'")

        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            sql.Append(" AND (")

            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = True Then
                        sql.Append(" OR ")
                    End If

                    sql.Append(" GAKUNEN_CODE_M = " & iCount)
                    bFlg = True
                End If
            Next iCount

            sql.Append(" )")
        End If

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetString("SEIKYU_KIN_M"))
                End If

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function


    Private Function PFUNC_FURIHI_HANI_CHECK() As Boolean

        '怳懼擔丄嵞怳懼擔偺宊栺婜娫僠僃僢僋
        PFUNC_FURIHI_HANI_CHECK = False

        ' 2016/05/06 僞僗僋乯埢晹 CHG 亂OM亃UI_B-99-99(RSV2懳墳(昗弨僶僌廋惓)) -------------------- START
        'If Mid(STR怳懼擔, 1, 6) >= GAKKOU_INFO.KAISI_DATE And Mid(STR怳懼擔, 1, 6) <= GAKKOU_INFO.SYURYOU_DATE Then
        'Else
        '    Exit Function
        'End If
        If STR怳懼擔 >= GAKKOU_INFO.KAISI_DATE And STR怳懼擔 <= GAKKOU_INFO.SYURYOU_DATE Then
        Else
            Exit Function
        End If
        ' 2016/05/06 僞僗僋乯埢晹 CHG 亂OM亃UI_B-99-99(RSV2懳墳(昗弨僶僌廋惓)) -------------------- END

        PFUNC_FURIHI_HANI_CHECK = True

    End Function
    Private Function PFUNC_SCHMAST_SERCH() As Boolean

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)
        Dim bret As Boolean = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & Trim(txtGAKKOU_CODE.Text) & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & Trim(txt懳徾擭搙.Text) & "04'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & CStr(CInt(Trim(txt懳徾擭搙.Text)) + 1) & "03'")

        If orareader.DataReader(sql) = True Then
            bret = True
        End If

        orareader.Close()

        Return bret

    End Function

    Private Function PFUNC_SCHMAST_UPDATE_SFURIDATE(ByVal pSCH_KBN_S As String) As Boolean

        Dim sql As New StringBuilder(128)

        '張棟拞偺弶怳擔僗働僕儏乕儖偵傕偮嵞怳擔偑尰忬峏怴偱偒側偄偺偱
        '嵞怳傪嶌惉偟偰偄傞帪偵堦弿偵峏怴傕峴偆
        sql.Append(" UPDATE  G_SCHMAST SET ")
        sql.Append(" SFURI_DATE_S ='" & Str_SFURI_DATE & "'")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='" & pSCH_KBN_S & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & Str_FURI_DATE & "'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_DELETE_GSCHMAST() As Boolean

        ' 2017/05/26 僞僗僋乯埢晹 CHG 亂ME亃(RSV2懳墳 擭娫僗働僕儏乕儖偺嶍彍忦審晄旛廋惓) -------------------- START
        '=========================================================================
        ' 摿暿僗働僕儏乕儖傪嶌惉偟偨応崌丄摨堦偺擭寧搙偺擭娫僗働僕儏乕儖偼
        ' 懚嵼偟偰偼側傜側偄偨傔丄擭娫僗働僕儏乕儖偼擭寧搙扨埵偵嶍彍偡傞傛偆
        ' 曄峏偡傞丅
        '=========================================================================
        'Dim sql As New StringBuilder(128)

        ''摿暿僗働僕儏乕儖傪嶌惉偟偨偙偲偵傛傝
        ''擭娫偱懳徾妛擭偑懚嵼偟側偄儗僐乕僪偑嶌惉偝傟偰偟傑偆堊
        ''摿暿偺張棟妋掕屻丄擭娫偺僗働僕儏乕儖偱妛擭僼儔僌偑ALLZERO偺
        ''儗僐乕僪傪嶍彍偡傞
        'sql.Append(" DELETE  FROM G_SCHMAST")
        'sql.Append(" WHERE")
        'sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        'sql.Append(" AND")
        'sql.Append(" SCH_KBN_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN1_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN2_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN3_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN4_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN5_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN6_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN7_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN8_FLG_S ='0'")
        'sql.Append(" AND")
        'sql.Append(" GAKUNEN9_FLG_S ='0'")

        'If MainDB.ExecuteNonQuery(sql) < 0 Then
        '    Return False
        'End If

        'Return True
        Dim SQL As New StringBuilder(128)
        Dim SQL_DEL_TOKUSCH As New StringBuilder(128)
        Dim OraReader_Tokubetu As CASTCommon.MyOracleReader = Nothing

        Try

            SQL.Length = 0
            SQL.Append(" SELECT ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" FROM ")
            SQL.Append("     G_SCHMAST")
            SQL.Append(" WHERE")
            SQL.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
            SQL.Append(" AND SCH_KBN_S     = '1'")
            SQL.Append(" GROUP BY ")
            SQL.Append("     NENGETUDO_S")
            SQL.Append(" ORDER BY ")
            SQL.Append("     NENGETUDO_S")

            OraReader_Tokubetu = New CASTCommon.MyOracleReader(MainDB)
            If OraReader_Tokubetu.DataReader(SQL) = False Then
                '=================================================================
                ' 摿暿僗働僕儏乕儖偑懚嵼偟側偄偨傔丄嶍彍張棟晄梫
                '=================================================================
                Return True
            Else
                '=================================================================
                ' 摿暿僗働僕儏乕儖偑懚嵼偡傞偨傔丄嶍彍張棟奐巒
                '=================================================================
                Do Until OraReader_Tokubetu.EOF
                    SQL_DEL_TOKUSCH.Length = 0
                    SQL_DEL_TOKUSCH.Append(" DELETE FROM G_SCHMAST ")
                    SQL_DEL_TOKUSCH.Append(" WHERE")
                    SQL_DEL_TOKUSCH.Append("     GAKKOU_CODE_S = '" & GAKKOU_INFO.GAKKOU_CODE & "'")
                    SQL_DEL_TOKUSCH.Append(" AND NENGETUDO_S   = '" & OraReader_Tokubetu.GetString("NENGETUDO_S") & "'")
                    SQL_DEL_TOKUSCH.Append(" AND SCH_KBN_S     = '0'")

                    If MainDB.ExecuteNonQuery(SQL_DEL_TOKUSCH) < 0 Then
                        Return False
                    Else
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "晄梫擭娫僗働僕儏乕儖嶍彍", "惉岟", "擭寧搙:" & OraReader_Tokubetu.GetString("NENGETUDO_S"))
                    End If

                    OraReader_Tokubetu.NextRead()
                Loop
            End If

            OraReader_Tokubetu.Close()

            Return True

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "晄梫擭娫僗働僕儏乕儖嶍彍", "幐攕", ex.Message)
            Return False
        Finally
            If Not OraReader_Tokubetu Is Nothing Then
                OraReader_Tokubetu.Close()
                OraReader_Tokubetu = Nothing
            End If
        End Try
        ' 2017/05/26 僞僗僋乯埢晹 CHG 亂ME亃(RSV2懳墳 擭娫僗働僕儏乕儖偺嶍彍忦審晄旛廋惓) -------------------- END

    End Function

    Private Function PFUNC_CHECK_SFURI() As Boolean
        '2006/10/12丂弶怳偲嵞怳偑摨偠擔偱側偄偐僠僃僢僋偡傞

        PFUNC_CHECK_SFURI = False

        '擭娫僗働僕儏乕儖晹暘僠僃僢僋
        If (chk係寧怳懼擔.Checked = True And chk係寧嵞怳懼擔.Checked = True And txt係寧怳懼擔.Text <> "" And txt係寧怳懼擔.Text = txt係寧嵞怳懼擔.Text) Or _
           (chk俆寧怳懼擔.Checked = True And chk俆寧嵞怳懼擔.Checked = True And txt俆寧怳懼擔.Text <> "" And txt俆寧怳懼擔.Text = txt俆寧嵞怳懼擔.Text) Or _
           (chk俇寧怳懼擔.Checked = True And chk俇寧嵞怳懼擔.Checked = True And txt俇寧怳懼擔.Text <> "" And txt俇寧怳懼擔.Text = txt俇寧嵞怳懼擔.Text) Or _
           (chk俈寧怳懼擔.Checked = True And chk俈寧嵞怳懼擔.Checked = True And txt俈寧怳懼擔.Text <> "" And txt俈寧怳懼擔.Text = txt俈寧嵞怳懼擔.Text) Or _
           (chk俉寧怳懼擔.Checked = True And chk俉寧嵞怳懼擔.Checked = True And txt俉寧怳懼擔.Text <> "" And txt俉寧怳懼擔.Text = txt俉寧嵞怳懼擔.Text) Or _
           (chk俋寧怳懼擔.Checked = True And chk俋寧嵞怳懼擔.Checked = True And txt俋寧怳懼擔.Text <> "" And txt俋寧怳懼擔.Text = txt俋寧嵞怳懼擔.Text) Or _
           (chk侾侽寧怳懼擔.Checked = True And chk侾侽寧嵞怳懼擔.Checked = True And txt侾侽寧怳懼擔.Text <> "" And txt侾侽寧怳懼擔.Text = txt侾侽寧嵞怳懼擔.Text) Or _
           (chk侾侾寧怳懼擔.Checked = True And chk侾侾寧嵞怳懼擔.Checked = True And txt侾侾寧怳懼擔.Text <> "" And txt侾侾寧怳懼擔.Text = txt侾侾寧嵞怳懼擔.Text) Or _
           (chk侾俀寧怳懼擔.Checked = True And chk侾俀寧嵞怳懼擔.Checked = True And txt侾俀寧怳懼擔.Text <> "" And txt侾俀寧怳懼擔.Text = txt侾俀寧嵞怳懼擔.Text) Or _
           (chk侾寧怳懼擔.Checked = True And chk侾寧嵞怳懼擔.Checked = True And txt侾寧怳懼擔.Text <> "" And txt侾寧怳懼擔.Text = txt侾寧嵞怳懼擔.Text) Or _
           (chk俀寧怳懼擔.Checked = True And chk俀寧嵞怳懼擔.Checked = True And txt俀寧怳懼擔.Text <> "" And txt俀寧怳懼擔.Text = txt俀寧嵞怳懼擔.Text) Or _
           (chk俁寧怳懼擔.Checked = True And chk俁寧嵞怳懼擔.Checked = True And txt俁寧怳懼擔.Text <> "" And txt俁寧怳懼擔.Text = txt俁寧嵞怳懼擔.Text) Then

            MessageBox.Show("怳懼擔偲嵞怳懼擔偑摨偠傕偺偑偁傝傑偡", "擭娫僗働僕儏乕儖", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '摿暿怳懼擔晹暘僠僃僢僋
        If (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text <> "" And txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text) Or _
           (txt摿暿惪媮寧俀.Text <> "" And txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text <> "" And txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text) Or _
           (txt摿暿惪媮寧俁.Text <> "" And txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text <> "" And txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text) Or _
           (txt摿暿惪媮寧係.Text <> "" And txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text <> "" And txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text) Or _
           (txt摿暿惪媮寧俆.Text <> "" And txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text <> "" And txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text) Or _
           (txt摿暿惪媮寧俇.Text <> "" And txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text <> "" And txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Then

            MessageBox.Show("怳懼擔偲嵞怳懼擔偑摨偠傕偺偑偁傝傑偡", "摿暿怳懼擔", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If

        '2007/02/12KG 丟摿暿怳懼擔偱丄摨堦寧偺弶怳擔偲嵞怳擔偑摨堦偺応崌ERR偲傒側偡丅
        '****************************************************************************
        If (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧俀.Text <> "") And ((txt摿暿惪媮寧侾.Text = txt摿暿怳懼寧俀.Text) And (txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text) Or (txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text)) Or _
            (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧俁.Text <> "") And ((txt摿暿惪媮寧侾.Text = txt摿暿怳懼寧俁.Text) And (txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text) Or (txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text)) Or _
            (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧係.Text <> "") And ((txt摿暿惪媮寧侾.Text = txt摿暿怳懼寧係.Text) And (txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text) Or (txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text)) Or _
            (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧俆.Text <> "") And ((txt摿暿惪媮寧侾.Text = txt摿暿怳懼寧俆.Text) And (txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text) Or (txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text)) Or _
           (txt摿暿惪媮寧侾.Text <> "" And txt摿暿怳懼寧俇.Text <> "") And ((txt摿暿惪媮寧侾.Text = txt摿暿怳懼寧俇.Text) And (txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Or (txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text)) Or _
            (txt摿暿惪媮寧俀.Text <> "" And txt摿暿怳懼寧俁.Text <> "") And ((txt摿暿惪媮寧俀.Text = txt摿暿怳懼寧俁.Text) And (txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text) Or (txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text)) Or _
            (txt摿暿惪媮寧俀.Text <> "" And txt摿暿怳懼寧係.Text <> "") And ((txt摿暿惪媮寧俀.Text = txt摿暿怳懼寧係.Text) And (txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text) Or (txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text)) Or _
            (txt摿暿惪媮寧俀.Text <> "" And txt摿暿怳懼寧俆.Text <> "") And ((txt摿暿惪媮寧俀.Text = txt摿暿怳懼寧俆.Text) And (txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text) Or (txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text)) Or _
            (txt摿暿惪媮寧俀.Text <> "" And txt摿暿怳懼寧俇.Text <> "") And ((txt摿暿惪媮寧俀.Text = txt摿暿怳懼寧俇.Text) And (txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Or (txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text)) Or _
            (txt摿暿惪媮寧俁.Text <> "" And txt摿暿怳懼寧係.Text <> "") And ((txt摿暿惪媮寧俁.Text = txt摿暿怳懼寧係.Text) And (txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text) Or (txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text)) Or _
            (txt摿暿惪媮寧俁.Text <> "" And txt摿暿怳懼寧俆.Text <> "") And ((txt摿暿惪媮寧俁.Text = txt摿暿怳懼寧俆.Text) And (txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text) Or (txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text)) Or _
            (txt摿暿惪媮寧俁.Text <> "" And txt摿暿怳懼寧俇.Text <> "") And ((txt摿暿惪媮寧俁.Text = txt摿暿怳懼寧俇.Text) And (txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Or (txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text)) Or _
            (txt摿暿惪媮寧係.Text <> "" And txt摿暿怳懼寧俆.Text <> "") And ((txt摿暿惪媮寧係.Text = txt摿暿怳懼寧係.Text) And (txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text) Or (txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text)) Or _
            (txt摿暿惪媮寧係.Text <> "" And txt摿暿怳懼寧俇.Text <> "") And ((txt摿暿惪媮寧係.Text = txt摿暿怳懼寧係.Text) And (txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Or (txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text)) Or _
            (txt摿暿惪媮寧俆.Text <> "" And txt摿暿怳懼寧俇.Text <> "") And ((txt摿暿惪媮寧俆.Text = txt摿暿怳懼寧俆.Text) And (txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text = txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text) Or (txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text = txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text)) Then


            MessageBox.Show("摨堦寧偱墣U擔脾嵞怳擔獜d暋偟偰偄傑偡丅", "摿暿怳懼擔", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function

        End If
        '****************************************************************************

        PFUNC_CHECK_SFURI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '摿暿僗働僕儏乕儖僠僃僢僋 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Private Function PFUNC_CHECK_TOKUBETSU() As Boolean
        PFUNC_CHECK_TOKUBETSU = False

        '------------------------------------------
        '摨堦怳懼擔偺搊榐偼偱偒側偄
        '------------------------------------------
        Dim blnCHECK(13) As Boolean ' 怳懼幚峴僠僃僢僋
        Dim blnSCHECK(13) As Boolean '嵞怳幚峴僠僃僢僋
        Dim strNyuuryoku(13) As String ' 怳懼擔棑偵擖椡偝傟偨抣
        Dim strSNyuuryoku(13) As String '嵞怳擔棑偵擖椡偝傟偨抣
        Dim strTsuujyou(13) As String '捠忢僗働僕儏乕儖
        Dim strTokubetsu(6) As String '摿暿僗働僕儏乕儖

        '塩嬈擔傪庢摼偟丄惪媮寧丒弶怳丒嵞怳傪侾偮偺暥帤楍偵寢崌
        '仭捠忢僗働僕儏乕儖暘乮strTsuujyou乯
        PFUNC_SET_EIGYOBI(chk係寧怳懼擔.Checked, "04", Trim(txt懳徾擭搙.Text), "04", Trim(txt係寧怳懼擔.Text), chk係寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "04", Trim(txt係寧嵞怳懼擔.Text), True, strTsuujyou(4))
        PFUNC_SET_EIGYOBI(chk俆寧怳懼擔.Checked, "05", Trim(txt懳徾擭搙.Text), "05", Trim(txt俆寧怳懼擔.Text), chk俆寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "05", Trim(txt俆寧嵞怳懼擔.Text), True, strTsuujyou(5))
        PFUNC_SET_EIGYOBI(chk俇寧怳懼擔.Checked, "06", Trim(txt懳徾擭搙.Text), "06", Trim(txt俇寧怳懼擔.Text), chk俇寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "06", Trim(txt俇寧嵞怳懼擔.Text), True, strTsuujyou(6))
        PFUNC_SET_EIGYOBI(chk俈寧怳懼擔.Checked, "07", Trim(txt懳徾擭搙.Text), "07", Trim(txt俈寧怳懼擔.Text), chk俈寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "07", Trim(txt俈寧嵞怳懼擔.Text), True, strTsuujyou(7))
        PFUNC_SET_EIGYOBI(chk俉寧怳懼擔.Checked, "08", Trim(txt懳徾擭搙.Text), "08", Trim(txt俉寧怳懼擔.Text), chk俉寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "08", Trim(txt俉寧嵞怳懼擔.Text), True, strTsuujyou(8))
        PFUNC_SET_EIGYOBI(chk俋寧怳懼擔.Checked, "09", Trim(txt懳徾擭搙.Text), "09", Trim(txt俋寧怳懼擔.Text), chk俋寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "09", Trim(txt俋寧嵞怳懼擔.Text), True, strTsuujyou(9))
        PFUNC_SET_EIGYOBI(chk侾侽寧怳懼擔.Checked, "10", Trim(txt懳徾擭搙.Text), "10", Trim(txt侾侽寧怳懼擔.Text), chk侾侽寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "10", Trim(txt侾侽寧嵞怳懼擔.Text), True, strTsuujyou(10))
        PFUNC_SET_EIGYOBI(chk侾侾寧怳懼擔.Checked, "11", Trim(txt懳徾擭搙.Text), "11", Trim(txt侾侾寧怳懼擔.Text), chk侾侾寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "11", Trim(txt侾侾寧嵞怳懼擔.Text), True, strTsuujyou(11))
        PFUNC_SET_EIGYOBI(chk侾俀寧怳懼擔.Checked, "12", Trim(txt懳徾擭搙.Text), "12", Trim(txt侾俀寧怳懼擔.Text), chk侾俀寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "12", Trim(txt侾俀寧嵞怳懼擔.Text), True, strTsuujyou(12))
        PFUNC_SET_EIGYOBI(chk侾寧怳懼擔.Checked, "01", Trim(txt懳徾擭搙.Text), "01", Trim(txt侾寧怳懼擔.Text), chk侾寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "01", Trim(txt侾寧嵞怳懼擔.Text), True, strTsuujyou(1))
        PFUNC_SET_EIGYOBI(chk俀寧怳懼擔.Checked, "02", Trim(txt懳徾擭搙.Text), "02", Trim(txt俀寧怳懼擔.Text), chk俀寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "02", Trim(txt俀寧嵞怳懼擔.Text), True, strTsuujyou(2))
        PFUNC_SET_EIGYOBI(chk俁寧怳懼擔.Checked, "03", Trim(txt懳徾擭搙.Text), "03", Trim(txt俁寧怳懼擔.Text), chk俁寧嵞怳懼擔.Checked, Trim(txt懳徾擭搙.Text), "03", Trim(txt俁寧嵞怳懼擔.Text), True, strTsuujyou(3))

        '仭摿暿僗働僕儏乕儖暘乮strTokubetsu乯
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧侾.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧侾.Text), Trim(txt摿暿怳懼擔侾.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧侾.Text), Trim(txt摿暿嵞怳懼擔侾.Text), False, strTokubetsu(0))
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧俀.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧俀.Text), Trim(txt摿暿怳懼擔俀.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧俀.Text), Trim(txt摿暿嵞怳懼擔俀.Text), False, strTokubetsu(1))
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧俁.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧俁.Text), Trim(txt摿暿怳懼擔俁.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧俁.Text), Trim(txt摿暿嵞怳懼擔俁.Text), False, strTokubetsu(2))
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧係.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧係.Text), Trim(txt摿暿怳懼擔係.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧係.Text), Trim(txt摿暿嵞怳懼擔係.Text), False, strTokubetsu(3))
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧俆.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧俆.Text), Trim(txt摿暿怳懼擔俆.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧俆.Text), Trim(txt摿暿嵞怳懼擔俆.Text), False, strTokubetsu(4))
        PFUNC_SET_EIGYOBI(True, Trim(txt摿暿惪媮寧俇.Text), Trim(txt懳徾擭搙.Text), Trim(txt摿暿怳懼寧俇.Text), Trim(txt摿暿怳懼擔俇.Text), True, Trim(txt懳徾擭搙.Text), Trim(txt摿暿嵞怳懼寧俇.Text), Trim(txt摿暿嵞怳懼擔俇.Text), False, strTokubetsu(5))

        '捠忢僗働僕儏乕儖偲摿暿僗働僕儏乕儖偺僠僃僢僋
        For i As Integer = 0 To 5
            If Trim(strTokubetsu(i)) <> "" Then '枹擖椡偺応崌丄僠僃僢僋偺昁梫側偟
                '仸strTokubetsu(i).Substring(0, 2)偼惪媮寧
                '2010/10/21 strTsuujyou偵偼怳懼擔亄嵞怳擔偑擖偭偰偄傞応崌偑偁傞偺偱峫椂偡傞
                'If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) = strTokubetsu(i) Then
                If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                   strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(0, 8) = strTokubetsu(i).Substring(0, 8) Then
                    MessageBox.Show("捠忢僗働僕儏乕儖偲摿暿僗働僕儏乕儖偵摨堦怳懼擔偺僨乕僞偑懚嵼偟傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

                '2010/10/21 嵞怳傕僠僃僢僋偡傞 偙偙偐傜
                If strTokubetsu(i).Length = 16 Then
                    If strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))) IsNot Nothing AndAlso _
                       strTsuujyou(CInt(strTokubetsu(i).Substring(4, 2))).PadRight(16).Substring(8, 8) = strTokubetsu(i).Substring(8, 8) Then
                        MessageBox.Show("捠忢僗働僕儏乕儖偲摿暿僗働僕儏乕儖偵摨堦嵞怳懼擔偺僨乕僞偑懚嵼偟傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                End If
                '2010/10/21 嵞怳傕僠僃僢僋偡傞 偙偙傑偱
            End If
        Next

        '摿暿僗働僕儏乕儖摨巑偺僠僃僢僋
        For i As Integer = 0 To 4
            If strTokubetsu(i) <> "" Then '枹擖椡偺応崌丄僠僃僢僋偺昁梫側偟
                For j As Integer = i + 1 To 5
                    If strTokubetsu(i) = strTokubetsu(j) Then
                        MessageBox.Show("摿暿僗働僕儏乕儖偵摨堦怳懼擔偺僨乕僞偑懚嵼偟傑偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_TOKUBETSU = True

    End Function

    '2010/10/21
    '悘帪僗働僕儏乕儖僠僃僢僋
    Private Function PFUNC_CHECK_ZUIJI() As Boolean
        PFUNC_CHECK_ZUIJI = False

        '------------------------------------------
        '摨堦擖弌嬫暘丄摨堦怳懼擔偺搊榐偼偱偒側偄
        '------------------------------------------
        Dim strZuiji(6) As String '悘帪僗働僕儏乕儖
        Dim intNsKbn(6) As Integer
        intNsKbn(0) = cmb擖弌嬫暘侾.SelectedIndex
        intNsKbn(1) = cmb擖弌嬫暘俀.SelectedIndex
        intNsKbn(2) = cmb擖弌嬫暘俁.SelectedIndex
        intNsKbn(3) = cmb擖弌嬫暘係.SelectedIndex
        intNsKbn(4) = cmb擖弌嬫暘俆.SelectedIndex
        intNsKbn(5) = cmb擖弌嬫暘俇.SelectedIndex

        '塩嬈擔傪庢摼
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧侾.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧侾.Text.Trim, txt悘帪怳懼擔侾.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧侾.Text.Trim, txt悘帪怳懼擔侾.Text.Trim, False, strZuiji(0))
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧俀.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俀.Text.Trim, txt悘帪怳懼擔俀.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俀.Text.Trim, txt悘帪怳懼擔俀.Text.Trim, False, strZuiji(1))
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧俁.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俁.Text.Trim, txt悘帪怳懼擔俁.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俁.Text.Trim, txt悘帪怳懼擔俁.Text.Trim, False, strZuiji(2))
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧係.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧係.Text.Trim, txt悘帪怳懼擔係.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧係.Text.Trim, txt悘帪怳懼擔係.Text.Trim, False, strZuiji(3))
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧俆.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俆.Text.Trim, txt悘帪怳懼擔俆.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俆.Text.Trim, txt悘帪怳懼擔俆.Text.Trim, False, strZuiji(4))
        PFUNC_SET_EIGYOBI(True, txt悘帪怳懼寧俇.Text.Trim, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俇.Text.Trim, txt悘帪怳懼擔俇.Text.Trim, True, txt懳徾擭搙.Text.Trim, txt悘帪怳懼寧俇.Text.Trim, txt悘帪怳懼擔俇.Text.Trim, False, strZuiji(5))

        '悘帪僗働僕儏乕儖摨巑偺僠僃僢僋
        For i As Integer = 0 To 4
            If strZuiji(i) <> "" Then '枹擖椡偺応崌丄僠僃僢僋偺昁梫側偟
                For j As Integer = i + 1 To 5
                    If intNsKbn(i) = intNsKbn(j) AndAlso strZuiji(i) = strZuiji(j) Then
                        MessageBox.Show("悘帪僗働僕儏乕儖偵摨堦擖弌嬫暘丄摨堦怳懼擔偺僨乕僞偑懚嵼偟傑偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Function
                    End If
                Next
            End If
        Next

        PFUNC_CHECK_ZUIJI = True

    End Function

    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    '塩嬈擔庢摼 2006/11/22
    '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
    Function PFUNC_SET_EIGYOBI(ByVal blnBOX As Boolean, ByVal strSeikyuTuki As String, ByVal strFuridateY As String, ByVal strFuridateM As String, ByVal strFuridateD As String, ByVal blnSBOX As Boolean, ByVal strSFuridateY As String, ByVal strSFuridateM As String, ByVal strSFuridateD As String, ByVal blnCheckFLG As Boolean, ByRef strReturnDate As String) As Boolean
        Dim strEigyobiY As String = "" '怳懼塩嬈擭
        Dim strEigyobiM As String = "" '怳懼塩嬈寧
        Dim strEigyobiD As String = "" '怳懼塩嬈擔
        Dim strSEigyobiY As String = "" '嵞怳塩嬈擭
        Dim strSEigyobiM As String = "" '嵞怳塩嬈寧
        Dim strSEigyobiD As String = "" '嵞怳塩嬈擔

        '惪媮寧偑嬻敀偺応崌丒怳懼偟側偄応崌丄庢摼偡傞昁梫側偟
        If strSeikyuTuki = "" Or blnBOX = False Then
            Exit Function
        End If

        '惪媮寧偑侾乣俁寧偺応崌偼擭搙傪曄偊傞
        If CInt(strSeikyuTuki) <= 3 Then
            strFuridateY = CStr(CInt(strFuridateY + 1))
            strSFuridateY = CStr(CInt(strSFuridateY + 1))
        End If

        '擔晅偑嬻敀偩偭偨応崌丄婎弨擔傪巊梡偡傞
        If blnCheckFLG = True Then
            If strFuridateD = "" Then
                strFuridateD = GAKKOU_INFO.FURI_DATE
            End If

            If blnSBOX = True And strSFuridateD = "" Then
                strSFuridateD = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        '塩嬈擔傪庢摼
        Dim FuriDate As String = fn_GetEigyoubi(strFuridateY & strFuridateM & strFuridateD, "0", "+")
        Dim SFuriDate As String = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")

        'START 20121114 maeda 廋惓 嵞怳懼擔偑枹擖椡帪偺峫椂傪捛壛
        '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------START
        If SFuriDate <> "" Then
            If FuriDate >= SFuriDate Then
                If strSFuridateM = "12" Then
                    strSFuridateY = (CInt(strSFuridateY) + 1).ToString("0000")
                    strSFuridateM = "01"
                Else
                    strSFuridateM = (CInt(strSFuridateM) + 1).ToString("00")
                End If
                SFuriDate = fn_GetEigyoubi(strSFuridateY & strSFuridateM & strSFuridateD, "0", "+")
            End If
        End If
        '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------END
        'END   20121114 maeda 廋惓 嵞怳懼擔偑枹擖椡帪偺峫椂傪捛壛

        '嵞怳僗働僕儏乕儖乮捠忢僗働僕儏乕儖偲寢崌偟丄侾偮偺曄悢偲偟偰曉偡乯
        strReturnDate = FuriDate & SFuriDate

    End Function

    '婇嬈帺怳楢実岦偗 2006/12/06
    Public Function fn_CHECK_CHANGE() As Boolean
        '================================================================
        '戅旔偟偨嶲徠帪偺僗働僕儏乕儖偑峏怴屻偺曄悢偵巆偭偰偄傞偐僠僃僢僋
        '峏怴屻偵巆偭偰偄側偄応崌=嶍彍偝傟偨偺偱婇嬈帺怳懁偺僗働僕儏乕儖傕嶍彍
        '================================================================

        fn_CHECK_CHANGE = False

        '擭娫僗働僕儏乕儖峏怴
        For i As Integer = 1 To 12
            '弶怳擭娫僠僃僢僋
            If strSYOFURI_NENKAN(i).Length = 8 And strSYOFURI_NENKAN(i) <> strSYOFURI_NENKAN_AFTER(i) Then

                For j As Integer = 1 To 6
                    '摿暿怳懼擔偲堦抳偡傞怳懼擔偑偁傞応崌丄嶍彍偟側偄偺偱儖乕僾傪敳偗傞
                    If strSYOFURI_NENKAN(i) = strSYOFURI_TOKUBETU_AFTER(j) And strSYOFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '摿暿怳懼擔联廔椆
                        If fn_DELETESCHMAST("01", strSYOFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
            '嵞怳擭娫僠僃僢僋
            If strSAIFURI_NENKAN(i).Length = 8 And strSAIFURI_NENKAN(i) <> strSAIFURI_NENKAN_AFTER(i) Then
                For j As Integer = 1 To 6
                    '摿暿怳懼擔偲堦抳偡傞怳懼擔偑偁傞応崌丄嶍彍偟側偄偺偱儖乕僾傪敳偗傞
                    If strSAIFURI_NENKAN(i) = strSAIFURI_TOKUBETU_AFTER(j) And strSAIFURI_TOKUBETU_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then '摿暿怳懼擔联廔椆
                        If fn_DELETESCHMAST("02", strSAIFURI_NENKAN(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next

            End If
        Next

        '摿暿峏怴
        For i As Integer = 1 To 6
            '弶怳摿暿僠僃僢僋
            If strSYOFURI_TOKUBETU(i).Length = 8 And strSYOFURI_TOKUBETU(i) <> strSYOFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '擭娫怳懼擔偲堦抳偡傞怳懼擔偑偁傞応崌丄嶍彍偟側偄偺偱儖乕僾傪敳偗傞
                    If strSYOFURI_TOKUBETU(i) = strSYOFURI_NENKAN_AFTER(j) And strSYOFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '擭娫怳懼擔联廔椆
                        If fn_DELETESCHMAST("01", strSYOFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
            '嵞怳摿暿僠僃僢僋
            If strSAIFURI_TOKUBETU(i).Length = 8 And strSAIFURI_TOKUBETU(i) <> strSAIFURI_TOKUBETU_AFTER(i) Then
                For j As Integer = 1 To 12
                    '擭娫怳懼擔偲堦抳偡傞怳懼擔偑偁傞応崌丄嶍彍偟側偄偺偱儖乕僾傪敳偗傞
                    If strSAIFURI_TOKUBETU(i) = strSAIFURI_NENKAN_AFTER(j) And strSAIFURI_NENKAN_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 12 Then '擭娫怳懼擔联廔椆
                        If fn_DELETESCHMAST("02", strSAIFURI_TOKUBETU(i)) = False Then
                            Exit Function
                        End If
                    End If
                Next
            End If
        Next

        '悘帪峏怴
        For i As Integer = 1 To 6
            If strFURI_ZUIJI(i).Length = 8 And strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) <> strFURIKBN_ZUIJI_AFTER(i) & strFURI_ZUIJI_AFTER(i) Then
                For j As Integer = 1 To 6
                    If strFURIKBN_ZUIJI(i) & strFURI_ZUIJI(i) = strFURIKBN_ZUIJI_AFTER(j) & strFURI_ZUIJI_AFTER(j) And strFURI_ZUIJI_AFTER(j).Length = 8 Then
                        Exit For
                    End If

                    If j = 6 Then
                        If strFURIKBN_ZUIJI(i) = "2" Then '擖嬥
                            If fn_DELETESCHMAST("03", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        Else '弌嬥
                            If fn_DELETESCHMAST("04", strFURI_ZUIJI(i)) = False Then
                                Exit Function
                            End If
                        End If
                    End If

                Next
            End If
        Next

        If Err.Number = 0 Then
            fn_CHECK_CHANGE = True
        End If
    End Function

#End Region

#Region " Private Sub(擭娫僗働僕儏乕儖)"
    Private Sub PSUB_NENKAN_GET(ByRef Get_Data() As NenkanData)

        Get_Data(4).Furikae_Check = chk係寧怳懼擔.Checked
        Get_Data(4).Furikae_Enabled = chk係寧怳懼擔.Enabled
        Get_Data(4).Furikae_Date = txt係寧怳懼擔.Text
        Get_Data(4).Furikae_Day = lab係寧怳懼擔.Text

        Get_Data(4).SaiFurikae_Check = chk係寧嵞怳懼擔.Checked
        Get_Data(4).SaiFurikae_Enabled = chk係寧嵞怳懼擔.Enabled
        Get_Data(4).SaiFurikae_Date = txt係寧嵞怳懼擔.Text
        Get_Data(4).SaiFurikae_Day = lab係寧嵞怳懼擔.Text

        Get_Data(5).Furikae_Check = chk俆寧怳懼擔.Checked
        Get_Data(5).Furikae_Enabled = chk俆寧怳懼擔.Enabled
        Get_Data(5).Furikae_Date = txt俆寧怳懼擔.Text
        Get_Data(5).Furikae_Day = lab俆寧怳懼擔.Text

        Get_Data(5).SaiFurikae_Check = chk俆寧嵞怳懼擔.Checked
        Get_Data(5).SaiFurikae_Enabled = chk俆寧嵞怳懼擔.Enabled
        Get_Data(5).SaiFurikae_Date = txt俆寧嵞怳懼擔.Text
        Get_Data(5).SaiFurikae_Day = lab俆寧嵞怳懼擔.Text

        Get_Data(6).Furikae_Check = chk俇寧怳懼擔.Checked
        Get_Data(6).Furikae_Enabled = chk俇寧怳懼擔.Enabled
        Get_Data(6).Furikae_Date = txt俇寧怳懼擔.Text
        Get_Data(6).Furikae_Day = lab俇寧怳懼擔.Text

        Get_Data(6).SaiFurikae_Check = chk俇寧嵞怳懼擔.Checked
        Get_Data(6).SaiFurikae_Enabled = chk俇寧嵞怳懼擔.Enabled
        Get_Data(6).SaiFurikae_Date = txt俇寧嵞怳懼擔.Text
        Get_Data(6).SaiFurikae_Day = lab俇寧嵞怳懼擔.Text

        Get_Data(7).Furikae_Check = chk俈寧怳懼擔.Checked
        Get_Data(7).Furikae_Enabled = chk俈寧怳懼擔.Enabled
        Get_Data(7).Furikae_Date = txt俈寧怳懼擔.Text
        Get_Data(7).Furikae_Day = lab俈寧怳懼擔.Text

        Get_Data(7).SaiFurikae_Check = chk俈寧嵞怳懼擔.Checked
        Get_Data(7).SaiFurikae_Enabled = chk俈寧嵞怳懼擔.Enabled
        Get_Data(7).SaiFurikae_Date = txt俈寧嵞怳懼擔.Text
        Get_Data(7).SaiFurikae_Day = lab俈寧嵞怳懼擔.Text

        Get_Data(8).Furikae_Check = chk俉寧怳懼擔.Checked
        Get_Data(8).Furikae_Enabled = chk俉寧怳懼擔.Enabled
        Get_Data(8).Furikae_Date = txt俉寧怳懼擔.Text
        Get_Data(8).Furikae_Day = lab俉寧怳懼擔.Text

        Get_Data(8).SaiFurikae_Check = chk俉寧嵞怳懼擔.Checked
        Get_Data(8).SaiFurikae_Enabled = chk俉寧嵞怳懼擔.Enabled
        Get_Data(8).SaiFurikae_Date = txt俉寧嵞怳懼擔.Text
        Get_Data(8).SaiFurikae_Day = lab俉寧嵞怳懼擔.Text

        Get_Data(9).Furikae_Check = chk俋寧怳懼擔.Checked
        Get_Data(9).Furikae_Enabled = chk俋寧怳懼擔.Enabled
        Get_Data(9).Furikae_Date = txt俋寧怳懼擔.Text
        Get_Data(9).Furikae_Day = lab俋寧怳懼擔.Text

        Get_Data(9).SaiFurikae_Check = chk俋寧嵞怳懼擔.Checked
        Get_Data(9).SaiFurikae_Enabled = chk俋寧嵞怳懼擔.Enabled
        Get_Data(9).SaiFurikae_Date = txt俋寧嵞怳懼擔.Text
        Get_Data(9).SaiFurikae_Day = lab俋寧嵞怳懼擔.Text

        Get_Data(10).Furikae_Check = chk侾侽寧怳懼擔.Checked
        Get_Data(10).Furikae_Enabled = chk侾侽寧怳懼擔.Enabled
        Get_Data(10).Furikae_Date = txt侾侽寧怳懼擔.Text
        Get_Data(10).Furikae_Day = lab侾侽寧怳懼擔.Text

        Get_Data(10).SaiFurikae_Check = chk侾侽寧嵞怳懼擔.Checked
        Get_Data(10).SaiFurikae_Enabled = chk侾侽寧嵞怳懼擔.Enabled
        Get_Data(10).SaiFurikae_Date = txt侾侽寧嵞怳懼擔.Text
        Get_Data(10).SaiFurikae_Day = lab侾侽寧嵞怳懼擔.Text

        Get_Data(11).Furikae_Check = chk侾侾寧怳懼擔.Checked
        Get_Data(11).Furikae_Enabled = chk侾侾寧怳懼擔.Enabled
        Get_Data(11).Furikae_Date = txt侾侾寧怳懼擔.Text
        Get_Data(11).Furikae_Day = lab侾侾寧怳懼擔.Text

        Get_Data(11).SaiFurikae_Check = chk侾侾寧嵞怳懼擔.Checked
        Get_Data(11).SaiFurikae_Enabled = chk侾侾寧嵞怳懼擔.Enabled
        Get_Data(11).SaiFurikae_Date = txt侾侾寧嵞怳懼擔.Text
        Get_Data(11).SaiFurikae_Day = lab侾侾寧嵞怳懼擔.Text

        Get_Data(12).Furikae_Check = chk侾俀寧怳懼擔.Checked
        Get_Data(12).Furikae_Enabled = chk侾俀寧怳懼擔.Enabled
        Get_Data(12).Furikae_Date = txt侾俀寧怳懼擔.Text
        Get_Data(12).Furikae_Day = lab侾俀寧怳懼擔.Text

        Get_Data(12).SaiFurikae_Check = chk侾俀寧嵞怳懼擔.Checked
        Get_Data(12).SaiFurikae_Enabled = chk侾俀寧嵞怳懼擔.Enabled
        Get_Data(12).SaiFurikae_Date = txt侾俀寧嵞怳懼擔.Text
        Get_Data(12).SaiFurikae_Day = lab侾俀寧嵞怳懼擔.Text

        Get_Data(1).Furikae_Check = chk侾寧怳懼擔.Checked
        Get_Data(1).Furikae_Enabled = chk侾寧怳懼擔.Enabled
        Get_Data(1).Furikae_Date = txt侾寧怳懼擔.Text
        Get_Data(1).Furikae_Day = lab侾寧怳懼擔.Text

        Get_Data(1).SaiFurikae_Check = chk侾寧嵞怳懼擔.Checked
        Get_Data(1).SaiFurikae_Enabled = chk侾寧嵞怳懼擔.Enabled
        Get_Data(1).SaiFurikae_Date = txt侾寧嵞怳懼擔.Text
        Get_Data(1).SaiFurikae_Day = lab侾寧嵞怳懼擔.Text

        Get_Data(2).Furikae_Check = chk俀寧怳懼擔.Checked
        Get_Data(2).Furikae_Enabled = chk俀寧怳懼擔.Enabled
        Get_Data(2).Furikae_Date = txt俀寧怳懼擔.Text
        Get_Data(2).Furikae_Day = lab俀寧怳懼擔.Text

        Get_Data(2).SaiFurikae_Check = chk俀寧嵞怳懼擔.Checked
        Get_Data(2).SaiFurikae_Enabled = chk俀寧嵞怳懼擔.Enabled
        Get_Data(2).SaiFurikae_Date = txt俀寧嵞怳懼擔.Text
        Get_Data(2).SaiFurikae_Day = lab俀寧嵞怳懼擔.Text

        Get_Data(3).Furikae_Check = chk俁寧怳懼擔.Checked
        Get_Data(3).Furikae_Enabled = chk俁寧怳懼擔.Enabled
        Get_Data(3).Furikae_Date = txt俁寧怳懼擔.Text
        Get_Data(3).Furikae_Day = lab俁寧怳懼擔.Text

        Get_Data(3).SaiFurikae_Check = chk俁寧嵞怳懼擔.Checked
        Get_Data(3).SaiFurikae_Enabled = chk俁寧嵞怳懼擔.Enabled
        Get_Data(3).SaiFurikae_Date = txt俁寧嵞怳懼擔.Text
        Get_Data(3).SaiFurikae_Day = lab俁寧嵞怳懼擔.Text

    End Sub

#End Region

#Region " Private Sub(擭娫僗働僕儏乕儖夋柺惂屼)"
    Private Sub PSUB_NENKAN_FORMAT()

        '擭娫僗働僕儏乕儖晹暘弶婜昞帵

        '僠僃僢僋儃僢僋僗抣
        Call PSUB_NENKAN_CHK(True)

        '僠僃僢僋儃僢僋僗Enable抣
        Call PSUB_NENKAN_CHKBOXEnabled(True)

        '僥僉僗僩儃僢僋僗
        Call PSUB_NENKAN_DAYCLER()

        '僥僉僗僩儃僢僋僗Enable抣
        Call PSUB_NENKAN_TEXTEnabled(True)

        '昞帵梡儔儀儖
        Call PSUB_NENKAN_LABCLER()

    End Sub
    Private Sub PSUB_NENKAN_CHK(ByVal pValue As Boolean)

        '怳懼擔偺桳岠僠僃僢僋
        chk係寧怳懼擔.Checked = pValue
        chk俆寧怳懼擔.Checked = pValue
        chk俇寧怳懼擔.Checked = pValue
        chk俈寧怳懼擔.Checked = pValue
        chk俉寧怳懼擔.Checked = pValue
        chk俋寧怳懼擔.Checked = pValue
        chk侾侽寧怳懼擔.Checked = pValue
        chk侾侾寧怳懼擔.Checked = pValue
        chk侾俀寧怳懼擔.Checked = pValue
        chk侾寧怳懼擔.Checked = pValue
        chk俀寧怳懼擔.Checked = pValue
        chk俁寧怳懼擔.Checked = pValue

        '嵞怳懼擔偺桳岠僠僃僢僋
        chk係寧嵞怳懼擔.Checked = pValue
        chk俆寧嵞怳懼擔.Checked = pValue
        chk俇寧嵞怳懼擔.Checked = pValue
        chk俈寧嵞怳懼擔.Checked = pValue
        chk俉寧嵞怳懼擔.Checked = pValue
        chk俋寧嵞怳懼擔.Checked = pValue
        chk侾侽寧嵞怳懼擔.Checked = pValue
        chk侾侾寧嵞怳懼擔.Checked = pValue
        chk侾俀寧嵞怳懼擔.Checked = pValue
        chk侾寧嵞怳懼擔.Checked = pValue
        chk俀寧嵞怳懼擔.Checked = pValue
        chk俁寧嵞怳懼擔.Checked = pValue

    End Sub
    Private Sub PSUB_NENKAN_CHKBOXEnabled(ByVal pValue As Boolean)

        '怳懼擔僠僃僢僋BOX偺桳岠壔
        chk係寧怳懼擔.Enabled = pValue
        chk俆寧怳懼擔.Enabled = pValue
        chk俇寧怳懼擔.Enabled = pValue
        chk俈寧怳懼擔.Enabled = pValue
        chk俉寧怳懼擔.Enabled = pValue
        chk俋寧怳懼擔.Enabled = pValue
        chk侾侽寧怳懼擔.Enabled = pValue
        chk侾侾寧怳懼擔.Enabled = pValue
        chk侾俀寧怳懼擔.Enabled = pValue
        chk侾寧怳懼擔.Enabled = pValue
        chk俀寧怳懼擔.Enabled = pValue
        chk俁寧怳懼擔.Enabled = pValue

        '嵞怳懼擔僠僃僢僋BOX偺桳岠壔
        chk係寧嵞怳懼擔.Enabled = pValue
        chk俆寧嵞怳懼擔.Enabled = pValue
        chk俇寧嵞怳懼擔.Enabled = pValue
        chk俈寧嵞怳懼擔.Enabled = pValue
        chk俉寧嵞怳懼擔.Enabled = pValue
        chk俋寧嵞怳懼擔.Enabled = pValue
        chk侾侽寧嵞怳懼擔.Enabled = pValue
        chk侾侾寧嵞怳懼擔.Enabled = pValue
        chk侾俀寧嵞怳懼擔.Enabled = pValue
        chk侾寧嵞怳懼擔.Enabled = pValue
        chk俀寧嵞怳懼擔.Enabled = pValue
        chk俁寧嵞怳懼擔.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_DAYCLER()

        '怳懼擔偺僋儕傾張棟
        txt係寧怳懼擔.Text = ""
        txt俆寧怳懼擔.Text = ""
        txt俇寧怳懼擔.Text = ""
        txt俈寧怳懼擔.Text = ""
        txt俉寧怳懼擔.Text = ""
        txt俋寧怳懼擔.Text = ""
        txt侾侽寧怳懼擔.Text = ""
        txt侾侾寧怳懼擔.Text = ""
        txt侾俀寧怳懼擔.Text = ""
        txt侾寧怳懼擔.Text = ""
        txt俀寧怳懼擔.Text = ""
        txt俁寧怳懼擔.Text = ""

        '嵞怳懼擔偺僋儕傾張棟
        txt係寧嵞怳懼擔.Text = ""
        txt俆寧嵞怳懼擔.Text = ""
        txt俇寧嵞怳懼擔.Text = ""
        txt俈寧嵞怳懼擔.Text = ""
        txt俉寧嵞怳懼擔.Text = ""
        txt俋寧嵞怳懼擔.Text = ""
        txt侾侽寧嵞怳懼擔.Text = ""
        txt侾侾寧嵞怳懼擔.Text = ""
        txt侾俀寧嵞怳懼擔.Text = ""
        txt侾寧嵞怳懼擔.Text = ""
        txt俀寧嵞怳懼擔.Text = ""
        txt俁寧嵞怳懼擔.Text = ""

    End Sub
    Private Sub PSUB_NENKAN_TEXTEnabled(ByVal pValue As Boolean)

        '怳懼擔僥僉僗僩BOX偺桳岠壔
        txt係寧怳懼擔.Enabled = pValue
        txt俆寧怳懼擔.Enabled = pValue
        txt俇寧怳懼擔.Enabled = pValue
        txt俈寧怳懼擔.Enabled = pValue
        txt俉寧怳懼擔.Enabled = pValue
        txt俋寧怳懼擔.Enabled = pValue
        txt侾侽寧怳懼擔.Enabled = pValue
        txt侾侾寧怳懼擔.Enabled = pValue
        txt侾俀寧怳懼擔.Enabled = pValue
        txt侾寧怳懼擔.Enabled = pValue
        txt俀寧怳懼擔.Enabled = pValue
        txt俁寧怳懼擔.Enabled = pValue

        '怳懼擔僥僉僗僩BOX偺桳岠壔
        txt係寧嵞怳懼擔.Enabled = pValue
        txt俆寧嵞怳懼擔.Enabled = pValue
        txt俇寧嵞怳懼擔.Enabled = pValue
        txt俈寧嵞怳懼擔.Enabled = pValue
        txt俉寧嵞怳懼擔.Enabled = pValue
        txt俋寧嵞怳懼擔.Enabled = pValue
        txt侾侽寧嵞怳懼擔.Enabled = pValue
        txt侾侾寧嵞怳懼擔.Enabled = pValue
        txt侾俀寧嵞怳懼擔.Enabled = pValue
        txt侾寧嵞怳懼擔.Enabled = pValue
        txt俀寧嵞怳懼擔.Enabled = pValue
        txt俁寧嵞怳懼擔.Enabled = pValue

    End Sub
    Private Sub PSUB_NENKAN_LABCLER()

        '擭娫僗働僕儏乕儖偺怳懼擔儔儀儖丄嵞怳懼擔儔儀儖偺僋儕傾
        lab係寧怳懼擔.Text = ""
        lab俆寧怳懼擔.Text = ""
        lab俇寧怳懼擔.Text = ""
        lab俈寧怳懼擔.Text = ""
        lab俉寧怳懼擔.Text = ""
        lab俋寧怳懼擔.Text = ""
        lab侾侽寧怳懼擔.Text = ""
        lab侾侾寧怳懼擔.Text = ""
        lab侾俀寧怳懼擔.Text = ""
        lab侾寧怳懼擔.Text = ""
        lab俀寧怳懼擔.Text = ""
        lab俁寧怳懼擔.Text = ""

        lab係寧嵞怳懼擔.Text = ""
        lab俆寧嵞怳懼擔.Text = ""
        lab俇寧嵞怳懼擔.Text = ""
        lab俈寧嵞怳懼擔.Text = ""
        lab俉寧嵞怳懼擔.Text = ""
        lab俋寧嵞怳懼擔.Text = ""
        lab侾侽寧嵞怳懼擔.Text = ""
        lab侾侾寧嵞怳懼擔.Text = ""
        lab侾俀寧嵞怳懼擔.Text = ""
        lab侾寧嵞怳懼擔.Text = ""
        lab俀寧嵞怳懼擔.Text = ""
        lab俁寧嵞怳懼擔.Text = ""

    End Sub
    Private Sub PSUB_SAIFURI_PROTECT(ByVal pValue As Boolean, Optional ByVal pTuki As Integer = 0)

        '怳懼擔桳岠僠僃僢僋偲怳懼擔擖椡棑偺僾儘僥僋僩(ON/OFF)張棟
        Select Case pTuki
            Case 0
                '慡寧懳徾
                chk係寧嵞怳懼擔.Checked = pValue
                chk係寧嵞怳懼擔.Enabled = pValue
                txt係寧嵞怳懼擔.Enabled = pValue

                chk俆寧嵞怳懼擔.Checked = pValue
                chk俆寧嵞怳懼擔.Enabled = pValue
                txt俆寧嵞怳懼擔.Enabled = pValue

                chk俇寧嵞怳懼擔.Checked = pValue
                chk俇寧嵞怳懼擔.Enabled = pValue
                txt俇寧嵞怳懼擔.Enabled = pValue

                chk俈寧嵞怳懼擔.Checked = pValue
                chk俈寧嵞怳懼擔.Enabled = pValue
                txt俈寧嵞怳懼擔.Enabled = pValue

                chk俉寧嵞怳懼擔.Checked = pValue
                chk俉寧嵞怳懼擔.Enabled = pValue
                txt俉寧嵞怳懼擔.Enabled = pValue

                chk俋寧嵞怳懼擔.Checked = pValue
                chk俋寧嵞怳懼擔.Enabled = pValue
                txt俋寧嵞怳懼擔.Enabled = pValue

                chk侾侽寧嵞怳懼擔.Checked = pValue
                chk侾侽寧嵞怳懼擔.Enabled = pValue
                txt侾侽寧嵞怳懼擔.Enabled = pValue

                chk侾侾寧嵞怳懼擔.Checked = pValue
                chk侾侾寧嵞怳懼擔.Enabled = pValue
                txt侾侾寧嵞怳懼擔.Enabled = pValue

                chk侾俀寧嵞怳懼擔.Checked = pValue
                chk侾俀寧嵞怳懼擔.Enabled = pValue
                txt侾俀寧嵞怳懼擔.Enabled = pValue

                chk侾寧嵞怳懼擔.Checked = pValue
                chk侾寧嵞怳懼擔.Enabled = pValue
                txt侾寧嵞怳懼擔.Enabled = pValue

                chk俀寧嵞怳懼擔.Checked = pValue
                chk俀寧嵞怳懼擔.Enabled = pValue
                txt俀寧嵞怳懼擔.Enabled = pValue

                chk俁寧嵞怳懼擔.Checked = pValue
                chk俁寧嵞怳懼擔.Enabled = pValue
                txt俁寧嵞怳懼擔.Enabled = pValue
            Case 1
                '侾寧
                chk侾寧嵞怳懼擔.Checked = pValue
                chk侾寧嵞怳懼擔.Enabled = pValue
                txt侾寧嵞怳懼擔.Enabled = pValue
            Case 2
                '俀寧
                chk俀寧嵞怳懼擔.Checked = pValue
                chk俀寧嵞怳懼擔.Enabled = pValue
                txt俀寧嵞怳懼擔.Enabled = pValue
            Case 3
                '俁寧
                chk俁寧嵞怳懼擔.Checked = pValue
                chk俁寧嵞怳懼擔.Enabled = pValue
                txt俁寧嵞怳懼擔.Enabled = pValue
            Case 4
                '係寧
                chk係寧嵞怳懼擔.Checked = pValue
                chk係寧嵞怳懼擔.Enabled = pValue
                txt係寧嵞怳懼擔.Enabled = pValue
            Case 5
                '俆寧
                chk俆寧嵞怳懼擔.Checked = pValue
                chk俆寧嵞怳懼擔.Enabled = pValue
                txt俆寧嵞怳懼擔.Enabled = pValue
            Case 6
                '俇寧
                chk俇寧嵞怳懼擔.Checked = pValue
                chk俇寧嵞怳懼擔.Enabled = pValue
                txt俇寧嵞怳懼擔.Enabled = pValue
            Case 7
                '俈寧
                chk俈寧嵞怳懼擔.Checked = pValue
                chk俈寧嵞怳懼擔.Enabled = pValue
                txt俈寧嵞怳懼擔.Enabled = pValue
            Case 8
                '俉寧
                chk俉寧嵞怳懼擔.Checked = pValue
                chk俉寧嵞怳懼擔.Enabled = pValue
                txt俉寧嵞怳懼擔.Enabled = pValue
            Case 9
                '俋寧
                chk俋寧嵞怳懼擔.Checked = pValue
                chk俋寧嵞怳懼擔.Enabled = pValue
                txt俋寧嵞怳懼擔.Enabled = pValue
            Case 10
                '侾侽寧
                chk侾侽寧嵞怳懼擔.Checked = pValue
                chk侾侽寧嵞怳懼擔.Enabled = pValue
                txt侾侽寧嵞怳懼擔.Enabled = pValue
            Case 11
                '侾侾寧
                chk侾侾寧嵞怳懼擔.Checked = pValue
                chk侾侾寧嵞怳懼擔.Enabled = pValue
                txt侾侾寧嵞怳懼擔.Enabled = pValue
            Case 12
                '侾俀寧
                chk侾俀寧嵞怳懼擔.Checked = pValue
                chk侾俀寧嵞怳懼擔.Enabled = pValue
                txt侾俀寧嵞怳懼擔.Enabled = pValue
        End Select

    End Sub

    Private Sub PSUB_NENKAN_SET(ByVal A As CheckBox, ByVal B As TextBox, ByVal C As Label, ByVal aReader As MyOracleReader)

        '擭娫僗働僕儏乕儖偺怳懼擔桳岠僠僃僢僋丄怳懼擔丄擔晅昞帵丄嵞怳懼擔桳岠僠僃僢僋丄怳懼擔丄擔晅昞帵偺曇廤
        A.Checked = True

        '梊旛椞堟侾偐傜擖椡偝傟偨怳懼擔傪摼傞
        B.Text = Trim(aReader.GetString("YOBI1_S"))
        C.Text = Mid(aReader.GetString("FURI_DATE_S"), 1, 4) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 5, 2) & "/" & Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        '張棟僼儔僌敾掕
        '擔忢嬈柋張棟拞偼曇廤偱偒側偄
        A.Enabled = False
        B.Enabled = False
        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
            Case aReader.GetString("CHECK_FLG_S") = "1"
            Case aReader.GetString("DATA_FLG_S") = "1"
            Case aReader.GetString("FUNOU_FLG_S") = "1"
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
            Case aReader.GetString("KESSAI_FLG_S") = "1"
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
            Case Else
                A.Enabled = True
                B.Enabled = True
        End Select

    End Sub
#End Region

#Region " Private Function(擭娫僗働僕儏乕儖)"
    Private Function PFUNC_SCH_GET_NENKAN() As Boolean

        PFUNC_SCH_GET_NENKAN = False

        '怳懼擔偺桳岠僠僃僢僋OFF丄嵞怳懼擔偺桳岠僠僃僢僋OFF
        Call PSUB_NENKAN_CHK(False)

        '怳懼擔擖椡棑丄嵞怳懼擔擖椡棑偺僋儕傾
        Call PSUB_NENKAN_DAYCLER()

        '怳懼擔丄嵞怳懼擔儔儀儖僋儕傾
        Call PSUB_NENKAN_LABCLER()

        If PFUNC_NENKAN_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_NENKAN = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_NENKAN() As Boolean

        '擭娫僗働僕儏乕儖峏怴張棟
        If PFUNC_NENKAN_KOUSIN() = False Then
            '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
            Int_Syori_Flag(0) = 2
            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_NENKAN_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String, ByVal astrFURI_DATE As String) As Boolean

        Dim iGakunen(8) As Integer
        Dim iCount As Integer
        Dim bFlg As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '捠忢儗僐乕僪偺懚嵼僠僃僢僋
        PFUNC_SCH_NENKAN_GET = True

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

        If oraReader.DataReader(sql) = False Then
            '捠忢儗僐亅僪柍偟
            oraReader.Close()
            Exit Function
        End If
        oraReader.Close()

        PFUNC_SCH_NENKAN_GET = False
        bFlg = False

        sql = New StringBuilder(128)

        '妛擭巜掕偑側偄応崌偼張棟傪偟側偄
        If PFUNC_GAKUNEN_GET(iGakunen) = True Then
            '摿暿儗僐乕僪偺懳徾妛擭僼儔僌偺忬懺傪尦偵捠忢儗僐乕僪偺懳徾妛擭僼儔僌傪俷俥俥偵偡傞
            '俷俶偵偡傞婡擻傪帩偨偣偨応崌丄摿暿儗僐乕僪偑暋悢審懚嵼偟偨応崌偵慜儗僐乕僪偱偺張棟偑柍懯偵側傞
            '摿暿儗僐乕僪偺懳徾妛擭侾僼儔僌偑乽侾乿偺応崌
            sql.Append(" UPDATE  G_SCHMAST")
            sql.Append(" SET ")
            For iCount = 1 To 9
                If iGakunen(iCount - 1) = 1 Then
                    If bFlg = False Then
                        sql.Append(" ")

                        bFlg = True
                    Else
                        sql.Append(",")
                    End If

                    sql.Append(" GAKUNEN" & iCount & "_FLG_S ='0'")
                End If
            Next iCount
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S ='0'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_SCH_NENKAN_GET = True

    End Function
    Private Function PFUNC_GAKUNEN_GET(ByRef pValue() As Integer) As Boolean

        PFUNC_GAKUNEN_GET = False

        ReDim pValue(8)

        If STR侾妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(0) = 1
        Else
            pValue(0) = 0
        End If
        If STR俀妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(1) = 1
        Else
            pValue(1) = 0
        End If
        If STR俁妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(2) = 1
        Else
            pValue(2) = 0
        End If
        If STR係妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(3) = 1
        Else
            pValue(3) = 0
        End If
        If STR俆妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(4) = 1
        Else
            pValue(4) = 0
        End If
        If STR俇妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(5) = 1
        Else
            pValue(5) = 0
        End If
        If STR俈妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(6) = 1
        Else
            pValue(6) = 0
        End If
        If STR俉妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(7) = 1
        Else
            pValue(7) = 0
        End If
        If STR俋妛擭 = "1" Then
            PFUNC_GAKUNEN_GET = True

            pValue(8) = 1
        Else
            pValue(8) = 0
        End If

    End Function

    Private Function PFUNC_NENKAN_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '擭娫僗働僕儏乕儖丂嶲徠張棟
        PFUNC_NENKAN_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 0")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF
            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    '弶怳僗働僕儏乕儖
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"   '怳懼擔偺寧
                            Call PSUB_NENKAN_SET(chk係寧怳懼擔, txt係寧怳懼擔, lab係寧怳懼擔, oraReader)
                            '2006/11/22丂昞帵帪偺怳懼擔傪庢摼
                            str捠忢怳懼擔(4) = Replace(lab係寧怳懼擔.Text, "/", "")
                            '2006/11/30丂僠僃僢僋僼儔僌丒晄擻僼儔僌傪峔憿懱偵奿擺
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(4).FunouFurikae_Flag = False
                            End If
                        Case "05"
                            Call PSUB_NENKAN_SET(chk俆寧怳懼擔, txt俆寧怳懼擔, lab俆寧怳懼擔, oraReader)
                            str捠忢怳懼擔(5) = Replace(lab俆寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(5).FunouFurikae_Flag = False
                            End If
                        Case "06"
                            Call PSUB_NENKAN_SET(chk俇寧怳懼擔, txt俇寧怳懼擔, lab俇寧怳懼擔, oraReader)
                            str捠忢怳懼擔(6) = Replace(lab俇寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(6).FunouFurikae_Flag = False
                            End If
                        Case "07"
                            Call PSUB_NENKAN_SET(chk俈寧怳懼擔, txt俈寧怳懼擔, lab俈寧怳懼擔, oraReader)
                            str捠忢怳懼擔(7) = Replace(lab俈寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(7).FunouFurikae_Flag = False
                            End If
                        Case "08"
                            Call PSUB_NENKAN_SET(chk俉寧怳懼擔, txt俉寧怳懼擔, lab俉寧怳懼擔, oraReader)
                            str捠忢怳懼擔(8) = Replace(lab俉寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(8).FunouFurikae_Flag = False
                            End If
                        Case "09"
                            Call PSUB_NENKAN_SET(chk俋寧怳懼擔, txt俋寧怳懼擔, lab俋寧怳懼擔, oraReader)
                            str捠忢怳懼擔(9) = Replace(lab俋寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(9).FunouFurikae_Flag = False
                            End If
                        Case "10"
                            Call PSUB_NENKAN_SET(chk侾侽寧怳懼擔, txt侾侽寧怳懼擔, lab侾侽寧怳懼擔, oraReader)
                            str捠忢怳懼擔(10) = Replace(lab侾侽寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(10).FunouFurikae_Flag = False
                            End If
                        Case "11"
                            Call PSUB_NENKAN_SET(chk侾侾寧怳懼擔, txt侾侾寧怳懼擔, lab侾侾寧怳懼擔, oraReader)
                            str捠忢怳懼擔(11) = Replace(lab侾侾寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(11).FunouFurikae_Flag = False
                            End If
                        Case "12"
                            Call PSUB_NENKAN_SET(chk侾俀寧怳懼擔, txt侾俀寧怳懼擔, lab侾俀寧怳懼擔, oraReader)
                            str捠忢怳懼擔(12) = Replace(lab侾俀寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(12).FunouFurikae_Flag = False
                            End If
                        Case "01"
                            Call PSUB_NENKAN_SET(chk侾寧怳懼擔, txt侾寧怳懼擔, lab侾寧怳懼擔, oraReader)
                            str捠忢怳懼擔(1) = Replace(lab侾寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(1).FunouFurikae_Flag = False
                            End If
                        Case "02"
                            Call PSUB_NENKAN_SET(chk俀寧怳懼擔, txt俀寧怳懼擔, lab俀寧怳懼擔, oraReader)
                            str捠忢怳懼擔(2) = Replace(lab俀寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(2).FunouFurikae_Flag = False
                            End If
                        Case "03"
                            Call PSUB_NENKAN_SET(chk俁寧怳懼擔, txt俁寧怳懼擔, lab俁寧怳懼擔, oraReader)
                            str捠忢怳懼擔(3) = Replace(lab俁寧怳懼擔.Text, "/", "")
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).CheckFurikae_Flag = False
                            End If
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = True
                            Else
                                SYOKI_NENKAN_SCHINFO(3).FunouFurikae_Flag = False
                            End If
                    End Select
                Case "1"
                    '嵞怳僗働僕儏乕儖
                    Select Case Mid(oraReader.GetString("NENGETUDO_S"), 5, 2)
                        Case "04"    '嵞怳懼擔偺寧
                            Call PSUB_NENKAN_SET(chk係寧嵞怳懼擔, txt係寧嵞怳懼擔, lab係寧嵞怳懼擔, oraReader)
                            '2006/11/22丂昞帵帪偺怳懼擔傪庢摼
                            str捠忢嵞怳擔(4) = Replace(lab係寧嵞怳懼擔.Text, "/", "")
                            '2006/11/30丂嵞怳擔偺嵞怳擔傪媮傔傞
                            str捠忢嵞乆怳擔(4) = oraReader.GetString("SFURI_DATE_S")
                            '2006/11/30丂僠僃僢僋僼儔僌傪庢摼
                            SYOKI_NENKAN_SCHINFO(4).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "05"
                            Call PSUB_NENKAN_SET(chk俆寧嵞怳懼擔, txt俆寧嵞怳懼擔, lab俆寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(5) = Replace(lab俆寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(5) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(5).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "06"
                            Call PSUB_NENKAN_SET(chk俇寧嵞怳懼擔, txt俇寧嵞怳懼擔, lab俇寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(6) = Replace(lab俇寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(6) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(6).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "07"
                            Call PSUB_NENKAN_SET(chk俈寧嵞怳懼擔, txt俈寧嵞怳懼擔, lab俈寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(7) = Replace(lab俈寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(7) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(7).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "08"
                            Call PSUB_NENKAN_SET(chk俉寧嵞怳懼擔, txt俉寧嵞怳懼擔, lab俉寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(8) = Replace(lab俉寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(8) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(8).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "09"
                            Call PSUB_NENKAN_SET(chk俋寧嵞怳懼擔, txt俋寧嵞怳懼擔, lab俋寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(9) = Replace(lab俋寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(9) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(9).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "10"
                            Call PSUB_NENKAN_SET(chk侾侽寧嵞怳懼擔, txt侾侽寧嵞怳懼擔, lab侾侽寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(10) = Replace(lab侾侽寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(10) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(10).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "11"
                            Call PSUB_NENKAN_SET(chk侾侾寧嵞怳懼擔, txt侾侾寧嵞怳懼擔, lab侾侾寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(11) = Replace(lab侾侾寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(11) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(11).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "12"
                            Call PSUB_NENKAN_SET(chk侾俀寧嵞怳懼擔, txt侾俀寧嵞怳懼擔, lab侾俀寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(12) = Replace(lab侾俀寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(12) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(12).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "01"
                            Call PSUB_NENKAN_SET(chk侾寧嵞怳懼擔, txt侾寧嵞怳懼擔, lab侾寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(1) = Replace(lab侾寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(1) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(1).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "02"
                            Call PSUB_NENKAN_SET(chk俀寧嵞怳懼擔, txt俀寧嵞怳懼擔, lab俀寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(2) = Replace(lab俀寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(2) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(2).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                        Case "03"
                            Call PSUB_NENKAN_SET(chk俁寧嵞怳懼擔, txt俁寧嵞怳懼擔, lab俁寧嵞怳懼擔, oraReader)
                            str捠忢嵞怳擔(3) = Replace(lab俁寧嵞怳懼擔.Text, "/", "")
                            str捠忢嵞乆怳擔(3) = oraReader.GetString("SFURI_DATE_S")
                            SYOKI_NENKAN_SCHINFO(3).CheckSaiFurikae_Flag = oraReader.GetString("CHECK_FLG_S")
                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        PFUNC_NENKAN_SANSYOU = True

    End Function
    Private Function PFUNC_NENKAN_DATE_CHECK(ByVal pFurikae As String, ByVal pSaifuri As String) As Boolean

        PFUNC_NENKAN_DATE_CHECK = False

        '怳懼擔偲嵞怳懼擔偑摨堦丠
        If Trim(pFurikae) <> "" And Trim(pSaifuri) <> "" Then
            If Trim(pFurikae) = Trim(pSaifuri) Then
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DATE_CHECK = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI() As Boolean

        Dim sTuki As String

        PFUNC_NENKAN_SAKUSEI = False

        ''擖椡撪梕傪曄悢偵戅旔
        ''丂屻偺張棟傪娙棯壔偡傞堊偵昁梫
        'Call PSUB_NENKAN_GET() '2006/11/30丂僐儊儞僩壔

        '怳懼擔偲嵞怳懼擔偑摨堦偺応崌偼僄儔乕
        For i As Integer = 1 To 12
            If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Check = True Then
                If PFUNC_NENKAN_DATE_CHECK(NENKAN_SCHINFO(i).Furikae_Date, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                    MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "怳懼擔偲嵞怳懼擔偑摨堦偱偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        Next i

        '怳懼擔僠僃僢僋
        For i As Integer = 1 To 12
            If bln擭娫峏怴(i) = True Then '2006/11/30丂峏怴偺側偄傕偺偼峏怴偺昁梫側偟

                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR侾妛擭 = "1"
                    STR俀妛擭 = "1"
                    STR俁妛擭 = "1"
                    STR係妛擭 = "1"
                    STR俆妛擭 = "1"
                    STR俇妛擭 = "1"
                    STR俈妛擭 = "1"
                    STR俉妛擭 = "1"
                    STR俋妛擭 = "1"

                    '僷儔儊僞偼嘆寧 嘇擖椡怳懼擔 嘊嵞怳懼寧 嘋嵞怳懼擔
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                Exit Function
                            End If
                    End Select

                    '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                    Int_Syori_Flag(0) = 1
                Else
                    '弶怳偺僗働僕儏乕儖偑張棟拞偱傕嵞怳偺傎偆偼
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR侾妛擭 = "1"
                        STR俀妛擭 = "1"
                        STR俁妛擭 = "1"
                        STR係妛擭 = "1"
                        STR俆妛擭 = "1"
                        STR俇妛擭 = "1"
                        STR俈妛擭 = "1"
                        STR俉妛擭 = "1"
                        STR俋妛擭 = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '嶌惉偟偨嵞怳偺僗働僕儏乕儖偺怳懼擔傪弶怳偺僗働僕儏乕儖偺嵞怳擔傊峏怴偡傞
                        If PFUNC_SCHMAST_UPDATE_SFURIDATE("0") = False Then

                            Exit Function
                        End If
                        '捛婰 2006/12/04
                        '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            Else '峏怴偟側偔偰傕婇嬈懁偺僗働僕儏乕儖傪尒傞

                '婇嬈帺怳楢実帪偺傒
                If NENKAN_SCHINFO(i).Furikae_Check = True And NENKAN_SCHINFO(i).Furikae_Enabled = True Then
                    sTuki = Format(i, "00")

                    STR侾妛擭 = "1"
                    STR俀妛擭 = "1"
                    STR俁妛擭 = "1"
                    STR係妛擭 = "1"
                    STR俆妛擭 = "1"
                    STR俇妛擭 = "1"
                    STR俈妛擭 = "1"
                    STR俉妛擭 = "1"
                    STR俋妛擭 = "1"

                    '僷儔儊僞偼嘆寧 嘇擖椡怳懼擔 嘊嵞怳懼寧 嘋嵞怳懼擔
                    Select Case NENKAN_SCHINFO(i).SaiFurikae_Check
                        Case True
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                        Case False
                            If PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, "", NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                                Exit Function
                            End If
                    End Select

                    '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                    Int_Syori_Flag(0) = 1
                Else
                    '弶怳偺僗働僕儏乕儖偑張棟拞偱傕嵞怳偺傎偆偼
                    If NENKAN_SCHINFO(i).SaiFurikae_Check = True And NENKAN_SCHINFO(i).SaiFurikae_Enabled = True Then

                        sTuki = Format(i, "00")
                        STR侾妛擭 = "1"
                        STR俀妛擭 = "1"
                        STR俁妛擭 = "1"
                        STR係妛擭 = "1"
                        STR俆妛擭 = "1"
                        STR俇妛擭 = "1"
                        STR俈妛擭 = "1"
                        STR俉妛擭 = "1"
                        STR俋妛擭 = "1"
                        If PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(sTuki, sTuki, NENKAN_SCHINFO(i).Furikae_Date, sTuki, NENKAN_SCHINFO(i).SaiFurikae_Date) = False Then
                            Exit Function
                        End If
                        '捛婰 2006/12/04
                        '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                        Int_Syori_Flag(0) = 1

                    End If
                End If
            End If
        Next i

        PFUNC_NENKAN_SAKUSEI = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal i As Integer) As Boolean
        '僗働僕儏乕儖丂捠忢儗僐乕僪(弶怳)嶌惉

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB = False
        Dim updade As Boolean

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '嵞怳擔偺梻寧敾掕偲嵞怳懼擭丄嵞怳懼寧愝掕
        '嵞怳懼擔偑擖椡偝傟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶乮s嵞怳懼寧偵寧偑愝掕乯偺応崌
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            'If Mid(STR怳懼擔, 7, 2) <= s嵞怳懼擔 Then
            If STR怳懼擔 <= STR惪媮擭寧 & s嵞怳懼擔 Then
                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = s嵞怳懼擔
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)

                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = s嵞怳懼擔
            End If
        End If

        '嵞怳懼擔偑擖椡側偟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 = "" Then

            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            'If Mid(STR怳懼擔, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR怳懼擔 <= STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE Then
                'STRW嵞怳懼寧 = s嵞怳懼寧
                'STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
                If Mid(STR怳懼擔, 5, 2) > Mid(STR惪媮擭寧, 5, 2) Then
                    If s寧 = "12" Then
                        STRW嵞怳懼寧 = "01"
                        STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                    Else
                        STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                    End If
                    STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW嵞怳懼寧 = s嵞怳懼寧
                    STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s嵞怳懼寧 = "" Then
            STR嵞怳懼擔 = "00000000"
        Else
            '嵞怳懼擔嶼弌
            STR嵞怳懼擔 = PFUNC_SAIFURIHI_MAKE(Trim(STRW嵞怳懼寧), Trim(STRW嵞怳懼擔))
        End If

        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '怳懼擔偺桳岠斖埻僠僃僢僋
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "怳懼宊栺婜娫乮" & GAKKOU_INFO.KAISI_DATE & "乣" & GAKKOU_INFO.SYURYOU_DATE & "乯奜偺寧偱偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '摿暿儗僐乕僪偺懳徾妛擭偺愝掕偟捈偟
        '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘乮0:弶怳乯
        If PFUNC_SCH_TOKUBETU_GET(STR惪媮擭寧, "0") = False Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(弶怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '婛懚儗僐乕僪桳柍僠僃僢僋
        If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
            updade = True
        End If

        '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
        STR僗働嬫暘 = "0"

        '怳懼嬫暘偺嫟捠曄悢愝掕
        STR怳懼嬫暘 = "0"

        '擖椡怳懼擔偺嫟捠曄悢愝掕
        If s怳懼擔 = "" Then
            STR擭娫擖椡怳懼擔 = Space(15)
        Else
            STR擭娫擖椡怳懼擔 = s怳懼擔
        End If

        Dim strSQL As String = ""
        If updade = False Then
            '僗働僕儏乕儖儅僗僞搊榐SQL暥(弶怳)嶌惉
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        Else
            '僗働僕儏乕儖儅僗僞峏怴SQL暥(弶怳)嶌惉
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""))
        End If

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            MessageBox.Show("搊榐偵幐攕偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26丂婇嬈帺怳偺弶怳偺僗働僕儏乕儖傕嶌惉
        '-----------------------------------------------
        '婇嬈帺怳楢実帪偺傒

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

        If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
        Else     '僗働僕儏乕儖偑懚嵼偟側偄
            '僐儊儞僩 2006/12/11
            'If intPUSH_BTN = 2 Then '峏怴帪
            '    MessageBox.Show("婇嬈帺怳懁偺僗働僕儏乕儖(" & STR惪媮擭寧.Substring(0, 4) & "擭" & STR惪媮擭寧.Substring(4, 2) & "寧暘)偑懚嵼偟傑偣傫" & vbCrLf & "婇嬈帺怳懁偱寧娫僗働僕儏乕儖嶌惉屻丄" & vbCrLf & "妛峑僗働僕儏乕儖偺峏怴張棟傪嵞搙峴偭偰偔偩偝偄", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            '僗働僕儏乕儖嶌惉
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨")
                    MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()


        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '2006/11/30丂update僼儔僌偺弶婜壔
            updade = False

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------START
            'STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            If s嵞怳懼擔 = "" Then
                STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            Else
                STR宊栺怳懼擔 = STR怳懼擔
            End If
            '2011/06/15 昗弨斉廋惓 宊栺怳懼擔偲宊栺嵞怳擔偑媡揮偡傞応崌偼梻寧偺嵞怳擔偵偡傞 -------------END

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------START

                    'If s嵞怳懼擔 = "" Then
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(strSFURI_DT))
                    'Else
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    'End If
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(STR怳懼擔)
                    '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------END
            End Select


            '摿暿儗僐乕僪偺懳徾妛擭偺愝掕偟捈偟
            '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘乮1:嵞怳乯
            If PFUNC_SCH_TOKUBETU_GET(STR惪媮擭寧, "1") = False Then
                MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str捠忢嵞乆怳擔(i)) = True Then
                updade = True
            End If

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "0"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            If s嵞怳懼擔 = "" Then
                STR擭娫擖椡怳懼擔 = Space(15)
            Else
                STR擭娫擖椡怳懼擔 = s嵞怳懼擔
            End If

            '2006/11/30丂怴婯搊榐偐峏怴偐敾掕
            strSQL = ""
            If updade = False Then
                '僗働僕儏乕儖儅僗僞搊榐SQL暥(嵞怳)嶌惉
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                '僗働僕儏乕儖儅僗僞峏怴SQL暥(嵞怳)嶌惉
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str捠忢嵞乆怳擔(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("搊榐偵幐攕偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            STR擭娫擖椡怳懼擔 = Space(15)
            updade = False

            '-----------------------------------------------
            '2006/07/26丂婇嬈帺怳偺嵞怳偺僗働僕儏乕儖傕嶌惉
            '-----------------------------------------------
            '婇嬈帺怳楢実帪偺傒
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)

            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            '撉崬偺傒
            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨")
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()

        End If

        PFUNC_NENKAN_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal i As Integer) As Boolean
        '僗働僕儏乕儖丂捠忢儗僐乕僪嶌惉

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB2 = False

        Dim updade As Boolean

        '弶怳儗僐乕僪偺嶌惉

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '嵞怳擔偺梻寧敾掕偲嵞怳懼擭丄嵞怳懼寧愝掕
        '嵞怳懼擔偑擖椡偝傟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶乮s嵞怳懼寧偵寧偑愝掕乯偺応崌
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------START
            If STR怳懼擔 <= STR惪媮擭寧 & s嵞怳懼擔 Then
                'If Mid(STR怳懼擔, 7, 2) <= s嵞怳懼擔 Then
                '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------END

                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = s嵞怳懼擔
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)

                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = s嵞怳懼擔
            End If
        End If

        '嵞怳懼擔偑擖椡側偟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 = "" Then

            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)
            '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------START
            If STR怳懼擔 <= STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE Then
            'If Mid(STR怳懼擔, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
                '2011/06/15 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 -------------END
                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s嵞怳懼寧 = "" Then
            STR嵞怳懼擔 = "00000000"
        Else
            '嵞怳懼擔嶼弌
            STR嵞怳懼擔 = PFUNC_SAIFURIHI_MAKE(Trim(STRW嵞怳懼寧), Trim(STRW嵞怳懼擔))
        End If

        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR怳懼擔
        Str_SFURI_DATE = STR嵞怳懼擔

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------START
            'STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            If s嵞怳懼擔 = "" Then
                STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            Else
                STR宊栺怳懼擔 = STR怳懼擔
            End If
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------END

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------START
                    ''s嵞怳懼擔偵妛峑儅僗僞俀偺嵞怳懼擔傪僙僢僩 2005/12/09
                    'If s嵞怳懼擔 = "" Then
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(strSFURI_DT))
                    'Else
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    'End If
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(STR怳懼擔)
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------END
            End Select


            '摿暿儗僐乕僪偺懳徾妛擭偺愝掕偟捈偟
            '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘乮1:嵞怳乯
            If PFUNC_SCH_TOKUBETU_GET(STR惪媮擭寧, "1") = False Then
                MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '婛懚儗僐乕僪桳柍僠僃僢僋
            '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------START
            If PFUNC_SCHMAST_GET("0", "1", Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str捠忢嵞乆怳擔(i)) = True Then
                'If PFUNC_SCHMAST_GET("0", "0", Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", ""), Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "")) = True Then
                '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------END

                updade = True
            End If

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "0"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            If s嵞怳懼擔 = "" Then
                STR擭娫擖椡怳懼擔 = Space(15)
            Else
                STR擭娫擖椡怳懼擔 = s嵞怳懼擔
            End If

            '2006/11/30丂怴婯搊榐偐峏怴偐敾掕
            Dim strSQL As String = ""
            If updade = False Then
                '僗働僕儏乕儖儅僗僞搊榐SQL暥(弶怳)嶌惉
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            Else
                '僗働僕儏乕儖儅僗僞峏怴SQL暥(弶怳)嶌惉
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", ""), str捠忢嵞乆怳擔(i))
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                MessageBox.Show("搊榐偵幐攕偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26丂婇嬈帺怳偺嵞怳偺僗働僕儏乕儖傕嶌惉
            '-----------------------------------------------
            '婇嬈帺怳楢実帪偺傒
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            If oraReader.DataReader(sql) = True Then '僗働僕儏乕儖偑婛偵懚嵼偡傞

            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨")
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If
                End If

            End If
            oraReader.Close()
        End If

        '-----------------------------------------------
        PFUNC_NENKAN_SAKUSEI_SUB2 = True

    End Function
    '婇嬈偺僗働僕儏乕儖峏怴梡 2006/12/08
    Private Function PFUNC_NENKAN_SAKUSEI_SUB_KIGYO(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String) As Boolean
        '僗働僕儏乕儖丂捠忢儗僐乕僪(弶怳)嶌惉

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = False
        Dim updade As Boolean

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '*** 廋惓 mitsu 2009/07/29 宊栺怳懼擔傪嶼弌偡傞 ***
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '嵞怳擔偺梻寧敾掕偲嵞怳懼擭丄嵞怳懼寧愝掕
        '嵞怳懼擔偑擖椡偝傟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶乮s嵞怳懼寧偵寧偑愝掕乯偺応崌
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------START
            'If Mid(STR怳懼擔, 7, 2) <= s嵞怳懼擔 Then
            If STR怳懼擔 <= STR惪媮擭寧 & s嵞怳懼擔 Then
                '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------END
                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = s嵞怳懼擔
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)

                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = s嵞怳懼擔
            End If
        End If

        '嵞怳懼擔偑擖椡側偟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 = "" Then

            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------START
            'If Mid(STR怳懼擔, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR怳懼擔 <= STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------END
                'STRW嵞怳懼寧 = s嵞怳懼寧
                'STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
                If Mid(STR怳懼擔, 5, 2) > Mid(STR惪媮擭寧, 5, 2) Then
                    If s寧 = "12" Then
                        STRW嵞怳懼寧 = "01"
                        STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                    Else
                        STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                    End If
                    STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE

                Else
                    STRW嵞怳懼寧 = s嵞怳懼寧
                    STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
                End If
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            End If
        End If


        If s嵞怳懼寧 = "" Then
            STR嵞怳懼擔 = "00000000"
        Else
            '嵞怳懼擔嶼弌
            STR嵞怳懼擔 = PFUNC_SAIFURIHI_MAKE(Trim(STRW嵞怳懼寧), Trim(STRW嵞怳懼擔))
        End If

        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If


        '怳懼擔偺桳岠斖埻僠僃僢僋
        If PFUNC_FURIHI_HANI_CHECK() = False Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "怳懼宊栺婜娫乮" & GAKKOU_INFO.KAISI_DATE & "乣" & GAKKOU_INFO.SYURYOU_DATE & "乯奜偺寧偱偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
        STR僗働嬫暘 = "0"

        '怳懼嬫暘偺嫟捠曄悢愝掕
        STR怳懼嬫暘 = "0"

        '擖椡怳懼擔偺嫟捠曄悢愝掕
        If s怳懼擔 = "" Then
            STR擭娫擖椡怳懼擔 = Space(15)
        Else
            STR擭娫擖椡怳懼擔 = s怳懼擔
        End If

        '-----------------------------------------------
        '2006/07/26丂婇嬈帺怳偺弶怳偺僗働僕儏乕儖傕嶌惉
        '-----------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
        sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).Furikae_Day, "/", "") & "'")
        'sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")
        '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END

        '撉崬偺傒
        If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
        Else     '僗働僕儏乕儖偑懚嵼偟側偄
            '僐儊儞僩 2006/12/11
            'If intPUSH_BTN = 2 Then '峏怴帪
            '    MessageBox.Show("婇嬈帺怳懁偺僗働僕儏乕儖(" & STR惪媮擭寧.Substring(0, 4) & "擭" & STR惪媮擭寧.Substring(4, 2) & "寧暘)偑懚嵼偟傑偣傫" & vbCrLf & "婇嬈帺怳懁偱寧娫僗働僕儏乕儖嶌惉屻丄" & vbCrLf & "妛峑僗働僕儏乕儖偺峏怴張棟傪嵞搙峴偭偰偔偩偝偄", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            'Else
            '僗働僕儏乕儖嶌惉
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
                If fn_INSERTSCHMAST(strGakkouCode, "01", Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).Furikae_Day, "/", ""), gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    'If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                    MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()
        '-----------------------------------------------

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '2006/11/30丂update僼儔僌偺弶婜壔
            updade = False

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------START
            'STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            If s嵞怳懼擔 = "" Then
                STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            Else
                STR宊栺怳懼擔 = STR怳懼擔
            End If
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------END

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------START
                    'If s嵞怳懼擔 = "" Then
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(strSFURI_DT))
                    'Else
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    'End If
                                        STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(STR怳懼擔)
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------END
            End Select

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "0"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            If s嵞怳懼擔 = "" Then
                STR擭娫擖椡怳懼擔 = Space(15)
            Else
                STR擭娫擖椡怳懼擔 = s嵞怳懼擔
            End If

            STR擭娫擖椡怳懼擔 = Space(15)

            '-----------------------------------------------
            '2006/07/26丂婇嬈帺怳偺嵞怳偺僗働僕儏乕儖傕嶌惉
            '-----------------------------------------------
            sql = New StringBuilder(128)
            oraReader = New MyOracleReader(MainDB)

            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")
            '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END

            '撉崬偺傒
            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僐儊儞僩 2006/12/11
                'If intPUSH_BTN = 2 Then '峏怴帪
                '    MessageBox.Show("婇嬈帺怳懁偺僗働僕儏乕儖(" & STR惪媮擭寧.Substring(0, 4) & "擭" & STR惪媮擭寧.Substring(4, 2) & "寧暘)偑懚嵼偟傑偣傫" & vbCrLf & "婇嬈帺怳懁偱寧娫僗働僕儏乕儖嶌惉屻丄" & vbCrLf & "妛峑僗働僕儏乕儖偺峏怴張棟傪嵞搙峴偭偰偔偩偝偄", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB_KIGYO = True

    End Function
    Private Function PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String) As Boolean
        '僗働僕儏乕儖丂捠忢儗僐乕僪嶌惉

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = False

        '弶怳儗僐乕僪偺嶌惉

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "0", "0")

        '嵞怳擔偺梻寧敾掕偲嵞怳懼擭丄嵞怳懼寧愝掕
        '嵞怳懼擔偑擖椡偝傟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶乮s嵞怳懼寧偵寧偑愝掕乯偺応崌
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------START
            'If Mid(STR怳懼擔, 7, 2) <= s嵞怳懼擔 Then
            If STR怳懼擔 <= STR惪媮擭寧 & s嵞怳懼擔 Then            
                '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------END
                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = s嵞怳懼擔
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)

                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = s嵞怳懼擔
            End If
        End If

        '嵞怳懼擔偑擖椡側偟丄偐偮擭娫僗働僕儏乕儖偺僠僃僢僋儃僢僋僗偑俷俶
        If s嵞怳懼寧 <> "" And s嵞怳懼擔 = "" Then

            STRW嵞怳懼擭 = Mid(STR怳懼擔, 1, 4)

            '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------START
            'If Mid(STR怳懼擔, 7, 2) <= GAKKOU_INFO.SFURI_DATE Then
            If STR怳懼擔 <= STR惪媮擭寧 & GAKKOU_INFO.SFURI_DATE Then
                '2011/06/16 昗弨斉廋惓 擭傪峫椂偡傞 ------------------END
                STRW嵞怳懼寧 = s嵞怳懼寧
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            Else
                If s寧 = "12" Then
                    STRW嵞怳懼寧 = "01"
                    STRW嵞怳懼擭 = CStr(CInt(Mid(STR惪媮擭寧, 1, 4)) + 1)
                Else
                    STRW嵞怳懼寧 = Format((CInt(s嵞怳懼寧) + 1), "00")
                End If
                STRW嵞怳懼擔 = GAKKOU_INFO.SFURI_DATE
            End If
        End If

        If s嵞怳懼寧 = "" Then
            STR嵞怳懼擔 = "00000000"
        Else
            '嵞怳懼擔嶼弌
            STR嵞怳懼擔 = PFUNC_SAIFURIHI_MAKE(Trim(STRW嵞怳懼寧), Trim(STRW嵞怳懼擔))
        End If

        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR怳懼擔
        Str_SFURI_DATE = STR嵞怳懼擔

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------START
            'STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            If s嵞怳懼擔 = "" Then
                STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "0", "1")
            Else
                STR宊栺怳懼擔 = STR怳懼擔
            End If
            '2011/06/16 昗弨斉廋惓 嵞怳擔偑擖椡偝傟偰偄傞応崌偼幚怳懼擔傪宊栺怳懼擔偲偡傞 ------------------END

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------START
                    ''s嵞怳懼擔偵妛峑儅僗僞俀偺嵞怳懼擔傪僙僢僩 2005/12/09
                    'If s嵞怳懼擔 = "" Then
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(strSFURI_DT))
                    'Else
                    '    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    'End If
                                        STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(STR怳懼擔)
                    '2011/06/16 昗弨斉廋惓 幚嵺偺怳懼擔偐傜嵞怳擔傪嶼弌偡傞 ------------------END
            End Select

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "0"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            If s嵞怳懼擔 = "" Then
                STR擭娫擖椡怳懼擔 = Space(15)
            Else
                STR擭娫擖椡怳懼擔 = s嵞怳懼擔
            End If

            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
            sql.Append("FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).SaiFurikae_Day, "/", "") & "'")
            'sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")
            '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END

            '撉崬偺傒
            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------START
                    If fn_INSERTSCHMAST(strGakkouCode, "02", Replace(SYOKI_NENKAN_SCHINFO(Int(s寧)).SaiFurikae_Day, "/", ""), gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        '2011/06/16 昗弨斉廋惓 岥怳僗働僕儏乕儖偼夋柺偵昞帵偝傟偨抣傪婎弨偵専嶕 ------------------END
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
            End If
            oraReader.Close()
        End If

        PFUNC_NENKAN_SAKUSEI_SUB2_KIGYO = True

    End Function


    Private Function PFUNC_NENKAN_KOUSIN() As Boolean

        '擭娫僗働僕儏乕儖丂峏怴張棟

        '嶍彍張棟乮DELETE乯 2006/11/30
        If PFUNC_NENKAN_DELETE() = False Then
            Return False
        End If

        '嶌惉張棟乮INSERT)
        If PFUNC_NENKAN_SAKUSEI() = False Then
            Return False
        End If

        Return True

    End Function

    '================================================
    '擭娫僗働僕儏乕儖嶍彍丂2006/11/30
    '================================================
    Private Function PFUNC_NENKAN_DELETE() As Boolean
        PFUNC_NENKAN_DELETE = False

        Dim sql As New StringBuilder(128)
        Dim orareader As New MyOracleReader(MainDB)

        Dim blnSakujo_Check As Boolean = False '2006/11/30

        '慡嶍彍張棟丄僉乕偼妛峑僐乕僪丄懳徾擭搙丄僗働僕儏乕儖嬫暘乮侽乯丄張棟僼儔僌乮侽乯
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =0")
        sql.Append(" AND")
        sql.Append(" ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30丂忦審捛壛乮曄峏偺偁偭偨僨乕僞偺傒嶍彍乯=========================
        For i As Integer = 1 To 12
            '曄峏偑偁傝丄僠僃僢僋偑奜傟偰偄傞傕偺傪嶍彍偡傞
            If bln擭娫峏怴(i) = True And NENKAN_SCHINFO(i).Furikae_Check = False And Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '忦審捛壛
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).Furikae_Day, "/", "") & "'")

                '嵞怳偺僗働僕儏乕儖傕嶍彍偡傞
                If SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                    sql.Append(" or")
                    sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")
                End If

                bln擭娫峏怴(i) = False '曄峏僼儔僌傪崀傠偡
                blnSakujo_Check = True '嶍彍僼儔僌傪棫偰傞

            ElseIf bln擭娫峏怴(i) = True And NENKAN_SCHINFO(i).SaiFurikae_Check = False And SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day <> "" Then
                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    sql.Append(" and(")
                End If

                '忦審捛壛
                sql.Append(" FURI_DATE_S = '" & Replace(SYOKI_NENKAN_SCHINFO(i).SaiFurikae_Day, "/", "") & "'")

                '嵞怳偺傒嶍彍偟偨応崌丄弶怳偺僗働僕儏乕儖傕曄峏偑昁梫側偺偱曄峏僼儔僌偼崀傠偝側偄
                blnSakujo_Check = True '嶍彍僼儔僌傪棫偰傞

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '嶍彍僨乕僞偑偁傞応崌偺傒幚峴偡傞
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '嶍彍張棟僄儔乕
                MessageBox.Show("(擭娫僗働僕儏乕儖)" & vbCrLf & "僗働僕儏乕儖偺嶍彍張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        PFUNC_NENKAN_DELETE = True

    End Function

#End Region

#Region " Private Sub(摿暿僗働僕儏乕儖)"
    Private Sub PSUB_TOKUBETU_GET(ByRef Get_Data() As TokubetuData)

        Get_Data(1).Seikyu_Tuki = txt摿暿惪媮寧侾.Text
        Get_Data(1).Furikae_Tuki = txt摿暿怳懼寧侾.Text
        Get_Data(1).Furikae_Date = txt摿暿怳懼擔侾.Text
        Get_Data(1).SaiFurikae_Tuki = txt摿暿嵞怳懼寧侾.Text
        Get_Data(1).SaiFurikae_Date = txt摿暿嵞怳懼擔侾.Text

        Select Case chk侾_慡妛擭.Checked
            Case True
                Get_Data(1).SiyouGakunenALL_Check = True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunenALL_Check = False
                Get_Data(1).SiyouGakunen1_Check = chk侾_侾妛擭.Checked
                Get_Data(1).SiyouGakunen2_Check = chk侾_俀妛擭.Checked
                Get_Data(1).SiyouGakunen3_Check = chk侾_俁妛擭.Checked
                Get_Data(1).SiyouGakunen4_Check = chk侾_係妛擭.Checked
                Get_Data(1).SiyouGakunen5_Check = chk侾_俆妛擭.Checked
                Get_Data(1).SiyouGakunen6_Check = chk侾_俇妛擭.Checked
                Get_Data(1).SiyouGakunen7_Check = chk侾_俈妛擭.Checked
                Get_Data(1).SiyouGakunen8_Check = chk侾_俉妛擭.Checked
                Get_Data(1).SiyouGakunen9_Check = chk侾_俋妛擭.Checked
        End Select


        Get_Data(2).Seikyu_Tuki = txt摿暿惪媮寧俀.Text
        Get_Data(2).Furikae_Tuki = txt摿暿怳懼寧俀.Text
        Get_Data(2).Furikae_Date = txt摿暿怳懼擔俀.Text
        Get_Data(2).SaiFurikae_Tuki = txt摿暿嵞怳懼寧俀.Text
        Get_Data(2).SaiFurikae_Date = txt摿暿嵞怳懼擔俀.Text

        Select Case chk俀_慡妛擭.Checked
            Case True
                Get_Data(2).SiyouGakunenALL_Check = True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunenALL_Check = False
                Get_Data(2).SiyouGakunen1_Check = chk俀_侾妛擭.Checked
                Get_Data(2).SiyouGakunen2_Check = chk俀_俀妛擭.Checked
                Get_Data(2).SiyouGakunen3_Check = chk俀_俁妛擭.Checked
                Get_Data(2).SiyouGakunen4_Check = chk俀_係妛擭.Checked
                Get_Data(2).SiyouGakunen5_Check = chk俀_俆妛擭.Checked
                Get_Data(2).SiyouGakunen6_Check = chk俀_俇妛擭.Checked
                Get_Data(2).SiyouGakunen7_Check = chk俀_俈妛擭.Checked
                Get_Data(2).SiyouGakunen8_Check = chk俀_俉妛擭.Checked
                Get_Data(2).SiyouGakunen9_Check = chk俀_俋妛擭.Checked
        End Select


        Get_Data(3).Seikyu_Tuki = txt摿暿惪媮寧俁.Text
        Get_Data(3).Furikae_Tuki = txt摿暿怳懼寧俁.Text
        Get_Data(3).Furikae_Date = txt摿暿怳懼擔俁.Text
        Get_Data(3).SaiFurikae_Tuki = txt摿暿嵞怳懼寧俁.Text
        Get_Data(3).SaiFurikae_Date = txt摿暿嵞怳懼擔俁.Text

        Select Case chk俁_慡妛擭.Checked
            Case True
                Get_Data(3).SiyouGakunenALL_Check = True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunenALL_Check = False
                Get_Data(3).SiyouGakunen1_Check = chk俁_侾妛擭.Checked
                Get_Data(3).SiyouGakunen2_Check = chk俁_俀妛擭.Checked
                Get_Data(3).SiyouGakunen3_Check = chk俁_俁妛擭.Checked
                Get_Data(3).SiyouGakunen4_Check = chk俁_係妛擭.Checked
                Get_Data(3).SiyouGakunen5_Check = chk俁_俆妛擭.Checked
                Get_Data(3).SiyouGakunen6_Check = chk俁_俇妛擭.Checked
                Get_Data(3).SiyouGakunen7_Check = chk俁_俈妛擭.Checked
                Get_Data(3).SiyouGakunen8_Check = chk俁_俉妛擭.Checked
                Get_Data(3).SiyouGakunen9_Check = chk俁_俋妛擭.Checked
        End Select


        Get_Data(4).Seikyu_Tuki = txt摿暿惪媮寧係.Text
        Get_Data(4).Furikae_Tuki = txt摿暿怳懼寧係.Text
        Get_Data(4).Furikae_Date = txt摿暿怳懼擔係.Text
        Get_Data(4).SaiFurikae_Tuki = txt摿暿嵞怳懼寧係.Text
        Get_Data(4).SaiFurikae_Date = txt摿暿嵞怳懼擔係.Text

        Select Case chk係_慡妛擭.Checked
            Case True
                Get_Data(4).SiyouGakunenALL_Check = True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunenALL_Check = False
                Get_Data(4).SiyouGakunen1_Check = chk係_侾妛擭.Checked
                Get_Data(4).SiyouGakunen2_Check = chk係_俀妛擭.Checked
                Get_Data(4).SiyouGakunen3_Check = chk係_俁妛擭.Checked
                Get_Data(4).SiyouGakunen4_Check = chk係_係妛擭.Checked
                Get_Data(4).SiyouGakunen5_Check = chk係_俆妛擭.Checked
                Get_Data(4).SiyouGakunen6_Check = chk係_俇妛擭.Checked
                Get_Data(4).SiyouGakunen7_Check = chk係_俈妛擭.Checked
                Get_Data(4).SiyouGakunen8_Check = chk係_俉妛擭.Checked
                Get_Data(4).SiyouGakunen9_Check = chk係_俋妛擭.Checked
        End Select


        Get_Data(5).Seikyu_Tuki = txt摿暿惪媮寧俆.Text
        Get_Data(5).Furikae_Tuki = txt摿暿怳懼寧俆.Text
        Get_Data(5).Furikae_Date = txt摿暿怳懼擔俆.Text
        Get_Data(5).SaiFurikae_Tuki = txt摿暿嵞怳懼寧俆.Text
        Get_Data(5).SaiFurikae_Date = txt摿暿嵞怳懼擔俆.Text

        Select Case chk俆_慡妛擭.Checked
            Case True
                Get_Data(5).SiyouGakunenALL_Check = True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunenALL_Check = False
                Get_Data(5).SiyouGakunen1_Check = chk俆_侾妛擭.Checked
                Get_Data(5).SiyouGakunen2_Check = chk俆_俀妛擭.Checked
                Get_Data(5).SiyouGakunen3_Check = chk俆_俁妛擭.Checked
                Get_Data(5).SiyouGakunen4_Check = chk俆_係妛擭.Checked
                Get_Data(5).SiyouGakunen5_Check = chk俆_俆妛擭.Checked
                Get_Data(5).SiyouGakunen6_Check = chk俆_俇妛擭.Checked
                Get_Data(5).SiyouGakunen7_Check = chk俆_俈妛擭.Checked
                Get_Data(5).SiyouGakunen8_Check = chk俆_俉妛擭.Checked
                Get_Data(5).SiyouGakunen9_Check = chk俆_俋妛擭.Checked
        End Select

        Get_Data(6).Seikyu_Tuki = txt摿暿惪媮寧俇.Text
        Get_Data(6).Furikae_Tuki = txt摿暿怳懼寧俇.Text
        Get_Data(6).Furikae_Date = txt摿暿怳懼擔俇.Text
        Get_Data(6).SaiFurikae_Tuki = txt摿暿嵞怳懼寧俇.Text
        Get_Data(6).SaiFurikae_Date = txt摿暿嵞怳懼擔俇.Text

        Select Case chk俇_慡妛擭.Checked
            Case True
                Get_Data(6).SiyouGakunenALL_Check = True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunenALL_Check = False
                Get_Data(6).SiyouGakunen1_Check = chk俇_侾妛擭.Checked
                Get_Data(6).SiyouGakunen2_Check = chk俇_俀妛擭.Checked
                Get_Data(6).SiyouGakunen3_Check = chk俇_俁妛擭.Checked
                Get_Data(6).SiyouGakunen4_Check = chk俇_係妛擭.Checked
                Get_Data(6).SiyouGakunen5_Check = chk俇_俆妛擭.Checked
                Get_Data(6).SiyouGakunen6_Check = chk俇_俇妛擭.Checked
                Get_Data(6).SiyouGakunen7_Check = chk俇_俈妛擭.Checked
                Get_Data(6).SiyouGakunen8_Check = chk俇_俉妛擭.Checked
                Get_Data(6).SiyouGakunen9_Check = chk俇_俋妛擭.Checked
        End Select

    End Sub

#End Region

#Region " Private Sub(摿暿僗働僕儏乕儖夋柺惂屼)"
    Private Sub PSUB_TOKUBETU_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '懳徾妛擭僠僃僢僋俛俷倃偺桳岠壔
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)
        'End Select

        '張棟懳徾妛擭巜掕僠僃僢僋OFF
        Call PSUB_TOKUBETU_CHK(False)

        '怳懼擔擖椡棑丄嵞怳懼擔擖椡棑偺僋儕傾
        Call PSUB_TOKUBETU_DAYCLER()

    End Sub
    Private Sub PSUB_TOKUBETU_CHKBOXEnabled(ByVal pValue As Boolean)

        '懳徾妛擭僠僃僢僋BOX偺桳岠壔
        chk侾_侾妛擭.Enabled = pValue
        chk侾_俀妛擭.Enabled = pValue
        chk侾_俁妛擭.Enabled = pValue
        chk侾_係妛擭.Enabled = pValue
        chk侾_俆妛擭.Enabled = pValue
        chk侾_俇妛擭.Enabled = pValue
        chk侾_俈妛擭.Enabled = pValue
        chk侾_俉妛擭.Enabled = pValue
        chk侾_俋妛擭.Enabled = pValue
        chk侾_慡妛擭.Enabled = pValue

        chk俀_侾妛擭.Enabled = pValue
        chk俀_俀妛擭.Enabled = pValue
        chk俀_俁妛擭.Enabled = pValue
        chk俀_係妛擭.Enabled = pValue
        chk俀_俆妛擭.Enabled = pValue
        chk俀_俇妛擭.Enabled = pValue
        chk俀_俈妛擭.Enabled = pValue
        chk俀_俉妛擭.Enabled = pValue
        chk俀_俋妛擭.Enabled = pValue
        chk俀_慡妛擭.Enabled = pValue

        chk俁_侾妛擭.Enabled = pValue
        chk俁_俀妛擭.Enabled = pValue
        chk俁_俁妛擭.Enabled = pValue
        chk俁_係妛擭.Enabled = pValue
        chk俁_俆妛擭.Enabled = pValue
        chk俁_俇妛擭.Enabled = pValue
        chk俁_俈妛擭.Enabled = pValue
        chk俁_俉妛擭.Enabled = pValue
        chk俁_俋妛擭.Enabled = pValue
        chk俁_慡妛擭.Enabled = pValue

        chk係_侾妛擭.Enabled = pValue
        chk係_俀妛擭.Enabled = pValue
        chk係_俁妛擭.Enabled = pValue
        chk係_係妛擭.Enabled = pValue
        chk係_俆妛擭.Enabled = pValue
        chk係_俇妛擭.Enabled = pValue
        chk係_俈妛擭.Enabled = pValue
        chk係_俉妛擭.Enabled = pValue
        chk係_俋妛擭.Enabled = pValue
        chk係_慡妛擭.Enabled = pValue

        chk俆_侾妛擭.Enabled = pValue
        chk俆_俀妛擭.Enabled = pValue
        chk俆_俁妛擭.Enabled = pValue
        chk俆_係妛擭.Enabled = pValue
        chk俆_俆妛擭.Enabled = pValue
        chk俆_俇妛擭.Enabled = pValue
        chk俆_俈妛擭.Enabled = pValue
        chk俆_俉妛擭.Enabled = pValue
        chk俆_俋妛擭.Enabled = pValue
        chk俆_慡妛擭.Enabled = pValue

        chk俇_侾妛擭.Enabled = pValue
        chk俇_俀妛擭.Enabled = pValue
        chk俇_俁妛擭.Enabled = pValue
        chk俇_係妛擭.Enabled = pValue
        chk俇_俆妛擭.Enabled = pValue
        chk俇_俇妛擭.Enabled = pValue
        chk俇_俈妛擭.Enabled = pValue
        chk俇_俉妛擭.Enabled = pValue
        chk俇_俋妛擭.Enabled = pValue
        chk俇_慡妛擭.Enabled = pValue

    End Sub
    Private Sub PSUB_TOKUBETU_DAYCLER()

        '惪媮寧偺僋儕傾張棟
        txt摿暿惪媮寧侾.Text = ""
        txt摿暿惪媮寧俀.Text = ""
        txt摿暿惪媮寧俁.Text = ""
        txt摿暿惪媮寧係.Text = ""
        txt摿暿惪媮寧俆.Text = ""
        txt摿暿惪媮寧俇.Text = ""

        '怳懼擔偺僋儕傾張棟
        txt摿暿怳懼寧侾.Text = ""
        txt摿暿怳懼擔侾.Text = ""
        txt摿暿怳懼寧俀.Text = ""
        txt摿暿怳懼擔俀.Text = ""
        txt摿暿怳懼寧俁.Text = ""
        txt摿暿怳懼擔俁.Text = ""
        txt摿暿怳懼寧係.Text = ""
        txt摿暿怳懼擔係.Text = ""
        txt摿暿怳懼寧俆.Text = ""
        txt摿暿怳懼擔俆.Text = ""
        txt摿暿怳懼寧俇.Text = ""
        txt摿暿怳懼擔俇.Text = ""

        '嵞怳懼擔偺僋儕傾張棟
        txt摿暿嵞怳懼寧侾.Text = ""
        txt摿暿嵞怳懼擔侾.Text = ""
        txt摿暿嵞怳懼寧俀.Text = ""
        txt摿暿嵞怳懼擔俀.Text = ""
        txt摿暿嵞怳懼寧俁.Text = ""
        txt摿暿嵞怳懼擔俁.Text = ""
        txt摿暿嵞怳懼寧係.Text = ""
        txt摿暿嵞怳懼擔係.Text = ""
        txt摿暿嵞怳懼寧俆.Text = ""
        txt摿暿嵞怳懼擔俆.Text = ""
        txt摿暿嵞怳懼寧俇.Text = ""
        txt摿暿嵞怳懼擔俇.Text = ""

    End Sub
    Private Sub PSUB_TOKUBETU_CHK(ByVal pValue As Boolean)

        '懳徾妛擭桳岠僠僃僢僋OFF
        chk侾_侾妛擭.Checked = pValue
        chk侾_俀妛擭.Checked = pValue
        chk侾_俁妛擭.Checked = pValue
        chk侾_係妛擭.Checked = pValue
        chk侾_俆妛擭.Checked = pValue
        chk侾_俇妛擭.Checked = pValue
        chk侾_俈妛擭.Checked = pValue
        chk侾_俉妛擭.Checked = pValue
        chk侾_俋妛擭.Checked = pValue
        chk侾_慡妛擭.Checked = pValue

        chk俀_侾妛擭.Checked = pValue
        chk俀_俀妛擭.Checked = pValue
        chk俀_俁妛擭.Checked = pValue
        chk俀_係妛擭.Checked = pValue
        chk俀_俆妛擭.Checked = pValue
        chk俀_俇妛擭.Checked = pValue
        chk俀_俈妛擭.Checked = pValue
        chk俀_俉妛擭.Checked = pValue
        chk俀_俋妛擭.Checked = pValue
        chk俀_慡妛擭.Checked = pValue

        chk俁_侾妛擭.Checked = pValue
        chk俁_俀妛擭.Checked = pValue
        chk俁_俁妛擭.Checked = pValue
        chk俁_係妛擭.Checked = pValue
        chk俁_俆妛擭.Checked = pValue
        chk俁_俇妛擭.Checked = pValue
        chk俁_俈妛擭.Checked = pValue
        chk俁_俉妛擭.Checked = pValue
        chk俁_俋妛擭.Checked = pValue
        chk俁_慡妛擭.Checked = pValue

        chk係_侾妛擭.Checked = pValue
        chk係_俀妛擭.Checked = pValue
        chk係_俁妛擭.Checked = pValue
        chk係_係妛擭.Checked = pValue
        chk係_俆妛擭.Checked = pValue
        chk係_俇妛擭.Checked = pValue
        chk係_俈妛擭.Checked = pValue
        chk係_俉妛擭.Checked = pValue
        chk係_俋妛擭.Checked = pValue
        chk係_慡妛擭.Checked = pValue

        chk俆_侾妛擭.Checked = pValue
        chk俆_俀妛擭.Checked = pValue
        chk俆_俁妛擭.Checked = pValue
        chk俆_係妛擭.Checked = pValue
        chk俆_俆妛擭.Checked = pValue
        chk俆_俇妛擭.Checked = pValue
        chk俆_俈妛擭.Checked = pValue
        chk俆_俉妛擭.Checked = pValue
        chk俆_俋妛擭.Checked = pValue
        chk俆_慡妛擭.Checked = pValue

        chk俇_侾妛擭.Checked = pValue
        chk俇_俀妛擭.Checked = pValue
        chk俇_俁妛擭.Checked = pValue
        chk俇_係妛擭.Checked = pValue
        chk俇_俆妛擭.Checked = pValue
        chk俇_俇妛擭.Checked = pValue
        chk俇_俈妛擭.Checked = pValue
        chk俇_俉妛擭.Checked = pValue
        chk俇_俋妛擭.Checked = pValue
        chk俇_慡妛擭.Checked = pValue

    End Sub

    Private Sub PSUB_TOKUBETU_SET(ByVal txtbox惪媮寧 As TextBox, ByVal txtbox寧 As TextBox, ByVal txtbox擔 As TextBox, ByVal chkbox1 As CheckBox, ByVal chkbox2 As CheckBox, ByVal chkbox3 As CheckBox, ByVal chkbox4 As CheckBox, ByVal chkbox5 As CheckBox, ByVal chkbox6 As CheckBox, ByVal chkbox7 As CheckBox, ByVal chkbox8 As CheckBox, ByVal chkbox9 As CheckBox, ByVal chkboxALL As CheckBox, ByVal aReader As MyOracleReader)

        '摿暿怳懼擔丂嶲徠儃僞儞嫟捠曇廤

        '惪媮寧偺愝掕
        txtbox惪媮寧.Text = Mid(aReader.GetString("NENGETUDO_S"), 5, 2)

        '怳懼寧偺愝掕
        txtbox寧.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)

        '怳懼擔偺愝掕
        txtbox擔.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case CInt(aReader.GetString("FURI_KBN_S"))
            Case 0
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriFurikae_Flag = True
                End Select
            Case 1
                Select Case True
                    Case aReader.GetString("ENTRI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("CHECK_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                        '2006/11/30丂僠僃僢僋僼儔僌傪庢摼
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                    Case aReader.GetString("DATA_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("FUNOU_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("SAIFURI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("KESSAI_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                    Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                        SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).SyoriSaiFurikae_Flag = True
                End Select
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
           aReader.GetString("GAKUNEN9_FLG_S") = "1" Then

            '慡妛擭僠僃僢僋儃僢僋僗俷俶
            chkboxALL.Checked = True

            '侾偐傜俋妛擭僠僃僢僋儃僋僗偺巊梡晄壜
            chkbox1.Enabled = False
            chkbox2.Enabled = False
            chkbox3.Enabled = False
            chkbox4.Enabled = False
            chkbox5.Enabled = False
            chkbox6.Enabled = False
            chkbox7.Enabled = False
            chkbox8.Enabled = False
            chkbox9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '侾妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox1.Checked = True
            Else
                chkbox1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '俀妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox2.Checked = True
            Else
                chkbox2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '俁妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox3.Checked = True
            Else
                chkbox3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '係妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox4.Checked = True
            Else
                chkbox4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '俆妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox5.Checked = True
            Else
                chkbox5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '俇妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox6.Checked = True
            Else
                chkbox6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '俈妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox7.Checked = True
            Else
                chkbox7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '俉妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox8.Checked = True
            Else
                chkbox8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '俋妛擭僠僃僢僋儃僢僋僗俷俶
                chkbox9.Checked = True
            Else
                chkbox9.Checked = False
            End If
        End If

    End Sub
    Private Sub PSUB_TGAKUNEN_CHK()
        '2006/10/12丂巊梡偟偰偄側偄妛擭偺僠僃僢僋儃僢僋僗傪巊梡晄壜偵偡傞

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk侾_俋妛擭.Enabled = False
            chk俀_俋妛擭.Enabled = False
            chk俁_俋妛擭.Enabled = False
            chk係_俋妛擭.Enabled = False
            chk俆_俋妛擭.Enabled = False
            chk俇_俋妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk侾_俉妛擭.Enabled = False
            chk俀_俉妛擭.Enabled = False
            chk俁_俉妛擭.Enabled = False
            chk係_俉妛擭.Enabled = False
            chk俆_俉妛擭.Enabled = False
            chk俇_俉妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk侾_俈妛擭.Enabled = False
            chk俀_俈妛擭.Enabled = False
            chk俁_俈妛擭.Enabled = False
            chk係_俈妛擭.Enabled = False
            chk俆_俈妛擭.Enabled = False
            chk俇_俈妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk侾_俇妛擭.Enabled = False
            chk俀_俇妛擭.Enabled = False
            chk俁_俇妛擭.Enabled = False
            chk係_俇妛擭.Enabled = False
            chk俆_俇妛擭.Enabled = False
            chk俇_俇妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk侾_俆妛擭.Enabled = False
            chk俀_俆妛擭.Enabled = False
            chk俁_俆妛擭.Enabled = False
            chk係_俆妛擭.Enabled = False
            chk俆_俆妛擭.Enabled = False
            chk俇_俆妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk侾_係妛擭.Enabled = False
            chk俀_係妛擭.Enabled = False
            chk俁_係妛擭.Enabled = False
            chk係_係妛擭.Enabled = False
            chk俆_係妛擭.Enabled = False
            chk俇_係妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk侾_俁妛擭.Enabled = False
            chk俀_俁妛擭.Enabled = False
            chk俁_俁妛擭.Enabled = False
            chk係_俁妛擭.Enabled = False
            chk俆_俁妛擭.Enabled = False
            chk俇_俁妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk侾_俀妛擭.Enabled = False
            chk俀_俀妛擭.Enabled = False
            chk俁_俀妛擭.Enabled = False
            chk係_俀妛擭.Enabled = False
            chk俆_俀妛擭.Enabled = False
            chk俇_俀妛擭.Enabled = False
        End If
    End Sub

    '=============================================
    '妛擭僼儔僌傪擇師尦攝楍偵奿擺偡傞丂2006/11/30
    '=============================================
    Private Sub PSUB_GAKUNENFLG_GET(ByRef strGakunen_FLG(,) As Boolean)

        strGakunen_FLG(1, 1) = chk侾_侾妛擭.Checked
        strGakunen_FLG(1, 2) = chk侾_俀妛擭.Checked
        strGakunen_FLG(1, 3) = chk侾_俁妛擭.Checked
        strGakunen_FLG(1, 4) = chk侾_係妛擭.Checked
        strGakunen_FLG(1, 5) = chk侾_俆妛擭.Checked
        strGakunen_FLG(1, 6) = chk侾_俇妛擭.Checked
        strGakunen_FLG(1, 7) = chk侾_俈妛擭.Checked
        strGakunen_FLG(1, 8) = chk侾_俉妛擭.Checked
        strGakunen_FLG(1, 9) = chk侾_俋妛擭.Checked
        strGakunen_FLG(1, 10) = chk侾_慡妛擭.Checked

        strGakunen_FLG(2, 1) = chk俀_侾妛擭.Checked
        strGakunen_FLG(2, 2) = chk俀_俀妛擭.Checked
        strGakunen_FLG(2, 3) = chk俀_俁妛擭.Checked
        strGakunen_FLG(2, 4) = chk俀_係妛擭.Checked
        strGakunen_FLG(2, 5) = chk俀_俆妛擭.Checked
        strGakunen_FLG(2, 6) = chk俀_俇妛擭.Checked
        strGakunen_FLG(2, 7) = chk俀_俈妛擭.Checked
        strGakunen_FLG(2, 8) = chk俀_俉妛擭.Checked
        strGakunen_FLG(2, 9) = chk俀_俋妛擭.Checked
        strGakunen_FLG(2, 10) = chk俀_慡妛擭.Checked

        strGakunen_FLG(3, 1) = chk俁_侾妛擭.Checked
        strGakunen_FLG(3, 2) = chk俁_俀妛擭.Checked
        strGakunen_FLG(3, 3) = chk俁_俁妛擭.Checked
        strGakunen_FLG(3, 4) = chk俁_係妛擭.Checked
        strGakunen_FLG(3, 5) = chk俁_俆妛擭.Checked
        strGakunen_FLG(3, 6) = chk俁_俇妛擭.Checked
        strGakunen_FLG(3, 7) = chk俁_俈妛擭.Checked
        strGakunen_FLG(3, 8) = chk俁_俉妛擭.Checked
        strGakunen_FLG(3, 9) = chk俁_俋妛擭.Checked
        strGakunen_FLG(3, 10) = chk俁_慡妛擭.Checked

        strGakunen_FLG(4, 1) = chk係_侾妛擭.Checked
        strGakunen_FLG(4, 2) = chk係_俀妛擭.Checked
        strGakunen_FLG(4, 3) = chk係_俁妛擭.Checked
        strGakunen_FLG(4, 4) = chk係_係妛擭.Checked
        strGakunen_FLG(4, 5) = chk係_俆妛擭.Checked
        strGakunen_FLG(4, 6) = chk係_俇妛擭.Checked
        strGakunen_FLG(4, 7) = chk係_俈妛擭.Checked
        strGakunen_FLG(4, 8) = chk係_俉妛擭.Checked
        strGakunen_FLG(4, 9) = chk係_俋妛擭.Checked
        strGakunen_FLG(4, 10) = chk係_慡妛擭.Checked

        strGakunen_FLG(5, 1) = chk俆_侾妛擭.Checked
        strGakunen_FLG(5, 2) = chk俆_俀妛擭.Checked
        strGakunen_FLG(5, 3) = chk俆_俁妛擭.Checked
        strGakunen_FLG(5, 4) = chk俆_係妛擭.Checked
        strGakunen_FLG(5, 5) = chk俆_俆妛擭.Checked
        strGakunen_FLG(5, 6) = chk俆_俇妛擭.Checked
        strGakunen_FLG(5, 7) = chk俆_俈妛擭.Checked
        strGakunen_FLG(5, 8) = chk俆_俉妛擭.Checked
        strGakunen_FLG(5, 9) = chk俆_俋妛擭.Checked
        strGakunen_FLG(5, 10) = chk俆_慡妛擭.Checked

        strGakunen_FLG(6, 1) = chk俇_侾妛擭.Checked
        strGakunen_FLG(6, 2) = chk俇_俀妛擭.Checked
        strGakunen_FLG(6, 3) = chk俇_俁妛擭.Checked
        strGakunen_FLG(6, 4) = chk俇_係妛擭.Checked
        strGakunen_FLG(6, 5) = chk俇_俆妛擭.Checked
        strGakunen_FLG(6, 6) = chk俇_俇妛擭.Checked
        strGakunen_FLG(6, 7) = chk俇_俈妛擭.Checked
        strGakunen_FLG(6, 8) = chk俇_俉妛擭.Checked
        strGakunen_FLG(6, 9) = chk俇_俋妛擭.Checked
        strGakunen_FLG(6, 10) = chk俇_慡妛擭.Checked

    End Sub

#End Region

#Region " Private Function(摿暿僗働僕儏乕儖)"
    Private Function PFUNC_SCH_GET_TOKUBETU() As Boolean

        PFUNC_SCH_GET_TOKUBETU = False

        '摿暿怳懼擔
        '懳徾妛擭僠僃僢僋俛俷倃偺桳岠壔
        Call PSUB_TOKUBETU_CHKBOXEnabled(True)

        '張棟懳徾妛擭巜掕僠僃僢僋OFF
        Call PSUB_TOKUBETU_CHK(False)

        '怳懼擔擖椡棑丄嵞怳懼擔擖椡棑偺僋儕傾
        Call PSUB_TOKUBETU_DAYCLER()

        '摿暿怳懼擔嶲徠張棟
        If PFUNC_TOKUBETU_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_TOKUBETU = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_TOKUBETU() As Boolean

        '摿暿僗働僕儏乕儖峏怴張棟
        If PFUNC_TOKUBETU_KOUSIN() = False Then

            '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
            Int_Syori_Flag(1) = 2

            Return False
        End If

        Return True

    End Function
    Private Function PFUNC_SCH_TOKUBETU_GET(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean


        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        Try

            PFUNC_SCH_TOKUBETU_GET = False

            '摿暿僗働僕儏乕儖偺儗僐乕僪懚嵼僠僃僢僋

            sql.Append(" SELECT * FROM G_SCHMAST")
            sql.Append(" WHERE")
            sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
            sql.Append(" AND")
            sql.Append(" NENGETUDO_S = '" & strNENGETUDO & "'")
            sql.Append(" AND")
            sql.Append(" SCH_KBN_S = '1'")
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S = " & "'" & strFURIKUBUN & "'")

            If oraReader.DataReader(sql) = True Then '懚嵼偡傟偽

                '摿暿儗僐乕僪偺懳徾妛擭傪尦偵丄捠忢儗僐乕僪偺懳徾妛擭傪愝掕偟捈偡
                '仸摿暿僗働僕儏乕儖偱巜掕偝傟偰偄傞妛擭偼擭娫僗働僕儏乕儖偱偼巜掕偟側偄
                Do Until oraReader.EOF
                    If oraReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                        STR侾妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                        STR俀妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                        STR俁妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                        STR係妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                        STR俆妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                        STR俇妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                        STR俈妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                        STR俉妛擭 = "0"
                    End If
                    If oraReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                        STR俋妛擭 = "0"
                    End If
                    oraReader.NextRead()
                Loop

            Else    '懚嵼偟側偗傟偽True
                PFUNC_SCH_TOKUBETU_GET = True
                Return True
            End If

            PFUNC_SCH_TOKUBETU_GET = True

        Catch ex As Exception
            Throw
        Finally
            If Not oraReader Is Nothing Then oraReader.Close()
        End Try

    End Function

    Private Function PFUNC_TOKUBETU_SANSYOU() As Boolean

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '摿暿怳懼擔丂嶲徠張棟
        PFUNC_TOKUBETU_SANSYOU = False

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 1")
        sql.Append(" ORDER BY FURI_KBN_S asc , FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = False Then
            oraReader.Close()
            Exit Function
        End If

        Do Until oraReader.EOF

            Select Case oraReader.GetString("FURI_KBN_S")
                Case "0"
                    '傑偩抣偑愝掕偝傟偰偄側偄峴偵摿暿僗働僕儏乕儖傪愝掕偡傞
                    Select Case True
                        Case (txt摿暿怳懼寧侾.Text = "")
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧侾, txt摿暿怳懼寧侾, txt摿暿怳懼擔侾, chk侾_侾妛擭, chk侾_俀妛擭, chk侾_俁妛擭, chk侾_係妛擭, chk侾_俆妛擭, chk侾_俇妛擭, chk侾_俈妛擭, chk侾_俉妛擭, chk侾_俋妛擭, chk侾_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧侾.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            '2006/11/30丂僠僃僢僋僼儔僌傪庢摼
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            '2006/11/30丂晄擻僼儔僌傪庢摼
                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt摿暿怳懼寧俀.Text = "")
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俀, txt摿暿怳懼寧俀, txt摿暿怳懼擔俀, chk俀_侾妛擭, chk俀_俀妛擭, chk俀_俁妛擭, chk俀_係妛擭, chk俀_俆妛擭, chk俀_俇妛擭, chk俀_俈妛擭, chk俀_俉妛擭, chk俀_俋妛擭, chk俀_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧俀.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt摿暿怳懼寧俁.Text = "")
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俁, txt摿暿怳懼寧俁, txt摿暿怳懼擔俁, chk俁_侾妛擭, chk俁_俀妛擭, chk俁_俁妛擭, chk俁_係妛擭, chk俁_俆妛擭, chk俁_俇妛擭, chk俁_俈妛擭, chk俁_俉妛擭, chk俁_俋妛擭, chk俁_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧俁.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt摿暿怳懼寧係.Text = "")
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧係, txt摿暿怳懼寧係, txt摿暿怳懼擔係, chk係_侾妛擭, chk係_俀妛擭, chk係_俁妛擭, chk係_係妛擭, chk係_俆妛擭, chk係_俇妛擭, chk係_俈妛擭, chk係_俉妛擭, chk係_俋妛擭, chk係_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧係.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt摿暿怳懼寧俆.Text = "")
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俆, txt摿暿怳懼寧俆, txt摿暿怳懼擔俆, chk俆_侾妛擭, chk俆_俀妛擭, chk俆_俁妛擭, chk俆_係妛擭, chk俆_俆妛擭, chk俆_俇妛擭, chk俆_俈妛擭, chk俆_俉妛擭, chk俆_俋妛擭, chk俆_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧俆.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                        Case (txt摿暿怳懼寧俇.Text = "")
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俇, txt摿暿怳懼寧俇, txt摿暿怳懼擔俇, chk俇_侾妛擭, chk俇_俀妛擭, chk俇_俁妛擭, chk俇_係妛擭, chk俇_俆妛擭, chk俇_俇妛擭, chk俇_俈妛擭, chk俇_俉妛擭, chk俇_俋妛擭, chk俇_慡妛擭, oraReader)

                            '怳懼擔偲嵞怳懼擔偺昞帵忋偺懳墳娭學乮僙僢僩乯傪偲傞偨傔丄僞僌偵怳懼擔儗僐乕僪拞偺嵞怳懼擔傪堦帪曐懚偡傞
                            If oraReader.GetString("SFURI_DATE_S") <> "00000000" Then
                                txt摿暿怳懼寧俇.Tag = oraReader.GetString("SFURI_DATE_S")
                            End If

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckFurikae_Flag = False
                            End If

                            If oraReader.GetString("FUNOU_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).FunouFurikae_Flag = False
                            End If

                    End Select

                Case "1"
                    Select Case oraReader.GetString("FURI_DATE_S")
                        Case txt摿暿怳懼寧侾.Tag
                            Int_Tokubetu_Flag = 1
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧侾, txt摿暿嵞怳懼寧侾, txt摿暿嵞怳懼擔侾, chk侾_侾妛擭, chk侾_俀妛擭, chk侾_俁妛擭, chk侾_係妛擭, chk侾_俆妛擭, chk侾_俇妛擭, chk侾_俈妛擭, chk侾_俉妛擭, chk侾_俋妛擭, chk侾_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(1) = oraReader.GetString("SFURI_DATE_S")

                            '2006/11/30丂僠僃僢僋僼儔僌傪庢摼
                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt摿暿怳懼寧俀.Tag
                            Int_Tokubetu_Flag = 2
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俀, txt摿暿嵞怳懼寧俀, txt摿暿嵞怳懼擔俀, chk俀_侾妛擭, chk俀_俀妛擭, chk俀_俁妛擭, chk俀_係妛擭, chk俀_俆妛擭, chk俀_俇妛擭, chk俀_俈妛擭, chk俀_俉妛擭, chk俀_俋妛擭, chk俀_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(2) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt摿暿怳懼寧俁.Tag
                            Int_Tokubetu_Flag = 3
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俁, txt摿暿嵞怳懼寧俁, txt摿暿嵞怳懼擔俁, chk俁_侾妛擭, chk俁_俀妛擭, chk俁_俁妛擭, chk俁_係妛擭, chk俁_俆妛擭, chk俁_俇妛擭, chk俁_俈妛擭, chk俁_俉妛擭, chk俁_俋妛擭, chk俁_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(3) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt摿暿怳懼寧係.Tag
                            Int_Tokubetu_Flag = 4
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧係, txt摿暿嵞怳懼寧係, txt摿暿嵞怳懼擔係, chk係_侾妛擭, chk係_俀妛擭, chk係_俁妛擭, chk係_係妛擭, chk係_俆妛擭, chk係_俇妛擭, chk係_俈妛擭, chk係_俉妛擭, chk係_俋妛擭, chk係_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(4) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt摿暿怳懼寧俆.Tag
                            Int_Tokubetu_Flag = 5
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俆, txt摿暿嵞怳懼寧俆, txt摿暿嵞怳懼擔俆, chk俆_侾妛擭, chk俆_俀妛擭, chk俆_俁妛擭, chk俆_係妛擭, chk俆_俆妛擭, chk俆_俇妛擭, chk俆_俈妛擭, chk俆_俉妛擭, chk俆_俋妛擭, chk俆_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(5) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                        Case txt摿暿怳懼寧俇.Tag
                            Int_Tokubetu_Flag = 6
                            Call PSUB_TOKUBETU_SET(txt摿暿惪媮寧俇, txt摿暿嵞怳懼寧俇, txt摿暿嵞怳懼擔俇, chk俇_侾妛擭, chk俇_俀妛擭, chk俇_俁妛擭, chk俇_係妛擭, chk俇_俆妛擭, chk俇_俇妛擭, chk俇_俈妛擭, chk俇_俉妛擭, chk俇_俋妛擭, chk俇_慡妛擭, oraReader)

                            '2006/11/30丂嵞乆怳懼擔傪庢摼
                            str摿暿嵞乆怳擔(6) = oraReader.GetString("SFURI_DATE_S")

                            If oraReader.GetString("CHECK_FLG_S") = "1" Then
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = True
                            Else
                                SYOKI_TOKUBETU_SCHINFO(Int_Tokubetu_Flag).CheckSaiFurikae_Flag = False
                            End If

                    End Select
            End Select

            oraReader.NextRead()

        Loop

        oraReader.Close()

        'Tag偺徚嫀
        txt摿暿怳懼寧侾.Tag = ""
        txt摿暿怳懼寧俀.Tag = ""
        txt摿暿怳懼寧俁.Tag = ""
        txt摿暿怳懼寧係.Tag = ""
        txt摿暿怳懼寧俆.Tag = ""
        txt摿暿怳懼寧俇.Tag = ""

        PFUNC_TOKUBETU_SANSYOU = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI(ByVal str張棟 As String) As Boolean
        '摿暿怳懼擔丂嶌惉張棟丂丂丂
        PFUNC_TOKUBETU_SAKUSEI = False

        '擖椡僠僃僢僋
        Select Case True
            Case (Trim(txt摿暿嵞怳懼寧侾.Text) <> "" And Trim(txt摿暿嵞怳懼擔侾.Text) <> "" And Trim(txt摿暿惪媮寧侾.Text) = "" And Trim(txt摿暿怳懼寧侾.Text) = "" And Trim(txt摿暿怳懼擔侾.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt摿暿嵞怳懼寧俀.Text) <> "" And Trim(txt摿暿嵞怳懼擔俀.Text) <> "" And Trim(txt摿暿惪媮寧俀.Text) = "" And Trim(txt摿暿怳懼寧俀.Text) = "" And Trim(txt摿暿怳懼擔俀.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt摿暿嵞怳懼寧俁.Text) <> "" And Trim(txt摿暿嵞怳懼擔俁.Text) <> "" And Trim(txt摿暿惪媮寧俁.Text) = "" And Trim(txt摿暿怳懼寧俁.Text) = "" And Trim(txt摿暿怳懼擔俁.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt摿暿嵞怳懼寧係.Text) <> "" And Trim(txt摿暿嵞怳懼擔係.Text) <> "" And Trim(txt摿暿惪媮寧係.Text) = "" And Trim(txt摿暿怳懼寧係.Text) = "" And Trim(txt摿暿怳懼擔係.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt摿暿嵞怳懼寧俆.Text) <> "" And Trim(txt摿暿嵞怳懼擔俆.Text) <> "" And Trim(txt摿暿惪媮寧俆.Text) = "" And Trim(txt摿暿怳懼寧俆.Text) = "" And Trim(txt摿暿怳懼擔俆.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            Case (Trim(txt摿暿嵞怳懼寧俇.Text) <> "" And Trim(txt摿暿嵞怳懼擔俇.Text) <> "" And Trim(txt摿暿惪媮寧俇.Text) = "" And Trim(txt摿暿怳懼寧俇.Text) = "" And Trim(txt摿暿怳懼擔俇.Text) = "")
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "怳懼擔傑偨偼嵞怳懼擔偺擖椡偵岆傝偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
        End Select

        '2006/11/30丂儖乕僾壔
        For i As Integer = 1 To 6

            '2006/11/30丂曄峏偑偁偭偨応崌偺傒幚峴偡傞
            If bln摿暿峏怴(i) = True Then

                '2006/12/12丂媽怳懼擔庢摼
                If SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki = "" Then
                    '嬻敀偺応崌偼擖椡偺昁梫側偟
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki) < 4 Then
                    '侾乣俁寧
                    str媽怳懼擔(i) = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                Else
                    '係乣侾俀寧
                    str媽怳懼擔(i) = txt懳徾擭搙.Text & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date
                End If

                '2006/12/12丂媽嵞怳擔庢摼
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '嵞怳擔側偟
                    str媽嵞怳擔(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '侾乣俁寧
                    str媽嵞怳擔(i) = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '係乣侾俀寧
                    str媽嵞怳擔(i) = txt懳徾擭搙.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '怳懼擔僠僃僢僋 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "嵞怳偼峴傢側偄愝掕偵側偭偰偄傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                'CHKBOX僠僃僢僋&嫟捠曄悢偵愝掕
                                If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                                    Exit Function
                                End If

                                '嵞怳偺僗働僕儏乕儖偺傒嶌惉
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                                    Exit Function
                                End If

                                If PFUNC_SCHMAST_UPDATE_SFURIDATE(CStr(i)) = False Then
                                    Exit Function
                                End If

                                '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "嶍彍偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        '峔憿懱傪巊梡偡傞偨傔丄嫟捠曄悢偼晄梫
                        If PFUNC_GAKUNENFLG_CHECK(TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                            Exit Function
                        End If

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "嵞怳偼峴傢側偄愝掕偵側偭偰偄傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        '僷儔儊僞偼嘆寧丄嘇擖椡怳懼擔丄嘊嵞怳懼寧丂嘋嵞怳懼擔丂嘍怳懼嬫暘乮0:弶怳)丄嘐僗働僕儏乕儖嬫暘乮1:摿暿)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            Else '峏怴偑側偄応崌偱傕婇嬈帺怳懁偺僗働僕儏乕儖傪尒傞
                '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------START
                '2006/12/12丂媽嵞怳擔庢摼
                If Trim(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date) = "" Then
                    '嵞怳擔側偟
                    str媽嵞怳擔(i) = "00000000"
                ElseIf CInt(SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki) < 4 Then
                    '侾乣俁寧
                    str媽嵞怳擔(i) = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                Else
                    '係乣侾俀寧
                    str媽嵞怳擔(i) = txt懳徾擭搙.Text & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If
                '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------END

                '婇嬈帺怳楢実帪偺傒
                '怳懼擔僠僃僢僋 
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then
                        If PFUNC_TOKUBETU_CHECK(i, TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check, TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, TOKUBETU_SCHINFO(i).SiyouGakunen9_Check) = False Then
                            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                            Exit Function
                        End If

                        If SYOKI_TOKUBETU_SCHINFO(i).SyoriSaiFurikae_Flag = True Then
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If

                            If TOKUBETU_SCHINFO(i).SaiFurikae_Date <> SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "曄峏偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)

                                Exit Function
                            End If
                        Else
                            If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                                If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "嵞怳偼峴傢側偄愝掕偵側偭偰偄傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                    Exit Function
                                End If

                                '嵞怳偺僗働僕儏乕儖偺傒嶌惉
                                If PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date) = False Then
                                    Exit Function
                                End If

                                '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                                Int_Syori_Flag(1) = 1
                            End If
                        End If
                    Else
                        MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "偙偺僗働僕儏乕儖偼張棟拞偺僗働僕儏乕儖偱偡丅" & vbCrLf & "嶍彍偱偒傑偣傫丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    End If
                Else
                    If TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                        If TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then
                            If GAKKOU_INFO.SFURI_SYUBETU = "0" Or GAKKOU_INFO.SFURI_SYUBETU = "3" Then
                                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "嵞怳偼峴傢側偄愝掕偵側偭偰偄傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                Exit Function
                            End If
                        End If

                        '僷儔儊僞偼嘆寧丄嘇擖椡怳懼擔丄嘊嵞怳懼寧丂嘋嵞怳懼擔丂嘍怳懼嬫暘乮0:弶怳)丄嘐僗働僕儏乕儖嬫暘乮1:摿暿)
                        If PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(TOKUBETU_SCHINFO(i).Seikyu_Tuki, TOKUBETU_SCHINFO(i).Furikae_Tuki, TOKUBETU_SCHINFO(i).Furikae_Date, TOKUBETU_SCHINFO(i).SaiFurikae_Tuki, TOKUBETU_SCHINFO(i).SaiFurikae_Date, i) = False Then
                            Exit Function
                        End If

                        '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                        Int_Syori_Flag(1) = 1
                    End If
                End If

            End If
        Next

        If PFUNC_TOKUBETU_GAKNENFLG_CHECK() = False Then
            Exit Function
        End If

        PFUNC_TOKUBETU_SAKUSEI = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        '僗働僕儏乕儖丂摿暿儗僐乕僪嶌惉
        PFUNC_TOKUBETU_SAKUSEI_SUB = False

        '弶怳儗僐乕僪偺嶌惉

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            '嵞怳擔偺擭偺妋掕張棟
            If s嵞怳懼寧 = "01" Or s嵞怳懼寧 = "02" Or s嵞怳懼寧 = "03" Then
                STRW嵞怳懼擭 = CStr(CInt(txt懳徾擭搙.Text) + 1)
            Else
                STRW嵞怳懼擭 = txt懳徾擭搙.Text
            End If

            '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
            '塩嬈擔嶼弌
            Select Sai_Zengo_Kbn
                Case 0
                    '梻塩嬈擔
                    STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
                Case 1
                    '慜塩嬈擔
                    STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "-")
            End Select
            'STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")

            '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END
        Else
            STR嵞怳懼擔 = "00000000"
        End If

        '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
        STR僗働嬫暘 = "1"

        '怳懼嬫暘偺嫟捠曄悢愝掕
        STR怳懼嬫暘 = "0"

        '擖椡怳懼擔偺嫟捠曄悢愝掕
        STR擭娫擖椡怳懼擔 = Space(15)

        '捠忢儗僐乕僪偺懳徾妛擭偺僼儔僌峏怴乮弶怳儗僐乕僪乯
        '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘丅怳懼擔乮0:弶怳乯
        If PFUNC_SCH_NENKAN_GET(STR惪媮擭寧, "0", STR怳懼擔) = False Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "擭娫僗働僕儏乕儖偺懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(弶怳)")
            Exit Function
        End If

        '婛懚僗働僕儏乕儖偵乽嵞搙峏怴乿梡偺張棟審悢丒嬥妟丄怳懼嵪審悢丒嬥妟丄晄擻審悢丒嬥妟偺庢摼
        If PFUNC_G_MEIMAST_COUNT_MOTO(STR惪媮擭寧, "0", STR怳懼擔) = False Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "柧嵶儅僗僞忣曬庢摼幐攕")
            Exit Function
        End If

        Dim blnUP As Boolean = False

        '婛懚儗僐乕僪乮摿暿僗働僕儏乕儖偑偡偱偵嶌惉偝傟偰偄傞偐)桳柍僠僃僢僋
        '2006/11/22丂
        'If PFUNC_SCHMAST_GET("1", "0", STR怳懼擔, STR嵞怳懼擔) = True Then
        If PFUNC_SCHMAST_GET("1", "0", str媽怳懼擔(i), str媽嵞怳擔(i)) = True Then
            '懚嵼偟偰偄傞応崌UPDATE偲偡傞 2006/10/25
            blnUP = True
        End If

        '婛懚儗僐乕僪乮擭娫乯偺張棟僼儔僌桳柍 2006/10/24
        If PFUNC_SCHMAST_GET_FLG("0", "0", STR怳懼擔) = False Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "捠忢僗働僕儏乕儖張棟忬嫷庢摼幐攕", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        Else
            If strTYUUDAN_FLG = "1" Then
                MessageBox.Show("捠忢僗働僕儏乕儖(弶怳暘)偑拞抐拞偱偡" & vbCrLf & "怳懼擔丗" & STR怳懼擔.Substring(0, 4) & "擭" & STR怳懼擔.Substring(4, 2) & "寧" & STR怳懼擔.Substring(6, 2) & "擔偺拞抐傪庢徚偟偰偔偩偝偄", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If
        End If

        If strSAIFURI_DEF <> "00000000" Then '捠忢僗働僕儏乕儖偺嵞怳擔偑愝掕偝傟偰偄傞応崌
            '婛懚儗僐乕僪乮擭娫乯偺張棟僼儔僌桳柍 2006/10/24
            If PFUNC_SCHMAST_GET_FLG_SAI("0", "1", strSAIFURI_DEF) = False Then
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "捠忢僗働僕儏乕儖(嵞怳)張棟忬嫷庢摼幐攕", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            Else
                If strTYUUDAN_FLG_SAI = "1" Then
                    MessageBox.Show("捠忢僗働僕儏乕儖(嵞怳暘)偑張棟拞偱偡" & vbCrLf & "嵞怳擔丗" & strSAIFURI_DEF.Substring(0, 4) & "擭" & strSAIFURI_DEF.Substring(4, 2) & "寧" & strSAIFURI_DEF.Substring(6, 2) & "擔偺張棟傪庢徚偟偰偔偩偝偄", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '捠忢僗働僕儏乕儖乮嵞怳乯偺張棟懳徾妛擭僼儔僌峏怴
                If PFUNC_SCH_NENKAN_GET(STR惪媮擭寧, "1", strSAIFURI_DEF) = False Then
                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "擭娫僗働僕儏乕儖偺懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If

            End If
        End If

        If PFUNC_G_MEIMAST_COUNT("0", STR怳懼擔) = False Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "柧嵶儅僗僞忣曬庢摼幐攕", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        '----------------------------------------------
        '峏怴丒搊榐張棟
        '----------------------------------------------
        Dim strSQL As String = ""
        If blnUP = True Then
            '2006/11/30丂僗働僕儏乕儖偺張棟忬嫷僠僃僢僋
            If PFUNC_TOKUBETUFLG_CHECK("峏怴", "", i) = False Then
                Exit Function
            End If
            '婛偵僗働僕儏乕儖(弶怳)偑懚嵼偟偰偄傞応崌UPDATE
            strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str媽怳懼擔(i), str媽嵞怳擔(i))
        Else
            '2006/11/30丂僗働僕儏乕儖偺張棟忬嫷僠僃僢僋
            If PFUNC_TOKUBETUFLG_CHECK("嶌惉", "", i) = False Then
                Exit Function
            End If
            '2006/11/30丂擭娫僗働僕儏乕儖峏怴
            If PFUNC_TokINSERT_NenUPDATE(STR惪媮擭寧, Replace(SYOKI_NENKAN_SCHINFO(CInt(s惪媮寧)).Furikae_Day, "/", "")) = False Then
                Exit Function
            End If
            '僗働僕儏乕儖儅僗僞搊榐(弶怳)SQL暥嶌惉
            strSQL = PSUB_INSERT_G_SCHMAST_SQL()
        End If
        blnUP = False

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '嶌惉張棟僄儔乕
            Exit Function
        End If

        '2006/11/30丂擭娫僗働僕儏乕儖偺妛擭僼儔僌偺峏怴
        If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR惪媮擭寧, STR怳懼嬫暘) = False Then
            Exit Function
        End If

        '-----------------------------------------------
        '2006/07/26丂婇嬈帺怳偺弶怳偺僗働僕儏乕儖傕嶌惉
        '-----------------------------------------------
        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

        '撉崬偺傒
        If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
        Else     '僗働僕儏乕儖偑懚嵼偟側偄
            '僗働僕儏乕儖嶌惉
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                    MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
        End If
        oraReader.Close()
        '-----------------------------------------------

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔
            str媽怳懼擔(i) = str媽嵞怳擔(i)

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "1", "1")

            '怳懼嬫暘偼嵞怳偲偡傞

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                    '2006/11/22丂婛懚儗僐乕僪僠僃僢僋梡
                    str媽嵞怳擔(i) = "00000000"
                Case "2"
                    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    '2006/11/22丂婛懚儗僐乕僪僠僃僢僋梡
                    str媽嵞怳擔(i) = PFUNC_SAISAIFURIHI_MAKE(str媽嵞怳擔(i).Substring(4, 2), str媽嵞怳擔(i).Substring(6, 2))
            End Select

            '捠忢儗僐乕僪偺懳徾妛擭偺愝掕偟捈偟乮嵞怳儗僐乕僪乯
            '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘乮1:嵞怳乯
            If PFUNC_SCH_NENKAN_GET(STR惪媮擭寧, "1", STR嵞怳懼擔) = False Then
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            blnUP = False

            '婛懚儗僐乕僪桳柍僠僃僢僋
            '2006/11/22
            'If PFUNC_SCHMAST_GET("1", "1", STR怳懼擔, STR嵞怳懼擔) = True Then
            If PFUNC_SCHMAST_GET("1", "1", str媽怳懼擔(i), str媽嵞怳擔(i)) = True Then
                '懚嵼偟偰偄傞応崌UPDATE偲偡傞 2006/10/25
                blnUP = True
            End If

            '婛懚儗僐乕僪乮擭娫乯偺張棟僼儔僌桳柍 2006/10/24
            If PFUNC_SCHMAST_GET_FLG("0", "1", STR怳懼擔) = False Then
                '捠忢怳懼擔偑柍偄応崌(仸摿暿怳懼擔偱慡妛擭妱傝怳傜傟偰偄傞帪側偳偼柍帇
            End If

            If PFUNC_G_MEIMAST_COUNT("1", STR怳懼擔) = False Then
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "柧嵶儅僗僞忣曬庢摼幐攕", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "1"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            STR擭娫擖椡怳懼擔 = Space(15)

            strSQL = ""
            If blnUP = True Then
                '婛偵僗働僕儏乕儖(弶怳)偑懚嵼偟偰偄傞応崌UPDATE
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(str媽怳懼擔(i), str媽嵞怳擔(i))
            Else
                '僗働僕儏乕儖儅僗僞搊榐(嵞怳)SQL暥嶌惉
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            '2006/11/30丂擭娫僗働僕儏乕儖偺妛擭僼儔僌偺峏怴
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR惪媮擭寧, STR怳懼嬫暘) = False Then
                Exit Function
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                '嶌惉張棟僄儔乕
                Exit Function
            End If
            '-----------------------------------------------
            '2006/07/26丂婇嬈帺怳偺嵞怳偺僗働僕儏乕儖傕嶌惉
            '-----------------------------------------------
            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            '撉崬偺傒
            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If


            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        PFUNC_TOKUBETU_SAKUSEI_SUB = True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal i As Integer) As Boolean

        Dim kousin As Boolean = False   '僀儞僒乕僩儌乕僪

        PFUNC_TOKUBETU_SAKUSEI_SUB2 = False

        '僗働僕儏乕儖丂摿暿儗僐乕僪嶌惉
        '弶怳偑張棟拞偵嵞怳偺僗働僕儏乕儖傪捛壛偡傞嵺偵巊梡

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '嵞怳擔偺擭偺妋掕張棟
        If s嵞怳懼寧 = "01" Or s嵞怳懼寧 = "02" Or s嵞怳懼寧 = "03" Then
            STRW嵞怳懼擭 = CStr(CInt(txt懳徾擭搙.Text) + 1)
        Else
            STRW嵞怳懼擭 = txt懳徾擭搙.Text
        End If

        '塩嬈擔嶼弌
        '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
        '塩嬈擔嶼弌
        Select Case Sai_Zengo_Kbn
            Case 0
                '梻塩嬈擔
                STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
            Case 1
                '慜塩嬈擔
                STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "-")
        End Select
        'STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
        '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END


        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR怳懼擔
        Str_SFURI_DATE = STR嵞怳懼擔

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "1", "1")

            '怳懼嬫暘偼嵞怳偲偡傞

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"
                    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
            End Select

            '捠忢儗僐乕僪偺懳徾妛擭偺愝掕偟捈偟乮嵞怳儗僐乕僪乯
            '妛峑僐乕僪丄惪媮擭寧丄怳懼嬫暘乮1:嵞怳乯
            If PFUNC_SCH_NENKAN_GET(STR惪媮擭寧, "1", STR嵞怳懼擔) = False Then
                MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖懳徾妛擭愝掕偱僄儔乕偑敪惗偟傑偟偨(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Function
            End If

            '婛懚儗僐乕僪桳柍僠僃僢僋
            If PFUNC_SCHMAST_GET("1", "1", STR惪媮擭寧 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000") = True Then
                'If PFUNC_SCHMAST_GET("1", "1", STR怳懼擔, STR嵞怳懼擔) = True Then
                kousin = True   '傾僢僾僨乕僩儌乕僪
                'MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "摿暿僗働僕儏乕儖嶌惉嵪偱偡(嵞怳)", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Exit Function
            End If

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            STR擭娫擖椡怳懼擔 = Space(15)

            '僗働僕儏乕儖儅僗僞峏怴(弶怳)SQL暥嶌惉丂2006/11/30
            Dim strSQL As String = ""
            '壗偐傢偐傜側偄偺偱僐儊儞僩 2010.03.29 start
            'STR怳懼嬫暘 = "0" '弶怳偺斀塮偺偨傔丄堦帪揑偵0偵愝掕
            'strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR惪媮擭寧 & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, STR惪媮擭寧 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date)

            'If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            '    Return False
            'End If
            '壗偐傢偐傜側偄偺偱僐儊儞僩 2010.03.29 end

            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"

            '僗働僕儏乕儖儅僗僞搊榐(嵞怳)SQL暥嶌惉
            strSQL = ""
            If kousin = True Then
                '傾僢僾僨乕僩
                strSQL = PSUB_UPDATE_G_SCHMAST_SQL(STR惪媮擭寧 & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date, "00000000")
            Else
                '僀儞僒乕僩
                strSQL = PSUB_INSERT_G_SCHMAST_SQL()
            End If

            If MainDB.ExecuteNonQuery(strSQL) < 0 Then
                Return False
            End If

            '2006/11/30丂擭娫僗働僕儏乕儖偺妛擭僼儔僌偺峏怴
            If PFUNC_NENKAN_GAKUNENFLG_UPDATE(STR惪媮擭寧, STR怳懼嬫暘) = False Then
                Return False
            End If

            '-----------------------------------------------
            '2006/07/26丂婇嬈帺怳偺嵞怳偺僗働僕儏乕儖傕嶌惉
            '-----------------------------------------------
            Dim oraReader As New MyOracleReader(MainDB)
            Dim sql As New StringBuilder(128)

            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            '撉崬偺傒
            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僐儊儞僩 2006/12/11
                'If intPUSH_BTN = 2 Then '峏怴帪
                '    MessageBox.Show("婇嬈帺怳懁偺僗働僕儏乕儖(" & STR惪媮擭寧.Substring(0, 4) & "擭" & STR惪媮擭寧.Substring(4, 2) & "寧暘)偑懚嵼偟傑偣傫" & vbCrLf & "婇嬈帺怳懁偱寧娫僗働僕儏乕儖嶌惉屻丄" & vbCrLf & "妛峑僗働僕儏乕儖偺峏怴張棟傪嵞搙峴偭偰偔偩偝偄", gstrSYORI_R, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                'Else
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function
    '婇嬈偺僗働僕儏乕儖峏怴梡 2006/12/08
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal i As Integer) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        '僗働僕儏乕儖丂摿暿儗僐乕僪嶌惉
        PFUNC_TOKUBETU_SAKUSEI_SUB_KIGYO = False

        '弶怳儗僐乕僪偺嶌惉

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        If s嵞怳懼寧 <> "" And s嵞怳懼擔 <> "" Then
            '嵞怳擔偺擭偺妋掕張棟
            If s嵞怳懼寧 = "01" Or s嵞怳懼寧 = "02" Or s嵞怳懼寧 = "03" Then
                STRW嵞怳懼擭 = CStr(CInt(txt懳徾擭搙.Text) + 1)
            Else
                STRW嵞怳懼擭 = txt懳徾擭搙.Text
            End If
            '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
            '塩嬈擔嶼弌
            Select Case Sai_Zengo_Kbn
                Case 0
                    '梻塩嬈擔
                    STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
                Case 1
                    '慜塩嬈擔
                    STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "-")
            End Select
            'STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
            '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END
        Else
            STR嵞怳懼擔 = "00000000"
        End If

        '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
        STR僗働嬫暘 = "1"

        '怳懼嬫暘偺嫟捠曄悢愝掕
        STR怳懼嬫暘 = "0"

        '擖椡怳懼擔偺嫟捠曄悢愝掕
        STR擭娫擖椡怳懼擔 = Space(15)

        oraReader = New MyOracleReader(MainDB)
        sql = New StringBuilder(128)
        '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '01' AND ")
        sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

        If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
        Else     '僗働僕儏乕儖偑懚嵼偟側偄
            '僗働僕儏乕儖嶌惉
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, "01", gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                        gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                'If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, "01", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                    MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    oraReader.Close()
                    Return False
                End If
            End If
            'End If

        End If
        oraReader.Close()

        '-----------------------------------------------

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔
            str媽怳懼擔(i) = str媽嵞怳擔(i)

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "1", "1")

            '怳懼嬫暘偼嵞怳偲偡傞

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                    '2006/11/22丂婛懚儗僐乕僪僠僃僢僋梡
                    str媽嵞怳擔(i) = "00000000"
                Case "2"
                    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
                    '2006/11/22丂婛懚儗僐乕僪僠僃僢僋梡
                    str媽嵞怳擔(i) = PFUNC_SAISAIFURIHI_MAKE(str媽嵞怳擔(i).Substring(4, 2), str媽嵞怳擔(i).Substring(6, 2))
            End Select

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "1"
            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            STR擭娫擖椡怳懼擔 = Space(15)

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                         gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If
        '-----------------------------------------------

        Return True

    End Function
    Private Function PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String) As Boolean

        Dim oraReader As MyOracleReader
        Dim sql As StringBuilder

        PFUNC_TOKUBETU_SAKUSEI_SUB2_KIGYO = False

        '僗働僕儏乕儖丂摿暿儗僐乕僪嶌惉
        '弶怳偑張棟拞偵嵞怳偺僗働僕儏乕儖傪捛壛偡傞嵺偵巊梡

        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)

        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "1", "0")

        '嵞怳擔偺擭偺妋掕張棟
        If s嵞怳懼寧 = "01" Or s嵞怳懼寧 = "02" Or s嵞怳懼寧 = "03" Then
            STRW嵞怳懼擭 = CStr(CInt(txt懳徾擭搙.Text) + 1)
        Else
            STRW嵞怳懼擭 = txt懳徾擭搙.Text
        End If

        '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------START
        '塩嬈擔嶼弌
        Select Case Sai_Zengo_Kbn
            Case 0
                '梻塩嬈擔
                STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
            Case 1
                '慜塩嬈擔
                STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "-")
        End Select
        'STR嵞怳懼擔 = PFUNC_EIGYOUBI_GET(STRW嵞怳懼擭 & s嵞怳懼寧 & s嵞怳懼擔, "0", "+")
        '2011/06/16 昗弨斉廋惓 嵞怳媥擔僔僼僩偺梻塩嬈擔峫椂 ------------------END

        '塩嬈擔傪嶼弌偟偨寢壥偱怳懼擔偲嵞怳懼擔偑摨堦偵側傞応崌偑偁傞堊
        If STR怳懼擔 = STR嵞怳懼擔 Then
            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & Mid(STR怳懼擔, 5, 2) & "寧偺" & "怳懼擔偲嵞怳懼擔偑摨堦偱偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Function
        End If

        Str_FURI_DATE = STR怳懼擔
        Str_SFURI_DATE = STR嵞怳懼擔

        '嵞怳儗僐乕僪偺嶌惉
        If STR嵞怳懼擔 <> "00000000" Then

            '弶怳偱媮傔偨嵞怳擔傪怳懼擔偵愝掕
            STR怳懼擔 = STR嵞怳懼擔

            '2010/10/21 嵞怳偺宊栺怳懼擔傪嶼弌偡傞
            STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s嵞怳懼寧, s嵞怳懼擔, "1", "1")

            '怳懼嬫暘偼嵞怳偲偡傞

            '嵞怳擔偺嶼弌
            Select Case GAKKOU_INFO.SFURI_SYUBETU
                Case "1"
                    '1(嵞怳桳/孞墇柍)
                    STR嵞怳懼擔 = "00000000"
                Case "2"
                    '2(嵞怳桳/孞墇桳)   師夞弶怳擔傪愝掕
                    STR嵞怳懼擔 = PFUNC_SAISAIFURIHI_MAKE(Trim(s嵞怳懼寧), Trim(s嵞怳懼擔))
            End Select

            '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
            STR僗働嬫暘 = "1"
            '擖椡怳懼擔偺嫟捠曄悢愝掕
            STR擭娫擖椡怳懼擔 = Space(15)

            '怳懼嬫暘偺嫟捠曄悢愝掕
            STR怳懼嬫暘 = "1"

            oraReader = New MyOracleReader(MainDB)
            sql = New StringBuilder(128)
            '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
            sql.Append("SELECT * FROM SCHMAST WHERE ")
            sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
            sql.Append("TORIF_CODE_S = '02' AND ")
            sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

            If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
            Else     '僗働僕儏乕儖偑懚嵼偟側偄
                '僗働僕儏乕儖嶌惉
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
                '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
                If fn_IsExistToriMast(strGakkouCode, "02", gastrITAKU_KNAME_T, _
                                        gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                            gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                    '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                    'If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                    If fn_INSERTSCHMAST(strGakkouCode, "02", STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                        MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", Err.Description)
                        MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        oraReader.Close()
                        Return False
                    End If
                End If
                'End If

            End If
            oraReader.Close()
        End If

        Return True

    End Function

    '=========================================================
    '摿暿僗働僕儏乕儖搊榐帪偺擭娫僗働僕儏乕儖峏怴丂2006/11/30
    '=========================================================
    Private Function PFUNC_TokINSERT_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim j As Integer '               儖乕僾梡曄悢
        Dim strGakunen_FLG(9) As String '妛擭僼儔僌奿擺攝楍
        Dim bFlg As Boolean = False '    儖乕僾撪忦審捠夁敾掕

        '摿暿僗働僕儏乕儖偺妛擭僼儔僌傪攝楍偵奿擺
        strGakunen_FLG(1) = STR侾妛擭
        strGakunen_FLG(2) = STR俀妛擭
        strGakunen_FLG(3) = STR俁妛擭
        strGakunen_FLG(4) = STR係妛擭
        strGakunen_FLG(5) = STR俆妛擭
        strGakunen_FLG(6) = STR俇妛擭
        strGakunen_FLG(7) = STR俈妛擭
        strGakunen_FLG(8) = STR俉妛擭
        strGakunen_FLG(9) = STR俋妛擭

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)
        '------------------------------------------------
        '柧嵶儅僗僞専嶕乮審悢丒嬥妟偺庢摼乯
        '------------------------------------------------
        sql.Append(" SELECT * FROM G_MEIMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_M ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_M = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_M ='" & strFURI_DATE & "'")

        sql.Append(" AND (")

        '僼儔僌偺棫偭偰偄傞妛擭傪忦審偵捛壛
        For j = 1 To 9
            If strGakunen_FLG(j) = 1 Then
                If bFlg = True Then
                    sql.Append(" or")
                End If

                sql.Append(" GAKUNEN_CODE_M = " & j)
                bFlg = True
            End If
        Next j

        sql.Append(" )")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then

            '------------------------------------------------
            '審悢丒嬥妟庢摼
            '------------------------------------------------

            Do Until oraReader.EOF

                lngSYORI_KEN = lngSYORI_KEN + 1
                dblSYORI_KIN = dblSYORI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                If oraReader.GetString("FURIKETU_CODE_M") = "0" Then
                    lngFURI_KEN = lngFURI_KEN + 1
                    dblFURI_KIN = dblFURI_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                Else
                    lngFUNOU_KEN = lngFUNOU_KEN + 1
                    dblFUNOU_KIN = dblFUNOU_KIN + CDbl(oraReader.GetInt64("SEIKYU_KIN_M"))
                End If
                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '擭娫僗働僕儏乕儖峏怴
        '------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '尦偺僨乕僞偵崌嶼暘偺審悢丒嬥妟傪懌偡
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S - " & CDbl(lngSYORI_KEN) & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S - " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S - " & CDbl(lngFURI_KEN) & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S - " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S - " & CDbl(lngFUNOU_KEN) & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S - " & dblFUNOU_KIN)
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            '峏怴張棟僄儔乕
            MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-----------------------------------------------------
        '張棟僼儔僌庢摼乮摿暿僗働僕儏乕儖偺INSERT張棟偵巊梡乯
        '-----------------------------------------------------
        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                strENTRI_FLG = oraReader.GetString("ENTRI_FLG_S")
                strCHECK_FLG = oraReader.GetString("CHECK_FLG_S")
                strDATA_FLG = oraReader.GetString("DATA_FLG_S")
                strFUNOU_FLG = oraReader.GetString("FUNOU_FLG_S")
                strSAIFURI_FLG = oraReader.GetString("SAIFURI_FLG_S")
                strKESSAI_FLG = oraReader.GetString("KESSAI_FLG_S")

                oraReader.NextRead()

            Loop

        End If

        oraReader.Close()

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_KOUSIN() As Boolean

        '嶍彍張棟乮DELETE乯
        If PFUNC_TOKUBETU_DELETE() = False Then
            Return False
        End If

        '嶌惉張棟乮INSERT/UPDATE)
        If PFUNC_TOKUBETU_SAKUSEI("峏怴") = False Then
            Return False
        End If

        '晄梫擭娫僗働僕儏乕儖嶍彍張棟
        If PFUNC_DELETE_GSCHMAST() = False Then
            Return False
        End If

        Return True

    End Function

    '====================================================
    '擭娫僗働僕儏乕儖偺妛擭僼儔僌峏怴丂2006/11/30
    '====================================================
    Private Function PFUNC_NENKAN_GAKUNENFLG_UPDATE(ByVal strNENGETUDO As String, ByVal strFURIKUBUN As String) As Boolean

        PFUNC_NENKAN_GAKUNENFLG_UPDATE = False

        Dim strGakunen_FLG(9) As String '妛擭僼儔僌奿擺梡攝楍
        Dim sql As New StringBuilder(128) '             SQL暥奿擺曄悢

        '摿暿僗働僕儏乕儖偺妛擭僼儔僌傪攝楍偵奿擺
        strGakunen_FLG(1) = STR侾妛擭
        strGakunen_FLG(2) = STR俀妛擭
        strGakunen_FLG(3) = STR俁妛擭
        strGakunen_FLG(4) = STR係妛擭
        strGakunen_FLG(5) = STR俆妛擭
        strGakunen_FLG(6) = STR俇妛擭
        strGakunen_FLG(7) = STR俈妛擭
        strGakunen_FLG(8) = STR俉妛擭
        strGakunen_FLG(9) = STR俋妛擭

        '擭娫僗働僕儏乕儖偺妛擭僼儔僌偺峏怴
        sql.Append("UPDATE  G_SCHMAST SET ")

        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                sql.Append(" GAKUNEN" & j & "_FLG_S ='0'") '摿暿偱僼儔僌偑棫偭偰偄傞妛擭偼擭娫偱偼崀傠偡
            Else
                sql.Append(" GAKUNEN" & j & "_FLG_S ='1'") '摿暿偱僼儔僌偑崀傝偰偄傞妛擭偼擭娫偱偼棫偰傞
            End If
            If j <> 9 Then
                sql.Append(",")
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")

        If strFURIKUBUN <> "*" Then '*丗弶怳丒嵞怳椉曽峏怴
            sql.Append(" AND")
            sql.Append(" FURI_KBN_S ='" & strFURIKUBUN & "'")
        Else
            sql.Append(" AND")
            sql.Append(" (FURI_KBN_S ='0'")
            sql.Append(" or")
            sql.Append(" FURI_KBN_S ='1')")
        End If

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function


    '===============================================
    '摿暿僗働僕儏乕儖張棟僼儔僌僠僃僢僋丂2006/11/30
    '===============================================
    Private Function PFUNC_TOKUBETUFLG_CHECK(ByVal strSyori As String, ByVal strSeikyuNenGetu As String, ByVal i As Integer) As Boolean

        PFUNC_TOKUBETUFLG_CHECK = False

        '張棟偵傛偭偰僠僃僢僋撪梕傪曄峏
        Select Case strSyori

            Case "峏怴" '摿暿僗働僕儏乕儖偑張棟拞
                If SYOKI_TOKUBETU_SCHINFO(i).SyoriFurikae_Flag = True Then

                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & _
                                                  "張棟拞偺偨傔丄曄峏弌棃傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "嶌惉" '擭娫僗働僕儏乕儖偺 嘆僠僃僢僋僼儔僌偑棫偭偰偄偰丄晄擻僼儔僌偑崀傝偰偄傞
                '                           嘇嵞怳僗働僕儏乕儖偑張棟拞
                If SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag <> SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag Or _
                   SYOKI_NENKAN_SCHINFO(CInt(TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckSaiFurikae_Flag = True Then

                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & _
                                                  "擭娫僗働僕儏乕儖偑張棟拞偺偨傔丄嶌惉弌棃傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function

                End If

            Case "嶍彍" '擭娫丒摿暿僗働僕儏乕儖偑張棟拞偱丄堘偆怳懼擔
                If (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date Then

                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & _
                                                  "擭娫僗働僕儏乕儖偑張棟拞偺偨傔丄嶍彍偱偒傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                ElseIf (SYOKI_TOKUBETU_SCHINFO(i).CheckFurikae_Flag = True Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).CheckFurikae_Flag = True) And _
                (SYOKI_TOKUBETU_SCHINFO(i).FunouFurikae_Flag = False Or SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).FunouFurikae_Flag = False) And _
                    Replace(SYOKI_NENKAN_SCHINFO(CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki)).Furikae_Day, "/", "") <> strSeikyuNenGetu & TOKUBETU_SCHINFO(i).Furikae_Date Then
                    '嶍彍忦審捛壛(廋惓) 2007/01/09
                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & _
                                                      "擭娫僗働僕儏乕儖偑張棟拞偺偨傔丄嶍彍偱偒傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETUFLG_CHECK = True

    End Function

    '====================================================
    '摿暿僗働僕儏乕儖嶍彍張棟丂2006/11/30
    '====================================================
    Private Function PFUNC_TOKUBETU_DELETE() As Boolean
        PFUNC_TOKUBETU_DELETE = False

        Dim sql As New StringBuilder(128)

        Dim blnSakujo_Check As Boolean = False
        Dim strNengetu As String '   張棟擭寧
        Dim strSFuri_Date As String '嵞怳擔

        '慡嶍彍張棟丄僉乕偼妛峑僐乕僪丄懳徾擭搙丄僗働僕儏乕儖嬫暘乮侾丗摿暿乯丄張棟僼儔僌乮侽乯
        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =1")
        sql.Append(" AND")
        sql.Append(" ((CHECK_FLG_S =0 AND DATA_FLG_S =0 AND FUNOU_FLG_S =0 ) OR (CHECK_FLG_S =1 AND DATA_FLG_S =1 AND FUNOU_FLG_S =1 ))")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0")

        '2006/11/30丂忦審捛壛乮曄峏偺偁偭偨僨乕僞偺傒嶍彍乯=========================
        For i As Integer = 1 To 6

            '------------------------------------------------------------
            '曄峏偑偁傝丄惪媮寧丒弶怳寧丒弶怳擔棑偑嬻敀偺傕偺傪嶍彍偡傞
            '------------------------------------------------------------
            If bln摿暿峏怴(i) = True And TOKUBETU_SCHINFO(i).Seikyu_Tuki = "" And TOKUBETU_SCHINFO(i).Furikae_Date = "" And _
               TOKUBETU_SCHINFO(i).Furikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki <> "" And _
               SYOKI_TOKUBETU_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date <> "" Then

                '擭寧搙傪庢摼
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '侾乣俁寧
                    strNengetu = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '係乣侾俀寧
                    strNengetu = txt懳徾擭搙.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                '僗働僕儏乕儖偺張棟忬嫷僠僃僢僋
                If PFUNC_TOKUBETUFLG_CHECK("嶍彍", strNengetu, i) = False Then
                    Exit Function
                End If

                '嵞怳擔庢摼
                If SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" Then
                    '嵞怳擔偑嬻敀偺応崌丄0杽傔偡傞
                    strSFuri_Date = "00000000"
                Else
                    strSFuri_Date = strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date
                End If

                '愙懕帉捛壛
                If blnSakujo_Check = True Then
                    sql.Append(" or") '  擇暥栚埲崀
                Else
                    sql.Append(" and(") '堦暥栚
                End If

                '怳懼擔丒嵞怳擔丒怳懼嬫暘偺愝掕
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '0')") 'FURI_KBN_S = 0丗弶怳暘

                '嵞怳偺僗働僕儏乕儖傕嶍彍偡傞
                sql.Append(" or")
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str摿暿嵞乆怳擔(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1丗嵞怳暘

                '----------------------------------------------
                '擭娫僗働僕儏乕儖妛擭僼儔僌峏怴
                '----------------------------------------------
                '巊梡妛擭僼儔僌庢摼
                If PFUNC_GAKUNENFLG_CHECK(SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen1_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen2_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen3_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen4_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen5_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen6_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen7_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen8_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunen9_Check, SYOKI_TOKUBETU_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "柧嵶儅僗僞忣曬庢摼幐攕", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Function
                End If
                '擭娫僗働僕儏乕儖峏怴張棟
                If PFUNC_TokDELETE_NenUPDATE(strNengetu, strNengetu & SYOKI_TOKUBETU_SCHINFO(i).Furikae_Date, strSFuri_Date) = False Then
                    Exit Function
                End If

                bln摿暿峏怴(i) = False '曄峏僼儔僌傪崀傠偡
                blnSakujo_Check = True '嶍彍僼儔僌傪棫偰傞

                '------------------------------------------------------------
                '嵞怳僗働僕儏乕儖偺傒偺嶍彍
                '------------------------------------------------------------
            ElseIf bln摿暿峏怴(i) = True And TOKUBETU_SCHINFO(i).SaiFurikae_Tuki = "" And _
                TOKUBETU_SCHINFO(i).SaiFurikae_Date = "" And SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Tuki <> "" And _
                SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date <> "" Then

                '擭寧搙傪庢摼
                If CInt(SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki) < 4 Then
                    '侾乣俁寧
                    strNengetu = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                Else
                    '係乣侾俀寧
                    strNengetu = txt懳徾擭搙.Text & SYOKI_TOKUBETU_SCHINFO(i).Seikyu_Tuki
                End If

                If blnSakujo_Check = True Then
                    sql.Append(" or") '  擇暥栚埲崀
                Else
                    sql.Append(" and(") '堦暥栚
                End If

                '怳懼擔丒嵞怳擔丒怳懼嬫暘偺愝掕
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_TOKUBETU_SCHINFO(i).SaiFurikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & str摿暿嵞乆怳擔(i) & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '1')") 'FURI_KBN_S = 1丗嵞怳暘

                '嵞怳偺傒嶍彍偟偨応崌丄弶怳傕曄峏偑昁梫側偺偱曄峏僼儔僌偼崀傠偝側偄
                blnSakujo_Check = True '嶍彍僼儔僌傪棫偰傞

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
            '嶍彍僨乕僞偑偁傞応崌偺傒幚峴偡傞
            If MainDB.ExecuteNonQuery(sql) < 0 Then
                MessageBox.Show("僗働僕儏乕儖偺嶍彍張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return False
            End If
        End If

        Return True

    End Function

    '==========================================================
    '摿暿僗働僕儏乕儖嶍彍帪偺擭娫僗働僕儏乕儖峏怴丂2006/11/30
    '==========================================================
    Private Function PFUNC_TokDELETE_NenUPDATE(ByVal strNENGETUDO As String, ByVal strFURI_DATE As String, ByVal strSFURI_DATE As String) As Boolean

        Dim sql As StringBuilder
        Dim oraReader As MyOracleReader

        Dim strGakunen_FLG(9) As String '妛擭僼儔僌奿擺攝楍
        Dim bFlg As Boolean = False '    儖乕僾撪忦審捠夁敾掕

        '摿暿僗働僕儏乕儖偺妛擭僼儔僌傪攝楍偵奿擺
        strGakunen_FLG(1) = STR侾妛擭
        strGakunen_FLG(2) = STR俀妛擭
        strGakunen_FLG(3) = STR俁妛擭
        strGakunen_FLG(4) = STR係妛擭
        strGakunen_FLG(5) = STR俆妛擭
        strGakunen_FLG(6) = STR俇妛擭
        strGakunen_FLG(7) = STR俈妛擭
        strGakunen_FLG(8) = STR俉妛擭
        strGakunen_FLG(9) = STR俋妛擭

        sql = New StringBuilder(128)
        oraReader = New MyOracleReader(MainDB)

        '---------------------------------------------------
        '嶍彍偡傞僗働僕儏乕儖儅僗僞専嶕乮審悢丒嬥妟偺庢摼乯
        '---------------------------------------------------
        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S = '0'")
        sql.Append(" AND")
        sql.Append(" FURI_DATE_S ='" & strFURI_DATE & "'")
        sql.Append(" AND")
        sql.Append(" SFURI_DATE_S ='" & strSFURI_DATE & "'")

        lngSYORI_KEN = 0
        dblSYORI_KIN = 0
        lngFURI_KEN = 0
        dblFURI_KIN = 0
        lngFUNOU_KEN = 0
        dblFUNOU_KIN = 0

        If oraReader.DataReader(sql) = True Then
            '------------------------------------------------
            '審悢丒嬥妟庢摼
            '------------------------------------------------
            Do Until oraReader.EOF

                '張棟審悢丒嬥妟庢摼
                lngSYORI_KEN = CDbl(oraReader.GetInt64("SYORI_KEN_S"))
                dblSYORI_KIN = CDbl(oraReader.GetInt64("SYORI_KIN_S"))
                '怳懼審悢丒嬥妟庢摼
                lngFURI_KEN = CDbl(oraReader.GetInt64("FURI_KEN_S"))
                dblFURI_KIN = CDbl(oraReader.GetInt64("FURI_KIN_S"))
                '晄擻審悢丒嬥妟庢摼
                lngFUNOU_KEN = CDbl(oraReader.GetInt64("FUNOU_KEN_S"))
                dblFUNOU_KIN = CDbl(oraReader.GetInt64("FUNOU_KIN_S"))

                oraReader.NextRead()
            Loop

        End If
        oraReader.Close()

        '------------------------------------------------
        '擭娫僗働僕儏乕儖審悢丒嬥妟峏怴乮弶怳暘偺傒乯
        '------------------------------------------------
        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '尦偺僨乕僞偵崌嶼暘偺審悢丒嬥妟傪懌偡
        sql.Append(" SYORI_KEN_S = SYORI_KEN_S + " & lngSYORI_KEN & ",")
        sql.Append(" SYORI_KIN_S = SYORI_KIN_S + " & dblSYORI_KIN & ",")
        sql.Append(" FURI_KEN_S = FURI_KEN_S + " & lngFURI_KEN & ",")
        sql.Append(" FURI_KIN_S =  FURI_KIN_S + " & dblFURI_KIN & ",")
        sql.Append(" FUNOU_KEN_S = FUNOU_KEN_S + " & lngFUNOU_KEN & ",")
        sql.Append(" FUNOU_KIN_S = FUNOU_KIN_S + " & dblFUNOU_KIN)

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" FURI_KBN_S ='0'")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        '-------------------------------------------------
        '擭娫僗働僕儏乕儖妛擭僼儔僌曄峏乮弶怳丒嵞怳椉曽乯
        '-------------------------------------------------
        bFlg = False

        sql = New StringBuilder(128)

        sql.Append("UPDATE  G_SCHMAST SET ")

        '崌嶼僨乕僞暘偺妛擭僼儔僌傪棫偰傞
        For j As Integer = 1 To 9
            If strGakunen_FLG(j) = "1" Then
                If bFlg = True Then
                    sql.Append(",")
                End If
                sql.Append(" GAKUNEN" & j & "_FLG_S = '1'")
                bFlg = True
            End If
        Next

        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S ='" & strNENGETUDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S ='0'")
        sql.Append(" AND")
        sql.Append(" (FURI_KBN_S ='0'")
        sql.Append(" or")
        sql.Append(" FURI_KBN_S ='1')")

        If MainDB.ExecuteNonQuery(sql) < 0 Then
            MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_TOKUBETU_CHECK(ByVal pIndex As Integer, _
                                          ByVal pSeikyu_Tuki As String, _
                                          ByVal pFuri_Tuki As String, _
                                          ByVal pFuri_Hi As String, _
                                          ByVal pSaiFuri_Tuki As String, _
                                          ByVal pSaiFuri_Hi As String, _
                                          ByVal pSiyouFlag0 As Boolean, ByVal pSiyouFlag1 As Boolean, ByVal pSiyouFlag2 As Boolean, ByVal pSiyouFlag3 As Boolean, ByVal pSiyouFlag4 As Boolean, ByVal pSiyouFlag5 As Boolean, ByVal pSiyouFlag6 As Boolean, ByVal pSiyouFlag7 As Boolean, ByVal pSiyouFlag8 As Boolean, ByVal pSiyouFlag9 As Boolean) As Boolean

        PFUNC_TOKUBETU_CHECK = False

        '嶲徠帪偵庢摼偟偨撪梕偲峏怴帪偵庢摼偟偨撪梕偵曄峏偑偁傞偐偳偆偐偺敾掕傪峴偆

        If pSeikyu_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Seikyu_Tuki Then
            Exit Function
        End If

        If pFuri_Tuki <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Tuki Then
            Exit Function
        End If

        If pFuri_Hi <> SYOKI_TOKUBETU_SCHINFO(pIndex).Furikae_Date Then
            Exit Function
        End If

        Select Case pSiyouFlag0
            Case True
                If SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check = True And _
                   SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check = True Then
                Else
                    Exit Function
                End If
            Case False
                If pSiyouFlag1 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen1_Check Then
                    Exit Function
                End If
                If pSiyouFlag2 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen2_Check Then
                    Exit Function
                End If
                If pSiyouFlag3 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen3_Check Then
                    Exit Function
                End If
                If pSiyouFlag4 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen4_Check Then
                    Exit Function
                End If
                If pSiyouFlag5 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen5_Check Then
                    Exit Function
                End If
                If pSiyouFlag6 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen6_Check Then
                    Exit Function
                End If
                If pSiyouFlag7 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen7_Check Then
                    Exit Function
                End If
                If pSiyouFlag8 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen8_Check Then
                    Exit Function
                End If
                If pSiyouFlag9 <> SYOKI_TOKUBETU_SCHINFO(pIndex).SiyouGakunen9_Check Then
                    Exit Function
                End If
        End Select

        PFUNC_TOKUBETU_CHECK = True

    End Function

    '==================================================================
    '摨偠惪媮寧偵摨偠妛擭僼儔僌偑暋悢棫偭偰偄側偄偐僠僃僢僋 2006/11/30
    '==================================================================
    Private Function PFUNC_GAKNENFLG_CHECK() As Boolean

        PFUNC_GAKNENFLG_CHECK = False

        Dim strSeikyu_Tuki(6) As String '惪媮寧
        Dim strGakunen_FLG(6, 10) As Boolean '妛擭僼儔僌乮摿暿僗働僕儏乕儖斣崋,妛擭乯

        strSeikyu_Tuki(1) = txt摿暿惪媮寧侾.Text
        strSeikyu_Tuki(2) = txt摿暿惪媮寧俀.Text
        strSeikyu_Tuki(3) = txt摿暿惪媮寧俁.Text
        strSeikyu_Tuki(4) = txt摿暿惪媮寧係.Text
        strSeikyu_Tuki(5) = txt摿暿惪媮寧俆.Text
        strSeikyu_Tuki(6) = txt摿暿惪媮寧俇.Text

        '慡妛擭僼儔僌傪庢摼
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        '摨惪媮寧偐偮摨妛擭偺僼儔僌偑棫偭偰偄側偄偐僠僃僢僋
        For i As Integer = 1 To 5
            For j As Integer = i + 1 To 6
                '摨惪媮寧僠僃僢僋乮嬻棑偱側偔丄惪媮寧偑摨偠乯
                If strSeikyu_Tuki(i) <> "" And strSeikyu_Tuki(i) = strSeikyu_Tuki(j) Then
                    For k As Integer = 1 To 9
                        If strGakunen_FLG(i, k) = True And strGakunen_FLG(j, k) = True Then
                            '摨妛擭僼儔僌僠僃僢僋乮椉曽True乯
                            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "摨惪媮寧偵摨妛擭偺張棟偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        ElseIf strGakunen_FLG(i, 10) = True Or strGakunen_FLG(j, 10) = True Then
                            '慡妛擭僼儔僌僠僃僢僋乮偳偪傜偐偑True乯
                            MessageBox.Show("(摿暿僗働僕儏乕儖)" & vbCrLf & "摨惪媮寧偵慡妛擭偺張棟偑偁傝傑偡", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Function
                        End If
                    Next
                End If
            Next
        Next

        PFUNC_GAKNENFLG_CHECK = True

    End Function

    '2007/01/04
    Private Function PFUNC_TOKUBETU_GAKNENFLG_CHECK() As Boolean
        '==================================================================
        '摿暿僗働僕儏乕儖偵巜掕偝傟偰偄傞妛擭偵偮偄偰擭娫僗働僕儏乕儖偺
        '妛擭僼儔僌傪0偵峏怴偡傞 2007/01/04
        '==================================================================

        PFUNC_TOKUBETU_GAKNENFLG_CHECK = False
        Dim strSeikyu_Tuki(6) As String '惪媮寧
        Dim strGakunen_FLG(6, 10) As Boolean '妛擭僼儔僌乮摿暿僗働僕儏乕儖斣崋,妛擭乯

        Dim strSEIKYU_NENGETU As String = ""

        Dim sql As New StringBuilder(128)

        strSeikyu_Tuki(1) = txt摿暿惪媮寧侾.Text
        strSeikyu_Tuki(2) = txt摿暿惪媮寧俀.Text
        strSeikyu_Tuki(3) = txt摿暿惪媮寧俁.Text
        strSeikyu_Tuki(4) = txt摿暿惪媮寧係.Text
        strSeikyu_Tuki(5) = txt摿暿惪媮寧俆.Text
        strSeikyu_Tuki(6) = txt摿暿惪媮寧俇.Text

        '慡妛擭僼儔僌傪庢摼
        PSUB_GAKUNENFLG_GET(strGakunen_FLG)

        For i As Integer = 1 To 6
            If strSeikyu_Tuki(i).Trim = "" Then
                GoTo Next_SEIKYUTUKI
            End If

            '惪媮擭寧偺嶌惉
            strSEIKYU_NENGETU = PFUNC_SEIKYUTUKIHI(strSeikyu_Tuki(i))

            For j As Integer = 1 To 10
                If strGakunen_FLG(i, j) = True Then

                    sql.Length = 0
                    sql.Append("UPDATE  G_SCHMAST SET ")
                    If j = 10 Then
                        sql.Append(" GAKUNEN1_FLG_S ='0', ")
                        sql.Append(" GAKUNEN2_FLG_S ='0', ")
                        sql.Append(" GAKUNEN3_FLG_S ='0', ")
                        sql.Append(" GAKUNEN4_FLG_S ='0', ")
                        sql.Append(" GAKUNEN5_FLG_S ='0', ")
                        sql.Append(" GAKUNEN6_FLG_S ='0', ")
                        sql.Append(" GAKUNEN7_FLG_S ='0', ")
                        sql.Append(" GAKUNEN8_FLG_S ='0', ")
                        sql.Append(" GAKUNEN9_FLG_S ='0' ")
                    Else
                        sql.Append(" GAKUNEN" & j & "_FLG_S ='0' ")
                    End If
                    sql.Append(" WHERE GAKKOU_CODE_S = '" & txtGAKKOU_CODE.Text.Trim & "' ")
                    sql.Append(" AND SCH_KBN_S = '0'")
                    sql.Append(" AND NENGETUDO_S = '" & strSEIKYU_NENGETU & "' ")

                    If MainDB.ExecuteNonQuery(sql) < 0 Then
                        MessageBox.Show("僗働僕儏乕儖儅僗僞偺峏怴張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Return False
                    End If

                End If

            Next
Next_SEIKYUTUKI:
        Next

        Return True

    End Function


#End Region

#Region " Private Sub(悘帪僗働僕儏乕儖)"
    Private Sub PSUB_ZUIJI_GET(ByRef Get_Data() As ZuijiData)

        '悘帪僗働僕儏乕儖僞僽夋柺偱尰嵼昞帵偝傟偰偄傞崁栚偺撪梕傪峔憿懱偵庢摼
        Get_Data(1).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘侾)
        Get_Data(1).Furikae_Tuki = txt悘帪怳懼寧侾.Text
        Get_Data(1).Furikae_Date = txt悘帪怳懼擔侾.Text

        Select Case chk悘帪侾_慡妛擭.Checked
            Case True
                Get_Data(1).SiyouGakunen1_Check = True
                Get_Data(1).SiyouGakunen2_Check = True
                Get_Data(1).SiyouGakunen3_Check = True
                Get_Data(1).SiyouGakunen4_Check = True
                Get_Data(1).SiyouGakunen5_Check = True
                Get_Data(1).SiyouGakunen6_Check = True
                Get_Data(1).SiyouGakunen7_Check = True
                Get_Data(1).SiyouGakunen8_Check = True
                Get_Data(1).SiyouGakunen9_Check = True
            Case False
                Get_Data(1).SiyouGakunen1_Check = chk悘帪侾_侾妛擭.Checked
                Get_Data(1).SiyouGakunen2_Check = chk悘帪侾_俀妛擭.Checked
                Get_Data(1).SiyouGakunen3_Check = chk悘帪侾_俁妛擭.Checked
                Get_Data(1).SiyouGakunen4_Check = chk悘帪侾_係妛擭.Checked
                Get_Data(1).SiyouGakunen5_Check = chk悘帪侾_俆妛擭.Checked
                Get_Data(1).SiyouGakunen6_Check = chk悘帪侾_俇妛擭.Checked
                Get_Data(1).SiyouGakunen7_Check = chk悘帪侾_俈妛擭.Checked
                Get_Data(1).SiyouGakunen8_Check = chk悘帪侾_俉妛擭.Checked
                Get_Data(1).SiyouGakunen9_Check = chk悘帪侾_俋妛擭.Checked
        End Select

        Get_Data(2).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俀)
        Get_Data(2).Furikae_Tuki = txt悘帪怳懼寧俀.Text
        Get_Data(2).Furikae_Date = txt悘帪怳懼擔俀.Text

        Select Case chk悘帪俀_慡妛擭.Checked
            Case True
                Get_Data(2).SiyouGakunen1_Check = True
                Get_Data(2).SiyouGakunen2_Check = True
                Get_Data(2).SiyouGakunen3_Check = True
                Get_Data(2).SiyouGakunen4_Check = True
                Get_Data(2).SiyouGakunen5_Check = True
                Get_Data(2).SiyouGakunen6_Check = True
                Get_Data(2).SiyouGakunen7_Check = True
                Get_Data(2).SiyouGakunen8_Check = True
                Get_Data(2).SiyouGakunen9_Check = True
            Case False
                Get_Data(2).SiyouGakunen1_Check = chk悘帪俀_侾妛擭.Checked
                Get_Data(2).SiyouGakunen2_Check = chk悘帪俀_俀妛擭.Checked
                Get_Data(2).SiyouGakunen3_Check = chk悘帪俀_俁妛擭.Checked
                Get_Data(2).SiyouGakunen4_Check = chk悘帪俀_係妛擭.Checked
                Get_Data(2).SiyouGakunen5_Check = chk悘帪俀_俆妛擭.Checked
                Get_Data(2).SiyouGakunen6_Check = chk悘帪俀_俇妛擭.Checked
                Get_Data(2).SiyouGakunen7_Check = chk悘帪俀_俈妛擭.Checked
                Get_Data(2).SiyouGakunen8_Check = chk悘帪俀_俉妛擭.Checked
                Get_Data(2).SiyouGakunen9_Check = chk悘帪俀_俋妛擭.Checked
        End Select

        Get_Data(3).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俁)
        Get_Data(3).Furikae_Tuki = txt悘帪怳懼寧俁.Text
        Get_Data(3).Furikae_Date = txt悘帪怳懼擔俁.Text

        Select Case chk悘帪俁_慡妛擭.Checked
            Case True
                Get_Data(3).SiyouGakunen1_Check = True
                Get_Data(3).SiyouGakunen2_Check = True
                Get_Data(3).SiyouGakunen3_Check = True
                Get_Data(3).SiyouGakunen4_Check = True
                Get_Data(3).SiyouGakunen5_Check = True
                Get_Data(3).SiyouGakunen6_Check = True
                Get_Data(3).SiyouGakunen7_Check = True
                Get_Data(3).SiyouGakunen8_Check = True
                Get_Data(3).SiyouGakunen9_Check = True
            Case False
                Get_Data(3).SiyouGakunen1_Check = chk悘帪俁_侾妛擭.Checked
                Get_Data(3).SiyouGakunen2_Check = chk悘帪俁_俀妛擭.Checked
                Get_Data(3).SiyouGakunen3_Check = chk悘帪俁_俁妛擭.Checked
                Get_Data(3).SiyouGakunen4_Check = chk悘帪俁_係妛擭.Checked
                Get_Data(3).SiyouGakunen5_Check = chk悘帪俁_俆妛擭.Checked
                Get_Data(3).SiyouGakunen6_Check = chk悘帪俁_俇妛擭.Checked
                Get_Data(3).SiyouGakunen7_Check = chk悘帪俁_俈妛擭.Checked
                Get_Data(3).SiyouGakunen8_Check = chk悘帪俁_俉妛擭.Checked
                Get_Data(3).SiyouGakunen9_Check = chk悘帪俁_俋妛擭.Checked
        End Select

        Get_Data(4).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘係)
        Get_Data(4).Furikae_Tuki = txt悘帪怳懼寧係.Text
        Get_Data(4).Furikae_Date = txt悘帪怳懼擔係.Text

        Select Case chk悘帪係_慡妛擭.Checked
            Case True
                Get_Data(4).SiyouGakunen1_Check = True
                Get_Data(4).SiyouGakunen2_Check = True
                Get_Data(4).SiyouGakunen3_Check = True
                Get_Data(4).SiyouGakunen4_Check = True
                Get_Data(4).SiyouGakunen5_Check = True
                Get_Data(4).SiyouGakunen6_Check = True
                Get_Data(4).SiyouGakunen7_Check = True
                Get_Data(4).SiyouGakunen8_Check = True
                Get_Data(4).SiyouGakunen9_Check = True
            Case False
                Get_Data(4).SiyouGakunen1_Check = chk悘帪係_侾妛擭.Checked
                Get_Data(4).SiyouGakunen2_Check = chk悘帪係_俀妛擭.Checked
                Get_Data(4).SiyouGakunen3_Check = chk悘帪係_俁妛擭.Checked
                Get_Data(4).SiyouGakunen4_Check = chk悘帪係_係妛擭.Checked
                Get_Data(4).SiyouGakunen5_Check = chk悘帪係_俆妛擭.Checked
                Get_Data(4).SiyouGakunen6_Check = chk悘帪係_俇妛擭.Checked
                Get_Data(4).SiyouGakunen7_Check = chk悘帪係_俈妛擭.Checked
                Get_Data(4).SiyouGakunen8_Check = chk悘帪係_俉妛擭.Checked
                Get_Data(4).SiyouGakunen9_Check = chk悘帪係_俋妛擭.Checked
        End Select

        Get_Data(5).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俆)
        Get_Data(5).Furikae_Tuki = txt悘帪怳懼寧俆.Text
        Get_Data(5).Furikae_Date = txt悘帪怳懼擔俆.Text

        Select Case chk悘帪俆_慡妛擭.Checked
            Case True
                Get_Data(5).SiyouGakunen1_Check = True
                Get_Data(5).SiyouGakunen2_Check = True
                Get_Data(5).SiyouGakunen3_Check = True
                Get_Data(5).SiyouGakunen4_Check = True
                Get_Data(5).SiyouGakunen5_Check = True
                Get_Data(5).SiyouGakunen6_Check = True
                Get_Data(5).SiyouGakunen7_Check = True
                Get_Data(5).SiyouGakunen8_Check = True
                Get_Data(5).SiyouGakunen9_Check = True
            Case False
                Get_Data(5).SiyouGakunen1_Check = chk悘帪俆_侾妛擭.Checked
                Get_Data(5).SiyouGakunen2_Check = chk悘帪俆_俀妛擭.Checked
                Get_Data(5).SiyouGakunen3_Check = chk悘帪俆_俁妛擭.Checked
                Get_Data(5).SiyouGakunen4_Check = chk悘帪俆_係妛擭.Checked
                Get_Data(5).SiyouGakunen5_Check = chk悘帪俆_俆妛擭.Checked
                Get_Data(5).SiyouGakunen6_Check = chk悘帪俆_俇妛擭.Checked
                Get_Data(5).SiyouGakunen7_Check = chk悘帪俆_俈妛擭.Checked
                Get_Data(5).SiyouGakunen8_Check = chk悘帪俆_俉妛擭.Checked
                Get_Data(5).SiyouGakunen9_Check = chk悘帪俆_俋妛擭.Checked
        End Select

        Get_Data(6).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb擖弌嬫暘俇)
        Get_Data(6).Furikae_Tuki = txt悘帪怳懼寧俇.Text
        Get_Data(6).Furikae_Date = txt悘帪怳懼擔俇.Text

        Select Case chk悘帪俇_慡妛擭.Checked
            Case True
                Get_Data(6).SiyouGakunen1_Check = True
                Get_Data(6).SiyouGakunen2_Check = True
                Get_Data(6).SiyouGakunen3_Check = True
                Get_Data(6).SiyouGakunen4_Check = True
                Get_Data(6).SiyouGakunen5_Check = True
                Get_Data(6).SiyouGakunen6_Check = True
                Get_Data(6).SiyouGakunen7_Check = True
                Get_Data(6).SiyouGakunen8_Check = True
                Get_Data(6).SiyouGakunen9_Check = True
            Case False
                Get_Data(6).SiyouGakunen1_Check = chk悘帪俇_侾妛擭.Checked
                Get_Data(6).SiyouGakunen2_Check = chk悘帪俇_俀妛擭.Checked
                Get_Data(6).SiyouGakunen3_Check = chk悘帪俇_俁妛擭.Checked
                Get_Data(6).SiyouGakunen4_Check = chk悘帪俇_係妛擭.Checked
                Get_Data(6).SiyouGakunen5_Check = chk悘帪俇_俆妛擭.Checked
                Get_Data(6).SiyouGakunen6_Check = chk悘帪俇_俇妛擭.Checked
                Get_Data(6).SiyouGakunen7_Check = chk悘帪俇_俈妛擭.Checked
                Get_Data(6).SiyouGakunen8_Check = chk悘帪俇_俉妛擭.Checked
                Get_Data(6).SiyouGakunen9_Check = chk悘帪俇_俋妛擭.Checked
        End Select

    End Sub
    Private Sub PSUB_ZUIJI_CLEAR()

        '庢摼偟偨峔憿懱偺弶婜壔

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Date = ""
            SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki = ""
            SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn = 0
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check = False
            SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check = False
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next i

    End Sub
#End Region

#Region " Private Sub(悘帪僗働僕儏乕儖夋柺惂屼)"
    Private Sub PSUB_ZUIJI_FORMAT(Optional ByVal pIndex As Integer = 1)

        'Select case pIndex
        '    Case 0
        '懳徾妛擭僠僃僢僋俛俷倃偺桳岠壔
        Call PSUB_ZUIJI_CHKBOXEnabled(True)
        'End Select

        '張棟懳徾妛擭巜掕僠僃僢僋OFF
        Call PSUB_ZUIJI_CHK(False)

        '怳懼擔擖椡棑丄嵞怳懼擔擖椡棑偺僋儕傾
        Call PSUB_ZUIJI_DAYCLER()

        '擖弌嬫暘僐儞儃儃僢僋僗弶婜
        Call PSUB_ZUIJI_CMB()

        '嶲徠帪庢摼抣曐帩峔憿懱弶婜壔
        Call PSUB_ZUIJI_CLEAR()

    End Sub
    Private Sub PSUB_ZUIJI_CHKBOXEnabled(ByVal pValue As Boolean)

        '懳徾妛擭僠僃僢僋BOX偺桳岠壔
        chk悘帪侾_侾妛擭.Enabled = pValue
        chk悘帪侾_俀妛擭.Enabled = pValue
        chk悘帪侾_俁妛擭.Enabled = pValue
        chk悘帪侾_係妛擭.Enabled = pValue
        chk悘帪侾_俆妛擭.Enabled = pValue
        chk悘帪侾_俇妛擭.Enabled = pValue
        chk悘帪侾_俈妛擭.Enabled = pValue
        chk悘帪侾_俉妛擭.Enabled = pValue
        chk悘帪侾_俋妛擭.Enabled = pValue
        chk悘帪侾_慡妛擭.Enabled = pValue

        chk悘帪俀_侾妛擭.Enabled = pValue
        chk悘帪俀_俀妛擭.Enabled = pValue
        chk悘帪俀_俁妛擭.Enabled = pValue
        chk悘帪俀_係妛擭.Enabled = pValue
        chk悘帪俀_俆妛擭.Enabled = pValue
        chk悘帪俀_俇妛擭.Enabled = pValue
        chk悘帪俀_俈妛擭.Enabled = pValue
        chk悘帪俀_俉妛擭.Enabled = pValue
        chk悘帪俀_俋妛擭.Enabled = pValue
        chk悘帪俀_慡妛擭.Enabled = pValue

        chk悘帪俁_侾妛擭.Enabled = pValue
        chk悘帪俁_俀妛擭.Enabled = pValue
        chk悘帪俁_俁妛擭.Enabled = pValue
        chk悘帪俁_係妛擭.Enabled = pValue
        chk悘帪俁_俆妛擭.Enabled = pValue
        chk悘帪俁_俇妛擭.Enabled = pValue
        chk悘帪俁_俈妛擭.Enabled = pValue
        chk悘帪俁_俉妛擭.Enabled = pValue
        chk悘帪俁_俋妛擭.Enabled = pValue
        chk悘帪俁_慡妛擭.Enabled = pValue

        chk悘帪係_侾妛擭.Enabled = pValue
        chk悘帪係_俀妛擭.Enabled = pValue
        chk悘帪係_俁妛擭.Enabled = pValue
        chk悘帪係_係妛擭.Enabled = pValue
        chk悘帪係_俆妛擭.Enabled = pValue
        chk悘帪係_俇妛擭.Enabled = pValue
        chk悘帪係_俈妛擭.Enabled = pValue
        chk悘帪係_俉妛擭.Enabled = pValue
        chk悘帪係_俋妛擭.Enabled = pValue
        chk悘帪係_慡妛擭.Enabled = pValue

        chk悘帪俆_侾妛擭.Enabled = pValue
        chk悘帪俆_俀妛擭.Enabled = pValue
        chk悘帪俆_俁妛擭.Enabled = pValue
        chk悘帪俆_係妛擭.Enabled = pValue
        chk悘帪俆_俆妛擭.Enabled = pValue
        chk悘帪俆_俇妛擭.Enabled = pValue
        chk悘帪俆_俈妛擭.Enabled = pValue
        chk悘帪俆_俉妛擭.Enabled = pValue
        chk悘帪俆_俋妛擭.Enabled = pValue
        chk悘帪俆_慡妛擭.Enabled = pValue

        chk悘帪俇_侾妛擭.Enabled = pValue
        chk悘帪俇_俀妛擭.Enabled = pValue
        chk悘帪俇_俁妛擭.Enabled = pValue
        chk悘帪俇_係妛擭.Enabled = pValue
        chk悘帪俇_俆妛擭.Enabled = pValue
        chk悘帪俇_俇妛擭.Enabled = pValue
        chk悘帪俇_俈妛擭.Enabled = pValue
        chk悘帪俇_俉妛擭.Enabled = pValue
        chk悘帪俇_俋妛擭.Enabled = pValue
        chk悘帪俇_慡妛擭.Enabled = pValue

    End Sub
    Private Sub PSUB_ZUIJI_DAYCLER()

        '怳懼擔偺僋儕傾張棟
        txt悘帪怳懼寧侾.Text = ""
        txt悘帪怳懼擔侾.Text = ""
        txt悘帪怳懼寧俀.Text = ""
        txt悘帪怳懼擔俀.Text = ""
        txt悘帪怳懼寧俁.Text = ""
        txt悘帪怳懼擔俁.Text = ""
        txt悘帪怳懼寧係.Text = ""
        txt悘帪怳懼擔係.Text = ""
        txt悘帪怳懼寧俆.Text = ""
        txt悘帪怳懼擔俆.Text = ""
        txt悘帪怳懼寧俇.Text = ""
        txt悘帪怳懼擔俇.Text = ""

    End Sub
    Private Sub PSUB_ZUIJI_CHK(ByVal pValue As Boolean)

        '懳徾妛擭桳岠僠僃僢僋OFF
        chk悘帪侾_侾妛擭.Checked = pValue
        chk悘帪侾_俀妛擭.Checked = pValue
        chk悘帪侾_俁妛擭.Checked = pValue
        chk悘帪侾_係妛擭.Checked = pValue
        chk悘帪侾_俆妛擭.Checked = pValue
        chk悘帪侾_俇妛擭.Checked = pValue
        chk悘帪侾_俈妛擭.Checked = pValue
        chk悘帪侾_俉妛擭.Checked = pValue
        chk悘帪侾_俋妛擭.Checked = pValue
        chk悘帪侾_慡妛擭.Checked = pValue

        chk悘帪俀_侾妛擭.Checked = pValue
        chk悘帪俀_俀妛擭.Checked = pValue
        chk悘帪俀_俁妛擭.Checked = pValue
        chk悘帪俀_係妛擭.Checked = pValue
        chk悘帪俀_俆妛擭.Checked = pValue
        chk悘帪俀_俇妛擭.Checked = pValue
        chk悘帪俀_俈妛擭.Checked = pValue
        chk悘帪俀_俉妛擭.Checked = pValue
        chk悘帪俀_俋妛擭.Checked = pValue
        chk悘帪俀_慡妛擭.Checked = pValue

        chk悘帪俁_侾妛擭.Checked = pValue
        chk悘帪俁_俀妛擭.Checked = pValue
        chk悘帪俁_俁妛擭.Checked = pValue
        chk悘帪俁_係妛擭.Checked = pValue
        chk悘帪俁_俆妛擭.Checked = pValue
        chk悘帪俁_俇妛擭.Checked = pValue
        chk悘帪俁_俈妛擭.Checked = pValue
        chk悘帪俁_俉妛擭.Checked = pValue
        chk悘帪俁_俋妛擭.Checked = pValue
        chk悘帪俁_慡妛擭.Checked = pValue

        chk悘帪係_侾妛擭.Checked = pValue
        chk悘帪係_俀妛擭.Checked = pValue
        chk悘帪係_俁妛擭.Checked = pValue
        chk悘帪係_係妛擭.Checked = pValue
        chk悘帪係_俆妛擭.Checked = pValue
        chk悘帪係_俇妛擭.Checked = pValue
        chk悘帪係_俈妛擭.Checked = pValue
        chk悘帪係_俉妛擭.Checked = pValue
        chk悘帪係_俋妛擭.Checked = pValue
        chk悘帪係_慡妛擭.Checked = pValue

        chk悘帪俆_侾妛擭.Checked = pValue
        chk悘帪俆_俀妛擭.Checked = pValue
        chk悘帪俆_俁妛擭.Checked = pValue
        chk悘帪俆_係妛擭.Checked = pValue
        chk悘帪俆_俆妛擭.Checked = pValue
        chk悘帪俆_俇妛擭.Checked = pValue
        chk悘帪俆_俈妛擭.Checked = pValue
        chk悘帪俆_俉妛擭.Checked = pValue
        chk悘帪俆_俋妛擭.Checked = pValue
        chk悘帪俆_慡妛擭.Checked = pValue

        chk悘帪俇_侾妛擭.Checked = pValue
        chk悘帪俇_俀妛擭.Checked = pValue
        chk悘帪俇_俁妛擭.Checked = pValue
        chk悘帪俇_係妛擭.Checked = pValue
        chk悘帪俇_俆妛擭.Checked = pValue
        chk悘帪俇_俇妛擭.Checked = pValue
        chk悘帪俇_俈妛擭.Checked = pValue
        chk悘帪俇_俉妛擭.Checked = pValue
        chk悘帪俇_俋妛擭.Checked = pValue
        chk悘帪俇_慡妛擭.Checked = pValue

    End Sub
    Private Sub PSUB_ZUIJI_CMB(Optional ByVal pIndex As Integer = 0)

        cmb擖弌嬫暘侾.SelectedIndex = pIndex
        cmb擖弌嬫暘俀.SelectedIndex = pIndex
        cmb擖弌嬫暘俁.SelectedIndex = pIndex
        cmb擖弌嬫暘係.SelectedIndex = pIndex
        cmb擖弌嬫暘俆.SelectedIndex = pIndex
        cmb擖弌嬫暘俇.SelectedIndex = pIndex

    End Sub
    Private Sub PSUB_ZUIJI_SET(ByVal cmbBOX As ComboBox, ByVal txtBOX寧 As TextBox, ByVal txtBOX擔 As TextBox, ByVal chkBOX1 As CheckBox, ByVal chkBOX2 As CheckBox, ByVal chkBOX3 As CheckBox, ByVal chkBOX4 As CheckBox, ByVal chkBOX5 As CheckBox, ByVal chkBOX6 As CheckBox, ByVal chkBOX7 As CheckBox, ByVal chkBOX8 As CheckBox, ByVal chkBOX9 As CheckBox, ByVal chkBOXALL As CheckBox, ByVal aReader As MyOracleReader)

        '尰嵼OPEN偟偰偄傞僨乕僞儀乕僗偺撪梕傪夋柺偵昞帵偡傞乮侾崁栚峴扨埵乯

        '擖弌嬥僐儞儃偺愝掕
        cmbBOX.SelectedIndex = GFUNC_CODE_TO_INDEX(STR_TXT_PATH & STR_NYUSYUTU2_TXT, aReader.GetString("FURI_KBN_S"))

        txtBOX寧.Text = Mid(aReader.GetString("FURI_DATE_S"), 5, 2)
        txtBOX擔.Text = Mid(aReader.GetString("FURI_DATE_S"), 7, 2)

        Select Case True
            Case aReader.GetString("ENTRI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("CHECK_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("DATA_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("FUNOU_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("SAIFURI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("KESSAI_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
            Case aReader.GetString("TYUUDAN_FLG_S") = "1"
                SYOKI_ZUIJI_SCHINFO(Int_Zuiji_Flag).Syori_Flag = True
        End Select

        If aReader.GetString("GAKUNEN1_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN2_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN3_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN4_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN5_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN6_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN7_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN8_FLG_S") = "1" And _
               aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
            '慡妛擭僠僃僢僋儃僢僋僗俷俶
            chkBOXALL.Checked = True

            '侾偐傜俋妛擭僠僃僢僋儃僋僗偺巊梡晄壜
            chkBOX1.Enabled = False
            chkBOX2.Enabled = False
            chkBOX3.Enabled = False
            chkBOX4.Enabled = False
            chkBOX5.Enabled = False
            chkBOX6.Enabled = False
            chkBOX7.Enabled = False
            chkBOX8.Enabled = False
            chkBOX9.Enabled = False
        Else
            If aReader.GetString("GAKUNEN1_FLG_S") = "1" Then
                '侾妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX1.Checked = True
            Else
                chkBOX1.Checked = False
            End If

            If aReader.GetString("GAKUNEN2_FLG_S") = "1" Then
                '俀妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX2.Checked = True
            Else
                chkBOX2.Checked = False
            End If

            If aReader.GetString("GAKUNEN3_FLG_S") = "1" Then
                '俁妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX3.Checked = True
            Else
                chkBOX3.Checked = False
            End If

            If aReader.GetString("GAKUNEN4_FLG_S") = "1" Then
                '係妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX4.Checked = True
            Else
                chkBOX4.Checked = False
            End If

            If aReader.GetString("GAKUNEN5_FLG_S") = "1" Then
                '俆妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX5.Checked = True
            Else
                chkBOX5.Checked = False
            End If

            If aReader.GetString("GAKUNEN6_FLG_S") = "1" Then
                '俇妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX6.Checked = True
            Else
                chkBOX6.Checked = False
            End If

            If aReader.GetString("GAKUNEN7_FLG_S") = "1" Then
                '俈妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX7.Checked = True
            Else
                chkBOX7.Checked = False
            End If

            If aReader.GetString("GAKUNEN8_FLG_S") = "1" Then
                '俉妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX8.Checked = True
            Else
                chkBOX8.Checked = False
            End If

            If aReader.GetString("GAKUNEN9_FLG_S") = "1" Then
                '俋妛擭僠僃僢僋儃僢僋僗俷俶
                chkBOX9.Checked = True
            Else
                chkBOX9.Checked = False
            End If
        End If

    End Sub
#End Region

#Region " Private Function(悘帪僗働僕儏乕儖)"
    Private Function PFUNC_SCH_GET_ZUIJI() As Boolean

        PFUNC_SCH_GET_ZUIJI = False

        '悘帪張棟
        '懳徾妛擭僠僃僢僋俛俷倃偺桳岠壔
        Call PSUB_ZUIJI_CHKBOXEnabled(True)

        '張棟懳徾妛擭巜掕僠僃僢僋OFF
        Call PSUB_ZUIJI_CHK(False)

        '怳懼擔擖椡棑偺僋儕傾
        Call PSUB_ZUIJI_DAYCLER()

        '悘帪張棟 嶲徠婡擻
        If PFUNC_ZUIJI_SANSYOU() = False Then
            Exit Function
        End If

        PFUNC_SCH_GET_ZUIJI = True

    End Function

    Private Function PFUNC_SCH_DELETE_INSERT_ZUIJI() As Boolean

        '悘帪僗働僕儏乕儖峏怴張棟
        If PFUNC_ZUIJI_KOUSIN() = False Then

            '偙偙傪捠傞偲偄偆偙偲偼侾審偱傕張棟偟偨儗僐乕僪偑懚嵼偟偨偲偄偆偙偲側偺偱
            Int_Syori_Flag(2) = 2

            Return False
        End If

        Return True

    End Function

    Private Function PFUNC_ZUIJI_SANSYOU() As Boolean

        '悘帪怳懼擔丂嶲徠張棟
        PFUNC_ZUIJI_SANSYOU = False

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        For i As Integer = 1 To 6
            SYOKI_ZUIJI_SCHINFO(i).Syori_Flag = False
        Next

        sql.Append(" SELECT * FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S = 2")
        sql.Append(" ORDER BY FURI_DATE_S ASC")

        If oraReader.DataReader(sql) = True Then

            Do Until oraReader.EOF

                '嬻偄偰偄傞崁栚峴偵僨乕僞儀乕僗偺撪梕傪僙僢僩偡傞
                Select Case True
                    Case (txt悘帪怳懼寧侾.Text = "")
                        Int_Zuiji_Flag = 1
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘侾, txt悘帪怳懼寧侾, txt悘帪怳懼擔侾, chk悘帪侾_侾妛擭, chk悘帪侾_俀妛擭, chk悘帪侾_俁妛擭, chk悘帪侾_係妛擭, chk悘帪侾_俆妛擭, chk悘帪侾_俇妛擭, chk悘帪侾_俈妛擭, chk悘帪侾_俉妛擭, chk悘帪侾_俋妛擭, chk悘帪侾_慡妛擭, oraReader)
                    Case (txt悘帪怳懼寧俀.Text = "")
                        Int_Zuiji_Flag = 2
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘俀, txt悘帪怳懼寧俀, txt悘帪怳懼擔俀, chk悘帪俀_侾妛擭, chk悘帪俀_俀妛擭, chk悘帪俀_俁妛擭, chk悘帪俀_係妛擭, chk悘帪俀_俆妛擭, chk悘帪俀_俇妛擭, chk悘帪俀_俈妛擭, chk悘帪俀_俉妛擭, chk悘帪俀_俋妛擭, chk悘帪俀_慡妛擭, oraReader)
                    Case (txt悘帪怳懼寧俁.Text = "")
                        Int_Zuiji_Flag = 3
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘俁, txt悘帪怳懼寧俁, txt悘帪怳懼擔俁, chk悘帪俁_侾妛擭, chk悘帪俁_俀妛擭, chk悘帪俁_俁妛擭, chk悘帪俁_係妛擭, chk悘帪俁_俆妛擭, chk悘帪俁_俇妛擭, chk悘帪俁_俈妛擭, chk悘帪俁_俉妛擭, chk悘帪俁_俋妛擭, chk悘帪俁_慡妛擭, oraReader)
                    Case (txt悘帪怳懼寧係.Text = "")
                        Int_Zuiji_Flag = 4
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘係, txt悘帪怳懼寧係, txt悘帪怳懼擔係, chk悘帪係_侾妛擭, chk悘帪係_俀妛擭, chk悘帪係_俁妛擭, chk悘帪係_係妛擭, chk悘帪係_俆妛擭, chk悘帪係_俇妛擭, chk悘帪係_俈妛擭, chk悘帪係_俉妛擭, chk悘帪係_俋妛擭, chk悘帪係_慡妛擭, oraReader)
                    Case (txt悘帪怳懼寧俆.Text = "")
                        Int_Zuiji_Flag = 5
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘俆, txt悘帪怳懼寧俆, txt悘帪怳懼擔俆, chk悘帪俆_侾妛擭, chk悘帪俆_俀妛擭, chk悘帪俆_俁妛擭, chk悘帪俆_係妛擭, chk悘帪俆_俆妛擭, chk悘帪俆_俇妛擭, chk悘帪俆_俈妛擭, chk悘帪俆_俉妛擭, chk悘帪俆_俋妛擭, chk悘帪俆_慡妛擭, oraReader)
                    Case (txt悘帪怳懼寧俇.Text = "")
                        Int_Zuiji_Flag = 6
                        Call PSUB_ZUIJI_SET(cmb擖弌嬫暘俇, txt悘帪怳懼寧俇, txt悘帪怳懼擔俇, chk悘帪俇_侾妛擭, chk悘帪俇_俀妛擭, chk悘帪俇_俁妛擭, chk悘帪俇_係妛擭, chk悘帪俇_俆妛擭, chk悘帪俇_俇妛擭, chk悘帪俇_俈妛擭, chk悘帪俇_俉妛擭, chk悘帪俇_俋妛擭, chk悘帪俇_慡妛擭, oraReader)
                End Select

                oraReader.NextRead()

            Loop
        Else

            oraReader.Close()
            Return False

        End If

        oraReader.Close()

        Return True

    End Function
    Private Function PFUNC_ZUIJI_SAKUSEI(ByVal str張棟 As String) As Boolean

        '悘帪怳懼丂嶌惉張棟
        Dim str擖弌嬫暘 As String
        Dim cmbComboName(6) As ComboBox '2006/11/30丂僐儞儃儃僢僋僗柤

        PFUNC_ZUIJI_SAKUSEI = False

        '2006/11/30丂僐儞儃儃僢僋僗柤傪庢摼
        cmbComboName(1) = cmb擖弌嬫暘侾
        cmbComboName(2) = cmb擖弌嬫暘俀
        cmbComboName(3) = cmb擖弌嬫暘俁
        cmbComboName(4) = cmb擖弌嬫暘係
        cmbComboName(5) = cmb擖弌嬫暘俆
        cmbComboName(6) = cmb擖弌嬫暘俇

        For i As Integer = 1 To 6

            '怴婯嶌惉
            '2006/12/06丂曄峏偑偁偭偨棑偺傒傪峏怴丒嶌惉
            If bln悘帪峏怴(i) = True And ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                If PFUNC_GAKUNENFLG_CHECK(ZUIJI_SCHINFO(i).SiyouGakunen1_Check, ZUIJI_SCHINFO(i).SiyouGakunen2_Check, ZUIJI_SCHINFO(i).SiyouGakunen3_Check, ZUIJI_SCHINFO(i).SiyouGakunen4_Check, ZUIJI_SCHINFO(i).SiyouGakunen5_Check, ZUIJI_SCHINFO(i).SiyouGakunen6_Check, ZUIJI_SCHINFO(i).SiyouGakunen7_Check, ZUIJI_SCHINFO(i).SiyouGakunen8_Check, ZUIJI_SCHINFO(i).SiyouGakunen9_Check, ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) = False Then
                    Exit Function
                End If

                str擖弌嬫暘 = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmbComboName(i))

                '僷儔儊僞偼嘆寧丄嘇擖椡怳懼擔丄嘊嵞怳懼寧丂嘋嵞怳懼擔丂嘍怳懼嬫暘乮擖弌嬫暘)丄嘐僗働僕儏乕儖嬫暘乮2:悘帪)
                If PFUNC_ZUIJI_SAKUSEI_SUB(ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Tuki, ZUIJI_SCHINFO(i).Furikae_Date, "", "", str擖弌嬫暘) = False Then
                    Exit Function
                End If

                '偙偙傪捠傞偲偄偆偙偲偼張棟偵惉岟偟偨偲偄偆偙偲側偺偱
                Int_Syori_Flag(2) = 1
            End If
            'End If

        Next

        PFUNC_ZUIJI_SAKUSEI = True

    End Function

    Private Function PFUNC_ZUIJI_SAKUSEI_SUB(ByVal s惪媮寧 As String, ByVal s寧 As String, ByVal s怳懼擔 As String, ByVal s嵞怳懼寧 As String, ByVal s嵞怳懼擔 As String, ByVal s怳懼嬫暘 As String) As Boolean

        '僗働僕儏乕儖嶌惉丂悘帪儗僐乕僪嶌惉
        PFUNC_ZUIJI_SAKUSEI_SUB = False
        '惪媮擭寧偺嶌惉
        STR惪媮擭寧 = PFUNC_SEIKYUTUKIHI(s惪媮寧)
        '怳懼擔嶼弌
        STR怳懼擔 = PFUNC_FURIHI_MAKE(s寧, s怳懼擔, "2", s怳懼嬫暘)

        '2010/10/21 宊栺怳懼擔傪嶼弌偡傞
        STR宊栺怳懼擔 = PFUNC_KFURIHI_MAKE(s寧, s怳懼擔, "2", s怳懼嬫暘)
        '嵞怳擔
        STR嵞怳懼擔 = "00000000"

        '僗働僕儏乕儖嬫暘偺嫟捠曄悢愝掕
        STR僗働嬫暘 = "2"
        '怳懼嬫暘偺嫟捠曄悢愝掕
        STR怳懼嬫暘 = s怳懼嬫暘
        '擖椡怳懼擔偺嫟捠曄悢愝掕
        STR擭娫擖椡怳懼擔 = Space(15)

        Dim strSQL As String = ""
        '僗働僕儏乕儖儅僗僞搊榐(弶怳)SQL暥嶌惉
        strSQL = PSUB_INSERT_G_SCHMAST_SQL()

        If MainDB.ExecuteNonQuery(strSQL) < 0 Then
            Return False
        End If

        '-----------------------------------------------
        '2006/07/26丂婇嬈帺怳偺悘帪偺僗働僕儏乕儖傕嶌惉
        '-----------------------------------------------
        '婇嬈帺怳楢実帪偺傒
        Dim strTORIF_CODE_N As String
        If STR怳懼嬫暘 = "2" Then  '擖嬥
            strTORIF_CODE_N = "03"
        Else  '弌嬥
            strTORIF_CODE_N = "04"
        End If

        Dim sql As New StringBuilder(128)
        Dim oraReader As New MyOracleReader(MainDB)

        '婛偵搊榐偝傟偰偄傞偐僠僃僢僋
        sql.Append("SELECT * FROM SCHMAST WHERE ")
        sql.Append("TORIS_CODE_S = '" & strGakkouCode & "' AND ")
        sql.Append("TORIF_CODE_S = '" & strTORIF_CODE_N & "' AND ")
        sql.Append("FURI_DATE_S = '" & STR怳懼擔 & "'")

        '撉崬偺傒
        If oraReader.DataReader(sql) = True Then    '僗働僕儏乕儖偑婛偵懚嵼偡傞
        Else     '僗働僕儏乕儖偑懚嵼偟側偄
            '僗働僕儏乕儖嶌惉
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            '庢堷愭儅僗僞偵庢堷愭僐乕僪偑懚嵼偡傞偙偲傪妋擣
            '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
            If fn_IsExistToriMast(strGakkouCode, strTORIF_CODE_N, gastrITAKU_KNAME_T, _
                                    gastrITAKU_NNAME_T, gastrKIGYO_CODE_T, gastrFURI_CODE_T, _
                                     gastrBAITAI_CODE_T, gastrFMT_KBN_T, gastrFILE_NAME_T) = True Then '専嶕偵僸僢僩偟側偐偭偨傜

                '2010/10/21 宊栺怳懼擔懳墳 堷悢偵捛壛
                'If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR怳懼擔, gintPG_KBN.KOBETU) = gintKEKKA.NG Then
                If fn_INSERTSCHMAST(strGakkouCode, strTORIF_CODE_N, STR怳懼擔, gintPG_KBN.KOBETU, STR宊栺怳懼擔) = gintKEKKA.NG Then
                    oraReader.Close()
                    MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", "婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨")
                    MessageBox.Show("婇嬈帺怳偺僗働僕儏乕儖偑搊榐偱偒傑偣傫偱偟偨", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Return False
                End If
            End If
        End If
        oraReader.Close()

        '嵞怳儗僐乕僪嶌惉偼側偄
        PFUNC_ZUIJI_SAKUSEI_SUB = True

    End Function

    Private Function PFUNC_ZUIJI_KOUSIN() As Boolean

        '嶍彍張棟乮DELETE乯
        If PFUNC_ZUIJI_DELETE() = False Then
            Return False
        End If

        '2010/10/21 悘帪僗働僕儏乕儖偺曄峏偵懳墳偡傞
        '嶍彍偝傟偨儗僐乕僪偺峏怴僼儔僌偑False偲側偭偰偄傞偨傔丄傕偆堦搙丄嶌惉偟偰椙偄偐僠僃僢僋偡傞
        For i As Integer = 1 To 6
            '--------------------------------------
            '悘帪僗働僕儏乕儖僠僃僢僋
            '--------------------------------------
            '2006/12/12丂堦晹捛壛丗擖椡偑晄懌偟偰偄偨応崌丄峏怴偟側偄
            If (ZUIJI_SCHINFO(i).Furikae_Tuki = SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki And _
               ZUIJI_SCHINFO(i).Furikae_Date = SYOKI_ZUIJI_SCHINFO(i).Furikae_Date And _
               ZUIJI_SCHINFO(i).Nyusyutu_Kbn = SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn And _
               ZUIJI_SCHINFO(i).SiyouGakunen1_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen1_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen2_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen2_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen3_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen3_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen4_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen4_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen5_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen5_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen6_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen6_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen7_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen7_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen8_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen8_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunen9_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunen9_Check And _
               ZUIJI_SCHINFO(i).SiyouGakunenALL_Check = SYOKI_ZUIJI_SCHINFO(i).SiyouGakunenALL_Check) Or _
               ((ZUIJI_SCHINFO(i).Furikae_Tuki = "" And ZUIJI_SCHINFO(i).Furikae_Date <> "") Or _
               (ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And ZUIJI_SCHINFO(i).Furikae_Date = "")) Then

                bln悘帪峏怴(i) = False '曄峏側偟
            Else
                bln悘帪峏怴(i) = True ' 曄峏偁傝
            End If
        Next

        '嶌惉張棟乮INSERT & UPDATE)
        If PFUNC_ZUIJI_SAKUSEI("峏怴") = False Then
            Return False
        End If

        Return True

    End Function

    '===============================
    '悘帪僨乕僞嶍彍張棟丂2006/11/30
    '===============================
    Private Function PFUNC_ZUIJI_DELETE() As Boolean

        Dim sql As New StringBuilder(128)
        Dim bret As Boolean = False
        Dim blnSakujo_Check As Boolean = False '2006/11/30
        Dim strNengetu As String '   張棟擭寧
        Dim strSFuri_Date As String '嵞怳擔

        '慡嶍彍張棟丄僉乕偼妛峑僐乕僪丄懳徾擭搙丄僗働僕儏乕儖嬫暘乮俀丗悘帪乯丄張棟僼儔僌乮侽乯

        sql.Append(" DELETE  FROM G_SCHMAST")
        sql.Append(" WHERE")
        sql.Append(" GAKKOU_CODE_S ='" & GAKKOU_INFO.GAKKOU_CODE & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S >='" & GAKKOU_INFO.TAISYOU_START_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" NENGETUDO_S <='" & GAKKOU_INFO.TAISYOU_END_NENDO & "'")
        sql.Append(" AND")
        sql.Append(" SCH_KBN_S =2")

        '2006/11/30丂忦審曄峏乮僼儔僌偺棫偭偰偄側偄僨乕僞丒曄峏偺偁偭偨僨乕僞傪嶍彍乯
        sql.Append(" AND")
        sql.Append(" (ENTRI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" CHECK_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" DATA_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" FUNOU_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" SAIFURI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" KESSAI_FLG_S =0")
        sql.Append(" AND")
        sql.Append(" TYUUDAN_FLG_S =0)")

        For i As Integer = 1 To 6

            '曄峏偑偁偭偨傕偺傪嶍彍偡傞乮悘帪僗働僕儏乕儖偼忢偵嵞嶌惉壜擻偲偡傞乯
            If bln悘帪峏怴(i) = True And SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki <> "" And SYOKI_ZUIJI_SCHINFO(i).Furikae_Date <> "" Then

                '擭寧搙傪庢摼
                If CInt(SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki) < 4 Then
                    strNengetu = CInt(txt懳徾擭搙.Text) + 1 & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                Else
                    strNengetu = txt懳徾擭搙.Text & SYOKI_ZUIJI_SCHINFO(i).Furikae_Tuki
                End If

                '嵞怳擔偼 "0" 8寘
                strSFuri_Date = "00000000"

                If blnSakujo_Check = True Then
                    sql.Append(" or")
                Else
                    '2010/10/21 or偩偲慡偰偺悘帪僗働僕儏乕儖偑嶍彍偝傟偰偟傑偆
                    'sql.Append(" or(")
                    sql.Append(" AND (")
                End If

                '忦審捛壛
                sql.Append(" (FURI_DATE_S = '" & strNengetu & SYOKI_ZUIJI_SCHINFO(i).Furikae_Date & "'")
                sql.Append(" AND")
                sql.Append(" SFURI_DATE_S = '" & strSFuri_Date & "'")
                sql.Append(" AND")
                sql.Append(" FURI_KBN_S = '" & SYOKI_ZUIJI_SCHINFO(i).Nyusyutu_Kbn & "')")

                bln悘帪峏怴(i) = False '曄峏僼儔僌傪崀傠偡
                blnSakujo_Check = True '嶍彍僼儔僌傪棫偰傞

            End If
        Next

        If blnSakujo_Check = True Then
            sql.Append(")")
        End If

        '2006/12/11丂嶍彍偡傞懳徾偑堦審傕柍偐偭偨傜幚峴偟側偄
        If blnSakujo_Check = True Then

            If MainDB.ExecuteNonQuery(sql) < 0 Then
                '嶍彍張棟僄儔乕
                MessageBox.Show("(悘帪僗働僕儏乕儖)" & vbCrLf & "僗働僕儏乕儖偺嶍彍張棟偱僄儔乕偑敪惗偟傑偟偨丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                bret = False
            End If

        End If

        Return True

    End Function

    Private Sub PSUB_ZGAKUNEN_CHK()
        '2006/10/12丂巊梡偟偰偄側偄妛擭偺僠僃僢僋儃僢僋僗傪巊梡晄壜偵偡傞

        If GAKKOU_INFO.SIYOU_GAKUNEN <> 9 Then
            chk悘帪侾_俋妛擭.Enabled = False
            chk悘帪俀_俋妛擭.Enabled = False
            chk悘帪俁_俋妛擭.Enabled = False
            chk悘帪係_俋妛擭.Enabled = False
            chk悘帪俆_俋妛擭.Enabled = False
            chk悘帪俇_俋妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 8 Then
            chk悘帪侾_俉妛擭.Enabled = False
            chk悘帪俀_俉妛擭.Enabled = False
            chk悘帪俁_俉妛擭.Enabled = False
            chk悘帪係_俉妛擭.Enabled = False
            chk悘帪俆_俉妛擭.Enabled = False
            chk悘帪俇_俉妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 7 Then
            chk悘帪侾_俈妛擭.Enabled = False
            chk悘帪俀_俈妛擭.Enabled = False
            chk悘帪俁_俈妛擭.Enabled = False
            chk悘帪係_俈妛擭.Enabled = False
            chk悘帪俆_俈妛擭.Enabled = False
            chk悘帪俇_俈妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 6 Then
            chk悘帪侾_俇妛擭.Enabled = False
            chk悘帪俀_俇妛擭.Enabled = False
            chk悘帪俁_俇妛擭.Enabled = False
            chk悘帪係_俇妛擭.Enabled = False
            chk悘帪俆_俇妛擭.Enabled = False
            chk悘帪俇_俇妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 5 Then
            chk悘帪侾_俆妛擭.Enabled = False
            chk悘帪俀_俆妛擭.Enabled = False
            chk悘帪俁_俆妛擭.Enabled = False
            chk悘帪係_俆妛擭.Enabled = False
            chk悘帪俆_俆妛擭.Enabled = False
            chk悘帪俇_俆妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 4 Then
            chk悘帪侾_係妛擭.Enabled = False
            chk悘帪俀_係妛擭.Enabled = False
            chk悘帪俁_係妛擭.Enabled = False
            chk悘帪係_係妛擭.Enabled = False
            chk悘帪俆_係妛擭.Enabled = False
            chk悘帪俇_係妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 3 Then
            chk悘帪侾_俁妛擭.Enabled = False
            chk悘帪俀_俁妛擭.Enabled = False
            chk悘帪俁_俁妛擭.Enabled = False
            chk悘帪係_俁妛擭.Enabled = False
            chk悘帪俆_俁妛擭.Enabled = False
            chk悘帪俇_俁妛擭.Enabled = False
        End If

        If GAKKOU_INFO.SIYOU_GAKUNEN < 2 Then
            chk悘帪侾_俀妛擭.Enabled = False
            chk悘帪俀_俀妛擭.Enabled = False
            chk悘帪俁_俀妛擭.Enabled = False
            chk悘帪係_俀妛擭.Enabled = False
            chk悘帪俆_俀妛擭.Enabled = False
            chk悘帪俇_俀妛擭.Enabled = False
        End If
    End Sub

#End Region

#Region "娭悢"

    Public Function fn_DELETESCHMAST(ByVal astrTORIF_CODE As String, ByVal astrFURI_DATE As String) As Boolean
        '----------------------------------------------------------------------------
        'Name       :fn_UPDATE_SCHMAST
        'Description:SCHMAST峏怴(桳岠僼儔僌)
        'Create     :
        'UPDATE     :
        '----------------------------------------------------------------------------

        '婇嬈帺怳偺僗働僕儏乕儖傪嶍彍
        Dim ret As Boolean = False

        Dim SQL As New System.Text.StringBuilder(128)

        Try
            SQL.Append(" DELETE  FROM SCHMAST ")
            SQL.Append(" WHERE TORIS_CODE_S = '" & txtGAKKOU_CODE.Text.Trim.PadLeft(txtGAKKOU_CODE.MaxLength, "0"c) & "'")
            SQL.Append(" AND TORIF_CODE_S = '" & astrTORIF_CODE & "'")
            SQL.Append(" AND FURI_DATE_S = '" & astrFURI_DATE & "'")
            SQL.Append(" AND UKETUKE_FLG_S = '0'")
            SQL.Append(" AND TOUROKU_FLG_S = '0'")
            SQL.Append(" AND HAISIN_FLG_S = '0'")

            If MainDB.ExecuteNonQuery(SQL) < 0 Then
                MainLOG.Write("帺怳僗働僕儏乕儖DELETE", "幐攕", "SQL:" & SQL.ToString)
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            MainLOG.Write("帺怳僗働僕儏乕儖DELETE", "幐攕", "SQL:" & SQL.ToString & "DETAIL:" & ex.ToString)
        End Try

        Return ret

    End Function

#End Region

#Region "INSERTSCHMAST"
    '
    '丂娭悢柤丂-丂fn_INSERTSCHMAST
    '
    '丂婡擻    -  僗働僕儏乕儖嶌惉
    '
    '丂堷悢    -  TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:屄暿 丂2:堦妵
    '
    '丂旛峫    -  捠忢丄悘帪嫟偵弶婜壔
    '
    '
    '2010/10/21 宊栺怳懼擔懳墳 宊栺怳懼擔傪堷悢偵捛壛(徣棯壔)
    'Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer) As Integer
    Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String, ByVal aPG_KUBUN As Integer, Optional ByVal aKFURI_DATE As String = "") As Integer
        '----------------------------------------------------------------------------
        'Name       :fn_insert_SCHMAST
        'Description:僗働僕儏乕儖嶌惉
        'Parameta   :TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:屄暿 丂2:堦妵
        'Create     :2004/08/02
        'UPDATE     :2007/12/26
        '           :***廋惓丂偵督老步� (婇嬈帺怳焦嫁傧嚼惗惉帪偵婇嬈懁焦嫁傧嚼偺崁栚捛壛偺堊乯
        '----------------------------------------------------------------------------

        Dim RetCode As Integer = gintKEKKA.NG

        Dim oraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As StringBuilder
            Dim SCH_DATA(77) As String
            Dim strFURI_DATE As String
            Dim Ret As String

            Dim CLS As New GAKKOU.ClsSchduleMaintenanceClass
            CLS.SetSchTable = ClsSchduleMaintenanceClass.APL.JifuriApplication

            strFURI_DATE = aFURI_DATE.Substring(0, 4) & "/" & aFURI_DATE.Substring(4, 2) & "/" & aFURI_DATE.Substring(6, 2)

            '----------------
            '庢堷愭儅僗僞専嶕
            '----------------
            SQL = New StringBuilder(128)

            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = '" & aTORIS_CODE.Trim & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & aTORIF_CODE.Trim & "'")

            If oraReader.DataReader(SQL) = False Then
                MessageBox.Show("庢堷愭儅僗僞偵嵞怳庢堷愭偑搊榐偝傟偰偄傑偣傫", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                RetCode = gintKEKKA.NG
                Return RetCode
            End If

            '-------------------------------------
            '怳懼擔偼塩嬈擔偺塩嬈擔敾掕乮搚丒擔丒廽嵳擔敾掕乯
            '-------------------------------------
            '僗働僕儏乕儖嶌惉懳徾偺庢堷愭僐乕僪傪拪弌
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(aFURI_DATE), aTORIS_CODE, aTORIF_CODE)

            CLS.SCH.FURI_DATE = GCom.SET_DATE(aFURI_DATE)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            strFURI_DATE = CLS.SCH.FURI_DATE

            '2010/10/21 宊栺怳懼擔懳墳 偙偙偐傜
            If aKFURI_DATE = "" OrElse aKFURI_DATE.Length <> 8 Then
                '堷悢偑側偄応崌偼幚怳懼擔傪愝掕
                CLS.SCH.KFURI_DATE = strFURI_DATE
            Else
                CLS.SCH.KFURI_DATE = aKFURI_DATE
            End If
            '2010/10/21 宊栺怳懼擔懳墳 偙偙傑偱

            Ret = CLS.INSERT_NEW_SCHMAST(0, False, True)

            '------------------
            '儅僗僞搊榐崁栚愝掕
            '------------------
            SCH_DATA(0) = oraReader.GetString("FSYORI_KBN_T")                                       '怳懼張棟嬫暘
            SCH_DATA(1) = aTORIS_CODE                                                           '庢堷愭庡僐乕僪
            SCH_DATA(2) = aTORIF_CODE                                                           '庢堷愭暃僐乕僪
            SCH_DATA(3) = CLS.SCH.FURI_DATE 'strIN_NEN & strIN_TUKI & strIN_HI 'FURI_DATE_S丂 丂'怳懼擔
            '2010/10/21 宊栺怳懼擔懳墳
            'SCH_DATA(4) = CLS.SCH.FURI_DATE '"00000000" 'SAIFURI_DATE_S                         '宊栺怳懼擔=怳懼擔
            SCH_DATA(4) = CLS.SCH.KFURI_DATE                                                    '宊栺怳懼擔
            SCH_DATA(5) = "00000000"                                                            '嵞怳擔
            SCH_DATA(6) = CLS.SCH.KSAIFURI_DATE                                                 '嵞怳梊掕擔
            SCH_DATA(7) = CStr(ConvNullToString(oraReader.GetString("FURI_CODE_T"))).PadLeft(3, "0")  '怳懼僐乕僪俽
            SCH_DATA(8) = CStr(ConvNullToString(oraReader.GetString("KIGYO_CODE_T"))).PadLeft(4, "0") '婇嬈僐乕僪俽
            SCH_DATA(9) = CLS.TR(0).ITAKU_CODE '埾戸幰僐乕僪
            SCH_DATA(10) = CStr(oraReader.GetString("TKIN_NO_T")).PadLeft(4, "0")
            SCH_DATA(11) = CStr(oraReader.GetString("TSIT_NO_T")).PadLeft(3, "0")
            SCH_DATA(12) = oraReader.GetString("SOUSIN_KBN_T")
            SCH_DATA(13) = oraReader.GetString("MOTIKOMI_KBN_T")
            SCH_DATA(14) = oraReader.GetString("BAITAI_CODE_T") 'BAITAI_CODE_S
            SCH_DATA(15) = 0 'MOTIKOMI_SEQ_S
            SCH_DATA(16) = 0 'FILE_SEQ_S
            '庤悢椏寁嶼嬫暘偺嶼弌
            Dim strTUKI_KBN As String = ""
            Select Case aFURI_DATE.Substring(4, 2)
                Case "01"
                    strTUKI_KBN = oraReader.GetString("TUKI1_T")
                Case "02"
                    strTUKI_KBN = oraReader.GetString("TUKI2_T")
                Case "03"
                    strTUKI_KBN = oraReader.GetString("TUKI3_T")
                Case "04"
                    strTUKI_KBN = oraReader.GetString("TUKI4_T")
                Case "05"
                    strTUKI_KBN = oraReader.GetString("TUKI5_T")
                Case "06"
                    strTUKI_KBN = oraReader.GetString("TUKI6_T")
                Case "07"
                    strTUKI_KBN = oraReader.GetString("TUKI7_T")
                Case "08"
                    strTUKI_KBN = oraReader.GetString("TUKI8_T")
                Case "09"
                    strTUKI_KBN = oraReader.GetString("TUKI9_T")
                Case "10"
                    strTUKI_KBN = oraReader.GetString("TUKI10_T")
                Case "11"
                    strTUKI_KBN = oraReader.GetString("TUKI11_T")
                Case "12"
                    strTUKI_KBN = oraReader.GetString("TUKI12_T")
            End Select

            Select Case oraReader.GetString("TESUUTYO_KBN_T")
                Case 0
                    SCH_DATA(17) = "1"          'TESUU_KBN_S
                Case 1
                    Select Case strTUKI_KBN
                        Case "1", "3"
                            SCH_DATA(17) = "2"
                        Case Else
                            SCH_DATA(17) = "3"
                    End Select
                Case 2
                    SCH_DATA(17) = "0"
                Case Else
                    SCH_DATA(17) = "0"
            End Select

            SCH_DATA(18) = "00000000"              '埶棅彂嶌惉擔
            SCH_DATA(19) = CLS.SCH.IRAISYOK_YDATE  '埶棅彂夞廂梊掕擔
            SCH_DATA(20) = CLS.SCH.MOTIKOMI_DATE   'MOTIKOMI_DATE_S
            SCH_DATA(21) = "00000000"              'UKETUKE_DATE_S   
            SCH_DATA(22) = "00000000"              'TOUROKU_DATE_S
            SCH_DATA(23) = CLS.SCH.HAISIN_YDATE    'HAISIN_YDATE_S
            SCH_DATA(24) = "00000000"              'HAISIN_DATE_S
            SCH_DATA(25) = CLS.SCH.HAISIN_YDATE    'SOUSIN_YDATE_S
            SCH_DATA(26) = "00000000"              'SOUSIN_DATE_S
            SCH_DATA(27) = CLS.SCH.FUNOU_YDATE     'FUNOU_YDATE_S
            SCH_DATA(28) = "00000000"              'FUNOU_DATE_S
            SCH_DATA(29) = CLS.SCH.KESSAI_YDATE    'KESSAI_YDATE_S
            SCH_DATA(30) = "00000000"              'KESSAI_DATE_S
            SCH_DATA(31) = CLS.SCH.TESUU_YDATE     'TESUU_YDATE_S
            SCH_DATA(32) = "00000000"              'TESUU_DATE_S
            SCH_DATA(33) = CLS.SCH.HENKAN_YDATE    'HENKAN_YDATE_S
            SCH_DATA(34) = "00000000"              'HENKAN_DATE_S
            SCH_DATA(35) = "00000000"              'UKETORI_DATE_S
            SCH_DATA(36) = "0"                     'UKETUKE_FLG_S
            SCH_DATA(37) = "0"                     'TOUROKU_FLG_S
            SCH_DATA(38) = "0"                     'HAISIN_FLG_S
            SCH_DATA(39) = "0"                     'SAIFURI_FLG_S
            SCH_DATA(40) = "0"                     'SOUSIN_FLG_S
            SCH_DATA(41) = "0"                     'FUNOU_FLG_S
            SCH_DATA(42) = "0"                     'TESUUKEI_FLG_S
            SCH_DATA(43) = "0"                     'TESUUTYO_FLG_S
            SCH_DATA(44) = "0"                     'KESSAI_FLG_S
            SCH_DATA(45) = "0"                     'HENKAN_FLG_S
            SCH_DATA(46) = "0"                     'TYUUDAN_FLG_S
            SCH_DATA(47) = "0"                     'TAKOU_FLG_S
            SCH_DATA(48) = "0"                     'NIPPO_FLG_S
            SCH_DATA(49) = Space(3)                'ERROR_INF_S
            SCH_DATA(50) = 0                       'SYORI_KEN_S
            SCH_DATA(51) = 0                       'SYORI_KIN_S
            SCH_DATA(52) = 0                       'ERR_KEN_S
            SCH_DATA(53) = 0                       'ERR_KIN_S
            SCH_DATA(54) = 0                       'TESUU_KIN_S
            SCH_DATA(55) = 0                       'TESUU_KIN1_S
            SCH_DATA(56) = 0                       'TESUU_KIN2_S
            SCH_DATA(57) = 0                       'TESUU_KIN3_S
            SCH_DATA(58) = 0                       'FURI_KEN_S
            SCH_DATA(59) = 0                       'FURI_KIN_S
            SCH_DATA(60) = 0                       'FUNOU_KEN_S
            SCH_DATA(61) = 0                       'FUNOU_KIN_S
            SCH_DATA(62) = Space(50)               'UFILE_NAME_S
            SCH_DATA(63) = Space(50)               'SFILE_NAME_S
            SCH_DATA(64) = Format(Now, "yyyyMMdd") 'SAKUSEI_DATE_S
            SCH_DATA(65) = Space(14)               'JIFURI_TIME_STAMP_S
            SCH_DATA(66) = Space(14)               'KESSAI_TIME_STAMP_S
            SCH_DATA(67) = Space(14)               'TESUU_TIME_STAMP_S
            SCH_DATA(68) = Space(15)               'YOBI1_S
            SCH_DATA(69) = Space(15)               'YOBI2_S
            SCH_DATA(70) = Space(15)               'YOBI3_S
            SCH_DATA(71) = Space(15)               'YOBI4_S
            SCH_DATA(72) = Space(15)               'YOBI5_S
            SCH_DATA(73) = Space(15)               'YOBI6_S
            SCH_DATA(74) = Space(15)               'YOBI7_S
            SCH_DATA(75) = Space(15)               'YOBI8_S
            SCH_DATA(76) = Space(15)               'YOBI9_S
            SCH_DATA(77) = Space(15)               'YOBI10_S

            '----------------------
            '僗働僕儏乕儖儅僗僞搊榐
            '----------------------
            SQL = New StringBuilder(1024)

            SQL.Append("INSERT INTO SCHMAST ( ")
            SQL.Append("FSYORI_KBN_S")      '0
            SQL.Append(",TORIS_CODE_S")     '1
            SQL.Append(",TORIF_CODE_S")     '2
            SQL.Append(",FURI_DATE_S")      '3
            SQL.Append(",KFURI_DATE_S")     '4
            SQL.Append(",SAIFURI_DATE_S")   '5
            SQL.Append(",KSAIFURI_DATE_S")  '6
            SQL.Append(",FURI_CODE_S")      '7
            SQL.Append(",KIGYO_CODE_S")     '8
            SQL.Append(",ITAKU_CODE_S")     '9
            SQL.Append(",TKIN_NO_S")        '10
            SQL.Append(",TSIT_NO_S")        '11
            SQL.Append(",SOUSIN_KBN_S")     '12
            SQL.Append(",MOTIKOMI_KBN_S")   '13
            SQL.Append(",BAITAI_CODE_S")    '14
            SQL.Append(",MOTIKOMI_SEQ_S")   '15
            SQL.Append(",FILE_SEQ_S")       '16
            SQL.Append(",TESUU_KBN_S")      '17
            SQL.Append(",IRAISYO_DATE_S")   '18
            SQL.Append(",IRAISYOK_YDATE_S") '19
            SQL.Append(",MOTIKOMI_DATE_S")  '20
            SQL.Append(",UKETUKE_DATE_S")   '21
            SQL.Append(",TOUROKU_DATE_S")   '22
            SQL.Append(",HAISIN_YDATE_S")   '23
            SQL.Append(",HAISIN_DATE_S")    '24
            SQL.Append(",SOUSIN_YDATE_S")   '25
            SQL.Append(",SOUSIN_DATE_S")    '26
            SQL.Append(",FUNOU_YDATE_S")    '27
            SQL.Append(",FUNOU_DATE_S")     '28
            SQL.Append(",KESSAI_YDATE_S")   '29
            SQL.Append(",KESSAI_DATE_S")    '30
            SQL.Append(",TESUU_YDATE_S")    '31
            SQL.Append(",TESUU_DATE_S")     '32
            SQL.Append(",HENKAN_YDATE_S")   '33
            SQL.Append(",HENKAN_DATE_S")    '34
            SQL.Append(",UKETORI_DATE_S")   '35
            SQL.Append(",UKETUKE_FLG_S")    '36
            SQL.Append(",TOUROKU_FLG_S")    '37
            SQL.Append(",HAISIN_FLG_S")     '38
            SQL.Append(",SAIFURI_FLG_S")    '39
            SQL.Append(",SOUSIN_FLG_S")     '40
            SQL.Append(",FUNOU_FLG_S")      '41
            SQL.Append(",TESUUKEI_FLG_S")   '42
            SQL.Append(",TESUUTYO_FLG_S")   '43
            SQL.Append(",KESSAI_FLG_S")     '44
            SQL.Append(",HENKAN_FLG_S")     '45
            SQL.Append(",TYUUDAN_FLG_S")    '46
            SQL.Append(",TAKOU_FLG_S")      '47
            SQL.Append(",NIPPO_FLG_S")      '48
            SQL.Append(",ERROR_INF_S")      '49
            SQL.Append(",SYORI_KEN_S")      '50
            SQL.Append(",SYORI_KIN_S")      '51
            SQL.Append(",ERR_KEN_S")        '52
            SQL.Append(",ERR_KIN_S")        '53
            SQL.Append(",TESUU_KIN_S")      '54
            SQL.Append(",TESUU_KIN1_S")     '55
            SQL.Append(",TESUU_KIN2_S")     '56
            SQL.Append(",TESUU_KIN3_S")     '57
            SQL.Append(",FURI_KEN_S")       '58
            SQL.Append(",FURI_KIN_S")       '59
            SQL.Append(",FUNOU_KEN_S")      '60
            SQL.Append(",FUNOU_KIN_S")      '61
            SQL.Append(",UFILE_NAME_S")     '62
            SQL.Append(",SFILE_NAME_S")     '63
            SQL.Append(",SAKUSEI_DATE_S")   '64
            SQL.Append(",JIFURI_TIME_STAMP_S")      '65
            SQL.Append(",KESSAI_TIME_STAMP_S")      '66
            SQL.Append(",TESUU_TIME_STAMP_S")       '67
            SQL.Append(",YOBI1_S")          '68
            SQL.Append(",YOBI2_S")          '69
            SQL.Append(",YOBI3_S")          '70
            SQL.Append(",YOBI4_S")          '71
            SQL.Append(",YOBI5_S")          '72
            SQL.Append(",YOBI6_S")          '73
            SQL.Append(",YOBI7_S")          '74
            SQL.Append(",YOBI8_S")          '75
            SQL.Append(",YOBI9_S")          '76
            SQL.Append(",YOBI10_S")         '77
            SQL.Append(" ) VALUES ( ")
            For cnt As Integer = LBound(SCH_DATA) To UBound(SCH_DATA)
                SQL.Append("'" & SCH_DATA(cnt) & "',")
            Next

            Dim InsertSchmastSQL As String = SQL.ToString

            InsertSchmastSQL = InsertSchmastSQL.Substring(0, SQL.Length - 1) & ")"

            If MainDB.ExecuteNonQuery(InsertSchmastSQL) < 0 Then
                MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(搊榐)", "幐攕", SQL.ToString)
                Return False
                ' 2016/10/14 僞僗僋乯埢晹 ADD 亂ME亃UI_B-99-99(RSV2懳墳) -------------------- START
            Else
                If GetRSKJIni("RSV2_V1.0.0", "MASTPTN") = "2" Then
                    Dim ReturnMessage As String = String.Empty
                    Dim SubMastInsert_Ret As Integer = 0
                    Call CAstExternal.ModExternal.Ex_InsertSchmastSub(SCH_DATA(0), _
                                                                      SCH_DATA(1), _
                                                                      SCH_DATA(2), _
                                                                      SCH_DATA(3), _
                                                                      0, _
                                                                      ReturnMessage, _
                                                                      MainDB)
                End If
                ' 2016/10/14 僞僗僋乯埢晹 ADD 亂ME亃UI_B-99-99(RSV2懳墳) -------------------- END
            End If

            RetCode = gintKEKKA.OK

        Catch ex As Exception

            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "梊婜偣偸僄儔乕", "幐攕", ex.ToString)
            RetCode = gintKEKKA.NG

            Return RetCode

        Finally

            If Not oraReader Is Nothing Then oraReader.Close()

        End Try

        Return RetCode

    End Function
#End Region

#Region " 擖椡Key惂屼娭悢"

    Public Function GFUNC_KEYCHECK(ByRef P_FORM As Form, _
                                   ByRef P_e As System.Windows.Forms.KeyPressEventArgs, _
                                   ByVal P_Mode As Integer) As Boolean
        GFUNC_KEYCHECK = False

        '*****************************************
        '擖椡KEY惂屼
        '*****************************************
        'ENTER僉乕偱師Control傊Focus堏摦
        If P_e.KeyChar = ChrW(13) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

        'BS丒TAB丒ENTER僉乕擖椡帪偼僗僉僢僾
        Select Case P_e.KeyChar
            Case ControlChars.Back, ControlChars.Tab, ChrW(13)

                Exit Function
        End Select

        Select Case P_Mode
            Case 1
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
            Case 2
                If (P_e.KeyChar >= "0"c Or P_e.KeyChar <= "9"c) Or _
                   (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 3
                If (P_e.KeyChar >= "A"c Or P_e.KeyChar <= "Z"c) Or _
                   (P_e.KeyChar >= "a"c Or P_e.KeyChar <= "z"c) Then
                Else
                    P_e.Handled = True
                End If
            Case 5
                If (P_e.KeyChar < "�"c Or P_e.KeyChar > "�"c) Then
                    P_e.Handled = True
                End If
            Case 6 '2007/02/12丂僼儔僌梡
                If (P_e.KeyChar < "0"c Or P_e.KeyChar > "1"c) Then
                    P_e.Handled = True
                End If
            Case 10
                If (P_e.KeyChar < "1"c Or P_e.KeyChar > "9"c) Then
                    P_e.Handled = True
                End If
        End Select

        GFUNC_KEYCHECK = True
    End Function
    Public Sub GSUB_PRESEL(ByRef pTxtFile As TextBox)
        'TEXT堤藜蕺改偺撪梕傪慡慖戰
        pTxtFile.SelectionStart = 0
        pTxtFile.SelectionLength = Len(pTxtFile.Text)
    End Sub
    Public Sub GSUB_NEXTFOCUS(ByRef P_FORM As Form, _
                              ByRef P_e As System.Windows.Forms.KeyEventArgs, _
                              ByRef pTxtFile As TextBox)

        Select Case P_e.KeyData
            Case Keys.Right, Keys.Left
                '仺丒仼儃僞儞
                Exit Sub
            Case Keys.Back, Keys.Tab, Keys.Enter
                'BS丒TAB丒ENTER僉乕
                Exit Sub
            Case Keys.ShiftKey, 65545
                'Shift + Tab僉乕(KeyUp側偺偱Shift僉乕扨懱傕昁梫)
                Exit Sub
        End Select

        '擖椡寘偲嵟戝擖椡寘悢偑堦抳偡傟偽Focus堏摦
        If pTxtFile.MaxLength = Len(Trim(pTxtFile.Text)) Then
            P_FORM.SelectNextControl(P_FORM.ActiveControl, True, True, True, True)
        End If

    End Sub
#End Region

#Region " 巜掕寘慜ZERO媗傔嫟捠娭悢"
    Public Function GFUNC_ZERO_ADD(ByRef pTxtFile As TextBox, _
                                   ByVal pKeta As Byte) As Boolean
        GFUNC_ZERO_ADD = False
        pTxtFile.Text = pTxtFile.Text.Trim.PadLeft(pKeta, "0"c)
        GFUNC_ZERO_ADD = True
    End Function

#End Region

    '
    '丂娭悢柤丂-丂fn_ToriMastIsExist
    '
    '丂婡擻    -  庢堷愭儅僗僞懚嵼僠僃僢僋
    '
    '丂堷悢    -  
    '
    '丂旛峫    -  捠忢丄悘帪嫟偵弶婜壔
    '
    '
    Private Function fn_IsExistToriMast(ByVal TorisCode As String, _
                                        ByVal TorifCode As String, _
                                        ByRef ItakuKName As String, _
                                        ByRef ItakuNName As String, _
                                        ByRef KigyoCode As String, _
                                        ByRef FuriCode As String, _
                                        ByRef BaitaiCode As String, _
                                        ByRef FmtKbn As String, _
                                        ByRef FileName As String) As Boolean

        Dim ret As Boolean = False
        Dim OraReader As New MyOracleReader(MainDB)

        Try
            Dim SQL As String = ""
            SQL = " SELECT * "
            SQL &= " FROM TORIMAST "
            SQL &= " WHERE TORIS_CODE_T = '" & TorisCode & "'"
            SQL &= " AND TORIF_CODE_T = '" & TorifCode & "'"

            If OraReader.DataReader(SQL) = False Then
                ret = False
            Else
                ItakuKName = OraReader.GetString("ITAKU_KNAME_T")
                ItakuNName = OraReader.GetString("ITAKU_NNAME_T")
                KigyoCode = OraReader.GetString("KIGYO_CODE_T")
                FuriCode = OraReader.GetString("FURI_CODE_T")
                BaitaiCode = OraReader.GetString("BAITAI_CODE_T")
                FmtKbn = OraReader.GetString("FMT_KBN_T")
                FileName = OraReader.GetString("FILE_NAME_T")

                ret = True
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "梊婜偣偸僄儔乕", "幐攕", ex.ToString)
            ret = False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function
    Private Sub cmbKana_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged

        '2017/05/16 僞僗僋乯惣栰 DEL 昗弨斉廋惓乮僇僫専嶕偺僋儕傾懳墳乯----------------- START
        'If cmbKana.Text = "" Then
        '    Exit Sub
        'End If
        '2017/05/16 僞僗僋乯惣栰 DEL 昗弨斉廋惓乮僇僫専嶕偺僋儕傾懳墳乯----------------- END

        '妛峑専嶕
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Exit Sub
        End If

    End Sub
    Private Sub cmbGakkouName_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbGakkouName.SelectedIndexChanged

        If cmbGakkouName.SelectedIndex = -1 Then

            Exit Sub
        End If

        '擭娫僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_NENKAN_FORMAT()

        '摿暿僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_TOKUBETU_FORMAT()

        '悘帪僗働僕儏乕儖夋柺弶婜壔
        Call PSUB_ZUIJI_FORMAT()

        '妛峑専嶕屻偺妛峑僐乕僪愝掕
        txtGAKKOU_CODE.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
        '2007/02/15
        txtGAKKOU_CODE.Focus()

        '妛峑柤偺庢摼(妛峑忣曬傕曄悢偵奿擺偝傟傞)
        If PFUNC_GAKINFO_GET() = False Then
            Exit Sub
        End If

        '2006/10/12丂嵟崅妛擭埲忋偺妛擭偺巊梡晄壜
        PSUB_TGAKUNEN_CHK()
        PSUB_ZGAKUNEN_CHK()

        '嵞怳懼擔偺僾儘僥僋僩True
        Call PSUB_SAIFURI_PROTECT(True)

        Select Case GAKKOU_INFO.SFURI_SYUBETU
            Case "0", "3"
                Call PSUB_SAIFURI_PROTECT(False)
        End Select
        '2007/02/15
        If Trim(txtGAKKOU_CODE.Text) <> "" And Trim(txt懳徾擭搙.Text) <> "" Then
            '懳徾擭搙傕擖椡偝傟偰偄傞応崌丄僗働僕儏乕儖懚嵼僠僃僢僋傪偐偗
            '僗働僕儏乕儖偑懚嵼偡傞応崌偼嶲徠儃僞儞偵僼僅乕僇僗堏摦
            Call PSUB_SANSYOU_FOCUS()
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD 妛峑彅夛旓儊儞僥僫儞僗 ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END

#Region "摿暿怳懼擔擖椡僠僃僢僋"
    '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------START
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False
        Try
            '摿暿惪媮擔侾
            If txt摿暿惪媮寧侾.Text.Trim <> "" Then
                If txt摿暿怳懼寧侾.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧侾.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔侾.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔侾.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔侾.Text.Trim <> "" OrElse txt摿暿怳懼寧侾.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧侾.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧侾.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔侾.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧侾.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔侾.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔侾.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧侾.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧侾.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧侾.Text & txt摿暿怳懼擔侾.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧侾.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧侾.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text
                    Else
                        If txt摿暿怳懼寧侾.Text = "03" AndAlso txt摿暿嵞怳懼寧侾.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧侾.Text & txt摿暿嵞怳懼擔侾.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧侾.Focus()
                        Return False
                    End If
                End If
            End If

            '摿暿惪媮擔俀
            If txt摿暿惪媮寧俀.Text.Trim <> "" Then
                If txt摿暿怳懼寧俀.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧俀.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔俀.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔俀.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔俀.Text.Trim <> "" OrElse txt摿暿怳懼寧俀.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧俀.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧俀.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔俀.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧俀.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔俀.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔俀.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧俀.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧俀.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧俀.Text & txt摿暿怳懼擔俀.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧俀.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧俀.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text
                    Else
                        If txt摿暿怳懼寧俀.Text = "03" AndAlso txt摿暿嵞怳懼寧俀.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧俀.Text & txt摿暿嵞怳懼擔俀.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧俀.Focus()
                        Return False
                    End If
                End If
            End If

            '摿暿惪媮擔俁
            If txt摿暿惪媮寧俁.Text.Trim <> "" Then
                If txt摿暿怳懼寧俁.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧俁.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔俁.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔俁.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔俁.Text.Trim <> "" OrElse txt摿暿怳懼寧俁.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧俁.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧俁.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔俁.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧俁.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔俁.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔俁.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧俁.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧俁.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧俁.Text & txt摿暿怳懼擔俁.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧俁.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧俁.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text
                    Else
                        If txt摿暿怳懼寧俁.Text = "03" AndAlso txt摿暿嵞怳懼寧俁.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧俁.Text & txt摿暿嵞怳懼擔俁.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧俁.Focus()
                        Return False
                    End If
                End If
            End If

            '摿暿惪媮擔係
            If txt摿暿惪媮寧係.Text.Trim <> "" Then
                If txt摿暿怳懼寧係.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧係.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔係.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔係.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔係.Text.Trim <> "" OrElse txt摿暿怳懼寧係.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧係.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧係.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔係.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧係.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔係.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔係.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧係.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧係.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧係.Text & txt摿暿怳懼擔係.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧係.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧係.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text
                    Else
                        If txt摿暿怳懼寧係.Text = "03" AndAlso txt摿暿嵞怳懼寧係.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧係.Text & txt摿暿嵞怳懼擔係.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧係.Focus()
                        Return False
                    End If
                End If
            End If

            '摿暿惪媮擔俆
            If txt摿暿惪媮寧俆.Text.Trim <> "" Then
                If txt摿暿怳懼寧俆.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧俆.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔俆.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔俆.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔俆.Text.Trim <> "" OrElse txt摿暿怳懼寧俆.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧俆.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧俆.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔俆.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧俆.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔俆.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔俆.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧俆.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧俆.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧俆.Text & txt摿暿怳懼擔俆.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧俆.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧俆.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text
                    Else
                        If txt摿暿怳懼寧俆.Text = "03" AndAlso txt摿暿嵞怳懼寧俆.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧俆.Text & txt摿暿嵞怳懼擔俆.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧俆.Focus()
                        Return False
                    End If
                End If
            End If

            '摿暿惪媮擔俇
            If txt摿暿惪媮寧俇.Text.Trim <> "" Then
                If txt摿暿怳懼寧俇.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿怳懼寧俇.Focus()
                    Return False
                Else
                    If txt摿暿怳懼擔俇.Text.Trim = "" Then
                        MessageBox.Show(String.Format(MSG0285W, "摿暿怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿怳懼擔俇.Focus()
                        Return False
                    End If
                End If
            Else
                If txt摿暿怳懼擔俇.Text.Trim <> "" OrElse txt摿暿怳懼寧俇.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿惪媮寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿惪媮寧俇.Focus()
                    Return False
                End If
            End If
            If txt摿暿嵞怳懼寧俇.Text.Trim = "" Then
                If txt摿暿嵞怳懼擔俇.Text.Trim <> "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼寧"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼寧俇.Focus()
                    Return False
                End If
            Else
                If txt摿暿嵞怳懼擔俇.Text.Trim = "" Then
                    MessageBox.Show(String.Format(MSG0285W, "摿暿嵞怳懼擔"), msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt摿暿嵞怳懼擔俇.Focus()
                    Return False
                Else
                    '怳懼擔丄嵞怳擔憡娭僠僃僢僋
                    Dim FURI_DATE As String = ""
                    Dim SAIFURI_DATE As String = ""
                    '弶怳擔愝掕
                    If CInt(txt摿暿怳懼寧俇.Text) >= 1 AndAlso CInt(txt摿暿怳懼寧俇.Text) <= 3 Then
                        FURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text
                    Else
                        FURI_DATE = txt懳徾擭搙.Text & txt摿暿怳懼寧俇.Text & txt摿暿怳懼擔俇.Text
                    End If
                    '嵞怳擔愝掕
                    If CInt(txt摿暿嵞怳懼寧俇.Text) >= 1 AndAlso CInt(txt摿暿嵞怳懼寧俇.Text) <= 3 Then
                        SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text
                    Else
                        If txt摿暿怳懼寧俇.Text = "03" AndAlso txt摿暿嵞怳懼寧俇.Text = "04" Then
                            SAIFURI_DATE = CStr(CInt(txt懳徾擭搙.Text) + 1) & txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text
                        Else
                            SAIFURI_DATE = txt懳徾擭搙.Text & txt摿暿嵞怳懼寧俇.Text & txt摿暿嵞怳懼擔俇.Text
                        End If
                    End If
                    If CInt(FURI_DATE) > CInt(SAIFURI_DATE) Then
                        MessageBox.Show("嵞怳擔偵偼弶怳擔埲崀偺怳懼擔傪愝掕偟偰偔偩偝偄丅", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt摿暿嵞怳懼寧俇.Focus()
                        Return False
                    End If
                End If
            End If

        Catch ex As Exception
            MessageBox.Show(MSG0006E, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            MainLOG.Write(LW.UserID, LW.ToriCode, LW.FuriDate, "(擖椡僠僃僢僋)", "幐攕", ex.ToString)
            Return False
        End Try

        PFUNC_Nyuryoku_Check = True

    End Function
    '2011/06/16 昗弨斉廋惓 摿暿怳懼擔憡娭僠僃僢僋捛壛 ------------------END
#End Region

End Class
