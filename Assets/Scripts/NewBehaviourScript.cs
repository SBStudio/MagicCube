using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

public class NewBehaviourScript : MonoBehaviour
{
	private sealed class UserInfo : IDataInfo
	{
		public object key { get { return name; } }
		public string name { get; private set; }
		public string qq { get; private set; }
		public string email { get; private set; }

		public void Parse(Dictionary<string, object> arg)
		{
			name = arg["Name"].ToString();
			qq = arg["QQ"].ToString();
			email = arg["Email"].ToString();
		}
	}

	private void Start()
	{
		DebugUtil.Add<FPSInfo>();

		DataSystem<UserInfo>.Init("Database.db3", "UserInfo");

		UserInfo[] userInfos = DataSystem<UserInfo>.GetAll();
		for (int i = userInfos.Length; --i >= 0;)
		{
			LogUtil.LogError(userInfos[i].email);
		}
	}
}