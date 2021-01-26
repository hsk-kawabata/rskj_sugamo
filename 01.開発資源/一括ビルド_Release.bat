@ECHO OFF
ECHO ※全てのソリューションをビルドし、実行環境へ上書きします。
PAUSE
SET PATH="C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\"
IF EXIST build.log DEL build.log

ECHO バッチ、クラスをビルド中です．．．
devenv Batch/Batch.sln /Rebuild Release /out build.log
devenv Batch総振\Batch総振.sln /Rebuild Release /out build.log

ECHO RS企業自振_V2 追加機能をビルド中です．．．
devenv Common/CLS/ClsAstExternal/CAstExternal.sln /Rebuild Release /out build.log
devenv Common/CLS/ClsAstExternalPrint/CAstExternalPrint.sln /Rebuild Release /out build.log

ECHO 拡張印刷をビルド中です．．．
devenv Common/ExtendPrint/ExtendPrint.sln /Rebuild Release /out build.log

COPY DLL\*.dll EXE\ /Y >> build.log

ECHO 口座振替をビルド中です．．．
devenv 自振管理\JIFURI\JIFURI.sln /Rebuild Release /out build.log

ECHO 資金決済をビルド中です．．．
devenv 資金決済\KESSAI.sln /Rebuild Release /out build.log

ECHO 運用管理をビルド中です．．．
devenv JOB\ジョブ監視.sln /Rebuild Release /out build.log
devenv メイン\メイン.sln /Rebuild Release /out build.log
devenv 運用管理\Management.sln /Rebuild Release /out build.log

ECHO 総合振込をビルド中です．．．
devenv 総給振管理\SOUFURI.sln /Rebuild Release /out build.log

ECHO 学校諸会費をビルド中です．．．
devenv 学校自振\GAKKOU\GAKKOU.sln /Rebuild Release /out build.log

ECHO 媒体変換をビルド中です．．．
devenv 媒体変換\媒体変換.sln /Rebuild Release /out build.log

ECHO WEB伝送をビルド中です．．．
devenv WEB伝送\WEB_DENSO.sln /Rebuild Release /out build.log

ECHO 集金代行をビルド中です．．．
devenv 集金代行\SSS.sln /Rebuild Release /out build.log

ECHO 個別処理をビルド中です．．．
devenv 個別処理\DelDB\DelDB.sln /Rebuild Release /out build.log
devenv 個別処理\DelFile\DelFile.sln /Rebuild Release /out build.log
devenv 個別処理\伝送連携\伝送連携.sln /Rebuild Release /out build.log

ECHO 帳票印刷(口座振替)をビルド中です．．．
devenv 帳票印刷\企業自振\KFJP001.処理結果確認表(落込)\KFJP001_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP002.処理結果確認表(センター直接持込)\KFJP002_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP003.受付明細表\KFJP003_受付明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP006.他行明細表\KFJP006.他行明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP007.口座振替請求データ送付票\KFJP007.口座振替請求データ送付票.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP008.処理結果確認表(配信データ作成)\KFJP008.処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP009.センターカットデータ作成対象一覧\KFJP009.センターカットデータ作成対象一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP010.口座振替明細表\KFJP010.口座振替明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP012.預金口座振替変更通知書\預金口座振替変更通知書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP013.処理結果確認表(不能結果更新)\KFJP013_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP013_2.処理結果確認表(不能結果更新・企業持込)\KFJP013_処理結果確認表(不能結果更新・企業持込).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP014.スケジュールエラーリスト\スケジュールエラーリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP015.取引先マスタエラーリスト\取引先マスタエラーリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP016.返還データ作成一覧\KFJP016.返還データ作成一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP017.振替不能明細表\KFJP017.振替不能明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP018.振替結果明細表\KFJP018.振替結果明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP019.口座振替店別集計表\KFJP019.口座振替店別集計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP020.処理結果確認表(返還データ作成)\KFJP020_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP021.処理結果確認表(再振データ作成)\KFJP021_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP022.取引先マスタメンテ（登録）\取引先マスタメンテ(登録).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP023.取引先マスタメンテ（変更）\取引先マスタメンテ(変更).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP024.取引先マスタメンテ（削除）\取引先マスタメンテ(削除).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP025.他行マスタ一覧表\KFJP025.他行マスタ一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP026.処理結果確認表(落込取消)\KFJP026.処理結果確認表(落込取消).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP027.スケジュール進捗管理表\KFJP027.スケジュール進捗管理表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP028.口座振替依頼書\口座振替依頼書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP029_30.口座振替入力チェックリスト\口座振替入力チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP031.契約者一覧表\契約者一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP032.取引先マスタチェックリスト\取引先マスタチェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP033_34.スケジュール表\スケジュール表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP035.取引先マスタ索引簿\取引先マスタ索引簿.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP036.振替結果変更チェックリスト\振替結果変更チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP037.未処理一覧表(落込)\未処理一覧表(落込).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP038.口座振替データ送信報告書\口座振替データ送信報告書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP039.年金受取人別振込明細表\KFJP039.年金受取人別振込明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP040.年金振込支店コードチェックリスト\KFJP040.年金振込支店コードチェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP041.口座チェック確認表(当日受付分)\KFJP041.口座チェック確認表(当日受付分).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP042.口座チェック確認表(依頼分)\KFJP042.口座チェック確認表(依頼分).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP043.処理結果確認表(自振契約)\KFJP043.処理結果確認表(自振契約).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP044.処理結果確認表(自振契約結果)\KFJP044.処理結果確認表(自振契約結果).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP045.領収証書\KFJP045.領収証書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP048.領収控\KFJP048.領収控.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP049.口座振替用納付書送付書\KFJP049.口座振替用納付書送付書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP050.口座振替処理結果件数表\KFJP050.口座振替処理結果件数表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP051_地区伝送データ送信連絡票\KFJP051_地区伝送データ送信連絡票.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP052.振替不能事由別集計表\KFJP052.振替不能事由別集計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP053.自振管理リスト\自振管理リスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP054.処理結果確認表(落込一覧)\KFJP054_処理結果確認表(落込一覧).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP055.処理結果確認表(返還一覧)\KFJP055_処理結果確認表(返還一覧).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP056.振込手数料マスタ登録リスト\振込手数料マスタ登録リスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP057.手数料徴求フラグ一括更新一覧\KFJP057.手数料徴求フラグ一括更新一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP058.データ伝送通知書\データ伝送通知書.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP059.取引先マスタメンテ(登録)\取引先マスタメンテ(登録).sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP060.再振対象先チェックリスト\再振対象先チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP062.取引先マスタ項目確認票\KFJP062.取引先マスタ項目確認票.sln /Rebuild Release /out build.log
devenv 帳票印刷\企業自振\KFJP063.預金口座振替変更通知書\KFJP063.預金口座振替変更通知書.sln /Rebuild Release /out build.log

ECHO 帳票印刷(資金決済)をビルド中です．．．
devenv 帳票印刷\資金決済\KFKP001.処理結果確認表(資金決済)\処理結果確認表(資金決済).sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP002.処理結果確認表(資金決済取消)\処理結果確認表(資金決済取消).sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP003.処理結果確認表(資金決済結果)\処理結果確認表(資金決済結果).sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP004.自振処理企業一覧表\自振処理企業一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP005_06.資金決済企業一覧表\資金決済企業一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP007.手数料未徴求企業一覧表\手数料未徴求企業一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP008.手数料一括徴求明細表\手数料一括徴求明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\資金決済\KFKP009.預金口座振替内訳票手数料請求書\預金口座振替内訳票手数料請求書.sln /Rebuild Release /out build.log

ECHO 帳票印刷(運用管理)をビルド中です．．．
devenv 帳票印刷\運用管理\KFUP001.データ伝送ログ一覧\KFUP001.データ伝送ログ一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\運用管理\KFUP002.ジョブ監視状況一覧表\ジョブ監視状況確認一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\運用管理\KFUP003.登録ユーザ一覧表\登録ユーザ一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\運用管理\KFUP004.休日情報一覧表\休日情報一覧表.sln /Rebuild Release /out build.log

ECHO 帳票印刷(総合振込)をビルド中です．．．
devenv 帳票印刷\総合振込\KFSP001.処理結果確認表(落込)\KFSP001_処理結果確認表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP009.振込発信リエンタ対象一覧\振込発信リエンタ対象一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP010.為替振込明細表(本支店為替)\為替振込明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP011.為替振込明細表(他行為替)\為替振込明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP012.為替振込明細表(自振ロギング登録)\為替振込明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP013.総合振込明細表\総合振込明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP017.取引先マスタメンテ（登録）\取引先マスタメンテ(登録).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP018.取引先マスタメンテ（変更）\取引先マスタメンテ(変更).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP019.取引先マスタメンテ（削除）\取引先マスタメンテ(削除).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP020.処理結果確認表(落込取消)\KFSP020.処理結果確認表(落込取消).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP021.スケジュール進捗管理表\KFSP021.スケジュール進捗管理表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP022.総合振込依頼書\総合振込依頼書.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP023_24.総合振込入力チェックリスト\総合振込入力チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP025.契約者一覧表\契約者一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP026.取引先マスタチェックリスト\取引先マスタチェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP027_28.スケジュール表\スケジュール表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP029.取引先マスタ索引簿\取引先マスタ索引簿.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP031.取引先マスタメンテ(登録)\取引先マスタメンテ(登録).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP032.処理結果確認表(CSVリエンタ)\処理結果確認表(CSVリエンタ).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP033.総合振込明細表(CSVリエンタ)\総合振込明細表(CSVリエンタ).sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP034.FBタンキングデータ振込店別集計表\FBタンキングデータ振込店別集計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\総合振込\KFSP036.取引先マスタ項目確認票\KFSP036.取引先マスタ項目確認票.sln /Rebuild Release /out build.log

ECHO 帳票印刷(学校諸会費)をビルド中です．．．
devenv 帳票印刷\学校諸会費\KFGP001.インプットエラーリスト\インプットエラーリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP003_4.口座振替予定明細表\口座振替予定明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP005.口座振替予定集計表\口座振替予定集計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP006.学校マスタメンテ（登録）\学校マスタメンテ（登録）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP007.学校マスタメンテ（更新）\学校マスタメンテ（更新）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP008.学校マスタメンテ（削除）\学校マスタメンテ（削除）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP009.学校他行マスタ一覧\学校他行マスタ一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP010.費目マスタ一覧\費目マスタ一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP011.進級リスト\進級リスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP012.月間スケジュール表\月間スケジュール表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP013.口座振替予定一覧表\口座振替予定一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP014_15.口座振替予定一覧表(口座データ)\口座振替予定一覧表(口座データ).sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP016_17.口座振替結果一覧表\口座振替結果一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP018.収納報告書\収納報告書.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP019.口座振替店別集計表\口座振替店別集計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP020.口座振替未納のお知らせ\口座振替未納のお知らせ.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP021.普通預金入金伝票\普通預金入金伝票.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP022.未収リスト\未収リスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP023.収納状況一覧表\収納状況一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP024.収納状況一覧表(費目別合計)\収納状況一覧表(費目別合計).sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP025.学校マスタ索引簿\学校マスタ索引簿.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP026.学校生徒名簿\学校生徒名簿.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP027_28.生徒マスタ登録チェックリスト\生徒マスタ登録チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP029.長子チェックリスト明細表\長子チェックリスト明細表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP030.生徒明細入力チェックリスト\生徒明細入力チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP031.学校自振口座チェックリスト\学校自振口座チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP032.口座振替依頼書\口座振替依頼書.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP033.生徒マスタ管理表\生徒マスタ管理表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP035.学校マスタメンテ（登録）\学校マスタメンテ（登録）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP036.生徒マスタ整合性チェックリスト\生徒マスタ整合性チェックリスト.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP037.年間スケジュール表\年間スケジュール表.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP038.学校マスタ項目確認票\KFGP038.学校マスタ項目確認票.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP039.生徒登録情報一覧\生徒登録情報一覧.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP106.生徒マスタメンテ（登録）\生徒マスタメンテ（登録）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP107.生徒マスタメンテ（更新）\生徒マスタメンテ（更新）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP108.生徒マスタメンテ（削除）\生徒マスタメンテ（削除）.sln /Rebuild Release /out build.log
devenv 帳票印刷\学校諸会費\KFGP501.生徒データ取込登録リスト\KFGP501_生徒データ取込登録リスト.sln /Rebuild Release /out build.log

ECHO 帳票印刷(WEB伝送)をビルド中です．．．
devenv 帳票印刷\WEB伝送\KFWP001.WEB伝送ログ一覧\WEB伝送ログ一覧.sln /Rebuild Release /out build.log

ECHO 帳票印刷(集金代行)をビルド中です．．．
devenv 帳票印刷\集金代行\KF3SP001.SSS引落結果合計表\SSS引落結果合計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\集金代行\KF3SP002.SSS委託者別決済額一覧表\SSS委託者別決済額一覧表.sln /Rebuild Release /out build.log
devenv 帳票印刷\集金代行\KF3SP003.SSS銀行別振替結果合計表\SSS銀行別振替結果合計表.sln /Rebuild Release /out build.log
devenv 帳票印刷\集金代行\KF3SP004.SSS企業別結果表\SSS企業別結果表.sln /Rebuild Release /out build.log

ECHO RS企業自振_V2 追加機能をビルド中です．．．
devenv Common/CLS/ClsAstExternal/CAstExternal.sln /Rebuild Release /out build.log
devenv Common/CLS/ClsAstExternalPrint/CAstExternalPrint.sln /Rebuild Release /out build.log

ECHO 不要ファイルを削除中です・・・
FOR /F %%A IN ('DIR /B /S /A:D') DO (RMDIR /S /Q "%%A\obj" & RMDIR /S /Q "%%A\bin")
DEL  /S /Q *.pdb
DEL  /S /Q *.xml
DEL  /S /Q *.vshost.exe

