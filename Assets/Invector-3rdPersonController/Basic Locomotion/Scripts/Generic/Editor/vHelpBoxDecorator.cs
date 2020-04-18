using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof (vHelpBoxAttribute))]
public class vHelpBoxDecorator : DecoratorDrawer
{
    public Vector2 size;

    public override void OnGUI(Rect position)
    {
        EditorStyles.helpBox.richText = true;
        var helpbox = attribute as vHelpBoxAttribute;
        var content = new GUIContent(helpbox.text);
        var messageType = MessageType.None;
        switch (helpbox.messageType)
        {
            case vHelpBoxAttribute.MessageType.Info:messageType = MessageType.Info;
                break;
            case vHelpBoxAttribute.MessageType.Warning: messageType = MessageType.Warning;
                break;
        }
        EditorGUI.HelpBox(position, content.text, messageType);
    }

    public override float GetHeight()
    {
        var helpBoxAttribute = attribute as vHelpBoxAttribute;
        if (helpBoxAttribute == null) return base.GetHeight();
        var helpBoxStyle = (GUI.skin != null) ? GUI.skin.GetStyle("helpbox") : null;
        if (helpBoxStyle == null) return base.GetHeight();
        return Mathf.Max(EditorGUIUtility.singleLineHeight, helpBoxStyle.CalcHeight(new GUIContent(helpBoxAttribute.text), EditorGUIUtility.currentViewWidth) + 10);
    }
}