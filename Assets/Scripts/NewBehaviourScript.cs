using UnityEngine;
using Framework;

public class NewBehaviourScript : MonoBehaviour
{
	private void Start()
	{
		DebugUtil.Add<FPSInfo>();

		SqliteUtil db = new SqliteUtil("xuanyusong.db");
		db.Create("momo",new string[]{"name","qq","email","blog"}, new string[]{"text","text","text","text"});
		db.Close();
	}
}