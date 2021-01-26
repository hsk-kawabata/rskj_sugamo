Option Explicit On 
Option Strict Off

Imports System.Text

Public Class KFGMAST061

    Private ReadOnly ThisModuleName As String = "KFGMAST061"  'モジュール名

    Private ERRMSG As String = "" '共通エラーメッセージ
    Private StrYasumi_List(0) As String '休日情報格納配列
    Private MainDB As CASTCommon.MyOracle = Nothing

    Private Enum gintKEKKA As Integer
        OK = 0
        NG = 1
        OTHER = 2
    End Enum
    Private Structure LogWrite
        Dim UserID As String            'ユーザID
        Dim ToriCode As String          '取引先主副コード
        Dim FuriDate As String          '振替日
    End Structure

    Private LW As LogWrite

    Private MainLOG As New CASTCommon.BatchLOG("KFGMAST061", "月間スケジュール作成画面")
    Private Const msgTitle As String = "月間スケジュール作成画面(KFGMAST061)"
#Region "構造体"
    '*****学校情報格納用構造体*****
    Private Structure GAKDATA
        <VBFixedStringAttribute(10)> Public GAKKOU_CODE As String '学校コード
        <VBFixedStringAttribute(50)> Public GAKKOU_NNAME As String '学校名漢字
        Public SIYOU_GAKUNEN As Integer '使用学年
        <VBFixedStringAttribute(2)> Public FURI_DATE As String '振替日
        <VBFixedStringAttribute(2)> Public SFURI_DATE As String '再振日
        <VBFixedStringAttribute(1)> Public BAITAI_CODE As String '媒体コード
        <VBFixedStringAttribute(10)> Public ITAKU_CODE As String '委託者コード
        <VBFixedStringAttribute(4)> Public TKIN_CODE As String '金融機関コード
        <VBFixedStringAttribute(3)> Public TSIT_CODE As String '支店コード
        <VBFixedStringAttribute(1)> Public SFURI_SYUBETU As String '再振種別コード

        <VBFixedStringAttribute(1)> Public NKYU_CODE_T As String '入金休日コード
        <VBFixedStringAttribute(1)> Public SKYU_CODE_T As String '出金休日コード
        <VBFixedStringAttribute(6)> Public KAISI_DATE As String '開始日
        <VBFixedStringAttribute(6)> Public SYURYOU_DATE As String '終了日
        <VBFixedStringAttribute(1)> Public TESUUTYO_KBN As String '手数料徴求区分
        <VBFixedStringAttribute(1)> Public TESUUTYO_KIJITSU As String '手数料徴求期日
        Public TESUUTYO_NO As Integer
        <VBFixedStringAttribute(1)> Public TESUU_KYU_CODE As String '手数料休日コード
        <VBFixedStringAttribute(6)> Public TAISYOU_START_NENDO As String '対象開始年度
        <VBFixedStringAttribute(6)> Public TAISYOU_END_NENDO As String '対象終了年度

        Sub New(ByVal strGakkouCode As String, ByRef Flg As Boolean, ByVal db As CASTCommon.MyOracle)

            Dim Orareader As CASTCommon.MyOracleReader = Nothing
            '参照フラグ
            Flg = False

            Try
                Orareader = New CASTCommon.MyOracleReader(db)

                Dim SQL As New StringBuilder(1024)

                SQL.Append(" SELECT ")
                SQL.Append(" GAKKOU_NNAME_G ")         '学校名漢字
                SQL.Append(",SIYOU_GAKUNEN_T ")        '使用学年数
                SQL.Append(",FURI_DATE_T ")            '振替日
                SQL.Append(",SFURI_DATE_T ")           '再振日
                SQL.Append(",BAITAI_CODE_T ")          '媒体コード
                SQL.Append(",ITAKU_CODE_T ")           '委託者コード
                SQL.Append(",TKIN_NO_T ")              '金融機関コード
                SQL.Append(",TSIT_NO_T ")              '支店コード
                SQL.Append(",SFURI_SYUBETU_T ")        '再振種別
                SQL.Append(",NKYU_CODE_T ")            '入金休日コード
                SQL.Append(",SKYU_CODE_T ")            '出金休日コード
                SQL.Append(",KAISI_DATE_T ")           '開始日
                SQL.Append(",SYURYOU_DATE_T ")         '終了日
                SQL.Append(",TESUUTYO_KBN_T ")         '手数料徴求区分
                SQL.Append(",TESUUTYO_KIJITSU_T ")     '手数料徴求期日区分
                SQL.Append(",TESUUTYO_DAY_T ")          '手数料徴求日数
                SQL.Append(",TESUU_KYU_CODE_T ")       '手数料徴求
                SQL.Append(" FROM GAKMAST1,GAKMAST2 ")
                SQL.Append(" WHERE GAKKOU_CODE_G = GAKKOU_CODE_T ")
                SQL.Append(" AND GAKUNEN_CODE_G = 1 ")
                SQL.Append(" AND GAKKOU_CODE_G = '" & strGakkouCode & "'")

                If Orareader.DataReader(SQL) = True Then
                    With Orareader
                        GAKKOU_CODE = strGakkouCode                       '学校コード
                        GAKKOU_NNAME = .GetItem("GAKKOU_NNAME_G")         '学校名漢字
                        SIYOU_GAKUNEN = CInt(.GetItem("SIYOU_GAKUNEN_T")) '使用学年数
                        FURI_DATE = .GetItem("FURI_DATE_T")               '振替日
                        SFURI_DATE = .GetItem("SFURI_DATE_T")             '再振日
                        BAITAI_CODE = .GetItem("BAITAI_CODE_T")           '媒体コード
                        ITAKU_CODE = .GetItem("ITAKU_CODE_T")             '委託者コード
                        TKIN_CODE = .GetItem("TKIN_NO_T")                 '取扱金融機関コード
                        TSIT_CODE = .GetItem("TSIT_NO_T")                 '取扱支店コード
                        SFURI_SYUBETU = .GetItem("SFURI_SYUBETU_T")       '再振種別
                        NKYU_CODE_T = .GetItem("NKYU_CODE_T")             '入金休日コード
                        SKYU_CODE_T = .GetItem("SKYU_CODE_T")             '出金休日コード
                        KAISI_DATE = .GetItem("KAISI_DATE_T")             '自振開始年月
                        SYURYOU_DATE = .GetItem("SYURYOU_DATE_T")         '自振終了年月
                        TESUUTYO_KIJITSU = .GetItem("TESUUTYO_KIJITSU_T") '手数料徴求期日区分
                        TESUUTYO_NO = CInt(.GetItem("TESUUTYO_DAY_T"))     '手数料徴求日数
                        TESUUTYO_KBN = .GetItem("TESUUTYO_KBN_T")         '手数料徴求区分
                        TESUU_KYU_CODE = .GetItem("TESUU_KYU_CODE_T")     '決済休日コード
                    End With

                    '取得成功
                    Flg = True
                End If

                Orareader.Close()
                Orareader = Nothing

            Catch ex As Exception
                With GCom.GLog
                    .Job2 = "スケジュール取得"
                    .Result = "失敗"
                    .Discription = "予期せぬエラー:" & ex.ToString
                End With
                GCom.FN_LOG_WRITE("GFJMAST0602G", New StackTrace(True))
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                End If
            End Try

        End Sub
    End Structure
    '*****学校情報格納用構造体****

    '***学校スケジュールマスタデータ保持構造体***
    Private Structure G_SCHMASTDATA
        <VBFixedString(10)> Public GAKKOU_CODE_S As String    '1.学校コード
        <VBFixedString(6)> Public NENGETUDO_S As String      '2.年月度
        <VBFixedString(1)> Public SCH_KBN_S As String        '3.スケジュール区分
        <VBFixedString(1)> Public FURI_KBN_S As String       '4.振替区分
        <VBFixedString(8)> Public FURI_DATE_S As String      '5.振替日
        <VBFixedString(8)> Public SFURI_DATE_S As String     '6.再振日
        '対象学年フラグ
        <VBFixedString(1)> Public GAKUNEN1_FLG_S As String   '7.1年
        <VBFixedString(1)> Public GAKUNEN2_FLG_S As String   '8.2年
        <VBFixedString(1)> Public GAKUNEN3_FLG_S As String   '9.3年
        <VBFixedString(1)> Public GAKUNEN4_FLG_S As String   '10.4年
        <VBFixedString(1)> Public GAKUNEN5_FLG_S As String   '11.5年
        <VBFixedString(1)> Public GAKUNEN6_FLG_S As String   '12.6年
        <VBFixedString(1)> Public GAKUNEN7_FLG_S As String   '13.7年
        <VBFixedString(1)> Public GAKUNEN8_FLG_S As String   '14.8年
        <VBFixedString(1)> Public GAKUNEN9_FLG_S As String   '15.9年

        <VBFixedString(10)> Public ITAKU_CODE_S As String    '16.委託者コード
        <VBFixedString(4)> Public TKIN_NO_S As String        '17.金融機関コード
        <VBFixedString(3)> Public TSIT_NO_S As String        '18.支店コード
        <VBFixedString(10)> Public BAITAI_CODE_S As String   '19.媒体コード
        <VBFixedString(1)> Public TESUU_KBN_S As String      '20.手数料徴求区分
        '各予定日、処理日
        <VBFixedString(8)> Public ENTRI_YDATE_S As String    '21.エントリ予定日
        <VBFixedString(8)> Public ENTRI_DATE_S As String     '22.エントリ日
        <VBFixedString(8)> Public CHECK_YDATE_S As String    '23.チェック予定日
        <VBFixedString(8)> Public CHECK_DATE_S As String     '24.チェック日
        <VBFixedString(8)> Public DATA_YDATE_S As String     '25.データ予定日
        <VBFixedString(8)> Public DATA_DATE_S As String      '26.データ日
        <VBFixedString(8)> Public FUNOU_YDATE_S As String    '27.不能更新予定日
        <VBFixedString(8)> Public FUNOU_DATE_S As String     '28.不能更新日
        '<VBFixedString(8)> Public HENKAN_YDATE_S As String   '29.返還予定日
        '<VBFixedString(8)> Public HENKAN_DATE_S As String    '30.返還日
        <VBFixedString(8)> Public KESSAI_YDATE_S As String   '31.決済予定日
        <VBFixedString(8)> Public KESSAI_DATE_S As String    '32.決済日
        '処理フラグ
        <VBFixedString(1)> Public ENTRI_FLG_S As String      '33.エントリフラグ
        <VBFixedString(1)> Public CHECK_FLG_S As String      '34.チェックフラグ
        <VBFixedString(1)> Public DATA_FLG_S As String       '35.データフラグ
        <VBFixedString(1)> Public FUNOU_FLG_S As String      '36.不能フラグ
        '<VBFixedString(1)> Public HENKAN_FLG_S As String     '37.返還フラグ
        <VBFixedString(1)> Public SAIFURI_FLG_S As String    '38.再振フラグ
        <VBFixedString(1)> Public KESSAI_FLG_S As String     '39.決済フラグ
        <VBFixedString(1)> Public TYUUDAN_FLG_S As String    '40.中断フラグ
        '件数、金額
        Public SYORI_KEN_S As Long                           '41.処理件数
        Public SYORI_KIN_S As Long                           '42.処理金額
        Public TESUU_KIN_S As Long                           '43.手数料金額
        Public TESUU_KIN1_S As Long                          '44.手数料金額1
        Public TESUU_KIN2_S As Long                          '45.手数料金額2
        Public TESUU_KIN3_S As Long                          '46.手数料金額3
        Public FURI_KEN_S As Long                            '47.振替件数
        Public FURI_KIN_S As Long                            '48.振替金額

        <VBFixedString(8)> Public SAKUSEI_DATE_S As String   '49.作成日
        <VBFixedString(16)> Public TIME_STAMP_S As String    '50.タイムスタンプ
        <VBFixedString(15)> Public YOBI1_S As String         '51.予備1
        <VBFixedString(15)> Public YOBI2_S As String         '52.予備2
        <VBFixedString(15)> Public YOBI3_S As String         '53.予備3
        <VBFixedString(15)> Public YOBI4_S As String         '54.予備4
        <VBFixedString(15)> Public YOBI5_S As String         '55.予備5

        '
        '　関数名　-　SetG_SCHMASTDATA
        '
        '　機能    -  学校スケジュール取得
        '
        '　引数    -  aGakkouCode、aSchKbn、aFuriKbn、aFuriDate、aNengetudo
        '
        '　備考    -  
        '
        '　
        Public Function SetG_SCHMASTDATA(ByVal aGakkouCode As String, _
                                        ByVal aSchKbn As String, _
                                        ByVal aFuriKbn As String, _
                                        ByVal aFuriDate As String, _
                                        ByVal db As CASTCommon.MyOracle, _
                                        Optional ByVal aNengetudo As String = Nothing) As Boolean

            Dim ret As Boolean = False

            Dim Orareader As CASTCommon.MyOracleReader = Nothing

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "スケジュール情報取得"
                STR_LOG_GAKKOU_CODE = aGakkouCode
                STR_LOG_FURI_DATE = aFuriDate
                '***ログ設定***

                Orareader = New CASTCommon.MyOracleReader(db)

                Dim SQL As New StringBuilder(128)

                SQL.Append(" SELECT * FROM G_SCHMAST ")
                SQL.Append(" WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'")
                SQL.Append(" AND   SCH_KBN_S = '" & aSchKbn & "'")
                SQL.Append(" AND   FURI_KBN_S = '" & aFuriKbn & "'")
                SQL.Append(" AND   FURI_DATE_S = '" & aFuriDate & "'")
                If aNengetudo <> Nothing Then
                    SQL.Append(" AND   NENGETUDO_S = '" & aNengetudo & "'")
                End If

                If Orareader.DataReader(SQL) = True Then
                    '基本情報
                    GAKKOU_CODE_S = Orareader.GetItem("GAKKOU_CODE_S").Trim    '1.学校コード
                    NENGETUDO_S = Orareader.GetItem("NENGETUDO_S").Trim        '2.年月度
                    SCH_KBN_S = Orareader.GetItem("SCH_KBN_S").Trim            '3.スケジュール区分
                    FURI_KBN_S = Orareader.GetItem("FURI_KBN_S").Trim          '4.振替区分
                    FURI_DATE_S = Orareader.GetItem("FURI_DATE_S").Trim        '5.振替日
                    SFURI_DATE_S = Orareader.GetItem("SFURI_DATE_S").Trim      '6.再振日
                    '対象学年フラグ
                    GAKUNEN1_FLG_S = Orareader.GetItem("GAKUNEN1_FLG_S").Trim  '7.1年
                    GAKUNEN2_FLG_S = Orareader.GetItem("GAKUNEN2_FLG_S").Trim  '8.2年
                    GAKUNEN3_FLG_S = Orareader.GetItem("GAKUNEN3_FLG_S").Trim  '9.3年
                    GAKUNEN4_FLG_S = Orareader.GetItem("GAKUNEN4_FLG_S").Trim  '10.4年
                    GAKUNEN5_FLG_S = Orareader.GetItem("GAKUNEN5_FLG_S").Trim  '11.5年
                    GAKUNEN6_FLG_S = Orareader.GetItem("GAKUNEN6_FLG_S").Trim  '12.6年
                    GAKUNEN7_FLG_S = Orareader.GetItem("GAKUNEN7_FLG_S").Trim  '13.7年
                    GAKUNEN8_FLG_S = Orareader.GetItem("GAKUNEN8_FLG_S").Trim  '14.8年
                    GAKUNEN9_FLG_S = Orareader.GetItem("GAKUNEN9_FLG_S").Trim  '15.9年
                    '個別情報
                    ITAKU_CODE_S = Orareader.GetItem("ITAKU_CODE_S").Trim      '16.委託者コード
                    TKIN_NO_S = Orareader.GetItem("TKIN_NO_S").Trim            '17.金融機関コード
                    TSIT_NO_S = Orareader.GetItem("TSIT_NO_S").Trim            '18.支店コード
                    BAITAI_CODE_S = Orareader.GetItem("BAITAI_CODE_S").Trim    '19.媒体コード
                    TESUU_KBN_S = Orareader.GetItem("TESUU_KBN_S").Trim        '20.手数料徴求区分
                    '各日付
                    ENTRI_YDATE_S = Orareader.GetItem("ENTRI_YDATE_S").Trim    '21.エントリ予定日
                    ENTRI_DATE_S = Orareader.GetItem("ENTRI_DATE_S").Trim      '22.エントリ日
                    CHECK_YDATE_S = Orareader.GetItem("CHECK_YDATE_S").Trim    '23.チェック予定日
                    CHECK_DATE_S = Orareader.GetItem("CHECK_DATE_S").Trim      '24.チェック日
                    DATA_YDATE_S = Orareader.GetItem("DATA_YDATE_S").Trim      '25.データ予定日
                    DATA_DATE_S = Orareader.GetItem("DATA_DATE_S").Trim        '26.データ日
                    FUNOU_YDATE_S = Orareader.GetItem("FUNOU_YDATE_S").Trim    '27.不能更新予定日
                    FUNOU_DATE_S = Orareader.GetItem("FUNOU_DATE_S").Trim      '28.不能更新日
                    'HENKAN_YDATE_S = Orareader.GetItem("HENKAN_YDATE_S").Trim  '29.返還予定日
                    'HENKAN_DATE_S = Orareader.GetItem("HENKAN_DATE_S").Trim    '30.返還日
                    KESSAI_YDATE_S = Orareader.GetItem("KESSAI_YDATE_S").Trim  '31.決済予定日
                    KESSAI_DATE_S = Orareader.GetItem("KESSAI_DATE_S").Trim    '32.決済日
                    '処理フラグ
                    ENTRI_FLG_S = Orareader.GetItem("ENTRI_FLG_S").Trim        '33.エントリフラグ
                    CHECK_FLG_S = Orareader.GetItem("CHECK_FLG_S").Trim        '34.チェックフラグ
                    DATA_FLG_S = Orareader.GetItem("DATA_FLG_S").Trim          '35.データフラグ
                    FUNOU_FLG_S = Orareader.GetItem("FUNOU_FLG_S").Trim        '36.不能フラグ
                    'HENKAN_FLG_S = Orareader.GetItem("HENKAN_FLG_S").Trim      '37.返還フラグ
                    SAIFURI_FLG_S = Orareader.GetItem("SAIFURI_FLG_S").Trim    '38.再振フラグ
                    KESSAI_FLG_S = Orareader.GetItem("KESSAI_FLG_S").Trim      '39.決済フラグ
                    TYUUDAN_FLG_S = Orareader.GetItem("TYUUDAN_FLG_S").Trim    '40.中断フラグ
                    '件数、金額
                    SYORI_KEN_S = Orareader.GetItem("SYORI_KEN_S").Trim        '41.処理件数
                    SYORI_KIN_S = Orareader.GetItem("SYORI_KIN_S").Trim        '42.処理金額
                    TESUU_KIN_S = Orareader.GetItem("TESUU_KIN_S").Trim        '43.手数料金額
                    TESUU_KIN1_S = Orareader.GetItem("TESUU_KIN1_S").Trim      '44.手数料金額1
                    TESUU_KIN2_S = Orareader.GetItem("TESUU_KIN2_S").Trim      '45.手数料金額2
                    TESUU_KIN3_S = Orareader.GetItem("TESUU_KIN3_S").Trim      '46.手数料金額3
                    FURI_KEN_S = Orareader.GetItem("FURI_KEN_S").Trim          '47.振替件数
                    FURI_KIN_S = Orareader.GetItem("FURI_KIN_S").Trim          '48.振替金額
                    '作成日付
                    SAKUSEI_DATE_S = Orareader.GetItem("SAKUSEI_DATE_S").Trim  '49.作成日
                    TIME_STAMP_S = Orareader.GetItem("TIME_STAMP_S").Trim      '50.タイムスタンプ
                    '予備
                    YOBI1_S = Orareader.GetItem("YOBI1_S").Trim                '51.予備1(入力日付)
                    YOBI2_S = Orareader.GetItem("YOBI2_S").Trim                '52.予備2
                    YOBI3_S = Orareader.GetItem("YOBI3_S").Trim                '53.予備3
                    YOBI4_S = Orareader.GetItem("YOBI4_S").Trim                '54.予備4
                    YOBI5_S = Orareader.GetItem("YOBI5_S").Trim                '55.予備5

                    ret = True

                End If

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            Finally
                If Not Orareader Is Nothing Then
                    Orareader.Close()
                End If
            End Try

            Return ret

        End Function

        '
        '　関数名　-　GetGakunenFlg
        '
        '　機能    -  マスタの学年情報を配列で返す
        '
        '　引数    -  
        '
        '　備考    -  
        '
        Public Function GetSchMastGakunenFlg() As Boolean()

            Dim ret As Boolean() = {False, False, False, False, False, False, False, False, False, False}

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "学年フラグ取得"
                STR_LOG_GAKKOU_CODE = GAKKOU_CODE_S
                STR_LOG_FURI_DATE = ""
                '***ログ設定***

                If GAKUNEN1_FLG_S = "1" Then
                    ret(1) = True
                End If

                If GAKUNEN2_FLG_S = "1" Then
                    ret(2) = True
                End If

                If GAKUNEN3_FLG_S = "1" Then
                    ret(3) = True
                End If

                If GAKUNEN4_FLG_S = "1" Then
                    ret(4) = True
                End If

                If GAKUNEN5_FLG_S = "1" Then
                    ret(5) = True
                End If

                If GAKUNEN6_FLG_S = "1" Then
                    ret(6) = True
                End If

                If GAKUNEN7_FLG_S = "1" Then
                    ret(7) = True
                End If

                If GAKUNEN8_FLG_S = "1" Then
                    ret(8) = True
                End If

                If GAKUNEN9_FLG_S = "1" Then
                    ret(9) = True
                End If

                ret(0) = True

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            End Try

            Return ret

        End Function

    End Structure
    '***学校スケジュールマスタデータ保持構造体***

    '*****通常振替日画面情報格納用構造体*****
    Private Structure TuujyouData
        Public TaisyouFlg As Boolean '作成、更新、参照、削除の対象フラグ

        Public SyoriFurikae_Flag As Boolean '処理振替フラグ(スケジュールが処理中)
        Public CheckFurikae_Flag As Boolean 'チェック振替フラグ
        Public FunouFurikae_Flag As Boolean '不能振替フラグ

        Public SyoriSaiFurikae_Flag As Boolean '処理再振替フラグ(再振スケジュールが処理中)
        Public CheckSaiFurikae_Flag As Boolean 'チェック再振替フラグ

        '***画面入力情報***
        <VBFixedStringAttribute(4)> Public Seikyu_Nen As String '請求年
        <VBFixedStringAttribute(2)> Public Seikyu_Tuki As String '請求月

        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String '振替月
        <VBFixedStringAttribute(2)> Public Furikae_Date As String '振替日
        Public Furikae_Check As Boolean
        Public Furikae_Enabled As Boolean

        <VBFixedStringAttribute(2)> Public SaiFurikae_Tuki As String '再振月
        <VBFixedStringAttribute(2)> Public SaiFurikae_Date As String '再振日
        Public SaiFurikae_Check As Boolean
        Public SaiFurikae_Enabled As Boolean

        '処理学年フラグ
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
        '***画面入力情報***

        <VBFixedStringAttribute(10)> Public Furikae_Day As String 'yyyy/mm/dd
        <VBFixedStringAttribute(10)> Public SaiFurikae_Day As String 'yyyy/mm/dd
        '
        '　関数名　-　fn_GetEigyoubi
        '
        '　機能    -  通常振替構造体用営業日の取得
        '
        '　戻り値  -  配列(0) 初振の営業日補正後を取得、配列(1) 再振の営業日補正後を取得
        '
        '　備考    -  
        '
        Public Function fn_EigyoubiHosei(ByVal aInfoGakkou As GAKDATA, ByRef HoseiEigyoubi() As String) As Boolean

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "営業日取得"
                STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
                STR_LOG_FURI_DATE = ""
                '***ログ設定***

                Dim WorkDate As String = ""

                '請求月が空白の場合・振替しない場合、取得する必要なし
                If Seikyu_Tuki = "" OrElse Furikae_Check = False Then
                    HoseiEigyoubi(0) = ""
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                If Furikae_Date.Trim = "" AndAlso SaiFurikae_Date.Trim = "" Then
                    HoseiEigyoubi(0) = ""
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                '初振営業日補正
                HoseiEigyoubi(0) = fn_GetEigyoubi(Seikyu_Nen & Furikae_Tuki & Furikae_Date, "0", "+")

                '再振営業日補正
                If SaiFurikae_Date.Trim = "" Then
                    HoseiEigyoubi(1) = ""
                    Return True
                End If

                Select Case True
                    Case Furikae_Date < SaiFurikae_Date
                        '再振
                        HoseiEigyoubi(1) = fn_GetEigyoubi(Seikyu_Nen & Furikae_Tuki & SaiFurikae_Date, "0", "+")
                        Return True
                    Case Furikae_Date = SaiFurikae_Date
                        HoseiEigyoubi(0) = "err"
                        HoseiEigyoubi(1) = "err"
                        Return False
                    Case Furikae_Date > SaiFurikae_Date
                        '再振
                        If Furikae_Tuki = "12" Then
                            HoseiEigyoubi(1) = fn_GetEigyoubi(Format(CInt(Seikyu_Nen) + 1, "0000") & "01" & SaiFurikae_Date, "0", "+")
                            Return True
                        Else
                            HoseiEigyoubi(1) = fn_GetEigyoubi(Seikyu_Nen & Format(CInt(Furikae_Tuki) + 1, "00") & SaiFurikae_Date, "0", "+")
                            Return True
                        End If
                End Select

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
                HoseiEigyoubi(0) = "err"
                HoseiEigyoubi(1) = "err"
                Return False
            End Try

        End Function

        '
        '　関数名　-　GetGakunenFlg
        '
        '　機能    -  画面の学年情報を配列で返す
        '
        '　引数    -  
        '
        '　備考    -  
        '
        Public Function GetGakunenFlg() As String()

            Dim ret As String() = {"err", "0", "0", "0", "0", "0", "0", "0", "0", "0"}

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "画面情報取得"
                STR_LOG_GAKKOU_CODE = ""
                STR_LOG_FURI_DATE = ""
                '***ログ設定***

                If SiyouGakunenALL_Check = True Then
                    ret(1) = "1"
                    ret(2) = "1"
                    ret(3) = "1"
                    ret(4) = "1"
                    ret(5) = "1"
                    ret(6) = "1"
                    ret(7) = "1"
                    ret(8) = "1"
                    ret(9) = "1"
                Else
                    If SiyouGakunen1_Check = True Then
                        ret(1) = "1"
                    End If

                    If SiyouGakunen2_Check = True Then
                        ret(2) = "1"
                    End If

                    If SiyouGakunen3_Check = True Then
                        ret(3) = "1"
                    End If

                    If SiyouGakunen4_Check = True Then
                        ret(4) = "1"
                    End If

                    If SiyouGakunen5_Check = True Then
                        ret(5) = "1"
                    End If

                    If SiyouGakunen6_Check = True Then
                        ret(6) = "1"
                    End If

                    If SiyouGakunen7_Check = True Then
                        ret(7) = "1"
                    End If

                    If SiyouGakunen8_Check = True Then
                        ret(8) = "1"
                    End If

                    If SiyouGakunen9_Check = True Then
                        ret(9) = "1"
                    End If
                End If

                ret(0) = "ok"

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            End Try

            Return ret

        End Function

    End Structure
    Private Tuujyou_SchInfo(5) As TuujyouData
    Private Syoki_Tuujyou_SchInfo(5) As TuujyouData
    '*****通常振替日画面情報格納用構造体*****

    '*****随時振替日画面情報格納用構造体*****
    Private Structure ZuijiData
        Public TaisyouFlg As Boolean '作成、更新、参照、削除の対象フラグ

        <VBFixedStringAttribute(2)> Public Nyusyutu_Kbn As String '入出金区分

        <VBFixedStringAttribute(4)> Public Furikae_Nen As String '請求年
        <VBFixedStringAttribute(2)> Public Furikae_Tuki As String '振替月
        <VBFixedStringAttribute(2)> Public Furikae_Date As String '振替日
        Public Syori_Flag As Boolean

        '処理学年フラグ
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

        <VBFixedStringAttribute(10)> Public Furikae_Day As String 'yyyy/mm/dd

        '
        '　関数名　-　fn_GetEigyoubi
        '
        '　機能    -  随時振替構造体用営業日の取得
        '
        '　引数    -  
        '
        '　備考    -  
        '
        Public Function fn_GetEigyoubiZuiji(ByVal aInfoGakkou As GAKDATA) As String

            Dim StrReturnDate As String = "err"

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "営業日取得"
                STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
                STR_LOG_FURI_DATE = ""
                '***ログ設定***

                '請求月が空白の場合・振替しない場合、取得する必要なし
                If Furikae_Tuki = "" Then
                    Return ""
                End If

                '日付が空白だった場合、基準日を使用する

                If Furikae_Date = "" Then
                    Furikae_Date = aInfoGakkou.FURI_DATE
                End If

                '営業日を取得
                '通常スケジュール
                StrReturnDate = fn_GetEigyoubi(Furikae_Nen & Furikae_Tuki & Furikae_Date, "0", "+")

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
                StrReturnDate = "err"
            End Try

            Return StrReturnDate

        End Function

        '
        '　関数名　-　GetGakunenFlg
        '
        '　機能    -  画面の学年情報を配列で返す
        '
        '　引数    -  
        '
        '　備考    -  
        '
        Public Function GetGakunenFlg() As String()

            Dim ret As String() = {"err", "0", "0", "0", "0", "0", "0", "0", "0", "0"}

            Try
                '***ログ設定***
                STR_SYORI_NAME = "月間スケジュール作成"
                STR_COMMAND = "画面情報取得"
                STR_LOG_GAKKOU_CODE = ""
                STR_LOG_FURI_DATE = ""
                '***ログ設定***

                If SiyouGakunenALL_Check = True Then
                    ret(1) = "1"
                    ret(2) = "1"
                    ret(3) = "1"
                    ret(4) = "1"
                    ret(5) = "1"
                    ret(6) = "1"
                    ret(7) = "1"
                    ret(8) = "1"
                    ret(9) = "1"
                Else
                    If SiyouGakunen1_Check = True Then
                        ret(1) = "1"
                    End If

                    If SiyouGakunen2_Check = True Then
                        ret(2) = "1"
                    End If

                    If SiyouGakunen3_Check = True Then
                        ret(3) = "1"
                    End If

                    If SiyouGakunen4_Check = True Then
                        ret(4) = "1"
                    End If

                    If SiyouGakunen5_Check = True Then
                        ret(5) = "1"
                    End If

                    If SiyouGakunen6_Check = True Then
                        ret(6) = "1"
                    End If

                    If SiyouGakunen7_Check = True Then
                        ret(7) = "1"
                    End If

                    If SiyouGakunen8_Check = True Then
                        ret(8) = "1"
                    End If

                    If SiyouGakunen9_Check = True Then
                        ret(9) = "1"
                    End If
                End If

                ret(0) = "ok"

            Catch ex As Exception
                Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            End Try

            Return ret

        End Function
    End Structure
    Private Zuiji_SchInfo(6) As ZuijiData
    Private Syoki_Zuiji_SchInfo(6) As ZuijiData
    '*****随時振替日画面情報格納用構造体*****
#End Region

#Region "Form_Load"
    Private Sub KFGMAST061_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        Try
            '***ログ設定***
            STR_SYORI_NAME = "月間スケジュール作成"
            STR_COMMAND = "画面表示"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ設定***

            With Me
                .WindowState = FormWindowState.Normal
                .FormBorderStyle = FormBorderStyle.FixedDialog
                .ControlBox = True
            End With

            Call GCom.SetMonitorTopArea(Label1, Label2, lblUser, lblDate)
            MainLog = New CASTCommon.BatchLOG(Me.Name, STR_SYORI_NAME)

            '画面の初期化
            Call FormInitializa()

            MainDB = New CASTCommon.MyOracle

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try
    End Sub
#End Region
#Region "TextBox_Validating"

    Private Sub TextBox_Validating(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles txt対象年度.Validating, txt対象月.Validating, txtGakkou_Code.Validating, txt通常振替日1.Validating, txt通常振替日2.Validating, txt通常振替日3.Validating, txt通常振替日4.Validating, txt通常振替日5.Validating, txt通常再振日1.Validating, txt通常再振日2.Validating, txt通常再振日3.Validating, txt通常再振日4.Validating, txt通常再振日5.Validating, txt随時振替日1.Validating, txt随時振替日2.Validating, txt随時振替日3.Validating, txt随時振替日4.Validating, txt随時振替日5.Validating, txt随時振替日6.Validating

        Try
            '***ログ設定***
            STR_SYORI_NAME = "月間スケジュール作成"
            STR_COMMAND = "TextBox_Validating"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ設定***

            Select Case CType(sender, TextBox).Name
                Case "txt対象年度" '対象年度
                    Me.txt対象年度.BackColor = System.Drawing.Color.White

                    If Me.txt対象年度.Text.Trim <> "" Then
                        Me.txt対象年度.Text = Me.txt対象年度.Text.Trim.PadLeft(4, "0"c)
                    End If

                    '休日情報の表示
                    If fn_HolidayListSet() = False Then
                        Exit Sub
                    End If

                Case "txt対象月" '対象月

                    If Me.txt対象月.Text.Trim <> "" Then
                        Me.txt対象月.Text = Me.txt対象月.Text.Trim.PadLeft(2, "0"c)
                    End If

                    If Trim(txtGakkou_Code.Text) <> "" And Trim(txt対象年度.Text) <> "" And Trim(Me.txt対象月.Text) <> "" Then
                        '対象年度も入力されている場合、スケジュール存在チェックをかけ
                        'スケジュールが存在する場合は参照ボタンにフォーカス移動
                        Call Sb_Sansyou_Focus()
                    End If

                Case "txtGakkou_Code" '学校コード

                    Me.txtGakkou_Code.BackColor = System.Drawing.Color.White
                    '学校情報の取得
                    If txtGakkou_Code.Text.Trim <> "" Then

                        lbl使用学年.Text = "0"

                        '0埋め
                        Me.txtGakkou_Code.Text = Me.txtGakkou_Code.Text.PadLeft(10, "0"c)

                        Dim key As Boolean = False
                        Dim G_Info As New GAKDATA(txtGakkou_Code.Text.Trim, key, MainDB)

                        If key = True Then

                            Me.lab学校名.Text = G_Info.GAKKOU_NNAME.Trim
                            Me.lbl使用学年.Text = CStr(G_Info.SIYOU_GAKUNEN)

                            '最高学年以上の学年の使用不可
                            Call Sb_SiyouGakunenChkEnabled(G_Info.SIYOU_GAKUNEN)

                            '再振替日のプロテクト
                            Select Case G_Info.SFURI_SYUBETU
                                Case "0", "3"
                                    Call Sb_SaifuriProtect(False)
                                Case Else
                                    Call Sb_SaifuriProtect(True)
                            End Select

                            'スケジュールが存在する場合は参照ボタンにフォーカス移動
                            If Me.txt対象年度.Text.Trim <> "" And Trim(Me.txt対象月.Text) Then
                                Call Sb_Sansyou_Focus()
                            End If
                        Else
                            Me.lab学校名.Text = ""
                            Me.lbl使用学年.Text = "0"
                        End If
                    Else
                        Me.lab学校名.Text = ""
                        Me.lbl使用学年.Text = "0"
                    End If

                Case Else 'それ以外のテキストボックス
                    '0付加
                    If CType(sender, TextBox).Text.Trim <> "" Then
                        CType(sender, TextBox).Text = CType(sender, TextBox).Text.Trim.PadLeft(2, "0"c)
                    End If
            End Select

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try
    End Sub
#End Region
#Region "CheckedChanged(CheckBox)"
    'CheckBoxtチェック変更処理
    Private Sub Chk学年通常_全Changed(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk学年通常1_全.CheckedChanged, Chk学年通常2_全.CheckedChanged, Chk学年通常3_全.CheckedChanged, Chk学年通常4_全.CheckedChanged, Chk学年通常5_全.CheckedChanged, Chk随時1_全学年.CheckedChanged, Chk随時2_全学年.CheckedChanged, Chk随時3_全学年.CheckedChanged, Chk随時4_全学年.CheckedChanged, Chk随時5_全学年.CheckedChanged, Chk随時6_全学年.CheckedChanged


        Select Case CType(sender, CheckBox).Name
            Case "Chk学年通常1_全"  '***通常振替日タブ全学年チェックボックス***
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk学年通常1_全, _
                                                Chk学年通常1_1, Chk学年通常1_2, Chk学年通常1_3, _
                                                Chk学年通常1_4, _Chk学年通常1_5, Chk学年通常1_6, _
                                                Chk学年通常1_7, Chk学年通常1_8, Chk学年通常1_9)
            Case "Chk学年通常2_全"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk学年通常2_全, _
                                                Chk学年通常2_1, Chk学年通常2_2, Chk学年通常2_3, _
                                                Chk学年通常2_4, Chk学年通常2_5, Chk学年通常2_6, _
                                                Chk学年通常2_7, Chk学年通常2_8, Chk学年通常2_9)
            Case "Chk学年通常3_全"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk学年通常3_全, _
                                                Chk学年通常3_1, Chk学年通常3_2, Chk学年通常3_3, _
                                                Chk学年通常3_4, Chk学年通常3_5, Chk学年通常3_6, _
                                                Chk学年通常3_7, Chk学年通常3_8, Chk学年通常3_9)
            Case "Chk学年通常4_全"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk学年通常4_全, _
                                                Chk学年通常4_1, Chk学年通常4_2, Chk学年通常4_3, _
                                                Chk学年通常4_4, Chk学年通常4_5, Chk学年通常4_6, _
                                                Chk学年通常4_7, Chk学年通常4_8, Chk学年通常4_9)
            Case "Chk学年通常5_全"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk学年通常5_全, _
                                                Chk学年通常5_1, Chk学年通常5_2, Chk学年通常5_3, _
                                                Chk学年通常5_4, Chk学年通常5_5, Chk学年通常5_6, _
                                                Chk学年通常5_7, Chk学年通常5_8, Chk学年通常5_9)

            Case "Chk随時1_全学年" '***随時振替日タブ全学年チェックボックス***
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時1_全学年, _
                                                Chk随時1_1学年, Chk随時1_2学年, Chk随時1_3学年, _
                                                Chk随時1_4学年, Chk随時1_5学年, Chk随時1_6学年, _
                                                Chk随時1_7学年, Chk随時1_8学年, Chk随時1_9学年)
            Case "Chk随時2_全学年"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時2_全学年, _
                                                Chk随時2_1学年, Chk随時2_2学年, Chk随時2_3学年, _
                                                Chk随時2_4学年, Chk随時2_5学年, Chk随時2_6学年, _
                                                Chk随時2_7学年, Chk随時2_8学年, Chk随時2_9学年)
            Case "Chk随時3_全学年"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時3_全学年, _
                                                Chk随時3_1学年, Chk随時3_2学年, Chk随時3_3学年, _
                                                Chk随時3_4学年, Chk随時3_5学年, Chk随時3_6学年, _
                                                Chk随時3_7学年, Chk随時3_8学年, Chk随時3_9学年)
            Case "Chk随時4_全学年"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時4_全学年, _
                                                Chk随時4_1学年, Chk随時4_2学年, Chk随時4_3学年, _
                                                Chk随時4_4学年, Chk随時4_5学年, Chk随時4_6学年, _
                                                Chk随時4_7学年, Chk随時4_8学年, Chk随時4_9学年)
            Case "Chk随時5_全学年"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時5_全学年, _
                                                Chk随時5_1学年, Chk随時5_2学年, Chk随時5_3学年, _
                                                Chk随時5_4学年, Chk随時5_5学年, Chk随時5_6学年, _
                                                Chk随時5_7学年, Chk随時5_8学年, Chk随時5_9学年)
            Case "Chk随時6_全学年"
                Call Sb_AllGakunenChkBox_Control(CInt(Me.lbl使用学年.Text), Chk随時6_全学年, _
                                                Chk随時6_1学年, Chk随時6_2学年, Chk随時6_3学年, _
                                                Chk随時6_4学年, Chk随時6_5学年, Chk随時6_6学年, _
                                                Chk随時6_7学年, Chk随時6_8学年, Chk随時6_9学年)

            Case Else
        End Select
    End Sub


    Private Sub Chk有効振替日_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk有効振替日通常1.CheckedChanged, Chk有効振替日通常2.CheckedChanged, Chk有効振替日通常3.CheckedChanged, Chk有効振替日通常4.CheckedChanged, Chk有効振替日通常5.CheckedChanged

        '初振チェックを外したとき、再振チェックも外す（再振のみの登録を防ぐため）
        If CType(sender, CheckBox).Checked = False Then
            Select Case CType(sender, CheckBox).Name
                Case "Chk有効振替日通常1"
                    Me.Chk有効再振日通常1.Checked = False
                Case "Chk有効振替日通常2"
                    Me.Chk有効再振日通常2.Checked = False
                Case "Chk有効振替日通常3"
                    Me.Chk有効再振日通常3.Checked = False
                Case "Chk有効振替日通常4"
                    Me.Chk有効再振日通常4.Checked = False
                Case "Chk有効振替日通常5"
                    Me.Chk有効再振日通常5.Checked = False

                Case Else
            End Select
        End If
    End Sub

    Private Sub Chk有効再振日_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Chk有効再振日通常1.CheckedChanged, Chk有効再振日通常2.CheckedChanged, Chk有効再振日通常3.CheckedChanged, Chk有効再振日通常4.CheckedChanged, Chk有効再振日通常5.CheckedChanged

        '再振チェックを入れたとき、初振チェックも入れる（再振のみの登録を防ぐため）
        If CType(sender, CheckBox).Checked = True Then
            Select Case CType(sender, CheckBox).Name
                Case "Chk有効再振日通常1"
                    Me.Chk有効振替日通常1.Checked = True
                Case "Chk有効再振日通常2"
                    Me.Chk有効振替日通常2.Checked = True
                Case "Chk有効再振日通常3"
                    Me.Chk有効振替日通常3.Checked = True
                Case "Chk有効再振日通常4"
                    Me.Chk有効振替日通常4.Checked = True
                Case "Chk有効再振日通常5"
                    Me.Chk有効振替日通常5.Checked = True
                Case Else
            End Select
        End If
    End Sub
#End Region
    'ComboBox変更処理
    Private Sub ComboBox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmbKana.SelectedIndexChanged, cmbGakkouName.SelectedIndexChanged

        Try
            '***ログ設定***
            STR_SYORI_NAME = "月間スケジュール作成"
            STR_COMMAND = "ConboBox_SelectedIndexChanged"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ設定***

            Select Case CType(sender, ComboBox).Name
                Case "cmbKana"
                    If cmbKana.Text = "" Then
                        Exit Sub
                    End If

                    '■学校検索
                    If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
                        Exit Sub
                    End If

                Case "cmbGakkouName"

                    If cmbGakkouName.SelectedIndex = -1 Then
                        Exit Sub
                    End If

                    '■全項目初期化
                    Call FormInitializa(2)

                    '■学校検索後の学校コード設定
                    Me.txtGakkou_Code.Text = STR_GCOAD(cmbGakkouName.SelectedIndex())
                    Me.txtGakkou_Code.Focus()

                    '学校情報の取得
                    If txtGakkou_Code.Text.Trim <> "" Then

                        lbl使用学年.Text = "0"

                        Dim key As Boolean = False
                        Dim G_Info As New GAKDATA(txtGakkou_Code.Text.Trim, key, MainDB)

                        If key = True Then

                            lbl使用学年.Text = CStr(G_Info.SIYOU_GAKUNEN)

                            '最高学年以上の学年の使用不可
                            Call Sb_SiyouGakunenChkEnabled(G_Info.SIYOU_GAKUNEN)

                            '再振替日のプロテクト
                            Select Case G_Info.SFURI_SYUBETU
                                Case "0", "3"
                                    Call Sb_SaifuriProtect(False)
                                Case Else
                                    Call Sb_SaifuriProtect(True)
                            End Select

                            'スケジュールが存在する場合は参照ボタンにフォーカス移動
                            If Me.txt対象年度.Text.Trim <> "" Then
                                Call Sb_Sansyou_Focus()
                            End If
                        End If
                    End If

                Case Else
            End Select
        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try
    End Sub

#Region "ButtonClick"
    '作成ボタン処理
    Private Sub btnAction_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAction.Click

        Try
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()

            'カーソルをウェイト状態にする
            Cursor.Current = Cursors.WaitCursor()

            '***ログ情報設定***
            STR_COMMAND = "スケジュール作成"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***ログ情報設定***

            '■必須項目入力チェック
            If fn_Common_Check(1) = False Then
                Exit Try
            End If

            Dim ALLGakkouCode As String()

            If txtGakkou_Code.Text.Trim = "9999999999" Then
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '一括作成
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("全学校に対して、基準日で月間スケジュールの作成を行ないますか？", _
                "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                ALLGakkouCode = GetALLGakkouCode()

                If ALLGakkouCode Is Nothing Then
                    MessageBox.Show("学校コードの取得に失敗しました", _
                                        "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                '■取得学校分ループ
                For Cnt As Integer = 0 To ALLGakkouCode.Length - 1

                    'タブと構造体の初期化
                    Call FormInitializa(2)
                    Call FormInitializa(9)

                    '■学校スケジュールが対象年月度で存在するかチェック(一つでも存在すれば、作成済とみなす)
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & ALLGakkouCode(Cnt).Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt対象年度.Text.Trim & txt対象月.Text.Trim & "'"

                    '存在しなければ作成
                    If GFUNC_ISEXIST(SQL) = False Then
                        '■スケジュール作成
                        ERRMSG = ""
                        If fn_InsertG_SCHMAST(ALLGakkouCode(Cnt).Trim) = False Then
                            MessageBox.Show(ERRMSG & vbCrLf & "学校コード:" & ALLGakkouCode(Cnt).Trim, _
                            "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If
                    End If
                Next

                MessageBox.Show("スケジュールを作成しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '個別作成
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("月間スケジュールの作成を行ないますか？" & vbCrLf & "学校コード:" & txtGakkou_Code.Text.Trim, _
                                    "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                '■学校スケジュールが対象年月度で存在するかチェック(一つでも存在すれば、作成済とみなす)
                If txtGakkou_Code.Text.Trim <> "" Then
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & txtGakkou_Code.Text.Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt対象年度.Text.Trim & txt対象月.Text.Trim & "'"

                    If GFUNC_ISEXIST(SQL) = True Then
                        MessageBox.Show("月間スケジュール作成済です。参照後更新を行なってください。", _
                        "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End If

                '構造体の初期化
                Call FormInitializa(9)

                '■スケジュール作成
                ERRMSG = ""
                If fn_InsertG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '学校情報制御
                    txt対象年度.Enabled = True
                    txtGakkou_Code.Enabled = True
                    '入力ボタン制御
                    Call Sb_Btn_Enable(0)
                Else
                    '参照する
                    If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                        Exit Try
                    End If

                    MessageBox.Show("スケジュールを作成しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    '入力ボタン制御
                    Call Sb_Btn_Enable(1)
                End If
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            MessageBox.Show("スケジュール作成処理に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '参照ボタン処理
    Private Sub btnFind_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnFind.Click
        Try
            Cursor.Current = Cursors.WaitCursor()

            '***参照ボタンログ情報***
            STR_COMMAND = "スケジュール参照"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***参照ボタンログ情報***

            '■必須項目入力チェック
            If fn_Common_Check(2) = False Then
                Exit Try
            End If

            '■参照処理
            ERRMSG = ""
            If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            Else
                '入力ボタン制御
                Call Sb_Btn_Enable(1)
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            MessageBox.Show("スケジュール参照処理に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '更新ボタン処理
    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUPDATE.Click
        Try
            Cursor.Current = Cursors.WaitCursor()

            '***更新ボタンログ情報***
            STR_COMMAND = "スケジュール更新"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***更新ボタンログ情報***

            '■必須項目入力チェック
            If fn_Common_Check(3) = False Then
                Exit Try
            End If

            If MessageBox.Show("更新しますか？", "確認", MessageBoxButtons.OKCancel, _
            MessageBoxIcon.Question) = DialogResult.Cancel Then
                Exit Sub
            End If

            '■更新処理
            ERRMSG = ""
            If fn_UpdateG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Try
            Else
                '参照する
                If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                    Exit Try
                End If

                MessageBox.Show("スケジュールを更新しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information)
                '入力ボタン制御
                Call Sb_Btn_Enable(2)
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            MessageBox.Show("スケジュール更新処理に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '削除ボタン処理
    Private Sub btnDelete_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDelete.Click

        Try
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()

            'カーソルをウェイト状態にする
            Cursor.Current = Cursors.WaitCursor()

            '***ログ情報設定***
            STR_COMMAND = "スケジュール削除"
            STR_LOG_GAKKOU_CODE = Trim(txtGakkou_Code.Text)
            STR_LOG_FURI_DATE = ""
            '***ログ情報設定***

            '■必須項目入力チェック
            If fn_Common_Check(1) = False Then
                Exit Try
            End If

            Dim NENGETSUDO As String = txt対象年度.Text.Trim & txt対象月.Text.Trim
            Dim ALLGakkouCode As String()

            If txtGakkou_Code.Text.Trim = "9999999999" Then
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '一括削除
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("全学校に対して、未処理のスケジュールの削除を行ないますか？", _
                "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                ALLGakkouCode = GetALLGakkouCode()

                If ALLGakkouCode Is Nothing Then
                    MessageBox.Show("学校コードの取得に失敗しました", _
                                        "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                '■取得学校分ループ
                For Cnt As Integer = 0 To ALLGakkouCode.Length - 1

                    'タブと構造体の初期化
                    Call FormInitializa(2)
                    Call FormInitializa(9)

                    '■学校スケジュールが対象年月度で存在するかチェック(一つでも存在すれば、作成済とみなす)
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & ALLGakkouCode(Cnt).Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & NENGETSUDO & "'"

                    '存在すれば削除
                    If GFUNC_ISEXIST(SQL) = True Then
                        '■スケジュール削除
                        ERRMSG = ""
                        If fn_DeleteG_SCHMAST_ALL(NENGETSUDO, ALLGakkouCode(Cnt).Trim) = False Then
                            MessageBox.Show(ERRMSG & vbCrLf & "学校コード:" & ALLGakkouCode(Cnt).Trim, _
                            "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If
                    End If
                Next

                MessageBox.Show("スケジュールを削除しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Else
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '個別削除
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                If MessageBox.Show("未処理スケジュールの削除を行ないますか？" & vbCrLf & "学校コード:" & txtGakkou_Code.Text.Trim, _
                                    "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) = DialogResult.Cancel Then
                    Exit Sub
                End If

                '■学校スケジュールが対象年月度で存在するかチェック(一つでも存在すれば、作成済とみなす)
                If txtGakkou_Code.Text.Trim <> "" Then
                    Dim SQL As String
                    SQL = "  SELECT * FROM G_SCHMAST "
                    SQL &= " WHERE GAKKOU_CODE_S = '" & txtGakkou_Code.Text.Trim & "'"
                    SQL &= " AND NENGETUDO_S = '" & txt対象年度.Text.Trim & txt対象月.Text.Trim & "'"

                    If GFUNC_ISEXIST(SQL) = False Then
                        MessageBox.Show("月間スケジュール未作成です。", _
                        "確認", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End If

                '構造体の初期化
                Call FormInitializa(9)

                '■スケジュール削除
                ERRMSG = ""
                If fn_DeleteG_SCHMAST_ALL(NENGETSUDO, txtGakkou_Code.Text.Trim) = False Then
                    MessageBox.Show(ERRMSG, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    '学校情報制御
                    txt対象年度.Enabled = True
                    txtGakkou_Code.Enabled = True
                    '入力ボタン制御
                    Call Sb_Btn_Enable(0)
                Else
                    '参照する
                    If fn_SelectG_SCHMAST(txtGakkou_Code.Text.Trim) = False Then
                        Exit Try
                    End If

                    MessageBox.Show("スケジュールを削除しました。", "確認", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    '入力ボタン制御
                    Call Sb_Btn_Enable(1)
                End If
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            MessageBox.Show("スケジュール作成処理に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            OBJ_CONNECTION.Close()
            OBJ_CONNECTION.Open()
        End Try

    End Sub
    '取消ボタン処理
    Private Sub btnEraser_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraser.Click

        '画面初期状態
        Call FormInitializa(0)

        Me.btnAction.Enabled = True
        Me.btnFind.Enabled = True
        Me.btnEnd.Enabled = True
        Me.btnEraser.Enabled = True
        Me.btnUPDATE.Enabled = True

        txt対象年度.Focus()
    End Sub
    '終了ボタン処理
    Private Sub btnEnd_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnd.Click
        '終了ボタン
        Me.Close()
    End Sub
    '画面終了時の処理
    Private Sub GFJMAST0602G_Closed(ByVal sender As Object, ByVal e As System.EventArgs) Handles MyBase.Closed

    End Sub
#End Region
#Region "SELECT G_SCHMAST"
    '
    '　関数名　-　fn_SelectG_SCHMAST
    '
    '　機能    -  スケジュール参照
    '
    '　引数    -  aInfoGakkou
    '
    '　備考    -  
    '
    '　
    Private Function fn_SelectG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_SelectG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***ログ情報***

            '■学校情報の取得
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "学校情報の取得に失敗しました。" & vbCrLf & "学校コード:" & strGakkouCode
                Exit Try
            End If

            'タブと構造体の初期化
            Call FormInitializa(2)
            Call FormInitializa(9)

            '再振替日のプロテクト
            Select Case InfoGakkou.SFURI_SYUBETU
                Case "0", "3"
                    Call Sb_SaifuriProtect(False)
                Case Else
                    Call Sb_SaifuriProtect(True)
            End Select

            '■通常振替日情報を取得し画面にセット
            If fn_SelectTuujyouG_SCHMAST(InfoGakkou) = False Then
                Exit Try
            End If

            '■随時振替日情報を取得し画面にセット
            If fn_SelectZuijiG_SCHMAST(InfoGakkou) = False Then
                Exit Try
            End If

            '最高学年以上の学年の使用不可
            Call Sb_SiyouGakunenChkEnabled(InfoGakkou.SIYOU_GAKUNEN)

            '■画面情報を初期構造体にセットする
            Call Sb_GetData(Syoki_Tuujyou_SchInfo, Syoki_Zuiji_SchInfo)

            ret = True

        Catch ex As Exception
            ERRMSG = "参照に失敗しました。"
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ret = False
        End Try

        Return ret

    End Function
    '
    '　関数名　-　fn_SelectTuujyouG_SCHMAST
    '
    '　機能    -  通常スケジュール参照
    '
    '　引数    -  aInfoGakkou
    '
    '　備考    -  
    '
    '　
    Private Function fn_SelectTuujyouG_SCHMAST(ByVal aInfoGakkou As GAKDATA) As Boolean

        Dim ret As Boolean = False

        Dim Orareader As CASTCommon.MyOracleReader

        Try
            '***ログ情報***
            STR_COMMAND = "fn_SelectTuujyouG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報***

            Dim SyoriNen As String = Me.txt対象年度.Text.Trim
            Dim SyoriTuki As String = Me.txt対象月.Text.Trim

            Orareader = New CASTCommon.MyOracleReader()

            Dim SQL As String

            SQL = " SELECT * FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
            SQL &= " AND   NENGETUDO_S = '" & SyoriNen & SyoriTuki & "'"
            SQL &= " AND   SCH_KBN_S ='0'"   '通常スケジュールの取得
            SQL &= " AND   FURI_KBN_S = '0'"   '初振スケジュールの取得
            SQL &= " ORDER BY FURI_DATE_S"     '振替日順に抽出

            Dim InfoG_Schmast_Syofuri As New G_SCHMASTDATA '初振データ格納用
            Dim InfoG_Schmast_Saifuri As New G_SCHMASTDATA '再振データ格納用

            If Orareader.DataReader(SQL) = True Then
                '初振スケジュールのカウンタ
                Dim Cnt As Integer = 1

                While Orareader.EOF = False

                    '初振分を構造体に格納
                    If InfoG_Schmast_Syofuri.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "0", "0", Orareader.GetItem("FURI_DATE_S"), MainDB) = True Then
                        '画面にセットする
                        If Not SetMonitor(Cnt, InfoG_Schmast_Syofuri) Then
                            'ログは関数内で出力
                            Exit Try
                        End If
                    Else
                        Call GSUB_LOG(0, "スケジュール取得失敗")
                        Exit Try
                    End If

                    '再振レコードを検索
                    If InfoG_Schmast_Saifuri.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "0", "1", Orareader.GetItem("SFURI_DATE_S"), MainDB) = True Then
                        '画面にセットする
                        If Not SetMonitor(Cnt, InfoG_Schmast_Saifuri) Then
                            'ログは関数内で出力
                            Exit Try
                        End If
                    End If

                    Cnt += 1
                    Orareader.NextRead()
                End While
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

        Return ret

    End Function

    '
    '　関数名　-　fn_SelectTuujyouG_SCHMAST
    '
    '　機能    -  通常スケジュール参照
    '
    '　引数    -  aInfoGakkou
    '
    '　備考    -  
    '
    '　
    Private Function fn_SelectZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA) As Boolean

        Dim ret As Boolean = False

        Dim Orareader As CASTCommon.MyOracleReader

        Try
            '***ログ情報***
            STR_COMMAND = "fn_SelectZuijiG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報***

            Dim SyoriNen As String = Me.txt対象年度.Text.Trim
            Dim SyoriTuki As String = Me.txt対象月.Text.Trim

            Orareader = New CASTCommon.MyOracleReader()

            Dim SQL As New StringBuilder(1024)

            SQL.Append(" SELECT * FROM G_SCHMAST ")
            SQL.Append(" WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'")
            SQL.Append(" AND   NENGETUDO_S = '" & SyoriNen & SyoriTuki & "'")
            SQL.Append(" AND   SCH_KBN_S ='2'")     '随時スケジュールの取得
            SQL.Append(" ORDER BY FURI_DATE_S")     '振替日順に抽出

            Dim InfoG_Schmast_Zuiji As New G_SCHMASTDATA '初振データ格納用

            If Orareader.DataReader(SQL) = True Then
                '随時スケジュールのカウンタ
                Dim Cnt As Integer = 1

                While Orareader.EOF = False
                    '随時を構造体に格納
                    If InfoG_Schmast_Zuiji.SetG_SCHMASTDATA(aInfoGakkou.GAKKOU_CODE, "2", Orareader.GetItem("FURI_KBN_S"), _
                                                            Orareader.GetItem("FURI_DATE_S"), MainDB) = True Then
                        '画面にセットする
                        If Not SetMonitor(Cnt, InfoG_Schmast_Zuiji) Then
                            'ログは関数内で出力
                            Exit Try
                        End If
                    Else
                        'ログは関数内で出力
                        Exit Try
                    End If

                    Cnt += 1
                    Orareader.NextRead()
                End While
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

        Return ret

    End Function
    '
    '　関数名　-　SetMonitor
    '
    '　機能    -  取得した情報を画面にセット
    '
    '　引数    -  Cnt
    '
    '　備考    -  fn_SelectTuujyouG_SCHMAST,fn_SelectZuijiG_SCHMASTのサブ関数
    '             
    '
    Private Function SetMonitor(ByVal Cnt As Integer, ByVal InfoG_SCHMAST As G_SCHMASTDATA) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "SetMonitor"
            STR_LOG_GAKKOU_CODE = InfoG_SCHMAST.GAKKOU_CODE_S
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            With InfoG_SCHMAST
                '■学年情報の取得(個別か全学年か)
                Dim GakunenFlg(9) As Boolean
                Dim FlgGakunenAll As Boolean = False
                If .GAKUNEN1_FLG_S = "1" AndAlso _
                .GAKUNEN2_FLG_S = "1" AndAlso _
                .GAKUNEN3_FLG_S = "1" AndAlso _
                .GAKUNEN4_FLG_S = "1" AndAlso _
                .GAKUNEN5_FLG_S = "1" AndAlso _
                .GAKUNEN6_FLG_S = "1" AndAlso _
                .GAKUNEN7_FLG_S = "1" AndAlso _
                .GAKUNEN8_FLG_S = "1" AndAlso _
                .GAKUNEN9_FLG_S = "1" Then
                    '全学年フラグあり
                    FlgGakunenAll = True
                Else
                    GakunenFlg = .GetSchMastGakunenFlg
                    If GakunenFlg(0) = False Then
                        Exit Try
                    End If
                End If

                '■Enable判定(処理中かどうか)
                Dim FlgEnable As Boolean = False
                If .ENTRI_FLG_S <> "0" OrElse _
                .CHECK_FLG_S <> "0" OrElse _
                .DATA_FLG_S <> "0" OrElse _
                .FUNOU_FLG_S <> "0" OrElse _
                .SAIFURI_FLG_S <> "0" OrElse _
                .KESSAI_FLG_S <> "0" OrElse _
                .ENTRI_FLG_S <> "0" OrElse _
                .TYUUDAN_FLG_S <> "0" Then
                    '処理中
                    FlgEnable = True
                End If

                '入力なし対応　'20081009
                If .YOBI1_S.Trim = "" Then
                    .YOBI1_S = .FURI_DATE_S
                End If

                '情報のセット
                Select Case .SCH_KBN_S
                    Case "0" '通常
                        Select Case Cnt 'セットする場所
                            Case 1
                                '初振か再振
                                If .FURI_KBN_S = "0" Then
                                    '振替日設定
                                    Chk有効振替日通常1.Checked = True
                                    txt通常振替日1.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常振替日1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '対象学年設定
                                    If FlgGakunenAll = True Then
                                        Chk学年通常1_全.Checked = True
                                    Else
                                        Chk学年通常1_1.Checked = GakunenFlg(1)
                                        Chk学年通常1_2.Checked = GakunenFlg(2)
                                        Chk学年通常1_3.Checked = GakunenFlg(3)
                                        Chk学年通常1_4.Checked = GakunenFlg(4)
                                        Chk学年通常1_5.Checked = GakunenFlg(5)
                                        Chk学年通常1_6.Checked = GakunenFlg(6)
                                        Chk学年通常1_7.Checked = GakunenFlg(7)
                                        Chk学年通常1_8.Checked = GakunenFlg(8)
                                        Chk学年通常1_9.Checked = GakunenFlg(9)
                                    End If
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効振替日通常1.Enabled = False
                                        txt通常振替日1.Enabled = False
                                        Chk学年通常1_1.Enabled = False
                                        Chk学年通常1_2.Enabled = False
                                        Chk学年通常1_3.Enabled = False
                                        Chk学年通常1_4.Enabled = False
                                        Chk学年通常1_5.Enabled = False
                                        Chk学年通常1_6.Enabled = False
                                        Chk学年通常1_7.Enabled = False
                                        Chk学年通常1_8.Enabled = False
                                        Chk学年通常1_9.Enabled = False
                                        Chk学年通常1_全.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '振替日設定
                                    Chk有効再振日通常1.Checked = True
                                    txt通常再振日1.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常再振日1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効再振日通常1.Enabled = False
                                        txt通常再振日1.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If
                            Case 2
                                '初振か再振
                                If .FURI_KBN_S = "0" Then
                                    '振替日設定
                                    Chk有効振替日通常2.Checked = True
                                    txt通常振替日2.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常振替日2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk学年通常2_全.Checked = True
                                    Else
                                        Chk学年通常2_1.Checked = GakunenFlg(1)
                                        Chk学年通常2_2.Checked = GakunenFlg(2)
                                        Chk学年通常2_3.Checked = GakunenFlg(3)
                                        Chk学年通常2_4.Checked = GakunenFlg(4)
                                        Chk学年通常2_5.Checked = GakunenFlg(5)
                                        Chk学年通常2_6.Checked = GakunenFlg(6)
                                        Chk学年通常2_7.Checked = GakunenFlg(7)
                                        Chk学年通常2_8.Checked = GakunenFlg(8)
                                        Chk学年通常2_9.Checked = GakunenFlg(9)
                                    End If
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効振替日通常2.Enabled = False
                                        txt通常振替日2.Enabled = False
                                        Chk学年通常2_1.Enabled = False
                                        Chk学年通常2_2.Enabled = False
                                        Chk学年通常2_3.Enabled = False
                                        Chk学年通常2_4.Enabled = False
                                        Chk学年通常2_5.Enabled = False
                                        Chk学年通常2_6.Enabled = False
                                        Chk学年通常2_7.Enabled = False
                                        Chk学年通常2_8.Enabled = False
                                        Chk学年通常2_9.Enabled = False
                                        Chk学年通常2_全.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '振替日設定
                                    Chk有効再振日通常2.Checked = True
                                    txt通常再振日2.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常再振日2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効再振日通常2.Enabled = False
                                        txt通常再振日2.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If
                            Case 3
                                '初振か再振
                                If .FURI_KBN_S = "0" Then
                                    '振替日設定
                                    Chk有効振替日通常3.Checked = True
                                    txt通常振替日3.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常振替日3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk学年通常3_全.Checked = True
                                    Else
                                        Chk学年通常3_1.Checked = GakunenFlg(1)
                                        Chk学年通常3_2.Checked = GakunenFlg(2)
                                        Chk学年通常3_3.Checked = GakunenFlg(3)
                                        Chk学年通常3_4.Checked = GakunenFlg(4)
                                        Chk学年通常3_5.Checked = GakunenFlg(5)
                                        Chk学年通常3_6.Checked = GakunenFlg(6)
                                        Chk学年通常3_7.Checked = GakunenFlg(7)
                                        Chk学年通常3_8.Checked = GakunenFlg(8)
                                        Chk学年通常3_9.Checked = GakunenFlg(9)
                                    End If
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効振替日通常3.Enabled = False
                                        txt通常振替日3.Enabled = False
                                        Chk学年通常3_1.Enabled = False
                                        Chk学年通常3_2.Enabled = False
                                        Chk学年通常3_3.Enabled = False
                                        Chk学年通常3_4.Enabled = False
                                        Chk学年通常3_5.Enabled = False
                                        Chk学年通常3_6.Enabled = False
                                        Chk学年通常3_7.Enabled = False
                                        Chk学年通常3_8.Enabled = False
                                        Chk学年通常3_9.Enabled = False
                                        Chk学年通常3_全.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '振替日設定
                                    Chk有効再振日通常3.Checked = True
                                    txt通常再振日3.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常再振日3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効再振日通常3.Enabled = False
                                        txt通常再振日3.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If
                            Case 4
                                '初振か再振
                                If .FURI_KBN_S = "0" Then
                                    '振替日設定
                                    Chk有効振替日通常4.Checked = True
                                    txt通常振替日4.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常振替日4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk学年通常4_全.Checked = True
                                    Else
                                        Chk学年通常4_1.Checked = GakunenFlg(1)
                                        Chk学年通常4_2.Checked = GakunenFlg(2)
                                        Chk学年通常4_3.Checked = GakunenFlg(3)
                                        Chk学年通常4_4.Checked = GakunenFlg(4)
                                        Chk学年通常4_5.Checked = GakunenFlg(5)
                                        Chk学年通常4_6.Checked = GakunenFlg(6)
                                        Chk学年通常4_7.Checked = GakunenFlg(7)
                                        Chk学年通常4_8.Checked = GakunenFlg(8)
                                        Chk学年通常4_9.Checked = GakunenFlg(9)
                                    End If
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効振替日通常4.Enabled = False
                                        txt通常振替日4.Enabled = False
                                        Chk学年通常4_1.Enabled = False
                                        Chk学年通常4_2.Enabled = False
                                        Chk学年通常4_3.Enabled = False
                                        Chk学年通常4_4.Enabled = False
                                        Chk学年通常4_5.Enabled = False
                                        Chk学年通常4_6.Enabled = False
                                        Chk学年通常4_7.Enabled = False
                                        Chk学年通常4_8.Enabled = False
                                        Chk学年通常4_9.Enabled = False
                                        Chk学年通常4_全.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '振替日設定
                                    Chk有効再振日通常4.Checked = True
                                    txt通常再振日4.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常再振日4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効再振日通常4.Enabled = False
                                        txt通常再振日4.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If
                            Case 5
                                '初振か再振
                                If .FURI_KBN_S = "0" Then
                                    '振替日設定
                                    Chk有効振替日通常5.Checked = True
                                    txt通常振替日5.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常振替日5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    If FlgGakunenAll = True Then
                                        Chk学年通常5_全.Checked = True
                                    Else
                                        Chk学年通常5_1.Checked = GakunenFlg(1)
                                        Chk学年通常5_2.Checked = GakunenFlg(2)
                                        Chk学年通常5_3.Checked = GakunenFlg(3)
                                        Chk学年通常5_4.Checked = GakunenFlg(4)
                                        Chk学年通常5_5.Checked = GakunenFlg(5)
                                        Chk学年通常5_6.Checked = GakunenFlg(6)
                                        Chk学年通常5_7.Checked = GakunenFlg(7)
                                        Chk学年通常5_8.Checked = GakunenFlg(8)
                                        Chk学年通常5_9.Checked = GakunenFlg(9)
                                    End If
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効振替日通常5.Enabled = False
                                        txt通常振替日5.Enabled = False
                                        Chk学年通常5_1.Enabled = False
                                        Chk学年通常5_2.Enabled = False
                                        Chk学年通常5_3.Enabled = False
                                        Chk学年通常5_4.Enabled = False
                                        Chk学年通常5_5.Enabled = False
                                        Chk学年通常5_6.Enabled = False
                                        Chk学年通常5_7.Enabled = False
                                        Chk学年通常5_8.Enabled = False
                                        Chk学年通常5_9.Enabled = False
                                        Chk学年通常5_全.Enabled = False
                                    End If
                                ElseIf .FURI_KBN_S = "1" Then
                                    '振替日設定
                                    Chk有効再振日通常5.Checked = True
                                    txt通常再振日5.Text = .YOBI1_S.Substring(6, 2)
                                    lbl通常再振日5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                        .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                        .FURI_DATE_S.Substring(6)
                                    '更新可能か設定
                                    If FlgEnable = True Then
                                        Chk有効再振日通常5.Enabled = False
                                        txt通常再振日5.Enabled = False
                                    End If
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                            Case Else
                                Call GSUB_LOG(0, "通常スケジュールカウンタ異常")
                                Exit Try
                        End Select

                    Case "2" '随時
                        Select Case Cnt 'セットする場所
                            Case 1
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分1.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分1.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日1.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日1.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時1_全学年.Checked = True
                                Else
                                    Chk随時1_1学年.Checked = GakunenFlg(1)
                                    Chk随時1_2学年.Checked = GakunenFlg(2)
                                    Chk随時1_3学年.Checked = GakunenFlg(3)
                                    Chk随時1_4学年.Checked = GakunenFlg(4)
                                    Chk随時1_5学年.Checked = GakunenFlg(5)
                                    Chk随時1_6学年.Checked = GakunenFlg(6)
                                    Chk随時1_7学年.Checked = GakunenFlg(7)
                                    Chk随時1_8学年.Checked = GakunenFlg(8)
                                    Chk随時1_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分1.Enabled = False
                                    txt随時振替日1.Enabled = False
                                    Chk随時1_1学年.Enabled = False
                                    Chk随時1_2学年.Enabled = False
                                    Chk随時1_3学年.Enabled = False
                                    Chk随時1_4学年.Enabled = False
                                    Chk随時1_5学年.Enabled = False
                                    Chk随時1_6学年.Enabled = False
                                    Chk随時1_7学年.Enabled = False
                                    Chk随時1_8学年.Enabled = False
                                    Chk随時1_9学年.Enabled = False
                                    Chk随時1_全学年.Enabled = False
                                End If
                            Case 2
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分2.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分2.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日2.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日2.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時2_全学年.Checked = True
                                Else
                                    Chk随時2_1学年.Checked = GakunenFlg(1)
                                    Chk随時2_2学年.Checked = GakunenFlg(2)
                                    Chk随時2_3学年.Checked = GakunenFlg(3)
                                    Chk随時2_4学年.Checked = GakunenFlg(4)
                                    Chk随時2_5学年.Checked = GakunenFlg(5)
                                    Chk随時2_6学年.Checked = GakunenFlg(6)
                                    Chk随時2_7学年.Checked = GakunenFlg(7)
                                    Chk随時2_8学年.Checked = GakunenFlg(8)
                                    Chk随時2_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分2.Enabled = False
                                    txt随時振替日2.Enabled = False
                                    Chk随時2_1学年.Enabled = False
                                    Chk随時2_2学年.Enabled = False
                                    Chk随時2_3学年.Enabled = False
                                    Chk随時2_4学年.Enabled = False
                                    Chk随時2_5学年.Enabled = False
                                    Chk随時2_6学年.Enabled = False
                                    Chk随時2_7学年.Enabled = False
                                    Chk随時2_8学年.Enabled = False
                                    Chk随時2_9学年.Enabled = False
                                    Chk随時2_全学年.Enabled = False
                                End If
                            Case 3
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分1.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分1.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日3.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日3.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時3_全学年.Checked = True
                                Else
                                    Chk随時3_1学年.Checked = GakunenFlg(1)
                                    Chk随時3_2学年.Checked = GakunenFlg(2)
                                    Chk随時3_3学年.Checked = GakunenFlg(3)
                                    Chk随時3_4学年.Checked = GakunenFlg(4)
                                    Chk随時3_5学年.Checked = GakunenFlg(5)
                                    Chk随時3_6学年.Checked = GakunenFlg(6)
                                    Chk随時3_7学年.Checked = GakunenFlg(7)
                                    Chk随時3_8学年.Checked = GakunenFlg(8)
                                    Chk随時3_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分3.Enabled = False
                                    txt随時振替日3.Enabled = False
                                    Chk随時3_1学年.Enabled = False
                                    Chk随時3_2学年.Enabled = False
                                    Chk随時3_3学年.Enabled = False
                                    Chk随時3_4学年.Enabled = False
                                    Chk随時3_5学年.Enabled = False
                                    Chk随時3_6学年.Enabled = False
                                    Chk随時3_7学年.Enabled = False
                                    Chk随時3_8学年.Enabled = False
                                    Chk随時3_9学年.Enabled = False
                                    Chk随時3_全学年.Enabled = False
                                End If
                            Case 4
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分4.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分4.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日4.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日4.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時4_全学年.Checked = True
                                Else
                                    Chk随時4_1学年.Checked = GakunenFlg(1)
                                    Chk随時4_2学年.Checked = GakunenFlg(2)
                                    Chk随時4_3学年.Checked = GakunenFlg(3)
                                    Chk随時4_4学年.Checked = GakunenFlg(4)
                                    Chk随時4_5学年.Checked = GakunenFlg(5)
                                    Chk随時4_6学年.Checked = GakunenFlg(6)
                                    Chk随時4_7学年.Checked = GakunenFlg(7)
                                    Chk随時4_8学年.Checked = GakunenFlg(8)
                                    Chk随時4_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分4.Enabled = False
                                    txt随時振替日4.Enabled = False
                                    Chk随時4_1学年.Enabled = False
                                    Chk随時4_2学年.Enabled = False
                                    Chk随時4_3学年.Enabled = False
                                    Chk随時4_4学年.Enabled = False
                                    Chk随時4_5学年.Enabled = False
                                    Chk随時4_6学年.Enabled = False
                                    Chk随時4_7学年.Enabled = False
                                    Chk随時4_8学年.Enabled = False
                                    Chk随時4_9学年.Enabled = False
                                    Chk随時4_全学年.Enabled = False
                                End If
                            Case 5
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分5.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分5.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日5.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日5.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時5_全学年.Checked = True
                                Else
                                    Chk随時5_1学年.Checked = GakunenFlg(1)
                                    Chk随時5_2学年.Checked = GakunenFlg(2)
                                    Chk随時5_3学年.Checked = GakunenFlg(3)
                                    Chk随時5_4学年.Checked = GakunenFlg(4)
                                    Chk随時5_5学年.Checked = GakunenFlg(5)
                                    Chk随時5_6学年.Checked = GakunenFlg(6)
                                    Chk随時5_7学年.Checked = GakunenFlg(7)
                                    Chk随時5_8学年.Checked = GakunenFlg(8)
                                    Chk随時5_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分5.Enabled = False
                                    txt随時振替日5.Enabled = False
                                    Chk随時5_1学年.Enabled = False
                                    Chk随時5_2学年.Enabled = False
                                    Chk随時5_3学年.Enabled = False
                                    Chk随時5_4学年.Enabled = False
                                    Chk随時5_5学年.Enabled = False
                                    Chk随時5_6学年.Enabled = False
                                    Chk随時5_7学年.Enabled = False
                                    Chk随時5_8学年.Enabled = False
                                    Chk随時5_9学年.Enabled = False
                                    Chk随時5_全学年.Enabled = False
                                End If
                            Case 6
                                '入金か出金
                                If .FURI_KBN_S = "2" Then
                                    cmb入出区分6.SelectedIndex = 0 '入金
                                ElseIf .FURI_KBN_S = "3" Then
                                    cmb入出区分6.SelectedIndex = 1  '出金
                                Else
                                    Call GSUB_LOG(0, "振替区分異常:" & .FURI_KBN_S)
                                    'エラー
                                    Exit Try
                                End If

                                '振替日設定
                                txt随時振替日6.Text = .YOBI1_S.Substring(6, 2)
                                lbl随時振替日6.Text = .FURI_DATE_S.Substring(0, 4) & "/" & _
                                                    .FURI_DATE_S.Substring(4, 2) & "/" & _
                                                    .FURI_DATE_S.Substring(6)
                                If FlgGakunenAll = True Then
                                    Chk随時6_全学年.Checked = True
                                Else
                                    Chk随時6_1学年.Checked = GakunenFlg(1)
                                    Chk随時6_2学年.Checked = GakunenFlg(2)
                                    Chk随時6_3学年.Checked = GakunenFlg(3)
                                    Chk随時6_4学年.Checked = GakunenFlg(4)
                                    Chk随時6_5学年.Checked = GakunenFlg(5)
                                    Chk随時6_6学年.Checked = GakunenFlg(6)
                                    Chk随時6_7学年.Checked = GakunenFlg(7)
                                    Chk随時6_8学年.Checked = GakunenFlg(8)
                                    Chk随時6_9学年.Checked = GakunenFlg(9)
                                End If
                                '更新可能か設定
                                If FlgEnable = True Then
                                    cmb入出区分6.Enabled = False
                                    txt随時振替日6.Enabled = False
                                    Chk随時6_1学年.Enabled = False
                                    Chk随時6_2学年.Enabled = False
                                    Chk随時6_3学年.Enabled = False
                                    Chk随時6_4学年.Enabled = False
                                    Chk随時6_5学年.Enabled = False
                                    Chk随時6_6学年.Enabled = False
                                    Chk随時6_7学年.Enabled = False
                                    Chk随時6_8学年.Enabled = False
                                    Chk随時6_9学年.Enabled = False
                                    Chk随時6_全学年.Enabled = False
                                End If
                            Case Else
                                Call GSUB_LOG(0, "随時処理スケジュール数異常")
                                Exit Try
                        End Select
                    Case Else
                        Call GSUB_LOG(0, "スケジュール区分異常:" & .SCH_KBN_S)
                        Exit Try
                End Select
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ret = False
        End Try

        Return ret

    End Function
#End Region
#Region "UPDATE G_SCHMAST"
    '
    '　関数名　-　fn_UpdateG_SCHMAST
    '
    '　機能    -  スケジュール更新
    '
    '　引数    -  aInfoGakkou
    '
    '　備考    -  
    '
    '　
    Private Function fn_UpdateG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_UpdateG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '■トランザクション開始
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '■学校情報の取得
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "学校情報の取得に失敗しました。" & vbCrLf & "学校コード:" & strGakkouCode
                Exit Try
            End If

            '■画面情報を構造体にセットする(更新後)
            '※更新前情報は参照時にセットされているものとする
            Call Sb_GetData(Tuujyou_SchInfo, Zuiji_SchInfo)

            '■通常振替日タブ入力画面情報のチェック
            If fn_Check_TuujyouFuriDate(InfoGakkou, Tuujyou_SchInfo, False) = False Then
                Exit Try
            End If

            '■随時振替日タブ入力画面情報のチェック
            If fn_Check_ZuijiFuriDate(InfoGakkou, Zuiji_SchInfo) = False Then
                Exit Try
            End If

            Dim SyoriFlg As Boolean = False

            '■初期情報とのマッチング(チェック結果により関数内部で、Insert、Update、Deleteを行なう)
            For i As Integer = 1 To 5 Step 1
                If Not fn_CompareTuujyouData(InfoGakkou, Syoki_Tuujyou_SchInfo(i), Tuujyou_SchInfo(i)) Then
                    Exit Try
                End If
            Next

            '■初期情報とのマッチング(チェック結果により関数内部で、Insert、Update、Deleteを行なう)
            For i As Integer = 1 To 6 Step 1
                If Not fn_CompareZuijiData(InfoGakkou, Syoki_Zuiji_SchInfo(i), Zuiji_SchInfo(i)) Then
                    Exit Try
                End If
            Next

            ret = True

        Catch ex As Exception
            ret = False
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "更新に失敗しました。"
        Finally
            If ret = True Then
                'トランザクション終了（COMMIT）
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT失敗")
                    ret = False
                End If
            Else
                'トランザクション終了（ROLLBACK）
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK失敗")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

    '
    '　関数名　-　fn_CompareTuujyouData
    '
    '　機能    -  スケジュール更新
    '
    '　引数    -  aSyoki_Tuujyou_SchInfo,aTuujyou_SchInfo
    '
    '  戻り値　-　0:処理無、1:処理有、9:処理失敗
    '
    '　備考    -  
    '
    '　
    Private Function fn_CompareTuujyouData(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Tuujyou_SchInfo As TuujyouData, _
                                        ByVal aTuujyou_SchInfo As TuujyouData) As Boolean
        Dim ret As Boolean = False

        Try
            '処理対象フラグ(処理を行なう必要があるかどうか)
            Dim SyoriFlg As Boolean = False

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '参照時のスケジュール情報との比較
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            With aSyoki_Tuujyou_SchInfo
                '■有効振替日チェック
                Select Case True
                    Case (.Furikae_Check = True AndAlso aTuujyou_SchInfo.Furikae_Check = True)
                        '初期ありかつ、更新後あり→Update

                        '初振日のチェック
                        If .Furikae_Date <> aTuujyou_SchInfo.Furikae_Date Then
                            SyoriFlg = True
                        End If

                        '対象学年フラグのチェック
                        Dim SyokiGakunenFlg As String() = .GetGakunenFlg
                        Dim GakunenFlg As String() = aTuujyou_SchInfo.GetGakunenFlg

                        If SyokiGakunenFlg(0) = "err" OrElse GakunenFlg(0) = "err" Then
                            'エラーログは内部で出力
                            ret = False
                            Exit Try
                        Else
                            For i As Integer = 1 To 9 Step 1
                                If SyokiGakunenFlg(i) <> GakunenFlg(i) Then
                                    SyoriFlg = True
                                    Exit For
                                End If
                            Next
                        End If
                        '■初振スケジュールのUPDATE
                        If SyoriFlg = True Then
                            If Not UpDateTuujoyuG_SCHMAST(0, 0, aInfoGakkou, aSyoki_Tuujyou_SchInfo, aTuujyou_SchInfo) Then
                                Exit Try
                            End If
                        End If

                        '有効再振日チェック
                        Select Case True
                            Case (.SaiFurikae_Check = True AndAlso aTuujyou_SchInfo.SaiFurikae_Check = True)
                                '初期ありかつ、更新後あり→Update

                                '対象学年フラグのチェック
                                SyokiGakunenFlg = .GetGakunenFlg
                                GakunenFlg = aTuujyou_SchInfo.GetGakunenFlg


                                '■再振スケジュールのUPDATE
                                If Not UpDateTuujoyuG_SCHMAST(0, 1, aInfoGakkou, aSyoki_Tuujyou_SchInfo, aTuujyou_SchInfo) Then
                                    Exit Try
                                End If

                            Case (.SaiFurikae_Check = False AndAlso aTuujyou_SchInfo.SaiFurikae_Check = True)
                                '初期なしかつ、更新後あり→Insert

                                '■再振のみのInsert処理
                                If Not fn_InsertTuujyouG_SCHMAST(aInfoGakkou, aTuujyou_SchInfo, True) Then
                                    Exit Try
                                End If

                                Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki
                                Dim SaiFuriDate As String = ""

                                Select Case True
                                    Case aTuujyou_SchInfo.Furikae_Date < aTuujyou_SchInfo.SaiFurikae_Date
                                        '振替日算出
                                        SaiFuriDate = fn_GetFuriDate(aTuujyou_SchInfo.Seikyu_Nen, aTuujyou_SchInfo.Seikyu_Tuki, aTuujyou_SchInfo.SaiFurikae_Date, "0", "1", aInfoGakkou)
                                    Case aTuujyou_SchInfo.Furikae_Date > aTuujyou_SchInfo.SaiFurikae_Date
                                        If .Seikyu_Tuki = "12" Then
                                            '振替日算出
                                            SaiFuriDate = fn_GetFuriDate(CStr(CInt(aTuujyou_SchInfo.Seikyu_Nen) + 1), "01", aTuujyou_SchInfo.SaiFurikae_Date, "0", "1", aInfoGakkou)
                                        Else
                                            '振替日算出
                                            SaiFuriDate = fn_GetFuriDate(aTuujyou_SchInfo.Seikyu_Nen, Format(CInt(aTuujyou_SchInfo.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, "0", "1", aInfoGakkou)
                                        End If

                                    Case aTuujyou_SchInfo.Furikae_Date = aTuujyou_SchInfo.SaiFurikae_Date
                                        Call GSUB_LOG(0, "振替日異常:" & "1")
                                        ERRMSG = "振替日と再振日が同一です"
                                        Exit Try
                                End Select

                                '■再振のみ作成した場合は初振の再振日を更新
                                Dim SQL As String
                                SQL = " UPDATE  G_SCHMAST SET "
                                SQL &= " SFURI_DATE_S = '" & SaiFuriDate & "'"
                                SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                                SQL &= " AND   NENGETUDO_S = '" & Nengetsudo & "'"
                                SQL &= " AND   SCH_KBN_S = '0'"
                                SQL &= " AND   FURI_KBN_S = '0'"
                                SQL &= " AND   FURI_DATE_S = '" & .Furikae_Day & "'"

                                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                    Call GSUB_LOG(0, "学校スケジュール更新失敗、SQL:" & SQL)
                                    ERRMSG = "学校スケジュール更新に失敗しました"
                                    Exit Try
                                End If


                            Case (.SaiFurikae_Check = True AndAlso aTuujyou_SchInfo.SaiFurikae_Check = False)
                                '初期ありかつ、更新後なし→Delete

                                Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki

                                '再振のみ削除
                                If .SaiFurikae_Check = True Then
                                    If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 1, .SaiFurikae_Day, Nengetsudo) Then
                                        Exit Try
                                    End If

                                    '■再振のみ削除した場合は初振の再振日を更新
                                    Dim SQL As String

                                    SQL = " UPDATE  G_SCHMAST SET "
                                    SQL &= " SFURI_DATE_S = '00000000' "
                                    SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                                    SQL &= " AND   NENGETUDO_S = '" & Nengetsudo & "'"
                                    SQL &= " AND   SCH_KBN_S = '0'"
                                    SQL &= " AND   FURI_KBN_S = '0'"
                                    SQL &= " AND   FURI_DATE_S = '" & .Furikae_Day & "'"

                                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                        Call GSUB_LOG(0, "学校スケジュール更新失敗、SQL:" & SQL)
                                        ERRMSG = "学校スケジュール更新に失敗しました"
                                        Exit Try
                                    End If
                                End If
                            Case Else
                                '初期なしかつ、更新後なし→なにもしない
                        End Select

                        ret = True

                    Case (.Furikae_Check = False AndAlso aTuujyou_SchInfo.Furikae_Check = True)
                        '初期なしかつ、更新後あり→Insert

                        '■通常のInsertと同一処理
                        If Not fn_InsertTuujyouG_SCHMAST(aInfoGakkou, aTuujyou_SchInfo) Then
                            Exit Try
                        End If

                    Case (.Furikae_Check = True AndAlso aTuujyou_SchInfo.Furikae_Check = False)
                        '初期ありかつ、更新後なし→Delete

                        Dim Nengetsudo As String = .Seikyu_Nen & .Seikyu_Tuki

                        '初振の削除
                        If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 0, .Furikae_Day, Nengetsudo) Then
                            Exit Try
                        End If

                        '初期スケジュールに再振チェックがあれば、再振も削除
                        If .SaiFurikae_Check = True Then
                            If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 0, 1, .SaiFurikae_Day, Nengetsudo) Then
                                Exit Try
                            End If
                        End If

                    Case Else
                        '初期なしかつ、更新後なし→なにもしない
                End Select
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "学校スケジュールの更新に失敗しました"
        End Try

        Return ret

    End Function
    '
    '　関数名　-　UpDateTuujoyuG_SCHMAST
    '
    '　機能    -  スケジュール更新
    '
    '　引数    -  aInfoGakkou、aSyoki_Tuujyou_SchInfo、aTuujyou_SchInfo
    '
    '  戻り値　-　0:処理無、1:処理有、9:処理失敗
    '
    '　備考    -  
    '
    '　
    Private Function UpDateTuujoyuG_SCHMAST(ByVal aFurikbn As Integer, _
                                            ByVal aSchkbn As Integer, _
                                            ByVal aInfoGakkou As GAKDATA, _
                                            ByVal aSyoki_Tuujyou_SchInfo As TuujyouData, _
                                            ByVal aTuujyou_SchInfo As TuujyouData) As Boolean

        Dim ret As Boolean = False

        Try
            '処理日付を取得
            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '学年フラグの取得
            Dim GakunenFlg As String() = aTuujyou_SchInfo.GetGakunenFlg()
            If GakunenFlg(0) = "err" Then

                ERRMSG = "学年対象フラグの取得に失敗しました"
                Exit Try
            End If

            '入力された振替日(営業日を考慮しない)
            Dim EntryFuriDate As String = "00000000"
            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            '引数の振替区分、スケジュール区分をセット
            Dim Schkbn As Integer = aFurikbn
            Dim Furikbn As Integer = aSchkbn

            Dim NENGETDO As String = aSyoki_Tuujyou_SchInfo.Seikyu_Nen & aSyoki_Tuujyou_SchInfo.Seikyu_Tuki

            '振替日の抽出
            With aTuujyou_SchInfo
                Select Case Furikbn
                    Case 0
                        '初振用の設定
                        EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .Furikae_Date
                        '振替日算出
                        FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                        '再振日の年の確定処理
                        If .SaiFurikae_Date <> "" Then
                            Select Case True
                                Case .Furikae_Date < .SaiFurikae_Date
                                    '振替日算出
                                    SaiFuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                Case .Furikae_Date > .SaiFurikae_Date
                                    If .Seikyu_Tuki = "12" Then
                                        '振替日算出
                                        SaiFuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    Else
                                        '振替日算出
                                        SaiFuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    End If

                                Case .Furikae_Date = .SaiFurikae_Date
                                    Call GSUB_LOG(0, "振替日異常:" & Furikbn)
                                    ERRMSG = "振替日と再振日が同一です"
                                    Exit Try
                            End Select

                        Else
                            SaiFuriDate = "00000000"
                        End If
                    Case 1
                        '翌月判定
                        Select Case True
                            Case .Furikae_Date < .SaiFurikae_Date
                                '再振用の設定
                                EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date
                                '振替日算出
                                FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)

                            Case .Furikae_Date > .SaiFurikae_Date
                                If .Seikyu_Tuki = "12" Then
                                    '再振用の設定
                                    EntryFuriDate = CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date
                                    '振替日算出
                                    FuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                Else
                                    '再振用の設定
                                    EntryFuriDate = .Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date
                                    '振替日算出
                                    FuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                End If

                            Case .Furikae_Date = .SaiFurikae_Date
                                Call GSUB_LOG(0, "振替日異常:" & Furikbn)
                                ERRMSG = "振替日と再振日が同一です"
                                Exit Try
                        End Select

                        '持越ありの場合のみ再振日を設定
                        If aInfoGakkou.SFURI_SYUBETU = "3" Then '再振あり、持越あり
                            '請求月が12月なら翌年で1月
                            If .Seikyu_Tuki = "12" Then
                                .Seikyu_Nen = (CInt(.Seikyu_Nen) + 1).ToString
                                .Seikyu_Tuki = "01"
                            Else
                                .Seikyu_Tuki = (CInt(.Seikyu_Tuki) + 1).ToString
                            End If
                            '営業日算出
                            SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                        Else
                            SaiFuriDate = "00000000"
                        End If

                    Case Else
                        Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                        ERRMSG = "学校自振スケジュール作成に失敗しました"
                        Exit Try
                End Select
            End With

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '各種予定日の算出
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<

            'スケジュール作成対象の取引先コードを抽出
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

            CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

            Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

            Dim ENTRY_Y_DATE As String = "00000000"                                                   '明細作成予定日算出
            Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         'チェック予定日算出
            Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '振替データ作成予定日算出
            Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '不能結果更新予定日算出
            Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '資金決済予定日算出

            Dim TESSU_YDATE As String = CLS.SCH.TESUU_YDATE


            Dim SQL As String = ""

            SQL = "  UPDATE G_SCHMAST SET "

            SQL &= " FURI_DATE_S ='" & FuriDate & "'"         '5.振替日
            SQL &= ",SFURI_DATE_S ='" & SaiFuriDate & "'"     '6.再振日
            '対象学年フラグ
            SQL &= ",GAKUNEN1_FLG_S ='" & GakunenFlg(1) & "'" '7.1年
            SQL &= ",GAKUNEN2_FLG_S ='" & GakunenFlg(2) & "'" '8.2年
            SQL &= ",GAKUNEN3_FLG_S ='" & GakunenFlg(3) & "'" '9.3年
            SQL &= ",GAKUNEN4_FLG_S ='" & GakunenFlg(4) & "'" '10.4年
            SQL &= ",GAKUNEN5_FLG_S ='" & GakunenFlg(5) & "'" '11.5年
            SQL &= ",GAKUNEN6_FLG_S ='" & GakunenFlg(6) & "'" '12.6年
            SQL &= ",GAKUNEN7_FLG_S ='" & GakunenFlg(7) & "'" '13.7年
            SQL &= ",GAKUNEN8_FLG_S ='" & GakunenFlg(8) & "'" '14.8年
            SQL &= ",GAKUNEN9_FLG_S ='" & GakunenFlg(9) & "'" '15.9年

            '各日付
            SQL &= ",ENTRI_YDATE_S = '" & ENTRY_Y_DATE & "'"   '21.エントリ予定日
            SQL &= ",ENTRI_DATE_S = '00000000'"                '22.エントリ日
            SQL &= ",CHECK_YDATE_S = '" & CHECK_Y_DATE & "'"   '23.チェック予定日
            SQL &= ",CHECK_DATE_S = '00000000'"                '24.チェック日
            SQL &= ",DATA_YDATE_S = '" & DATA_Y_DATE & "'"     '25.データ予定日
            SQL &= ",DATA_DATE_S = '00000000'"                 '26.データ日
            SQL &= ",FUNOU_YDATE_S = '" & FUNOU_Y_DATE & "'"   '27.不能更新予定日
            SQL &= ",FUNOU_DATE_S = '00000000'"                '28.不能更新日
            SQL &= ",KESSAI_YDATE_S = '" & KESSAI_Y_DATE & "'" '31.決済予定日
            SQL &= ",KESSAI_DATE_S = '00000000'"               '32.決済日

            SQL &= ",YOBI1_S ='" & fn_Hosei(EntryFuriDate) & "'"         '51.予備1(入力日付)

            '■検索条件
            SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"          '学校コード
            SQL &= " AND   NENGETUDO_S = '" & NENGETDO & "'"                           '年月度
            SQL &= " AND   SCH_KBN_S = '" & Schkbn & "'"                               'スケジュール区分
            SQL &= " AND   FURI_KBN_S = '" & Furikbn & "'"                             '振替区分

            If Furikbn = 0 Then
                SQL &= " AND   FURI_DATE_S = '" & aSyoki_Tuujyou_SchInfo.Furikae_Day & "'"    '振替日
            Else
                SQL &= " AND   FURI_DATE_S = '" & aSyoki_Tuujyou_SchInfo.SaiFurikae_Day & "'" '振替日
            End If

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "学校スケジュールUpdate失敗、SQL:" & SQL)
                ERRMSG = "学校スケジュール更新に失敗しました"
                Exit Try
            End If

            '■再振の場合は学校初振スケジュールの再振日を更新する
            If Furikbn = 1 Then
                Dim UpdateSch_FuriDate As String

                With aTuujyou_SchInfo
                    UpdateSch_FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                    SQL = "  UPDATE G_SCHMAST SET "
                    SQL &= " SFURI_DATE_S = '" & FuriDate & "'"
                    SQL &= " WHERE GAKKOU_CODE_S = '" & aInfoGakkou.GAKKOU_CODE & "'"
                    SQL &= " AND   NENGETUDO_S = '" & NENGETDO & "'"
                    SQL &= " AND   SCH_KBN_S = '0'"
                    SQL &= " AND   FURI_KBN_S = '0'"
                    SQL &= " AND   FURI_DATE_S = '" & UpdateSch_FuriDate & "'"

                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                        Call GSUB_LOG(0, "学校スケジュールUpdate失敗、SQL:" & SQL)
                        ERRMSG = "学校スケジュール更新に失敗しました"
                        Exit Try
                    End If

                End With
            End If

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '企業自振スケジュールのUPDATE
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
            Dim ToriF_Code As String

            '振替日に対して依頼書回収区分を算出
            Dim KASIYU_DATE_S As String = ""



            KASIYU_DATE_S = fn_GetEigyoubi(FuriDate, STR_JIFURI_KAISYU, "-")

            Dim KigyouFuriDate As String

            Select Case Furikbn
                Case 0 '初振
                    ToriF_Code = "01"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case 1 '再振
                    ToriF_Code = "02"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.SaiFurikae_Day
                Case 2 '随時入金
                    ToriF_Code = "03"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case 3 '随時出金
                    ToriF_Code = "04"
                    KigyouFuriDate = aSyoki_Tuujyou_SchInfo.Furikae_Day
                Case Else
                    Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                    ERRMSG = "企業自振のスケジュール更新に失敗しました"
                    Exit Try
            End Select

            'スケジュールの存在チェック
            If fn_SchMastIsExist(ToriS_Code, ToriF_Code, KigyouFuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) Then

                SQL = "  UPDATE SCHMAST SET "
                SQL &= " FURI_DATE_S = '" & "" & FuriDate & "'"
                SQL &= ",KFURI_DATE_S = '" & "" & fn_Hosei(EntryFuriDate) & "'"
                '企業自振側でのスケジュールに再振日は設定しない
                SQL &= ",SAIFURI_DATE_S = '00000000'"
                SQL &= ",KSAIFURI_DATE_S = '" & "" & SaiFuriDate & "'"
                '振替日から算出した依頼書回収日をセット
                SQL &= ",IRAISYOK_YDATE_S  = '" & KASIYU_DATE_S & "'"
                '明細作成予定日をセット
                SQL &= ",HAISIN_YDATE_S = '" & DATA_Y_DATE & "'"
                '送信予定日をセット
                SQL &= ",SOUSIN_YDATE_S = '" & DATA_Y_DATE & "'"
                '不能更新予定日をセット
                SQL &= ",FUNOU_YDATE_S = '" & FUNOU_Y_DATE & "'"
                '決済予定日をセット
                SQL &= ",KESSAI_YDATE_S = '" & KESSAI_Y_DATE & "'"
                '手数料予定日をセット
                SQL &= ",TESUU_YDATE_S = '" & TESSU_YDATE & "'"
                '不能更新予定日をセット
                SQL &= ",HENKAN_YDATE_S = '" & FUNOU_Y_DATE & "'"

                SQL &= " WHERE TORIS_CODE_S = '" & ToriS_Code & "'"
                SQL &= " AND TORIF_CODE_S = '" & ToriF_Code & "'"
                SQL &= " AND FURI_DATE_S = '" & KigyouFuriDate & "'"

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Call GSUB_LOG(0, "自振スケジュールUpdate失敗、SQL:" & SQL)
                    ERRMSG = "企業自振スケジュール更新に失敗しました"
                    Exit Try
                End If
            Else
                Call GSUB_LOG(0, "企業自振スケジュール取得失敗、振替日:" & FuriDate)
                ERRMSG = "企業自振スケジュール取得に失敗しました"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "学校スケジュール更新に失敗しました"
            Exit Try
        End Try

        Return ret

    End Function


    '
    '　関数名　-　fn_CompareZuijiData
    '
    '　機能    -  スケジュール更新
    '
    '　引数    -  aSyoki_Zuiji_SchInfo,aZuiji_SchInfo
    '
    '  戻り値　-　0:処理無、1:処理有、9:処理失敗
    '
    '　備考    -  
    '
    '　
    Private Function fn_CompareZuijiData(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Zuiji_SchInfo As ZuijiData, _
                                        ByVal aZuiji_SchInfo As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '参照時のスケジュール情報との比較
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            With aSyoki_Zuiji_SchInfo
                '■有効振替日チェック
                If .Furikae_Date = "" Then
                    If aZuiji_SchInfo.Furikae_Date = "" Then
                        ret = True
                        Exit Try
                    Else
                        '***なし→あり:Insert***
                        If Not fn_InsertZuijiG_SCHMAST(aInfoGakkou, aZuiji_SchInfo) Then
                            Exit Try
                        End If
                    End If
                Else
                    If aZuiji_SchInfo.Furikae_Date = "" Then
                        '***あり→なし:Delete***
                        Dim Furikbn As Integer
                        Dim Nengetsudo As String = .Furikae_Nen & .Furikae_Tuki

                        'スケジュール区分の判定
                        If aSyoki_Zuiji_SchInfo.Nyusyutu_Kbn = "2" Then
                            Furikbn = 2 '入金
                        Else
                            Furikbn = 3 '出金
                        End If

                        If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 2, Furikbn, .Furikae_Day, Nengetsudo) Then
                            Exit Try
                        End If
                    Else
                        '***あり→あり:Update***

                        '処理対象フラグ(処理を行なう必要があるかどうか)
                        Dim SyoriFlg As Boolean = False

                        '■初振日チェック
                        If .Furikae_Date <> aZuiji_SchInfo.Furikae_Date Then
                            SyoriFlg = True
                        End If

                        '■入出金区分チェック
                        If .Nyusyutu_Kbn <> aZuiji_SchInfo.Nyusyutu_Kbn Then
                            SyoriFlg = True
                        End If

                        '■学年フラグチェック
                        Dim SyokiGakunenFlg As String() = .GetGakunenFlg
                        Dim GakunenFlg As String() = aZuiji_SchInfo.GetGakunenFlg

                        If SyokiGakunenFlg(0) = "err" OrElse GakunenFlg(0) = "err" Then
                            'エラーログは内部で出力
                            Exit Try
                        Else
                            For i As Integer = 1 To 9 Step 1
                                If SyokiGakunenFlg(i) <> GakunenFlg(i) Then
                                    SyoriFlg = True
                                    Exit For
                                End If
                            Next
                        End If

                        '■随時スケジュールUpdate
                        If SyoriFlg = True Then
                            If Not UpDateZuijiG_SCHMAST(aInfoGakkou, aSyoki_Zuiji_SchInfo, aZuiji_SchInfo) Then
                                'エラーログは内部で出力
                                Exit Try
                            End If
                        End If
                    End If
                End If
            End With

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "学校スケジュールの更新に失敗しました"
        End Try

        Return ret


    End Function
    '
    '　関数名　-　UpDateZuijiG_SCHMAST
    '
    '　機能    -  スケジュール更新
    '
    '　引数    -  aInfoGakkou、aSyoki_Zuiji_SchInfo、aTuujyou_SchInfo
    '
    '  戻り値　-　True,False
    '
    '　備考    -  入出金区分の変更があった場合更新がめんどうのため、Delete、Insertにする
    '
    '　
    Private Function UpDateZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA, _
                                        ByVal aSyoki_Zuiji_SchInfo As ZuijiData, _
                                        ByVal aZuiji_SchInfo As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***元スケジュールの削除***
            Dim Furikbn As Integer
            Dim Nengetsudo As String = aSyoki_Zuiji_SchInfo.Furikae_Nen & aSyoki_Zuiji_SchInfo.Furikae_Tuki

            'スケジュール区分の判定
            If aSyoki_Zuiji_SchInfo.Nyusyutu_Kbn = "2" Then
                Furikbn = 2 '入金
            Else
                Furikbn = 3 '出金
            End If

            If Not fn_DeleteG_SCHMAST(aInfoGakkou.GAKKOU_CODE, 2, Furikbn, aSyoki_Zuiji_SchInfo.Furikae_Day, Nengetsudo) Then
                ERRMSG = "学校スケジュールの更新に失敗しました"
                Exit Try
            End If

            '***新規に作成***
            If Not fn_InsertZuijiG_SCHMAST(aInfoGakkou, aZuiji_SchInfo) Then
                ERRMSG = "学校スケジュールの更新に失敗しました"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "学校スケジュール更新に失敗しました"
            Exit Try
        End Try

        Return ret

    End Function
#End Region
#Region "InsertG_SCHMAST"
    '
    '　関数名　-　fn_InsertG_SCHMAST
    '
    '　機能    -  スケジュール作成
    '
    '　引数    -  strGakkouCode
    '
    '　備考    -  
    '
    '　
    Private Function fn_InsertG_SCHMAST(ByVal strGakkouCode As String) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_InsertG_SCHMAST"
            STR_LOG_GAKKOU_CODE = strGakkouCode
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '■トランザクション開始
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '■学校情報の取得
            Dim Flg As Boolean = False
            Dim InfoGakkou As New GAKDATA(strGakkouCode, Flg, MainDB)

            If Flg = False Then
                ERRMSG = "学校情報の取得に失敗しました。" & vbCrLf & "学校コード:" & strGakkouCode
                Exit Try
            End If

            '■画面情報を構造体にセットする
            Call Sb_GetData(Tuujyou_SchInfo, Zuiji_SchInfo)

            '■通常振替日タブ入力画面情報のチェック(全セットなしなら基準日セット) 
            If fn_Check_TuujyouFuriDate(InfoGakkou, Tuujyou_SchInfo, True) = False Then
                Exit Try
            End If

            '■随時振替日タブ入力画面情報のチェック
            If fn_Check_ZuijiFuriDate(InfoGakkou, Zuiji_SchInfo) = False Then
                Exit Try
            End If

            Dim SyoriFlg As Boolean = False

            '■通常スケジュールの作成
            For i As Integer = 1 To 5 Step 1
                '対象フラグが立っていたら処理
                If Tuujyou_SchInfo(i).TaisyouFlg = True Then
                    '学校と自振のスケジュール作成
                    If fn_InsertTuujyouG_SCHMAST(InfoGakkou, Tuujyou_SchInfo(i)) = False Then
                        Exit Try
                    End If
                End If
            Next

            '■随時スケジュールの作成
            For i As Integer = 1 To 6 Step 1
                '対象フラグが立っていたら処理
                If Zuiji_SchInfo(i).TaisyouFlg = True Then
                    '学校と自振のスケジュールの作成
                    If fn_InsertZuijiG_SCHMAST(InfoGakkou, Zuiji_SchInfo(i)) = False Then
                        Exit Try
                    End If
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ret = False
        Finally
            If ret = True Then
                'トランザクション終了（COMMIT）
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT失敗")
                    ret = False
                End If
            Else
                'トランザクション終了（ROLLBACK）
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK失敗")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

    '
    '　関数名　-  fn_InsertTuujyouG_SCHMAST
    '
    '　機能    -  通常スケジュール作成
    '
    '　引数    -  aInfoGakkou、aInfoTuujyou、SaiFuriOnlyFlg
    '
    '　備考    -  fn_InsertG_SCHMASTのサブ関数
    '
    '　
    Private Function fn_InsertTuujyouG_SCHMAST(ByVal aInfoGakkou As GAKDATA, _
                                               ByVal aInfoTuujyou As TuujyouData, _
                                               Optional ByVal SaiFuriOnlyFlg As Boolean = False) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_InsertTuujyouG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '処理日付を取得
            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '学年フラグの取得
            Dim GakunenFlg As String() = aInfoTuujyou.GetGakunenFlg()
            If GakunenFlg(0) = "err" Then

                ERRMSG = "学年対象フラグの取得に失敗しました。"
                Exit Try
            End If

            '各日を設定
            Dim Nengetu As String = aInfoTuujyou.Seikyu_Nen & aInfoTuujyou.Seikyu_Tuki
            '入力された振替日(営業日を考慮しない)
            Dim EntryFuriDate As String = "00000000"
            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            Dim Schkbn As Integer = 0 '通常
            Dim Furikbn As Integer = 0 '初振

            Dim ITAKU_CODE_S As String

            '再振データ作成用にループ
            For SyoriCnt As Integer = 0 To 1 Step 1

                '再振のみ作成の場合
                If SaiFuriOnlyFlg = True Then
                    SyoriCnt += 1
                End If

                '再振データ作成
                If SyoriCnt = 1 Then
                    Schkbn = 0 '通常
                    Furikbn = 1 '再振
                End If

                '振替日の抽出
                With aInfoTuujyou
                    Select Case Furikbn
                        Case 0
                            '初振用の設定
                            EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .Furikae_Date
                            '振替日算出
                            FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .Furikae_Date, Schkbn, Furikbn, aInfoGakkou)

                            '再振日の年の確定処理
                            If .SaiFurikae_Date <> "" Then

                                '翌月判定
                                Select Case True
                                    Case .Furikae_Date < .SaiFurikae_Date
                                        '営業日算出
                                        SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                                    Case .Furikae_Date > .SaiFurikae_Date
                                        If .Seikyu_Tuki = "12" Then
                                            '営業日算出
                                            SaiFuriDate = fn_GetEigyoubi(CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date, "0", "+")
                                        Else
                                            '営業日算出
                                            SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date, "0", "+")
                                        End If
                                    Case .Furikae_Date = .SaiFurikae_Date
                                        Call GSUB_LOG(0, "振替日異常:" & Furikbn)
                                        ERRMSG = "振替日と再振日が同一です"
                                        Exit Try

                                End Select
                            Else
                                SaiFuriDate = "00000000"
                            End If

                            ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
                        Case 1
                            '翌月判定
                            Select Case True
                                Case .Furikae_Date < .SaiFurikae_Date
                                    '再振用の設定
                                    EntryFuriDate = .Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date
                                    '振替日算出
                                    FuriDate = fn_GetFuriDate(.Seikyu_Nen, .Seikyu_Tuki, .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)

                                Case .Furikae_Date > .SaiFurikae_Date
                                    If .Seikyu_Tuki = "12" Then
                                        '再振用の設定
                                        EntryFuriDate = CStr(CInt(.Seikyu_Nen) + 1) & "01" & .SaiFurikae_Date
                                        '振替日算出
                                        FuriDate = fn_GetFuriDate(CStr(CInt(.Seikyu_Nen) + 1), "01", .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    Else
                                        '再振用の設定
                                        EntryFuriDate = .Seikyu_Nen & Format(CInt(.Seikyu_Tuki) + 1, "00") & .SaiFurikae_Date
                                        '振替日算出
                                        FuriDate = fn_GetFuriDate(.Seikyu_Nen, Format(CInt(.Seikyu_Tuki) + 1, "00"), .SaiFurikae_Date, Schkbn, Furikbn, aInfoGakkou)
                                    End If

                                Case .Furikae_Date = .SaiFurikae_Date
                                    Call GSUB_LOG(0, "振替日異常:" & Furikbn)
                                    ERRMSG = "振替日と再振日が同一です"
                                    Exit Try
                            End Select


                            '持越ありの場合のみ再振日を設定
                            If aInfoGakkou.SFURI_SYUBETU = "3" Then '再振あり、持越あり
                                '請求月が12月なら翌年で1月
                                If .Seikyu_Tuki = "12" Then
                                    .Seikyu_Nen = (CInt(.Seikyu_Nen) + 1).ToString
                                    .Seikyu_Tuki = "01"
                                Else
                                    .Seikyu_Tuki = (CInt(.Seikyu_Tuki) + 1).ToString
                                End If
                                '営業日算出
                                SaiFuriDate = fn_GetEigyoubi(.Seikyu_Nen & .Seikyu_Tuki & .SaiFurikae_Date, "0", "+")
                            Else
                                SaiFuriDate = "00000000"
                            End If

                            '委託者コード頭1桁をカウントアップ
                            ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
                        Case 2
                            '随時のみのためなし
                            ERRMSG = "学校自振スケジュール作成に失敗しました"
                            Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                            Exit Try
                        Case 3
                            '随時のみのためなし
                            Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                            ERRMSG = "学校自振スケジュール作成に失敗しました"
                            Exit Try
                        Case Else
                            Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                            ERRMSG = "学校自振スケジュール作成に失敗しました"
                            Exit Try
                    End Select

                End With

                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                '各種予定日の算出
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim CLS As New MAIN.ClsSchduleMaintenanceClass
                Call CLS.SetKyuzituInformation()

                '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
                CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
                'CLS.SetSchTable = CLS.APL.JifuriApplication
                '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<

                'スケジュール作成対象の取引先コードを抽出
                CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

                CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
                If CLS.SCH.FURI_DATE = "00000000" Then
                Else
                    CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
                End If

                Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

                Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

                Dim ENTRY_Y_DATE As String = "00000000"                                                   '明細作成予定日算出
                Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         'チェック予定日算出
                Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '振替データ作成予定日算出
                Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '不能結果更新予定日算出
                Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '資金決済予定日算出
                Dim HENKAN_Y_DATE As String = CLS.SCH.HENKAN_YDATE                                          '返還予定日

                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                'INSERT文作成
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim SQL As String

                SQL = ""
                SQL += " INSERT INTO G_SCHMAST "
                SQL += " VALUES ( "
                SQL += "'" & aInfoGakkou.GAKKOU_CODE & "'"        '1.学校コード
                SQL += ",'" & Nengetu & "'"                       '2.請求年月
                SQL += ",'" & Schkbn & "'"                        '3.スケジュール区分
                SQL += ",'" & Furikbn & "'"                       '4.振替区分
                SQL += ",'" & FuriDate & "'"                      '5.振替日
                SQL += ",'" & SaiFuriDate & "'"                   '6.再振替日
                '■通常スケジュールの学年フラグを配列に変換する
                SQL += ",'" & GakunenFlg(1) & "'"                 '7.学年1
                SQL += ",'" & GakunenFlg(2) & "'"                 '8.学年2
                SQL += ",'" & GakunenFlg(3) & "'"                 '9.学年3
                SQL += ",'" & GakunenFlg(4) & "'"                 '10.学年4
                SQL += ",'" & GakunenFlg(5) & "'"                 '11.学年5
                SQL += ",'" & GakunenFlg(6) & "'"                 '12.学年6
                SQL += ",'" & GakunenFlg(7) & "'"                 '13.学年7
                SQL += ",'" & GakunenFlg(8) & "'"                 '14.学年8
                SQL += ",'" & GakunenFlg(9) & "'"                 '15.学年9
                '■学校情報
                SQL += ",'" & ITAKU_CODE_S & "'"                  '16.委託者コード
                SQL += ",'" & aInfoGakkou.TKIN_CODE & "'"         '17.取扱金融機関
                SQL += ",'" & aInfoGakkou.TSIT_CODE & "'"         '18.取扱支店
                SQL += ",'" & aInfoGakkou.BAITAI_CODE & "'"       '19.媒体コード 
                SQL += ",'" & aInfoGakkou.TESUUTYO_KBN & "'"      '20.手数料区分 
                '■各予定日
                SQL += "," & "'" & ENTRY_Y_DATE & "'"             '21.明細作成予定日
                SQL += "," & "'00000000'"                         '22.明細作成日
                SQL += "," & "'" & CHECK_Y_DATE & "'"             '23.チェック予定日
                SQL += "," & "'00000000'"                         '24.チェック日
                SQL += "," & "'" & DATA_Y_DATE & "'"              '25.振替データ作成予定日
                SQL += "," & "'00000000'"                         '26.振替データ作成日
                SQL += "," & "'" & FUNOU_Y_DATE & "'"             '27.不能結果更新予定日
                SQL += "," & "'00000000'"                         '28.不能結果更新日
                '***追加返還日2010.02.21
                SQL += "," & "'" & HENKAN_Y_DATE & "'"            '29.返還予定日
                SQL += "," & "'00000000'"                         '30.返還日
                '***追加返還日2010.02.21
                SQL += "," & "'" & KESSAI_Y_DATE & "'"            '31.決済予定日
                SQL += "," & "'00000000'"                         '32.決済日
                '■各種フラグ
                SQL += "," & "'0'"                                '33.明細作成済フラグ
                SQL += "," & "'0'"                                '34.金額確認済フラグ
                SQL += "," & "'0'"                                '35.振替データ作成済フラグ
                SQL += "," & "'0'"                                '36.不能結果更新済フラグ
                SQL += "," & "'0'"                                '37.返還フラグ
                SQL += "," & "'0'"                                '38.再振データ作成済フラグ
                SQL += "," & "'0'"                                '39.決済済フラグ
                SQL += "," & "'0'"                                '40.中断フラグ
                '■件数金額
                SQL += "," & 0                                    '41.処理件数
                SQL += "," & 0                                    '42.処理金額
                SQL += "," & 0                                    '43.手数料
                SQL += "," & 0                                    '44.手数料1
                SQL += "," & 0                                    '45.手数料2
                SQL += "," & 0                                    '46.手数料3
                SQL += "," & 0                                    '47.振替済件数
                SQL += "," & 0                                    '48.振替済金額
                SQL += "," & 0                                    '49.不能件数
                SQL += "," & 0                                    '50.不能金額
                '■日付
                SQL += "," & "'" & SyoriDate(0) & "'"             '51.作成日付
                SQL += "," & "'" & SyoriDate(1) & "'"             '52.タイムスタンプ
                SQL += "," & "'" & fn_Hosei(EntryFuriDate) & "'"  '53.予備1(入力された振替日)
                SQL += "," & "'" & Space(30) & "'"                '54.予備2
                SQL += "," & "'" & Space(30) & "'"                '55.予備3
                SQL += "," & "'" & Space(30) & "'"                '56.予備4
                SQL += "," & "'" & Space(30) & "'"                '57.予備5
                SQL += "," & "'" & Space(30) & "'"                '58.予備6
                SQL += "," & "'" & Space(30) & "'"                '59.予備7
                SQL += "," & "'" & Space(30) & "'"                '60.予備8
                SQL += "," & "'" & Space(30) & "'"                '61.予備9
                SQL += "," & "'" & Space(30) & "')"               '62.予備10

                If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                    Call GSUB_LOG(0, "学校スケジュールInsert失敗、SQL:" & SQL)
                    ERRMSG = "学校スケジュール作成に失敗しました"
                    Exit Try
                End If

                '企業自振連携なしなら、自振スケジュールは作成しない
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                'INSERT文作成(企業自振分)
                '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
                Dim ToriF_Code As String

                Select Case Furikbn
                    Case 0 '初振
                        ToriF_Code = "01"
                    Case 1 '再振
                        ToriF_Code = "02"
                    Case 2 '随時入金
                        ToriF_Code = "03"
                    Case 3 '随時出金
                        ToriF_Code = "04"
                    Case Else
                        Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                        ERRMSG = "企業自振のスケジュール作成に失敗しました"
                        Exit Try
                End Select


                '@@@既に登録されているかチェック@@@
                'スケジュール存在チェック
                If fn_SchMastIsExist(ToriS_Code, ToriF_Code, FuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) <> True Then
                    '取引先マスタ存在チェック
                    If fn_ToriMastIsExist(ToriS_Code, ToriF_Code, OBJ_CONNECTION, OBJ_TRANSACTION) = True Then
                        '検索にヒットしなかったら
                        If fn_INSERTSCHMAST(ToriS_Code, ToriF_Code, FuriDate) = gintKEKKA.NG Then

                            ret = False
                            Call GSUB_LOG(0, "スケジュール登録:失敗" & Err.Description)
                            MessageBox.Show("企業自振のスケジュールが登録できませんでした", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Try
                        End If
                    End If
                End If

                '再振スケジュール作成判定
                If aInfoTuujyou.SaiFurikae_Check = False Then
                    Exit For
                End If
            Next

            ret = True

        Catch ex As Exception
            ERRMSG = "学校スケジュール作成に失敗しました"
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

        Return ret

    End Function
    '
    '　関数名　-  fn_InsertZuijiG_SCHMAST
    '
    '　機能    -  随時スケジュール作成
    '
    '　引数    -  aInfoGakkou、aInfoZuiji
    '
    '　備考    -  fn_InsertG_SCHMASTのサブ関数
    '　
    Private Function fn_InsertZuijiG_SCHMAST(ByVal aInfoGakkou As GAKDATA, ByVal aInfoZuiji As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_InsertZuijiG_SCHMAST"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            Dim SyoriDate(1) As String
            SyoriDate(0) = Format(Now, "yyyyMMdd")
            SyoriDate(1) = Format(Now, "yyyyMMddHHmmss")

            '学年フラグの取得
            Dim GakunenFlg As String() = aInfoZuiji.GetGakunenFlg
            If GakunenFlg(0) = "err" Then
                Exit Try
            End If
            '各日を設定
            Dim Nengetu As String = aInfoZuiji.Furikae_Nen & aInfoZuiji.Furikae_Tuki
            Dim Schkbn As String = "2"
            Dim Furikbn As String

            Dim ITAKU_CODE_S As String

            '入出金区分
            If aInfoZuiji.Nyusyutu_Kbn = "2" Then
                Furikbn = "2"

                '委託者コードの頭1桁をカウントアップしない
                ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
            Else
                Furikbn = "3"

                '委託者コードの頭1桁をカウントアップしない
                ITAKU_CODE_S = aInfoGakkou.ITAKU_CODE
            End If

            '入力された振替日(営業日の考慮をしない)
            Dim EntryFuriDate As String = aInfoZuiji.Furikae_Nen & aInfoZuiji.Furikae_Tuki & aInfoZuiji.Furikae_Date

            Dim FuriDate As String = "00000000"
            Dim SaiFuriDate As String = "00000000"

            '振替日の算出
            With aInfoZuiji
                FuriDate = fn_GetFuriDate(.Furikae_Nen, .Furikae_Tuki, .Furikae_Date, "2", Furikbn, aInfoGakkou) '振替日算出
                '再振は行なわない
                SaiFuriDate = "00000000"
            End With

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '各種予定日の算出
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<

            'スケジュール作成対象の取引先コードを抽出
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(FuriDate), aInfoGakkou.GAKKOU_CODE, "01")

            CLS.SCH.FURI_DATE = GCom.SET_DATE(FuriDate)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            Dim strFURI_DATE As String = CLS.SCH.FURI_DATE

            Dim BRet As Boolean = CLS.INSERT_NEW_SCHMAST(0, False, True)

            Dim ENTRY_Y_DATE As String = "00000000"                                                   '明細作成予定日算出
            Dim CHECK_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_CHECK, "-")         'チェック予定日算出
            Dim DATA_Y_DATE As String = fn_GetEigyoubi(FuriDate, STR_JIFURI_HAISIN, "-")    '振替データ作成予定日算出
            Dim FUNOU_Y_DATE As String = CLS.SCH.FUNOU_YDATE                                          '不能結果更新予定日算出
            Dim KESSAI_Y_DATE As String = CLS.SCH.KESSAI_YDATE                                        '資金決済予定日算出

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'INSERT文作成
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim SQL As String

            SQL = ""
            SQL += " INSERT INTO G_SCHMAST "
            SQL += " VALUES ( "
            SQL += "'" & aInfoGakkou.GAKKOU_CODE & "'"        '1.学校コード
            SQL += ",'" & Nengetu & "'"                       '2.請求年月
            SQL += ",'" & Schkbn & "'"                        '3.スケジュール区分
            SQL += ",'" & Furikbn & "'"                       '4.振替区分
            SQL += ",'" & FuriDate & "'"                      '5.振替日
            SQL += ",'" & SaiFuriDate & "'"                   '6.再振替日
            '■通常スケジュールの学年フラグを配列に変換する
            SQL += ",'" & GakunenFlg(1) & "'"                 '7.学年1
            SQL += ",'" & GakunenFlg(2) & "'"                 '8.学年2
            SQL += ",'" & GakunenFlg(3) & "'"                 '9.学年3
            SQL += ",'" & GakunenFlg(4) & "'"                 '10.学年4
            SQL += ",'" & GakunenFlg(5) & "'"                 '11.学年5
            SQL += ",'" & GakunenFlg(6) & "'"                 '12.学年6
            SQL += ",'" & GakunenFlg(7) & "'"                 '13.学年7
            SQL += ",'" & GakunenFlg(8) & "'"                 '14.学年8
            SQL += ",'" & GakunenFlg(9) & "'"                 '15.学年9
            '■学校情報
            SQL += ",'" & ITAKU_CODE_S & "'"                  '16.委託者コード
            SQL += ",'" & aInfoGakkou.TKIN_CODE & "'"         '17.取扱金融機関
            SQL += ",'" & aInfoGakkou.TSIT_CODE & "'"         '18.取扱支店
            SQL += ",'" & aInfoGakkou.BAITAI_CODE & "'"       '19.媒体コード 
            SQL += ",'" & aInfoGakkou.TESUUTYO_KBN & "'"      '20.手数料区分 
            '■各予定日
            SQL += "," & "'" & ENTRY_Y_DATE & "'"             '21.明細作成予定日
            SQL += "," & "'00000000'"                         '22.明細作成日
            SQL += "," & "'" & CHECK_Y_DATE & "'"             '23.チェック予定日
            SQL += "," & "'00000000'"                         '24.チェック日
            SQL += "," & "'" & DATA_Y_DATE & "'"              '25.振替データ作成予定日
            SQL += "," & "'00000000'"                         '26.振替データ作成日
            SQL += "," & "'" & FUNOU_Y_DATE & "'"             '27.不能結果更新予定日
            SQL += "," & "'00000000'"                         '28.不能結果更新日
            SQL += "," & "'" & KESSAI_Y_DATE & "'"            '31.決済予定日
            SQL += "," & "'00000000'"                         '32.決済日
            '■各種フラグ
            SQL += "," & "'0'"                                '33.明細作成済フラグ
            SQL += "," & "'0'"                                '34.金額確認済フラグ
            SQL += "," & "'0'"                                '35.振替データ作成済フラグ
            SQL += "," & "'0'"                                '36.不能結果更新済フラグ
            SQL += "," & "'0'"                                '38.再振データ作成済フラグ
            SQL += "," & "'0'"                                '39.決済済フラグ
            SQL += "," & "'0'"                                '40.中断フラグ
            '■件数金額
            SQL += "," & 0                                    '41.処理件数
            SQL += "," & 0                                    '42.処理金額
            SQL += "," & 0                                    '43.手数料
            SQL += "," & 0                                    '44.手数料1
            SQL += "," & 0                                    '45.手数料2
            SQL += "," & 0                                    '46.手数料3
            SQL += "," & 0                                    '47.振替済件数
            SQL += "," & 0                                    '48.振替済金額
            SQL += "," & 0                                    '49.不能件数
            SQL += "," & 0                                    '50.不能金額
            '■日付
            SQL += "," & "'" & SyoriDate(0) & "'"             '51.作成日付
            SQL += "," & "'" & SyoriDate(1) & "'"             '52.タイムスタンプ
            SQL += "," & "'" & fn_Hosei(EntryFuriDate) & "'"  '53.予備1(入力振替日)
            SQL += "," & "'" & Space(15) & "'"                '54.予備2
            SQL += "," & "'" & Space(15) & "'"                '55.予備3
            SQL += "," & "'" & Space(15) & "'"                '56.予備4
            SQL += "," & "'" & Space(15) & "')"               '57.予備5

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "学校スケジュールInsert失敗、SQL:" & SQL)
                ERRMSG = "学校スケジュール作成に失敗しました"
                Exit Try
            End If

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            'INSERT文作成(企業自振分)
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

            '企業自振連携時のみ
            Dim ToriS_Code As String = aInfoGakkou.GAKKOU_CODE
            Dim ToriF_Code As String

            Select Case Furikbn
                Case 0 '初振
                    ToriF_Code = "01"
                Case 1 '再振
                    ToriF_Code = "02"
                Case 2 '随時入金
                    ToriF_Code = "03"
                Case 3 '随時出金
                    ToriF_Code = "04"
                Case Else
                    Call GSUB_LOG(0, "振替区分異常:" & Furikbn)
                    ERRMSG = "企業自振のスケジュール作成に失敗しました"
                    Exit Try
            End Select


            '@@@既に登録されているかチェック@@@
            'スケジュール存在チェック
            If fn_SchMastIsExist(ToriS_Code, ToriF_Code, FuriDate, OBJ_CONNECTION, OBJ_TRANSACTION) <> True Then
                '取引先マスタ存在チェック
                If fn_ToriMastIsExist(ToriS_Code, ToriF_Code, OBJ_CONNECTION, OBJ_TRANSACTION) = True Then
                    '検索にヒットしなかったら
                    If fn_INSERTSCHMAST(ToriS_Code, ToriF_Code, FuriDate) = gintKEKKA.NG Then

                        ret = False
                        Call GSUB_LOG(0, "振替日:" & FuriDate & "企業自振スケジュール作成失敗")
                        ERRMSG = "企業自振のスケジュール作成に失敗しました"
                        Exit Try
                    End If
                End If
            End If

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

        Return ret

    End Function
#End Region
#Region "DELETE G_SCHMAST"
    '
    '　関数名　-　fn_DeleteG_SCHMAST
    '
    '　機能    -  個別のスケジュールの削除
    '
    '　引数    -  aGakkouCode,aSchkbn,aFurikbn,aFuriDate,aNengetudo
    '
    '　備考    -  
    '
    '　
    Private Function fn_DeleteG_SCHMAST(ByVal aGakkouCode As String, _
                                        ByVal aSchkbn As Integer, _
                                        ByVal aFurikbn As Integer, _
                                        ByVal aFuriDate As String, _
                                        ByVal aNengetudo As String) As Boolean

        Dim ret As Boolean = False

        Try
            Dim InfoG_SCHMAST As New G_SCHMASTDATA

            If Not InfoG_SCHMAST.SetG_SCHMASTDATA(aGakkouCode, aSchkbn, aFurikbn, aFuriDate, MainDB, aNengetudo) Then
                ERRMSG = "スケジュール取得に失敗しました"
                Exit Try
            End If

            With InfoG_SCHMAST
                If .ENTRI_FLG_S = "1" OrElse _
                .CHECK_FLG_S = "1" OrElse _
                .DATA_FLG_S = "1" OrElse _
                .FUNOU_FLG_S = "1" OrElse _
                .SAIFURI_FLG_S = "1" OrElse _
                .KESSAI_FLG_S = "1" OrElse _
                .TYUUDAN_FLG_S = "1" Then

                    ERRMSG = "処理中のスケジュールが存在するため削除できません。"
                    Exit Try
                End If
            End With

            Dim SQL As String

            SQL = "  DELETE FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'"          '学校コード
            SQL &= " AND   NENGETUDO_S = '" & aNengetudo & "'"             '年月度
            SQL &= " AND   SCH_KBN_S = '" & aSchkbn.ToString & "'"         'スケジュール区分
            SQL &= " AND   FURI_KBN_S = '" & aFurikbn.ToString & "'"       '振替区分
            SQL &= " AND   FURI_DATE_S = '" & aFuriDate & "'"              '振替日

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "学校スケジュール削除失敗、SQL:" & SQL)
                ERRMSG = "学校スケジュール削除に失敗しました"
                Exit Try
            End If

            Dim ToriS_Code As String = aGakkouCode
            Dim ToriF_Code As String

            Select Case aFurikbn
                Case 0 '初振
                    ToriF_Code = "01"
                Case 1 '再振
                    ToriF_Code = "02"
                Case 2 '随時入金
                    ToriF_Code = "03"
                Case 3 '随時出金
                    ToriF_Code = "04"
                Case Else
                    Call GSUB_LOG(0, "振替区分異常:" & aFurikbn)
                    ERRMSG = "振替区分の取得に失敗しました"
                    Exit Try
            End Select

            SQL = "  DELETE FROM SCHMAST "
            SQL &= " WHERE TORIS_CODE_S = '" & ToriS_Code & "'" '取引先主コード
            SQL &= " AND   TORIF_CODE_S = '" & ToriF_Code & "'" '取引先副コード
            SQL &= " AND   FURI_DATE_S = '" & aFuriDate & "'"   '振替日

            If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                Call GSUB_LOG(0, "学校スケジュール削除失敗、SQL:" & SQL)
                ERRMSG = "企業自振スケジュール削除に失敗しました"
                Exit Try
            End If

            ret = True

        Catch ex As Exception
            ret = False
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "学校スケジュール削除に失敗しました"
        End Try

        Return ret

    End Function

    Private Function fn_DeleteG_SCHMAST_ALL(ByVal aNENGETSUDO As String, ByVal aGakkouCode As String) As Boolean

        Dim ret As Boolean = False
        Dim SSQL As StringBuilder 'SELECT用

        Dim ArrayFuriDate As New ArrayList

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            '■トランザクション開始
            If GFUNC_EXECUTESQL_TRANS("", 0) = False Then
                Exit Try
            End If

            '初振を削除したかどうか
            Dim DelFlg As Boolean = False
            Dim InfoSaifuriSch As New G_SCHMASTDATA

            Orareader = New CASTCommon.MyOracleReader(MainDB)
            SSQL = New StringBuilder(128)

            'まず対象学校のスケジュールを取得
            '条件は通常(初振)、随時(入金、出金)　
            SSQL.Append(" SELECT * FROM G_SCHMAST ")
            SSQL.Append(" WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'")
            SSQL.Append(" AND SCH_KBN_S IN ('0','2') ")
            SSQL.Append(" AND FURI_KBN_S IN ('0','2','3') ")
            SSQL.Append(" AND NENGETUDO_S = '" & aNENGETSUDO & "'")

            If Orareader.DataReader(SSQL) = True Then
                While Orareader.EOF = False
                    '削除フラグの初期化
                    DelFlg = False

                    '処理中かどうか
                    If Orareader.GetItem("ENTRI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("CHECK_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("DATA_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("FUNOU_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("SAIFURI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("KESSAI_FLG_S").Trim = "1" OrElse _
                    Orareader.GetItem("TYUUDAN_FLG_S").Trim = "1" Then
                        '処理中なら削除しない
                        DelFlg = False
                    Else
                        '未処理なら削除
                        If Not fn_DeleteG_SCHMAST(aGakkouCode, _
                        Orareader.GetItem("SCH_KBN_S"), _
                        Orareader.GetItem("FURI_KBN_S"), _
                        Orareader.GetItem("FURI_DATE_S"), _
                        aNENGETSUDO) Then
                            Exit Try
                        End If

                        '削除した場合はフラグを立てる
                        DelFlg = True
                    End If

                    'スケジュール区分が初振かつ再振日が"000000000"でない場合
                    If Orareader.GetItem("SCH_KBN_S") = "0" AndAlso _
                    Orareader.GetItem("SFURI_DATE_S").Trim <> "00000000" Then

                        '再振スケジュールの取得
                        If Not InfoSaifuriSch.SetG_SCHMASTDATA(aGakkouCode, 0, 1, Orareader.GetItem("SFURI_DATE_S"), MainDB) Then
                            ERRMSG = "再振スケジュール取得に失敗しました"
                            Exit Try
                        End If

                        '再振が処理中かどうか判定
                        With InfoSaifuriSch
                            If .ENTRI_FLG_S = "1" OrElse _
                            .CHECK_FLG_S = "1" OrElse _
                            .DATA_FLG_S = "1" OrElse _
                            .FUNOU_FLG_S = "1" OrElse _
                            .SAIFURI_FLG_S = "1" OrElse _
                            .KESSAI_FLG_S = "1" OrElse _
                            .TYUUDAN_FLG_S = "1" Then

                            Else
                                '未処理なら削除
                                If Not fn_DeleteG_SCHMAST(aGakkouCode, 0, 1, .FURI_DATE_S, aNENGETSUDO) Then
                                    Exit Try
                                End If

                                '初振を削除していなければ、初振の再振日をUPDATE
                                If DelFlg = False Then
                                    Dim SQL As String = ""
                                    SQL = " UPDATE  G_SCHMAST SET "
                                    SQL &= " SFURI_DATE_S = '00000000'"
                                    SQL &= " WHERE GAKKOU_CODE_S = '" & aGakkouCode & "'"
                                    SQL &= " AND   NENGETUDO_S = '" & aNENGETSUDO & "'"
                                    SQL &= " AND   SCH_KBN_S = '0'"
                                    SQL &= " AND   FURI_KBN_S = '0'"
                                    SQL &= " AND   FURI_DATE_S = '" & Orareader.GetItem("FURI_DATE_S") & "'"

                                    If GFUNC_EXECUTESQL_TRANS(SQL, 1) = False Then
                                        Call GSUB_LOG(0, "学校スケジュール削除失敗、SQL:" & SQL)
                                        ERRMSG = "学校スケジュール初振更新に失敗しました"
                                        Exit Try
                                    End If
                                End If
                            End If
                        End With
                    End If

                    Orareader.NextRead()
                End While
            End If

            Orareader.Close()
            Orareader = Nothing

            ret = True

        Catch ex As Exception
            ERRMSG = "削除に失敗しました"
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
            End If

            If ret = True Then
                'トランザクション終了（COMMIT）
                If GFUNC_EXECUTESQL_TRANS("", 2) = False Then
                    Call GSUB_LOG(0, "COMMIT失敗")
                    ret = False
                End If
            Else
                'トランザクション終了（ROLLBACK）
                If GFUNC_EXECUTESQL_TRANS("", 3) = False Then
                    Call GSUB_LOG(0, "ROLLBACK失敗")
                    ret = False
                End If
            End If
        End Try

        Return ret

    End Function

#End Region
#Region "DB情報取得"                     '
    '　関数名　-　GetALLGakkouCode
    '
    '　機能    -  全学校コードの取得
    '
    '　引数    -  
    '
    '　備考    -  
    '
    Private Function GetALLGakkouCode() As String()

        Dim Orareader As CASTCommon.MyOracleReader = Nothing

        Try
            '***ログ情報***
            STR_COMMAND = "GetALLGakkouCode"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            Orareader = New CASTCommon.MyOracleReader(MainDB)

            Dim SQL As String
            Dim cnt As Integer

            SQL = "SELECT COUNT(*) FROM GAKMAST2"

            If Orareader.DataReader(SQL) = False Then
                Return Nothing
            Else
                cnt = CInt(Orareader.GetItem("COUNT(*)"))
                If cnt = 0 Then
                    Exit Try
                End If
            End If

            Orareader.Close()
            Orareader = Nothing
            Orareader = New CASTCommon.MyOracleReader(MainDB)

            Dim GakkouCode(cnt - 1) As String '結果配列

            SQL = "SELECT GAKKOU_CODE_T FROM GAKMAST2"

            If Orareader.DataReader(SQL) = False Then
            Else
                cnt = 0
                While Orareader.EOF = False
                    GakkouCode(cnt) = Orareader.GetItem("GAKKOU_CODE_T")

                    cnt += 1
                    Orareader.NextRead()
                End While

                Return GakkouCode '正常時
            End If

            Orareader.Close()
            Orareader = Nothing

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        Finally
            If Not Orareader Is Nothing Then
                Orareader.Close()
            End If
        End Try

        Return Nothing '異常

    End Function
    '
    '　関数名　-　fn_Common_Check
    '
    '　機能    -  必須項目の入力チェック
    '
    '　引数    -  mode 1:作成時、2:参照時、3:更新時、4:削除時
    '
    '　備考    -  
    '
    Private Function fn_Common_Check(ByVal mode As Integer) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_Common_Check"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '■年度、月度入力値チェック
            Select Case True
                Case txt対象年度.Text.Trim = "" '***対象年度***
                    MessageBox.Show("年度が入力されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Exit Try

                Case txt対象年度.Text.Trim.Length <> 4
                    MessageBox.Show("年度の入力が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Exit Try

                Case Not IsNumeric(txt対象年度.Text)
                    MessageBox.Show("年度の入力が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Exit Try

                Case txt対象月.Text.Trim = "" '***対象月***
                    MessageBox.Show("月度が入力されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象月.Focus()
                    Exit Try

                Case txt対象月.Text.Trim.Length <> 2
                    MessageBox.Show("月度の入力が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象月.Focus()
                    Exit Try


                Case Not IsNumeric(txt対象月.Text)
                    MessageBox.Show("月度の入力が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象月.Focus()
                    Exit Try

                Case Not IsDate(Me.txt対象年度.Text & "/" & Me.txt対象月.Text & "/" & "01")
                    MessageBox.Show("年月度の入力が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Exit Try
            End Select

            Dim sStart As String
            Dim sEnd As String

            '■学校コード存在チェック
            If txtGakkou_Code.Text.Trim = "" Then
                MessageBox.Show("学校コードが入力されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtGakkou_Code.Focus()
                Exit Try
            Else
                '数値チェック
                If IsNumeric(txtGakkou_Code.Text) = False Then
                    MessageBox.Show("学校コードの入力値が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try

                End If

                '桁数チェック
                If txtGakkou_Code.Text.Trim.Length <> 10 Then
                    MessageBox.Show("学校コードの入力値が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try

                End If

                '作成時と削除時は学校コードのALL9を許す
                If txtGakkou_Code.Text.Trim = "9999999999" Then
                    Select Case mode
                        Case 1, 4
                            '学校コードがALL9かつ作成時と削除時は学校存在チェックをしない
                            '正常終了
                            ret = True
                            Exit Try
                        Case Else
                            MessageBox.Show("学校コードの入力値が不正です。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            txtGakkou_Code.Focus()
                            Exit Try
                    End Select
                End If

                Dim SQL As String

                '学校マスタ存在チェック
                SQL = "  SELECT *"
                SQL += " FROM GAKMAST2"
                SQL += " WHERE GAKKOU_CODE_T = '" & txtGakkou_Code.Text.Trim.PadLeft(txtGakkou_Code.MaxLength, "0"c) & "'"

                If GFUNC_SELECT_SQL2(SQL, 0) = False Then
                    MessageBox.Show("学校マスタの検索に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    Exit Try
                End If

                If OBJ_DATAREADER.HasRows = False Then
                    MessageBox.Show("入力された学校コードが存在しません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txtGakkou_Code.Focus()
                    Exit Try
                End If

                OBJ_DATAREADER.Read()

                sStart = Mid(OBJ_DATAREADER.Item("KAISI_DATE_T"), 1, 4)
                sEnd = Mid(OBJ_DATAREADER.Item("SYURYOU_DATE_T"), 1, 4)

                If (sStart <= txt対象年度.Text >= sEnd) = False Then
                    MessageBox.Show("対象年度が入力範囲外です(" & sStart & "～" & sEnd & ")", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    txt対象年度.Focus()
                    Exit Try
                End If

            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show("学校マスタの検索に失敗しました。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Call GSUB_LOG(0, "mode:" & mode & "予期せぬエラー:" & ex.ToString)
        Finally
            If OBJ_DATAREADER.IsClosed = False Then
                Call GFUNC_SELECT_SQL2("", 1)
            End If
        End Try

        Return ret

    End Function
    '
    '　関数名　-　fn_IsExistG_SCHMASTNENGATUDO
    '
    '　機能    -  スケジュールの存在チェック(年月度でチェック)
    '
    '　引数    -  Gakkou_Code,year,month
    '
    '　備考    -  yyyyMM
    '
    Private Function fn_IsExistG_SCHMAST_NENGATUDO(ByVal Gakkou_Code As String, ByVal year As String, ByVal month As String) As Boolean

        Try
            '***ログ情報***
            STR_COMMAND = "fn_IsExistG_SCHMAST_NENGATUDO"
            STR_LOG_GAKKOU_CODE = Gakkou_Code
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            Dim nengetudo As String = year & month
            Dim SQL As String

            SQL = "  SELECT * FROM G_SCHMAST "
            SQL &= " WHERE GAKKOU_CODE_S = '" & Gakkou_Code & "'"
            SQL &= " AND NENGETUDO_S = '" & nengetudo & "'"


            If GFUNC_ISEXIST(SQL) = True Then
                Return True
            Else
                Return False
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

        Return False

    End Function
    '
    '　関数名　-　fn_HolidayListSet
    '
    '　機能    -  休日マスタ取得
    '
    '　引数    -  
    '
    '　備考    -  
    '
    Private Function fn_HolidayListSet() As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_HolidayListSet"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            Dim SQL As String

            '休日情報の表示
            Dim sTuki As String
            Dim sDay As String
            Dim sYName As String

            lst休日.Items.Clear()

            If Trim(txt対象年度.Text) <> "" Then
                Select Case CInt(txt対象年度.Text)
                    Case Is > 1900
                        SQL = "  SELECT"
                        SQL += " YASUMI_DATE_Y"
                        SQL += ",YASUMI_NAME_Y"
                        SQL += " FROM YASUMIMAST"
                        SQL += " WHERE"
                        SQL += " YASUMI_DATE_Y > '" & txt対象年度.Text & "0400'"
                        SQL += " AND"
                        SQL += " YASUMI_DATE_Y < '" & CStr(CInt(txt対象年度.Text) + 1) & "0399'"
                        SQL += " ORDER BY YASUMI_DATE_Y ASC"

                        If GFUNC_SELECT_SQL2(SQL, 0) = False Then
                            MessageBox.Show("対象年度の休日情報が登録されていません。", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                            Exit Try
                        End If

                        While (OBJ_DATAREADER.Read = True)
                            With OBJ_DATAREADER
                                sTuki = Mid(.Item("YASUMI_DATE_Y"), 5, 2)
                                sDay = Mid(.Item("YASUMI_DATE_Y"), 7, 2)
                                sYName = Trim(.Item("YASUMI_NAME_Y"))

                                lst休日.Items.Add(sTuki & "月" & sDay & "日" & Space(1) & sYName)

                                StrYasumi_List(StrYasumi_List.Length - 1) = txt対象年度.Text & sTuki & sDay
                                ReDim Preserve StrYasumi_List(StrYasumi_List.Length)
                            End With
                        End While

                        ReDim Preserve StrYasumi_List(StrYasumi_List.Length - 1)

                    Case Else
                        MessageBox.Show("対象年度は1900年以降を入力してください", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        txt対象年度.Focus()
                        Exit Try
                End Select
            End If

            ret = True

        Catch ex As Exception
            MessageBox.Show("休日情報の取得に失敗しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        Finally
            If OBJ_DATAREADER.IsClosed = False Then
                Call GFUNC_SELECT_SQL2("", 1)
            End If
        End Try

        Return ret

    End Function
    '
    '　関数名　-　fn_IsExistYASUMIMAST
    '
    '　機能    -  休日マスタ存在チェック
    '
    '　引数    -  Str年月日
    '
    '　備考    -  
    '　
    Private Function fn_IsExistYASUMIMAST(ByVal Str年月日 As String) As Boolean

        '休日マスタ存在チェック
        fn_IsExistYASUMIMAST = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_IsExistYASUMIMAST"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            For cnt As Integer = 0 To StrYasumi_List.Length - 1
                If Not StrYasumi_List(cnt) Is Nothing Then
                    If StrYasumi_List(cnt).Trim = Str年月日 Then
                        fn_IsExistYASUMIMAST = True
                        Exit For
                    End If
                End If
            Next

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

    End Function

#End Region
#Region "日付算出"
    '
    '　関数名　-　fn_GetFuriDate
    '
    '　機能    -  振替日の休日補正を行なう
    '
    '　引数    -  StrFuriYear、StrFuriMonth、StrFuriDay、StrSchKbn、StrFuriKbn、aInfoGakkou
    '
    '　備考    -  
    '
    '　
    Private Function fn_GetFuriDate(ByVal StrFuriYear As String, _
                                    ByVal StrFuriMonth As String, _
                                    ByVal StrFuriDay As String, _
                                    ByVal StrSchKbn As String, _
                                    ByVal StrFuriKbn As String, _
                                    ByVal aInfoGakkou As GAKDATA) As String

        Dim FuriDate As String = "err"

        Try
            '***ログ情報***
            STR_COMMAND = "fn_GetFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '振替日を設定
            Select Case StrSchKbn
                Case "0" '通常
                    '振替日が未入力の場合
                    If StrFuriDay = "" Then
                        Select Case StrFuriKbn
                            Case "0" '初振
                                FuriDate = StrFuriYear & StrFuriMonth & aInfoGakkou.FURI_DATE
                            Case "1" '再振
                                FuriDate = StrFuriYear & StrFuriMonth & aInfoGakkou.SFURI_DATE
                        End Select
                    Else
                        FuriDate = StrFuriYear & StrFuriMonth & StrFuriDay
                    End If
                Case "2"     '随時
                    FuriDate = StrFuriYear & StrFuriMonth & StrFuriDay
            End Select

            '振替日休日補正
            Select Case StrFuriKbn
                Case "0", "1", "2"
                    '振替日休日補正(出金休日コードで補正)
                    Select Case aInfoGakkou.SKYU_CODE_T
                        Case "0"
                            '翌営業日
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "+")
                        Case "1"
                            '前営業日
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "-")
                    End Select
                Case "3"
                    '振替日休日補正(入金休日コードで補正)
                    Select Case aInfoGakkou.NKYU_CODE_T
                        Case "0"
                            '翌営業日
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "+")
                        Case "1"
                            '前営業日
                            FuriDate = fn_GetEigyoubi(FuriDate, "0", "-")
                    End Select
            End Select

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            FuriDate = "err"
        End Try

        Return FuriDate

    End Function

#End Region
#Region "画面制御"
    '
    '　関数名　-　Sb_Btn_Enable
    '
    '　機能    -  ボタン制御 
    '
    '　引数    -  pIndex
    '
    '　備考    -  
    '
    Private Sub Sb_Btn_Enable(Optional ByVal pIndex As Integer = 0)

        Select Case pIndex
            Case 0
                btnAction.Enabled = True
                btnFind.Enabled = True
                btnUPDATE.Enabled = False
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt対象年度.Enabled = True
                txt対象月.Enabled = True
            Case 1
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUPDATE.Enabled = True
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = False
                cmbGakkouName.Enabled = False
                cmbKana.Enabled = False
                txt対象年度.Enabled = False
                txt対象月.Enabled = False
            Case 2
                btnAction.Enabled = False
                btnFind.Enabled = True
                btnUPDATE.Enabled = False
                btnEraser.Enabled = True
                txtGakkou_Code.Enabled = True
                cmbGakkouName.Enabled = True
                cmbKana.Enabled = True
                txt対象年度.Enabled = True
                txt対象月.Enabled = True
        End Select

    End Sub
    '
    '　関数名　-　Sb_Sansyou_Focus
    '
    '　機能    -  スケジュールの存在チェックを行い、存在すれば、参照にフォーカスをセット
    '
    '　引数    -  
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_Sansyou_Focus()

        Dim SQL As String

        SQL = "  SELECT * FROM G_SCHMAST"
        SQL += " WHERE"
        SQL += " GAKKOU_CODE_S ='" & Trim(txtGakkou_Code.Text) & "'"
        SQL += " AND"
        SQL += " NENGETUDO_S ='" & Trim(txt対象年度.Text) & txt対象月.Text.Trim & "'"

        If GFUNC_ISEXIST(SQL) = True Then
            btnAction.Enabled = False
            btnFind.Enabled = True
            btnFind.Focus()
            Me.btnUPDATE.Enabled = False
        Else '追加 2007/02/15
            btnFind.Enabled = False
            btnAction.Enabled = True
            btnAction.Focus()
            Me.btnUPDATE.Enabled = False
        End If

    End Sub
    '
    '　関数名　-　Sb_AllGakunenChkBox_Control
    '
    '　機能    -  チェックボックスのコントロール
    '
    '　引数    - 
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_AllGakunenChkBox_Control(ByVal SIYOU_GAKUNEN As Integer, _
                                            ByVal ChkBoxAll As CheckBox, _
                                            ByVal ChkBox1 As CheckBox, _
                                            ByVal ChkBox2 As CheckBox, _
                                            ByVal ChkBox3 As CheckBox, _
                                            ByVal ChkBox4 As CheckBox, _
                                            ByVal ChkBox5 As CheckBox, _
                                            ByVal ChkBox6 As CheckBox, _
                                            ByVal ChkBox7 As CheckBox, _
                                            ByVal ChkBox8 As CheckBox, _
                                            ByVal ChkBox9 As CheckBox)

        If ChkBoxAll.Checked = True Then
            '各学年のチェックｏｆｆ
            ChkBox1.Checked = False
            ChkBox2.Checked = False
            ChkBox3.Checked = False
            ChkBox4.Checked = False
            ChkBox5.Checked = False
            ChkBox6.Checked = False
            ChkBox7.Checked = False
            ChkBox8.Checked = False
            ChkBox9.Checked = False
            '各学年のチェックボックス使用不可 
            ChkBox1.Enabled = False
            ChkBox2.Enabled = False
            ChkBox3.Enabled = False
            ChkBox4.Enabled = False
            ChkBox5.Enabled = False
            ChkBox6.Enabled = False
            ChkBox7.Enabled = False
            ChkBox8.Enabled = False
            ChkBox9.Enabled = False
        Else
            ChkBox1.Enabled = True
            ChkBox2.Enabled = True
            ChkBox3.Enabled = True
            ChkBox4.Enabled = True
            ChkBox5.Enabled = True
            ChkBox6.Enabled = True
            ChkBox7.Enabled = True
            ChkBox8.Enabled = True
            ChkBox9.Enabled = True

            '各学年のチェックボックス使用可 
            If SIYOU_GAKUNEN = 0 Then
                Exit Sub
            End If

            If SIYOU_GAKUNEN < 9 Then
                ChkBox9.Enabled = False
            End If

            If SIYOU_GAKUNEN < 8 Then
                ChkBox8.Enabled = False
            End If

            If SIYOU_GAKUNEN < 7 Then
                ChkBox7.Enabled = False
            End If

            If SIYOU_GAKUNEN < 6 Then
                ChkBox6.Enabled = False
            End If

            If SIYOU_GAKUNEN < 5 Then
                ChkBox5.Enabled = False
            End If

            If SIYOU_GAKUNEN < 4 Then
                ChkBox4.Enabled = False
            End If

            If SIYOU_GAKUNEN < 3 Then
                ChkBox3.Enabled = False
            End If

            If SIYOU_GAKUNEN < 2 Then
                ChkBox2.Enabled = False
            End If

            ChkBox1.Enabled = True

        End If
    End Sub
    '
    '　関数名　-　Sb_SaifuriProtect
    '
    '　機能    -  有効再振日の制御
    '
    '　引数    -  pValue 制御情報(True,False) 
    '　　　　　　 
    '　備考    -  
    '
    Private Sub Sb_SaifuriProtect(ByVal pValue As Boolean)

        '振替日有効チェックと振替日入力欄のプロテクト(ON/OFF)処理
        '通常振替日タブ
        Chk有効再振日通常1.Checked = False
        Chk有効再振日通常1.Enabled = pValue
        txt通常再振日1.Enabled = pValue
        Chk有効再振日通常2.Checked = False
        Chk有効再振日通常2.Enabled = pValue
        txt通常再振日2.Enabled = pValue
        Chk有効再振日通常3.Checked = False
        Chk有効再振日通常3.Enabled = pValue
        txt通常再振日3.Enabled = pValue
        Chk有効再振日通常4.Checked = False
        Chk有効再振日通常4.Enabled = pValue
        txt通常再振日4.Enabled = pValue
        Chk有効再振日通常5.Checked = False
        Chk有効再振日通常5.Enabled = pValue
        txt通常再振日5.Enabled = pValue
    End Sub

    '　関数名　-　Sb_SiyouGakunenChkEnabled
    '
    '　機能    -  使用していない学年のチェックボックスを使用不可にする
    '
    '　引数    -  SiyouGakunen
    '
    '　備考    -  画面上に項目追加
    '
    Private Sub Sb_SiyouGakunenChkEnabled(ByVal SiyouGakunen As Integer)

        If SiyouGakunen = 0 Then
            Exit Sub
        End If

        If SiyouGakunen < 9 Then
            Chk学年通常1_9.Enabled = False
            Chk学年通常2_9.Enabled = False
            Chk学年通常3_9.Enabled = False
            Chk学年通常4_9.Enabled = False
            Chk学年通常5_9.Enabled = False

            Chk随時1_9学年.Enabled = False
            Chk随時2_9学年.Enabled = False
            Chk随時3_9学年.Enabled = False
            Chk随時4_9学年.Enabled = False
            Chk随時5_9学年.Enabled = False
            Chk随時6_9学年.Enabled = False
        End If

        If SiyouGakunen < 8 Then
            Chk学年通常1_8.Enabled = False
            Chk学年通常2_8.Enabled = False
            Chk学年通常3_8.Enabled = False
            Chk学年通常4_8.Enabled = False
            Chk学年通常5_8.Enabled = False

            Chk随時1_8学年.Enabled = False
            Chk随時2_8学年.Enabled = False
            Chk随時3_8学年.Enabled = False
            Chk随時4_8学年.Enabled = False
            Chk随時5_8学年.Enabled = False
            Chk随時6_8学年.Enabled = False
        End If

        If SiyouGakunen < 7 Then
            Chk学年通常1_7.Enabled = False
            Chk学年通常2_7.Enabled = False
            Chk学年通常3_7.Enabled = False
            Chk学年通常4_7.Enabled = False
            Chk学年通常5_7.Enabled = False

            Chk随時1_7学年.Enabled = False
            Chk随時2_7学年.Enabled = False
            Chk随時3_7学年.Enabled = False
            Chk随時4_7学年.Enabled = False
            Chk随時5_7学年.Enabled = False
            Chk随時6_7学年.Enabled = False
        End If

        If SiyouGakunen < 6 Then
            Chk学年通常1_6.Enabled = False
            Chk学年通常2_6.Enabled = False
            Chk学年通常3_6.Enabled = False
            Chk学年通常4_6.Enabled = False
            Chk学年通常5_6.Enabled = False

            Chk随時1_6学年.Enabled = False
            Chk随時2_6学年.Enabled = False
            Chk随時3_6学年.Enabled = False
            Chk随時4_6学年.Enabled = False
            Chk随時5_6学年.Enabled = False
            Chk随時6_6学年.Enabled = False
        End If

        If SiyouGakunen < 5 Then
            Chk学年通常1_5.Enabled = False
            Chk学年通常2_5.Enabled = False
            Chk学年通常3_5.Enabled = False
            Chk学年通常4_5.Enabled = False
            Chk学年通常5_5.Enabled = False

            Chk随時1_5学年.Enabled = False
            Chk随時2_5学年.Enabled = False
            Chk随時3_5学年.Enabled = False
            Chk随時4_5学年.Enabled = False
            Chk随時5_5学年.Enabled = False
            Chk随時6_5学年.Enabled = False
        End If

        If SiyouGakunen < 4 Then
            Chk学年通常1_4.Enabled = False
            Chk学年通常2_4.Enabled = False
            Chk学年通常3_4.Enabled = False
            Chk学年通常4_4.Enabled = False
            Chk学年通常5_4.Enabled = False

            Chk随時1_4学年.Enabled = False
            Chk随時2_4学年.Enabled = False
            Chk随時3_4学年.Enabled = False
            Chk随時4_4学年.Enabled = False
            Chk随時5_4学年.Enabled = False
            Chk随時6_4学年.Enabled = False
        End If

        If SiyouGakunen < 3 Then
            Chk学年通常1_3.Enabled = False
            Chk学年通常2_3.Enabled = False
            Chk学年通常3_3.Enabled = False
            Chk学年通常4_3.Enabled = False
            Chk学年通常5_3.Enabled = False

            Chk随時1_3学年.Enabled = False
            Chk随時2_3学年.Enabled = False
            Chk随時3_3学年.Enabled = False
            Chk随時4_3学年.Enabled = False
            Chk随時5_3学年.Enabled = False
            Chk随時6_3学年.Enabled = False
        End If

        If SiyouGakunen < 2 Then
            Chk学年通常1_2.Enabled = False
            Chk学年通常2_2.Enabled = False
            Chk学年通常3_2.Enabled = False
            Chk学年通常4_2.Enabled = False
            Chk学年通常5_2.Enabled = False

            Chk随時1_2学年.Enabled = False
            Chk随時2_2学年.Enabled = False
            Chk随時3_2学年.Enabled = False
            Chk随時4_2学年.Enabled = False
            Chk随時5_2学年.Enabled = False
            Chk随時6_2学年.Enabled = False
        End If

    End Sub
#End Region
#Region "構造体制御"
    '
    '　関数名　-　Sb_GetData
    '
    '　機能    -   スケジュールマスタ情報を構造体にセット
    '
    '　引数    -  Get_TuujyouData,Get_ZuijiData
    '
    '　備考    -  
    '
    Private Sub Sb_GetData(ByRef Get_TuujyouData() As TuujyouData, ByRef Get_ZuijiData() As ZuijiData)

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '通常振替日情報を構造体にセット
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        'スケジュールタブ画面で現在表示されている項目の内容を構造体に取得

        Get_TuujyouData(1).Seikyu_Nen = txt対象年度.Text.Trim
        Get_TuujyouData(1).Seikyu_Tuki = txt対象月.Text.Trim
        '振替日
        Get_TuujyouData(1).Furikae_Check = Chk有効振替日通常1.Checked
        Get_TuujyouData(1).Furikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(1).Furikae_Date = txt通常振替日1.Text.Trim
        '休日補正後の振替日
        Get_TuujyouData(1).Furikae_Day = Replace(lbl通常振替日1.Text.Trim, "/", "")

        '再振日
        Get_TuujyouData(1).SaiFurikae_Check = Chk有効再振日通常1.Checked
        Get_TuujyouData(1).SaiFurikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(1).SaiFurikae_Date = txt通常再振日1.Text
        '休日補正後の再振日
        Get_TuujyouData(1).SaiFurikae_Day = Replace(lbl通常再振日1.Text.Trim, "/", "")

        Select Case Chk学年通常1_全.Checked
            Case True
                Get_TuujyouData(1).SiyouGakunenALL_Check = True
                Get_TuujyouData(1).SiyouGakunen1_Check = True
                Get_TuujyouData(1).SiyouGakunen2_Check = True
                Get_TuujyouData(1).SiyouGakunen3_Check = True
                Get_TuujyouData(1).SiyouGakunen4_Check = True
                Get_TuujyouData(1).SiyouGakunen5_Check = True
                Get_TuujyouData(1).SiyouGakunen6_Check = True
                Get_TuujyouData(1).SiyouGakunen7_Check = True
                Get_TuujyouData(1).SiyouGakunen8_Check = True
                Get_TuujyouData(1).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(1).SiyouGakunenALL_Check = False
                Get_TuujyouData(1).SiyouGakunen1_Check = Chk学年通常1_1.Checked
                Get_TuujyouData(1).SiyouGakunen2_Check = Chk学年通常1_2.Checked
                Get_TuujyouData(1).SiyouGakunen3_Check = Chk学年通常1_3.Checked
                Get_TuujyouData(1).SiyouGakunen4_Check = Chk学年通常1_4.Checked
                Get_TuujyouData(1).SiyouGakunen5_Check = Chk学年通常1_5.Checked
                Get_TuujyouData(1).SiyouGakunen6_Check = Chk学年通常1_6.Checked
                Get_TuujyouData(1).SiyouGakunen7_Check = Chk学年通常1_7.Checked
                Get_TuujyouData(1).SiyouGakunen8_Check = Chk学年通常1_8.Checked
                Get_TuujyouData(1).SiyouGakunen9_Check = Chk学年通常1_9.Checked
        End Select

        Get_TuujyouData(2).Seikyu_Nen = txt対象年度.Text.Trim
        Get_TuujyouData(2).Seikyu_Tuki = txt対象月.Text.Trim
        '振替日
        Get_TuujyouData(2).Furikae_Check = Chk有効振替日通常2.Checked
        Get_TuujyouData(2).Furikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(2).Furikae_Date = txt通常振替日2.Text.Trim
        '休日補正後の振替日
        Get_TuujyouData(2).Furikae_Day = Replace(lbl通常振替日2.Text.Trim, "/", "")

        '再振日
        Get_TuujyouData(2).SaiFurikae_Check = Chk有効再振日通常2.Checked
        Get_TuujyouData(2).SaiFurikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(2).SaiFurikae_Date = txt通常再振日2.Text
        '休日補正後の再振日
        Get_TuujyouData(2).SaiFurikae_Day = Replace(lbl通常再振日2.Text.Trim, "/", "")


        Select Case Chk学年通常2_全.Checked
            Case True
                Get_TuujyouData(2).SiyouGakunenALL_Check = True
                Get_TuujyouData(2).SiyouGakunen1_Check = True
                Get_TuujyouData(2).SiyouGakunen2_Check = True
                Get_TuujyouData(2).SiyouGakunen3_Check = True
                Get_TuujyouData(2).SiyouGakunen4_Check = True
                Get_TuujyouData(2).SiyouGakunen5_Check = True
                Get_TuujyouData(2).SiyouGakunen6_Check = True
                Get_TuujyouData(2).SiyouGakunen7_Check = True
                Get_TuujyouData(2).SiyouGakunen8_Check = True
                Get_TuujyouData(2).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(2).SiyouGakunenALL_Check = False
                Get_TuujyouData(2).SiyouGakunen1_Check = Chk学年通常2_1.Checked
                Get_TuujyouData(2).SiyouGakunen2_Check = Chk学年通常2_2.Checked
                Get_TuujyouData(2).SiyouGakunen3_Check = Chk学年通常2_3.Checked
                Get_TuujyouData(2).SiyouGakunen4_Check = Chk学年通常2_4.Checked
                Get_TuujyouData(2).SiyouGakunen5_Check = Chk学年通常2_5.Checked
                Get_TuujyouData(2).SiyouGakunen6_Check = Chk学年通常2_6.Checked
                Get_TuujyouData(2).SiyouGakunen7_Check = Chk学年通常2_7.Checked
                Get_TuujyouData(2).SiyouGakunen8_Check = Chk学年通常2_8.Checked
                Get_TuujyouData(2).SiyouGakunen9_Check = Chk学年通常2_9.Checked
        End Select

        Get_TuujyouData(3).Seikyu_Nen = txt対象年度.Text.Trim
        Get_TuujyouData(3).Seikyu_Tuki = txt対象月.Text.Trim
        '振替日
        Get_TuujyouData(3).Furikae_Check = Chk有効振替日通常3.Checked
        Get_TuujyouData(3).Furikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(3).Furikae_Date = txt通常振替日3.Text.Trim
        '休日補正後の振替日
        Get_TuujyouData(3).Furikae_Day = Replace(lbl通常振替日3.Text.Trim, "/", "")

        '再振日
        Get_TuujyouData(3).SaiFurikae_Check = Chk有効再振日通常3.Checked
        Get_TuujyouData(3).SaiFurikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(3).SaiFurikae_Date = txt通常再振日3.Text
        '休日補正後の再振日
        Get_TuujyouData(3).SaiFurikae_Day = Replace(lbl通常再振日3.Text.Trim, "/", "")

        Select Case Chk学年通常3_全.Checked
            Case True
                Get_TuujyouData(3).SiyouGakunenALL_Check = True
                Get_TuujyouData(3).SiyouGakunen1_Check = True
                Get_TuujyouData(3).SiyouGakunen2_Check = True
                Get_TuujyouData(3).SiyouGakunen3_Check = True
                Get_TuujyouData(3).SiyouGakunen4_Check = True
                Get_TuujyouData(3).SiyouGakunen5_Check = True
                Get_TuujyouData(3).SiyouGakunen6_Check = True
                Get_TuujyouData(3).SiyouGakunen7_Check = True
                Get_TuujyouData(3).SiyouGakunen8_Check = True
                Get_TuujyouData(3).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(3).SiyouGakunenALL_Check = False
                Get_TuujyouData(3).SiyouGakunen1_Check = Chk学年通常3_1.Checked
                Get_TuujyouData(3).SiyouGakunen2_Check = Chk学年通常3_2.Checked
                Get_TuujyouData(3).SiyouGakunen3_Check = Chk学年通常3_3.Checked
                Get_TuujyouData(3).SiyouGakunen4_Check = Chk学年通常3_4.Checked
                Get_TuujyouData(3).SiyouGakunen5_Check = Chk学年通常3_5.Checked
                Get_TuujyouData(3).SiyouGakunen6_Check = Chk学年通常3_6.Checked
                Get_TuujyouData(3).SiyouGakunen7_Check = Chk学年通常3_7.Checked
                Get_TuujyouData(3).SiyouGakunen8_Check = Chk学年通常3_8.Checked
                Get_TuujyouData(3).SiyouGakunen9_Check = Chk学年通常3_9.Checked
        End Select

        Get_TuujyouData(4).Seikyu_Nen = txt対象年度.Text.Trim
        Get_TuujyouData(4).Seikyu_Tuki = txt対象月.Text.Trim
        '振替日
        Get_TuujyouData(4).Furikae_Check = Chk有効振替日通常4.Checked
        Get_TuujyouData(4).Furikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(4).Furikae_Date = txt通常振替日4.Text.Trim
        '休日補正後の振替日
        Get_TuujyouData(4).Furikae_Day = Replace(lbl通常振替日4.Text.Trim, "/", "")

        '再振日
        Get_TuujyouData(4).SaiFurikae_Check = Chk有効再振日通常4.Checked
        Get_TuujyouData(4).SaiFurikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(4).SaiFurikae_Date = txt通常再振日4.Text
        '休日補正後の再振日
        Get_TuujyouData(4).SaiFurikae_Day = Replace(lbl通常再振日4.Text.Trim, "/", "")


        Select Case Chk学年通常4_全.Checked
            Case True
                Get_TuujyouData(4).SiyouGakunenALL_Check = True
                Get_TuujyouData(4).SiyouGakunen1_Check = True
                Get_TuujyouData(4).SiyouGakunen2_Check = True
                Get_TuujyouData(4).SiyouGakunen3_Check = True
                Get_TuujyouData(4).SiyouGakunen4_Check = True
                Get_TuujyouData(4).SiyouGakunen5_Check = True
                Get_TuujyouData(4).SiyouGakunen6_Check = True
                Get_TuujyouData(4).SiyouGakunen7_Check = True
                Get_TuujyouData(4).SiyouGakunen8_Check = True
                Get_TuujyouData(4).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(4).SiyouGakunenALL_Check = False
                Get_TuujyouData(4).SiyouGakunen1_Check = Chk学年通常4_1.Checked
                Get_TuujyouData(4).SiyouGakunen2_Check = Chk学年通常4_2.Checked
                Get_TuujyouData(4).SiyouGakunen3_Check = Chk学年通常4_3.Checked
                Get_TuujyouData(4).SiyouGakunen4_Check = Chk学年通常4_4.Checked
                Get_TuujyouData(4).SiyouGakunen5_Check = Chk学年通常4_5.Checked
                Get_TuujyouData(4).SiyouGakunen6_Check = Chk学年通常4_6.Checked
                Get_TuujyouData(4).SiyouGakunen7_Check = Chk学年通常4_7.Checked
                Get_TuujyouData(4).SiyouGakunen8_Check = Chk学年通常4_8.Checked
                Get_TuujyouData(4).SiyouGakunen9_Check = Chk学年通常4_9.Checked
        End Select

        Get_TuujyouData(5).Seikyu_Nen = txt対象年度.Text.Trim
        Get_TuujyouData(5).Seikyu_Tuki = txt対象月.Text.Trim
        '振替日
        Get_TuujyouData(5).Furikae_Check = Chk有効振替日通常5.Checked
        Get_TuujyouData(5).Furikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(5).Furikae_Date = txt通常振替日5.Text.Trim
        '休日補正後の振替日
        Get_TuujyouData(5).Furikae_Day = Replace(lbl通常振替日5.Text.Trim, "/", "")

        '再振日
        Get_TuujyouData(5).SaiFurikae_Check = Chk有効再振日通常5.Checked
        Get_TuujyouData(5).SaiFurikae_Tuki = txt対象月.Text.Trim
        Get_TuujyouData(5).SaiFurikae_Date = txt通常再振日5.Text
        '休日補正後の再振日
        Get_TuujyouData(5).SaiFurikae_Day = Replace(lbl通常再振日5.Text.Trim, "/", "")

        Select Case Chk学年通常5_全.Checked
            Case True
                Get_TuujyouData(5).SiyouGakunenALL_Check = True
                Get_TuujyouData(5).SiyouGakunen1_Check = True
                Get_TuujyouData(5).SiyouGakunen2_Check = True
                Get_TuujyouData(5).SiyouGakunen3_Check = True
                Get_TuujyouData(5).SiyouGakunen4_Check = True
                Get_TuujyouData(5).SiyouGakunen5_Check = True
                Get_TuujyouData(5).SiyouGakunen6_Check = True
                Get_TuujyouData(5).SiyouGakunen7_Check = True
                Get_TuujyouData(5).SiyouGakunen8_Check = True
                Get_TuujyouData(5).SiyouGakunen9_Check = True
            Case False
                Get_TuujyouData(5).SiyouGakunenALL_Check = False
                Get_TuujyouData(5).SiyouGakunen1_Check = Chk学年通常5_1.Checked
                Get_TuujyouData(5).SiyouGakunen2_Check = Chk学年通常5_2.Checked
                Get_TuujyouData(5).SiyouGakunen3_Check = Chk学年通常5_3.Checked
                Get_TuujyouData(5).SiyouGakunen4_Check = Chk学年通常5_4.Checked
                Get_TuujyouData(5).SiyouGakunen5_Check = Chk学年通常5_5.Checked
                Get_TuujyouData(5).SiyouGakunen6_Check = Chk学年通常5_6.Checked
                Get_TuujyouData(5).SiyouGakunen7_Check = Chk学年通常5_7.Checked
                Get_TuujyouData(5).SiyouGakunen8_Check = Chk学年通常5_8.Checked
                Get_TuujyouData(5).SiyouGakunen9_Check = Chk学年通常5_9.Checked
        End Select

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '随時振替日情報を構造体にセット
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '随時スケジュールタブ画面で現在表示されている項目の内容を構造体に取得
        Get_ZuijiData(1).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分1)
        Get_ZuijiData(1).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(1).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(1).Furikae_Date = txt随時振替日1.Text

        Get_ZuijiData(1).Furikae_Day = Replace(lbl随時振替日1.Text.Trim, "/", "")

        Select Case Chk随時1_全学年.Checked
            Case True
                Get_ZuijiData(1).SiyouGakunen1_Check = True
                Get_ZuijiData(1).SiyouGakunen2_Check = True
                Get_ZuijiData(1).SiyouGakunen3_Check = True
                Get_ZuijiData(1).SiyouGakunen4_Check = True
                Get_ZuijiData(1).SiyouGakunen5_Check = True
                Get_ZuijiData(1).SiyouGakunen6_Check = True
                Get_ZuijiData(1).SiyouGakunen7_Check = True
                Get_ZuijiData(1).SiyouGakunen8_Check = True
                Get_ZuijiData(1).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(1).SiyouGakunen1_Check = Chk随時1_1学年.Checked
                Get_ZuijiData(1).SiyouGakunen2_Check = Chk随時1_2学年.Checked
                Get_ZuijiData(1).SiyouGakunen3_Check = Chk随時1_3学年.Checked
                Get_ZuijiData(1).SiyouGakunen4_Check = Chk随時1_4学年.Checked
                Get_ZuijiData(1).SiyouGakunen5_Check = Chk随時1_5学年.Checked
                Get_ZuijiData(1).SiyouGakunen6_Check = Chk随時1_6学年.Checked
                Get_ZuijiData(1).SiyouGakunen7_Check = Chk随時1_7学年.Checked
                Get_ZuijiData(1).SiyouGakunen8_Check = Chk随時1_8学年.Checked
                Get_ZuijiData(1).SiyouGakunen9_Check = Chk随時1_9学年.Checked
        End Select

        Get_ZuijiData(2).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分2)
        Get_ZuijiData(2).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(2).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(2).Furikae_Date = txt随時振替日2.Text

        Get_ZuijiData(2).Furikae_Day = Replace(lbl随時振替日2.Text.Trim, "/", "")

        Select Case Chk随時2_全学年.Checked
            Case True
                Get_ZuijiData(2).SiyouGakunen1_Check = True
                Get_ZuijiData(2).SiyouGakunen2_Check = True
                Get_ZuijiData(2).SiyouGakunen3_Check = True
                Get_ZuijiData(2).SiyouGakunen4_Check = True
                Get_ZuijiData(2).SiyouGakunen5_Check = True
                Get_ZuijiData(2).SiyouGakunen6_Check = True
                Get_ZuijiData(2).SiyouGakunen7_Check = True
                Get_ZuijiData(2).SiyouGakunen8_Check = True
                Get_ZuijiData(2).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(2).SiyouGakunen1_Check = Chk随時2_1学年.Checked
                Get_ZuijiData(2).SiyouGakunen2_Check = Chk随時2_2学年.Checked
                Get_ZuijiData(2).SiyouGakunen3_Check = Chk随時2_3学年.Checked
                Get_ZuijiData(2).SiyouGakunen4_Check = Chk随時2_4学年.Checked
                Get_ZuijiData(2).SiyouGakunen5_Check = Chk随時2_5学年.Checked
                Get_ZuijiData(2).SiyouGakunen6_Check = Chk随時2_6学年.Checked
                Get_ZuijiData(2).SiyouGakunen7_Check = Chk随時2_7学年.Checked
                Get_ZuijiData(2).SiyouGakunen8_Check = Chk随時2_8学年.Checked
                Get_ZuijiData(2).SiyouGakunen9_Check = Chk随時2_9学年.Checked
        End Select

        Get_ZuijiData(3).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分3)
        Get_ZuijiData(3).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(3).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(3).Furikae_Date = txt随時振替日3.Text

        Get_ZuijiData(3).Furikae_Day = Replace(lbl随時振替日3.Text.Trim, "/", "")

        Select Case Chk随時3_全学年.Checked
            Case True
                Get_ZuijiData(3).SiyouGakunen1_Check = True
                Get_ZuijiData(3).SiyouGakunen2_Check = True
                Get_ZuijiData(3).SiyouGakunen3_Check = True
                Get_ZuijiData(3).SiyouGakunen4_Check = True
                Get_ZuijiData(3).SiyouGakunen5_Check = True
                Get_ZuijiData(3).SiyouGakunen6_Check = True
                Get_ZuijiData(3).SiyouGakunen7_Check = True
                Get_ZuijiData(3).SiyouGakunen8_Check = True
                Get_ZuijiData(3).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(3).SiyouGakunen1_Check = Chk随時3_1学年.Checked
                Get_ZuijiData(3).SiyouGakunen2_Check = Chk随時3_2学年.Checked
                Get_ZuijiData(3).SiyouGakunen3_Check = Chk随時3_3学年.Checked
                Get_ZuijiData(3).SiyouGakunen4_Check = Chk随時3_4学年.Checked
                Get_ZuijiData(3).SiyouGakunen5_Check = Chk随時3_5学年.Checked
                Get_ZuijiData(3).SiyouGakunen6_Check = Chk随時3_6学年.Checked
                Get_ZuijiData(3).SiyouGakunen7_Check = Chk随時3_7学年.Checked
                Get_ZuijiData(3).SiyouGakunen8_Check = Chk随時3_8学年.Checked
                Get_ZuijiData(3).SiyouGakunen9_Check = Chk随時3_9学年.Checked
        End Select

        Get_ZuijiData(4).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分4)
        Get_ZuijiData(4).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(4).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(4).Furikae_Date = txt随時振替日4.Text

        Get_ZuijiData(4).Furikae_Day = Replace(lbl随時振替日4.Text.Trim, "/", "")

        Select Case Chk随時4_全学年.Checked
            Case True
                Get_ZuijiData(4).SiyouGakunen1_Check = True
                Get_ZuijiData(4).SiyouGakunen2_Check = True
                Get_ZuijiData(4).SiyouGakunen3_Check = True
                Get_ZuijiData(4).SiyouGakunen4_Check = True
                Get_ZuijiData(4).SiyouGakunen5_Check = True
                Get_ZuijiData(4).SiyouGakunen6_Check = True
                Get_ZuijiData(4).SiyouGakunen7_Check = True
                Get_ZuijiData(4).SiyouGakunen8_Check = True
                Get_ZuijiData(4).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(4).SiyouGakunen1_Check = Chk随時4_1学年.Checked
                Get_ZuijiData(4).SiyouGakunen2_Check = Chk随時4_2学年.Checked
                Get_ZuijiData(4).SiyouGakunen3_Check = Chk随時4_3学年.Checked
                Get_ZuijiData(4).SiyouGakunen4_Check = Chk随時4_4学年.Checked
                Get_ZuijiData(4).SiyouGakunen5_Check = Chk随時4_5学年.Checked
                Get_ZuijiData(4).SiyouGakunen6_Check = Chk随時4_6学年.Checked
                Get_ZuijiData(4).SiyouGakunen7_Check = Chk随時4_7学年.Checked
                Get_ZuijiData(4).SiyouGakunen8_Check = Chk随時4_8学年.Checked
                Get_ZuijiData(4).SiyouGakunen9_Check = Chk随時4_9学年.Checked
        End Select

        Get_ZuijiData(5).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分5)
        Get_ZuijiData(5).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(5).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(5).Furikae_Date = txt随時振替日5.Text

        Get_ZuijiData(5).Furikae_Day = Replace(lbl随時振替日5.Text.Trim, "/", "")

        Select Case Chk随時5_全学年.Checked
            Case True
                Get_ZuijiData(5).SiyouGakunen1_Check = True
                Get_ZuijiData(5).SiyouGakunen2_Check = True
                Get_ZuijiData(5).SiyouGakunen3_Check = True
                Get_ZuijiData(5).SiyouGakunen4_Check = True
                Get_ZuijiData(5).SiyouGakunen5_Check = True
                Get_ZuijiData(5).SiyouGakunen6_Check = True
                Get_ZuijiData(5).SiyouGakunen7_Check = True
                Get_ZuijiData(5).SiyouGakunen8_Check = True
                Get_ZuijiData(5).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(5).SiyouGakunen1_Check = Chk随時5_1学年.Checked
                Get_ZuijiData(5).SiyouGakunen2_Check = Chk随時5_2学年.Checked
                Get_ZuijiData(5).SiyouGakunen3_Check = Chk随時5_3学年.Checked
                Get_ZuijiData(5).SiyouGakunen4_Check = Chk随時5_4学年.Checked
                Get_ZuijiData(5).SiyouGakunen5_Check = Chk随時5_5学年.Checked
                Get_ZuijiData(5).SiyouGakunen6_Check = Chk随時5_6学年.Checked
                Get_ZuijiData(5).SiyouGakunen7_Check = Chk随時5_7学年.Checked
                Get_ZuijiData(5).SiyouGakunen8_Check = Chk随時5_8学年.Checked
                Get_ZuijiData(5).SiyouGakunen9_Check = Chk随時5_9学年.Checked
        End Select

        Get_ZuijiData(6).Nyusyutu_Kbn = GFUNC_NAME_TO_CODE(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分6)
        Get_ZuijiData(6).Furikae_Nen = txt対象年度.Text.Trim
        Get_ZuijiData(6).Furikae_Tuki = txt対象月.Text
        Get_ZuijiData(6).Furikae_Date = txt随時振替日6.Text

        Get_ZuijiData(6).Furikae_Day = Replace(lbl随時振替日6.Text.Trim, "/", "")

        Select Case Chk随時6_全学年.Checked
            Case True
                Get_ZuijiData(6).SiyouGakunen1_Check = True
                Get_ZuijiData(6).SiyouGakunen2_Check = True
                Get_ZuijiData(6).SiyouGakunen3_Check = True
                Get_ZuijiData(6).SiyouGakunen4_Check = True
                Get_ZuijiData(6).SiyouGakunen5_Check = True
                Get_ZuijiData(6).SiyouGakunen6_Check = True
                Get_ZuijiData(6).SiyouGakunen7_Check = True
                Get_ZuijiData(6).SiyouGakunen8_Check = True
                Get_ZuijiData(6).SiyouGakunen9_Check = True
            Case False
                Get_ZuijiData(6).SiyouGakunen1_Check = Chk随時6_1学年.Checked
                Get_ZuijiData(6).SiyouGakunen2_Check = Chk随時6_2学年.Checked
                Get_ZuijiData(6).SiyouGakunen3_Check = Chk随時6_3学年.Checked
                Get_ZuijiData(6).SiyouGakunen4_Check = Chk随時6_4学年.Checked
                Get_ZuijiData(6).SiyouGakunen5_Check = Chk随時6_5学年.Checked
                Get_ZuijiData(6).SiyouGakunen6_Check = Chk随時6_6学年.Checked
                Get_ZuijiData(6).SiyouGakunen7_Check = Chk随時6_7学年.Checked
                Get_ZuijiData(6).SiyouGakunen8_Check = Chk随時6_8学年.Checked
                Get_ZuijiData(6).SiyouGakunen9_Check = Chk随時6_9学年.Checked
        End Select

    End Sub
    '
    '　関数名　-　fn_Check_TuujyouFuriDate
    '
    '　機能    -  通常振替日タブ情報のチェック
    '
    '　引数    -  aTuujyouData
    '
    '　備考    -  
    '　
    Private Function fn_Check_TuujyouFuriDate(ByVal aInfoGakkou As GAKDATA, _
                                              ByRef aTuujyouData() As TuujyouData, _
                                              Optional ByVal SetDataflg As Boolean = True) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_Check_TuujyouFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '通常振替日タブチェック
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            Dim FuriExistFlg As Boolean = False
            Dim SFuriExistFlg As Boolean = False

            For i As Integer = 1 To 5 Step 1
                With aTuujyouData(i)
                    '■通常振替日入力チェック
                    If .Furikae_Check = True Then
                        '未入力チェック
                        If .Furikae_Date.Trim = "" Then

                            ERRMSG = "未入力の振替日が存在します。"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        Else
                            '日付判定
                            If IsDate(txt対象年度.Text & "/" & txt対象月.Text & "/" & .Furikae_Date) = False Then

                                ERRMSG = "不正な入力振替日が存在します。"
                                Me.txt通常振替日1.Focus()
                                Exit Try
                            Else
                                '学年フラグ未入力チェック(入力あり時は対象学年のいずれかにフラグがあればOK)
                                If .SiyouGakunen1_Check = False _
                                AndAlso .SiyouGakunen2_Check = False _
                                AndAlso .SiyouGakunen3_Check = False _
                                AndAlso .SiyouGakunen4_Check = False _
                                AndAlso .SiyouGakunen5_Check = False _
                                AndAlso .SiyouGakunen6_Check = False _
                                AndAlso .SiyouGakunen7_Check = False _
                                AndAlso .SiyouGakunen8_Check = False _
                                AndAlso .SiyouGakunen9_Check = False _
                                AndAlso .SiyouGakunenALL_Check = False Then

                                    ERRMSG = "対象学年が設定されていません。"
                                    Me.txt通常振替日1.Focus()
                                    Exit Try
                                End If
                            End If
                        End If

                        FuriExistFlg = True
                    Else
                        If .Furikae_Date.Trim <> "" Then

                            ERRMSG = "振替日を有効にする場合は、振替日を入力してください。"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        End If
                    End If
                    '■通常再振日入力チェック
                    If .SaiFurikae_Check = True Then
                        '未入力チェック
                        If .SaiFurikae_Date.Trim = "" Then

                            ERRMSG = "未入力の再振日が存在します。"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        Else
                            '日付判定
                            If IsDate(txt対象年度.Text & "/" & txt対象月.Text & "/" & .SaiFurikae_Date) = False Then

                                ERRMSG = "不正な入力再振日が存在します。"
                                Me.txt通常振替日1.Focus()
                                Exit Try
                            End If
                        End If

                        SFuriExistFlg = True
                    Else
                        If .SaiFurikae_Date.Trim <> "" Then

                            ERRMSG = "再振日を有効にする場合は、振替日を入力してください。"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        End If
                    End If
                End With
            Next

            '■入力値の存在チェック
            Select Case True
                Case (FuriExistFlg = False And SFuriExistFlg = False)
                    '初期値セットフラグあり時はマスタ情報を設定(Insert用)
                    If SetDataflg = True Then
                        '基準日でスケジュール作成
                        With aTuujyouData(1)
                            .TaisyouFlg = True
                            .Furikae_Check = True
                            .Furikae_Date = aInfoGakkou.FURI_DATE
                            If aInfoGakkou.SFURI_SYUBETU = 1 Then
                                .SaiFurikae_Check = True
                                .SaiFurikae_Date = aInfoGakkou.SFURI_DATE
                            End If
                            .SiyouGakunen1_Check = False
                            .SiyouGakunen2_Check = False
                            .SiyouGakunen3_Check = False
                            .SiyouGakunen4_Check = False
                            .SiyouGakunen5_Check = False
                            .SiyouGakunen6_Check = False
                            .SiyouGakunen7_Check = False
                            .SiyouGakunen8_Check = False
                            .SiyouGakunen9_Check = False
                            .SiyouGakunenALL_Check = True
                        End With
                    End If

                Case (FuriExistFlg = False And SFuriExistFlg = True)
                    '再振日のみの入力はエラー
                    ERRMSG = "再振日のみの入力はできません。"
                    Me.txt通常振替日1.Focus()
                    Exit Try

                Case (FuriExistFlg = True And SFuriExistFlg = True)
                    '入力ありの場合はチェック

                Case (FuriExistFlg = True And SFuriExistFlg = False)
                    '入力ありの場合はチェック

            End Select

            '■振替日と再振日の同一日チェック
            For i As Integer = 1 To 5 Step 1
                With aTuujyouData(i)
                    If .Furikae_Check = True AndAlso .SaiFurikae_Check = True AndAlso .Furikae_Date <> "" AndAlso .SaiFurikae_Date <> "" Then
                        If .Furikae_Date = .SaiFurikae_Date Then

                            ERRMSG = "振替日内で振替日と再振替日が同じものがあります"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        End If
                    End If
                End With
            Next

            '■振替日内重複チェック
            For num1 As Integer = 1 To 5 Step 1
                For num2 As Integer = 1 To 5 Step 1
                    If aTuujyouData(num1).Furikae_Date <> "" OrElse aTuujyouData(num2).Furikae_Date <> "" Then
                        If num1 <> num2 AndAlso _
                        aTuujyouData(num1).Furikae_Date = aTuujyouData(num2).Furikae_Date AndAlso _
                        aTuujyouData(num1).Furikae_Check = True AndAlso aTuujyouData(num2).Furikae_Check = True Then

                            ERRMSG = "同一振替日は設定できません。"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        End If
                    End If
                Next
            Next

            '営業日の補正
            Dim HoseiEigyoubi1(1) As String
            Dim HoseiEigyoubi2(1) As String
            Dim HoseiEigyoubi3(1) As String
            Dim HoseiEigyoubi4(1) As String
            Dim HoseiEigyoubi5(1) As String
            Dim HoseiEigyoubi6(1) As String
            Dim HoseiEigyoubi7(1) As String
            Dim HoseiEigyoubi8(1) As String
            Dim HoseiEigyoubi9(1) As String
            '1
            If aTuujyouData(1).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi1) Then
                If HoseiEigyoubi1(0) = "err" OrElse HoseiEigyoubi1(1) = "err" Then
                    ERRMSG = "振替日の休日補正に失敗しました。"
                    Exit Try
                End If
            End If
            '2
            If aTuujyouData(2).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi2) Then
                If HoseiEigyoubi2(0) = "err" OrElse HoseiEigyoubi2(1) = "err" Then
                    ERRMSG = "振替日の休日補正に失敗しました。"
                    Exit Try
                End If
            End If
            '3
            If aTuujyouData(3).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi3) Then
                If HoseiEigyoubi3(0) = "err" OrElse HoseiEigyoubi3(1) = "err" Then
                    ERRMSG = "振替日の休日補正に失敗しました。"
                    Exit Try
                End If
            End If
            '4
            If aTuujyouData(4).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi4) Then
                If HoseiEigyoubi4(0) = "err" OrElse HoseiEigyoubi4(1) = "err" Then
                    ERRMSG = "振替日の休日補正に失敗しました。"
                    Exit Try
                End If
            End If
            '5
            If aTuujyouData(5).fn_EigyoubiHosei(aInfoGakkou, HoseiEigyoubi5) Then
                If HoseiEigyoubi5(0) = "err" OrElse HoseiEigyoubi5(1) = "err" Then
                    ERRMSG = "振替日の休日補正に失敗しました。"
                    Exit Try
                End If
            End If

            Dim SyofuriDate As String() = {HoseiEigyoubi1(0), _
                                           HoseiEigyoubi2(0), _
                                           HoseiEigyoubi3(0), _
                                           HoseiEigyoubi4(0), _
                                           HoseiEigyoubi5(0), _
                                           HoseiEigyoubi6(0), _
                                           HoseiEigyoubi7(0), _
                                           HoseiEigyoubi8(0), _
                                           HoseiEigyoubi9(0)}

            Dim SaifuriDate As String() = {HoseiEigyoubi1(1), _
                                           HoseiEigyoubi2(1), _
                                           HoseiEigyoubi3(1), _
                                           HoseiEigyoubi4(1), _
                                           HoseiEigyoubi5(1), _
                                           HoseiEigyoubi6(1), _
                                           HoseiEigyoubi7(1), _
                                           HoseiEigyoubi8(1), _
                                           HoseiEigyoubi9(1)}

            For cnt As Integer = 0 To 8 Step 1
                If SyofuriDate(cnt) <> "" AndAlso SyofuriDate(cnt) = SaifuriDate(cnt) Then
                    ERRMSG = "休日補正後の振替日と再振日が一致しています"
                    Exit Try
                End If
            Next

            For cnt1 As Integer = 0 To 8 Step 1
                For cnt2 As Integer = 0 To 8 Step 1
                    If cnt1 <> cnt2 Then
                        If SyofuriDate(cnt1) <> "" AndAlso SyofuriDate(cnt1) = SyofuriDate(cnt2) Then
                            ERRMSG = "休日補正後の振替日が重複しています"
                            Exit Try
                        End If
                    End If
                Next
            Next

            For cnt1 As Integer = 0 To 8 Step 1
                For cnt2 As Integer = 0 To 8 Step 1
                    If cnt1 <> cnt2 Then
                        If SaifuriDate(cnt1) <> "" AndAlso SaifuriDate(cnt1) = SaifuriDate(cnt2) Then
                            ERRMSG = "休日補正後の再振日が重複しています"
                            Exit Try
                        End If
                    End If
                Next
            Next



            '■全チェック完了後に、日付入力があるものに、処理対象フラグを立てる
            Dim EntryExistflg As Boolean
            For i As Integer = 1 To 5 Step 1
                If aTuujyouData(i).Furikae_Date.Trim <> "" Then
                    aTuujyouData(i).TaisyouFlg = True
                    EntryExistflg = True
                Else
                    aTuujyouData(i).TaisyouFlg = False
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "入力振替日のチェックに失敗しました。"
            Me.txt通常振替日1.Focus()
        End Try

        Return ret

    End Function
    '
    '　関数名　-　fn_Check_ZuijiFuriDate
    '
    '　機能    -  随時振替日タブ情報のチェック
    '
    '　引数    -  aTuujyouData
    '
    '　備考    -  
    '
    Private Function fn_Check_ZuijiFuriDate(ByVal aInfoGakkou As GAKDATA, ByRef aZuijiData() As ZuijiData) As Boolean

        Dim ret As Boolean = False

        Try
            '***ログ情報***
            STR_COMMAND = "fn_Check_ZuijiFuriDate"
            STR_LOG_GAKKOU_CODE = aInfoGakkou.GAKKOU_CODE
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            '随時振替日タブチェック
            '@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
            For i As Integer = 1 To 6 Step 1
                With aZuijiData(i)
                    '■随時振替日入力チェック

                    '未入力チェック
                    If .Furikae_Date.Trim <> "" Then
                        '日付判定
                        If IsDate(txt対象年度.Text & "/" & txt対象月.Text & "/" & .Furikae_Date) = False Then

                            ERRMSG = "随時処理に不正な入力振替日が存在します。"
                            Me.txt随時振替日1.Focus()
                            Exit Try
                        Else
                            '学年フラグ未入力チェック(入力あり時は対象学年のいずれかにフラグがあればOK)
                            If .SiyouGakunen1_Check = False _
                            AndAlso .SiyouGakunen2_Check = False _
                            AndAlso .SiyouGakunen3_Check = False _
                            AndAlso .SiyouGakunen4_Check = False _
                            AndAlso .SiyouGakunen5_Check = False _
                            AndAlso .SiyouGakunen6_Check = False _
                            AndAlso .SiyouGakunen7_Check = False _
                            AndAlso .SiyouGakunen8_Check = False _
                            AndAlso .SiyouGakunen9_Check = False _
                            AndAlso .SiyouGakunenALL_Check = False Then

                                ERRMSG = "随時処理の対象学年が設定されていません。"
                                Me.txt随時振替日1.Focus()
                                Exit Try
                            End If
                        End If
                    End If
                End With
            Next

            '■振替日内重複チェック
            For num1 As Integer = 1 To 6 Step 1
                For num2 As Integer = 1 To 6 Step 1
                    If aZuijiData(num1).Furikae_Date <> "" OrElse aZuijiData(num2).Furikae_Date <> "" Then
                        If num1 <> num2 AndAlso _
                aZuijiData(num1).Furikae_Date = aZuijiData(num2).Furikae_Date Then

                            ERRMSG = "同一振替日は設定できません。"
                            Me.txt随時振替日1.Focus()
                            Exit Try
                        End If
                    End If
                Next
            Next

            Dim StrZuiji(6) As String

            '■初振の営業日を取得
            For i As Integer = 1 To 6 Step 1
                If aZuijiData(i).Furikae_Date.Trim <> "" Then
                    StrZuiji(i) = aZuijiData(i).fn_GetEigyoubiZuiji(aInfoGakkou)
                    If StrZuiji(i) = "err" Then

                        ERRMSG = "営業日の取得に失敗しました。"
                        Me.txt随時振替日1.Focus()
                        Exit Try
                    End If
                End If
            Next

            '■設定振替日休日補正後の重複チェック
            For i As Integer = 1 To 6 Step 1
                If StrZuiji(i) <> "" Then '未入力の場合、チェックの必要なし
                    For j As Integer = i + 1 To 6
                        If StrZuiji(i) = StrZuiji(j) Then

                            ERRMSG = "入力振替日に同一振替日のデータが存在します"
                            Me.txt通常振替日1.Focus()
                            Exit Try
                        End If
                    Next
                End If
            Next

            '■全チェック完了後に、日付入力のあったものに、処理対象フラグを立てる
            For i As Integer = 1 To 6 Step 1
                If aZuijiData(i).Furikae_Date.Trim <> "" Then
                    aZuijiData(i).TaisyouFlg = True
                Else
                    aZuijiData(i).TaisyouFlg = False
                End If
            Next

            ret = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "入力振替日のチェックに失敗しました。"
            Me.txt通常振替日1.Focus()
        End Try

        Return ret

    End Function
#End Region
#Region "FormInitializa"
    '
    '　関数名　-　FormInitializa
    '
    '　機能    -  スケジュールマスタ存在チェック 
    '
    '　引数    -  strSCHKBN、strFURIKBN、strFURIHI、strSAIFURIHI
    '
    '　備考    -  
    '
    '　
    Private Function FormInitializa(Optional ByVal pIndex As Integer = 0) As Boolean
        Try
            Select Case pIndex
                Case 0 '全項目初期化
                    '構造体の初期化
                    Call Sb_StructDataClear()

                    '学校基本情報初期化
                    Call Sb_Kihon_Clear()

                    'タブ情報の初期化
                    Call Sb_DayClear()
                    Call Sb_ChkGakunenClear()
                    Call Sb_ZuijiCmbClear()

                Case 1 '学校情基本情報のみ初期化
                    '構造体の初期化
                    Call Sb_StructDataClear()

                    '学校基本情報初期化
                    Call Sb_Kihon_Clear()

                Case 2 'タブ情報のみ初期化
                    'タブ情報の初期化
                    Call Sb_DayClear()
                    Call Sb_ChkGakunenClear()
                    Call Sb_ZuijiCmbClear()

                Case 9 '構造体のみ初期化
                    '構造体の初期化
                    Call Sb_StructDataClear()
                Case Else
                    Return False
            End Select

            Return True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            Return False
        End Try
    End Function

    '
    '　関数名　-　Sb_Kihon_Crear
    '
    '　機能    -  スケジュールマスタ存在チェック 
    '
    '　引数    -  strSCHKBN、strFURIKBN、strFURIHI、strSAIFURIHI
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_Kihon_Clear()

        txt対象年度.Enabled = True
        txt対象月.Enabled = True

        txtGakkou_Code.Enabled = True
        txtGakkou_Code.Text = ""
        lab学校名.Text = ""
        '休日リストボックス初期化
        lst休日.Items.Clear()
        '学校検索（カナ）
        cmbKana.SelectedIndex = -1

        lbl使用学年.Text = "0"

        '追加 2007/02/15
        '学校コンボ設定（全学校）
        If GFUNC_DB_COMBO_SET(cmbKana, cmbGakkouName) = False Then
            Call GSUB_LOG(0, "コンボボックス設定(cmbGakkouNAME)")
            MessageBox.Show("学校名コンボボックス設定でエラーが発生しました", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Exit Sub
        End If

        cmbKana.Enabled = True
        cmbGakkouName.Enabled = True

        '学校検索（学校名）
        cmbGakkouName.SelectedIndex = -1

    End Sub

    '
    '　関数名　-　Sb_DayClear
    '
    '　機能    -  入力日付の初期化
    '
    '　引数    - 
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_DayClear()

        '通常振替日
        '1
        Chk有効振替日通常1.Checked = False
        Chk有効振替日通常1.Enabled = True
        txt通常振替日1.Text = ""
        txt通常振替日1.Enabled = True
        lbl通常振替日1.Text = ""
        Chk有効再振日通常1.Checked = False
        Chk有効再振日通常1.Enabled = True
        txt通常再振日1.Text = ""
        txt通常再振日1.Enabled = True
        lbl通常再振日1.Text = ""
        '2
        Chk有効振替日通常2.Checked = False
        Chk有効振替日通常2.Enabled = True
        txt通常振替日2.Text = ""
        txt通常振替日2.Enabled = True
        lbl通常振替日2.Text = ""
        Chk有効再振日通常2.Checked = False
        Chk有効再振日通常2.Enabled = True
        txt通常再振日2.Text = ""
        txt通常再振日2.Enabled = True
        lbl通常再振日2.Text = ""
        '3
        Chk有効振替日通常3.Checked = False
        Chk有効振替日通常3.Enabled = True
        txt通常振替日3.Text = ""
        txt通常振替日3.Enabled = True
        lbl通常振替日3.Text = ""
        Chk有効再振日通常3.Checked = False
        Chk有効再振日通常3.Enabled = True
        txt通常再振日3.Text = ""
        txt通常再振日3.Enabled = True
        lbl通常再振日3.Text = ""
        '4
        Chk有効振替日通常4.Checked = False
        Chk有効振替日通常4.Enabled = True
        txt通常振替日4.Text = ""
        txt通常振替日4.Enabled = True
        lbl通常振替日4.Text = ""
        Chk有効再振日通常4.Checked = False
        Chk有効再振日通常4.Enabled = True
        txt通常再振日4.Text = ""
        txt通常再振日4.Enabled = True
        lbl通常再振日4.Text = ""
        '5
        Chk有効振替日通常5.Checked = False
        Chk有効振替日通常5.Enabled = True
        txt通常振替日5.Text = ""
        txt通常振替日5.Enabled = True
        lbl通常振替日5.Text = ""
        Chk有効再振日通常5.Checked = False
        Chk有効再振日通常5.Enabled = True
        txt通常再振日5.Text = ""
        txt通常再振日5.Enabled = True
        lbl通常再振日5.Text = ""

        '随時振替日
        txt随時振替日1.Text = ""
        lbl随時振替日1.Text = ""
        txt随時振替日2.Text = ""
        lbl随時振替日2.Text = ""
        txt随時振替日3.Text = ""
        lbl随時振替日3.Text = ""
        txt随時振替日4.Text = ""
        lbl随時振替日4.Text = ""
        txt随時振替日5.Text = ""
        lbl随時振替日5.Text = ""
        txt随時振替日6.Text = ""
        lbl随時振替日6.Text = ""

        txt随時振替日1.Enabled = True
        txt随時振替日2.Enabled = True
        txt随時振替日3.Enabled = True
        txt随時振替日4.Enabled = True
        txt随時振替日5.Enabled = True
        txt随時振替日6.Enabled = True



    End Sub

    '
    '　関数名　-　Sb_ChkGakunenClear
    '
    '　機能    -  有効学年チェックボックスの初期化
    '
    '　引数    -  通常、随時共に初期化
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_ChkGakunenClear()

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '通常振替日
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '対象学年チェックBoxの有効化
        Chk学年通常1_1.Enabled = True
        Chk学年通常1_2.Enabled = True
        Chk学年通常1_3.Enabled = True
        Chk学年通常1_4.Enabled = True
        Chk学年通常1_5.Enabled = True
        Chk学年通常1_6.Enabled = True
        Chk学年通常1_7.Enabled = True
        Chk学年通常1_8.Enabled = True
        Chk学年通常1_9.Enabled = True
        Chk学年通常1_全.Enabled = True

        Chk学年通常2_1.Enabled = True
        Chk学年通常2_2.Enabled = True
        Chk学年通常2_3.Enabled = True
        Chk学年通常2_4.Enabled = True
        Chk学年通常2_5.Enabled = True
        Chk学年通常2_6.Enabled = True
        Chk学年通常2_7.Enabled = True
        Chk学年通常2_8.Enabled = True
        Chk学年通常2_9.Enabled = True
        Chk学年通常2_全.Enabled = True

        Chk学年通常3_1.Enabled = True
        Chk学年通常3_2.Enabled = True
        Chk学年通常3_3.Enabled = True
        Chk学年通常3_4.Enabled = True
        Chk学年通常3_5.Enabled = True
        Chk学年通常3_6.Enabled = True
        Chk学年通常3_7.Enabled = True
        Chk学年通常3_8.Enabled = True
        Chk学年通常3_9.Enabled = True
        Chk学年通常3_全.Enabled = True

        Chk学年通常4_1.Enabled = True
        Chk学年通常4_2.Enabled = True
        Chk学年通常4_3.Enabled = True
        Chk学年通常4_4.Enabled = True
        Chk学年通常4_5.Enabled = True
        Chk学年通常4_6.Enabled = True
        Chk学年通常4_7.Enabled = True
        Chk学年通常4_8.Enabled = True
        Chk学年通常4_9.Enabled = True
        Chk学年通常4_全.Enabled = True

        Chk学年通常5_1.Enabled = True
        Chk学年通常5_2.Enabled = True
        Chk学年通常5_3.Enabled = True
        Chk学年通常5_4.Enabled = True
        Chk学年通常5_5.Enabled = True
        Chk学年通常5_6.Enabled = True
        Chk学年通常5_7.Enabled = True
        Chk学年通常5_8.Enabled = True
        Chk学年通常5_9.Enabled = True
        Chk学年通常5_全.Enabled = True

        '対象学年有効チェック
        Chk学年通常1_1.Checked = False
        Chk学年通常1_2.Checked = False
        Chk学年通常1_3.Checked = False
        Chk学年通常1_4.Checked = False
        Chk学年通常1_5.Checked = False
        Chk学年通常1_6.Checked = False
        Chk学年通常1_7.Checked = False
        Chk学年通常1_8.Checked = False
        Chk学年通常1_9.Checked = False
        Chk学年通常1_全.Checked = False

        Chk学年通常2_1.Checked = False
        Chk学年通常2_2.Checked = False
        Chk学年通常2_3.Checked = False
        Chk学年通常2_4.Checked = False
        Chk学年通常2_5.Checked = False
        Chk学年通常2_6.Checked = False
        Chk学年通常2_7.Checked = False
        Chk学年通常2_8.Checked = False
        Chk学年通常2_9.Checked = False
        Chk学年通常2_全.Checked = False

        Chk学年通常3_1.Checked = False
        Chk学年通常3_2.Checked = False
        Chk学年通常3_3.Checked = False
        Chk学年通常3_4.Checked = False
        Chk学年通常3_5.Checked = False
        Chk学年通常3_6.Checked = False
        Chk学年通常3_7.Checked = False
        Chk学年通常3_8.Checked = False
        Chk学年通常3_9.Checked = False
        Chk学年通常3_全.Checked = False

        Chk学年通常4_1.Checked = False
        Chk学年通常4_2.Checked = False
        Chk学年通常4_3.Checked = False
        Chk学年通常4_4.Checked = False
        Chk学年通常4_5.Checked = False
        Chk学年通常4_6.Checked = False
        Chk学年通常4_7.Checked = False
        Chk学年通常4_8.Checked = False
        Chk学年通常4_9.Checked = False
        Chk学年通常4_全.Checked = False

        Chk学年通常5_1.Checked = False
        Chk学年通常5_2.Checked = False
        Chk学年通常5_3.Checked = False
        Chk学年通常5_4.Checked = False
        Chk学年通常5_5.Checked = False
        Chk学年通常5_6.Checked = False
        Chk学年通常5_7.Checked = False
        Chk学年通常5_8.Checked = False
        Chk学年通常5_9.Checked = False
        Chk学年通常5_全.Checked = False

        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '随時振替日
        '@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        '対象学年チェックBoxの有効化
        Chk随時1_1学年.Enabled = True
        Chk随時1_2学年.Enabled = True
        Chk随時1_3学年.Enabled = True
        Chk随時1_4学年.Enabled = True
        Chk随時1_5学年.Enabled = True
        Chk随時1_6学年.Enabled = True
        Chk随時1_7学年.Enabled = True
        Chk随時1_8学年.Enabled = True
        Chk随時1_9学年.Enabled = True
        Chk随時1_全学年.Enabled = True

        Chk随時2_1学年.Enabled = True
        Chk随時2_2学年.Enabled = True
        Chk随時2_3学年.Enabled = True
        Chk随時2_4学年.Enabled = True
        Chk随時2_5学年.Enabled = True
        Chk随時2_6学年.Enabled = True
        Chk随時2_7学年.Enabled = True
        Chk随時2_8学年.Enabled = True
        Chk随時2_9学年.Enabled = True
        Chk随時2_全学年.Enabled = True

        Chk随時3_1学年.Enabled = True
        Chk随時3_2学年.Enabled = True
        Chk随時3_3学年.Enabled = True
        Chk随時3_4学年.Enabled = True
        Chk随時3_5学年.Enabled = True
        Chk随時3_6学年.Enabled = True
        Chk随時3_7学年.Enabled = True
        Chk随時3_8学年.Enabled = True
        Chk随時3_9学年.Enabled = True
        Chk随時3_全学年.Enabled = True

        Chk随時4_1学年.Enabled = True
        Chk随時4_2学年.Enabled = True
        Chk随時4_3学年.Enabled = True
        Chk随時4_4学年.Enabled = True
        Chk随時4_5学年.Enabled = True
        Chk随時4_6学年.Enabled = True
        Chk随時4_7学年.Enabled = True
        Chk随時4_8学年.Enabled = True
        Chk随時4_9学年.Enabled = True
        Chk随時4_全学年.Enabled = True

        Chk随時5_1学年.Enabled = True
        Chk随時5_2学年.Enabled = True
        Chk随時5_3学年.Enabled = True
        Chk随時5_4学年.Enabled = True
        Chk随時5_5学年.Enabled = True
        Chk随時5_6学年.Enabled = True
        Chk随時5_7学年.Enabled = True
        Chk随時5_8学年.Enabled = True
        Chk随時5_9学年.Enabled = True
        Chk随時5_全学年.Enabled = True

        Chk随時6_1学年.Enabled = True
        Chk随時6_2学年.Enabled = True
        Chk随時6_3学年.Enabled = True
        Chk随時6_4学年.Enabled = True
        Chk随時6_5学年.Enabled = True
        Chk随時6_6学年.Enabled = True
        Chk随時6_7学年.Enabled = True
        Chk随時6_8学年.Enabled = True
        Chk随時6_9学年.Enabled = True
        Chk随時6_全学年.Enabled = True

        '対象学年有効チェックOFF
        Chk随時1_1学年.Checked = False
        Chk随時1_2学年.Checked = False
        Chk随時1_3学年.Checked = False
        Chk随時1_4学年.Checked = False
        Chk随時1_5学年.Checked = False
        Chk随時1_6学年.Checked = False
        Chk随時1_7学年.Checked = False
        Chk随時1_8学年.Checked = False
        Chk随時1_9学年.Checked = False
        Chk随時1_全学年.Checked = False

        Chk随時2_1学年.Checked = False
        Chk随時2_2学年.Checked = False
        Chk随時2_3学年.Checked = False
        Chk随時2_4学年.Checked = False
        Chk随時2_5学年.Checked = False
        Chk随時2_6学年.Checked = False
        Chk随時2_7学年.Checked = False
        Chk随時2_8学年.Checked = False
        Chk随時2_9学年.Checked = False
        Chk随時2_全学年.Checked = False

        Chk随時3_1学年.Checked = False
        Chk随時3_2学年.Checked = False
        Chk随時3_3学年.Checked = False
        Chk随時3_4学年.Checked = False
        Chk随時3_5学年.Checked = False
        Chk随時3_6学年.Checked = False
        Chk随時3_7学年.Checked = False
        Chk随時3_8学年.Checked = False
        Chk随時3_9学年.Checked = False
        Chk随時3_全学年.Checked = False

        Chk随時4_1学年.Checked = False
        Chk随時4_2学年.Checked = False
        Chk随時4_3学年.Checked = False
        Chk随時4_4学年.Checked = False
        Chk随時4_5学年.Checked = False
        Chk随時4_6学年.Checked = False
        Chk随時4_7学年.Checked = False
        Chk随時4_8学年.Checked = False
        Chk随時4_9学年.Checked = False
        Chk随時4_全学年.Checked = False

        Chk随時5_1学年.Checked = False
        Chk随時5_2学年.Checked = False
        Chk随時5_3学年.Checked = False
        Chk随時5_4学年.Checked = False
        Chk随時5_5学年.Checked = False
        Chk随時5_6学年.Checked = False
        Chk随時5_7学年.Checked = False
        Chk随時5_8学年.Checked = False
        Chk随時5_9学年.Checked = False
        Chk随時5_全学年.Checked = False

        Chk随時6_1学年.Checked = False
        Chk随時6_2学年.Checked = False
        Chk随時6_3学年.Checked = False
        Chk随時6_4学年.Checked = False
        Chk随時6_5学年.Checked = False
        Chk随時6_6学年.Checked = False
        Chk随時6_7学年.Checked = False
        Chk随時6_8学年.Checked = False
        Chk随時6_9学年.Checked = False
        Chk随時6_全学年.Checked = False

    End Sub

    '
    '　関数名　-　Sb_Zuiji_Cmb
    '
    '　機能    -  随時振替日入出金区分初期化 
    '
    '　引数    -  随時共に初期化
    '
    '　備考    -  
    '
    '　
    Private Sub Sb_ZuijiCmbClear()

        Try
            '***ログ情報***
            STR_COMMAND = "Sb_ZuijiCmbClear"
            STR_LOG_GAKKOU_CODE = ""
            STR_LOG_FURI_DATE = ""
            '***ログ情報**

            'テキストファイルからコンボボックス設定
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分1) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分1)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分2) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分2)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分3) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分3)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分4) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分4)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分5) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分5)")
                Exit Sub
            End If
            If GFUNC_TXT_TO_DBCOMBO(STR_TXT_PATH & STR_NYUSYUTU2_TXT, cmb入出区分6) = False Then
                Call GSUB_LOG(0, "コンボボックス設定(cmb入出区分6)")
                Exit Sub
            End If

            cmb入出区分1.SelectedIndex = 0
            cmb入出区分2.SelectedIndex = 0
            cmb入出区分3.SelectedIndex = 0
            cmb入出区分4.SelectedIndex = 0
            cmb入出区分5.SelectedIndex = 0
            cmb入出区分6.SelectedIndex = 0

            cmb入出区分1.Enabled = True
            cmb入出区分2.Enabled = True
            cmb入出区分3.Enabled = True
            cmb入出区分4.Enabled = True
            cmb入出区分5.Enabled = True
            cmb入出区分6.Enabled = True

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
        End Try

    End Sub
    '
    '　関数名　-　Sb_ClearStructData
    '
    '　機能    -  画面データ保持構造体の初期化
    '
    '　引数    -  
    '
    '　備考    -  通常、随時共に初期化
    '
    '　
    Private Sub Sb_StructDataClear()

        '通常振替日
        For No As Integer = 1 To 5 Step 1
            '対象フラグ
            Tuujyou_SchInfo(No).TaisyouFlg = False
            '各フラグ
            Tuujyou_SchInfo(No).SyoriFurikae_Flag = False
            Tuujyou_SchInfo(No).CheckFurikae_Flag = False
            Tuujyou_SchInfo(No).FunouFurikae_Flag = False
            Tuujyou_SchInfo(No).SyoriSaiFurikae_Flag = False
            Tuujyou_SchInfo(No).CheckSaiFurikae_Flag = False
            '請求年月度
            Tuujyou_SchInfo(No).Seikyu_Nen = ""
            Tuujyou_SchInfo(No).Seikyu_Tuki = ""
            '振替日
            Tuujyou_SchInfo(No).Furikae_Tuki = ""
            Tuujyou_SchInfo(No).Furikae_Date = ""
            Tuujyou_SchInfo(No).Furikae_Check = False
            Tuujyou_SchInfo(No).Furikae_Enabled = False
            '再振日
            Tuujyou_SchInfo(No).SaiFurikae_Tuki = ""
            Tuujyou_SchInfo(No).SaiFurikae_Date = ""
            Tuujyou_SchInfo(No).SaiFurikae_Check = False
            Tuujyou_SchInfo(No).SaiFurikae_Enabled = False
            '学年フラグ
            Tuujyou_SchInfo(No).SiyouGakunen1_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen2_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen3_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen4_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen5_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen6_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen7_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen8_Check = False
            Tuujyou_SchInfo(No).SiyouGakunen9_Check = False
            '画面表示振替、再振日
            Tuujyou_SchInfo(No).Furikae_Day = ""
            Tuujyou_SchInfo(No).SaiFurikae_Day = ""

            '対象フラグ
            Syoki_Tuujyou_SchInfo(No).TaisyouFlg = False
            '各フラグ
            Syoki_Tuujyou_SchInfo(No).SyoriFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).CheckFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).FunouFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).SyoriSaiFurikae_Flag = False
            Syoki_Tuujyou_SchInfo(No).CheckSaiFurikae_Flag = False
            '請求年月度
            Syoki_Tuujyou_SchInfo(No).Seikyu_Nen = ""
            Syoki_Tuujyou_SchInfo(No).Seikyu_Tuki = ""
            '振替日
            Syoki_Tuujyou_SchInfo(No).Furikae_Tuki = ""
            Syoki_Tuujyou_SchInfo(No).Furikae_Date = ""
            Syoki_Tuujyou_SchInfo(No).Furikae_Check = False
            Syoki_Tuujyou_SchInfo(No).Furikae_Enabled = False
            '再振日
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Tuki = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Date = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Check = False
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Enabled = False
            '学年フラグ
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen1_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen2_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen3_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen4_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen5_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen6_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen7_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen8_Check = False
            Syoki_Tuujyou_SchInfo(No).SiyouGakunen9_Check = False
            '画面表示振替、再振日
            Syoki_Tuujyou_SchInfo(No).Furikae_Day = ""
            Syoki_Tuujyou_SchInfo(No).SaiFurikae_Day = ""
        Next

        '随時振替日
        For No As Integer = 1 To 6 Step 1
            Zuiji_SchInfo(No).TaisyouFlg = False
            Zuiji_SchInfo(No).Furikae_Nen = ""
            Zuiji_SchInfo(No).Furikae_Tuki = ""
            Zuiji_SchInfo(No).Furikae_Date = ""
            Zuiji_SchInfo(No).Nyusyutu_Kbn = "2"
            Zuiji_SchInfo(No).SiyouGakunen1_Check = False
            Zuiji_SchInfo(No).SiyouGakunen2_Check = False
            Zuiji_SchInfo(No).SiyouGakunen3_Check = False
            Zuiji_SchInfo(No).SiyouGakunen4_Check = False
            Zuiji_SchInfo(No).SiyouGakunen5_Check = False
            Zuiji_SchInfo(No).SiyouGakunen6_Check = False
            Zuiji_SchInfo(No).SiyouGakunen7_Check = False
            Zuiji_SchInfo(No).SiyouGakunen8_Check = False
            Zuiji_SchInfo(No).SiyouGakunen9_Check = False
            Zuiji_SchInfo(No).Syori_Flag = False

            Syoki_Zuiji_SchInfo(No).TaisyouFlg = False
            Syoki_Zuiji_SchInfo(No).Furikae_Nen = ""
            Syoki_Zuiji_SchInfo(No).Furikae_Tuki = ""
            Syoki_Zuiji_SchInfo(No).Furikae_Date = ""
            Syoki_Zuiji_SchInfo(No).Nyusyutu_Kbn = "2"
            Syoki_Zuiji_SchInfo(No).SiyouGakunen1_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen2_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen3_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen4_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen5_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen6_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen7_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen8_Check = False
            Syoki_Zuiji_SchInfo(No).SiyouGakunen9_Check = False
            Syoki_Zuiji_SchInfo(No).Syori_Flag = False
        Next

    End Sub

#End Region

#Region "INSERTSCHMAST"
    '
    '　関数名　-　fn_INSERTSCHMAST
    '
    '　機能    -  スケジュール作成
    '
    '　引数    -  TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:個別 　2:一括
    '
    '　備考    -  通常、随時共に初期化
    '
    '　
    Private Function fn_INSERTSCHMAST(ByVal aTORIS_CODE As String, ByVal aTORIF_CODE As String, ByVal aFURI_DATE As String) As Integer
        '----------------------------------------------------------------------------
        'Name       :fn_insert_SCHMAST
        'Description:スケジュール作成
        'Parameta   :TORIS_CODE , TORIF_CODE,FURI_DATE,TIME_STAMP,PG_KUBUN 1:個別 　2:一括
        'Create     :2004/08/02
        'UPDATE     :2007/12/26
        '           :***修正　にｶｽﾀﾏｲｽﾞ (企業自振ｽｹｼﾞｭｰﾙﾏｽﾀ生成時に企業側ｽｹｼﾞｭｰﾙﾏｽﾀの項目追加の為）
        '----------------------------------------------------------------------------

        Dim RetCode As Integer = gintKEKKA.NG

        Try
            Dim SQL As StringBuilder
            Dim SCH_DATA(77) As String

            Dim strFURI_DATE As String
            Dim strSOU_KBN As String
            Dim strKYU_KBN As String
            Dim intSFURI_KBN As Integer
            Dim strTESUU_KIJITSU As String
            Dim strKESSAI_KYUCD As Integer '決済休日コード
            Dim strTESUU_CNTDATE As Integer '日数/基準日

            Dim Ret As String

            Dim CLS As New MAIN.ClsSchduleMaintenanceClass
            '2012/09/04 saitou 警告解除 MODIFY -------------------------------------------------->>>>
            CLS.SetSchTable = MAIN.ClsSchduleMaintenanceClass.APL.JifuriApplication
            'CLS.SetSchTable = CLS.APL.JifuriApplication
            '2012/09/04 saitou 警告解除 MODIFY --------------------------------------------------<<<<

            strFURI_DATE = aFURI_DATE.Substring(0, 4) & "/" & aFURI_DATE.Substring(4, 2) & "/" & aFURI_DATE.Substring(6, 2)

            '----------------
            '取引先マスタ検索
            '----------------
            SQL = New StringBuilder(128)

            SQL.Append(" SELECT * FROM TORIMAST ")
            SQL.Append(" WHERE TORIS_CODE_T = '" & aTORIS_CODE.Trim & "'")
            SQL.Append(" AND TORIF_CODE_T = '" & aTORIF_CODE.Trim & "'")

            Dim OraCommand As New OracleClient.OracleCommand
            Dim OraReader As OracleClient.OracleDataReader
            OraCommand.CommandText = SQL.ToString
            OraCommand.Connection = OBJ_CONNECTION
            OraCommand.Transaction = OBJ_TRANSACTION

            OraReader = OraCommand.ExecuteReader   '読込のみ

            '読込のみ
            If OraReader.Read = False Then
                MessageBox.Show("取引先マスタに再振取引先が登録されていません", msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning)
                RetCode = gintKEKKA.NG
                Exit Try
            End If

            strSOU_KBN = ConvNullToString(OraReader.Item("SOUSIN_KBN_T"))
            strKYU_KBN = GCom.NzDec(OraReader.Item("FURI_KYU_CODE_T"), 0).ToString
            strTESUU_KIJITSU = GCom.NzDec(OraReader.Item("TESUUTYO_KIJITSU_T"), 0).ToString

            strTESUU_CNTDATE = OraReader.Item("TESUUTYO_DAY_T")
            strKESSAI_KYUCD = GCom.NzDec(OraReader.Item("TESUU_KYU_CODE_T"), 0).ToString
            intSFURI_KBN = OraReader.Item("SFURI_FLG_T")


            '-------------------------------------
            '振替日は営業日の営業日判定（土・日・祝祭日判定）
            '-------------------------------------
            'スケジュール作成対象の取引先コードを抽出
            CLS.GET_SELECT_TORIMAST(GCom.SET_DATE(aFURI_DATE), aTORIS_CODE, aTORIF_CODE)

            CLS.SCH.FURI_DATE = GCom.SET_DATE(aFURI_DATE)
            If CLS.SCH.FURI_DATE = "00000000" Then
            Else
                CLS.SCH.FURI_DATE = CLS.SCH.FURI_DATE.Substring(0, 10).Replace("/"c, "")
            End If

            strFURI_DATE = CLS.SCH.FURI_DATE

            Ret = CLS.INSERT_NEW_SCHMAST(0, False, True)

            '------------------
            'マスタ登録項目設定
            '------------------
            SCH_DATA(0) = OraReader.Item("FSYORI_KBN_T")                                       '振替処理区分
            SCH_DATA(1) = aTORIS_CODE                                                           '取引先主コード
            SCH_DATA(2) = aTORIF_CODE                                                           '取引先副コード
            SCH_DATA(3) = CLS.SCH.FURI_DATE 'strIN_NEN & strIN_TUKI & strIN_HI 'FURI_DATE_S　 　'振替日
            SCH_DATA(4) = CLS.SCH.FURI_DATE '"00000000" 'SAIFURI_DATE_S                         '契約振替日=振替日
            SCH_DATA(5) = "00000000"                                                            '再振日
            SCH_DATA(6) = CLS.SCH.KSAIFURI_DATE                                                 '再振予定日
            SCH_DATA(7) = CStr(ConvNullToString(OraReader.Item("FURI_CODE_T"))).PadLeft(3, "0")  '振替コードＳ
            SCH_DATA(8) = CStr(ConvNullToString(OraReader.Item("KIGYO_CODE_T"))).PadLeft(5, "0") '企業コードＳ
            SCH_DATA(9) = CLS.TR(0).ITAKU_CODE '委託者コード
            SCH_DATA(10) = CStr(OraReader.Item("TKIN_NO_T")).PadLeft(4, "0")
            SCH_DATA(11) = CStr(OraReader.Item("TSIT_NO_T")).PadLeft(3, "0")
            SCH_DATA(12) = OraReader.Item("SOUSIN_KBN_T")
            SCH_DATA(13) = OraReader.Item("MOTIKOMI_KBN_T")
            SCH_DATA(14) = OraReader.Item("BAITAI_CODE_T") 'BAITAI_CODE_S
            SCH_DATA(15) = 0 'MOTIKOMI_SEQ_S
            SCH_DATA(16) = 0 'FILE_SEQ_S
            '手数料計算区分の算出
            Dim strTUKI_KBN As String = ""
            Select Case aFURI_DATE.Substring(4, 2)
                Case "01"
                    strTUKI_KBN = OraReader.Item("TUKI1_T")
                Case "02"
                    strTUKI_KBN = OraReader.Item("TUKI2_T")
                Case "03"
                    strTUKI_KBN = OraReader.Item("TUKI3_T")
                Case "04"
                    strTUKI_KBN = OraReader.Item("TUKI4_T")
                Case "05"
                    strTUKI_KBN = OraReader.Item("TUKI5_T")
                Case "06"
                    strTUKI_KBN = OraReader.Item("TUKI6_T")
                Case "07"
                    strTUKI_KBN = OraReader.Item("TUKI7_T")
                Case "08"
                    strTUKI_KBN = OraReader.Item("TUKI8_T")
                Case "09"
                    strTUKI_KBN = OraReader.Item("TUKI9_T")
                Case "10"
                    strTUKI_KBN = OraReader.Item("TUKI10_T")
                Case "11"
                    strTUKI_KBN = OraReader.Item("TUKI11_T")
                Case "12"
                    strTUKI_KBN = OraReader.Item("TUKI12_T")
            End Select

            Select Case OraReader.Item("TESUUTYO_KBN_T")
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

            SCH_DATA(18) = "00000000"              '依頼書作成日
            SCH_DATA(19) = CLS.SCH.IRAISYOK_YDATE  '依頼書回収予定日
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
            'スケジュールマスタ登録
            '----------------------
            SQL = New StringBuilder(1024)

            SQL.Append(" INSERT INTO SCHMAST ( ")
            SQL.Append(" FSYORI_KBN_S")     '0
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

            If GFUNC_EXECUTESQL_TRANS(InsertSchmastSQL, 1) = False Then
                Call GSUB_LOG(0, "自振スケジュールInsert失敗、SQL:" & InsertSchmastSQL)
                ERRMSG = "自振スケジュール作成に失敗しました"
                Exit Try
            End If

            RetCode = gintKEKKA.OK

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ERRMSG = "自振スケジュール作成に失敗しました"
            RetCode = gintKEKKA.NG
        End Try

        Return RetCode

    End Function
#End Region
    '
    '　関数名　-　fn_Hosei
    '
    '　機能    -  補正
    '
    '　引数    -  補正
    '
    '　備考    -  補正
    '
    '　
    Private Function fn_Hosei(ByVal aDate As String) As String

        Try
            If aDate.Length <> 8 Then
                Return aDate
            End If

            If IsDate(aDate.Substring(0, 4) & "/" & aDate.Substring(4, 2) & "/" & aDate.Substring(6, 2)) = True Then
                Return aDate
            Else
                Dim MaxDay As Integer = Date.DaysInMonth(CInt(aDate.Substring(0, 4)), CInt(aDate.Substring(4, 2)))
                Return aDate.Substring(0, 4) & aDate.Substring(4, 2) & Format(MaxDay, "00")
            End If

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            Return aDate
        End Try

    End Function
    '
    '　関数名　-　fn_ToriMastIsExist
    '
    '　機能    -  取引先マスタ存在チェック
    '
    '　引数    -  TORIS_CODE,TORIF_CODE,CONNECTION
    '
    '　備考    -  通常、随時共に初期化
    '
    '　
    Private Function fn_ToriMastIsExist(ByVal TorisCode As String, _
                                        ByVal TorifCode As String, _
                                        ByVal OraConn As OracleClient.OracleConnection, _
                                        ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim SQL As String = ""

        SQL = " SELECT * "
        SQL &= " FROM TORIMAST "
        SQL &= " WHERE TORIS_CODE_T = '" & TorisCode & "'"
        SQL &= " AND TORIF_CODE_T = '" & TorifCode & "'"

        Return fn_IsExist(SQL, OraConn, OraTran)

    End Function
    '
    '　関数名　-　fn_SchMastIsExist
    '
    '　機能    -  スケジュールマスタ存在チェック
    '
    '　引数    -  TORIS_CODE,TORIF_CODE,FURI_DATE,CONNECTION
    '
    '　備考    -  通常、随時共に初期化
    '
    '　
    Private Function fn_SchMastIsExist(ByVal TorisCode As String, _
                                       ByVal TorifCode As String, _
                                       ByVal FuriDate As String, _
                                       ByVal OraConn As OracleClient.OracleConnection, _
                                       ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim SQL As String = ""

        SQL = " SELECT * "
        SQL &= " FROM SCHMAST "
        SQL &= " WHERE TORIS_CODE_S = '" & TorisCode & "'"
        SQL &= " AND TORIF_CODE_S = '" & TorifCode & "'"
        SQL &= " AND FURI_DATE_S = '" & FuriDate & "'"

        Return fn_IsExist(SQL, OraConn, OraTran)

    End Function
    '
    '　関数名　-　fn_IsExist
    '
    '　機能    -  スケジュール作成
    '
    '　引数    -  SQL,CONNECTION
    '
    '　備考    -  通常、随時共に初期化
    '
    '　
    Private Function fn_IsExist(ByVal SQL As String, _
                                ByVal OraConn As OracleClient.OracleConnection, _
                                ByVal OraTran As OracleClient.OracleTransaction) As Boolean

        Dim ret As Boolean = False
        Dim ConnFlg As Boolean = True
        Dim OraReader As OracleClient.OracleDataReader = Nothing

        Try
            Dim OraCommand As New OracleClient.OracleCommand
            OraCommand.CommandText = SQL
            OraCommand.Connection = OraConn
            OraCommand.Transaction = OraTran
            OraReader = OraCommand.ExecuteReader

            If OraReader.Read = False Then
                ret = False
            Else
                ret = True
            End If

            OraReader.Close()
            OraReader = Nothing

        Catch ex As Exception
            Call GSUB_LOG(0, "予期せぬエラー:" & ex.ToString)
            ret = False
        Finally
            If Not OraReader Is Nothing Then
                OraReader.Close()
                OraReader = Nothing
            End If
        End Try

        Return ret

    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Private Sub Label3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label3.Click

    End Sub
End Class
