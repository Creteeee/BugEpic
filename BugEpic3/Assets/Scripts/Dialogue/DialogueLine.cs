using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string speaker;      // 说话人名字
    [TextArea(2, 5)]
    public string englishText;  // 英文
    [TextArea(2, 5)]
    public string customText;   // 自创语言
    [TextArea(2, 5)]
    public string chineseText;  // 中文
    [TextArea(2, 5)]
    public AudioClip voiceClip;
}

