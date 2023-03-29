#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

using System.Collections;
using System.Collections.Generic;
    
[CustomPropertyDrawer(typeof(Damage))]
public class Damage_PropertyDrawer : PropertyDrawer{

    float SpaceSize => EditorGUIUtility.standardVerticalSpacing;
    float LineSize => EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
        return (SpaceSize + LineSize) * 2;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var value = property.FindPropertyRelative("Value");
        var type = property.FindPropertyRelative("Type");
        var repulsion = property.FindPropertyRelative("Repulsion");

        EditorGUI.BeginProperty(position, label, property);

        // Damage
        var damageRect = new Rect(position);
        damageRect.height = LineSize;
        damageRect.width /= 2;

        value.floatValue = EditorGUI.FloatField(damageRect, value.floatValue);



        damageRect.height += damageRect.width;

        //value.enumValueFlag = EditorGUI.EnumPopup(damageRect, value.enumValueFlag);

        var repulsionRect = new Rect(position);
        repulsionRect.height /= 2;
        repulsionRect.x += repulsionRect.height;
        

        EditorGUI.EndProperty();
    }
}
#endif