using UnityEngine;

namespace Framework
{
	public interface IDebugInfo
	{
		void OnGUI();
		void OnUpdate();
		void OnLateUpdate();
		void OnFixedUpdate();
	}
}