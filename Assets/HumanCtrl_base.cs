#region ' Using '
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion
public class HumanCtrl_base : MonoBehaviour
{
    #region ' Decl '
    public GameObject brain__camera; //  GameObjectも，ダブルスネーク．
    public ScOb_PlayerData scri__obj;
    public CharacterController chara__con; //  Componentは，ダブルスネークで命名．("_"を2コつなげる)
    public Animator ani__mator;
    private Vector2 moving; // Vector2のnew

    private float animationBlend;
    private int animIDSpeed;
    private int animIDMotionSpeed;
    #endregion
    #region ' 入力処理 '  (Vector2)->moving
    // キー入力を受け取る処理．スティックの入力(Vector2)を moving にset．
    void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    void MoveInput(Vector2 direction)
    {
        moving = direction;
    }
    #endregion
    #region ' Start() '  St=>Anim; Up=>Move_Ctrl();
    void Start()
    {
        AssignAnimationIDs();
    }
    void Update()
    {
        Move_Ctrl();
    }
    #endregion
    void Move_Ctrl()
    {
        #region ' Local Decl '
        float _wantRotation = 0.0f;
        float _tempSpeed;
        float _conclusiveSpeed;
        float _speedOffset = 0.1f;
        float _targetSpeed = scri__obj.moveSpeed; //  ローカル変数は，"_"を冒頭につけて，キャメル．
        float nowSpeed_Horizon = new Vector3(chara__con.velocity.x, 0.0f, chara__con.velocity.z).magnitude; //  こんな命名規則(aA_A)はないが，ローカル変数だから多少命名規則から外れてもいいかぁ．
        #endregion

        if (moving == Vector2.zero) _targetSpeed = 0.0f; // _targetSpeedには，既に，規定速度 (=moveSpeed) が代入済み．

        if ((nowSpeed_Horizon < _targetSpeed - _speedOffset) || (nowSpeed_Horizon > _targetSpeed + _speedOffset))   //  条件の反転(&&の使用)を考えたが，そうすれば条件を毎回2条件とも調べることになる(今は，"または"だから片方調べなくていい可能性アリ)．<=いやfalseだったらが反転時が速い．
        {  //  現在の速さ と _targetSpeed に差があるときの処理．                                                               //☟1sで到達してほしい速さの割合がSpeedChangeRate(0~1)である．
            _tempSpeed = Mathf.Lerp(nowSpeed_Horizon, _targetSpeed * moving.magnitude, Time.deltaTime * scri__obj.speedChangeRate); //  これのおかげで，スティックをいきなり全倒し(いきなり入力値が0→1)しても，移動速度が0→1にならずに済んでいる．//  moving.magnitudeは，Keyboard操作の時に多分思い通りに動いてくれない．analog入力に対応させていないから．まぁとりあえず放置．
            _conclusiveSpeed = Mathf.Round(_tempSpeed * 1000f) / 1000f;  //  偶数丸めして，小数第3位までのfloatにする処理．//  なんでこれをするのかは理解していない．    
        }
        else
        {  // 現在の速さ ~ _targetSpeed のときの処理．
            _conclusiveSpeed = _targetSpeed;
        }
        #region ' Anim '
        animationBlend = Mathf.Lerp(animationBlend, _targetSpeed, Time.deltaTime * scri__obj.speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
        #endregion
        Vector3 _movingDirection = new Vector3(moving.x, 0.0f, moving.y).normalized; //  Gamepad_Schemeのときは既に，正規化されたベクトルを送ってくれてるとは思うけど，一応ここでも正規化．

        if (moving != Vector2.zero)
        {   //☟  if文の中で宣言されたローカルローカル変数なので，"_"は頭に2コつける．
            float __num = 0; //  計算の過程で内部的に使われる変数らしい．
            _wantRotation = Mathf.Atan2(_movingDirection.x, _movingDirection.z) * Mathf.Rad2Deg + brain__camera.transform.eulerAngles.y; //  最後の足し算はカメラの正面を向いてほしいのか？
            float __rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _wantRotation, ref __num, scri__obj.rotationSmoothTime);  //  これもMathf.Lerpのように急激な変化を防いで，段階的に振りむけてくれる．
                                                                                                                                        //  詳しくは，h_ttps://nekojara.city/unity-smooth-damp参照．
            transform.rotation = Quaternion.Euler(0.0f, __rotation, 0.0f); //  このおかげで，キャラは絶対に傾かない．
        }
        //☟  ここだけ，よく分かってない．
        Vector3 _conclusiveDirection = Quaternion.Euler(0.0f, _wantRotation, 0.0f) * Vector3.forward; //  当人(ここでは，P0_Man)の向き(右を向いているか，左を向いているか)に関わらず，当人の現在のまっすぐ前を得る計算．h_ttps://tsubakit1.hateblo.jp/entry/2014/08/02/030919 参照
        chara__con.Move(_conclusiveDirection.normalized * (_conclusiveSpeed * Time.deltaTime) + new Vector3(0.0f, -2.0f, 0.0f) * Time.deltaTime); //  これが本命の，移動を命令している箇所．引数は3次元ベクトル．詳しくは，h_ttps://nekojara.city/unity-input-system-character-controllerの下の方参照．
        #region ' Anim '
        ani__mator.SetFloat(animIDSpeed, animationBlend);
        ani__mator.SetFloat(animIDMotionSpeed, moving.magnitude);
        #endregion
    }

    #region ' Anim_Hash '
    void AssignAnimationIDs()    // ハッシュ値にすると，なんで処理が速くなるんだっけ？
    {  //Start() でanimationの遷移条件に ID を"Assign"(付与)
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
    #endregion
}
