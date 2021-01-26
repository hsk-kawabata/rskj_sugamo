Option Explicit Off
Option Strict Off

Imports System.IO
Imports System.Text

Public Class KFGMAIN050

    '�萔���v�Z�p�@�ic:\fskj\txt\�萔���e�[�u��.TXT�̓Ǎ��p�j
    Structure TESUU_TABLE
        Public intKIJYUN_KINKO As Integer
        Public intKIJYUN_KINKEN As Integer
        Public KIJYUN_KIJYUN1 As Double
        Public KIJYUN_KIJYUN2 As Double
        Public KIJYUN_KIJYUN3 As Double
        Public KIJYUN_KIJYUN4 As Double
        Public KIJYUN_KIJYUN5 As Double
        Public KIJYUN1_TESUU As Long
        Public KIJYUN2_TESUU As Long
        Public KIJYUN3_TESUU As Long
        Public KIJYUN4_TESUU As Long
        Public KIJYUN5_TESUU As Long
    End Structure
    Private TESUU_TABLE_DATA(10) As TESUU_TABLE

#Region " ���ʕϐ���` "
    Private Str_Gakkou_Code As String

    Private SNG_ZEIRITU As Single
    Private STR_ZEIRITU As String
    Private Str_Function_Ret As String
    Private Str_Honbu_Code As String
    Private Str_Tesuuryo_Kouza As String
    Private Str_Tesuuryo_Kouza2 As String
    Private Str_Tesuuryo_Kouza3 As String

    Private Str_Utiwake_Code As String
    Private Str_Utiwake_Code2 As String
    Private Str_Utiwake_Code3 As String
    Private Str_Kawase_CenterName As String
    Private Str_IraiNinName As String
    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
    'Private Str_KijyunDate As String
    ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END
    Private Str_Kekka_Path As String

    Private Str_Ret As String

    Private Str_Ginko(3) As String

    Private Str_Meigi As String
    Private Str_Biko1 As String
    Private Str_Biko2 As String

    Private Str_Prt_GAKKOU_CODE As String
    Private Str_Prt_GAKKOU_KNAME As String
    Private Str_Prt_GAKKOU_NNAME As String
    Private Str_Prt_KESSAI_DATE As String
    Private Str_Prt_HONBU_KOUZA As String
    Private Str_Prt_TESUURYO_JIFURI As String
    Private Str_Prt_TESUURYO_FURI As String
    Private Str_Prt_TESUURYO_ETC As String
    Private Str_Prt_JIFURI_KOUZA As String
    Private Str_Prt_FURI_KOUZA As String
    Private Str_Prt_ETC_KOUZA As String
    Private Str_Prt_HIMOKU_NNAME As String
    Private Str_Prt_KESSAI_PATERN As String
    Private Str_Prt_NYUKIN_KINGAKU As String
    Private Str_Prt_KESSAI_KIN_CODE As String
    Private Str_Prt_KESSAI_KIN_KNAME As String
    Private Str_Prt_KESSAI_KIN_NNAME As String
    Private Str_Prt_KESSAI_TENPO As String
    Private Str_Prt_KESSAI_SIT_KNAME As String
    Private Str_Prt_KESSAI_SIT_NNAME As String
    Private Str_Prt_KESSAI_KAMOKU As String
    Private Str_Prt_KESSAI_KOUZA As String
    Private Str_Prt_KESSAI_DATA As String

    Private Str_Prt_FURI_KBN As String '�U�֋敪 2005/06/28
    Private dblFURIKOMI_TESUU_ALL As Double = 0 '�U���萔�����v 2006/04/15
    Private Str_Prt_FURI_DATE As String '�U�֓� 2006/04/18

    Private Str_Err_Gakkou_Name As String
    Private Str_Err_Kessai_Kbn As String
    Private Str_Err_Tesuutyo_Kbn As String

    Private Int_FD_Kbn As Integer

    Private Lng_Upd_Count As Long
    Private Lng_RecordNo As Long

    Private Structure LineData
        <VBFixedStringAttribute(303)> Public Data As String
    End Structure

    Private Line As LineData
    Private NextLine As LineData

    Private Structure Plus
        <VBFixedStringAttribute(280)> Public Data As String
    End Structure

    Private StrLine As Plus

    Public lngSAKI_SUU As Long '����搔
    Public lngALL_KEN As Long '�����v����
    Public lngALL_KIN As Long '�����v���z
    Public lngFURI_ALL_KEN As Long '���U�֌���
    Public lngFURI_ALL_KIN As Long '���U�֋��z
    Public lngFUNOU_ALL_KEN As Long '���s�\����
    Public lngFUNOU_ALL_KIN As Long '���s�\���z
    Public lngTESUU_ALL As Long '�萔�������v
    Public lngTESUU1 As Long '���萔��1
    Public lngTESUU2 As Long '���萔��2
    Public lngTESUU3 As Long '���萔��3

    Public strTESUUTYO_KBN As String '�萔�����敪
    Public strIN_OUT_KBN As String '���o���敪

    Public lngNYUKIN_SCH_CNT As Long '���������σX�P�W���[����
    Public sSyori_Date As String '�ړ� 2005/06/30

    '�t�@�C�����p�ϐ� 2006/04/14
    Structure typ_TITLE
        <VBFixedString(16)> Dim strTITLE_16 As String
    End Structure
    Private strTITLE As typ_TITLE
    '�U���萔���p�ϐ� 2006/04/17
    Public lngFURI_TESUU1 As Long
    Public lngFURI_TESUU2 As Long
    Public lngFURI_TESUU3 As Long
    Public lngFURI_TESUU4 As Long
    Public lngFURI_TESUU5 As Long

    Public lngG_PRTKESSAI_REC As Long

    Public lngSYORIKEN_TOKUBETU As Long = 0
    Public dblSYORIKIN_TOKUBETU As Double = 0
    Public lngFURIKEN_TOKUBETU As Long = 0
    Public dblFURIKIN_TOKUBETU As Double = 0
    Public lngFUNOUKEN_TOKUBETU As Long = 0
    Public dblFUNOUKIN_TOKUBETU As Double = 0
    Public dblTESUU_TOKUBETU As Double = 0
    Public dblTESUU_A1_TOKUBETU As Double = 0
    Public dblTESUU_A2_TOKUBETU As Double = 0
    Public dblTESUU_A3_TOKUBETU As Double = 0


    Public dblTESUU_KIN1_S As Double = 0
    Public dblTESUU_KIN2_S As Double = 0
    Public dblTESUU_KIN3_S As Double = 0
    Public intTESUUTYO_KBN_T As Integer = 0
    Public strGAKKOU_CODE_S As String = ""
    Public strGAKKOU_KNAME_G As String = ""
    Public strTESUUTYOKIN_NO_T As String = ""
    Public strTESUUTYO_SIT_T As String = ""
    Public strTESUUTYO_KAMOKU_T As String = ""
    Public strTESUUTYO_KOUZA_T As String = ""
    Public dblNYUKIN_GAK As Double = 0 '�w�Z���̓������z�i�������z�܂��͐U�֋��z)
    Public iGakunen_Flag() As Integer

#End Region

#Region " Form Load "
    Private Sub KFGMAIN050_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        With Me
            .WindowState = FormWindowState.Normal
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .ControlBox = True
        End With

        Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)

        STR_SYORI_NAME = "�������σf�[�^�쐬"
        STR_COMMAND = "Form_Load"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = ""

        MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

        '2016/10/07 saitou RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
        Call GSUB_CONNECT()
        '2016/10/07 saitou RSV2 ADD --------------------------------------------------------------- END

        '����ŗ��擾
        STR_ZEIRITU = CSng(GFUNC_INI_READ("COMMON", "ZEIRITU"))
        '�{���R�[�h�擾
        Str_Honbu_Code = GFUNC_INI_READ("COMMON", "HONBUCD")

        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- START
        '�a��ϊ�����擾
        'Str_KijyunDate = GFUNC_INI_READ("COMMON", "WAREKIKIJYUNBI")
        ' GENGOU 2019/04/19 DEL ITL)OOKUBO ------------------------------------------------- END

        '�萔�����������擾(���U�萔���E�X�����j
        Str_Tesuuryo_Kouza = GFUNC_INI_READ("KESSAI", "TESUUKOUZA1")

        '�萔�����������擾(���g�p)
        Str_Tesuuryo_Kouza2 = GFUNC_INI_READ("KESSAI", "TESUUKOUZA2")

        '�萔�����������擾(�U���萔��)
        Str_Tesuuryo_Kouza3 = GFUNC_INI_READ("KESSAI", "TESUUKOUZA3")

        '����擾(���U�萔���E�X����)
        Str_Utiwake_Code = GFUNC_INI_READ("KESSAI", "UTIWAKE1")

        '����2�擾(���g�p)
        Str_Utiwake_Code2 = GFUNC_INI_READ("KESSAI", "UTIWAKE2")

        '����3�擾(�X����)
        Str_Utiwake_Code3 = GFUNC_INI_READ("KESSAI", "UTIWAKE3")

        '�˗��l���擾
        Str_IraiNinName = GFUNC_INI_READ("KESSAI", "IRAININ")

        '�בփZ���^�[���擾
        Str_Kawase_CenterName = GFUNC_INI_READ("KAWASE", "KAWASECENTER")

        '����FDPATH�擾
        Str_Kekka_Path = GFUNC_INI_READ("KESSAI", "KEKKAFD")

        'FD�敪�擾
        Int_FD_Kbn = CInt(GFUNC_INI_READ("KESSAI", "FDKBN"))


        'Oracle �ڑ�(Read��p)
        OBJ_CONNECTION_DREAD = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read��p)
        OBJ_CONNECTION_DREAD.Open()

        'Oracle �ڑ�(Read��p)
        OBJ_CONNECTION_DREAD2 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read��p)
        OBJ_CONNECTION_DREAD2.Open()

        'Oracle �ڑ�(Read��p)
        OBJ_CONNECTION_DREAD3 = New Data.OracleClient.OracleConnection(STR_CONNECTION)
        'Oracle OPEN(Read��p)
        OBJ_CONNECTION_DREAD3.Open()

        'TXT�ǂݍ��ݒǉ� 206/04/15
        Call PSUB_TesuryoKijyun_Input()

    End Sub
#End Region

#Region " Button Click "
    Private Sub btnCreate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreate.Click
        '�m�F���b�Z�[�W
        If GFUNC_MESSAGE_QUESTION("�쐬���܂����H") <> vbOK Then
            Exit Sub
        End If

        Cursor.Current = Cursors.WaitCursor()

        '���[���[�N�̍폜
        If PFUNC_DeletePrtKessai() = False Then
            Exit Sub
        End If

        '���͒l�`�F�b�N
        If PFUNC_Nyuryoku_Check() = False Then
            Exit Sub
        End If

        STR_COMMAND = "�������σf�[�^�쐬"
        STR_LOG_GAKKOU_CODE = ""
        STR_LOG_FURI_DATE = STR_FURIKAE_DATE(1)

        If PFUNC_Himoku_Create() = False Then
            Exit Sub
        End If

        '�����ɂ��ēd���͍쐬���Ȃ�(�W����) 2006/04/17
        ''�����̃X�P�W���[�������݂��邩�`�F�b�N
        'If fn_select_G_SCHMAST_NYU() = False Then
        '    Exit Sub
        'End If

        'Select case CInt(STR_FSKJ_FLG)
        '    Case 0
        '�w�Z���U�P��
        '�萔���v�Z
        If PFUNC_Tesuuryo_Keisan() = False Then

            Exit Sub
        End If
        'Case 1
        '        '��Ǝ��U�A�g
        '        '�萔���v�Z
        'End Select

        '���G���^(����)�f�[�^�쐬
        '���[�N�t�@�C���쐬
        If PFUNC_Write_WorkD() = False Then

            Exit Sub
        End If

        '�w�b�_���A�f�[�^���t�@�C���쐬
        If PFUNC_Split_WorkD() = False Then
            MessageBox.Show("���G���^FD�쐬���s", "�������σf�[�^�쐬", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        Else
            MessageBox.Show("���G���^FD�쐬����", "�������σf�[�^�쐬", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
        btnEnd.Focus()
    End Sub
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click

        If OBJ_CONNECTION_DREAD Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD.Close()
            OBJ_CONNECTION_DREAD = Nothing
        End If

        If OBJ_CONNECTION_DREAD2 Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD2.Close()
            OBJ_CONNECTION_DREAD2 = Nothing
        End If

        If OBJ_CONNECTION_DREAD3 Is Nothing Then
        Else
            'Oracle CLOSE
            OBJ_CONNECTION_DREAD3.Close()
            OBJ_CONNECTION_DREAD3 = Nothing
        End If

        Me.Close()

    End Sub
#End Region

#Region " Private Sub "
    Private Sub PSUB_TesuryoKijyun_Input()

        Dim iFileNo As Integer
        Dim iFileCount As Integer

        Dim sFileIndex As String = ""
        Dim sFileItemName As String = ""

        '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        '�萔���e�[�u���t�@�C���̓Ǎ�
        '=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
        iFileNo = FreeFile()
        FileOpen(iFileNo, STR_TXT_PATH & STR_TESUURYO_TXT, OpenMode.Input)

        iFileCount = -1

        Do Until EOF(iFileNo)
            iFileCount += 1
            Input(iFileNo, sFileIndex)
            Input(iFileNo, sFileItemName)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).intKIJYUN_KINKO)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).intKIJYUN_KINKEN)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN1)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN2)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN3)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN4)
            Input(iFileNo, TESUU_TABLE_DATA(iFileCount).KIJYUN_KIJYUN5)
        Loop

        FileClose(iFileNo)

    End Sub
    Private Sub PSUB_Put_Data(ByVal pFileNo As Integer, ByVal pValue As String)

        Line.Data = pValue
        Lng_RecordNo += 1

        FilePut(pFileNo, Line, Lng_RecordNo)

    End Sub
    Private Sub PSUB_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String)

        '���Z�@�փR�[�h�Ǝx�X�R�[�h������Z�@�֖��A�x�X���𒊏o

        Str_Ginko(0) = ""
        Str_Ginko(1) = ""
        Str_Ginko(2) = ""
        Str_Ginko(3) = ""

        If Trim(pGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Sub
        End If

        STR_SQL = "SELECT KIN_KNAME_N , KIN_NNAME_N , SIT_KNAME_N , SIT_NNAME_N  FROM TENMAST "
        STR_SQL += " WHERE KIN_NO_N = '" & pGinko_Code & "'"
        STR_SQL += " AND SIT_NO_N = '" & pSiten_Code & "'"

        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER_DREAD3.HasRows = False Then
            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If
        With OBJ_DATAREADER_DREAD3
            .Read()

            Str_Ginko(0) = .Item("KIN_KNAME_N")
            Str_Ginko(1) = .Item("SIT_KNAME_N")
            Str_Ginko(2) = .Item("KIN_NNAME_N")
            Str_Ginko(3) = .Item("SIT_NNAME_N")
        End With

        If GFUNC_SELECT_SQL5("", 1) = False Then
            Exit Sub
        End If

    End Sub
    '2005/06/20�@�ǉ�
    Private Sub PSUB_GET_TESUUTYO_KBN_T(ByVal pGAKKOU_Code As String)

        '�w�Z�R�[�h����萔�����敪�𒊏o

        If Trim(pGAKKOU_Code) = "" Then
            Exit Sub
        End If

        STR_SQL = "SELECT TESUUTYO_KBN_T  FROM GAKMAST2 "
        STR_SQL += " WHERE GAKKOU_CODE_T = '" & pGAKKOU_Code & "'"

        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
            Exit Sub
        End If

        If OBJ_DATAREADER_DREAD3.HasRows = False Then
            If GFUNC_SELECT_SQL5("", 1) = False Then
                Exit Sub
            End If

            Exit Sub
        End If
        With OBJ_DATAREADER_DREAD3
            .Read()

            strTESUUTYO_KBN = .Item("TESUUTYO_KBN_T")
        End With

        If GFUNC_SELECT_SQL5("", 1) = False Then
            Exit Sub
        End If

    End Sub
    '2016/10/07 ayabe RSV2 ADD �w�Z�������e�i���X ---------------------------------------- START
    Private Sub DispFormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Call GSUB_CLOSE()
    End Sub
    '2016/10/07 ayabe RSV2 ADD --------------------------------------------------------------- END
#End Region

#Region " Private Function "
    Private Function PFUNC_Nyuryoku_Check() As Boolean

        PFUNC_Nyuryoku_Check = False

        '���ϓ�
        If Trim(txtKessaiDateY.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("���ϓ�(�N)�������͂ł��B")
            txtKessaiDateY.Focus()

            Exit Function
        Else
            Select Case (CInt(txtKessaiDateY.Text))
                Case Is >= 2004
                Case Else
                    Call GSUB_MESSAGE_WARNING("���ϓ�(�N)�͂Q�O�O�S�N�ȍ~��ݒ肵�Ă��������B")
                    txtKessaiDateY.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtKessaiDateM.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("���ϓ�(��)�������͂ł��B")
            txtKessaiDateM.Focus()

            Exit Function
        Else
            Select Case (CInt(txtKessaiDateM.Text))
                Case 1 To 12
                Case Else
                    Call GSUB_MESSAGE_WARNING("���ό��͂P���`�P�Q����ݒ肵�Ă��������B")
                    txtKessaiDateM.Focus()

                    Exit Function
            End Select
        End If

        If Trim(txtKessaiDateD.Text) = "" Then
            Call GSUB_MESSAGE_WARNING("���ϓ�(��)�������͂ł��B")
            txtKessaiDateD.Focus()

            Exit Function
        End If

        STR_FURIKAE_DATE(0) = Trim(txtKessaiDateY.Text) & "/" & Trim(txtKessaiDateM.Text) & "/" & Trim(txtKessaiDateD.Text)
        STR_FURIKAE_DATE(1) = Trim(txtKessaiDateY.Text) & Format(CInt(txtKessaiDateM.Text), "00") & Format(CInt(txtKessaiDateD.Text), "00")

        If Not IsDate(STR_FURIKAE_DATE(0)) Then
            Call GSUB_MESSAGE_WARNING("���ϓ�������������܂���")

            txtKessaiDateY.Focus()

            Exit Function
        End If

        '�X�P�W���[�����݃`�F�b�N
        STR_SQL = " SELECT * FROM G_SCHMAST"
        STR_SQL += " WHERE"
        STR_SQL += " KESSAI_YDATE_S ='" & STR_FURIKAE_DATE(1) & "'"


        If GFUNC_ISEXIST(STR_SQL) = False Then
            Call GSUB_MESSAGE_WARNING("���͂������ϓ��ɊY������X�P�W���[�������݂��܂���")

            Exit Function
        End If

        PFUNC_Nyuryoku_Check = True

    End Function

    Private Function PFUNC_Tesuuryo_Keisan() As Boolean

        Dim iTableID As Integer

        Dim lKingaku1 As Long

        Dim lTesuuryo1 As Long
        Dim lTesuuryo2 As Long
        Dim lTesuuryo3 As Long
        '�ǉ� 2006/04/17
        Dim strGAKUNEN1 As String
        Dim strGAKUNEN2 As String
        Dim strGAKUNEN3 As String
        Dim strGAKUNEN4 As String
        Dim strGAKUNEN5 As String
        Dim strGAKUNEN6 As String
        Dim strGAKUNEN7 As String
        Dim strGAKUNEN8 As String
        Dim strGAKUNEN9 As String

        PFUNC_Tesuuryo_Keisan = False

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.*"
        STR_SQL += ",GAKMAST2.*"
        STR_SQL += " FROM "
        STR_SQL += " G_SCHMAST"
        STR_SQL += ",GAKMAST2"
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_S = GAKKOU_CODE_T"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " FUNOU_FLG_S = '1'"
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S = '0'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_FLG_S = '0'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_KBN_T <> '99'"
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S <> '2'" '�����E�Վ��o���f�[�^�ɂ��Ă͎萔���v�Z���Ȃ� 2007/02/10
        'STR_SQL += " FURI_KBN_S <> '2'" '�����f�[�^�ɂ��Ă͎萔���v�Z���Ȃ� 2005/06/30
        STR_SQL += " ORDER BY GAKKOU_CODE_S,FURI_KBN_S"


        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            If lngNYUKIN_SCH_CNT > 0 Then
                PFUNC_Tesuuryo_Keisan = True
            Else
                Call GSUB_MESSAGE_WARNING("�w����̏����Ώې�͂���܂���")
            End If

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD

                '�w�N�t���O�擾 2006/04/17
                strGAKUNEN1 = .Item("GAKUNEN1_FLG_S")
                strGAKUNEN2 = .Item("GAKUNEN2_FLG_S")
                strGAKUNEN3 = .Item("GAKUNEN3_FLG_S")
                strGAKUNEN4 = .Item("GAKUNEN4_FLG_S")
                strGAKUNEN5 = .Item("GAKUNEN5_FLG_S")
                strGAKUNEN6 = .Item("GAKUNEN6_FLG_S")
                strGAKUNEN7 = .Item("GAKUNEN7_FLG_S")
                strGAKUNEN8 = .Item("GAKUNEN8_FLG_S")
                strGAKUNEN9 = .Item("GAKUNEN9_FLG_S")


                iTableID = .Item("TESUU_TABLE_ID_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN1_TESUU = .Item("TESUU_A1_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN2_TESUU = .Item("TESUU_A2_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN3_TESUU = .Item("TESUU_A3_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN4_TESUU = .Item("TESUU_B1_T")
                TESUU_TABLE_DATA(iTableID).KIJYUN5_TESUU = .Item("TESUU_B2_T")

                '�萔�������敪���S�Ă̒����敪�Ŏ萔���v�Z���s�� 2005/06/20
                'Select case CInt(.Item("TESUUTYO_KBN_T"))

                'Case 0, 1
                '�s�x����
                '�ꊇ����

                '���������敪
                Select Case (CInt(.Item("SEIKYU_KBN_T")))
                    Case 0
                        '����������
                        '�@���z = �U�֎萔���P�� * ��������
                        lKingaku1 = .Item("TESUU_TANKA_T") * .Item("SYORI_KEN_S")
                    Case 1
                        '�U�֕�����
                        '�@���z = �U�֎萔���P�� * �U�֍ό���
                        lKingaku1 = .Item("TESUU_TANKA_T") * .Item("FURI_KEN_S")
                End Select

                '����ŋ敪
                Select Case (CInt(.Item("SYOUHI_KBN_T")))
                    Case 0
                        '�O��
                        '�萔���P = �����؎�(�@���z * �ŗ�(INI̧�ق��擾))
                        'lTesuuryo1 = Fix(lKingaku1 * Sng_Zeiritu)
                        lTesuuryo1 = Fix(CDbl(lKingaku1) * Val(STR_ZEIRITU))
                    Case 1
                        '����
                        '�萔���P = �@���z
                        lTesuuryo1 = lKingaku1
                End Select

                '�����v��敪
                Select Case (CInt(.Item("SOURYO_KBN_T")))
                    Case 0
                        '�v�サ�Ȃ�
                        '�萔���Q = 0
                        lTesuuryo2 = 0
                    Case 1
                        '�v�シ��
                        '�萔���Q = ����
                        lTesuuryo2 = CInt(.Item("SOURYO_T"))
                End Select

                '�w�Z���U�͔�ڂ̌��ό������ɐU�荞�ݎ萔���̌v�Z���K�v�Ȃ̂ł����ł͍s��Ȃ� 2006/04/15
                lTesuuryo3 = 0
                ''�萔���R = �U���萔���v�Z
                'lTesuuryo3 = PFUNC_Get_FuriTesuryo(Str_Jikou_Ginko_Code, (.Item("FURI_KIN_S") - (lTesuuryo1 + lTesuuryo2)), .Item("FURI_KEN_S"), iTableID)


                'Case 2
                '    '���ʖƏ�

                '    '�萔���P = 0
                '    '�萔���Q = 0
                '    '�萔���R = 0
                '    lTesuuryo1 = 0
                '    lTesuuryo2 = 0
                '    lTesuuryo3 = 0
                'End Select

                '�Z�o�����萔�����X�P�W���[��(���ݎQ�ƒ��̃��R�[�h)�ɍX�V����
                STR_SQL = " UPDATE  G_SCHMAST SET "
                STR_SQL += " TESUU_KIN_S = " & (lTesuuryo1 + lTesuuryo2 + lTesuuryo3)
                STR_SQL += ",TESUU_KIN1_S = " & lTesuuryo1
                STR_SQL += ",TESUU_KIN2_S = " & lTesuuryo2
                STR_SQL += ",TESUU_KIN3_S = " & lTesuuryo3
                STR_SQL += " WHERE"
                STR_SQL += " GAKKOU_CODE_S = '" & Trim(.Item("GAKKOU_CODE_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " SCH_KBN_S = '" & Trim(.Item("SCH_KBN_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_S = '" & Trim(.Item("FURI_KBN_S")) & "'"
                STR_SQL += " AND"
                STR_SQL += " FURI_DATE_S = '" & Trim(.Item("FURI_DATE_S")) & "'"
                '�w�N�R�[�h�̎w��
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN1_FLG_S = '" & strGAKUNEN1 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN2_FLG_S = '" & strGAKUNEN2 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN3_FLG_S = '" & strGAKUNEN3 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN4_FLG_S = '" & strGAKUNEN4 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN5_FLG_S = '" & strGAKUNEN5 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN6_FLG_S = '" & strGAKUNEN6 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN7_FLG_S = '" & strGAKUNEN7 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN8_FLG_S = '" & strGAKUNEN8 & "'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN9_FLG_S = '" & strGAKUNEN9 & "'"


                If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                    Exit Function
                End If
            End With
        End While

        If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then
            Exit Function
        End If

        PFUNC_Tesuuryo_Keisan = True

    End Function

    Private Function PFUNC_Get_FuriTesuryo(ByVal pGinko_Code As String, _
                                           ByVal pKingaku As Long, _
                                           ByVal pKensuu As Long, _
                                           ByVal pIndex As Integer) As Long

        PFUNC_Get_FuriTesuryo = 0

        If pKingaku <= 0 Then
            Exit Function
        End If

        If pKensuu <= 0 Then
            Exit Function
        End If

        With TESUU_TABLE_DATA(pIndex)
            Select Case .intKIJYUN_KINKO
                Case 0
                    '���X�E���s�𔻒肵�Ȃ�

                    Select Case .intKIJYUN_KINKEN
                        Case 0
                            '�U�֍ϋ��z�̒l�Ŏ萔�����Z�o
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '�U�֕ʎ萔���P =< X > �U�֕ʎ萔���Q
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case .KIJYUN_KIJYUN2 To .KIJYUN_KIJYUN3 - 1
                                    '�U�֕ʎ萔���Q =< X > �U�֕ʎ萔���R
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Is >= .KIJYUN_KIJYUN3
                                    '�U�֕ʎ萔���R =< X 
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                        Case Else
                            '�U�֍ό����̒l�Ŏ萔�����Z�o
                            Select Case (pKensuu)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '�U�֕ʎ萔���P =< X > �U�֕ʎ萔���Q
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case .KIJYUN_KIJYUN2 To .KIJYUN_KIJYUN3 - 1
                                    '�U�֕ʎ萔���Q =< X > �U�֕ʎ萔���R
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Is >= .KIJYUN_KIJYUN3
                                    '�U�֕ʎ萔���R =< X 
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                    End Select
                Case Else
                    '���X�E���s�𔻒肷��

                    '���s�����s���̔���
                    Select Case (pGinko_Code = STR_JIKINKO_CODE)
                        Case True
                            '���s

                            '�U�֍ϋ��z�̒l�Ŏ萔�����Z�o
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '�U�֕ʎ萔���P =< X > �U�֕ʎ萔���Q
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU1
                                Case Is >= .KIJYUN_KIJYUN2
                                    '�U�֕ʎ萔���Q < X
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU2
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                        Case False
                            '���s

                            '�U�֍ϋ��z�̒l�Ŏ萔�����Z�o
                            Select Case (pKingaku)
                                Case .KIJYUN_KIJYUN1 To .KIJYUN_KIJYUN2 - 1
                                    '�U�֕ʎ萔���P =< X > �U�֕ʎ萔���Q
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU3
                                Case Is >= .KIJYUN_KIJYUN2
                                    '�U�֕ʎ萔���Q < X
                                    PFUNC_Get_FuriTesuryo = lngFURI_TESUU4
                                Case Else
                                    PFUNC_Get_FuriTesuryo = 0
                            End Select
                    End Select
            End Select
        End With

    End Function

    Private Function PFUNC_Write_WorkD() As Boolean

        Dim iFileNo As Integer

        Dim sErrorMessage As String

        Dim sEsc_Gakkou_Code As String = ""
        Dim sEsc_Gakkou_Code2 As String = ""
        Dim strFURI_CHK_GAKKOU As String = "" '�U�荞�ݎ萔���p�w�Z�R�[�h�`�F�b�N 2006/04/19
        Dim sEsc_Furi_Kbn As String = "" '��r�U�֋敪 2005/06/30
        Dim sEsc_Furi_Kbn2 As String = "" '��r�U�֋敪 2005/06/30
        Dim sLine_Data As String = ""
        Dim sSortData As String = ""

        Dim lBetudan_Count As Long
        Dim lFutuu_Count As Long
        Dim lTouza_Count As Long
        Dim lKawase_Count As Long
        Dim lTesuuKano_Count As Long

        Dim lBetudan_Nyu_Count As Long '�ʒi�����f�[�^����
        Dim lKawase_Seikyu_Count As Long '�ב֐����f�[�^����

        Dim lRecord_Count As Long
        Dim lCreateRecord_Count As Long

        Dim lTesuuFuka_Count As Long
        Dim lTesuuFuka_Kingaku As Long

        Dim lngKESSAIREC As Long '���σf�[�^�쐬�ΏۃX�P�W���[����

        Dim strGAKKOU_NAME As String = ""
        Dim strTESUU_KIN As String = ""
        Dim strTESUU_SIT As String = ""
        Dim strTESUU_KAMOKU As String = ""
        Dim strTESUU_KOUZA As String = ""


        PFUNC_Write_WorkD = False

        sSyori_Date = Format(Now, "yyyyMMddHHmmss")
        sErrorMessage = ""

        STR_SQL = " SELECT "
        STR_SQL += " G_SCHMAST.*"
        STR_SQL += ",GAKMAST1.*"
        STR_SQL += ",GAKMAST2.*"
        STR_SQL += ",MAIN0500G_WORK.*"
        STR_SQL += " FROM "
        STR_SQL += " G_SCHMAST"
        STR_SQL += ",GAKMAST1"
        STR_SQL += ",GAKMAST2"
        STR_SQL += ",MAIN0500G_WORK"
        STR_SQL += " WHERE GAKKOU_CODE_S = GAKKOU_CODE_H"
        STR_SQL += " AND GAKKOU_CODE_H = GAKKOU_CODE_G"
        STR_SQL += " AND GAKUNEN_CODE_G = 1"
        STR_SQL += " AND GAKKOU_CODE_G = GAKKOU_CODE_T"
        STR_SQL += " AND FURI_KBN_S = FURI_KBN_H" '�U�֋敪�ǉ� 2005/06/28
        STR_SQL += " AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND KESSAI_FLG_S = '0'"
        STR_SQL += " AND TYUUDAN_FLG_S = '0'"
        STR_SQL += " AND DATA_FLG_S = '1'"
        STR_SQL += " AND FUNOU_FLG_S = '1'" '�Վ������̏ꍇ�͂������R�����g
        STR_SQL += " AND SYORI_KIN_S > 0"
        '��ڋ��z��0�̂��͍̂쐬���Ȃ� 2005/06/20
        STR_SQL += " AND HIMOKU_FURI_KIN_H > 0" 'HIMOKU_KINGAKU_H����HIMOKU_FURI_KIN_H�ɕύX 2006/04/14
        STR_SQL += " AND KESSAI_KBN_T <> '99'" '���ϑΏۊO�̓f�[�^�쐬���Ȃ� 2005/06/30
        STR_SQL += " AND KESSAI_KBN_T <> '02'" '��Ǝ��U�A�g���A��Ǝ��U�Ō��ς̂��͍̂쐬���Ȃ� 2005/11/02
        '�Վ������E�o���ɂ��Ă͍쐬���Ȃ� 2006/04/13
        STR_SQL += " AND SCH_KBN_S <> '2'"
        '���֏��� ���ϗ\����E�U�֓��E�w�Z�R�[�h�E���ό������i���E�X�E�ȁE��)�� 2006/10/19
        STR_SQL += " ORDER BY KESSAI_YDATE_S,FURI_DATE_S,GAKKOU_CODE_S,KESSAI_KIN_CODE_H,KESSAI_TENPO_H,KESSAI_KAMOKU_H,KESSAI_KOUZA_H"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If

        If OBJ_DATAREADER_DREAD.HasRows = False Then
            If GFUNC_SELECT_SQL3(STR_SQL, 1) = False Then
                Exit Function
            End If

            Call GSUB_MESSAGE_WARNING("�w����̏����Ώې�͂���܂���")

            Exit Function
        End If

        sEsc_Gakkou_Code = ""
        sEsc_Furi_Kbn = ""

        '�ǉ� 2006/04/17
        lngTESUU_ALL = 0 '�萔�������v
        lngTESUU1 = 0 '���萔��1
        lngTESUU2 = 0 '���萔��2
        lngTESUU3 = 0 '���萔��3

        lngFURI_TESUU1 = 0
        lngFURI_TESUU2 = 0
        lngFURI_TESUU3 = 0
        lngFURI_TESUU4 = 0
        lngFURI_TESUU5 = 0

        dblFURIKOMI_TESUU_ALL = 0 '���v�U���萔�������� 2006/04/15

        iFileNo = FreeFile()

        If Dir$(STR_DAT_PATH & "FD_WORK1_D.DAT") <> "" Then
            Kill(STR_DAT_PATH & "FD_WORK1_D.DAT")
        End If

        FileOpen(iFileNo, STR_DAT_PATH & "FD_WORK1_D.DAT", OpenMode.Random, , , 303)
        lngKESSAIREC = 0
        While (OBJ_DATAREADER_DREAD.Read = True)
            With OBJ_DATAREADER_DREAD
                If .Item("FUNOU_FLG_S") = "0" And .Item("FURI_KBN_S") <> "2" Then
                    GoTo NEXT_RECORD
                Else
                    lngKESSAIREC = lngKESSAIREC + 1
                End If

                Str_Err_Gakkou_Name = .Item("GAKKOU_NNAME_G")
                Str_Err_Kessai_Kbn = .Item("KESSAI_KBN_T")
                Str_Err_Tesuutyo_Kbn = .Item("TESUUTYO_PATN_T")

                '�ǉ� 2006/04/17
                lngFURI_TESUU1 = CLng(.Item("TESUU_A1_T"))
                lngFURI_TESUU2 = CLng(.Item("TESUU_A2_T"))
                lngFURI_TESUU3 = CLng(.Item("TESUU_A3_T"))
                lngFURI_TESUU4 = CLng(.Item("TESUU_B1_T"))
                lngFURI_TESUU5 = CLng(.Item("TESUU_B2_T"))


                '�萔���������@����
                Select Case (CInt(.Item("TESUUTYO_PATN_T")))
                    Case 0, 1
                    Case Else
                        'Goto �}�X�^�ݒ�G���[
                        sErrorMessage = "���Ϗ��Ɍ�肪����܂�(�萔���������@)" & vbCrLf

                        GoTo ERROR_MAST_SET
                End Select

                '���ϋ敪����
                Select Case (CInt(.Item("KESSAI_KBN_T")))
                    Case 99
                        'Goto Next While
                        GoTo NEXT_RECORD
                    Case Else
                        lRecord_Count += 1
                End Select

                '�ʒi�o���d���쐬
                '�A�������̏ꍇ�͕ʒi����
                If Trim(sEsc_Gakkou_Code) <> Trim(.Item("GAKKOU_CODE_S")) Then
                    sEsc_Gakkou_Code = Trim(.Item("GAKKOU_CODE_S"))
                    sEsc_Furi_Kbn = Trim(.Item("FURI_KBN_S")) '��r�U�֋敪

                    If strFURI_CHK_GAKKOU = "" Then '���̂�
                        strFURI_CHK_GAKKOU = .Item("GAKKOU_CODE_S")
                        strGAKKOU_NAME = .Item("GAKKOU_KNAME_G")
                        strTESUU_KIN = .Item("TESUUTYOKIN_NO_T")
                        strTESUU_SIT = .Item("TESUUTYO_SIT_T")
                        strTESUU_KAMOKU = .Item("TESUUTYO_KAMOKU_T  ")
                        strTESUU_KOUZA = .Item("TESUUTYO_KOUZA_T ")
                    End If


                    '�������U�ցE�X��������ѐU���萔��������(��O�̊w�Z�R�[�h�p�d��)
                    If Trim(strFURI_CHK_GAKKOU) <> Trim(.Item("GAKKOU_CODE_S")) Then

                        '�U���萔���X�V 2006/04/17
                        If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
                            'Goto �������݃G���[
                            GoTo ERROR_RIENT_WRITE

                        End If
                        lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

                        '�������z���萔���i�U�ցE�X���萔���{�U���萔��)�������ꍇ�͎萔���͈����Ȃ�
                        If dblNYUKIN_GAK < (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL) Then
                            lngTESUU_ALL = lngTESUU_ALL - (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL)
                        Else
                            If dblTESUU_KIN1_S + dblTESUU_KIN2_S > 0 Then '�萔��+�X����
                                Select Case (intTESUUTYO_KBN_T)
                                    Case 0
                                        sLine_Data = PFUNC_Syokanjo_Nyuukin(strGAKKOU_CODE_S, _
                                                                            strGAKKOU_KNAME_G, _
                                                                            CLng(dblTESUU_KIN1_S + dblTESUU_KIN2_S), _
                                                                            strTESUUTYO_SIT_T, _
                                                                            strTESUUTYO_KAMOKU_T, _
                                                                            strTESUUTYO_KOUZA_T)


                                        sSortData = strGAKKOU_CODE_S & _
                                                    "8" & _
                                                    strTESUUTYOKIN_NO_T & _
                                                    strTESUUTYO_SIT_T & _
                                                    CInt(strTESUUTYO_KAMOKU_T) & _
                                                    strTESUUTYO_KOUZA_T

                                        sLine_Data = sSortData & sLine_Data

                                        Select Case (Str_Function_Ret)
                                            Case "OK"
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                lTesuuKano_Count += 1
                                                lCreateRecord_Count += 1
                                            Case Else
                                                'Goto �������݃G���[
                                                GoTo ERROR_RIENT_WRITE
                                        End Select
                                    Case Else
                                        'Goto �}�X�^�ݒ�G���[
                                        'sErrorMessage = "���Ϗ��Ɍ�肪����܂�(�萔�������敪)" & vbCrLf

                                        'GoTo ERROR_MAST_SET
                                End Select
                            End If

                            '�������U���萔��������
                            If CLng(dblFURIKOMI_TESUU_ALL) > 0 Then  '�U���萔��
                                ''�U���萔���X�V 2006/04/17
                                sLine_Data = PFUNC_Syokanjo_Nyuukin_FURIKOMI(Trim(strFURI_CHK_GAKKOU), _
                                                                    Trim(strGAKKOU_NAME), _
                                                                    CLng(dblFURIKOMI_TESUU_ALL), _
                                                                    Trim(strTESUU_SIT), _
                                                                    Trim(strTESUU_KAMOKU), _
                                                                    Trim(strTESUU_KOUZA))

                                sSortData = Trim(sEsc_Gakkou_Code2) & _
                                            "8" & _
                                            Trim(strTESUU_KIN) & _
                                            Trim(strTESUU_SIT) & _
                                            CInt(Trim(strTESUU_KAMOKU)) & _
                                            Trim(strTESUU_KOUZA)

                                sLine_Data = sSortData & sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"
                                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                                        lTesuuKano_Count += 1
                                        lCreateRecord_Count += 1
                                    Case Else
                                        'Goto �������݃G���[
                                        GoTo ERROR_RIENT_WRITE
                                End Select
                            End If '�U���萔���d���쐬


                            '������
                        End If

                    End If

                    dblTESUU_KIN1_S = 0
                    dblTESUU_KIN2_S = 0
                    dblTESUU_KIN3_S = 0
                    intTESUUTYO_KBN_T = 0
                    strGAKKOU_CODE_S = ""
                    strGAKKOU_KNAME_G = ""
                    strTESUUTYOKIN_NO_T = ""
                    strTESUUTYO_SIT_T = ""
                    strTESUUTYO_KAMOKU_T = ""
                    strTESUUTYO_KOUZA_T = ""


                    dblFURIKOMI_TESUU_ALL = 0 '�U���萔�������� 2006/04/15 
                    strGAKKOU_NAME = ""
                    strTESUU_KIN = ""
                    strTESUU_SIT = ""
                    strTESUU_KAMOKU = ""
                    strTESUU_KOUZA = ""

                    strFURI_CHK_GAKKOU = .Item("GAKKOU_CODE_S")
                    strGAKKOU_NAME = .Item("GAKKOU_KNAME_G")
                    strTESUU_KIN = .Item("TESUUTYOKIN_NO_T")
                    strTESUU_SIT = .Item("TESUUTYO_SIT_T")
                    strTESUU_KAMOKU = .Item("TESUUTYO_KAMOKU_T  ")
                    strTESUU_KOUZA = .Item("TESUUTYO_KOUZA_T ")

                    dblNYUKIN_GAK = 0 '������

                    'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then '�����ǉ� 2006/08/11
                    If .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                        If .Item("FURI_KBN_S") <> "2" Then '���U�E�ĐU�E�Վ��o��
                            sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                            Trim(.Item("GAKKOU_KNAME_G")), _
                                                            Trim(.Item("HONBU_KOUZA_T")), _
                                                            CDbl(Trim(.Item("FURI_KIN_S"))))

                            dblNYUKIN_GAK = CDbl(Trim(.Item("FURI_KIN_S")))

                        Else '���U������
                            sLine_Data = PFUNC_Betudan_Nyukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                               Trim(.Item("GAKKOU_KNAME_G")), _
                                                               Trim(.Item("KESSAI_TENPO_H")), _
                                                               Trim(.Item("HONBU_KOUZA_T")), _
                                                               CDbl(Trim(.Item("SYORI_KIN_S"))))

                            dblNYUKIN_GAK = CDbl(Trim(.Item("SYORI_KIN_S")))
                        End If

                        lngSYORIKEN_TOKUBETU = .Item("SYORI_KEN_S")
                        dblSYORIKIN_TOKUBETU = .Item("SYORI_KIN_S")
                        lngFURIKEN_TOKUBETU = .Item("FURI_KEN_S")
                        dblFURIKIN_TOKUBETU = .Item("FURI_KIN_S")
                        lngFUNOUKEN_TOKUBETU = .Item("FUNOU_KEN_S")
                        dblFUNOUKIN_TOKUBETU = .Item("FUNOU_KIN_S")
                        dblTESUU_A1_TOKUBETU = .Item("TESUU_KIN1_S")
                        dblTESUU_A2_TOKUBETU = .Item("TESUU_KIN2_S")

                    Else
                        '�����ɕ����ꂽG_SCHMAST�̏������E���A�ό��E���A�s���E���̍��v���J�E���g 2006/04/17
                        If fn_COUNT_G_SCHMAST(Trim(.Item("GAKKOU_CODE_S"))) = False Then
                            'Goto �������݃G���[
                            GoTo ERROR_RIENT_WRITE
                        End If
                        sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                           Trim(.Item("GAKKOU_KNAME_G")), _
                                                           Trim(.Item("HONBU_KOUZA_T")), _
                                                           dblFURIKIN_TOKUBETU)

                        dblNYUKIN_GAK = dblFURIKIN_TOKUBETU
                    End If

                    sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                 "1" & _
                                 STR_JIKINKO_CODE & _
                                 Trim(.Item("TSIT_NO_T")) & _
                                 "0" & _
                                 Trim(.Item("HONBU_KOUZA_T"))

                    sLine_Data = sSortData & sLine_Data

                    Select Case (Str_Function_Ret)
                        Case "OK"
                            Call PSUB_Put_Data(iFileNo, sLine_Data)
                            If .Item("FURI_KBN_S") <> "2" Then '���U�E�ĐU�E�Վ��o��
                                lBetudan_Count += 1
                            Else
                                lBetudan_Nyu_Count += 1
                            End If
                            lCreateRecord_Count += 1
                        Case "ZERO"
                            '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                            'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                            If .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                                If .Item("TESUU_KIN_S") > 0 Then
                                    lTesuuFuka_Count += 1
                                    lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                End If
                            Else
                                If dblTESUU_TOKUBETU > 0 Then
                                    lTesuuFuka_Count += 1
                                    lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                End If
                            End If

                            'Goto �X�P�W���[���̍X�V
                            GoTo UPD_SCHEDULE
                        Case Else
                            'Goto �������݃G���[
                            GoTo ERROR_RIENT_WRITE
                    End Select
                Else '�w�Z�R�[�h������ŐU�֋敪���قȂ鎞
                    If sEsc_Furi_Kbn <> Trim(.Item("FURI_KBN_S")) Then
                        sEsc_Gakkou_Code = Trim(.Item("GAKKOU_CODE_S"))
                        sEsc_Furi_Kbn = Trim(.Item("FURI_KBN_S")) '��r�U�֋敪

                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                        If .Item("SCH_KBN_S") = "2" Then   '�����ǉ� 2006/08/11
                            If .Item("FURI_KBN_S") <> "2" Then '���U�E�ĐU�E�Վ��o��

                                '�U�֕�����
                                sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                    Trim(.Item("GAKKOU_KNAME_G")), _
                                                                    Trim(.Item("HONBU_KOUZA_T")), _
                                                                    CDbl(Trim(.Item("FURI_KIN_S"))))
                                dblNYUKIN_GAK = CDbl(Trim(.Item("FURI_KIN_S")))
                            Else '���U������
                                sLine_Data = PFUNC_Betudan_Nyukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                   Trim(.Item("GAKKOU_KNAME_G")), _
                                                                   Trim(.Item("KESSAI_TENPO_H")), _
                                                                   Trim(.Item("HONBU_KOUZA_T")), _
                                                                   CDbl(Trim(.Item("SYORI_KIN_S"))))
                                dblNYUKIN_GAK = CDbl(Trim(.Item("SYORI_KIN_S")))
                            End If

                            lngSYORIKEN_TOKUBETU = .Item("SYORI_KEN_S")
                            dblSYORIKIN_TOKUBETU = .Item("SYORI_KIN_S")
                            lngFURIKEN_TOKUBETU = .Item("FURI_KEN_S")
                            dblFURIKIN_TOKUBETU = .Item("FURI_KIN_S")
                            lngFUNOUKEN_TOKUBETU = .Item("FUNOU_KEN_S")
                            dblFUNOUKIN_TOKUBETU = .Item("FUNOU_KIN_S")


                        Else
                            '�����ɕ����ꂽG_SCHMAST�̏������E���A�ό��E���A�s���E���̍��v���J�E���g 2006/04/17
                            If fn_COUNT_G_SCHMAST(Trim(.Item("GAKKOU_CODE_S"))) = False Then
                                'Goto �������݃G���[
                                GoTo ERROR_RIENT_WRITE
                            End If
                            sLine_Data = PFUNC_Betudan_Syukkin(Trim(.Item("GAKKOU_CODE_S")), _
                                                               Trim(.Item("GAKKOU_KNAME_G")), _
                                                               Trim(.Item("HONBU_KOUZA_T")), _
                                                               dblFURIKIN_TOKUBETU)
                            dblNYUKIN_GAK = dblFURIKIN_TOKUBETU
                        End If


                        sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                     "1" & _
                                     STR_JIKINKO_CODE & _
                                     Trim(.Item("TSIT_NO_T")) & _
                                     "0" & _
                                     Trim(.Item("HONBU_KOUZA_T"))

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"
                                Call PSUB_Put_Data(iFileNo, sLine_Data)
                                If .Item("FURI_KBN_S") <> "2" Then '���U�E�ĐU�E�Վ��o��
                                    lBetudan_Count += 1
                                Else
                                    lBetudan_Nyu_Count += 1
                                End If
                                lCreateRecord_Count += 1
                            Case "ZERO"
                                '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                                If .Item("SCH_KBN_S") = "2" Then   '�����ǉ� 2006/08/11
                                    If .Item("TESUU_KIN_S") > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                    End If
                                Else
                                    If dblTESUU_TOKUBETU > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                    End If
                                End If

                                'Goto �X�P�W���[���̍X�V
                                GoTo UPD_SCHEDULE
                            Case Else
                                'Goto �������݃G���[
                                GoTo ERROR_RIENT_WRITE
                        End Select
                    End If
                End If 'END �ʒi�o���d���쐬����

                If IsDBNull(.Item("KESSAI_MEIGI_H")) = False Then
                    Str_Meigi = Trim(.Item("KESSAI_MEIGI_H"))
                Else
                    Str_Meigi = ""
                End If

                If IsDBNull(.Item("DENPYO_BIKOU1_T")) = False Then
                    Str_Biko1 = Trim(.Item("DENPYO_BIKOU1_T"))
                Else
                    Str_Biko1 = ""
                End If

                If IsDBNull(.Item("DENPYO_BIKOU2_T")) = False Then
                    Str_Biko2 = Trim(.Item("DENPYO_BIKOU2_T"))
                Else
                    Str_Biko2 = ""
                End If

                Call PSUB_GET_GINKONAME(.Item("KESSAI_KIN_CODE_H"), .Item("KESSAI_TENPO_H"))

                Str_Prt_KESSAI_KIN_KNAME = Str_Ginko(0)
                Str_Prt_KESSAI_KIN_NNAME = Str_Ginko(2)
                Str_Prt_KESSAI_SIT_KNAME = Str_Ginko(1)
                Str_Prt_KESSAI_SIT_NNAME = Str_Ginko(3)

                Str_Prt_GAKKOU_CODE = .Item("GAKKOU_CODE_S")
                Str_Prt_GAKKOU_KNAME = .Item("GAKKOU_KNAME_G")
                Str_Prt_GAKKOU_NNAME = .Item("GAKKOU_NNAME_G")

                Str_Prt_HONBU_KOUZA = .Item("HONBU_KOUZA_T")

                Str_Prt_KESSAI_DATE = .Item("KESSAI_YDATE_S")
                '�U�֓� 2006/04/18
                Str_Prt_FURI_DATE = .Item("FURI_DATE_S")

                '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                If .Item("SCH_KBN_S") = "2" Then   '�����ǉ� 2006/08/11
                    Str_Prt_TESUURYO_JIFURI = .Item("TESUU_KIN1_S")
                    Str_Prt_TESUURYO_FURI = .Item("TESUU_KIN3_S") '�U���萔��
                    Str_Prt_TESUURYO_ETC = .Item("TESUU_KIN2_S") '�X����
                Else
                    Str_Prt_TESUURYO_JIFURI = dblTESUU_A1_TOKUBETU
                    Str_Prt_TESUURYO_FURI = dblTESUU_A3_TOKUBETU '�U���萔��
                    Str_Prt_TESUURYO_ETC = dblTESUU_A2_TOKUBETU '�X����
                End If
                Str_Prt_JIFURI_KOUZA = Str_Tesuuryo_Kouza
                Str_Prt_FURI_KOUZA = Str_Tesuuryo_Kouza3 '�U���萔��
                Str_Prt_ETC_KOUZA = Str_Tesuuryo_Kouza2 '�X����

                Str_Prt_HIMOKU_NNAME = .Item("HIMOKU_NAME_H")

                '�萔�������敪���߂� 2005/06/20
                'Call PSUB_GET_TESUUTYO_KBN_T(Str_Prt_GAKKOU_CODE)

                Str_Prt_KESSAI_KIN_CODE = .Item("KESSAI_KIN_CODE_H")
                Str_Prt_KESSAI_TENPO = .Item("KESSAI_TENPO_H")
                Str_Prt_KESSAI_KAMOKU = CInt(Trim(.Item("KESSAI_KAMOKU_H")))
                Str_Prt_KESSAI_KOUZA = .Item("KESSAI_KOUZA_H")

                Str_Prt_FURI_KBN = CStr(.Item("FURI_KBN_H")) '�U�֋敪�ǉ� 2005/06/28

                'G_SCHMAST�œ��o���敪�������̏ꍇ�͒��[�ɂ͏������z�E������\��
                If CStr(.Item("FURI_KBN_S")) = "2" Then '����
                    Str_Prt_NYUKIN_KINGAKU = CLng(.Item("HIMOKU_KINGAKU_H"))
                    '���U�������̓d���쐬��A�X�P�W���[���X�V�������s��
                    '�ב֐������X�P�W���[���X�V
                    '�ב֐���
                    Str_Prt_KESSAI_PATERN = 5

                    sLine_Data = PFUNC_Kawase_Seikyu(Trim(.Item("GAKKOU_CODE_S")), _
                                                    Trim(.Item("GAKKOU_KNAME_G")), _
                                                    Trim(.Item("KESSAI_KIN_CODE_H")), _
                                                    Trim(.Item("KESSAI_TENPO_H")), _
                                                    Str_Prt_NYUKIN_KINGAKU, _
                                                    Trim(Str_Biko1), _
                                                    Trim(Str_Biko2))

                    sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                "2" & _
                                Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                Trim(.Item("KESSAI_TENPO_H")) & _
                                CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                Trim(.Item("KESSAI_KOUZA_H"))

                    Str_Prt_KESSAI_DATA = sLine_Data

                    sLine_Data = sSortData & sLine_Data

                    sLine_Data = sLine_Data

                    Select Case (Str_Function_Ret)
                        Case "OK"
                            If fn_select_G_PRTKESSAI() = False Then '�o�^�`�F�b�N 2006/04/17
                                GoTo ERROR_RIENT_WRITE
                            Else
                                If lngG_PRTKESSAI_REC = 0 Then '�o�^����Ă��Ȃ������� 2006/04/17
                                    Call PSUB_Put_Data(iFileNo, sLine_Data)

                                    '�ב֐U���ȊO�͎萔��0�~�Œ� 2006/04/15
                                    Str_Prt_TESUURYO_FURI = 0

                                    '���ϒ��[�f�[�^�o�^
                                    If PFUNC_InsertPrtKessai() = False Then
                                        GoTo ERROR_RIENT_WRITE
                                    End If

                                    lKawase_Seikyu_Count += 1
                                    lCreateRecord_Count += 1

                                End If
                            End If

                        Case "ZERO"
                            lKawase_Seikyu_Count += 1
                            lCreateRecord_Count += 1

                            'lTesuuFuka_Count += 1
                            'lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                        Case Else
                            'Goto �������݃G���[
                            GoTo ERROR_RIENT_WRITE
                    End Select


                    GoTo UPD_SCHEDULE '���U�����̓d���͕ʒi����&�ב֐����̂�
                Else '�����ȊO
                    '�U�֍ς݋��z�œ�������f�[�^���쐬����
                    Str_Prt_NYUKIN_KINGAKU = CLng(.Item("HIMOKU_FURI_KIN_H"))
                End If

                '��ڂɋ��Z�@�ւ��ݒ肳��Ă�����݂̂̂������ΏۂƂ���
                If Trim(.Item("KESSAI_KIN_CODE_H")) <> "" Then
                    '��ڂɐݒ肳��Ă�����Z�@�ւ����s���ǂ����Ō��ϋ敪�𔻒肷��
                    If Trim(.Item("KESSAI_KIN_CODE_H")) = STR_JIKINKO_CODE Then
                        Select Case (CInt(Trim(.Item("KESSAI_KAMOKU_H"))))
                            Case 1
                                '����

                                Str_Prt_KESSAI_PATERN = 1

                                sLine_Data = PFUNC_Futuu_Nyuukin(Trim(.Item("GAKKOU_CODE_S")), _
                                                                Trim(.Item("GAKKOU_KNAME_G")), _
                                                                Trim(.Item("KESSAI_KOUZA_H")), _
                                                                Str_Prt_NYUKIN_KINGAKU, _
                                                                Trim(.Item("KESSAI_TENPO_H")), _
                                                                Trim(Str_Meigi), _
                                                                Trim(.Item("FURI_DATE_S")), _
                                                                Trim(.Item("TESUUTYO_PATN_T")))


                                sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                            "2" & _
                                            Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                            Trim(.Item("KESSAI_TENPO_H")) & _
                                            CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                            Trim(.Item("KESSAI_KOUZA_H"))

                                Str_Prt_KESSAI_DATA = sLine_Data

                                sLine_Data = sSortData & sLine_Data

                                sLine_Data = sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"

                                        If fn_select_G_PRTKESSAI() = False Then '�o�^�`�F�b�N 2006/04/17
                                            GoTo ERROR_RIENT_WRITE
                                        Else
                                            If lngG_PRTKESSAI_REC = 0 Then '�o�^����Ă��Ȃ������� 2006/04/17
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                '�ב֐U���ȊO�͎萔��0�~�Œ� 2006/04/15
                                                Str_Prt_TESUURYO_FURI = 0

                                                '���ϒ��[�f�[�^�o�^
                                                If PFUNC_InsertPrtKessai() = False Then
                                                    GoTo ERROR_RIENT_WRITE
                                                End If

                                                lFutuu_Count += 1
                                                lCreateRecord_Count += 1

                                            End If
                                        End If

                                    Case "ZERO"
                                        lFutuu_Count += 1
                                        lCreateRecord_Count += 1

                                        '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                                        If .Item("SCH_KBN_S") = "2" Then   '�����ǉ� 2006/08/11
                                            If .Item("TESUU_KIN_S") > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                            End If
                                        Else
                                            If dblTESUU_TOKUBETU > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                            End If
                                        End If

                                    Case Else
                                        'Goto �������݃G���[
                                        GoTo ERROR_RIENT_WRITE
                                End Select


                            Case 2
                                '����
                                Str_Prt_KESSAI_PATERN = 2

                                sLine_Data = PFUNC_Touza_Nyuukin(Trim(.Item("GAKKOU_CODE_S")), _
                                          Trim(.Item("GAKKOU_KNAME_G")), _
                                          Trim(.Item("KESSAI_KOUZA_H")), _
                                          Str_Prt_NYUKIN_KINGAKU, _
                                          Trim(.Item("KESSAI_TENPO_H")), _
                                          Trim(Str_Meigi), _
                                          Trim(.Item("FURI_DATE_S")), _
                                          Trim(.Item("TESUUTYO_PATN_T")))

                                sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                            "2" & _
                                            Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                            Trim(.Item("KESSAI_TENPO_H")) & _
                                            CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                            Trim(.Item("KESSAI_KOUZA_H"))

                                Str_Prt_KESSAI_DATA = sLine_Data

                                sLine_Data = sSortData & sLine_Data

                                Select Case (Str_Function_Ret)
                                    Case "OK"

                                        If fn_select_G_PRTKESSAI() = False Then '�o�^�`�F�b�N 2006/04/17
                                            GoTo ERROR_RIENT_WRITE
                                        Else
                                            If lngG_PRTKESSAI_REC = 0 Then '�o�^����Ă��Ȃ������� 2006/04/17
                                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                                '�ב֐U���ȊO�͎萔��0�~�Œ� 2006/04/15
                                                Str_Prt_TESUURYO_FURI = 0

                                                '���ϒ��[�f�[�^�o�^
                                                If PFUNC_InsertPrtKessai() = False Then
                                                    GoTo ERROR_RIENT_WRITE
                                                End If

                                                lTouza_Count += 1
                                                lCreateRecord_Count += 1

                                            End If
                                        End If


                                    Case "ZERO"
                                        lTouza_Count += 1
                                        lCreateRecord_Count += 1

                                        '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then '�����ǉ� 2006/08/11
                                        If .Item("SCH_KBN_S") = "2" Then '�����ǉ� 2006/08/11
                                            If .Item("TESUU_KIN_S") > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                            End If
                                        Else
                                            If dblTESUU_TOKUBETU > 0 Then
                                                lTesuuFuka_Count += 1
                                                lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                            End If
                                        End If
                                    Case Else
                                        'Goto �������݃G���[
                                        GoTo ERROR_RIENT_WRITE
                                End Select
                        End Select
                    Else

                        '�ב֐U��
                        Str_Prt_KESSAI_PATERN = 3

                        '���ό����ԍ��ǉ� 2006/04/24
                        sLine_Data = PFUNC_Kawase_FuriKomi(Trim(.Item("GAKKOU_CODE_S")), _
                                                            Trim(.Item("GAKKOU_KNAME_G")), _
                                                            Trim(.Item("KESSAI_KIN_CODE_H")), _
                                                            Trim(.Item("KESSAI_TENPO_H")), _
                                                            Trim(.Item("KESSAI_KAMOKU_H")), _
                                                            Trim(.Item("KESSAI_KOUZA_H")), _
                                                            Trim(Str_Meigi), _
                                                            Str_Prt_NYUKIN_KINGAKU, _
                                                            Trim(Str_Biko1), _
                                                            Trim(Str_Biko2))

                        sSortData = Trim(.Item("GAKKOU_CODE_S")) & _
                                    "2" & _
                                    Trim(.Item("KESSAI_KIN_CODE_H")) & _
                                    Trim(.Item("KESSAI_TENPO_H")) & _
                                    CInt(Trim(.Item("KESSAI_KAMOKU_H"))) & _
                                    Trim(.Item("KESSAI_KOUZA_H"))

                        Str_Prt_KESSAI_DATA = sLine_Data

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"

                                If fn_select_G_PRTKESSAI() = False Then '�o�^�`�F�b�N 2006/04/17
                                    GoTo ERROR_RIENT_WRITE
                                Else
                                    If lngG_PRTKESSAI_REC = 0 Then '�o�^����Ă��Ȃ������� 2006/04/17
                                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                                        '�ב֐U���̎��͐U�荞�ݎ萔���v�Z 2006/04/15
                                        Str_Prt_TESUURYO_FURI = 0 '������

                                        'iTableID=0(��l�����ݒ�ł��Ȃ��̂�)�Œ�
                                        Str_Prt_TESUURYO_FURI = PFUNC_Get_FuriTesuryo(STR_JIKINKO_CODE, Str_Prt_NYUKIN_KINGAKU, .Item("FURI_KEN_S"), 0)
                                        dblFURIKOMI_TESUU_ALL = dblFURIKOMI_TESUU_ALL + CDbl(Str_Prt_TESUURYO_FURI)

                                        '���ϒ��[�f�[�^�o�^
                                        If PFUNC_InsertPrtKessai() = False Then
                                            GoTo ERROR_RIENT_WRITE
                                        End If

                                        lKawase_Count += 1
                                        lCreateRecord_Count += 1
                                    End If
                                End If

                            Case "ZERO"
                                lKawase_Count += 1
                                lCreateRecord_Count += 1

                                '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                                'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                                If .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                                    If .Item("TESUU_KIN_S") > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += .Item("TESUU_KIN_S")
                                    End If
                                Else
                                    If dblTESUU_TOKUBETU > 0 Then
                                        lTesuuFuka_Count += 1
                                        lTesuuFuka_Kingaku += dblTESUU_TOKUBETU
                                    End If
                                End If

                            Case Else
                                'Goto �������݃G���[
                                GoTo ERROR_RIENT_WRITE
                        End Select

                    End If
                End If


                '������A�������i�U�֎萔���{�X����)
                If Trim(sEsc_Gakkou_Code2) <> Trim(.Item("GAKKOU_CODE_S")) Then

                    sEsc_Gakkou_Code2 = Trim(.Item("GAKKOU_CODE_S"))
                    sEsc_Furi_Kbn2 = Trim(.Item("FURI_KBN_S")) '��r�U�֋敪

                    '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                    'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                    If .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                        If CLng(.Item("TESUU_KIN1_S")) + CLng(.Item("TESUU_KIN2_S")) > 0 Then '�萔��+�X����
                            '���[�o�͗p�萔���Q�̍��v 2006/04/18
                            lngTESUU2 = lngTESUU2 + CLng(.Item("TESUU_KIN2_S"))

                            dblTESUU_KIN1_S = CDbl(.Item("TESUU_KIN1_S"))
                            dblTESUU_KIN2_S = CDbl(.Item("TESUU_KIN2_S"))
                        Else
                            dblTESUU_KIN1_S = 0
                            dblTESUU_KIN2_S = 0
                        End If
                    Else
                        If CLng(dblTESUU_A1_TOKUBETU) + CLng(dblTESUU_A2_TOKUBETU) > 0 Then '�萔��+�X����
                            '���[�o�͗p�萔���Q�̍��v 2006/04/18
                            lngTESUU2 = lngTESUU2 + CLng(dblTESUU_A2_TOKUBETU)

                            dblTESUU_KIN1_S = CDbl(dblTESUU_A1_TOKUBETU)
                            dblTESUU_KIN2_S = CDbl(dblTESUU_A2_TOKUBETU)
                        Else
                            dblTESUU_KIN1_S = 0
                            dblTESUU_KIN2_S = 0
                        End If
                    End If

                    intTESUUTYO_KBN_T = CInt(.Item("TESUUTYO_KBN_T"))
                    strGAKKOU_CODE_S = Trim(.Item("GAKKOU_CODE_S"))
                    strGAKKOU_KNAME_G = Trim(.Item("GAKKOU_KNAME_G"))
                    strTESUUTYOKIN_NO_T = Trim(.Item("TESUUTYOKIN_NO_T"))
                    strTESUUTYO_SIT_T = Trim(.Item("TESUUTYO_SIT_T"))
                    strTESUUTYO_KAMOKU_T = Trim(.Item("TESUUTYO_KAMOKU_T  "))
                    strTESUUTYO_KOUZA_T = Trim(.Item("TESUUTYO_KOUZA_T "))

                Else '�w�Z�R�[�h������ŐU�֋敪���قȂ鎞
                    If sEsc_Furi_Kbn2 <> Trim(.Item("FURI_KBN_S")) Then
                        sEsc_Gakkou_Code2 = Trim(.Item("GAKKOU_CODE_S"))
                        sEsc_Furi_Kbn2 = Trim(.Item("FURI_KBN_S")) '��r�U�֋敪

                        '���ʃX�P�W���[���̏ꍇ�͍��v�l���Z�b�g 2006/04/17
                        'If .Item("SCH_KBN_S") = "0" OR .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                        If .Item("SCH_KBN_S") = "2" Then  '�����ǉ� 2006/08/11
                            If CLng(.Item("TESUU_KIN1_S")) + CLng(.Item("TESUU_KIN2_S")) > 0 Then '�萔��+�X����
                                '���[�o�͗p�萔���Q�̍��v 2006/04/18
                                lngTESUU2 = lngTESUU2 + CLng(.Item("TESUU_KIN2_S"))

                                dblTESUU_KIN1_S = CDbl(.Item("TESUU_KIN1_S"))
                                dblTESUU_KIN2_S = CDbl(.Item("TESUU_KIN2_S"))
                            Else
                                dblTESUU_KIN1_S = 0
                                dblTESUU_KIN2_S = 0
                            End If
                        Else
                            If CLng(dblTESUU_A1_TOKUBETU) + CLng(dblTESUU_A2_TOKUBETU) > 0 Then '�萔��+�X����
                                '���[�o�͗p�萔���Q�̍��v 2006/04/18
                                lngTESUU2 = lngTESUU2 + CLng(dblTESUU_A2_TOKUBETU)

                                dblTESUU_KIN1_S = CDbl(dblTESUU_A1_TOKUBETU)
                                dblTESUU_KIN2_S = CDbl(dblTESUU_A2_TOKUBETU)
                            Else
                                dblTESUU_KIN1_S = 0
                                dblTESUU_KIN2_S = 0
                            End If
                        End If

                        intTESUUTYO_KBN_T = CInt(.Item("TESUUTYO_KBN_T"))
                        strGAKKOU_CODE_S = Trim(.Item("GAKKOU_CODE_S"))
                        strGAKKOU_KNAME_G = Trim(.Item("GAKKOU_KNAME_G"))
                        strTESUUTYOKIN_NO_T = Trim(.Item("TESUUTYOKIN_NO_T"))
                        strTESUUTYO_SIT_T = Trim(.Item("TESUUTYO_SIT_T"))
                        strTESUUTYO_KAMOKU_T = Trim(.Item("TESUUTYO_KAMOKU_T  "))
                        strTESUUTYO_KOUZA_T = Trim(.Item("TESUUTYO_KOUZA_T "))
                    End If

                End If 'END ������A������(�U�֎萔���{�X����)

UPD_SCHEDULE:
                '�X�P�W���[���X�V
                If PFUNC_Update_Schedule(sEsc_Gakkou_Code, Trim(.Item("FURI_DATE_S")), sSyori_Date, Trim(.Item("FURI_KBN_S"))) = False Then
                    'Goto �X�P�W���[���X�V�G���[
                    GoTo ERROR_UPD_SCHEDULE
                End If
NEXT_RECORD:
            End With
        End While


        '�U���萔���X�V 2006/04/17
        If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
            'Goto �������݃G���[
            GoTo ERROR_RIENT_WRITE

        End If
        lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

        '�������z���萔���i�U�ցE�X���萔���{�U���萔��)�������ꍇ�͎萔���͈����Ȃ�
        If dblNYUKIN_GAK < (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL) Then
            lngTESUU_ALL = lngTESUU_ALL - (dblTESUU_KIN1_S + dblTESUU_KIN2_S + dblFURIKOMI_TESUU_ALL)
        Else

            '�������U�ցE�X��������ѐU���萔��������(��O�̊w�Z�R�[�h�p�d��)
            If dblTESUU_KIN1_S + dblTESUU_KIN2_S > 0 Then '�萔��+�X����
                Select Case (intTESUUTYO_KBN_T)
                    Case 0
                        sLine_Data = PFUNC_Syokanjo_Nyuukin(strGAKKOU_CODE_S, _
                                                            strGAKKOU_KNAME_G, _
                                                            CLng(dblTESUU_KIN1_S + dblTESUU_KIN2_S), _
                                                            strTESUUTYO_SIT_T, _
                                                            strTESUUTYO_KAMOKU_T, _
                                                            strTESUUTYO_KOUZA_T)


                        sSortData = strGAKKOU_CODE_S & _
                                    "8" & _
                                    strTESUUTYOKIN_NO_T & _
                                    strTESUUTYO_SIT_T & _
                                    CInt(strTESUUTYO_KAMOKU_T) & _
                                    strTESUUTYO_KOUZA_T

                        sLine_Data = sSortData & sLine_Data

                        Select Case (Str_Function_Ret)
                            Case "OK"
                                Call PSUB_Put_Data(iFileNo, sLine_Data)

                                lTesuuKano_Count += 1
                                lCreateRecord_Count += 1
                            Case Else
                                'Goto �������݃G���[
                                GoTo ERROR_RIENT_WRITE
                        End Select
                    Case Else
                        'Goto �}�X�^�ݒ�G���[
                        'sErrorMessage = "���Ϗ��Ɍ�肪����܂�(�萔�������敪)" & vbCrLf

                        'GoTo ERROR_MAST_SET
                End Select
            End If


            '�U�荞�ݎ萔�����̓d���@�e�w�Z�ɂP�d���쐬
            If CLng(dblFURIKOMI_TESUU_ALL) > 0 Then '�U���萔��

                ''�U���萔���X�V 2006/04/17
                'If PFUNC_UPDATE_PrtKessai(Trim(sEsc_Gakkou_Code2), CLng(dblFURIKOMI_TESUU_ALL)) = False Then
                '    'Goto �������݃G���[
                '    GoTo ERROR_RIENT_WRITE

                'End If

                'lngTESUU3 = lngTESUU3 + CLng(dblFURIKOMI_TESUU_ALL)

                sLine_Data = PFUNC_Syokanjo_Nyuukin_FURIKOMI(Trim(sEsc_Gakkou_Code2), _
                                                    Trim(strGAKKOU_NAME), _
                                                    CLng(dblFURIKOMI_TESUU_ALL), _
                                                    Trim(strTESUU_SIT), _
                                                    Trim(strTESUU_KAMOKU), _
                                                    Trim(strTESUU_KOUZA))

                sSortData = Trim(sEsc_Gakkou_Code2) & _
                            "8" & _
                            Trim(strTESUU_KIN) & _
                            Trim(strTESUU_SIT) & _
                            CInt(Trim(strTESUU_KAMOKU)) & _
                            Trim(strTESUU_KOUZA)

                sLine_Data = sSortData & sLine_Data

                Select Case (Str_Function_Ret)
                    Case "OK"
                        Call PSUB_Put_Data(iFileNo, sLine_Data)

                        lTesuuKano_Count += 1
                        lCreateRecord_Count += 1
                    Case Else
                        'Goto �������݃G���[
                        GoTo ERROR_RIENT_WRITE
                End Select
            End If '�U���萔���d���쐬
            '������
        End If

        '���σX�P�W���[�����`�F�b�N
        If lngKESSAIREC = 0 Then
            Call GSUB_MESSAGE_WARNING("��������f�[�^������܂���")
            GoTo ERROR_HANDLER
        End If

        '�J�E���g�`�F�b�N
        If lRecord_Count = 0 Then
            Call GSUB_MESSAGE_WARNING("��������f�[�^������܂���")
            GoTo ERROR_HANDLER
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        FileClose(iFileNo)

        '�����v�����E���z�����擾
        If fn_select_G_SCHMAST() = False Then
            Exit Function
        Else
            '�萔�������v�Z�o 2006/04/18
            lngTESUU_ALL = lngTESUU_ALL + (lngTESUU1 + lngTESUU2 + lngTESUU3)
        End If

        Dim strDIR As String
        strDIR = CurDir()

        'Dim CRXApplication As New CRAXDDRT.Application
        'Dim CRXReport As CRAXDDRT.Report
        'Dim CPProperty As CRAXDDRT.ConnectionProperty
        'Dim DBTable As CRAXDDRT.DatabaseTable

        'CRXReport = CRXApplication.OpenReport(STR_LST_PATH & "�������ϊw�Z�ꗗ�\.RPT", 1)

        'DBTable = CRXReport.Database.Tables(1)

        'CPProperty = DBTable.ConnectionProperties("Password")
        'CPProperty.Value = "KZFMAST"
        'CRXReport.RecordSelectionFormula = ""

        'Dim CRX_FORMULA As CRAXDDRT.FormulaFieldDefinition
        'Dim strFORMULA_NAME As String
        'For i As Integer = 1 To CRXReport.FormulaFields.Count
        '    CRX_FORMULA = CRXReport.FormulaFields.Item(i)
        '    strFORMULA_NAME = CRX_FORMULA.FormulaFieldName
        '    Select Case (strFORMULA_NAME)
        '        Case "�����v����"
        '            CRX_FORMULA.Text = CStr(lngALL_KEN)
        '        Case "�����v���z"
        '            CRX_FORMULA.Text = CStr(lngALL_KIN)
        '        Case "���U�֌���"
        '            CRX_FORMULA.Text = CStr(lngFURI_ALL_KEN)
        '        Case "���U�֋��z"
        '            CRX_FORMULA.Text = CStr(lngFURI_ALL_KIN)
        '        Case "���s�\����"
        '            CRX_FORMULA.Text = CStr(lngFUNOU_ALL_KEN)
        '        Case "���s�\���z"
        '            CRX_FORMULA.Text = CStr(lngFUNOU_ALL_KIN)
        '        Case "���萔�����v"
        '            CRX_FORMULA.Text = CStr(lngTESUU_ALL)
        '        Case "���萔��1"
        '            CRX_FORMULA.Text = CStr(lngTESUU1)
        '        Case "���萔��3" '�X����
        '            CRX_FORMULA.Text = CStr(lngTESUU2)
        '        Case "���萔��2" '�U���萔��
        '            CRX_FORMULA.Text = CStr(lngTESUU3)

        '    End Select
        'Next

        'CRXReport.PrintOut(False, 1)

        ChDir(strDIR)

        PFUNC_Write_WorkD = True

        ''�������b�Z�[�W
        'Call GSUB_MESSAGE_INFOMATION("���G���^�e�c���쐬���܂���" & vbCrLf & _
        '                                 "�쐬���R�[�h����  = " & lCreateRecord_Count & vbCrLf & _
        '                                 "  �ʒi�o��          = " & lBetudan_Count & vbCrLf & _
        '                                 "  �a����             = " & "0" & vbCrLf & _
        '                                 "  ���ʓ���          = " & lFutuu_Count & vbCrLf & _
        '                                 "  ��������          = " & lTouza_Count & vbCrLf & _
        '                                 "  �ʒi����          = " & "0" & vbCrLf & _
        '                                 "  �ב֐U��          = " & lKawase_Count & vbCrLf & _
        '                                 "  �ב֕t��          = " & "0" & vbCrLf & _
        '                                 "  �ב֐���          = " & "0" & vbCrLf & _
        '                                 "  �萔������       = " & lTesuuKano_Count & vbCrLf & _
        '                                 "  �萔�������s�� = " & lTesuuFuka_Count)

        Exit Function

ERROR_RIENT_WRITE:
        sErrorMessage = "���G���^�f�[�^�������݂Ɏ��s���܂���" & vbCrLf
        sErrorMessage += " �w�Z���@�@�@�@ = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " ���ϋ敪�@�@�@ = " & Str_Err_Kessai_Kbn & vbCrLf
        sErrorMessage += " �萔���������@ = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " �萔�������敪 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " �������z�@�@�@ = " & "" & vbCrLf
        sErrorMessage += " �U�֍ϋ��z�@�@ = " & "" & vbCrLf
        sErrorMessage += " �萔�����z�@�@ = " & "" & vbCrLf
        sErrorMessage += " �G���[���@�@ = " & Err.Description

        GoTo ERROR_HANDLER
ERROR_MAST_SET:
        sErrorMessage += " �w�Z���@�@�@�@ = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " ���ϋ敪�@�@�@ = " & Str_Err_Kessai_Kbn & vbCrLf
        sErrorMessage += " �萔���������@ = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " �萔�������敪 = " & Str_Err_Tesuutyo_Kbn & vbCrLf
        sErrorMessage += " �����Ȗځ@�@�@ = " & "" & vbCrLf

        GoTo ERROR_HANDLER
ERROR_UPD_SCHEDULE:
        sErrorMessage = "�X�P�W���[���X�V�Ɏ��s���܂���" & vbCrLf
        sErrorMessage += " �w�Z���@�@�@�@ = " & Str_Err_Gakkou_Name & vbCrLf
        sErrorMessage += " �U�֓��@�@�@�@ = " & "" & vbCrLf
        sErrorMessage += " �G���[���@�@ = " & Err.Description

        GoTo ERROR_HANDLER
ERROR_HANDLER:
        If Trim(sErrorMessage) <> "" Then
            Call GSUB_MESSAGE_WARNING(sErrorMessage)
        End If

        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        FileClose(iFileNo)

    End Function
    Private Function PFUNC_Betudan_Syukkin(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHonbu_Kouza As String, _
                                           ByVal pFurikae_Kingaku As Double) As String

        Dim FD_WORK_D As String
        'Betudan_Syukkin = INDEX��
        ' 0 = �ȖڃR�[�h
        ' 1 = ����R�[�h
        ' 2 = �����ԍ�
        ' 3 = ���z
        ' 4 = �U�փR�[�h
        ' 5 = ��ƃR�[�h
        ' 6 = �E�v
        ' 7 = �戵�ԍ�1
        ' 8 = ����
        ' 9 = ��`���؎�ԍ�
        '10 = ���X�ԍ�
        '11 = �N�Z��
        '12 = �\��1
        Dim Betudan_Syukkin(12) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Betudan_Syukkin(0) = "04"
        Betudan_Syukkin(1) = "099"

        If Format(pHonbu_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("�ʒi�����ԍ����ݒ肳��Ă��܂���" & vbCrLf & "�w�Z�R�[�h = " & pGakkou_Code & vbCrLf & "�������ϗp���G���^�e�c�쐬�i�ʒi�����j")
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(2) = Format(CInt(pHonbu_Kouza), "0000000")
        Betudan_Syukkin(3) = Format(pFurikae_Kingaku, "0000000000")

        '�U�֍ϋ��z���O�~���͂����ŏ������I��
        If CLng(Betudan_Syukkin(3)) = 0 Then
            Str_Function_Ret = "ZERO"
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(4) = Space(3)

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�w�Z�R�[�h = " & pGakkou_Code & vbCrLf & "�w�Z�� = " & pGakkou_Name & vbCrLf & "�E�v������܂���")
            Return Str_Function_Ret
        End If

        Betudan_Syukkin(5) = Space(3)
        Betudan_Syukkin(6) = Mid(Trim(pGakkou_Name) & Space(16), 1, 16)
        Betudan_Syukkin(7) = Space(5)
        Betudan_Syukkin(8) = Space(4)
        Betudan_Syukkin(9) = Space(6)
        Betudan_Syukkin(10) = Space(3)
        Betudan_Syukkin(11) = Space(6)
        Betudan_Syukkin(12) = Space(212)

        For iCount = LBound(Betudan_Syukkin) To UBound(Betudan_Syukkin)
            FD_WORK_D += Betudan_Syukkin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Betudan_Syukkin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�w�Z�� : " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Return Str_Function_Ret
        End If

        Str_Function_Ret = "OK"

        Return Str_Function_Ret

    End Function
    '�ʒi���� 2005/06/28
    Private Function PFUNC_Betudan_Nyukin(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pTukekae_Sit As String, _
                                           ByVal pHonbu_Kouza As String, _
                                           ByVal pNyukin_Kingaku As Double) As String

        Dim FD_WORK_D As String
        'PFUNC_Betudan_Nyukin = INDEX��
        ' 0 = �ȖڃR�[�h*2
        ' 1 = ����R�[�h*3
        ' 2 = �����ԍ�*7
        ' 3 = ���z*10
        ' 4 = �������敪�R�[�h*1
        ' 5 = ���X���E�v*2
        ' 6 = �U�փR�[�h*3
        ' 7 = ��ƃR�[�h*3
        ' 8 = �E�v*16
        ' 9 = �戵�ԍ�1*5
        '10 = ����*4
        '11 = �������[�敪*1
        '12 = �󎆌���*3
        '13 = ��`���؎�ԍ�*6
        '14 = ���s��ڋq�ԍ�*7
        '15 = �萔�������敪*1
        '16 = �萔���z*5
        '17 = �N�Z��*6
        '18 = ����t�\���*6
        '19 = ���X�ԍ�*3
        '20 = �\��1*186
        Dim Betudan_Nyukin(20) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Betudan_Nyukin(0) = "04"
        Betudan_Nyukin(1) = "019"

        If Format(pHonbu_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("�{�������ԍ����ݒ肳��Ă��܂���" & vbCrLf & "�w�Z�R�[�h = " & pGakkou_Code & vbCrLf & "�������ϗp���G���^�e�c�쐬�i�ʒi�����j")
            Return Str_Function_Ret
        End If

        Betudan_Nyukin(2) = Format(CInt(pHonbu_Kouza), "0000000")
        Betudan_Nyukin(3) = Format(pNyukin_Kingaku, "0000000000")

        '��ƌ����������z���O�~���͂����ŏ������I��
        If CLng(Betudan_Nyukin(3)) = 0 Then
            Str_Function_Ret = "ZERO"
            Return Str_Function_Ret
        End If
        '�������敪�R�[�h
        Betudan_Nyukin(4) = Space(1)

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�w�Z�R�[�h = " & pGakkou_Code & vbCrLf & "�w�Z�� = " & pGakkou_Name & vbCrLf & "�E�v������܂���")

            Return Str_Function_Ret
        End If

        Betudan_Nyukin(5) = Space(2) '���X���E�v
        Betudan_Nyukin(6) = Space(3) '�U�փR�[�h
        Betudan_Nyukin(7) = Space(3) '��ƃR�[�h
        Betudan_Nyukin(8) = CStr(pGakkou_Name.Trim & Space(16)).Substring(0, 16) '�E�v�F�ϑ��Җ��J�i
        Betudan_Nyukin(9) = Space(5) '�戵�ԍ�1
        Betudan_Nyukin(10) = Space(4) '����
        Betudan_Nyukin(11) = Space(1) '�������[�敪
        Betudan_Nyukin(12) = Space(3) '�󎆌���
        Betudan_Nyukin(13) = Space(6) '��`���؎�ԍ�
        Betudan_Nyukin(14) = Space(7) '���s���ڋq�ԍ�
        Betudan_Nyukin(15) = Space(1) '�萔�������敪
        Betudan_Nyukin(16) = Space(5) '�萔���z
        Betudan_Nyukin(17) = Space(6) '�N�Z��
        Betudan_Nyukin(18) = Space(6) '����t�\���
        If pTukekae_Sit.Trim = "" Then
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�w�Z�R�[�h = " & pGakkou_Code & vbCrLf & "�w�Z�� = " & pGakkou_Name & vbCrLf & "���X�ԍ�������܂���")
            Return Str_Function_Ret
        Else
            If pTukekae_Sit = Str_Honbu_Code Then
                Betudan_Nyukin(19) = Space(3) '���_�ԍ�
            Else
                Betudan_Nyukin(19) = pTukekae_Sit '���_�ԍ�
            End If

        End If
        Betudan_Nyukin(20) = Space(186) '�\���P

        For iCount = LBound(Betudan_Nyukin) To UBound(Betudan_Nyukin)
            FD_WORK_D += Betudan_Nyukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Betudan_Nyukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�w�Z�� : " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Return Str_Function_Ret
        End If

        Str_Function_Ret = "OK"

        Return Str_Function_Ret

    End Function

    Private Function PFUNC_Futuu_Nyuukin(ByVal pGakkou_Code As String, _
                                         ByVal pGakkou_Name As String, _
                                         ByVal pHimoku_Kouza As String, _
                                         ByVal pHimoku_Kingaku As Long, _
                                         ByVal pHimoku_Tenpo As String, _
                                         ByVal pHimoku_Meigi As String, _
                                         ByVal pFurikae_Date As String, _
                                         ByVal pTesuuryo_kbn As String) As String
        '*********************���ʓ���********************************
        'UPDATE:2005/10/19 �V�t�H�[�}�b�g�Ή� ������pHimoku_Meigi�ǉ�
        '*************************************************************

        Dim FD_WORK_D As String
        'Futuu_Nyuukin = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = �����ԍ� * 7
        ' 3 = �s * 2
        ' 4 = ���z * 10
        ' 5 = �������敪�R�[�h * 1
        ' 6 = ���X���E�v * 2
        ' 7 = �U�փR�[�h * 3
        ' 8 = �E�v * 13
        ' 9 = �萔�������敪 * 1
        '10 = �萔���z * 5
        '11 = �N�Z�� * 6
        '12 = ����t�\��� * 6
        '13 = �U���˗��l�� * 48 2005/10/19�@�ǉ�
        '14 = ���X�ԍ� * 3
        '15 = ���z1 * 10
        '16 = �������敪�R�[�h1 * 1
        '17 = ���X���E�v1 * 2
        '18 = ���z2 * 10
        '19 = �������敪�R�[�h2 * 1
        '20 = ���X���E�v2 * 2
        '21 = ���z3 * 10
        '22 = �������敪�R�[�h3 * 1
        '23 = ���X���E�v3 * 2
        '24 = �\��1 * 129  2005/10/19�@177��129�ɕύX
        Dim Futuu_Nyuukin(24) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Futuu_Nyuukin(0) = "02"
        Futuu_Nyuukin(1) = "019"

        If Trim(pHimoku_Kouza) = "" Then
            Call GSUB_MESSAGE_WARNING("�����ԍ�������܂��� : " & Trim(pHimoku_Kouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Futuu_Nyuukin(2) = Format(CInt(pHimoku_Kouza), "0000000")
        Futuu_Nyuukin(3) = "01"

        Select Case (Trim(pTesuuryo_kbn))
            Case "0"
                Futuu_Nyuukin(4) = Format((pHimoku_Kingaku), "0000000000")

                If CLng(Futuu_Nyuukin(4)) <= 0 Then
                    Futuu_Nyuukin(4) = Format(pHimoku_Kingaku, "0000000000")
                    Str_Function_Ret = "ZERO"
                End If
            Case "1"
                Futuu_Nyuukin(4) = Format(pHimoku_Kingaku, "0000000000")
        End Select

        Futuu_Nyuukin(5) = Space(1)
        Futuu_Nyuukin(6) = Space(2)
        Futuu_Nyuukin(7) = "040"

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("�E�v������܂���" & vbCrLf & "�������ϗp���G���^�e�c�쐬�i���ʓ����j")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If
        '�E�v
        Futuu_Nyuukin(8) = Mid(Mid(pFurikae_Date, 5, 2) & "/" & Mid(pFurikae_Date, 7, 2) & Space(1) & Trim(pGakkou_Name) & Space(13), 1, 13)
        Futuu_Nyuukin(9) = Space(1)
        Futuu_Nyuukin(10) = Space(5)
        Futuu_Nyuukin(11) = Space(6)
        Futuu_Nyuukin(12) = Space(6)

        '�V�t�H�[�}�b�g�Ή� 2005/10/19
        Select Case (Futuu_Nyuukin(7))
            Case "040", "041", "044"
                If Futuu_Nyuukin(8) = "" Then
                    Futuu_Nyuukin(13) = pHimoku_Meigi
                Else
                    Futuu_Nyuukin(13) = Space(48)
                End If
            Case Else
                Futuu_Nyuukin(13) = Space(48)
        End Select

        If Trim(pHimoku_Tenpo) = "" Then
            Call GSUB_MESSAGE_WARNING("���X�ԍ�������܂���" & vbCrLf & "�������ϗp���G���^�e�c�쐬�i���ʓ����j")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Honbu_Code = pHimoku_Tenpo Then
            Futuu_Nyuukin(14) = Space(3)
        Else
            Futuu_Nyuukin(14) = Format(CInt(pHimoku_Tenpo), "000")
        End If

        Futuu_Nyuukin(15) = Space(10)
        Futuu_Nyuukin(16) = Space(1)
        Futuu_Nyuukin(17) = Space(2)
        Futuu_Nyuukin(18) = Space(10)
        Futuu_Nyuukin(19) = Space(1)
        Futuu_Nyuukin(20) = Space(2)
        Futuu_Nyuukin(21) = Space(10)
        Futuu_Nyuukin(22) = Space(1)
        Futuu_Nyuukin(23) = Space(2)
        Futuu_Nyuukin(24) = Space(129)

        For iCount = LBound(Futuu_Nyuukin) To UBound(Futuu_Nyuukin)
            FD_WORK_D += Futuu_Nyuukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Futuu_Nyuukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function
    Private Function PFUNC_Touza_Nyuukin(ByVal pGakkou_Code As String, _
                                         ByVal pGakkou_Name As String, _
                                         ByVal pHimoku_Kouza As String, _
                                         ByVal pHimoku_Kingaku As Long, _
                                         ByVal pHimoku_Tenpo As String, _
                                         ByVal pHimoku_Meigi As String, _
                                         ByVal pFurikae_Date As String, _
                                         ByVal pTesuuryo_kbn As String) As String

        '*********************��������********************************
        'UPDATE:2005/10/19 �V�t�H�[�}�b�g�Ή� ������pHimoku_Meigi�ǉ�
        '*************************************************************

        Dim FD_WORK_D As String
        'Touza_Nyuukin = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = �����ԍ� * 7
        ' 3 = ���z * 10
        ' 4 = �������敪�R�[�h * 1
        ' 5 = ���X���E�v * 2
        ' 6 = �U�փR�[�h * 3
        ' 7 = �E�v * 13
        ' 8 = �萔�������敪 * 1
        ' 9 = �萔���z * 5
        '10 = �N�Z�� * 6
        '11 = ����t�\��� * 6
        '12 = �U���˗��l�� * 48
        '13 = ���X�ԍ� * 3
        '14 = ���z1 * 10
        '15 = �������敪�R�[�h1 * 1
        '16 = ���X���E�v1 * 2
        '17 = ���z2 * 10
        '18 = �������敪�R�[�h2 * 1
        '19 = ���X���E�v2 * 2
        '20 = ���z3 * 10
        '21 = �������敪�R�[�h3 * 1
        '22 = ���X���E�v3 * 2
        '23 = �\��1 * 131
        Dim Touza_Nyuukin(23) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Touza_Nyuukin(0) = "01"
        Touza_Nyuukin(1) = "010"

        If Trim(pHimoku_Kouza) = "" Then
            Call GSUB_MESSAGE_WARNING("�����ԍ�������܂��� : " & Trim(pHimoku_Kouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Touza_Nyuukin(2) = Format(CInt(pHimoku_Kouza), "0000000")

        Select Case (Trim(pTesuuryo_kbn))
            Case "0"
                Touza_Nyuukin(3) = Format((pHimoku_Kingaku), "0000000000")

                If CLng(Touza_Nyuukin(4)) <= 0 Then
                    Touza_Nyuukin(3) = Format(pHimoku_Kingaku, "0000000000")
                    Str_Function_Ret = "ZERO"
                End If
            Case "1"
                Touza_Nyuukin(3) = Format(pHimoku_Kingaku, "0000000000")
        End Select

        Touza_Nyuukin(4) = Space(1)
        Touza_Nyuukin(5) = Space(2)
        Touza_Nyuukin(6) = "040"

        If Trim(pGakkou_Name) = "" Then
            Call GSUB_MESSAGE_WARNING("�E�v������܂���" & vbCrLf & "�������ϗp���G���^�e�c�쐬�i���ʓ����j")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        Touza_Nyuukin(7) = Mid(Mid(pFurikae_Date, 5, 2) & "/" & Mid(pFurikae_Date, 7, 2) & Space(1) & Trim(pGakkou_Name) & Space(13), 1, 13)
        Touza_Nyuukin(8) = Space(1)
        Touza_Nyuukin(9) = Space(5)
        Touza_Nyuukin(10) = Space(6)
        Touza_Nyuukin(11) = Space(6)

        '�V�t�H�[�}�b�g�Ή� 2005/10/19
        Select Case (Touza_Nyuukin(6))
            Case "040", "041", "044"
                If Touza_Nyuukin(7) = "" Then
                    Touza_Nyuukin(12) = pHimoku_Meigi
                Else
                    Touza_Nyuukin(12) = Space(48)
                End If
            Case Else
                Touza_Nyuukin(12) = Space(48)
        End Select

        If Trim(pHimoku_Tenpo) = "" Then
            Call GSUB_MESSAGE_WARNING("���X�ԍ�������܂���" & vbCrLf & "�������ϗp���G���^�e�c�쐬�i���ʓ����j")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        If Str_Honbu_Code = pHimoku_Tenpo Then
            Touza_Nyuukin(13) = Space(3)
        Else
            Touza_Nyuukin(13) = Format(CInt(pHimoku_Tenpo), "000")
        End If

        Touza_Nyuukin(14) = Space(10)
        Touza_Nyuukin(15) = Space(1)
        Touza_Nyuukin(16) = Space(2)
        Touza_Nyuukin(17) = Space(10)
        Touza_Nyuukin(18) = Space(1)
        Touza_Nyuukin(19) = Space(2)
        Touza_Nyuukin(20) = Space(10)
        Touza_Nyuukin(21) = Space(1)
        Touza_Nyuukin(22) = Space(2)
        Touza_Nyuukin(23) = Space(131)

        For iCount = LBound(Touza_Nyuukin) To UBound(Touza_Nyuukin)
            FD_WORK_D += Touza_Nyuukin(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Touza_Nyuukin = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Str_Function_Ret = "NG"

            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function
    Private Function PFUNC_Kawase_FuriKomi(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kamoku As String, _
                                           ByVal pHimoku_Kouza As String, _
                                           ByVal pHimoku_Meigi As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'TKawase_Furikae = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = ��M�X�� * 30
        ' 3 = ���M�X�� * 20
        ' 4 = �戵�� * 6
        ' 5 = ��� * 4
        ' 6 = �ڋq�萔�� * 4
        ' 7 = ���z * 12
        ' 8 = ���z���L���� * 15
        ' 9 = ���l�Ȗڌ��� * 15
        '10 = ���l�� * 29
        '11 = �˗��l�� * 29
        '12 = ���l1 * 29
        '13 = ���l2 * 29
        '14 = �\��1 * 53

        Dim Kawase_Furikae(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Furikae(0) = "48"
        Kawase_Furikae(1) = "100"

        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("���Z�@�փR�[�h�捞�G���[ : " & Trim(pHimoku_Ginko) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Furikae(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Furikae(3) = Mid(Chr(223) & Space(1) & "����" & Space(20), 1, 20)
        Else
            Kawase_Furikae(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Furikae(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If

        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Furikae(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Furikae(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
        Kawase_Furikae(5) = "�غ�"
        Kawase_Furikae(6) = Space(4)
        Kawase_Furikae(7) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Furikae(7)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("���L�����ݒ菈���G���[ : " & Trim(Kawase_Furikae(8)) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Kawase_Furikae(8) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '���ό����ԍ���ݒ� 2006/04/24
        Select Case (Format(CInt(pHimoku_Kamoku), "00"))
            Case "02" '��ڂ�02�F���� 2006/04/24
                Kawase_Furikae(9) = Mid("�" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
            Case "01" '��ڂ�01�F���� 2006/04/24
                Kawase_Furikae(9) = Mid("�" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
            Case Else
                Kawase_Furikae(9) = Mid("�" & Format(Val(pHimoku_Kouza), "0000000") & Space(15), 1, 15)
        End Select

        If Trim(pHimoku_Meigi) = "" Then
            Call GSUB_MESSAGE_WARNING("���l��������܂��� : " & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Kawase_Furikae(10) = Mid(Trim(pHimoku_Meigi) & Space(29), 1, 29)
        Kawase_Furikae(11) = Mid(Str_IraiNinName & Space(29), 1, 29)

        If Mid(pDenpyo_Biko1, 1, 7) = "@MM-DD@" Then
            Kawase_Furikae(12) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Furikae(4), 7, 2) & Mid(pDenpyo_Biko1, 8) & Space(29), 1, 29)
        Else
            Kawase_Furikae(12) = Mid(Trim(pDenpyo_Biko1) & Space(29), 1, 29)
        End If
        If Mid(pDenpyo_Biko2, 1, 7) = "@MM-DD@" Then
            Kawase_Furikae(13) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(STR_FURIKAE_DATE(1), 7, 2) & Mid(pDenpyo_Biko2, 8) & Space(29), 1, 29)
        Else
            Kawase_Furikae(13) = Mid(Trim(pDenpyo_Biko2) & Space(29), 1, 29)
        End If

        Kawase_Furikae(14) = Space(53)

        For iCount = LBound(Kawase_Furikae) To UBound(Kawase_Furikae)
            FD_WORK_D += Kawase_Furikae(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_FuriKomi = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    Private Function PFUNC_Kawase_Tukekae(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kamoku As String, _
                                           ByVal pHimoku_Meigi As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'PFUNC_Kawase_Tukekae = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = ��M�X�� * 30
        ' 3 = ���M�X�� * 20
        ' 4 = �戵�� * 6
        ' 5 = ��� * 4
        ' 6 = ���z * 12
        ' 7 = ���z���L���� * 15
        ' 8 = �ԍ� * 16
        ' 9 = �����t�֎��R1 * 48
        '10 = �����t�֎��R2 * 48
        '11 = �����t�֎��R3 * 20
        '12 = �����t�֎��R4 * 20
        '13 = �Ɖ�ԍ� * 15
        '14 = �\��1 * 21

        Dim Kawase_Tukekae(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Tukekae(0) = "48"
        Kawase_Tukekae(1) = "500"
        '------------------------------
        '��M�X���擾
        '------------------------------
        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("���Z�@�փR�[�h�捞�G���[ : " & Trim(pHimoku_Ginko) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Tukekae(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Tukekae(3) = Mid(Chr(223) & Space(1) & "����" & Space(20), 1, 20)
        Else
            Kawase_Tukekae(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Tukekae(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If

        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Tukekae(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Tukekae(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END
        Kawase_Tukekae(5) = "¹��"
        Kawase_Tukekae(6) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Tukekae(6)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("���L�����ݒ菈���G���[ : " & Trim(Kawase_Tukekae(7)) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If
        Kawase_Tukekae(7) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '------------------------------
        '�ԍ�����E�擾
        '------------------------------
        If pHimoku_Ginko = "1000" Then           '�S�M�A�̏ꍇ
            Kawase_Tukekae(8) = "����-1"
        Else
            Kawase_Tukekae(8) = Space(16)              '�S�M�A�ȊO�̏ꍇ
        End If

        '----------------------------------------
        ' �����t�֎��R���ݒ�
        ' �����t�֎��R1=���l���i�������`�l���j
        ' �����t�֎��R2=�˗��l���iINI�t�@�C��)
        ' �����t�֎��R3=���l�P�i�`�[�p���l�P�j
        ' �����t�֎��R4=���l�Q�i�`�[�p���l�P�j
        '----------------------------------------
        If ConvNullToString(pHimoku_Meigi) = "" Or pHimoku_Meigi.Trim = "" Then
            Call GSUB_MESSAGE_WARNING("���l��������܂���" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        Else
            Kawase_Tukekae(9) = Mid(Trim(pHimoku_Meigi) & Space(48), 1, 48)
        End If
        '�˗��l
        Kawase_Tukekae(10) = Mid(Str_IraiNinName & Space(48), 1, 48)

        '���l���ɐU�֓���ݒ肷��ꍇ�� "@" �ň͂�
        If pDenpyo_Biko1.Length >= 7 Then
            If pDenpyo_Biko1.Substring(0, 7) = "@MM-DD@" Then
                Kawase_Tukekae(11) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Tukekae(4), 7, 2) & Mid(pDenpyo_Biko1, 8) & Space(20), 1, 20)
            Else
                Kawase_Tukekae(11) = Mid(Trim(pDenpyo_Biko1) & Space(20), 1, 20)
            End If
        Else
            Kawase_Tukekae(11) = pDenpyo_Biko1.Trim
        End If

        If pDenpyo_Biko2.Trim.Length > 0 Then
            If pDenpyo_Biko2.Trim.Length >= 7 Then
                If pDenpyo_Biko2.Substring(0, 7) = "@MM-DD@" Then
                    Kawase_Tukekae(12) = Mid(Mid(STR_FURIKAE_DATE(1), 5, 2) & "-" & Mid(Kawase_Tukekae(4), 7, 2) & Mid(pDenpyo_Biko2, 8) & Space(20), 1, 20)
                ElseIf pDenpyo_Biko2.Substring(0, 5) = "@TEN@" Then
                    '�����}�X�^���l�Q�Ɂu@TEN@�v�Ƃ�������
                    '�x�X���������ҏW����@�\�ǉ�
                    Kawase_Tukekae(12) = Str_Ginko(1).Trim & "�¶�" & pDenpyo_Biko2.Substring(5, pDenpyo_Biko2.Length - 5)
                Else
                    Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
                End If
            ElseIf pDenpyo_Biko2.Trim.Length >= 5 Then '�ǉ� 2005/03/22
                If pDenpyo_Biko2.Substring(0, 5) = "@TEN@" Then
                    '�����}�X�^���l�Q�Ɂu@TEN@�v�Ƃ�������
                    '�x�X���������ҏW����@�\�ǉ�
                    Kawase_Tukekae(12) = Str_Ginko(0).Trim & "�¶�" & pDenpyo_Biko2.Substring(5, pDenpyo_Biko2.Length - 5)
                Else
                    Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
                End If
            Else
                Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
            End If
        Else
            Kawase_Tukekae(12) = pDenpyo_Biko2.Trim
        End If

        Kawase_Tukekae(11) = Kawase_Tukekae(11).PadRight(20, " ") '�ǉ� 2005/03/22
        Kawase_Tukekae(12) = Kawase_Tukekae(12).PadRight(20, " ") '�ǉ� 2005/03/22

        Kawase_Tukekae(13) = Space(15)
        Kawase_Tukekae(14) = Space(21)

        For iCount = LBound(Kawase_Tukekae) To UBound(Kawase_Tukekae)
            FD_WORK_D += Kawase_Tukekae(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_Tukekae = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '�ב֐��� 2005/06/28
    Private Function PFUNC_Kawase_Seikyu(ByVal pGakkou_Code As String, _
                                           ByVal pGakkou_Name As String, _
                                           ByVal pHimoku_Ginko As String, _
                                           ByVal pHimoku_Siten As String, _
                                           ByVal pHimoku_Kingaku As Long, _
                                           ByVal pDenpyo_Biko1 As String, _
                                           ByVal pDenpyo_Biko2 As String) As String

        Dim FD_WORK_D As String
        'Kawase_Seikyu = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = ��M�X�� * 30
        ' 3 = ���M�X�� * 20
        ' 4 = �戵�� * 6
        ' 5 = ��� * 4
        ' 6 = ���z * 12
        ' 7 = ���z���L���� * 15
        ' 8 = �ԍ� * 16
        ' 9 = �����t�֎��R * 48
        '10 = �����t�֎��R2 * 48
        '11 = ���l1 * 29
        '12 = ���l2 * 29
        '13 = �Ɖ�ԍ� * 15
        '14 = �\��1 * 3

        Dim Kawase_Seikyu(14) As String

        Dim iCount As Integer

        Dim sAngo As String = ""

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Kawase_Seikyu(0) = "48"
        Kawase_Seikyu(1) = "600"

        '------------------------------
        '��M�X���擾
        '------------------------------
        If PFUNC_GET_GINKONAME(pHimoku_Ginko, pHimoku_Siten) = False Then
            Call GSUB_MESSAGE_WARNING("���Z�@�փR�[�h�捞�G���[ : " & Trim(pHimoku_Ginko) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        If STR_JIKINKO_CODE = pHimoku_Ginko Then
            Kawase_Seikyu(2) = Mid(Chr(223) & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Seikyu(3) = Mid(Chr(223) & Space(1) & "����" & Space(20), 1, 20)
        Else
            Kawase_Seikyu(2) = Mid(Str_Ginko(0).Trim & Space(1) & Str_Ginko(1).Trim & Space(30), 1, 30)
            Kawase_Seikyu(3) = Mid(Str_Kawase_CenterName.Trim & Space(20), 1, 20)
        End If
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- START
        'Kawase_Seikyu(4) = Trim(Str(Val(Mid(STR_FURIKAE_DATE(1), 1, 4)) - CInt(Mid(Str_KijyunDate, 1, 4)))) & Mid(STR_FURIKAE_DATE(1), 5, 4)
        Kawase_Seikyu(4) = CASTCommon.GetWAREKI(Trim(STR_FURIKAE_DATE(1)), "yyMMdd")
        ' GENGOU 2019/04/19 UPD ITL)OOKUBO ------------------------------------------------- END

        Kawase_Seikyu(5) = "����"
        Kawase_Seikyu(6) = Format(CDbl(pHimoku_Kingaku), "000000000000")

        Select Case (pHimoku_Kingaku)
            Case Is <= 0
                Str_Function_Ret = "ZERO"
        End Select

        If PFUNC_Set_FukukiKigo(Format(CLng(Kawase_Seikyu(6)), "#,##0"), sAngo) = False Then
            Call GSUB_MESSAGE_WARNING("���L�����ݒ菈���G���[ : " & Trim(Kawase_Seikyu(7)) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If
        Kawase_Seikyu(7) = Mid(Trim(sAngo) & Space(15), 1, 15)

        '------------------------------
        '�ԍ�����E�擾
        '------------------------------
        Kawase_Seikyu(8) = Space(16)              '�ԍ�

        '----------------------------------------
        ' �����t�֎��R���ݒ�
        ' �����t�֎��R=Space(48)
        ' �����t�֎��R2=Space(48)
        ' �����t�֎��R3=���l�P�i�`�[�p���l�P�j
        ' �����t�֎��R4=���l�Q�i�`�[�p���l�P�j
        '----------------------------------------
        '�����t�֎��R
        Kawase_Seikyu(9) = Space(48)
        '�����t�֎��R2
        Kawase_Seikyu(10) = Space(48)

        '���l�P���Q�@�Ƃ肠������
        Kawase_Seikyu(11) = Space(29)
        Kawase_Seikyu(12) = Space(29)

        Kawase_Seikyu(13) = Space(15)
        Kawase_Seikyu(14) = Space(3)


        For iCount = LBound(Kawase_Seikyu) To UBound(Kawase_Seikyu)
            FD_WORK_D += Kawase_Seikyu(iCount)
        Next iCount

        If Len(FD_WORK_D) = 280 Then
            PFUNC_Kawase_Seikyu = FD_WORK_D
        Else
            Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
            Str_Function_Ret = "NG"
            Return Str_Function_Ret
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '������A������(���U�萔��)
    Private Function PFUNC_Syokanjo_Nyuukin(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'Syokanjo_Nyuukin = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = �����ԍ� * 7
        ' 3 = �s * 2
        ' 4 = ����R�[�h * 2
        ' 5 = �O�c * 12
        ' 6 = �����R�[�h * 1
        ' 7 = ���z * 12
        ' 8 = ���� * 5
        ' 9 = �U�փR�[�h * 3
        '10 = �戵�ԍ�1 * 3
        '11 = �l�i�R�[�h * 2
        '12 = �ېŃR�[�h * 1
        '13 = �E�v * 20
        '14 = �A���X�� * 3
        '15 = �A���Ȗڌ��� * 9
        '16 = �������R�[�h * 2
        '17 = �A���\�t�g�@�� * 1
        '18 = �戵�ԍ�2 * 5
        '19 = ����E�v * 20
        '20 = �N�Z�� * 6
        '21 = �\��1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza) = 0 Then
            Call GSUB_MESSAGE_WARNING("�����ԍ�������܂��� : " & Trim(Str_Tesuuryo_Kouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code) = 0 Then
            Call GSUB_MESSAGE_WARNING("����R�[�h������܂��� : " & Trim(Str_Utiwake_Code) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("ý��ֳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���X�Ԃ�����܂��� : " & Trim(pTSiten_Code) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���Ȗڌ��Ԃ�����܂��� : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
                Str_Function_Ret = "NG"
                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '������A������(�X����)
    Private Function PFUNC_Syokanjo_Nyuukin_SOURYO(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'Syokanjo_Nyuukin_SOURYO = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = �����ԍ� * 7
        ' 3 = �s * 2
        ' 4 = ����R�[�h * 2
        ' 5 = �O�c * 12
        ' 6 = �����R�[�h * 1
        ' 7 = ���z * 12
        ' 8 = ���� * 5
        ' 9 = �U�փR�[�h * 3
        '10 = �戵�ԍ�1 * 3
        '11 = �l�i�R�[�h * 2
        '12 = �ېŃR�[�h * 1
        '13 = �E�v * 20
        '14 = �A���X�� * 3
        '15 = �A���Ȗڌ��� * 9
        '16 = �������R�[�h * 2
        '17 = �A���\�t�g�@�� * 1
        '18 = �戵�ԍ�2 * 5
        '19 = ����E�v * 20
        '20 = �N�Z�� * 6
        '21 = �\��1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza2) = 0 Then
            Call GSUB_MESSAGE_WARNING("�����ԍ�������܂��� : " & Trim(Str_Tesuuryo_Kouza2) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza2), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code2) = 0 Then
            Call GSUB_MESSAGE_WARNING("����R�[�h������܂��� : " & Trim(Str_Utiwake_Code2) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code2), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("���ֳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���X�Ԃ�����܂��� : " & Trim(pTSiten_Code) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���Ȗڌ��Ԃ�����܂��� : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)

            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin_SOURYO = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
                Str_Function_Ret = "NG"

                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function

    '������A������(�U���萔��)
    Private Function PFUNC_Syokanjo_Nyuukin_FURIKOMI(ByVal pGakkou_Code As String, _
                                            ByVal pGakkou_Name As String, _
                                            ByVal pTesuuryo_Kingaku As Long, _
                                            ByVal pTSiten_Code As String, _
                                            ByVal pTKamoku_Code As String, _
                                            ByVal pTKouza As String) As String


        Dim FD_WORK_D As String
        'PFUNC_Syokanjo_Nyuukin_FURIKOMI = INDEX��
        ' 0 = �ȖڃR�[�h * 2
        ' 1 = ����R�[�h * 3
        ' 2 = �����ԍ� * 7
        ' 3 = �s * 2
        ' 4 = ����R�[�h * 2
        ' 5 = �O�c * 12
        ' 6 = �����R�[�h * 1
        ' 7 = ���z * 12
        ' 8 = ���� * 5
        ' 9 = �U�փR�[�h * 3
        '10 = �戵�ԍ�1 * 3
        '11 = �l�i�R�[�h * 2
        '12 = �ېŃR�[�h * 1
        '13 = �E�v * 20
        '14 = �A���X�� * 3
        '15 = �A���Ȗڌ��� * 9
        '16 = �������R�[�h * 2
        '17 = �A���\�t�g�@�� * 1
        '18 = �戵�ԍ�2 * 5
        '19 = ����E�v * 20
        '20 = �N�Z�� * 6
        '21 = �\��1 * 159
        Dim Syokanjo_Nyuukin(21) As String

        Dim iCount As Integer

        Str_Function_Ret = "NG"
        FD_WORK_D = ""

        Syokanjo_Nyuukin(0) = "99"
        Syokanjo_Nyuukin(1) = "419"

        If CLng(Str_Tesuuryo_Kouza3) = 0 Then
            Call GSUB_MESSAGE_WARNING("�����ԍ�������܂��� : " & Trim(Str_Tesuuryo_Kouza3) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(2) = Format(CLng(Str_Tesuuryo_Kouza3), "0000000")
        Syokanjo_Nyuukin(3) = "01"

        If CInt(Str_Utiwake_Code3) = 0 Then
            Call GSUB_MESSAGE_WARNING("����R�[�h������܂��� : " & Trim(Str_Utiwake_Code3) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(4) = Format(CInt(Str_Utiwake_Code3), "00")
        Syokanjo_Nyuukin(5) = "000000000000"
        Syokanjo_Nyuukin(6) = "1"
        Syokanjo_Nyuukin(7) = Format(pTesuuryo_Kingaku, "000000000000")
        Syokanjo_Nyuukin(8) = "00001"
        Syokanjo_Nyuukin(9) = Space(3)
        Syokanjo_Nyuukin(10) = Space(3)
        Syokanjo_Nyuukin(11) = Space(2)
        Syokanjo_Nyuukin(12) = Space(1)
        Syokanjo_Nyuukin(13) = Mid("�غ�ý��ֳ" & Space(20), 1, 20)

        If Trim(pTSiten_Code) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���X�Ԃ�����܂��� : " & Trim(pTSiten_Code) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(14) = Format(CInt(pTSiten_Code), "000")

        If Trim(pTKamoku_Code) = "" Or Trim(pTKouza) = "" Then
            Call GSUB_MESSAGE_WARNING("�A���Ȗڌ��Ԃ�����܂��� : " & Trim(pTKamoku_Code) & " & " & Trim(pTKouza) & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name)
            Return Str_Function_Ret
        End If

        Syokanjo_Nyuukin(15) = Format(CInt(pTKamoku_Code), "00") & Format(CInt(pTKouza), "0000000")
        Syokanjo_Nyuukin(16) = Space(2)
        Syokanjo_Nyuukin(17) = Space(1)
        Syokanjo_Nyuukin(18) = Space(5)
        Syokanjo_Nyuukin(19) = Space(20)
        Syokanjo_Nyuukin(20) = Space(6)
        Syokanjo_Nyuukin(21) = Space(159)

        If CLng(Syokanjo_Nyuukin(7)) = 0 Then
            Str_Function_Ret = "ZERO"
        Else
            For iCount = LBound(Syokanjo_Nyuukin) To UBound(Syokanjo_Nyuukin)
                FD_WORK_D += Syokanjo_Nyuukin(iCount)
            Next iCount

            If Len(FD_WORK_D) = 280 Then
                PFUNC_Syokanjo_Nyuukin_FURIKOMI = FD_WORK_D
            Else
                Call GSUB_MESSAGE_WARNING("���Ϗ��Ɍ�肪����܂�" & vbCrLf & "�����R�[�h = " & pGakkou_Code & vbCrLf & "����於 = " & pGakkou_Name & vbCrLf & "�쐬�����f�[�^�T�C�Y���Ⴂ�܂� : " & Len(FD_WORK_D) & "�o�C�g")
                Str_Function_Ret = "NG"
                Return Str_Function_Ret
            End If
        End If

        If Str_Function_Ret <> "ZERO" Then
            Str_Function_Ret = "OK"
        End If

        Return Str_Function_Ret

    End Function


    Private Function PFUNC_Update_Schedule(ByVal pGakkou_Code As String, _
                                           ByVal pFurikae_Date As String, _
                                           ByVal pTimeStamp As String, _
                                           ByVal pFuri_KBN_S As String) As Boolean

        PFUNC_Update_Schedule = False

        STR_SQL = " UPDATE  G_SCHMAST SET "
        STR_SQL += " KESSAI_FLG_S = '1'"
        STR_SQL += ",KESSAI_DATE_S = '" & pTimeStamp.Substring(0, 8) & "'" '�ǉ�2005/06/16
        STR_SQL += ",TIME_STAMP_S = '" & pTimeStamp & "'"
        STR_SQL += ",TESUU_KIN3_S = " & CLng(dblFURIKOMI_TESUU_ALL)  '�U���萔���X�V 2006/04/15
        STR_SQL += ",TESUU_KIN_S = TESUU_KIN1_S + TESUU_KIN2_S + " & CLng(dblFURIKOMI_TESUU_ALL)   '�U���萔���X�V 2006/04/15
        'STR_SQL += ",TESUU_KIN_S = TESUU_KIN_S + " & CLng(dblFURIKOMI_TESUU_ALL)   '�U���萔���X�V 2006/04/15
        STR_SQL += " WHERE"
        STR_SQL += " FURI_DATE_S = '" & pFurikae_Date & "'"
        STR_SQL += " AND"
        STR_SQL += " GAKKOU_CODE_S ='" & pGakkou_Code & "'"
        STR_SQL += " AND"
        STR_SQL += " FURI_KBN_S ='" & pFuri_KBN_S & "'" '�ǉ� 2005/06/28
        'STR_SQL += " SCH_KBN_S <> '2'"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        Lng_Upd_Count += 1

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_Update_Schedule = True

    End Function
    Private Function PFUNC_Set_FukukiKigo(ByVal pValue1 As String, ByRef pValue2 As String) As Boolean

        Dim iLoopCount As Integer

        Dim sLZH As String
        Dim sFugo(14) As String

        PFUNC_Set_FukukiKigo = False

        sLZH = "Y"
        pValue2 = ""

        '�n���ꂽ���z�����Í�������
        For iLoopCount = 0 To 14
            sFugo(iLoopCount) = Space(1)
            Select Case (Mid(Format(CLng(pValue1), "000,000,000,000"), iLoopCount + 1, 1))
                Case "0"
                    If sLZH = "Y" Then
                        sFugo(iLoopCount) = Space(1)
                    Else
                        sFugo(iLoopCount) = "�"
                    End If
                Case "1"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "2"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "3"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "4"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "5"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "6"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "7"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "8"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case "9"
                    sLZH = "N"
                    sFugo(iLoopCount) = "�"
                Case ","
                    sFugo(iLoopCount) = Space(1)
            End Select
        Next iLoopCount


        For iLoopCount = 0 To 14
            pValue2 += sFugo(iLoopCount)
        Next

        pValue2 = Trim(pValue2)

        PFUNC_Set_FukukiKigo = True

    End Function
    Private Function PFUNC_GET_GINKONAME(ByVal pGinko_Code As String, ByVal pSiten_Code As String) As Boolean

        Dim ret As Boolean = False

        '���Z�@�փR�[�h�Ǝx�X�R�[�h������Z�@�֖��A�x�X���𒊏o
        Str_Ginko(0) = ""
        Str_Ginko(1) = ""

        If Trim(pGinko_Code) = "" Or Trim(pSiten_Code) = "" Then
            Exit Function
        End If

        Dim SQL As String = ""

        Dim OraReader As CASTCommon.MyOracleReader = Nothing

        Try
            SQL = "SELECT KIN_KNAME_N , SIT_KNAME_N  FROM TENMAST "
            SQL &= " WHERE KIN_NO_N = '" & pGinko_Code & "'"
            SQL &= " AND SIT_NO_N = '" & pSiten_Code & "'"

            OraReader = New CASTCommon.MyOracleReader

            If OraReader.DataReader(SQL) Then
                Str_Ginko(0) = OraReader.GetItem("KIN_KNAME_N")
                Str_Ginko(1) = OraReader.GetItem("SIT_KNAME_N")
            End If

            OraReader.Close()
            OraReader = Nothing

            ret = True

        Catch ex As Exception
            Throw New Exception("TENMAST�擾���s", ex)
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Private Function PFUNC_Split_WorkD() As Boolean

        Dim oFileSys As Object
        Dim oDrive As Object

        Dim sFileName As String
        Dim sHedder As String
        Dim sOut As String

        Dim iFileNo As Integer
        Dim iFileNo2 As Integer
        Dim iFileNo3 As Integer

        Dim sFILE_NAME1 As String '�f�[�^�x�[�X���쐬���ꂽ�t�@�C��
        Dim sFILE_NAME2 As String 'FILE1��PowerSort���g�p���ĕ��ёւ�������
        Dim sFILE_NAME3 As String '

        Dim iCnt As Integer
        Dim iSCount As Integer
        Dim iLCount As Integer

        Dim Rient_Rec_Cnt As Integer

        Dim HEAD_DATA(1007) As Byte
        Dim OUT_DATA(255) As Byte

        Dim lRead_Point As Long
        Dim lWrite_Point As Long

        PFUNC_Split_WorkD = False

        On Error Resume Next

        sFILE_NAME1 = STR_DAT_PATH & "FD_WORK1_D.DAT"
        sFILE_NAME2 = STR_DAT_PATH & "FD_WORK2_D.DAT"
        sFILE_NAME3 = STR_DAT_PATH & "FD_WORK_D.DAT"

        'PowerSort�Ń��[�N�t�@�C���̓��e���\�[�g����
        '�\�[�g�L�[�E�t�B�[���h(�w�Z�R�[�h�A���R�[�h�敪�A���ɁA�X�܁A�ȖځA����)���w��
        With AxPowerSORT1
            '�G���[���b�Z�[�W��\�����Ȃ��|���w�肵�܂��B
            'True :���b�Z�[�W�\��
            'False:���b�Z�[�W��\��
            .DispMessage = False

            '�\�[�g�������w�肵�܂��B
            '0	(�ȗ��l) �\�[�g�@�\�����s���܂��B
            '1	�}�[�W�@�\�����s���܂��B
            '2	�R�s�[�@�\�����s���܂��B
            .DisposalNumber = 0

            '�t�B�[���h�̓J�����ʒu�Ŏw��
            '0	(�ȗ��l) �����t�B�[���h�Ŏw�肵�܂��B
            '1	�Œ�t�B�[���h�Ŏw�肵�܂��B
            .FieldDefinition = 1

            '�\�[�g�L�[�E�t�B�[���h(�w�Z�R�[�h�A���R�[�h�敪�A���ɁA�X�܁A�ȖځA����)���w��
            .KeyCmdStr = "0.7asca 7.1asca 8.4asca 12.3asca 15.1asca 16.7asca"

            '���̓t�@�C�������w�肵�܂��B
            .InputFiles = sFILE_NAME1
            '���̓t�@�C����ʂɃo�C�i���Œ蒷�t�@�C���i1�j���w�肵�܂��B
            .InputFileType = 1

            '�o�̓t�@�C�������w�肵�܂��B
            .OutputFile = sFILE_NAME2
            '�o�̓t�@�C����ʂɃo�C�i���Œ蒷�t�@�C���i1�j���w�肵�܂��B
            .OutputFileType = 1

            .MaxRecordLength = 303
            .Action()                     '���R�[�h�I�����������s���܂��B

            If .ErrorCode <> 0 Then       '�G���[���o���̏����B
                Call GSUB_MESSAGE_WARNING("PowerSORT�ŃG���[�����o���܂����B ErrorDetail=" & .ErrorDetail)

                Exit Function
            End If
        End With

        iFileNo2 = FreeFile()
        FileOpen(iFileNo2, sFILE_NAME2, OpenMode.Random, , , 303)    '���[�N�t�@�C��FD_WORK2_D.DAT

        If Err.Number <> 0 Then
            Call GSUB_MESSAGE_WARNING("���ԃt�@�C���̃I�[�v���Ɏ��s���܂���")
            FileClose(iFileNo2)

            Exit Function
        End If

        If Dir$(sFILE_NAME3) <> "" Then
            Kill(sFILE_NAME3)
        End If

        Err.Clear()

        iFileNo3 = FreeFile()
        FileOpen(iFileNo3, sFILE_NAME3, OpenMode.Random, , , 280)    '���[�N�t�@�C��FD_WORK_D.DAT

        If Err.Number <> 0 Then
            Call GSUB_MESSAGE_WARNING("���ԃt�@�C���̃I�[�v���Ɏ��s���܂���")
            FileClose(iFileNo3)
            Exit Function
        End If


        Dim Data As String
        Dim Next_Data As String
        Dim OUT_DATA1 As String
        Dim OUT_Data2 As String
        Dim KBN As String
        Dim Cnt As Long
        Dim Next_Cnt As Long
        Dim recHIMOKU As String = ""
        Dim recTESUU As String = ""
        Dim ALL_KINGAKU As Long
        Dim ALL_TESUU As Long
        Dim DATA_TYPE As String = ""

        Dim WriteCnt As Long

        Dim FILE_REC As Long

        ALL_KINGAKU = 0
        ALL_TESUU = 0
        Cnt = 1
        WriteCnt = 0
        FILE_REC = 0
        '-------------------------------------------------------------------------------
        'FD_WORK2_D.DAT�̍s���J�E���g
        '-------------------------------------------------------------------------------
        Do Until EOF(iFileNo2) = True
            FILE_REC = FILE_REC + 1
            FileGet(iFileNo2, Line, FILE_REC)
        Loop

        For Cnt = 1 To FILE_REC
            FileGet(iFileNo2, Line, Cnt)

            KBN = Line.Data.Substring(7, 1)
            Select Case (KBN)
                Case "1"    '<---���R�[�h�敪"1"
                    recHIMOKU = ""
                    recTESUU = ""

                    StrLine.Data = Line.Data.Substring(23, 280)

                    WriteCnt += 1

                    FilePut(iFileNo3, StrLine, WriteCnt)
                Case "2"    '<---���R�[�h�敪"2"
                    DATA_TYPE = Line.Data.Substring(23, 5)
                    Next_Cnt = Cnt + 1

                    WriteCnt += 1

                    FileGet(iFileNo2, NextLine, Next_Cnt)

                    If Line.Data.Substring(8, 15) <> NextLine.Data.Substring(8, 15) Then
                        '<---���E�X�E�ȁE��������Ă��ꍇ
                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---���ʓ����̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(38, 10)
                            Case "01010"
                                '<---���������̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(36, 10)
                            Case "48100"
                                '<---�ב֐U���̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(92, 12)
                            Case "48500"
                                '<---�ב֕t�ւ̏ꍇ 2005/06/20
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                            Case "48600"
                                '<---�ב֐����̏ꍇ 2005/06/28
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                        End Select

                        OUT_DATA1 = Line.Data.Substring(23, 280)
                        OUT_Data2 = ""

                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---���ʓ����̏ꍇ 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 14) & Format(ALL_KINGAKU, "0000000000") & Mid(OUT_DATA1, 25, 256)
                            Case "01010"
                                '<---���������̏ꍇ 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 12) & Format(ALL_KINGAKU, "0000000000") & Mid(OUT_DATA1, 23, 256)
                            Case "48100"
                                '<---�ב֐U���̏ꍇ 2003/08/27
                                StrLine.Data = Mid(OUT_DATA1, 1, 69) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 82, 256)
                            Case "48500"
                                '<---�ב֕t�ւ̏ꍇ 2005/06/20
                                StrLine.Data = Mid(OUT_DATA1, 1, 65) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 78, 256)
                            Case "48600"
                                '<---�ב֐����̏ꍇ 2005/06/28
                                StrLine.Data = Mid(OUT_DATA1, 1, 65) & Format(ALL_KINGAKU, "000000000000") & Mid(OUT_DATA1, 78, 256)
                        End Select

                        FilePut(iFileNo3, StrLine, WriteCnt)

                        ALL_KINGAKU = 0
                    Else
                        Select Case (DATA_TYPE)
                            Case "02019"
                                '<---���ʓ����̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(38, 10)
                            Case "01010"
                                '<---���������̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(36, 10)
                            Case "48100"
                                '<---�ב֐U���̏ꍇ 2003/08/27
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(92, 12)
                            Case "48500"
                                '<---�ב֕t�ւ̏ꍇ 2005/06/20
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                            Case "48600"
                                '<---�ב֐����̏ꍇ 2005/06/28
                                ALL_KINGAKU = ALL_KINGAKU + Line.Data.Substring(88, 12)
                        End Select

                        GoTo NEXT_LOOP
                    End If
                Case "8"    '<---���R�[�h�敪"8"
                    If recTESUU <> Line.Data.Substring(28, 7) Then 'ini�t�@�C���̎萔�������Ⴄ�ꍇ
                        recTESUU = Line.Data.Substring(28, 7)
                        ALL_TESUU = ALL_TESUU + Line.Data.Substring(52, 12)
                        StrLine.Data = Line.Data.Substring(23, 280)
                        WriteCnt += 1

                        FilePut(iFileNo3, StrLine, WriteCnt)

                        ALL_TESUU = 0
                    Else
                        ALL_TESUU = ALL_TESUU + Line.Data.Substring(52, 12)

                        GoTo NEXT_LOOP
                    End If
            End Select
NEXT_LOOP:

        Next

        FileClose(iFileNo2)
        FileClose(iFileNo3)

        '************************************************************
        '�ꎞ�I�ɃR�����g����
        '���G���^�͍��Ȃ�(�������ω�ʂ���쐬���邽��)
        ''���G���^�p�f�[�^�쐬
        'If clsFUSION.fn_MAKE_RIENTDATA("", _
        '                               STR_DAT_PATH & "FD_WORK_D.DAT", _
        '                               STR_DAT_PATH & "FD_WORK_O.DAT", _
        '                               STR_DAT_PATH & "FD_WORK_H.DAT", _
        '                               Str_Jikou_Ginko_Code & Str_Honbu_Code, _
        '                               STR_FURIKAE_DATE(1), _
        '                               Rient_Rec_Cnt) = False Then

        '    Err.Clear()
        '    Exit Function
        'End If
        '************************************************************


        '�쐬�����Q�̃t�@�C�����烊�G���^�t�@�C�����쐬����
Retry:
        If GFUNC_MESSAGE_QUESTION("�e�c���Z�b�g���ĉ�����") <> vbOK Then

            Exit Function
        End If

        Select Case (Int_FD_Kbn)
            Case 1
                'FD�敪��IBM�`���̏ꍇ

            Case Else
                Str_Ret = InputBox("�t�@�C��������͂��Ă�������", , "RIENTER.RNT")

                If Trim(Str_Ret) <> "" Then
                    sFileName = Str_Kekka_Path & Str_Ret

                    '�擾����PATH��FD��ײ�ނ̏ꍇ
                    Select Case (StrConv(Mid(Str_Kekka_Path, 1, 1), vbProperCase))
                        Case "A", "B"
                            '�e�c�ǂݎ�菈��
                            oFileSys = CreateObject("Scripting.FileSystemObject")
                            oDrive = oFileSys.GetDrive(Str_Kekka_Path)

                            If oDrive.IsReady <> True Then
                                If GFUNC_MESSAGE_QUESTION("�����t���b�s�[����������ł�������") <> vbOK Then
                                    Exit Function
                                End If
                                GoTo Retry

                            End If
                    End Select

                    If Dir$(sFileName) <> "" Then
                        Kill(sFileName)
                    End If

                    iFileNo = FreeFile()

                    Err.Number = 0

                    FileOpen(iFileNo, sFileName, OpenMode.Binary, OpenAccess.Write, , )     '���[�N�t�@�C��

                    If Err.Number <> 0 Then
                        Exit Function
                    End If

                    'FilePut(iFileNo, Str_Ret, )
                    '�t�@�C�����ݒ� 2006/04/14
                    strTITLE.strTITLE_16 = Str_Ret
                    FilePut(iFileNo, strTITLE, )

                    iFileNo2 = FreeFile()
                    '�쐬�����w�b�_�t�@�C����ǂݍ��݃w�b�_�f�[�^����������
                    FileOpen(iFileNo2, STR_DAT_PATH & "FD_WORK_H.DAT", OpenMode.Binary, OpenAccess.Read, , 1008)

                    FileGet(iFileNo2, HEAD_DATA, 1)
                    FilePut(iFileNo, HEAD_DATA, )

                    FileClose(iFileNo2)

                    iFileNo3 = FreeFile()
                    '�쐬�����o�̓t�@�C����ǂݍ��ݏo�̓f�[�^����������
                    FileOpen(iFileNo3, STR_DAT_PATH & "FD_WORK_O.DAT", OpenMode.Binary, OpenAccess.Read, , 256)

                    iCnt = 1

                    Do Until EOF(iFileNo3)
                        lRead_Point = ((iCnt - 1) * 256) + 1
                        FileGet(iFileNo3, OUT_DATA, lRead_Point)

                        iSCount = 32 + CInt(OUT_DATA(5))

                        For iLCount = iSCount To 255
                            OUT_DATA(iLCount) = 0
                        Next iLCount

                        lWrite_Point = 1025 + ((iCnt - 1) * 256)
                        FilePut(iFileNo, OUT_DATA, lWrite_Point)

                        iCnt += 1
                    Loop

                    FileClose(iFileNo)
                    FileClose(iFileNo3)

                End If
        End Select

        PFUNC_Split_WorkD = True

    End Function

    Private Function PFUNC_DeletePrtKessai() As Boolean

        PFUNC_DeletePrtKessai = False

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 0) = False Then

            Exit Function
        End If

        STR_SQL = " DELETE   FROM G_PRTKESSAI"

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then

            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 2) = False Then

            Exit Function
        End If

        PFUNC_DeletePrtKessai = True


    End Function
    Private Function PFUNC_InsertPrtKessai() As Boolean

        PFUNC_InsertPrtKessai = False

        STR_SQL = " INSERT INTO G_PRTKESSAI"
        STR_SQL += " values("
        STR_SQL += "'" & Str_Prt_GAKKOU_CODE & "'"
        STR_SQL += ",'" & Str_Prt_GAKKOU_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_GAKKOU_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_HONBU_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_DATE & "'"
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_JIFURI)
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_FURI)
        STR_SQL += "," & CInt(Str_Prt_TESUURYO_ETC)
        STR_SQL += ",'" & Str_Utiwake_Code & "'" '���U����R�[�h�ǉ� 2006/04/17
        STR_SQL += ",'" & Str_Prt_JIFURI_KOUZA & "'"
        STR_SQL += ",'" & Str_Utiwake_Code3 & "'" '�U������R�[�h�ǉ� 2006/04/17
        STR_SQL += ",'" & Str_Prt_FURI_KOUZA & "'"
        STR_SQL += ",'" & Str_Utiwake_Code2 & "'" '���̑�����R�[�h�ǉ� 2006/04/17
        STR_SQL += ",'" & Str_Prt_ETC_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_HIMOKU_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_PATERN & "'"
        STR_SQL += "," & CLng(Str_Prt_NYUKIN_KINGAKU)
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_CODE & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KIN_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_TENPO & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_SIT_KNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_SIT_NNAME & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KAMOKU & "'"
        STR_SQL += ",'" & Str_Prt_KESSAI_KOUZA & "'"
        STR_SQL += ",'" & Str_Prt_FURI_KBN & "'"
        STR_SQL += "," & Str_Prt_TESUURYO_FURI '�U���萔���ǉ� 2006/04/15
        STR_SQL += "," & lngSYORIKEN_TOKUBETU '��������
        STR_SQL += "," & dblSYORIKIN_TOKUBETU '�������z
        STR_SQL += "," & lngFURIKEN_TOKUBETU '�U�֍ό���
        STR_SQL += "," & dblFURIKIN_TOKUBETU '�U�֍ϋ��z
        STR_SQL += "," & lngFUNOUKEN_TOKUBETU '�s�\����
        STR_SQL += "," & dblFUNOUKIN_TOKUBETU '�s�\���z
        STR_SQL += ",'" & sSyori_Date & "'" '�^�C���X�^���v
        STR_SQL += ",'" & Str_Prt_FURI_DATE & "'"   '�U�֓�
        STR_SQL += " )"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_InsertPrtKessai = True

    End Function

    Private Function PFUNC_Himoku_Create() As Boolean

        Dim iNo As Integer

        '�ǉ�
        Dim CHK_GAKUNEN As Integer
        Dim CHK_HIMOKU_NO As Integer
        Dim iLcount As Integer
        Dim bFlg As Boolean = False
        Dim bLoopFlg As Boolean = False
        Dim iLoopCount As Integer

        Dim strFURI_KBN_LOCAL As String = "0"
        Dim strFURI_DATE_LOCAL As String = "00000000"
        Dim strNENGETUDO_LOCAL As String = "000000"
        Dim strGAKKOU_CODE_LOCAL As String = ""

        PFUNC_Himoku_Create = False

        STR_SQL = " DELETE  FROM MAIN0500G_WORK"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If
        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If
        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        STR_SQL = " SELECT * FROM G_SCHMAST WHERE"
        STR_SQL += " KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'"
        STR_SQL += " AND"
        STR_SQL += " KESSAI_FLG_S = '0'" '���σt���O=0�̂��� 2005/06/30
        STR_SQL += " AND"
        STR_SQL += " TYUUDAN_FLG_S = '0'" '���f�t���O=0�̂��� 2006/04/17
        STR_SQL += " AND"
        STR_SQL += " SCH_KBN_S <> '2'" '�X�P�W���[���敪<>2�̂���(�W����) 2006/04/13
        STR_SQL += " ORDER BY GAKKOU_CODE_S,NENGETUDO_S,FURI_DATE_S,FURI_KBN_S"

        If GFUNC_SELECT_SQL3(STR_SQL, 0) = False Then
            Exit Function
        End If
        ReDim iGakunen_Flag(9)
        While (OBJ_DATAREADER_DREAD.Read = True)
            '�w�N�t���O�擾 2006/10/27
            For iLoopCount = 1 To 9
                'If OBJ_DATAREADER_DREAD.Item("GAKUNEN" & iLoopCount & "_FLG_S") = "1" Then
                iGakunen_Flag(iLoopCount) = OBJ_DATAREADER_DREAD.Item("GAKUNEN" & iLoopCount & "_FLG_S")
                'End If
            Next
            strFURI_KBN_LOCAL = OBJ_DATAREADER_DREAD.Item("FURI_KBN_S")
            strFURI_DATE_LOCAL = OBJ_DATAREADER_DREAD.Item("FURI_DATE_S")
            strNENGETUDO_LOCAL = OBJ_DATAREADER_DREAD.Item("NENGETUDO_S")
            strGAKKOU_CODE_LOCAL = OBJ_DATAREADER_DREAD.Item("GAKKOU_CODE_S")

            STR_SQL = " SELECT "
            STR_SQL += " G_MEIMAST.*"
            STR_SQL += ",HIMOMAST.*"

            If strFURI_KBN_LOCAL = "2" Or strFURI_KBN_LOCAL = "3" Then
                STR_SQL += " FROM G_MEIMAST ,HIMOMAST "
                STR_SQL += " WHERE"
                STR_SQL += " FURI_DATE_M ='" & strFURI_DATE_LOCAL & "'"
                STR_SQL += " AND"
                STR_SQL += " HIMOKU_ID_H ='000'"
                STR_SQL += " AND"
                STR_SQL += " GAKUNEN_CODE_M = GAKUNEN_CODE_H"
                STR_SQL += " AND"
                STR_SQL += " GAKKOU_CODE_M = GAKKOU_CODE_H"
                STR_SQL += " AND"
                STR_SQL += " FURI_KBN_M = '" & strFURI_KBN_LOCAL & "'"
            Else
                STR_SQL += " FROM G_MEIMAST LEFT JOIN HIMOMAST ON (G_MEIMAST.HIMOKU_ID_M = HIMOMAST.HIMOKU_ID_H) AND (G_MEIMAST.GAKUNEN_CODE_M = HIMOMAST.GAKUNEN_CODE_H) AND (G_MEIMAST.GAKKOU_CODE_M = HIMOMAST.GAKKOU_CODE_H)"
                STR_SQL += " WHERE"
                STR_SQL += " FURI_DATE_M ='" & strFURI_DATE_LOCAL & "'"
                STR_SQL += " AND"
                STR_SQL += " TUKI_NO_H ='" & strNENGETUDO_LOCAL.Substring(4, 2) & "'"
                '�����E�Վ��o���f�[�^�͏��O 2007/02/10
                STR_SQL += " AND"
                STR_SQL += " (FURI_KBN_M <> '2' OR FURI_KBN_M <> '3')"

            End If

            '�w�Z�R�[�h�ǉ� 2006/04/18 
            STR_SQL += " AND"
            STR_SQL += " GAKKOU_CODE_M = '" & strGAKKOU_CODE_LOCAL & "'"
            '��2006/10/26
            STR_SQL += " AND ("
            bLoopFlg = False
            For iLcount = 1 To 9
                If iGakunen_Flag(iLcount) = 1 Then
                    If bLoopFlg = True Then
                        STR_SQL += " OR "
                    End If
                    STR_SQL += " GAKUNEN_CODE_M=" & iLcount
                    bLoopFlg = True
                End If
            Next iLcount
            STR_SQL += " )"

            If GFUNC_SELECT_SQL4(STR_SQL, 0) = False Then
                Exit Function
            End If

            While (OBJ_DATAREADER_DREAD2.Read = True)
                For iNo = 1 To 15

                    If IsDBNull(OBJ_DATAREADER_DREAD2.Item("HIMOKU_NAME" & Format(iNo, "00") & "_H")) = False Then
                        CHK_HIMOKU_NO = iNo
                        CHK_GAKUNEN = OBJ_DATAREADER_DREAD2.Item("GAKUNEN_CODE_M")

                        '�U�֋敪=2(����)�܂���3(�Վ��o��)���A
                        '��ڃ}�X�^��YOBI1_H�`YOBI3_H�ɒl�������ꍇ�G���[�Ƃ��� 2005/07/01
                        If strFURI_KBN_LOCAL = "2" Then '����
                            If ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Trim.Length <> 7 Then
                                MessageBox.Show("�������̌��ϋ��Z�@�ւ��o�^����Ă��܂���", STR_COMMAND, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                If GFUNC_SELECT_SQL4("", 1) = False Then
                                    Exit Function
                                End If

                                Exit Function
                            End If
                        ElseIf strFURI_KBN_LOCAL = "3" Then '�Վ��o��
                            If ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Trim.Length <> 17 Then
                                MessageBox.Show("�Վ��o�����̌��ϋ��Z�@�ւ��o�^����Ă��܂���", STR_COMMAND, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                If GFUNC_SELECT_SQL4("", 1) = False Then
                                    Exit Function
                                End If

                                Exit Function
                            End If
                        End If

                        STR_SQL = " SELECT * FROM MAIN0500G_WORK"
                        STR_SQL += " WHERE"
                        STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                        '���ʎ��ƒʏ펞�͌��ϋ��Z�@�֏��͕ʍ��ڂ���擾
                        '�Վ��f�[�^��YOBI1_H�܂���YOBI2_H����擾 2005/06/28
                        If strFURI_KBN_LOCAL = "2" Then '����
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='01'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H = '9999999'"

                        ElseIf strFURI_KBN_LOCAL = "3" Then '�Վ��o��
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H = '" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"

                        Else
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KIN_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_TENPO_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KAMOKU_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                            STR_SQL += " AND"
                            STR_SQL += " KESSAI_KOUZA_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"

                        End If

                        If GFUNC_SELECT_SQL5(STR_SQL, 0) = False Then
                            Exit Function
                        End If

                        If OBJ_DATAREADER_DREAD3.HasRows = True Then
                            OBJ_DATAREADER_DREAD3.Read()

                            STR_SQL = " UPDATE  MAIN0500G_WORK SET "

                            If CHK_HIMOKU_NO <> OBJ_DATAREADER_DREAD3.Item("HIMOKU_NO_H") Then
                                STR_SQL += " HIMOKU_NAME_H = '*** ���Z ***',"
                            End If

                            '�Վ��f�[�^���E�ʏ펞�̌��ϋ��Z�@�֏��͕ʍ��ڂ���擾
                            '�Վ��f�[�^��YOBI1_H�܂���YOBI2_H��YOBI3_H����擾 2005/06/28
                            If strFURI_KBN_LOCAL = "2" Then '����
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='01'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H = '9999999'"

                                iNo = 15 '��ڂ��Ƃ̋��z�͐ݒ肳��Ă��Ȃ��̂�iNO=15�ɂ��ă��[�v�I��
                            ElseIf strFURI_KBN_LOCAL = "3" Then '�Վ��o��
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H = '" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"
                                iNo = 15 '��ڂ��Ƃ̋��z�͐ݒ肳��Ă��Ȃ��̂�iNO=15�ɂ��ă��[�v�I��
                            Else
                                STR_SQL += " HIMOKU_KINGAKU_H = HIMOKU_KINGAKU_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += ", HIMOKU_FURI_KIN_H = HIMOKU_FURI_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                Else
                                    STR_SQL += ", HIMOKU_FUNOU_KIN_H = HIMOKU_FUNOU_KIN_H + " & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                End If
                                STR_SQL += " WHERE"
                                STR_SQL += " GAKKOU_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KIN_CODE_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_TENPO_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KAMOKU_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += " AND"
                                STR_SQL += " KESSAI_KOUZA_H ='" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"

                            End If

                        Else
                            STR_SQL = "insert into MAIN0500G_WORK values("
                            STR_SQL += "'" & OBJ_DATAREADER_DREAD2.Item("GAKKOU_CODE_H") & "'"
                            '���ʎ��ƒʏ펞�͌��ϋ��Z�@�֏��͕ʍ��ڂ���擾
                            '�Վ��f�[�^��YOBI1_H�܂���YOBI2_H����擾 2005/06/28
                            If strFURI_KBN_LOCAL = "2" Then '����

                                STR_SQL += ",'*** ���Z ***'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(0, 4) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI1_H")).Substring(4, 3) & "'"
                                STR_SQL += ",'01'"
                                'If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                '    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                'Else
                                STR_SQL += ",''" '�󂯎�薼�`�l
                                'End If
                                STR_SQL += ",'9999999'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                iNo = 15 '��ڂ��Ƃ̋��z�͐ݒ肳��Ă��Ȃ��̂�iNO=15�ɂ��ă��[�v�I��
                            ElseIf strFURI_KBN_LOCAL = "3" Then '�Վ��o��
                                STR_SQL += ",'*** ���Z ***'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(0, 4) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(4, 3) & "'"
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(7, 2) & "'"
                                'If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                '    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                'Else
                                STR_SQL += ",'" & ConvNullToString(OBJ_DATAREADER_DREAD2.Item("YOBI3_H")) & "'" '�󂯎�薼�`�l
                                'End If
                                STR_SQL += ",'" & CStr(OBJ_DATAREADER_DREAD2.Item("YOBI2_H")).Substring(9, 7) & "'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("SEIKYU_KIN_M")
                                End If
                                iNo = 15 '��ڂ��Ƃ̋��z�͐ݒ肳��Ă��Ȃ��̂�iNO=15�ɂ��ă��[�v�I��
                            Else
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("HIMOKU_NAME" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KIN_CODE" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_TENPO" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KAMOKU" & Format(iNo, "00") & "_H") & "'"
                                If IsDBNull(OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H")) = False Then
                                    STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_MEIGI" & Format(iNo, "00") & "_H") & "'"
                                Else
                                    STR_SQL += ",''"
                                End If
                                STR_SQL += ",'" & OBJ_DATAREADER_DREAD2.Item("KESSAI_KOUZA" & Format(iNo, "00") & "_H") & "'"
                                STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                If OBJ_DATAREADER_DREAD2.Item("FURIKETU_CODE_M") = 0 Then
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                    STR_SQL += ",0"
                                Else
                                    STR_SQL += ",0"
                                    STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("HIMOKU" & iNo & "_KIN_M")
                                End If
                            End If

                            STR_SQL += "," & OBJ_DATAREADER_DREAD2.Item("GAKUNEN_CODE_M") '�ǉ� 2005/06/16
                            STR_SQL += "," & iNo '�ǉ� 2005/06/16
                            STR_SQL += "," & CInt(strFURI_KBN_LOCAL) '�ǉ� 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '�ǉ� 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '�ǉ� 2005/06/28
                            STR_SQL += ",'" & Space(10) & "'" '�ǉ� 2005/06/28
                            STR_SQL += ")"
                        End If

                        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                            Exit Function
                        End If
                        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
                            Exit Function
                        End If

                        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                            Exit Function
                        End If

                        If GFUNC_SELECT_SQL5("", 1) = False Then
                            Exit Function
                        End If
                    End If
                Next iNo
            End While
            If GFUNC_SELECT_SQL4(STR_SQL, 1) = False Then
                Exit Function
            End If
            '2006/12/08
        End While
        If GFUNC_SELECT_SQL3("", 1) = False Then
            Exit Function
        End If

        PFUNC_Himoku_Create = True

    End Function
    Function fn_select_G_SCHMAST() As Boolean
        '============================================================================
        'NAME           :fn_select_G_SCHMAST
        'Parameter      :
        'Description    :G_SCHMAST����o�͍��ڒ��o
        'Return         :True=OK,False=NG
        'Create         :2005/06/16
        'UPDATE         :
        '============================================================================
        fn_select_G_SCHMAST = False

        Dim SQL As New StringBuilder(128)

        SQL.Append(" SELECT ")
        SQL.Append(" COUNT(GAKKOU_CODE_S)")
        SQL.Append(",SUM(SYORI_KEN_S)")
        SQL.Append(",SUM(SYORI_KIN_S)")
        SQL.Append(",SUM(FURI_KEN_S)")
        SQL.Append(",SUM(FURI_KIN_S)")
        SQL.Append(",SUM(FUNOU_KEN_S)")
        SQL.Append(",SUM(FUNOU_KIN_S)")
        SQL.Append(",SUM(TESUU_KIN_S)")
        SQL.Append(",SUM(TESUU_KIN1_S)")
        SQL.Append(",SUM(TESUU_KIN2_S)")
        SQL.Append(",SUM(TESUU_KIN3_S)")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE KESSAI_FLG_S = '1'")
        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND TIME_STAMP_S = '" & sSyori_Date & "'")
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '�Ǎ��̂�


        While (gdbrREADER.Read)
            lngSAKI_SUU = CLng(gdbrREADER.Item(0))
            lngALL_KEN = CLng(gdbrREADER.Item(1))
            lngALL_KIN = CLng(gdbrREADER.Item(2))
            lngFURI_ALL_KEN = CLng(gdbrREADER.Item(3))
            lngFURI_ALL_KIN = CLng(gdbrREADER.Item(4))
            lngFUNOU_ALL_KEN = CLng(gdbrREADER.Item(5))
            lngFUNOU_ALL_KIN = CLng(gdbrREADER.Item(6))
            'lngTESUU_ALL = CLng(gdbrREADER.Item(7))
            lngTESUU1 = CLng(gdbrREADER.Item(8))
            'lngTESUU2 = CLng(gdbrREADER.Item(9)) '�X����
            'lngTESUU3 = CLng(gdbrREADER.Item(10)) '�U���萔��

        End While

        If Err.Number = 0 Then
            fn_select_G_SCHMAST = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_SCHMAST = False
            MessageBox.Show("�������ɃG���[���������܂���" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_select_G_SCHMAST_NYU() As Boolean
        '============================================================================
        'NAME           :fn_select_G_SCHMAST_NYU
        'Parameter      :
        'Description    :G_SCHMAST��������̃X�P�W���[�����o
        'Return         :True=OK,False=NG
        'Create         :2005/06/28
        'UPDATE         :
        '============================================================================
        fn_select_G_SCHMAST_NYU = False

        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT ")
        SQL.Append(" * ")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE KESSAI_FLG_S = '0'")
        SQL.Append(" AND TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND DATA_FLG_S = '1'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND FURI_KBN_S = '2'")
        SQL.Append(" AND SYORI_KIN_S > 0 ")
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC ")


        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '�Ǎ��̂�

        lngNYUKIN_SCH_CNT = 0

        While (gdbrREADER.Read)
            lngNYUKIN_SCH_CNT = lngNYUKIN_SCH_CNT + 1

        End While

        If Err.Number = 0 Then
            fn_select_G_SCHMAST_NYU = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_SCHMAST_NYU = False
            MessageBox.Show("�������ɃG���[���������܂���" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_select_G_PRTKESSAI() As Boolean
        '============================================================================
        'NAME           :fn_select_G_PRTKESSAI
        'Parameter      :
        'Description    :G_PRTKESSAI�̑��݃`�F�b�N
        'Return         :True=OK,False=NG
        'Create         :2006/04/17
        'UPDATE         :2006/04/18 �U�֓��𒊏o�����ɒǉ�
        '============================================================================
        fn_select_G_PRTKESSAI = False
        Dim SQL As New StringBuilder(128)
        SQL.Append(" SELECT * FROM G_PRTKESSAI")
        SQL.Append(" WHERE GAKKOU_CODE_P = '" & Str_Prt_GAKKOU_CODE & "'")
        SQL.Append(" AND KESSAI_DATE_P = '" & Str_Prt_KESSAI_DATE & "'")
        SQL.Append(" AND KESSAI_KIN_CODE_P = '" & Str_Prt_KESSAI_KIN_CODE & "'")
        SQL.Append(" AND KESSAI_TENPO_P = '" & Str_Prt_KESSAI_TENPO & "'")
        SQL.Append(" AND KESSAI_KAMOKU_P = '" & Str_Prt_KESSAI_KAMOKU & "'")
        SQL.Append(" AND KESSAI_KOUZA_P = '" & Str_Prt_KESSAI_KOUZA & "'")
        SQL.Append(" AND FURI_DATE_P = '" & Str_Prt_FURI_DATE & "'")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '�Ǎ��̂�

        lngG_PRTKESSAI_REC = 0
        While (gdbrREADER.Read)
            lngG_PRTKESSAI_REC = lngG_PRTKESSAI_REC + 1
        End While

        If Err.Number = 0 Then
            fn_select_G_PRTKESSAI = True
            gdbcCONNECT.Close()
        Else
            fn_select_G_PRTKESSAI = False
            MessageBox.Show("�������ɃG���[���������܂���" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Function fn_COUNT_G_SCHMAST(ByVal astrGAKKOU_CODE As String) As Boolean
        '============================================================================
        'NAME           :fn_COUNT_G_SCHMAST
        'Parameter      :astrGAKKOU_CODE:�w�Z�R�[�h
        'Description    :G_SCHMAST����o�͍��ڒ��o
        'Return         :True=OK,False=NG
        'Create         :2006/04/17
        'UPDATE         :
        '============================================================================
        Dim lngYUUSOU_CNT As Long

        fn_COUNT_G_SCHMAST = False

        Dim SQL As New StringBuilder(128)

        SQL.Append("SELECT SYORI_KEN_S,SYORI_KIN_S")
        SQL.Append(" ,FURI_KEN_S,FURI_KIN_S,FUNOU_KEN_S,FUNOU_KIN_S")
        SQL.Append(" ,TESUU_KIN_S,TESUU_KIN1_S,TESUU_KIN2_S,TESUU_KIN3_S")
        SQL.Append(" FROM G_SCHMAST")
        SQL.Append(" WHERE TYUUDAN_FLG_S = '0'")
        SQL.Append(" AND KESSAI_YDATE_S = '" & STR_FURIKAE_DATE(1) & "'")
        SQL.Append(" AND GAKKOU_CODE_S = '" & astrGAKKOU_CODE & "'")
        SQL.Append(" AND SCH_KBN_S <> '2'") '2007/02/10
        SQL.Append(" ORDER BY GAKKOU_CODE_S ASC")

        gdbcCONNECT.ConnectionString = STR_CONNECTION
        gdbcCONNECT.Open()

        gdbCOMMAND = New OracleClient.OracleCommand
        gdbCOMMAND.CommandText = SQL.ToString
        gdbCOMMAND.Connection = gdbcCONNECT
        gdbrREADER = gdbCOMMAND.ExecuteReader   '�Ǎ��̂�

        lngSYORIKEN_TOKUBETU = 0
        dblSYORIKIN_TOKUBETU = 0
        lngFURIKEN_TOKUBETU = 0
        dblFURIKIN_TOKUBETU = 0
        lngFUNOUKEN_TOKUBETU = 0
        dblFUNOUKIN_TOKUBETU = 0
        dblTESUU_TOKUBETU = 0
        dblTESUU_A1_TOKUBETU = 0
        dblTESUU_A2_TOKUBETU = 0
        dblTESUU_A3_TOKUBETU = 0
        lngYUUSOU_CNT = 0

        While (gdbrREADER.Read)
            lngSYORIKEN_TOKUBETU = lngSYORIKEN_TOKUBETU + CLng(gdbrREADER.Item(0))
            dblSYORIKIN_TOKUBETU = dblSYORIKIN_TOKUBETU + CDbl(gdbrREADER.Item(1))
            lngFURIKEN_TOKUBETU = lngFURIKEN_TOKUBETU + CLng(gdbrREADER.Item(2))
            dblFURIKIN_TOKUBETU = dblFURIKIN_TOKUBETU + CDbl(gdbrREADER.Item(3))
            lngFUNOUKEN_TOKUBETU = lngFUNOUKEN_TOKUBETU + CLng(gdbrREADER.Item(4))
            dblFUNOUKIN_TOKUBETU = dblFUNOUKIN_TOKUBETU + CDbl(gdbrREADER.Item(5))

            dblTESUU_TOKUBETU = dblTESUU_TOKUBETU + CDbl(gdbrREADER.Item(6))
            dblTESUU_A1_TOKUBETU = dblTESUU_A1_TOKUBETU + CDbl(gdbrREADER.Item(7))
            dblTESUU_A2_TOKUBETU = dblTESUU_A2_TOKUBETU + CDbl(gdbrREADER.Item(8))
            dblTESUU_A3_TOKUBETU = dblTESUU_A3_TOKUBETU + CDbl(gdbrREADER.Item(9))
            lngYUUSOU_CNT += 1
        End While
        '�X�����͈�U�֓��ň����̂ŁA�X�P�W���[�����Ŋ��������̂��X����
        dblTESUU_A2_TOKUBETU = dblTESUU_A2_TOKUBETU / lngYUUSOU_CNT

        If Err.Number = 0 Then
            fn_COUNT_G_SCHMAST = True
            gdbcCONNECT.Close()
        Else
            fn_COUNT_G_SCHMAST = False
            MessageBox.Show("�������ɃG���[���������܂���" & vbCrLf & CStr(Err.Number) & " :" & Err.Description, STR_SYORI_NAME, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            gdbcCONNECT.Close()
        End If

    End Function

    Private Function PFUNC_UPDATE_PrtKessai(ByVal astrGAKKOU_CODE As String, ByVal alngFURI_TESUU As Long) As Boolean

        PFUNC_UPDATE_PrtKessai = False

        STR_SQL = " UPDATE  G_PRTKESSAI SET "
        STR_SQL += " FURIKOMI_TESUU_P = " & alngFURI_TESUU
        STR_SQL += " WHERE"
        STR_SQL += " GAKKOU_CODE_P  = '" & astrGAKKOU_CODE & "'"

        If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS(STR_SQL, 1) = False Then
            Exit Function
        End If

        If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
            Exit Function
        End If

        PFUNC_UPDATE_PrtKessai = True

    End Function

#End Region

End Class
