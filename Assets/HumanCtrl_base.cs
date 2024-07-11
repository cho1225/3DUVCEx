#region ' Using '
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#endregion
public class HumanCtrl_base : MonoBehaviour
{
    #region ' Decl '
    public GameObject brain__camera;
    public ScOb_PlayerData scri__obj;
    public CharacterController chara__con;
    public Animator ani__mator;
    private Vector2 inputVec; // Vector2のnew

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
        inputVec = direction;
    }
    #endregion
    #region ' Start() '  St=>Anim系のみ; Up=>Move_Ctrl()のみ;
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
        float _tempDirection = 0.0f;
        float _tempSpeed;
        float _conclusiveSpeed;
        float _speedOffset = 0.1f;
        float _targetSpeed = scri__obj.moveSpeed; //  ローカル変数は，"_"を冒頭につけて，キャメル．
        float nowSpeed_Horizon = new Vector3(chara__con.velocity.x, 0.0f, chara__con.velocity.z).magnitude; //  こんな命名規則(aA_A)はないが，ローカル変数だから多少命名規則から外れてもいいかぁ．
        #endregion

        if (inputVec == Vector2.zero) _targetSpeed = 0.0f; // _targetSpeedには，既に，規定速度 (=moveSpeed) が代入済み．

        if ((nowSpeed_Horizon < _targetSpeed - _speedOffset) || (nowSpeed_Horizon > _targetSpeed + _speedOffset))   //  条件の反転(&&の使用)を考えたが，そうすれば条件を毎回2条件とも調べることになる(今は，"または"だから片方調べなくていい可能性アリ)．<=いやfalseだったらが反転時が速い．
        {  //  現在の速さ と _targetSpeed に差があるときの処理．                                                               //☟1sで到達してほしい速さの割合がSpeedChangeRate(0~1)である．
            _tempSpeed = Mathf.Lerp(nowSpeed_Horizon, _targetSpeed * inputVec.magnitude, Time.deltaTime * scri__obj.speedChangeRate); // 急激な速度の変化を防ぐ．
            _conclusiveSpeed = Mathf.Round(_tempSpeed * 1000f) / 1000f;  //  偶数丸めして，小数第3位までのfloatにする処理．なぜこれをするのかは不明．    
        }
        else
        {  // 現在の速さが"_targetSpeed"に近い場合．
            _conclusiveSpeed = _targetSpeed;
        }
        #region ' Anim '
        animationBlend = Mathf.Lerp(animationBlend, _targetSpeed, Time.deltaTime * scri__obj.speedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;
        #endregion

        Vector3 _inputDirection = new Vector3(inputVec.x, 0.0f, inputVec.y).normalized; //  Gamepad_Schemeのときは既に，正規化されたベクトルを送ってくれてるとは思うけど，一応ここでも正規化．
        if (inputVec != Vector2.zero)
        { 
            float __num = 0;
            _tempDirection = Mathf.Atan2(_inputDirection.x, _inputDirection.z) * Mathf.Rad2Deg + brain__camera.transform.eulerAngles.y;
            float __rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _tempDirection, ref __num, scri__obj.rotationSmoothTime);  //  Mathf.Lerpのように急激な変化を防いで，段階的に回転させてくれる．h_ttps://nekojara.city/unity-smooth-damp参照．
            transform.rotation = Quaternion.Euler(0.0f, __rotation, 0.0f); // 体の向きの決定．
        }

        Vector3 _conclusiveDirection = Quaternion.Euler(0.0f, _tempDirection, 0.0f) * Vector3.forward; // h_ttps://tsubakit1.hateblo.jp/entry/2014/08/02/030919 参照．
        chara__con.Move(_conclusiveDirection.normalized * _conclusiveSpeed * Time.deltaTime + new Vector3(0.0f, -2.0f, 0.0f) * Time.deltaTime); // 移動処理 h_ttps://nekojara.city/unity-input-system-character-controllerの下方参照．
        #region ' Anim '
        ani__mator.SetFloat(animIDSpeed, animationBlend);
        ani__mator.SetFloat(animIDMotionSpeed, inputVec.magnitude);
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
