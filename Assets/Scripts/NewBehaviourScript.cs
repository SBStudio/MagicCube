using UnityEngine;
using Framework;

public class NewBehaviourScript : MonoBehaviour
{
	private class UserInfo : IDatabase
	{
		public override string[] columns {
			get {
				return Columns;
			}
		}

		public static readonly string[] Columns = new string[]
		{
			"Name", "QQ", "Email"
		};
	}

	private void Start()
	{
		DebugUtil.Add<FPSInfo>();

		DataSystem.Add<UserInfo>("Database.db3");

//		SqliteUtil db = new SqliteUtil("xuanyusong.db");
//		db.Set("momo", "email", "herox25000@gmail.com", new string[] { "name", "qq" }, new string[] { "yogi", "76288397" });
//		db.Add("momo", new string[] { "yogi1", "76288397", "herox25001@gmail.com", "www.yogi1.com" });
//		db.Delete("momo", new string[]{"email"}, new string[]{"herox25000@gmail.com"});
//		db.Close();
	}
}