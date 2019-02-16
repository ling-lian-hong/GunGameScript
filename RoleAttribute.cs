using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public  class RoleAttribute  {
    [Header("角色名字")]
    public string RoleName;
    //[System.Serializable]
    //public struct Attribute
    //{
    //    [Header("属性名字")]
    //    public string attributeName;
    //    [Header("属性值")]
    //    public float attributeValue;
    //}
    //[Header("属性")]
    //public Attribute[] attribute;
    [Header("生命")]
    public float LifeValue;
    [Header("攻击力")]
    public float AttackValue;
    [Header("射速")]
    public float ShootSpeedValue;
    [Header("敏捷")]
    public float AgileValue;
       
    [Header("角色对应的场景索引")]
    public int SceneIndex;
}
