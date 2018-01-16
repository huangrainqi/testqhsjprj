using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

public enum EarthState
{
    None,
    Earth,
    China,
    Tower,
}

public class EarthManager {

    static EarthManager _instance;

    public static EarthManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new EarthManager();
            }
            return _instance;
        }
    }
    public const float SunTenMinSpeed = 0.25f;
    /// <summary>
    /// 物体挂点
    /// </summary>
    public GameObject m_hitParent = null;
    /// <summary>
    /// 地球
    /// </summary>
    public GameObject m_earthPrefab = null;
    /// <summary>
    /// 中国
    /// </summary>
    public GameObject m_chinaPrefab = null;
    /// <summary>
    /// 塔
    /// </summary>
    public GameObject m_tower = null;
    public Camera m_camera = null;
    /// <summary>
    /// 标签动画
    /// </summary>
    public Animator m_biaojiAni = null;
    /// <summary>
    /// 复制地球
    /// </summary>
    public GameObject m_earthCopy = null;
    public GameObject m_cameraEarth = null;
    public bool isSeeLocation = false;
    public Transform m_sunEarth = null;
    public UnityARGeneratePlane m_generatePlanes = null;
    GameObject m_light = null;
    public List<Transform> dotList = new List<Transform>();
    // Use this for initialization
    public void Init() {
        m_earthPrefab = GameObject.Instantiate(Resources.Load("Map/Earth_16K_linear") as GameObject);
        //m_chinaPrefab = GameObject.Instantiate(Resources.Load("Map/zhongguo") as GameObject);
        m_earthCopy = GameObject.Instantiate(Resources.Load("Map/EarthControl") as GameObject);
		m_cameraEarth = m_earthCopy.transform.Find("Main_Camera1").gameObject;
        m_earthCopy.SetActive(false);
        m_earthPrefab.gameObject.SetActive(false);
        //m_chinaPrefab.gameObject.SetActive(false);
        m_hitParent = GameObject.Find("HitCubeParent");
        UGUIManager.GetInstance().GetUGUILogic<UI_EarthPanel>().InitPanel();
        m_camera = GameObject.Find("CameraParent/Main Camera").GetComponent<Camera>();
        //m_biaojiAni = m_earthPrefab.GetComponentInChildren<Animator>();
        //m_biaojiAni.SetBool("biaoji", false);
        //EventCenter.Regist(UI_EVENT.SUN_ROTATE.ToString(), RotateSun);
        //m_biaoji.gameObject.SetActive(false);
        m_sunEarth = m_earthPrefab.transform.Find("Sun");
     
        for (int i=0; i<3; i++)
        {
            string dot = "dot" + i.ToString();
            Transform a = m_earthPrefab.transform.Find(dot);
            dotList.Add(a);
        }
        m_light = GameObject.Find("Directional light");
        m_generatePlanes = GameObject.Find("GeneratePlanes").GetComponent<UnityARGeneratePlane>();
		EventCenter.Regist(UI_EVENT.SHOW_EARTHTWO.ToString(), ShowEarthTwo);
		//EventCenter.Regist(UI_EVENT.SHOW_EARTHDISAPPEARFINISH.ToString(), ShowChina);
    }

    public void ShowPanel(bool isShow)
    {
        UGUIManager.GetInstance().GetUGUILogic<UI_EarthPanel>().ShowUI(isShow);
    }

    EarthState m_earthState;

    public void Update()
    {
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    EarthType = EarthState.China;
        //}
        if(Input.GetKeyDown(KeyCode.A))
        {
            EarthType = EarthState.Tower;
        }
        //if(m_camera != null && m_earthPrefab.activeSelf == true)
        //{
        //    //if (m_earthPrefab.transform.up.x < 0)
        //    //{
        //        float dot = Vector3.Dot(m_camera.transform.forward, m_earthPrefab.transform.right);
        //        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //        //Debug.LogError(angle);

        //        //Debug.LogError(m_earthPrefab.transform.up.x);
        //        if (0 < angle && angle<40)
        //        {

        //            if (isBiaoji == false)
        //            {
        //                isBiaoji = true;
        //                //m_biaoji.gameObject.SetActive(true);
        //                //m_biaojiAni.SetBool("biaoji", true);
        //                //Debug.LogError(true);
        //            }


        //        }
        //        else
        //        {
        //            if (isBiaoji == true)
        //            {
        //                isBiaoji = false;

        //                //m_biaojiAni.SetBool("biaoji", false);
        //                //m_biaoji.gameObject.SetActive(false);
        //                //Debug.LogError(false);
        //            }
        //        }

        //    //}
        //}
        //UGUIManager.GetInstance().GetUGUILogic<UI_EarthPanel>().UpdatePanel();
    }

    EarthState m_backType;

    public EarthState EarthType
    {
        get
        {
            return m_earthState;
        }
        set
        {
            m_earthState = value;
            switch (m_earthState)
            {
                case EarthState.None:
                    CloseAllPrefab();
                    break;
			case EarthState.Earth:
                    CloseAllPrefab();

                    m_cameraEarth.transform.SetParent (m_earthPrefab.transform);
				    Utility.TransformReset (m_cameraEarth.transform);
					RotationScale rotation = m_earthPrefab.GetComponent<RotationScale>();
					if (rotation == null)
						rotation = m_earthPrefab.AddComponent<RotationScale>();
				    m_cameraEarth.transform.localPosition = new Vector3 (0, 0, -10);
				    Utility.SetLayer (m_earthPrefab, 4);
				    m_camera.cullingMask = ~(1 << 4);
				        //Utility.SetParentAndReset(m_cameraEarth, m_earthPrefab.gameObject);
				    m_earthCopy.transform.SetParent (m_hitParent.transform, false);
                    

				    var mfd = Utility.GetScript<FollowDirection> (m_cameraEarth);
				    mfd.fLookfrom = UGUIManager.GetInstance ().SceneCamera.transform;
				    mfd.fLookto = m_earthPrefab.transform;
				    mfd.lookFrom = m_cameraEarth.transform;
				    mfd.lookTo = m_earthPrefab.transform;
				    mfd.Calibrate ();
#if UNITY_EDITOR
                    m_earthCopy.transform.localPosition = new Vector3(0, 0, 700);
#endif
                    m_light.SetActive(false);
                    EarthControl earthControl = m_earthPrefab.GetComponent<EarthControl>();
                    if (earthControl == null)
                        earthControl = m_earthPrefab.AddComponent<EarthControl>();


                    if (m_hitParent != null)
                    {
                        earthControl.m_HitTransform = m_hitParent.transform;
                        m_earthPrefab.transform.SetParent(m_hitParent.transform, false);

#if UNITY_EDITOR
                        m_earthPrefab.transform.localPosition = new Vector3(0, 0, 700);
#endif
                    }
                    m_earthPrefab.gameObject.SetActive(true);
                    m_earthCopy.SetActive(true);
                    System.DateTime now = System.DateTime.Now;
                    Vector3 vec = new Vector3(0, (now.Hour - 18) * SunTenMinSpeed * 60 + now.Minute * SunTenMinSpeed-120, 0);
                    Debug.LogError(" vec  ////" + vec);
                    m_sunEarth.transform.rotation = Quaternion.Euler(vec);
                    //m_sunEarth.transform.rotation.eulerAngles
                    //m_location.gameObject.SetActive(false);
                    //LoginManager.Instance.ShowUI(true);
	
                    EventCenter.Dispatch(UI_EVENT.SHOW_EARTH.ToString());
                    LoginManager.Instance.TouchLogin();
                    EarthManager.Instance.m_generatePlanes.ClosePlane();

                    //UGUIManager.GetInstance().GetUGUILogic<UI_EarthPanel>().ShowButton();
                    break;
			    case EarthState.China:
                    //if (m_backType == EarthState.None)
                    //{
                    //    EventArguments a = new EventArguments();
                    //    ShowChina(a);
                    //}
                    //else if (m_backType == EarthState.Earth)
                    //{

                    //    m_earthCopy.SetActive(true);
                    //    m_cameraEarth.SetActive(true);
                    //    m_camera.cullingMask = ~(1 << 4);
                    //    Utility.SetLayer(m_earthPrefab, 4);
                    //}
                    break;
                case EarthState.Tower:
                    //UGUIManager.GetInstance ().GetUGUILogic <UI_EarthPanel> ().ShowUI (false);
                    if (m_tower == null)
                    {
                        CloseAllPrefab();
                        m_tower = GameObject.Instantiate(Resources.Load("Tower/TowerNew") as GameObject);
                        if (m_tower != null)
                        {
                            Utility.SetParentAndReset(m_tower, m_hitParent);
                            UnityARHitTestExample1 exam = m_tower.GetComponent<UnityARHitTestExample1>();
                            if (exam != null)
                            {
                                exam.m_HitTransform = m_hitParent.transform;
                            }
                            CharacterManager.Instance.towerScale = m_tower.transform.parent.localScale.x;
                            Debug.LogError(CharacterManager.Instance.towerScale);
						    m_tower.transform.localPosition = new Vector3(0, 5, 100);

#if UNITY_EDITOR
                            m_tower.transform.localPosition = new Vector3(0, 0, 960);
#endif
                        }
                    }



                    break;
                default:
                    break;
            }
            m_backType = value;
        }
    }

    public void CloseAllPrefab()
    {
        if(m_earthPrefab != null)
            m_earthPrefab.SetActive(false);
        //if (m_chinaPrefab != null)
        //    m_chinaPrefab.SetActive(false);
        if (m_tower != null)
            m_tower.gameObject.SetActive(false);
    }

    public void SetEarthState(EarthState state)
    {
        EarthType = state;
    }

    public void RotateSun(EventArguments e)
    {
        Vector3 vec = m_sunEarth.rotation.eulerAngles;
        vec.y += SunTenMinSpeed*10;

        if (vec.y > 360)
            vec.y -= 360;

        m_sunEarth.rotation = Quaternion.Euler(vec);
    }

    /// <summary>
    /// 显示地球播完动画之后
    /// </summary>
    /// <param name="e"></param>
    void ShowEarthTwo(EventArguments e)
    {
        m_earthCopy.SetActive(false);
        m_camera.cullingMask |= (1 << 4);
        m_cameraEarth.SetActive(false);
        Utility.SetLayer(m_earthPrefab, 0);
    }

	//void ShowChina(EventArguments e)
	//{
 //       UGUIManager.GetInstance().GetUGUILogic<UI_EarthPanel>().ShowChinaUI();

 //       CloseAllPrefab();
 //       m_earthCopy.SetActive(false);
	//	m_camera.cullingMask |= (1 << 4);
	//	m_cameraEarth.SetActive(false);
	//	Utility.SetLayer(m_earthPrefab, 0);
	//	m_light.SetActive(true);
	//	EarthControl earthControl1 = m_chinaPrefab.GetComponent<EarthControl>();
	//	if (earthControl1 == null)
	//		earthControl1 = m_chinaPrefab.AddComponent<EarthControl>();
	//	RotationScale rotation1 = m_chinaPrefab.GetComponent<RotationScale>();
	//	if (rotation1 == null)
	//		rotation1 = m_chinaPrefab.AddComponent<RotationScale>();
	//	Animator[] animator = m_chinaPrefab.GetComponentsInChildren<Animator>();
	//	foreach(Animator k in animator)
	//	{
	//		k.SetBool("biaoji", true);
	//	}
	//	if (m_hitParent != null)
	//	{
	//		earthControl1.m_HitTransform = m_hitParent.transform;
	//		m_chinaPrefab.transform.SetParent(m_hitParent.transform, false);
	//	}
	//	m_chinaPrefab.SetActive(true);
	//	#if UNITY_EDITOR
	//	m_chinaPrefab.transform.localPosition = new Vector3(0, 0, 700);
	//	#endif
	//	m_earthPrefab.SetActive(false);
	//}
}
