using UnityEngine;

public class Character : MonoBehaviour
{
    //战斗属性枚举
    enum CombatAttribute
    {
        //奥秘
        mystery,
        //诡异
        weird,
        //弦心
        heartstring
    }
    //角色职业枚举
    enum RoleProfession
    {
        //近卫
        sentinelTaverns,
        //突袭
        surpriseAttack,
        //支援
        support
    }
    //战斗属性
    CombatAttribute combatAttribute = CombatAttribute.mystery;
    //角色职业
    RoleProfession roleProfession = RoleProfession.sentinelTaverns;
    //体力
    public float physicalStrength = 0;
    //精神力
    public float mentalStrength = 0;
    //护甲值
    public float armorValue = 0;
    //格挡条
    public float gridBar = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}