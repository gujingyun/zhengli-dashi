using UnityEngine;
using System.Collections.Generic;
using TuanjieFramework;

namespace ZhengLiMaster
{
    /// <summary>
    /// 收纳盒组件
    /// </summary>
    public class Box : TuanjieMonoBehaviour
    {
        [Header("盒子数据")]
        public string boxId;                    // 盒子唯一ID
        public ItemType acceptType;             // 接受的物品类型
        public int capacity = 3;                // 容量
        public int currentCount = 0;            // 当前物品数量

        [Header("物品槽")]
        public List<Item> items = new List<Item>(); // 盒内物品列表

        [Header("视觉")]
        public Transform itemSlotsParent;       // 物品槽父物体
        public Vector2 slotSpacing = new Vector2(80, 0); // 物品槽间距

        [Header("特效")]
        public GameObject fullEffect;           // 满了的特效
        public GameObject successEffect;        // 成功放入的特效

        /// <summary>
        /// 初始化盒子
        /// </summary>
        public void Initialize(string id, ItemType type, int cap)
        {
            boxId = id;
            acceptType = type;
            capacity = cap;
            currentCount = 0;
            items.Clear();
        }

        /// <summary>
        /// 是否可以放入物品
        /// </summary>
        public bool CanAddItem(ItemType type)
        {
            return type == acceptType && currentCount < capacity;
        }

        /// <summary>
        /// 添加物品到盒子
        /// </summary>
        public void AddItem(Item item)
        {
            if (!CanAddItem(item.itemType)) return;

            items.Add(item);
            currentCount++;

            // 设置物品位置到槽位
            UpdateItemPositions();

            // 播放放入特效
            if (successEffect != null)
            {
                Instantiate(successEffect, transform.position, Quaternion.identity);
            }

            // 检查是否满了
            if (currentCount >= capacity)
            {
                OnBoxFull();
            }
        }

        /// <summary>
        /// 从盒子移除物品
        /// </summary>
        public void RemoveItem(Item item)
        {
            if (items.Contains(item))
            {
                items.Remove(item);
                currentCount--;
                UpdateItemPositions();
            }
        }

        /// <summary>
        /// 清空盒子
        /// </summary>
        public void Clear()
        {
            items.Clear();
            currentCount = 0;
        }

        /// <summary>
        /// 更新物品位置
        /// </summary>
        private void UpdateItemPositions()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    Vector3 slotPos = GetSlotPosition(i);
                    items[i].transform.SetParent(itemSlotsParent ?? transform);
                    items[i].transform.localPosition = slotPos;
                }
            }
        }

        /// <summary>
        /// 获取槽位位置
        /// </summary>
        public Vector3 GetSlotPosition(int index)
        {
            float totalWidth = (capacity - 1) * slotSpacing.x;
            float startX = -totalWidth / 2f;
            return new Vector3(startX + index * slotSpacing.x, 0, 0);
        }

        /// <summary>
        /// 盒子满了
        /// </summary>
        private void OnBoxFull()
        {
            Debug.Log($"盒子 {boxId} 已满!");

            // 播放满特效
            if (fullEffect != null)
            {
                Instantiate(fullEffect, transform.position, Quaternion.identity);
            }

            // 通知游戏管理器
            GameManager.Instance.OnBoxFull(this);
        }

        /// <summary>
        /// 获取相同类型物品数量
        /// </summary>
        public int GetSameTypeCount(ItemType type)
        {
            int count = 0;
            foreach (var item in items)
            {
                if (item.itemType == type)
                    count++;
            }
            return count;
        }
    }
}
