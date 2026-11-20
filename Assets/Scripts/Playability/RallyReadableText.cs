using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public static class RallyReadableText
{
 private static TMP_FontAsset font;
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
 private static void Boot() { SceneManager.sceneLoaded += (_,__)=>Apply(); Apply(); }
 private static void Apply() { if(font==null) font=Resources.Load<TMP_FontAsset>("RallyFont"); if(font==null) throw new System.InvalidOperationException("Bundled RallyFont missing"); foreach(TMP_Text t in Resources.FindObjectsOfTypeAll<TMP_Text>()) if(t.gameObject.scene.IsValid()) {t.font=font;t.fontSharedMaterial=font.material;t.ForceMeshUpdate();} }
}
