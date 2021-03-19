using UnityEngine;

namespace MackySoft.Vision.Example {

	[AddComponentMenu("MackySoft/Vision/Example/Mover")]
	[HelpURL("https://github.com/mackysoft/Vision")]
	public class Mover : MonoBehaviour {

		[SerializeField]
		Transform m_TargetPoint;

		[SerializeField]
		float m_Speed;

		Transform m_Transform;
		Vector3 m_InitialPosition;

		void Awake () {
			m_Transform = transform;
			m_InitialPosition = m_Transform.position;
		}

		void Update () {
			m_Transform.position = Vector3.Slerp(m_InitialPosition,m_TargetPoint.position,Mathf.PingPong(Time.time * m_Speed,1f));
		}

	}
}