using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using RenderHeads.Media.AVProVideo;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitSceneMG : MonoBehaviour {
    public Camera waitSceneCam;
    [Header("射线检测的层"), SerializeField]
    private LayerMask layermask;
    [Header("射线检测距离"), SerializeField]
    public float castDis;

    [Header("没选中时的material")]
    public Material[] noSelectMaterials;
    [Header("选中时的material")]
    public Material[] selectMaterials;
    [Header("角色对应的SelectEffet")]
    public SelectEffet[] RoleSelectEffetScript;
    [Header("确认按钮的ConfirmUI")]
    public ConfirmUI confirmPlane;
    [Header("选中角色的名字")]
    public Renderer nameRenderer;
    [Header("选中角色的信息")]
    public Renderer roleDataRenderer;

    //[Header("当前选中的ID"),SerializeField]
    private int currSelectRoleID;
    // [Header("之前选中的ID"), SerializeField]
    private int oldSelectRoleID;
    private bool leaveConfirmPlane;

    #region 固定字符串
    const string Role_1_Name = "Role_1";
    const string Role_2_Name = "Role_2";
    const string Role_3_Name = "Role_3";
    const string Role_4_Name = "Role_4";
    const string Role_5_Name = "Role_5";
    const string Role_6_Name = "Role_6";
    const string Role_7_Name = "Role_7";
    const string Role_8_Name = "Role_8";
    const string ConfirmUIName = "Plane (1)";
    #endregion

    #region 人物属性
    [Header("-----------人物属性------------")]
    public RoleAttribute[] roleAttribute;
    private Dictionary<int, RoleAttribute> roleAttributeDis = new Dictionary<int, RoleAttribute>();

    public AttributeUI attributeUI;
    #endregion
    //---------------------------2.15-----
    /// <summary>
    /// WaitScene 所有的游戏状态
    /// </summary>
    public enum WaitSceneState {
        Standby,
        Choosing
    }
    public WaitSceneState waitSceneState = WaitSceneState.Standby;

    // 是否倒计时
    private bool isAutoCountDown= true;

    //一秒时间
    private float oneTime;

    //倒计时
    private float timer = 0;

    //事件倒计时
    private float eventTime;

    //倒计时时间集合
    private float[] eventTimes = new float[2] { 7, 5};

    [Header("教学视频时长")]
    public float teachingVideoTime;

    [Header("待机视频时长")]
    public float StandbyVideoTime;

    //倒计时索引
    private int timeIndex = 0;

    //倒计时该执行的事件委托
    private Action Time2Do;

    [Header("选择人物阶段的物体")]
    public GameObject SelectObj;

    [Header("视频播放器")]
    public MediaPlayer mediaPlayer;

    [Header("币数")]
    public int coinSum = 0;
    //-------------------异步加载----
    int m_targetValue = 100;
    int m_oldValue;
    int m_currValue;
    public Text loadingText;
    private float loadingSpeed = 1;

    private float targetValue;

    public AsyncOperation operation;

    public float loadingValue;

    private void OnEnable()
    {
        Time2Do += EmptyMethod;
        eventTimes[0] = StandbyVideoTime;
        eventTimes[1] = teachingVideoTime;
    }

    private void OnDisable()
    {
        Time2Do -= EmptyMethod;
    }

    void Start() {
        currSelectRoleID = 1;
        oldSelectRoleID = 1;
        leaveConfirmPlane = false;
        if (RoleSelectEffetScript[currSelectRoleID - 1] != null)
        {
            RoleSelectEffetScript[currSelectRoleID - 1].TransformChange(true);
            nameRenderer.material.SetFloat("_weizhi", currSelectRoleID);
            roleDataRenderer.material.SetFloat("_weizhi", currSelectRoleID);
        }
        oneTime = Time.time + 1;
        eventTime = eventTimes[0];
        if (coinSum>0)
        {
            ToChooseState();
        }
        //waitSceneCam.clearFlags = CameraClearFlags.Color;
    }

    void Update() {
        CursorBehavior();

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {

        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            PayACoin();
        }

        if (isAutoCountDown)
        {
            Timing();
        }
       
    }

    /// <summary>
    /// 准心行为
    /// </summary>
    private void CursorBehavior()
    {
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        RaycastHit hit;
        Ray ray = waitSceneCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, castDis, layermask))
        {

            if (hit.transform != null)
            {
                //   Debug.Log(hit.transform.name);
                switch (hit.transform.name)
                {
                    case Role_1_Name:
                        currSelectRoleID = 1;
                        SelectRole();
                        LeaveConfirmUI();

                        break;
                    case Role_2_Name:
                        currSelectRoleID = 2;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_3_Name:
                        currSelectRoleID = 3;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_4_Name:
                        currSelectRoleID = 4;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_5_Name:
                        currSelectRoleID = 5;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_6_Name:
                        currSelectRoleID = 6;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_7_Name:
                        currSelectRoleID = 7;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    case Role_8_Name:
                        currSelectRoleID = 8;
                        SelectRole();
                        LeaveConfirmUI();
                        break;
                    //case ConfirmUIName:
                    // Confirm();
                    // break;

                    default:
                        break;
                }

            }

            // }
        }
        else
        {
            //LeaveConfirmUI();
        }
    }

    /// <summary>
    /// 选中角色UI 行为
    /// </summary>
    private void SelectRole() {
        if (currSelectRoleID == oldSelectRoleID)
            return;
        if (RoleSelectEffetScript[currSelectRoleID - 1] != null)
        {
            RoleSelectEffetScript[oldSelectRoleID - 1].TransformChange(false);
            RoleSelectEffetScript[oldSelectRoleID - 1].ChangeMaterial(false);
            RoleSelectEffetScript[oldSelectRoleID - 1].ChangeModel(false);
        }
        if (RoleSelectEffetScript[currSelectRoleID - 1] != null)
        {

            RoleSelectEffetScript[currSelectRoleID - 1].TransformChange(true);
            RoleSelectEffetScript[currSelectRoleID - 1].ChangeMaterial(true);
            RoleSelectEffetScript[currSelectRoleID - 1].ChangeModel(true);
            nameRenderer.material.SetFloat("_weizhi", currSelectRoleID);
            roleDataRenderer.material.SetFloat("_weizhi", currSelectRoleID);
        }
        oldSelectRoleID = currSelectRoleID;
    }

    /// <summary>
    /// 确定(弃用)
    /// </summary>
    private void Confirm() {

        if (!leaveConfirmPlane)
        {
            if (confirmPlane != null)
            {
                confirmPlane.IsStayOn(true);
            }
        }

        leaveConfirmPlane = true;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Debug.Log("射击");
            if (confirmPlane != null)
            {
                RoleSelectEffetScript[currSelectRoleID - 1].ChangeMaterial(true);

                // confirmPlane.ClickConfirm();
            }
        }
    }

    /// <summary>
    /// 离开确认按键（弃用）
    /// </summary>
    private void LeaveConfirmUI() {
        if (leaveConfirmPlane)
        {
            leaveConfirmPlane = false;
            if (confirmPlane != null)
            {
                confirmPlane.IsStayOn(false);
            }

        }
    }

    /// <summary>
    /// 设置人物对应的属性
    /// </summary>
    private void SetRoleAttribute(int _roleID) {
        if (roleAttributeDis.ContainsKey(_roleID))
        {
            attributeUI.SetValue(roleAttributeDis[_roleID]);
        }

        //roleAttribute[0].attribute[0].
    }

    /// <summary>
    /// 计时
    /// </summary>
    private void Timing() {
        if (Time.time >= oneTime)
        {
            oneTime = Time.time + 1;
            timer += 1;
            if (timer >= eventTime)
            {
                timer = 0;
                DoSomething();
            }
            Debug.Log(timer);
        }
    }

    /// <summary>
    /// 倒计时到了该执行的方法
    /// </summary>
    private void DoSomething() {
        switch (waitSceneState)
        {
            case WaitSceneState.Standby:
               // isAutoCountDown = false;
                timeIndex = (timeIndex + 1) % eventTimes.Length;
                eventTime = eventTimes[timeIndex];
                SetTime2Do(timeIndex);
                break;
            case WaitSceneState.Choosing:
                isAutoCountDown = false;
                Time2Do = AutoChoose;
                break;
            default:
                break;
        }
        Time2Do();
    }

    /// <summary>
    /// 待机视频阶段的方法
    /// </summary>
    private void StandbyMethod(){
        if (mediaPlayer!=null)
        {
            mediaPlayer.CloseVideo();
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Standby Video.mp4");
        }     
        Debug.Log("StandbyMethod");
    }

    /// <summary>
    /// 教学视频阶段的方法
    /// </summary>
    private void TeachingMethod() {
        if (mediaPlayer != null)
        {
            mediaPlayer.CloseVideo();
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Teach Video.mp4");

        }
        /////
        Debug.Log("TeachingMethod");
    }

    /// <summary>
    /// 设置倒计时到了该执行的方法
    /// </summary>
    /// <param name="_index"></param>
    private void SetTime2Do(int _index) {
        switch (_index)
        {
            case 0:
                Time2Do = StandbyMethod;
                break;
            case 1:
                Time2Do = TeachingMethod;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 自动选择人物
    /// </summary>
    private void AutoChoose() {
        Debug.Log("自动选择");
    }

    /// <summary>
    /// 空方法
    /// </summary>
    private void EmptyMethod() { }

    /// <summary>
    /// 投一个币
    /// </summary>
    private void PayACoin() {
  
        if (coinSum==0)
        {
            ToChooseState();
        }
        coinSum++;
    }

    /// <summary>
    /// 变为选择人物阶段
    /// </summary>
    private void ToChooseState() {
        waitSceneState = WaitSceneState.Choosing;
        oneTime = Time.time + 1;
        timer =0;
        eventTime = 15;
        SelectObj.SetActive(true);
        if (mediaPlayer != null)
        {
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Standby Video.mp4");
        }
    }

    private void LoadScene() {
        StartCoroutine(AsyncLoading());
    }

    IEnumerator AsyncLoading()
    {
        yield return new WaitForSeconds(1);
        operation = SceneManager.LoadSceneAsync(0);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;

        yield return operation;
    }

    private void LoadSceneMethod() {
        if (operation == null)
            return;
        targetValue = operation.progress;

        if (operation.progress >= 0.9f)
        {
            //operation.progress的值最大为0.9
            targetValue = 1.0f;
        }

      
        if (targetValue != loadingValue)
        {
            loadingValue = Mathf.Lerp(loadingValue, targetValue, Time.deltaTime * loadingSpeed);
            if (Mathf.Abs(loadingValue - targetValue) < 0.01f)
            {
                loadingValue = targetValue;
            }
            m_currValue = (int)(loadingValue * 100);
            if (m_currValue != m_oldValue)
            {
                m_oldValue = m_currValue;
                loadingText.text = m_currValue.ToString() + "%";
            }

        }

        if ((int)(loadingValue * 100) == m_targetValue)
        {
            m_targetValue = -100;

            //允许异步加载完毕后自动切换场景
            operation.allowSceneActivation = true;

        }
    }
}
