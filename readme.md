# Penguintter
twitter client for windows

## 概要
ニコニコ動画コメント風Twitterクライアントです。

## 動作要件
.Net Framework 4.5.2以降が動作するWindowsPC

## 使い方
app以下にビルド済みの実行ファイルがありますので、penguintter.exeを実行して起動してください。初回はPINの入力を求められます。**PIN認証を済ませると生成されるpenguintter.accountは、絶対に他人に見せないようにしてください。**

- `Alt+Ctrl+T`で投稿画面を開く。
    - `Ctrl+Enter`または`Tweet`をクリックして投稿します。
- `Alt+Ctrl+L`でログ確認画面を開く。
    - ログをクリックすると詳細画面を開きます。
- `Alt+Ctrl+C`で設定画面を開く。
- 起動後に表示されるロゴ画面の`Shutdown`をクリックして終了する。


## そのうち直すかも
- アイコンの読み込みに失敗したとき、再読込処理を行いません。
- ツイートが重なって表示されることがあります。
- 各画面の表示位置が不定のため、投稿画面の上に詳細画面が来て鬱陶しいことがあります。

## そのうち更新するかも
常駐ソフト化するかも。

## 更新履歴
- ver 2.0.0: Formアプリケーションとして開発したPenguintterをWPFとして作り直しました。
