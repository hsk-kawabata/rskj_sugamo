' �X�P�W���[���̓����v�Z��p�N���X

' ���ʃ��W���[��
Public Module Comm
    Public GCom As MenteCommon.clsCommon
End Module

' ���C���N���X
Public Class ClsMain
    Dim CLS As New ClsSchduleMaintenanceClass

    Public SCHData As ClsSchduleMaintenanceClass.SCHMAST_Data

    Public Sub New(ByVal toriSCode As String, ByVal toriFCode As String, ByVal furiDate As String)
        GCom = New MenteCommon.clsCommon

        '�x�����̒~��
        Call CLS.SetKyuzituInformation()

        'SCHMAST���ږ��̒~��
        Call CLS.SetSchMastInformation()

        '�����}�X�^�Ɏ����R�[�h�����݂��邱�Ƃ��m�F
        Dim BRet As Boolean
        BRet = CLS.GET_SELECT_TORIMAST(Nothing, toriSCode, toriFCode)
        If Not BRet Then
            Return
        End If

        CLS.SCH.KFURI_DATE = furiDate
        CLS.SCH.FURI_DATE = furiDate

        If CLS.INSERT_NEW_SCHMAST(0, False, True) = True Then
            SCHData = CLS.SCH
        End If

        Return
    End Sub

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub
End Class
