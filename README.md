# UnityToolLauncher

## 機能一覧

### ツールを開く
ツール一覧をリスト表示し、選択したツールを開くことができます。

![1700891112.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/dc8118a6-393d-03f7-b2e0-f99149bd4e1c.gif)

### ヘルプを開く
ツールのヘルプページをブラウザで開くことができます。

![1700891262.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/ea3b3d05-5be7-1a22-0a27-76cbf1912ea8.gif)

### ツールの追加・カスタマイズ
チームや個人で自作したツールも追加可能です。

![1700892285.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/576990e9-8538-6f10-2cbb-ccc74eb4f930.gif)

### プリセット
プリセットを作成して、表示するツールを分けることができます。
たとえばデバッグ時に使うツール、UI制作時に使うツールなどを分けておき切り替えることができます。
![1700892763.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/fba97805-089c-dd10-98bf-6a3086a7a841.gif)

## インストール方法
PackageManagerから `https://github.com/rarudo/UnityToolLauncher.git`を入力してインストールします

<img width="182" alt="スクリーンショット 2023-11-27 20 57 45" src="https://github.com/rarudo/UnityToolLauncher/assets/15700036/73b0d85e-f33d-4d92-bc67-e69629ab8abc">

<img width="344" alt="スクリーンショット 2023-11-27 20 58 06" src="https://github.com/rarudo/UnityToolLauncher/assets/15700036/ddf706b0-19a7-43d2-821e-2cb5238cb40b">


### 設定方法
それぞれの設定方法は以下の通りです

| 設定項目        | 画像                                                                                                                      | 説明                                                                                                                                                                                                               |
| --------------- | ------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 略称            | ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/b3f04c2e-fd08-e5d9-ae13-bd81f63924c8.png) | アイコンとして表示される略称を指定します <br> ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/e867c608-f17f-93fa-90f1-401051547a43.png)                                            |
| 名前            | ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/7e7c5016-2be4-3868-154b-13fa4f185159.png) | ツールの名前を指定します <br> ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/8f4d834b-6baf-97de-af31-75ef3c89608d.png)                                                            |
| 説明            | ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/13704a6f-9027-4b5e-3abb-a8ad967f6e1a.png) | ツールの説明を記入します <br> ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/e30f6d24-071d-0050-cf41-7dfa43fb19ce.png)                                                            |
| 説明URL(省略化) | ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/bd17f7bc-81d4-07a3-9c72-785f68caf6a9.png) | ツールのマニュアルがあるURLを禁輸します。省略した場合はボタンが表示されなくなります <br> ![image.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/59c45fa4-69f9-f4ef-4371-be3aee707db5.png) |
| 起動ツール      | ![スクリーンショット 2023-11-26 9.14.12.png](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/83313cc7-c84e-5511-20d4-f81266b27b4d.png) |  起動ツールの設定方法を参照                                                                                                                                                                                                                  |

#### 起動ツールの設定方法
起動ツールを指定するにはツール検索ボタンを押して、ツールを選択します。

##### 例 ) Light Explorerを指定する場合

ツール検索ボタンからLight Explorerを指定します。
![toolselect.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/fd477210-0366-e712-2667-88d4d561e348.gif)

テスト実行ボタンから、指定したツールをテストすることができます

![画面収録 2023-11-26 9.44.09-lossy.gif](https://qiita-image-store.s3.ap-northeast-1.amazonaws.com/0/213392/4e863354-7560-e1c2-6f4a-8851014b4e2a.gif)

## ツールを作ったきっかけ

チームでUnityを使いゲーム開発を進めていく中で私たちは多くのツールを使用し、場合によっては独自ツールを開発します。
これらのツールは誰かの作業を便利にしますが、ツールが増えるにつれて、管理やアクセスが煩雑になり、せっかく作ったツールが使われない・チームで共有されないといった問題が発生します。
そこで制作した独自ツールを最大限活用するために、ツールランチャーを作成しました。
簡単な操作でツールを素早く開けるだけでなく、ヘルプページへ簡単にアクセスできる機能や日本語での説明文など、チームで使われることを意識した設計となっています。

