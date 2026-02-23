using UnityEngine;

namespace TuanjieFramework
{
    /// <summary>
    /// 团结引擎MonoBehaviour基类
    /// 兼容Unity和团结引擎
    /// </summary>
    public class TuanjieMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 查找子物体
        /// </summary>
        protected T FindChild<T>(string name) where T : Object
        {
            Transform child = transform.Find(name);
            if (child != null)
            {
                return child.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 获取或添加组件
        /// </summary>
        protected T GetOrAddComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }
    }

    /// <summary>
    /// GameObject扩展方法
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 递归设置层级
        /// </summary>
        public static void SetLayerRecursive(this GameObject obj, LayerMask layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                child.gameObject.SetLayerRecursive(layer);
            }
        }
    }

    /// <summary>
    /// 团结引擎单例基类
    /// </summary>
    public class TuanjieSingleton<T> : TuanjieMonoBehaviour where T : TuanjieMonoBehaviour
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        GameObject singleton = new GameObject(typeof(T).Name);
                        instance = singleton.AddComponent<T>();
                        DontDestroyOnLoad(singleton);
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}

// 为兼容Unity的别名
namespace TuanjieEngine
{
    using TuanjieFramework;
}
