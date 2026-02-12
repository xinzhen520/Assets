using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//角色基类
public class Character : MonoBehaviour
{
    protected UnityEvent<KeyCode> keyPressed = new UnityEvent<KeyCode>();
    protected UnityEvent<KeyCode> keyReleased = new UnityEvent<KeyCode>();
    //当所有移动按键松开时
    protected UnityEvent whenAllMoveKeyReleased = new UnityEvent();
    //战斗属性枚举
    public enum CombatAttribute
    {
        //奥秘
        mystery,
        //诡异
        weird,
        //弦心
        heartstring
    }
    //角色职业枚举
    public enum RoleProfession
    {
        //近卫
        sentinelTaverns,
        //突袭
        surpriseAttack,
        //支援
        support
    }
    //战斗属性
    public CombatAttribute combatAttribute = CombatAttribute.mystery;
    //角色职业
    public RoleProfession roleProfession = RoleProfession.sentinelTaverns;
    //体力,血量
    public float physicalStrength = 0;
    //精神力
    public float mentalStrength = 0;
    //护甲值
    public float armorValue = 0;
    //格挡条
    public float gridBar = 0;
    //最大移动速度
    public float maximumMovementSpeed = 0;
    //跳跃的速度改变量
    protected float jumpVelocity = 3.5f;
    public Rigidbody2D _rigidbody2D;
    public SpriteRenderer _spriteRenderer;
    //当前按下的所有移动按键
    protected List<KeyCode> allMovekeyCodes = new List<KeyCode>();
    protected bool hasBeginMove = false;
    protected bool isOnGround = true;
    //速度因子
    protected float speedFact = 0;
    //移动平滑度
    protected float moveSmoothness = 50;
    //移动方向
    protected Vector2 moveDirection = new Vector2(1, 0);
    //角色是否可转向
    protected bool canTurn = true;
    //是否处于闪避状态
    public bool InADodgingState = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        keyPressed.AddListener((KeyCode keyCode) =>
        {
            switch (keyCode)
            {
                case KeyCode.A:
                    PressA();
                    break;
                case KeyCode.D:
                    PressD();
                    break;
                case KeyCode.Space:
                    PressSpace();
                    break;
            }
        });
        keyReleased.AddListener((KeyCode keyCode) =>
        {
            switch (keyCode)
            {
                case KeyCode.A:
                    ReleaseA();
                    break;
                case KeyCode.D:
                    ReleaseD();
                    break;
            }
        });
        whenAllMoveKeyReleased.AddListener(() =>
        {
            if (hasBeginMove)
            {
                EndMove();
            }
        });
    }

    // Update is called once per frame
    protected void Update()
    {
        KeyCode pressedKey = GetAnyPressedKey();
        if (pressedKey != KeyCode.None)
        {
            keyPressed?.Invoke(pressedKey);
        }
        KeyCode releasedKey = GetAnyReleasedKey();
        if (releasedKey != KeyCode.None)
        {
            keyReleased?.Invoke(releasedKey);
        }
        if (allMovekeyCodes.Count > 0)
        {
            KeyCode key = allMovekeyCodes[allMovekeyCodes.Count - 1];
            moveDirection = GenJuAnJianHuoQuFangXiang(key);
        }
        if (moveDirection.x >= 0)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }
    protected void FixedUpdate()
    {
        _rigidbody2D.linearVelocityX = maximumMovementSpeed * speedFact * moveDirection.x;
    }
    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = true;
        }
    }
    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isOnGround = false;
        }
    }
    private void PressA()
    {
        allMovekeyCodes.Add(KeyCode.A);
        if (!hasBeginMove)
        {
            BeginMove();
        }
    }

    private void PressD()
    {
        allMovekeyCodes.Add(KeyCode.D);
        if (!hasBeginMove)
        {
            BeginMove();
        }
    }

    private void ReleaseA()
    {
        allMovekeyCodes.Remove(KeyCode.A);
        if (allMovekeyCodes.Count == 0)
        {
            whenAllMoveKeyReleased?.Invoke();
        }
    }

    private void ReleaseD()
    {
        allMovekeyCodes.Remove(KeyCode.D);
        if (allMovekeyCodes.Count == 0)
        {
            whenAllMoveKeyReleased?.Invoke();
        }
    }
    //点击跳跃
    private void PressSpace()
    {
        if (isOnGround)
        {
            _rigidbody2D.linearVelocityY += jumpVelocity;
        }
    }

    //闪避
    protected void Dodge()
    {

    }
    //开始起步
    private void BeginMove()
    {
        hasBeginMove = true;
        CancelInvoke(nameof(JiaSu));
        CancelInvoke(nameof(JianSu));
        InvokeRepeating(nameof(JiaSu), 0.01f, 0.01f);
    }
    //停止移动
    private void EndMove()
    {
        hasBeginMove = false;
        CancelInvoke(nameof(JiaSu));
        CancelInvoke(nameof(JianSu));
        InvokeRepeating(nameof(JianSu), 0.01f, 0.01f);
    }
    //加速
    private void JiaSu()
    {
        float v = 1.0f / (moveSmoothness + 1.0f);
        speedFact += v;
        if (speedFact >= 1.0f)
        {
            speedFact = 1.0f;
            CancelInvoke(nameof(JiaSu));
        }
    }
    //减速
    private void JianSu()
    {
        float v = 1.0f / (moveSmoothness + 1.0f);
        speedFact -= v;
        if (speedFact <= 0)
        {
            speedFact = 0;
            CancelInvoke(nameof(JianSu));
        }
    }
    //根据按键获取方向
    private Vector2 GenJuAnJianHuoQuFangXiang(KeyCode key)
    {
        switch (key)
        {
            case KeyCode.A:
                return new Vector2(-1, 0);
            case KeyCode.D:
                return new Vector2(1, 0);
        }
        return Vector2.zero;
    }
    public static IEnumerable DelayDo(float delaySecond,UnityAction call)
    {
        yield return new WaitForSeconds(delaySecond);
        call?.Invoke();
    }
    public KeyCode GetAnyPressedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }
    public KeyCode GetAnyReleasedKey()
    {
        foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyUp(key))
            {
                return key;
            }
        }
        return KeyCode.None;
    }
}

