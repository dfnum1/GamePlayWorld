/********************************************************************
生成日期:	1:11:2020 10:09
类    名: 	GameElementDatas
作    者:	HappLI
描    述:	游戏元素数据
*********************************************************************/
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "GameData/PrefabDatas")]
public class GameElementDatas : ScriptableObject
{
    public GameObject[] elements;

    static Dictionary<string, GameObject> ms_vElements = new Dictionary<string, GameObject>();
    private void OnEnable()
    {
        if (elements == null) return;
        for (int i =0; i < elements.Length; ++i)
        {
            if (elements[i] == null) continue;
            string name = BaseUtil.stringBuilder.AppendFormat("{0}/{1}", this.name, elements[i].name).ToString().ToLower();
            ms_vElements[name] = elements[i];
        }
    }
    private void OnDestroy()
    {
        for (int i = 0; i < elements.Length; ++i)
        {
            if (elements[i] == null) continue;
            string name = BaseUtil.stringBuilder.AppendFormat("{0}/{1}", this.name, elements[i].name).ToString().ToLower();
            ms_vElements.Remove(name);
        }
    }
    public static GameObject GetElement(string fullName)
    {
        if (ms_vElements == null)
            return null;
        if (ms_vElements.TryGetValue(fullName.ToLower(), out var ele))
            return ele;
        return null;
    }
}
