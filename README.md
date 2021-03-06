## 2D Player Controller の移動系動作サンプル

Unity 2020.3

ほぼ [これ](https://github.com/tukanpo/cm14-platformer/blob/master/Assets/App/Scripts/Scenes/Game/PlayerController.cs) がすべて。後はセンサー用のコンポーネント

- velocity で制御

  物理演算に影響されない動作を行う為に Rigidbody の Gravity はオフ

  Rigidbody は衝突処理と velocity の為に使用

- 慣性付き移動

- 大ジャンプ / 小ジャンプ

  ボタンを押し続けたら大ジャンプでは無く、離したら小ジャンプという考え方

- コーナーでジャンプ補正

  ジャンプ時に天井のコーナーを検出して位置補正

  スーパーマリオの時代からあるやつ

- Hangtime (Coyote time)

  地面から落ちた瞬間は空中でもジャンプ可能にする

- 壁スライド / 壁ジャンプ

  ついでに実装

## 参考

[Celesteの操作が心地よい理由 - Game Maker's Toolkit](https://www.youtube.com/watch?v=yorTG9at90g)

[【Unity】 Rigidbodyの移動方法](https://www.f-sp.com/entry/2016/08/16/211214)

[5 Tips for Better Platformers in Unity (With Code)!](https://www.youtube.com/watch?v=8QPmhDYn6rk)

[3 Tips for a Juicy Jump (Unity 2D Platformer - Part 3)](https://www.youtube.com/watch?v=A_F8R3eGtrs)

[2D Wall interactions (Unity 2D Platformer - Part 4)](https://www.youtube.com/watch?v=JIASeoOU274)
