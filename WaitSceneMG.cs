using RenderHeads.Media.AVProVideo;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
public class WaitSceneMG : MonoBehaviour {
    [Header("当前场景主摄像机"),SerializeField]
    private Camera waitSceneCam;
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
    public RoleAttributeData roleAttributeData;
    public List<RoleAttribute> roleAttributeList = new List<RoleAttribute>();
    public AttributeUI attributeUI;
    #endregion
    //---------------------------2.15-----
    /// <summary>
    /// WaitScene 所有的游戏状态
    /// </summary>
    public enum WaitSceneState
    {
        Standby,
        Choosing
    }
    public WaitSceneState waitSceneState = WaitSceneState.Standby;

    //一位数 个位数材质
    public Material singleM;
    //两位数 个位数材质
    public Material doubleSingleM;
    //两位数 个位数材质
    public Material doubleTenM;

    public GameObject singleMObj;

    public GameObject doubleMObj;

    // 是否倒计时
    private bool isAutoCountDown = true;

    //一秒时间
    private float oneTime;

    //倒计时
    private float timer = 0;

    //事件倒计时
    private float eventTime;

    //倒计时时间集合
    private float[] eventTimes = new float[2] { 7, 5 };

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
    //---------------------------------
    [Header("眨眼屏幕特效"),SerializeField]
    private CameraFilterPack_TV_WideScreenHorizontal cameraEffect;

    private bool isConfirm = false;
    //-------------------------------------
    private void OnEnable()
    {
        Time2Do += EmptyMethod;
        eventTimes[0] = StandbyVideoTime;
        eventTimes[1] = teachingVideoTime;
     //   Debug.Log(GameObject.Find("MainCamera"));
        waitSceneCam =GameObject.Find("MainCamera").GetComponent<Camera>();
        if (waitSceneCam.GetComponent<CameraFilterPack_TV_WideScreenHorizontal>()==null)
        {
            waitSceneCam.gameObject.AddComponent<CameraFilterPack_TV_WideScreenHorizontal>();
        }
        cameraEffect = waitSceneCam.GetComponent<CameraFilterPack_TV_WideScreenHorizontal>();
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

        RoleAttributeDataInit();

        oneTime = Time.time + 1;
        eventTime = eventTimes[0];
        if (coinSum > 0)
        {
            ToChooseState();
        }
    }

    void Update() {
        if (!isConfirm)
        {
            CursorBehavior();
        }
      

        MyInput();

        if (isAutoCountDown)
        {
            Timing();
        }

        LoadSceneMethod();
    }

    [ContextMenu("配置信息")]
    public void RoleAttributeDataInit() {
        if (roleAttributeData!=null)
        {
            roleAttributeList.Clear();
            for (int i = 0; i < roleAttributeData.roleAttributes.Length; i++)
            {
                roleAttributeList.Add(roleAttributeData.roleAttributes[i]);
            }
           
        }
        else
        {
            Debug.Log("roleAttributeData 未赋值");
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
                      
                      //  LeaveConfirmUI();
                        break;
                    case Role_2_Name:
                        currSelectRoleID = 2;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    case Role_3_Name:
                        currSelectRoleID = 3;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    case Role_4_Name:
                        currSelectRoleID = 4;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    case Role_5_Name:
                        currSelectRoleID = 5;
                        SelectRole();
                        //LeaveConfirmUI();
                        break;
                    case Role_6_Name:
                        currSelectRoleID = 6;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    case Role_7_Name:
                        currSelectRoleID = 7;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    case Role_8_Name:
                        currSelectRoleID = 8;
                        SelectRole();
                      //  LeaveConfirmUI();
                        break;
                    //case ConfirmUIName:
                       // Confirm();
                       // break;

                    default:
                        break;
                }

                #region 键鼠交互  确认
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (roleAttributeList.Count >= currSelectRoleID && roleAttributeList.Count != 0)
                    {
                        Confirm(roleAttributeList[currSelectRoleID - 1].SceneIndex);
                        RoleSelectEffetScript[currSelectRoleID - 1].SetConfirmAni();
                    }
                   
                }
                #endregion

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
        if (RoleSelectEffetScript[currSelectRoleID - 1]!=null)
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
            SetRoleAttribute(currSelectRoleID);
        }
      
        oldSelectRoleID= currSelectRoleID;
    }

    #region （弃用）
    /// <summary>
    /// 确定(弃用)
    /// </summary>
    //private void Confirm()
    //{

    //    if (!leaveConfirmPlane)
    //    {
    //        if (confirmPlane != null)
    //        {
    //            confirmPlane.IsStayOn(true);
    //        }
    //    }

    //    leaveConfirmPlane = true;

    //    if (Input.GetKeyDown(KeyCode.Mouse0))
    //    {
    //        // Debug.Log("射击");
    //        if (confirmPlane != null)
    //        {
    //            RoleSelectEffetScript[currSelectRoleID - 1].ChangeMaterial(true);

    //            // confirmPlane.ClickConfirm();
    //        }
    //    }
    //}

    /// <summary>
    /// 离开确认按键（弃用）
    /// </summary>
    //private void LeaveConfirmUI()
    //{
    //    if (leaveConfirmPlane)
    //    {
    //        leaveConfirmPlane = false;
    //        if (confirmPlane != null)
    //        {
    //            confirmPlane.IsStayOn(false);
    //        }

    //    }
    //}
    #endregion

    /// <summary>
    /// 设置人物对应的属性
    /// </summary>
    private void SetRoleAttribute(int _roleID)
    {
        if (attributeUI == null)
            return;
        if (roleAttributeList.Count >= _roleID && roleAttributeList.Count != 0)
        {

            attributeUI.SetValue(roleAttributeList[_roleID - 1]);
        }
        else
        {
            attributeUI.Hide(true);
        }
    }

    /// <summary>
    /// 计时
    /// </summary>
    private void Timing()
    {
        if (Time.time >= oneTime)
        {
            oneTime = Time.time + 1;
            timer += 1;
            if (waitSceneState==WaitSceneState.Choosing)
            {
                if (timer > 0 && timer < 10)
                {
                    singleMObj.SetActive(true);
                    doubleMObj.SetActive(false);

                    singleM.SetFloat("_fanda", 0f);
                    singleM.DOFloat(1, "_fanda", 1);
                    singleM.SetFloat("_daoshu_sil", timer);
                }
                else if (timer >= 10 && timer < 100)
                {
                    singleMObj.SetActive(false);
                    doubleMObj.SetActive(true);

                    doubleSingleM.SetFloat("_fanda", 0f);
                    doubleSingleM.DOFloat(1f, "_fanda", 1f);
                    doubleSingleM.SetFloat("_daoshu_sil", timer % 10);

                    doubleTenM.SetFloat("_fanda", 0f);
                    doubleTenM.DOFloat(1f, "_fanda", 1);
                    doubleTenM.SetFloat("_daoshu_sil", timer / 10);
                }
            }
          
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
    private void DoSomething()
    {
        switch (waitSceneState)
        {
            case WaitSceneState.Standby:
                // isAutoCountDown = false;
                timeIndex = (timeIndex + 1) % eventTimes.Length;
                eventTime = eventTimes[timeIndex];
                TransitionEffect();
                SetTime2Do(timeIndex);
                break;
            case WaitSceneState.Choosing:
                isAutoCountDown = false;
                Time2Do = AutoChoose;
                Time2Do();
                break;
            default:
                break;
        }
     
    }

    /// <summary>
    /// 待机视频阶段的方法
    /// </summary>
    private void StandbyMethod()
    {
        if (mediaPlayer != null)
        {
            mediaPlayer.CloseVideo();
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Standby Video.mp4");
        }
        Debug.Log("StandbyMethod");
    }

    /// <summary>
    /// 教学视频阶段的方法
    /// </summary>
    private void TeachingMethod()
    {
        if (mediaPlayer != null)
        {
            mediaPlayer.CloseVideo();
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Teach Video.mp4");

        }
        /////
        Debug.Log("TeachingMethod");
    }

    /// <summary>
    /// 选择人物阶段方法
    /// </summary>
    private void ChooseMethod() {
        if (mediaPlayer != null)
        {
            mediaPlayer.CloseVideo();
            mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Standby Video.mp4");
        }
        isAutoCountDown = true;
        oneTime = Time.time + 1;
        timer = 0;
        eventTime = 15;
        singleM.SetFloat("_fanda", 1f);
       // singleM.DOFloat(1, "_fanda", 1);
        singleM.SetFloat("_daoshu_sil", timer);
        SelectObj.SetActive(true);
    }

    /// <summary>
    /// 设置倒计时到了该执行的方法
    /// </summary>
    /// <param name="_index"></param>
    private void SetTime2Do(int _index)
    {
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
    private void AutoChoose()
    {
        Debug.Log("自动选择");
        int _roleIndex = UnityEngine.Random.Range(0, roleAttributeList.Count);
        RoleSelectEffetScript[_roleIndex].SetConfirmAni();
        Confirm(roleAttributeList[_roleIndex].SceneIndex);
    }

    /// <summary>
    /// 空方法
    /// </summary>
    private void EmptyMethod() { }

    /// <summary>
    /// 投一个币
    /// </summary>
    private void PayACoin()
    {
      //  coinSum = LLH_CoinData.GetCurrCoin();
        if (coinSum == 0)
        {
            ToChooseState();
        }
        coinSum++;
    }

    /// <summary>
    /// 变为选择人物阶段
    /// </summary>
    private void ToChooseState()
    {
        waitSceneState = WaitSceneState.Choosing;
        isAutoCountDown = false;
        Time2Do = ChooseMethod;
        TransitionEffect();
        //if (mediaPlayer != null)
        //{
        //    mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.RelativeToStreamingAssetsFolder, "Video/Standby Video.mp4");
        //}
    }

    /// <summary>
    /// 加载索引对应的场景
    /// </summary>
    /// <param name="_sceneIndex"></param>
    private void LoadScene(int _sceneIndex)
    {
        StartCoroutine(AsyncLoading(_sceneIndex));
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <param name="_sceneIndex"></param>
    /// <returns></returns>
    IEnumerator AsyncLoading(int _sceneIndex)
    {
      //  yield return new WaitForSeconds(1);
        operation = SceneManager.LoadSceneAsync(_sceneIndex);
        //阻止当加载完成自动切换
        operation.allowSceneActivation = false;

        yield return operation;
    }

    /// <summary>
    /// 加载方法
    /// </summary>
    private void LoadSceneMethod()
    {
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
               // loadingText.text = m_currValue.ToString() + "%";
            }

        }

        if ((int)(loadingValue * 100) == m_targetValue)
        {
            m_targetValue = -100;

            if (waitSceneCam.GetComponent<CameraFilterPack_TV_WideScreenHorizontal>() != null)
            {
                Destroy(waitSceneCam.gameObject.GetComponent<CameraFilterPack_TV_WideScreenHorizontal>());
            }
            //允许异步加载完毕后自动切换场景
            operation.allowSceneActivation = true;

        }
    }

    #region 场景镜头过渡效果
    private void TransitionEffect()
    {
        if (cameraEffect == null)
            return;
        DOTween.To(() => cameraEffect.Smooth, x => cameraEffect.Smooth = x, 0.4f, 0.5f).OnComplete(CameraEffect_EyeOpen);
        DOTween.To(() => cameraEffect.Size, x => cameraEffect.Size = x, 0f, 0.5f).OnComplete(CameraEffect_EyeOpen);
    }


    private void CameraEffect_EyeOpen()
    {
        Time2Do();
        DOTween.To(() => cameraEffect.Size, x => cameraEffect.Size = x, 0.8f,0.5f);
        DOTween.To(() => cameraEffect.Size, x => cameraEffect.Smooth = x, 0f, 0.5f);
    }

    #endregion

    /// <summary>
    /// 交互 输入
    /// </summary>
    private void MyInput() {
        #region 键鼠方法

        //枪射击
        //if (Input.GetKeyDown(KeyCode.Mouse0))
        //{
        //    if (waitSceneState == WaitSceneState.Choosing)
        //    {
        //        Confirm();
        //    }
        //}

        //投币
        if (Input.GetKeyDown(KeyCode.C))
        {
            PayACoin();
        }

        //模拟加载场景
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    LoadScene(1);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    LoadScene(2);
        //}
        //if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    LoadScene(3);
        //}
        #endregion

        #region 手柄或者其他输入

        #endregion
    }

    /// <summary>
    /// 确认
    /// </summary>
    private void Confirm(int _sceneIndex) {
        if (isConfirm)
            return;
        isConfirm = true;
        isAutoCountDown = false;
        SelectObj.SetActive(false);
        LoadScene(_sceneIndex);
    }

}
