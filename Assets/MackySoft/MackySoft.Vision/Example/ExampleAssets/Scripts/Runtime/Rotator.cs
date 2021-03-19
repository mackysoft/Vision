using UnityEngine;

namespace MackySoft.Vision.Example {

	[AddComponentMenu("MackySoft/Vision/Example/Rotator")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class Rotator : MonoBehaviour {

		[SerializeField]
		float m_Speed;

		Transform m_Transform;

		void Awake () {
			m_Transform = transform;
		}

		void Update () {
			m_Transform.Rotate(Vector3.up,m_Speed,Space.Self);
		}

	}
}