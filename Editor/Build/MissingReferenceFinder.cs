using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Rano.Editor.Build
{
	public class MissingReferenceFinder : MonoBehaviour
	{
		private const int PRIORITY = 300;
		private const string MENU_ROOT = "Build/Find Missing References/";

		[MenuItem(MENU_ROOT + "Search in scene", false, PRIORITY+1)]
		public static void FindMissingReferencesInCurrentScene()
		{
			var sceneObjects = GetSceneObjects();
			FindMissingReferences(SceneManager.GetActiveScene().path, sceneObjects);
		}
		
		[MenuItem(MENU_ROOT + "Search in build scenes", false, PRIORITY+2)]
		public static void FindMissingReferencesInBuildScenes()
		{
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
			foreach (var scene in EditorBuildSettings.scenes.Where(s => s.enabled))
			{
				EditorSceneManager.OpenScene(scene.path);
				FindMissingReferencesInCurrentScene();
			}
			EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
		}

		[MenuItem(MENU_ROOT + "Search in assets", false, PRIORITY+4)]
		public static void FindMissingReferencesInAssets()
		{
			var allAssets = AssetDatabase.GetAllAssetPaths().Where(path => path.StartsWith("Assets/")).ToArray();
			var objs = allAssets.Select(a => AssetDatabase.LoadAssetAtPath(a, typeof(GameObject)) as GameObject)
				.Where(a => a != null).ToArray();
			FindMissingReferences("Project", objs);
		}

		private static void FindMissingReferences(string context, GameObject[] gameObjects)
		{
			if (gameObjects == null)
			{
				return;
			}

			foreach (var go in gameObjects)
			{
				var components = go.GetComponents<Component>();

				foreach (var component in components)
				{
					// 잃어버린 컴포넌트는 null로 간주된다.
					if (!component)
					{
						Log.Warning($"컴포넌트 없음 (Component:{component.GetType().FullName}, GameObject:{GetFullPath(go)}");
						continue;
					}

					SerializedObject so = new SerializedObject(component);
					var sp = so.GetIterator();

					var objRefValueMethod = typeof(SerializedProperty).GetProperty("objectReferenceStringValue",
						BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

					// 컴포넌트의 프로퍼티를 체크한다.
					while (sp.NextVisible(true))
					{
						if (sp.propertyType == SerializedPropertyType.ObjectReference)
						{
							string objectReferenceStringValue = string.Empty;

							if (objRefValueMethod != null)
							{
								objectReferenceStringValue =
									(string) objRefValueMethod.GetGetMethod(true).Invoke(sp, new object[] { });
							}

							if (sp.objectReferenceValue == null
							    && (sp.objectReferenceInstanceIDValue != 0 ||
							        objectReferenceStringValue.StartsWith("Missing")))
							{
								ShowError(context, go, component.GetType().Name,
									ObjectNames.NicifyVariableName(sp.name));
							}
						}
					}
				}
			}
		}

		private static GameObject[] GetSceneObjects()
		{
			// Use this method since GameObject.FindObjectsOfType will not return disabled objects.
			return Resources.FindObjectsOfTypeAll<GameObject>()
				.Where(go => string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go))
				             && go.hideFlags == HideFlags.None).ToArray();
		}

		private static void ShowError(string context, GameObject go, string componentName, string propertyName)
		{
			const string errorTemplate = "Missing Ref in: [{3}]{0}. Component: {1}, Property: {2}";
			Debug.LogError(string.Format(errorTemplate, GetFullPath(go), componentName, propertyName, context), go);
		}

		private static string GetFullPath(GameObject go)
		{
			return go.transform.parent == null
				? go.name
				: GetFullPath(go.transform.parent.gameObject) + "/" + go.name;
		}
	}
}