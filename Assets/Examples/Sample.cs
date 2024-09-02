using UnityEngine;

namespace HyphenationJpn
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] public HyphenationJpn _hypJpn;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, 600, 600));

            GUILayout.Label("width");
            _hypJpn.TextWidth = GUILayout.HorizontalSlider(_hypJpn.TextWidth, 100, 600);
            GUILayout.Label("fontSize");
            _hypJpn.FontSize = (int)GUILayout.HorizontalSlider(_hypJpn.FontSize, 10, 40);

            GUILayout.Space(20);

            if (GUILayout.Button("ChangeText"))
            {
                const string sampleText =
                    "Unityマニュアルガイドは特定のプラットフォームにのみ適用されるセクションを含みます。\n自ら参照したいセクションを選択して下さい。プロットフォーム特有の情報は各ページの三角形の記号で示されるボタンにより展開して参照することが出来ます。";
                _hypJpn.SetText(sampleText);
            }

            GUILayout.EndArea();
        }
    }
}