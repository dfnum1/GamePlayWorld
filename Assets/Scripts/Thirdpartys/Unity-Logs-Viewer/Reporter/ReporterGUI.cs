#if UNITY_EDITOR
#define USE_REPORTVIEW
#endif
using UnityEngine;
using System.Collections;

public class ReporterGUI : MonoBehaviour
{
#if USE_REPORTVIEW
	Reporter reporter;
	void Awake()
	{
		reporter = gameObject.GetComponent<Reporter>();
	}

	void OnGUI()
	{
		reporter.OnGUIDraw();
	}
#endif
}
