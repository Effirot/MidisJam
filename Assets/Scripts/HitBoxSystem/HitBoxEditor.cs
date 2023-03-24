#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.UIElements;
    
    using UnityEngine;
    using UnityEngine.UIElements;
    
    using System.Collections;
    using System.Collections.Generic;
    


[CustomPropertyDrawer(typeof(AttackController))]
public class AttackController_PropertyDrawer : PropertyDrawer{

    int NowEditing = -1;
    int LinesCount = 0;

    float SpaceSize => EditorGUIUtility.standardVerticalSpacing;
    float LineSize => EditorGUIUtility.singleLineHeight;

    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (LineSize + SpaceSize) * LinesCount;
    }


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var attackArr = property.FindPropertyRelative("groups");
        
        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        {
            int lines = 0;
            property.isExpanded = true;
            
            var headerRect = new Rect(position);
            headerRect.height = LineSize;

            

            var tableRect = new Rect(position);
            tableRect.height = LineSize;
            tableRect.x -= tableRect.width / 2;
            tableRect.width *= 1.5f;
            lines += attackArr.arraySize;

            for(int i = 0; i < attackArr.arraySize; i++){

                tableRect.y += LineSize + SpaceSize;

                #region Edit button
                    var rectEditBtn = new Rect(tableRect);
                    rectEditBtn.width = 45;
                    
                    if(GUI.Button(rectEditBtn, "Edit"))
                        if(NowEditing != i)
                            NowEditing = i;
                        else NowEditing = -1;
                #endregion
                #region Name field
                    var rectLabel = new Rect(tableRect);
                    rectLabel.width = tableRect.width - rectEditBtn.width - (SpaceSize + LineSize) * 2;
                    rectLabel.x += rectEditBtn.width + SpaceSize;

                    var name = attackArr.GetArrayElementAtIndex(i).FindPropertyRelative("Name");
                    name.stringValue = EditorGUI.TextField(rectLabel, name.stringValue);
                #endregion
                #region Delete button
                    var rectDeleteBtn = new Rect(rectLabel);
                    rectDeleteBtn.width = LineSize;
                    rectDeleteBtn.x += SpaceSize + rectLabel.width;

                    if(GUI.Button(rectDeleteBtn, "D", new GUIStyle() { alignment = TextAnchor.MiddleCenter })){
                        attackArr.InsertArrayElementAtIndex(i);
                        NowEditing = i + 1;
                    }
                #endregion
                #region Duplicate button
                    var rectDuplicateBtn = new Rect(rectDeleteBtn);
                    rectDuplicateBtn.x += SpaceSize + LineSize;

                    if(GUI.Button(rectDuplicateBtn, "X")){
                        attackArr.DeleteArrayElementAtIndex(i);
                        NowEditing = -1;
                    }
                #endregion

                if(i == NowEditing)
                {
                    tableRect.y += (LineSize + SpaceSize) * 3;
                    lines += 3;
                }
            
            }

            

            LinesCount = lines;
        }
        EditorGUI.EndProperty();

    }
    

}

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

        var damageRect = new Rect(position);
        damageRect.height = SpaceSize;
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