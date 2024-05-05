namespace UnityEngine.Persistance
{
	public class Persistence : MonoBehaviour
	{
		void Awake() => DontDestroyOnLoad( gameObject );
	}
}