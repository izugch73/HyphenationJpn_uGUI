using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[ExecuteInEditMode]
public class HyphenationJpn : UIBehaviour
{
    // http://answers.unity3d.com/questions/424874/showing-a-textarea-field-for-a-string-variable-in.html
    [TextArea(3, 10), SerializeField] private string text;

    private RectTransform _RectTransform
    {
        get
        {
            if (_rectTransform == null)
                _rectTransform = GetComponent<RectTransform>();
            return _rectTransform;
        }
    }

    private RectTransform _rectTransform;

    private Text _Text
    {
        get
        {
            if (_text == null)
                _text = GetComponent<Text>();
            return _text;
        }
    }

    private Text _text;

    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateText(text);
    }

    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateText(text);
    }

    private void UpdateText(string str)
    {
        // update Text
        _Text.text = GetFormatedText(_Text, str);
    }

    public void SetText(string str)
    {
        text = str;
        UpdateText(text);
    }

    private float GetSpaceWidth(Text textComp)
    {
        var tmp0 = GetTextWidth(textComp, "m m");
        var tmp1 = GetTextWidth(textComp, "mm");
        return (tmp0 - tmp1);
    }

    private float GetTextWidth(Text textComp, string message)
    {
        if (_text.supportRichText)
        {
            message = Regex.Replace(message, RITCH_TEXT_REPLACE, string.Empty);
        }

        textComp.text = message;
        return textComp.preferredWidth;
    }

    private string GetFormatedText(Text textComp, string msg)
    {
        if (string.IsNullOrEmpty(msg))
        {
            return string.Empty;
        }

        var rectWidth = _RectTransform.rect.width;
        var spaceCharacterWidth = GetSpaceWidth(textComp);

        // override
        textComp.horizontalOverflow = HorizontalWrapMode.Overflow;

        // work
        var lineBuilder = new StringBuilder();

        float lineWidth = 0;
        foreach (var originalLine in GetWordList(Regex.Replace(msg, Environment.NewLine, "\n")))
        {
            lineWidth += GetTextWidth(textComp, originalLine);

            if (originalLine == "\n")
            {
                lineWidth = 0;
            }
            else
            {
                if (originalLine == " ")
                {
                    lineWidth += spaceCharacterWidth;
                }

                if (lineWidth > rectWidth)
                {
                    lineBuilder.Append(Environment.NewLine);
                    lineWidth = GetTextWidth(textComp, originalLine);
                }
            }

            lineBuilder.Append(originalLine);
        }

        return lineBuilder.ToString();
    }

    private static List<string> GetWordList(string tmpText)
    {
        var words = new List<string>();
        var line = new StringBuilder();
        var emptyChar = new char();

        for (var characterCount = 0; characterCount < tmpText.Length; characterCount++)
        {
            var currentCharacter = tmpText[characterCount];
            var nextCharacter = (characterCount < tmpText.Length - 1) ? tmpText[characterCount + 1] : emptyChar;
            char preCharacter = (characterCount > 0) ? preCharacter = tmpText[characterCount - 1] : emptyChar;

            line.Append(currentCharacter);

            if (((IsLatin(currentCharacter) && IsLatin(preCharacter)) &&
                 (IsLatin(currentCharacter) && !IsLatin(preCharacter))) ||
                (!IsLatin(currentCharacter) && CHECK_HYP_BACK(preCharacter)) ||
                (!IsLatin(nextCharacter) && !CHECK_HYP_FRONT(nextCharacter) && !CHECK_HYP_BACK(currentCharacter)) ||
                (characterCount == tmpText.Length - 1) ||
                (currentCharacter == '\n'))
            {
                words.Add(line.ToString());
                line = new StringBuilder();
                continue;
            }
        }

        return words;
    }

    // helper
    public float textWidth
    {
        set { _RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value); }
        get { return _RectTransform.rect.width; }
    }

    public int fontSize
    {
        set { _Text.fontSize = value; }
        get { return _Text.fontSize; }
    }

    // static
    private static readonly string RITCH_TEXT_REPLACE =
        "(\\<color=.*\\>|</color>|" +
        "\\<size=.n\\>|</size>|" +
        "<b>|</b>|" +
        "<i>|</i>)";

    // 禁則処理 http://ja.wikipedia.org/wiki/%E7%A6%81%E5%89%87%E5%87%A6%E7%90%86
    // 行頭禁則文字
    private static readonly char[] HYP_FRONT =
        (",)]｝、。）〕〉》」』】〙〗〟’”｠»" + // 終わり括弧類 簡易版
         "ァィゥェォッャュョヮヵヶっぁぃぅぇぉっゃゅょゎ" + //行頭禁則和字 
         "‐゠–〜ー" + //ハイフン類
         "?!！？‼⁇⁈⁉" + //区切り約物
         "・:;" + //中点類
         "。.").ToCharArray(); //句点類

    private static readonly char[] HYP_BACK =
        "(（[｛〔〈《「『【〘〖〝‘“｟«".ToCharArray(); //始め括弧類

    private static readonly char[] HYP_LATIN =
        ("abcdefghijklmnopqrstuvwxyz" +
         "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
         "0123456789" +
         "<>=/().,").ToCharArray();

    private static bool CHECK_HYP_FRONT(char str)
    {
        return Array.Exists<char>(HYP_FRONT, item => item == str);
    }

    private static bool CHECK_HYP_BACK(char str)
    {
        return Array.Exists<char>(HYP_BACK, item => item == str);
    }

    private static bool IsLatin(char s)
    {
        return Array.Exists<char>(HYP_LATIN, item => item == s);
    }
}