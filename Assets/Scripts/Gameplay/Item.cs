using UnityEngine;
using TuanjieFramework;

namespace ZhengLiMaster
{
    /// <summary>
    /// 物品类型
    /// </summary>
    public enum ItemType
    {
        Clothes,     // 衣服
        Toy,        // 玩具
        Book,       // 书籍
        Food,       // 食物
        Electronics // 电子产品
    }

    /// <summary>
    /// 物品组件 - 挂在物品预制体上
    /// </summary>
    public class Item : TuanjieMonoBehaviour
    {
        [Header("物品数据")]
        public string itemId;           // 物品唯一ID
        public ItemType itemType;       // 物品类型
        public Vector2Int gridPosition; // 网格位置

        [Header("状态")]
        public bool isDragging = false;     // 是否正在拖拽
        public bool isInBox = false;       // 是否在盒子中
        public string currentBoxId;         // 所在盒子ID

        private Vector2 originalPosition;  // 原始位置
        private Transform originalParent;  // 原始父物体

        /// <summary>
        /// 初始化物品
        /// </summary>
        public void Initialize(string id, ItemType type, Vector2Int gridPos)
        {
            itemId = id;
            itemType = type;
            gridPosition = gridPos;
            isDragging = false;
            isInBox = false;
            currentBoxId = null;
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        public void OnDragStart()
        {
            if (isInBox) return;

            isDragging = true;
            originalPosition = transform.localPosition;
            originalParent = transform.parent;

            // 设置为拖拽层级的父物体
            transform.SetParent(DragManager.Instance.dragRoot);
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        public void OnDragging(Vector2 screenPosition)
        {
            if (!isDragging) return;

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
            transform.localPosition = new Vector3(worldPos.x, worldPos.y, 0);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void OnDragEnd()
        {
            if (!isDragging) return;

            isDragging = false;

            // 检测是否放入盒子
            Box targetBox = DragManager.Instance.GetTargetBox(transform.position);

            if (targetBox != null && targetBox.CanAddItem(itemType))
            {
                // 放入正确盒子
                targetBox.AddItem(this);
                isInBox = true;
                currentBoxId = targetBox.boxId;
                
                // 通知游戏管理器
                GameManager.Instance.OnItemPlacedInBox(this, targetBox);
            }
            else
            {
                // 弹回原位
                ReturnToOriginalPosition();
            }
        }

        /// <summary>
        /// 弹回原位
        /// </summary>
        public void ReturnToOriginalPosition()
        {
            transform.SetParent(originalParent);
            transform.localPosition = originalPosition;
        }

        /// <summary>
        /// 从盒子中移除
        /// </summary>
        public void RemoveFromBox()
        {
            isInBox = false;
            currentBoxId = null;
            originalPosition = transform.localPosition;
            originalParent = transform.parent;
        }
    }
}
