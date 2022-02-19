using System.Linq;
using UnityEngine;

namespace CIFER.Tech.Utils
{
    /// <summary>
    /// Unity関連のUtils
    /// </summary>
    public static class CiferTechUnityUtils
    {
        /// <summary>
        /// 探索Root以下にある探索名のオブジェクトを探す
        /// </summary>
        /// <param name="name">探索名</param>
        /// <param name="searchRoot">探索するRoot</param>
        /// <returns>探索結果</returns>
        public static Transform FindSameNameTransformInChildren(string name, Transform searchRoot)
        {
            return searchRoot.GetComponentsInChildren<Transform>().FirstOrDefault(tf => tf.name == name);
        }

        /// <summary>
        /// 探索Root以下にある探索名のオブジェクトを探し、もし無ければNULLを返す
        /// その後Tを取得して返す、無ければNULLを返す 
        /// </summary>
        /// <param name="searchName">探索名</param>
        /// <param name="searchRoot">探索するRoot</param>
        /// <typeparam name="T">探索するコンポーネント</typeparam>
        /// <returns>探索結果（コンポーネント）</returns>
        public static T FindT<T>(string searchName, Transform searchRoot) where T : Component
        {
            var convertGameObject = FindSameNameTransformInChildren(searchName, searchRoot)?.gameObject;
            if (convertGameObject == null)
                return null;

            var component = convertGameObject.GetComponent<T>();
            return component;
        }

        /// <summary>
        /// 探索Root以下にある探索名のオブジェクトを探し、もし無ければNULLを返す
        /// その後Tがあるかチェックし、もし無ければ追加してそのコンポーネントを返す
        /// </summary>
        /// <param name="searchName">探索名</param>
        /// <param name="searchRoot">探索するRoot</param>
        /// <typeparam name="T">探索するコンポーネント</typeparam>
        /// <returns>探索結果（コンポーネント）</returns>
        public static T FindOrCreateT<T>(string searchName, Transform searchRoot) where T : Component
        {
            var convertGameObject = FindSameNameTransformInChildren(searchName, searchRoot)?.gameObject;
            if (convertGameObject == null)
                return null;

            var component = convertGameObject.GetComponent<T>();
            if (component == null)
                component = convertGameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 探索Root以下にある探索名のオブジェクトを探し、もし無ければ新しく生成する
        /// その後Tがあるかチェックし、もし無ければ追加してそのコンポーネントを返す
        /// </summary>
        /// <param name="searchName">探索名</param>
        /// <param name="searchRoot">探索するRoot</param>
        /// <typeparam name="T">探索するコンポーネント</typeparam>
        /// <returns>探索結果（コンポーネント）</returns>
        public static T FindOrCreateTObject<T>(string searchName, Transform searchRoot) where T : Component
        {
            var convertGameObject = FindSameNameTransformInChildren(searchName, searchRoot)?.gameObject;
            if (convertGameObject == null)
                convertGameObject = new GameObject(searchName);

            var component = convertGameObject.GetComponent<T>();
            if (component == null)
                component = convertGameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// Tを持っているオブジェクトを削除する
        /// </summary>
        /// <param name="target">探索対象</param>
        /// <param name="isIncludeChildren">子を含めるか否か</param>
        /// <typeparam name="T">探索するコンポーネント</typeparam>
        public static void DeleteExistSetting<T>(Transform target, bool isIncludeChildren) where T : Component
        {
            foreach (var targetClass in isIncludeChildren
                         ? target.GetComponentsInChildren<T>()
                         : target.GetComponents<T>())
                Object.DestroyImmediate(targetClass);
        }
    }
}