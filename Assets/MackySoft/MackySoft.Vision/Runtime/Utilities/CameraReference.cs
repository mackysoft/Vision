#pragma warning disable CA2235 // Mark all non-serializable fields

using System;
using UnityEngine;

namespace MackySoft.Vision.Utilities {

	public enum CameraSourceMode {
		CurrentCamera = 0,
		MainCamera = 1,
		TaggedCamera = 2,
		Custom = 3
	}

	[Serializable]
	public class CameraReference {

		[SerializeField]
		CameraSourceMode m_Mode = CameraSourceMode.MainCamera;
		
		[SerializeField]
		string m_Tag = "Untagged";

		[SerializeField]
		Camera m_CustomCamera;

		Camera m_CachedCamera;

		public CameraSourceMode Mode {
			get => m_Mode;
			set {
				if (m_Mode != value) {
					ClearCache();
				}
				m_Mode = value;
			}
		}

		public string Tag {
			get => m_Tag;
			set {
				if (m_Mode == CameraSourceMode.TaggedCamera) {
					ClearCache();
				}
				m_Tag = value;
			}
		}

		public Camera CustomCamera {
			get => m_CustomCamera;
			set {
				if (m_Mode == CameraSourceMode.Custom) {
					ClearCache();
				}
				m_CustomCamera = value;
			}
		}

		/// <summary>
		/// <para> Returns a camera based on the <see cref="CameraSourceMode"/>. </para>
		/// <para> The retrieved camera will be cached. </para>
		/// </summary>
		public Camera Camera {
			get {
				if (m_CachedCamera != null) {
					return m_CachedCamera;
				}
				switch (m_Mode) {
					case CameraSourceMode.CurrentCamera:
						return m_CachedCamera = Camera.current;
					case CameraSourceMode.MainCamera:
						return m_CachedCamera = Camera.main;
					case CameraSourceMode.TaggedCamera:
						Camera[] allCameras = Camera.allCameras;
						for (int i = 0;allCameras.Length > i;i++) {
							Camera camera = allCameras[i];
							if (camera.CompareTag(m_Tag)) {
								return m_CachedCamera = camera;
							}
						}
						return m_CachedCamera = null;
					case CameraSourceMode.Custom:
						return m_CachedCamera = m_CustomCamera;
					default:
						throw new NotImplementedException();
				}
			}
		}

		public CameraReference () : this(CameraSourceMode.MainCamera) {

		}

		public CameraReference (CameraSourceMode source) {
			m_Mode = source;
		}

		public CameraReference (Camera customCamera) {
			m_Mode = CameraSourceMode.Custom;
			m_CustomCamera = customCamera;
		}

		public CameraReference (string tag) {
			m_Mode = CameraSourceMode.TaggedCamera;
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