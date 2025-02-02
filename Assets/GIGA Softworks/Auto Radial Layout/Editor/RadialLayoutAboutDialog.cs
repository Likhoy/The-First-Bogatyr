#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GIGA.AutoRadialLayout.Editor
{
	public class RadialLayoutAboutDialog : EditorWindow
	{
		public Texture2D starTexture;
		public Texture2D icon;

		private static float starAnimTime;
		private static float animTime;
		
		public static void Init()
		{
			var window = (RadialLayoutAboutDialog)EditorWindow.GetWindow(typeof(RadialLayoutAboutDialog), true, "Auto Radial Layout");
			window.minSize = new Vector2(400, 520);
			window.maxSize = new Vector2(400, 520);
			window.Show();
			starAnimTime = (float)EditorApplication.timeSinceStartup;
			animTime = 0;
		}

		void OnGUI()
		{
			string version = RadialLayout.VERSION;

			EditorGUILayout.BeginHorizontal();

			GUILayout.Space(10);
			EditorGUILayout.BeginVertical();

			EditorGUILayout.BeginHorizontal();

			GUI.DrawTexture(new Rect(10 ,8, 64, 64), this.icon);
			GUILayout.Space(70);

			EditorGUILayout.BeginVertical();
			GUILayout.Space(10);
			EditorGUILayout.LabelField(string.Format("Auto Radial Layout"), EditorStyles.boldLabel);
			EditorGUILayout.LabelField(string.Format("Version: {0}", version)); 
			EditorGUILayout.LabelField(string.Format("Copyright \u00A9 GIGA Softworks, 2024 "));
			EditorGUILayout.EndVertical();

			EditorGUILayout.EndHorizontal();

			GUILayout.Space(30);
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


			GUIStyle linkStyle = new GUIStyle("label");
			linkStyle.normal.textColor = Color.blue;

			EditorGUILayout.LabelField(string.Format("Online Documentation:"), EditorStyles.boldLabel);
			if (GUILayout.Button("Documentation", linkStyle))
				Application.OpenURL("https://www.gigasoftworks.com/products/radiallayout/docs/overview.html");


			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

			EditorGUILayout.LabelField(string.Format("GIGA Softworks Website:"), EditorStyles.boldLabel);
			if (GUILayout.Button("http://www.gigasoftworks.com", linkStyle))
				Application.OpenURL("http://www.gigasoftworks.com");

			EditorGUILayout.LabelField(string.Format("Contact:"), EditorStyles.boldLabel);
			string address = "contact@gigasoftworks.com";
			if (GUILayout.Button(address, linkStyle))
			{
				string subject = "";
				Application.OpenURL(string.Format("mailto:{0}?subject={1}", address, subject));
			}

			EditorGUILayout.LabelField(string.Format("Bug Report:"), EditorStyles.boldLabel);
			address = "bugs@gigasoftworks.com";

			if (GUILayout.Button(address, linkStyle))
			{
				string subject = string.Format("Auto Radial Layout Bug Report - Ver. {0} Unity {1}", version, Application.unityVersion);
				Application.OpenURL(string.Format("mailto:{0}?subject={1}", address, subject));
			}

			EditorGUILayout.LabelField(string.Format("Follow me on X for latest updates:"), EditorStyles.boldLabel);
			if (GUILayout.Button("https://x.com/GigaSoftworks", linkStyle))
				Application.OpenURL("https://x.com/GigaSoftworks");


			EditorGUILayout.EndVertical();

			GUILayout.Space(10);
			EditorGUILayout.EndHorizontal();

			float elapsed = (float)EditorApplication.timeSinceStartup - starAnimTime;
			animTime += elapsed;
			EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			for (int k = 0; k < elapsed % 5; k++)
			{
				GUI.DrawTexture(new Rect(20 + k * 32 + k * 4, 360, 32, 32), this.starTexture);
			}
			GUILayout.BeginArea(new Rect(20, 400, 300, 100), "Thank you for downloading this asset!\nIf you found it useful please consider leaving a feedback\non the Asset Store page, it would be of great help :)");
			GUILayout.EndArea();

			if (GUI.Button(new Rect(55, 460, 300, 40), "Leave feedback"))
				Application.OpenURL("https://assetstore.unity.com/packages/slug/293726");

			Repaint();

		}

	}
}
#endif
