# Epic Siege (Continued) – バグ修正とパフォーマンス改善

このリポジトリは、以下のMODのバグ修正およびパフォーマンス改善を行ったものです。
Epic Siege (Continued)  
https://steamcommunity.com/sharedfiles/filedetails/?id=3599382443

RimWorld 1.6でのみ動作確認済みです。

---

## バグ修正

元の Epic Siege (Continued) では、2つ目の拠点を破壊した際に、3つ目の拠点も同時に破壊する処理が存在します。

この処理は Harmony patch により、以下のメソッドにフックして実装されていました：

Site.Notify_MyMapAboutToBeRemoved


しかし、この処理により：

- 現在プレイヤーがいる拠点（Site）
- 破壊対象の別の拠点（Site）

の現在プレイヤーがいる拠点に対して Destroy() が実行され、削除処理が二重に実行されていました。

その結果、以下のエラーが発生していました：

Error while deiniting map: could not notify things/regions/rooms/etc:

本MODでは、拠点削除処理の管理方法を修正し、Destroy() が重複して実行されないようにすることで、この問題を解決しています。

---

## パフォーマンス改善
本MODでは、以下の方法に変更しました：

- `WorldObjectComp` を使用して拠点を追跡
- `Site` を継承したクラスとコンポーネントで対象を直接管理
- コンポーネント登録ベースで拠点を管理

これにより：

- `Find.WorldObjects` による全検索が不要
- 不要なループ処理を削減
- より安全かつ効率的な拠点管理が可能

---

## Harmony Patch の削除

従来は Harmony patch を使用して拠点削除処理を制御していましたが、

本MODでは：

- `WorldObjectComp`
- `Site` 継承クラス

を使用することで、Harmony patch を使用せずに同等の機能を実装しています。

これにより：

- HarmonyPatch自体も正直重いのでそれがなくなる
- 競合リスクの低減
- より自然な RimWorld のライフサイクルに沿った実装

を実現しています。
