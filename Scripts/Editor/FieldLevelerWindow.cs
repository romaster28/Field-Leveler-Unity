using System;
using Editor.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Editor
{
    public class FieldLevelerWindow : UnityEditor.EditorWindow
    {
        private const string ColumnsKey = "FieldLeveler_Columns";

        private const string OffsetXKey = "FieldLeveler_OffsetX";

        private const string OffsetYKey = "FieldLeveler_OffsetY";

        private const string AddOffsetFromRendererKey = "FieldLeveler_AddOffsetFromRenderer";

        private static int ColumnsCount
        {
            get => PlayerPrefs.GetInt(ColumnsKey);
            set => PlayerPrefs.SetInt(ColumnsKey, value);
        }

        private static float OffsetX
        {
            get => PlayerPrefs.GetFloat(OffsetXKey, 1);
            set => PlayerPrefs.SetFloat(OffsetXKey, value);
        }

        private static float OffsetY
        {
            get => PlayerPrefs.GetFloat(OffsetYKey, 1);
            set => PlayerPrefs.SetFloat(OffsetYKey, value);
        }

        private static bool AddOffsetFromRenderer
        {
            get => PlayerPrefs.GetInt(AddOffsetFromRendererKey) == 1;
            set => PlayerPrefs.SetInt(AddOffsetFromRendererKey, value ? 1 : 0);
        }

        [UnityEditor.MenuItem("Tools/FieldLeveler")]
        private static void ShowWindow()
        {
            var window = GetWindow<FieldLevelerWindow>();
            window.titleContent = new UnityEngine.GUIContent("Field Leveler");
            window.Show();
        }

        private void OnGUI()
        {
            Vector2 offset = new Vector2(OffsetX, OffsetY);

            ColumnsCount = EditorGUILayout.IntField("Columns: ", ColumnsCount);

            ColumnsCount = Mathf.Clamp(ColumnsCount, 0, ColumnsCount);

            offset = EditorGUILayout.Vector2Field("Offset: ", offset);

            AddOffsetFromRenderer = EditorGUILayout.Toggle("SpriteRenderer Bounds: ", AddOffsetFromRenderer);

            OffsetX = offset.x;

            OffsetY = offset.y;

            if (GUILayout.Button("Align selected objects"))
                AlignObjects(Selection.objects as Transform[]);

            if (GUILayout.Button("Align selected children"))
                AlignObjects(Selection.activeObject.GetChildren());
        }

        private static void AlignObjects(Transform[] objects)
        {
            if (ColumnsCount > objects.Length)
                throw new InvalidOperationException(
                    $"Columns count({ColumnsCount}) is more than elements count({objects.Length})");

            Vector2 addOffset = AddOffsetFromRenderer ? objects[0].gameObject.GetBounds() : Vector2.zero;

            addOffset.x += OffsetX;

            addOffset.y += OffsetY;
            
            int linesCount = Mathf.CeilToInt(objects.Length / (float)ColumnsCount);

            float startX = -addOffset.x;

            if (ColumnsCount % 2 != 0)
                startX *= ColumnsCount / 2;
            else
            {
                startX *= ColumnsCount / 2;

                startX += addOffset.x / 2;
            }

            float startY = addOffset.y;

            if (linesCount % 2 != 0)
            {
                startY *= linesCount / 2;
            }
            else
            {
                startY *= linesCount / 2;

                startY -= addOffset.y / 2;
            }

            Vector2 current = new Vector2(startX, startY);

            int xIndex = 0;

            foreach (var @object in objects)
            {
                @object.position = current;

                current.x += addOffset.x;

                xIndex++;

                if (xIndex == ColumnsCount)
                {
                    current.x = startX;

                    current.y -= addOffset.y;

                    xIndex = 0;
                }
            }
        }
    }
}