/***********************************************
    *ProjectsName:          FlyppyBobTrr
    *Copyright(C) 2018 by   LuckCompany  
    *All rights reserved. 
    *FileName:              LLH_CoinData.cs 
    *Author:                凌LH 
    *Version:               1.0 
    *UnityVersion:          2017.3.1f1 
    *Date:                  2018-11-16 
    *Description:           项目名称
    *Function:               自定义，可以不写内容，可以删除该行
/***********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class LLH_CoinData
{
    private static int coinCount;

    private static int m_currCoin = 0;
    private static int CurrCoin
    {
        get { return m_currCoin; }
        //set { m_currCoin = value; }
        set
        {
            if (value < 0)
            {
                m_currCoin = 0;
            }
            else
            {
                m_currCoin = value;
            }
        }
    }

    private static int m_needCoin = 1;
    private static int NeedCoin
    {
        get { return m_needCoin; }
        //set { m_needCoin = value; }
        set
        {
            if (value < 0)
            {
                m_needCoin = 0;
            }
            else
            {
                m_needCoin = value;
            }
        }
    }

    public delegate void CoinNumberChangeHandle();
    public static CoinNumberChangeHandle CoinNumberChange = EnptyMethod;

    public static int GetCurrCoin()
    {
        return CurrCoin;
    }
    public static int GetNeedCoin()
    {
        return NeedCoin;
    }

    public static void AddCoin(int _coin)
    {
        CurrCoin += _coin;
        PlayerPrefs.SetInt("CurrCoin", CurrCoin);
        if (_coin > 0)
        {
            coinCount = PlayerPrefs.GetInt("CoinCount");
            coinCount += _coin;
            PlayerPrefs.SetInt("CoinCount", coinCount);
        }
        CoinNumberChange();
    }
    public static void SetCurrCoin(int _currcoin)
    {
        CurrCoin = _currcoin;
        PlayerPrefs.SetInt("CurrCoin", CurrCoin);
        CoinNumberChange();
    }
    public static void SetNeedCoin(int _needcoin)
    {
        NeedCoin = _needcoin;
        PlayerPrefs.SetInt("NeedCoin", NeedCoin);
        CoinNumberChange();
    }

    public static int GetCoinCount()
    {
        return coinCount;
    }

    public static void SetCoinCount(int _coinCount)
    {
        coinCount = _coinCount;
    }

    public static void Save()
    {
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.SetInt("CurrCoin", m_currCoin);
        PlayerPrefs.SetInt("NeedCoin", m_needCoin);
    }
    public static void EnptyMethod() { }
}

