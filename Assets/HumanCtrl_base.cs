#region _System_
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion
public class HumanCtrl_base : MonoBehaviour{
    #region _Declarations_
    public GameObject brain__camera; //  GameObjectも，ダブルスネーク．
    public ScOb_PlayerData scri__obj;
    public CharacterController chara__con; //  Componentは，ダブルスネークで命名．("_"を2コつなげる)
    public Animator ani__mator;
    private Vector2 moving; //  初期化なしは，falseが初期値．

    private float animationBlend;
    private int animIDSpeed;
    private int animIDMotionSpeed;
    #endregion
    #region _OnMove_
    void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }
    void MoveInput(Vector2 direction)
    {
        moving = direction;
    }
    #endregion
    void Start()
    {
        AssignAnimationIDs();
    }
    void Update()
    {
        Debug.Log("test");
        Move_Ctrl();
    }
    void Move_Ctrl()
    {
        float _wantRotation = 0.0f;
        float _conclusiveSpeed;
        float _speedOffset = 0.1f;
        float _wantSpeed = scri__obj.moveSpeed; //  ローカル変数は，"_"を冒頭につけて，キャメル．
        float nowSpeed_Horizon = new Vector3(chara__con.velocity.x, 0.0f, chara__con.velocity.z).magnitude; //  newでこれらの値を保持する場所を新たに作ってるということ？//  こんな命名規則はないが，ローカル変数だから多少命名規則から外れてもいいかぁ．
        if (moving == Vector2.zero) _wantSpeed = 0.0f;
        
        if ((nowSpeed_Horizon < _wantSpeed - _speedOffset)|| //  wantとして設定しているSpeedにまだ遠かったら
            (nowSpeed_Horizon > _wantSpeed + _speedOffset))  //  条件を反転して，&&にしてもいいと思ったが，そうすると．条件を毎回に二個とも調べる必要が出てくるし，elseよりもこっちの処理の方が多いと思うので，こっちの処理をelseにする理由もないかぁ．
        {                                                                                                                               //☟1sで到達してほしい速さの割合がSpeedChangeRate(0~1)である．
            _conclusiveSpeed = Mathf.Lerp(nowSpeed_Horizon, _wantSpeed * moving.magnitude, Time.deltaTime * scri__obj.speedChangeRate); //  これのおかげで，スティックをいきなり全倒し(いきなり入力値が0→1)になったとしても，移動速度が0→1にならずに済んでいる．//  moving.magnitudeは，Keyboard操作の時に多分思い通りに動いてくれない．analog入力に対応させていないから．まぁとりあえず放置．
            _conclusiveSpeed = Mathf.Round(_conclusiveSpeed * 1000f) / 1000f;  //  偶数丸めして，小数第3位までのfloatにする処理．//  なんでこれをするのかは理解していない．    
        }
        else //  elseでは，nowSpeedとwantSpeedがほぼ一緒だったら，の処理．
        {
            _conclusiveSpeed = _wantSpeed;
        }
        #region Anim<Anim系>
        animationBlend = Mathf.Lerp(animationBlend, _wantSpeed, Time.deltaTime * scri__obj.speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
        #endregion
        Vector3 _movingDirection = new Vector3(moving.x, 0.0f, moving.y).normalized; //  多分，GamepadSchemeの時は既に，正規化されたベクトルを送ってくれてると思うけど，一応ここでも正規化．

        if (moving != Vector2.zero)
        {   //☟  if文の中で宣言されたローカルローカル変数なので，"_"は頭に2コつける．
            float __num = 0; //  計算の過程で内部的に使われる変数らしい．
            _wantRotation = Mathf.Atan2(_movingDirection.x, _movingDirection.z) * Mathf.Rad2Deg + brain__camera.transform.eulerAngles.y; //  最後の足し算はカメラの正面を向いてほしいのか？
            float __rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _wantRotation, ref __num, scri__obj.rotationSmoothTime);  //  これもMathf.Lerpのように急激な変化を防いで，段階的に振りむけてくれる．
                                                                                                                                         //  詳しくは，h_ttps://nekojara.city/unity-smooth-damp参照．
            transform.rotation = Quaternion.Euler(0.0f, __rotation, 0.0f); //  このおかげで，キャラは絶対に傾かない．
        }
        //☟  ここだけ，よく分かってない．
        Vector3 _conclusiveDirection = Quaternion.Euler(0.0f,_wantRotation, 0.0f) * Vector3.forward; //  当人(ここでは，P0_Man)の向き(右を向いているか，左を向いているか)に関わらず，当人の現在のまっすぐ前を得る計算．h_ttps://tsubakit1.hateblo.jp/entry/2014/08/02/030919 参照
        chara__con.Move(_conclusiveDirection.normalized * (_conclusiveSpeed * Time.deltaTime) + new Vector3(0.0f, -2.0f, 0.0f) * Time.deltaTime); //  これが本命の，移動を命令している箇所．引数は3次元ベクトル．詳しくは，h_ttps://nekojara.city/unity-input-system-character-controllerの下の方参照．
        #region Anim<Anim系>
        ani__mator.SetFloat(animIDSpeed, animationBlend);
        ani__mator.SetFloat(animIDMotionSpeed, moving.magnitude);
        #endregion
    }                                                                                                               //  ここは，0.0fじゃなくて，ちゃんとyの速さをいれよう．
    void AssignAnimationIDs()                //start でanimationの遷移条件に ID を"Assign"(付与)
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }
}
