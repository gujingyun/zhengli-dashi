using UnityEngine;
using ZhengLiMaster;

namespace ZhengLiMaster
{
    /// <summary>
    /// 拖拽测试场景引导器
    /// 用于测试拖拽系统
    /// </summary>
    public class DragTestScene : MonoBehaviour
    {
        [Header("测试配置")]
        public GameObject itemPrefab;       // 物品预制体
        public GameObject boxPrefab;         // 盒子预制体
        
        [Header("测试参数")]
        public Vector2 itemStartPos = new Vector2(-3, 2);  // 物品起始位置
        public Vector2 boxPos = new Vector2(3, 0);         // 盒子位置
        public int itemCount = 3;            // 测试物品数量

        private void Start()
        {
            // 初始化游戏
            GameManager.Instance.StartGame();
            
            Debug.Log("拖拽测试场景已启动");
            Debug.Log("请在手机上测试：拖拽物品到对应盒子");
        }

        private void Update()
        {
            // 调试：用R键重新开始
            if (Input.GetKeyDown(KeyCode.R))
            {
                GameManager.Instance.RestartLevel();
            }
        }

        /// <summary>
        /// 创建测试物品（由编辑器调用）
        /// </summary>
        public void CreateTestItems()
        {
            if (itemPrefab == null)
            {
                Debug.LogWarning("请先设置物品预制体!");
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                Vector3 pos = itemStartPos + new Vector2(i * 1.5f, 0);
                GameObject obj = Instantiate(itemPrefab, transform);
                obj.transform.localPosition = pos;
                
                Item item = obj.GetComponent<Item>();
                if (item != null)
                {
                    item.Initialize($"test_item_{i}", ItemType.Clothes, new Vector2Int(i, 0));
                }
            }
        }

        /// <summary>
        /// 创建测试盒子（由编辑器调用）
        /// </summary>
        public void CreateTestBox()
        {
            if (boxPrefab == null)
            {
                Debug.LogWarning("请先设置盒子预制体!");
                return;
            }

            GameObject obj = Instantiate(boxPrefab, transform);
            obj.transform.localPosition = boxPos;
            
            Box box = obj.GetComponent<Box>();
            if (box != null)
            {
                box.Initialize("test_box_1", ItemType.Clothes, 3);
            }
        }
    }
}
