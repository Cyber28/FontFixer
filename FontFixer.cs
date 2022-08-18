using BepInEx;
using HarmonyLib;
using System.IO;
using TMPro;
using UnityEngine;

namespace FontFixerNS
{
	[BepInPlugin("FontFixer", "FontFixer", "1.0.0")]
	class FontFixer : BaseUnityPlugin
	{
		public static BepInEx.Logging.ManualLogSource L;
		public static AssetBundle FontBundle;
		public static string BundlePath;

		private void Awake()
		{
			L = Logger;
			BundlePath = Path.Combine(Directory.GetParent(this.Info.Location).ToString(), "mod.bundle");
			var harmony = new Harmony("FontFixer");
			harmony.PatchAll(typeof(Patches));
		}
	}

	class Patches
	{
		[HarmonyPatch(typeof(FontManager), "Awake")]
		[HarmonyPostfix]
		public static void FMAwake()
		{
			FontFixer.L.LogInfo("loading fonts..");
			if (FontFixer.FontBundle == null)
				FontFixer.FontBundle = AssetBundle.LoadFromFile(FontFixer.BundlePath);

			if (FontFixer.FontBundle == null)
			{
				FontFixer.L.LogError("Failed to load Font AssetBundle!");
				return;
			}

			FontManager.instance.ChineseTraditionalFontAsset = FontFixer.FontBundle.LoadAsset<TMP_FontAsset>("DYNTCB");
			FontManager.instance.ChineseSimplifiedFontAsset = FontFixer.FontBundle.LoadAsset<TMP_FontAsset>("DYNSCB");
			FontManager.instance.JapaneseFontAsset = FontFixer.FontBundle.LoadAsset<TMP_FontAsset>("DYNJPB");
			FontManager.instance.KoreanFontAsset = FontFixer.FontBundle.LoadAsset<TMP_FontAsset>("DYNKRB");
		}
	}
}