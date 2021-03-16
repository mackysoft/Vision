#pragma warning disable CA2235 // Mark all non-serializable fields

using System;
using UnityEngine;

namespace MackySoft.Vision.Utilities {

	public enum CameraReferenceMode {
		CurrentCamera = 0,
		MainCamera = 1,
		TaggedCamera = 2,
		Custom = 3
	}

	/// <summary>
	/// <para> Advanced camera reference. </para>
	/// <para> Useful when referencing camera from multiple scenes and prefabs. </para>
	/// </summary>
	[Serializable]
	public class CameraReference {

		[SerializeField]
		CameraReferenceMode m_Mode = CameraReferenceMode.MainCamera;
		
		[SerializeField]
		string m_Tag = "Untagged";

		[SerializeField]
		Camera m_CustomCamera;

		Camera m_CachedCamera;

		/// <summary>
		/// Method of referring to the camera.
		/// </summary>
		public CameraReferenceMode Mode {
			get => m_Mode;
			set {
				if (m_Mode != value) {
					ClearCache();
				}
				m_Mode = value;
			}
		}

		/// <summary>
		/// A tag for referencing a camera when <see cref="Mode"/> is <see cref="CameraReferenceMode.TaggedCamera"/>.
		/// </summary>
		public string Tag {
			get => m_Tag;
			set {
				if (m_Mode == CameraReferenceMode.TaggedCamera) {
					ClearCache();
				}
				m_Tag = value;
			}
		}

		/// <summary>
		/// Reference to the camera when <see cref="Mode"/> is <see cref="CameraReferenceMode.Custom"/>.
		/// </summary>
		public Camera CustomCamera {
			get => m_CustomCamera;
			set {
				if (m_Mode == CameraReferenceMode.Custom) {
					ClearCache();
				}
				m_CustomCamera = value;
			}
		}

		/// <summary>
		/// <para> Returns a camera based on the <see cref="CameraReferenceMode"/>. </para>
		/// <para> The retrieved camera will be cached. </para>
		/// </summary>
		public Camera Camera {
			get {
				if (m_CachedCamera != null) {
					return m_CachedCamera;
				}
				switch (m_Mode) {
					case CameraReferenceMode.CurrentCamera:
						return m_CachedCamera = Camera.current;
					case CameraReferenceMode.MainCamera:
						return m_CachedCamera = Camera.main;
					case CameraReferenceMode.TaggedCamera:
						Camera[] allCameras = Camera.allCameras;
						for (int i = 0;allCameras.Length > i;i++) {
							Camera camera = allCameras[i];
							if (camera.CompareTag(m_Tag)) {
								return m_CachedCamera = camera;
							}
						}
						return m_CachedCamera = null;
					case CameraReferenceMode.Custom:
						return m_CachedCamera = m_CustomCamera;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public CameraReference () : this(CameraReferenceMode.MainCamera) {

		}

		public CameraReference (CameraReferenceMode mode) {
			m_Mode = mode;
		}

		public CameraReference (Camera customCamera) {
			m_Mode = CameraReferenceMode.Custom;
			m_CustomCamera = customCamera;
		}

		public CameraReference (string tag) {
			m_Mode = CameraReferenceMode.TaggedCamera;
			m_Tag = tag;
		}

		/// <summary>
		/// Set the cached camera to null.
		/// </summary>
		public void ClearCache () {
			m_CachedCamera = null;
		}

		public static implicit operator Camera (CameraReference source) {
			return source.Camera;
		}

	}
}