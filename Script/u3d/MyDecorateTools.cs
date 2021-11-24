using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class MyDecorateTools : EditorWindow
{
    private static MyDecorateTools window;
    private string alignUnderBtn = "对齐到下面";
    private string selectGeneratdStuff = "选中生成物";
    private string genObj = "生成物体";
    [SerializeField]
    private List<GameObject> generatedObj = new List<GameObject>();
    private string helpString;
    //随机功能
    #region 随机功能
    float scalePos = 1.0f;
    float scaleScl = 1.0f;
    float scaleRot = 1.0f;
    string randomTranslateConfirm = "随机";
    #endregion
    //线形排列功能
    #region 线形排列功能
    int lrCount = 0;
    int lrOffset = 0;
    string linearRangeConfirm = "确认";
    #endregion
    //范围随机分布
    #region 范围随机分布
    int ScatterCount = 0;
    int ScatterRadius = 0;
    int ScatterRandom = 0;
    string[] ScatterToolBar = { "设置分布中心", "确认" };
    int sToolBarSelect = 0;
    Vector3 ScatterCenter = Vector3.zero;
    List<GameObject> scatterObjs = new List<GameObject>();
    #endregion
    //阵列分布
    #region 阵列分布
    int GridCountX = 1;
    int GridCountY = 1;
    int GridCountZ = 1;

    float GridSpaceX = 1;
    float GridSpaceY = 1;
    float GridSpaceZ = 1;

    string GridConfirm = "确认";
    #endregion
    [MenuItem("MyTools/DecorateTools",false,1000)]
   private static void showEditor()
    {
        window = EditorWindow.GetWindow<MyDecorateTools>("MyDecorateTools");
        window.Show();
    }

    private void OnGUI()
    {
        //generatedObj = new List<GameObject>();
        GUILayout.Label("提示:"+helpString, EditorStyles.helpBox);

        //if (GUILayout.Button(genObj))
        //{
        //    GenCubeTest();
        //}
        #region 通用功能
        GUILayout.Label("通用功能:", EditorStyles.boldLabel);
        if (GUILayout.Button(alignUnderBtn))
        {
            
            if (Selection.objects.Length > 0 || generatedObj.Count > 0)
            {
                foreach (GameObject item in Selection.objects)
                {
                    CheckUnderAndMove(item.transform);
                }
            }
            else
            {
                helpString = "没有选中物体或者没有已生成的物体";
            }
        }
        if (GUILayout.Button(selectGeneratdStuff))
        {

            if (generatedObj.Count != 0)
            {              
                Selection.objects = generatedObj.ToArray();
                Debug.Log(generatedObj.Count);
            }
            else
            {
                helpString = "没有已生成的物体";
            }
        }
        float tempSP = 0;
        float tempSS = 0;
        float tempSRot = 0;
        tempSP = EditorGUILayout.Slider("随机偏移范围",scalePos, 0, 100);
        tempSS = EditorGUILayout.Slider("随机缩放范围", scaleScl, 0, 100);
        tempSRot = EditorGUILayout.Slider("随机旋转范围", scaleRot, 0, 100);
        if(scalePos!=tempSP || scaleScl!=tempSS || scaleRot != tempSRot)
        {
            scalePos = tempSP;
            scaleScl = tempSS;
            scaleRot = tempSRot;
            RandomTranslate();
        }
        if (GUILayout.Button(randomTranslateConfirm))
        {           
            RandomTranslate();
        }
        #endregion
        GUILayout.Space(10);
        #region 线形排列
        GUILayout.Label("线形排列:", EditorStyles.boldLabel);
        GUILayout.Label("请选择两个相同物体");
        int tempLC = 0;
        int tempLO = 0;
        tempLC = (int)EditorGUILayout.Slider("数量", lrCount, 0, 100);
        tempLO = (int)EditorGUILayout.Slider("随机偏移", lrOffset, 0, 20);
        if (lrCount != tempLC || lrOffset!=tempLO)
        {
            lrCount = tempLC;
            lrOffset = tempLO;
            foreach (GameObject item in generatedObj)
            {
                if (item != null)
                {
                    Undo.DestroyObjectImmediate(item);
                    
                }
                    
            }
            generatedObj.Clear();           
            LinearRange();
        }
        if (GUILayout.Button(linearRangeConfirm))
        {
            Undo.ClearAll();
        }
        #endregion
        GUILayout.Space(10);
        #region 范围随机分布
        GUILayout.Label("范围随机分布:", EditorStyles.boldLabel);
        GUILayout.Label("请选择Prefab资源");
        int tempSC = 0;
        int tempSR = 0;
        int tempSRam = 0;
        tempSC = (int)EditorGUILayout.Slider("数量", ScatterCount, 0, 50);
        tempSR = (int)EditorGUILayout.Slider("分布范围", ScatterRadius, 0, 20);
        tempSRam = (int)EditorGUILayout.Slider("随机化", ScatterRandom, 0, 20);
        
        sToolBarSelect =  GUILayout.Toolbar(2, ScatterToolBar);

        if (sToolBarSelect == 0 && Selection.gameObjects.Length==1)
        {
            ScatterCenter = Selection.gameObjects[0].transform.position;
            ScatterCount = 0;
            ScatterRadius = 0;
            ScatterRandom = 0;
            helpString = "设置成功";
            scatterObjs.Clear();
        }
        if (ScatterCenter != Vector3.zero && Selection.objects.Length>0)
        {
            
            foreach (var item in Selection.objects)
            {
                if (PrefabUtility.IsPartOfPrefabAsset(item) && PrefabUtility.GetPrefabAssetType(item) == PrefabAssetType.Model)
                {
                    
                    Debug.Log("is a Prefab");
                    if(!scatterObjs.Contains((GameObject)item))
                        scatterObjs.Add((GameObject)item);
                }
            }
            Debug.Log(scatterObjs.Count);
            if ((ScatterCount != tempSC || ScatterRadius != tempSR || ScatterRandom != tempSRam) && scatterObjs.Count > 0)
            {
                Debug.Log("启动!!!");
                //tempSC = ScatterCount;
                ScatterCount = tempSC;
                ScatterRadius = tempSR;
                ScatterRandom = tempSRam;
                foreach (GameObject item in generatedObj)
                {
                    if (item != null)
                    {
                        Undo.DestroyObjectImmediate(item);

                    }

                }
                generatedObj.Clear();
                ScatterRange();
            }
        }
        //else
        //{
        //    scatterObjs.Clear();
        //}
        if (sToolBarSelect == 1)
        {
            scatterObjs.Clear();
            generatedObj.Clear();
        }
        #endregion
        GUILayout.Space(10);
        #region 阵列分布
        GUILayout.Label("阵列:", EditorStyles.boldLabel);
        GUILayout.Label("请选择一个游戏对象");
        int tempCTX = 1;
        int tempCTY = 1;
        int tempCTZ = 1;
        float tempSpacingX = 1;
        float tempSpacingY = 1;
        float tempSpacingZ = 1;
        tempCTX = EditorGUILayout.IntSlider("Count X", GridCountX, 1, 10);
        tempCTY = EditorGUILayout.IntSlider("Count Y", GridCountY, 1, 10);
        tempCTZ = EditorGUILayout.IntSlider("Count Z", GridCountZ, 1, 10);
        GUILayout.Space(5);
        tempSpacingX = EditorGUILayout.Slider("Space X", GridSpaceX, 1, 10);
        tempSpacingY = EditorGUILayout.Slider("Space Y", GridSpaceY, 1, 10);
        tempSpacingZ = EditorGUILayout.Slider("Space Z", GridSpaceZ, 1, 10);
        if (GridCountX != tempCTX || GridCountY != tempCTY || GridCountZ != tempCTZ
            || GridSpaceX != tempSpacingX || GridSpaceY != tempSpacingY || GridSpaceZ != tempSpacingZ)
        {
            GridCountX = tempCTX;
            GridCountY = tempCTY;
            GridCountZ = tempCTZ;
            GridSpaceX = tempSpacingX;
            GridSpaceY = tempSpacingY;
            GridSpaceZ = tempSpacingZ;
            foreach (GameObject item in generatedObj)
            {
                if (item != null)
                    GameObject.DestroyImmediate(item);
            }
            generatedObj.Clear();

            CloneSelected();
        }
        if (GUILayout.Button(GridConfirm))
        {
            generatedObj.Clear();
        }
        
        #endregion

    }

    private void CloneSelected()
    {
        if (!Selection.activeGameObject)
        {
            helpString = "请选择一个gameobject";
            return;
        }

        for (int i = 0; i < GridCountX; i++)
            for (int j = 0; j < GridCountY; j++)
                for (int k = 0; k < GridCountZ; k++)
                {
                    GameObject gameObject = Instantiate(Selection.activeGameObject, new Vector3(i * GridSpaceX, j * GridSpaceY, k * GridSpaceZ) + Selection.activeGameObject.transform.position, Selection.activeGameObject.transform.rotation);
                    //Undo.RegisterCreatedObjectUndo(gameObject, $"gen{i}");
                    generatedObj.Add(gameObject);
                }
    }
    void CheckUnderAndMove(Transform obj)
    {
        RaycastHit hit;
        bool grounded = Physics.Raycast(obj.position, -Vector3.up, out hit);
        
        if (grounded)
        {
            Renderer objRenderer = obj.GetComponent<Renderer>();
            if (objRenderer == null)
            {
                objRenderer = obj.GetChild(0).GetComponent<Renderer>();
            }
            Vector3 size = objRenderer.bounds.size;
            Vector3 newPos = new Vector3(hit.point.x, hit.point.y + size.y/2, hit.point.z);
            obj.position = newPos;
            
        }
    }
    void GenCubeTest()
    {
        if (Selection.gameObjects.Length == 1)
        {
            
            GameObject gameObject = Instantiate(Selection.gameObjects[0], Vector3.zero, Quaternion.identity);
            generatedObj.Add(gameObject);
        }
        
    }
    void RandomTranslate()
    {
        if (Selection.objects.Length > 0)
        {
            foreach (GameObject item in Selection.objects)
            {
                if(scalePos!=0)
                    item.transform.position = new Vector3(Random.value, Random.value, Random.value) * scalePos;
                if(scaleScl!=0)
                    item.transform.localScale = new Vector3(Random.value, Random.value, Random.value) * scaleScl;
                if(scaleRot!=0)
                    item.transform.rotation = Quaternion.Euler(new Vector3(Random.value, Random.value, Random.value) * scaleRot);
            }
        }
        else
        {
            helpString = "没有选中物体";
        }
    }
    void LinearRange()
    {

        if (Selection.gameObjects.Length == 2 && lrCount>0)
        {
            GameObject[] gobj = new GameObject[lrCount];
            //float ranVal = Random.value;
            for (int i = 1; i <= lrCount; i++)
            {
                Vector3 start = Selection.gameObjects[0].transform.position;
                Vector3 end = Selection.gameObjects[1].transform.position;

                Vector3 tempPos = Vector3.Lerp(start, end, (1.0f/(lrCount + 1)) * i);

                Vector3 startAngle = Selection.gameObjects[0].transform.localEulerAngles;
                Vector3 endAngle = Selection.gameObjects[1].transform.localEulerAngles;
                Vector3 tempAngle = Vector3.Lerp(startAngle, endAngle, (1.0f / (lrCount + 1)) * i);

                GameObject gameObject = Instantiate(Selection.gameObjects[0], tempPos, Quaternion.Euler(tempAngle));
                if (lrOffset > 0)
                {
                    Vector3 newPos = new Vector3(Random.Range(-lrOffset / 20.0f, lrOffset / 20.0f),
                        Random.Range(-lrOffset / 20.0f, lrOffset / 20.0f),
                        Random.Range(-lrOffset / 20.0f, lrOffset / 20.0f));

                    gameObject.transform.position += newPos;
                    gameObject.transform.rotation = Quaternion.Euler(tempAngle * Random.Range(-lrOffset / 20.0f, lrOffset / 20.0f));
                }
                //gobj[i - 1] = gameObject;
                generatedObj.Add(gameObject);
                Undo.RegisterCreatedObjectUndo(gameObject,$"gen{i}");
            }
            
            //else
            //{
            //    helpString = "不是相同物体";
            //    Debug.Log("不是同一物体");
            //}
            
        }
        else
        {
            helpString = "没有选择两个物体";
            Debug.Log("没有选择两个物体");
        }
    }
    void ScatterRange()
    {
        foreach (GameObject item in scatterObjs)
        {
            for (int i = 0; i < ScatterCount; i++)
            {
                Vector3 c = ScatterCenter;
                Vector3 pos = new Vector3(Random.Range(c.x - ScatterRadius, c.x+ ScatterRadius), Random.Range(c.y - ScatterRadius, c.y +ScatterRadius), Random.Range(c.z - ScatterRadius, c.z+ScatterRadius));
                GameObject gameObject = Instantiate(item, pos, Quaternion.identity);
                Undo.RegisterCreatedObjectUndo(gameObject, $"gen{i}");
                generatedObj.Add(gameObject);
            }
        }
    }
}
