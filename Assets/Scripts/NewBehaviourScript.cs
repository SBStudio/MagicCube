using UnityEngine;
using System;
using System.Collections.Generic;
using Framework;

public class NewBehaviourScript : MonoBehaviour
{
	private sealed class UserInfo : IDataInfo<string>
	{
		public string database { get { return "Database.db3"; } }
		public string table { get { return "UserInfo"; } }
		public Dictionary<string, Type> types { get { return m_Types; } }
		private readonly Dictionary<string, Type> m_Types = new Dictionary<string, Type>()
		{
			{ "Name", typeof(string) },
			{ "QQ", typeof(string) },
			{ "Email", typeof(string) },
		};
		public string key { get; private set; }
		public string qq { get; private set; }
		public string email { get; private set; }

		public void Parse(Dictionary<string, object> arg)
		{
			key = arg["Name"].ToString();
			qq = arg["QQ"].ToString();
			email = arg["Email"].ToString();
		}
	}

	private void Start()
	{
		DebugUtil.Add<FPSInfo>();

		LogUtil.LogError(DataSystem<string, UserInfo>.instance.Get("Yogi").email);

//		SqliteUtil db = new SqliteUtil("xuanyusong.db");
//		db.Set("momo", "email", "herox25000@gmail.com", new string[] { "name", "qq" }, new string[] { "yogi", "76288397" });
//		db.Add("momo", new string[] { "yogi1", "76288397", "herox25001@gmail.com", "www.yogi1.com" });
//		db.Delete("momo", new string[]{"email"}, new string[]{"herox25000@gmail.com"});
//		db.Close();
	}
}