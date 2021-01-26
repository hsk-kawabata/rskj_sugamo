' スケジュールの日数計算専用クラス

' 共通モジュール
Public Module Comm
    Public GCom As MenteCommon.clsCommon
End Module

' メインクラス
Public Class ClsMain
    Dim CLS As New ClsSchduleMaintenanceClass

    Public SCHData As ClsSchduleMaintenanceClass.SCHMAST_Data

    Public Sub New(ByVal toriSCode As String, ByVal toriFCode As String, ByVal furiDate As String)
        GCom = New MenteCommon.clsCommon

        '休日情報の蓄積
        Call CLS.SetKyuzituInformation()

        'SCHMAST項目名の蓄積
        Call CLS.SetSchMastInformation()

        '取引先マスタに取引先コードが存在することを確認
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
