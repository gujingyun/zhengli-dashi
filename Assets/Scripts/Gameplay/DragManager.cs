using UnityEngine;
using System.Collections.Generic;
using TuanjieFramework;

namespace ZhengLiMaster
{
    /// <summary>
    /// 拖拽管理器 - 处理所有拖拽逻辑
    /// </summary>
    public class DragManager : TuanjieSingleton<DragManager>
    {
        [Header("拖拽配置")]
        public Transform dragRoot;          // 拖拽时的父物体
        public float dragZOffset = 0f;     // 拖拽时的Z轴偏移

        [Header("射线检测")]
        public LayerMask boxLayer;          // 盒子层
        public float checkRadius = 1f;      // 检测半径

        private Item currentDragItem;       // 当前拖拽的物品
        private Camera mainCamera;

        protected override void Awake()
        {
            base.Awake();
            mainCamera = Camera.main;

            // 创建拖拽根物体
            if (dragRoot == null)
            {
                GameObject root = new GameObject("DragRoot");
                root.transform.SetParent(transform);
                root.transform.localPosition = new Vector3(0, 0, -10);
                dragRoot = root.transform;
            }
        }

        /// <summary>
        /// 开始拖拽物品
        /// </summary>
        public void StartDrag(Item item)
        {
            if (item == null) return;
            currentDragItem = item;
            item.OnDragStart();
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        public void Dragging(Vector2 screenPosition)
        {
            if (currentDragItem == null) return;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPosition);
            worldPos.z = dragZOffset;
            currentDragItem.OnDragging(screenPosition);
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        public void EndDrag()
        {
            if (currentDragItem == null) return;

            currentDragItem.OnDragEnd();
            currentDragItem = null;
        }

        /// <summary>
        /// 获取目标盒子
        /// </summary>
        public Box GetTargetBox(Vector3 position)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(position, checkRadius, boxLayer);

            Box nearestBox = null;
            float nearestDistance = float.MaxValue;

            foreach (var collider in colliders)
            {
                Box box = collider.GetComponent<Box>();
                if (box != null)
                {
                    float distance = Vector3.Distance(position, box.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestBox = box;
                    }
                }
            }

            return nearestBox;
        }

        /// <summary>
        /// 是否正在拖拽
        /// </summary>
        public bool IsDragging => currentDragItem != null;

        /// <summary>
        /// 获取当前拖拽的物品
        /// </summary>
        public Item CurrentDragItem => currentDragItem;

        private void Update()
        {
            // 移动端触摸输入
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        OnTouchBegan(touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        OnTouchMoved(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        OnTouchEnded();
                        break;
                }
            }
            // PC端鼠标输入（测试用）
            else if (Input.GetMouseButtonDown(0))
            {
                OnTouchBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                OnTouchMoved(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnded();
            }
        }

        private void OnTouchBegan(Vector2 position)
        {
            RaycastHit2D hit = Physics2D.Raycast(mainCamera.ScreenToWorldPoint(position), Vector2.zero);

            if (hit.collider != null)
            {
                Item item = hit.collider.GetComponent<Item>();
                if (item != null && !item.isInBox)
                {
                    StartDrag(item);
                }
            }
        }

        private void OnTouchMoved(Vector2 position)
        {
            Dragging(position);
        }

        private void OnTouchEnded()
        {
            EndDrag();
        }

        private void OnDrawGizmos()
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, checkRadius);
            #endif
        }
    }
}
