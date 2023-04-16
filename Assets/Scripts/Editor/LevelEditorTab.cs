using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class LevelEditorTab : EditorTab
{
    //LEVEL DATA
    private Object levelDataObject; // The object which holds our levelInfo.
    private LevelInfo currentLevelInfo; // Scriptable object that holds level info currently selected from EditorGUILayout.ObjectField.
    private readonly Dictionary<string, Texture> objectTextures = new Dictionary<string, Texture>(); // Textures for visualize level design.

    // HELPERS
    private Vector2 scrollPos;
    private int previousTypeCount;
    private const float ButtonSize = 90f;
    private CollectableObject.ObjectType currentObjectType;
    public string[] numbersOfThree = { "3", "6", "9", "12", "15", "18", "21" };
    int selectedIndex = 0;

    public LevelEditorTab(GameEditor editor) : base(editor) // Constructor.
    {
        GetTexturesFromDirectory();
    }

    public override void Draw() // Draw the selected editor tab. overrided.
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        float oldLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = 90;
        GUILayout.Space(15);
        DrawSelectLevelField();

        if (currentLevelInfo != null)
        {
            GUILayout.Space(15);
            GUILayout.BeginHorizontal();
            DrawLevel(); // Draws level via levelInfo object.
            GUILayout.EndHorizontal();
        }

        EditorGUIUtility.labelWidth = oldLabelWidth;
        EditorGUILayout.EndScrollView();

        if (currentLevelInfo != null && GUI.changed)
            EditorUtility.SetDirty(currentLevelInfo); // You can use SetDirty when you want to modify an object without creating an undo entry.
    }

    private void DrawSelectLevelField() // Draws ObjectField for select LevelInfo scriptable obj.
    {
        Object previousLevelData = levelDataObject;
        levelDataObject = EditorGUILayout.ObjectField("Level Info Asset", levelDataObject, typeof(LevelInfo), false, GUILayout.Width(340));
        if (levelDataObject != previousLevelData)
        {
            currentLevelInfo = (LevelInfo)levelDataObject; // Set current levelInfo.
        }
    }

    private void DrawLevel()
    {
        GUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(500));
        SetTitle();
        HowToUse();
        SetLevelNumber();
        SetTypeCount();
        SetLevelTime();
        SelectObjectType();
        SetObjectOnLevel();
        FillObjectsOnLevel();
        GUILayout.EndVertical();
    }

    public T CreateInstance<T>() where T : ScriptableObject // Generic
    {
        T instance = ScriptableObject.CreateInstance<T>();
        instance.hideFlags = HideFlags.HideInHierarchy;
        if (instance != null && !AssetDatabase.IsSubAsset(instance))
            AssetDatabase.AddObjectToAsset(instance, currentLevelInfo); // Add produced instance to levelData
        return instance;
    }
    public void SetLevelNumber() // Which level are we editting.
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Level number", "The number of this level."), // Level number input field.
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevelInfo.LevelNumber = EditorGUILayout.IntField(currentLevelInfo.LevelNumber, GUILayout.Width(30));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
        previousTypeCount = currentLevelInfo.TypeCount;
    }
    public void SetTypeCount() // How many kinds of objects will be used in this level.
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Type Count", "The number of object types the level contains."), // Type count input field.
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevelInfo.TypeCount = EditorGUILayout.IntField(currentLevelInfo.TypeCount, GUILayout.Width(30));
        GUILayout.EndHorizontal();
    }
    public void SetLevelTime() // Time given to finish the level.
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Time to Finish", "Time to complete the level."), // Level timer input field.
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentLevelInfo.TimeToFinish = EditorGUILayout.FloatField(currentLevelInfo.TimeToFinish, GUILayout.Width(30));
        GUILayout.EndHorizontal();
    }
    public void SelectObjectType() // The empty boxes that appear after entering the type number will be filled with the type selected here when box clicked.
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Object Type", "The type of object to fill the empty box."), // ObjectType enumpopup.
            GUILayout.Width(EditorGUIUtility.labelWidth));
        currentObjectType = (CollectableObject.ObjectType)EditorGUILayout.EnumPopup(currentObjectType, GUILayout.Width(100));
        GUILayout.EndHorizontal();
        GUILayout.Space(10);
    }
    public void HowToUse() // How to use this Level Editor.
    {
        GUILayout.BeginHorizontal(GUILayout.Width(600)); 
        EditorGUILayout.HelpBox(
            "Nasıl kullanılır?" + "\n" + "İlk olarak düzenlenen assetin hangi level'a ait olduğunu belirlemek için Level Number'a değer girin." +
            " Sonrasında bu levelda kaç çeşit toplanabilir obje olacağını belirleyin ve bu değeri Type Count'a girin." +
            "Type Count'a girdiğiniz değer kadar boş kutu oluşacaktır. Bu kutuları hangi objelerle doldurmak istiyorsanız Object Type kısmından o objeyi seçin." +
            "Boş kutulara tıkladığınızda seçmiş olduğunuz obje ile doldurulacaktır. Doldurulan kutunun sol tarafında objenin sahnede kaç adet olacağını seçin." +
            " Hepsi bu kadar. Seçtiğiniz objelere ve sayılara göre level otomatik oluşturulacak.",
            MessageType.Info);
        GUILayout.EndHorizontal();
    }
    public void SetTitle()
    {
        GUIStyle style = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };
        EditorGUILayout.LabelField("Level Design", style);
        GUILayout.Space(10);
    }
    public void SetObjectOnLevel() // Create empty places for Collectable objects.
    {
        if (currentLevelInfo.TypeCount != previousTypeCount)
        {
            if (currentLevelInfo.ObjectsOnLevel != null)
            {
                foreach (CollectableObject c_Object in currentLevelInfo.ObjectsOnLevel)
                    Object.DestroyImmediate(c_Object, true);
            }

            if (currentLevelInfo.TypeCount > 0)
            {
                currentLevelInfo.ObjectsOnLevel = new List<CollectableObject>();
                currentLevelInfo.ObjectsOnLevel.AddRange(Enumerable.Repeat<CollectableObject>(null, currentLevelInfo.TypeCount)); // Add empty object to fill later
            }
        }
    }
    public void FillObjectsOnLevel()
    {
        if (currentLevelInfo.ObjectsOnLevel != null)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < currentLevelInfo.ObjectsOnLevel.Count; i++)
            {
                CreateButton(i); // create button for every object type.
            }
            GUILayout.EndHorizontal();
        }
    }
    private void CreateButton(int index) // The created buttons will be filled with the selected object type when clicked.
    {
        string objectTypeName = string.Empty;
        if (currentLevelInfo.ObjectsOnLevel.Count == 0) return;
        CollectableObject obj = currentLevelInfo.ObjectsOnLevel[index];
        if (obj != null)
        {
            objectTypeName = obj.objectType.ToString();
            SelectObjectCount(obj);
            DrawObjectTexture(objectTypeName,index);
        }
        else
        {
            DrawEmptyBox(index);
        }
    }
    private void SelectObjectCount(CollectableObject obj) // Select count for object.
    {
        GUILayout.BeginVertical(EditorStyles.helpBox); 
        EditorGUILayout.LabelField(new GUIContent("Object" + "\n" + "Count", "Count of objects to be placed on Game Area"),
            GUILayout.Width(EditorGUIUtility.labelWidth), GUILayout.Height(60));
        int kk = System.Array.IndexOf(numbersOfThree, obj.objectCount.ToString());
        selectedIndex = (int)EditorGUILayout.Popup(kk, numbersOfThree, GUILayout.Width(100)); // Object Count input field.
        obj.objectCount = System.Int32.Parse(numbersOfThree[selectedIndex]);
        GUILayout.EndVertical();
    }
    private void DrawObjectTexture(string objectTypeName,int index) // Draw object texture into the box.
    {
        if (GUILayout.Button(objectTextures[objectTypeName], GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize))) // Draw object by type.
        {
            Object.DestroyImmediate(currentLevelInfo.ObjectsOnLevel[index], true);
            currentLevelInfo.ObjectsOnLevel[index] = CreateInstance<CollectableObject>();
            currentLevelInfo.ObjectsOnLevel[index].objectType = currentObjectType;
        }
    }
    private void DrawEmptyBox(int index) // Draw empty box. These empty boxes can then be filled by clicking.
    {
        if (GUILayout.Button("", GUILayout.Width(ButtonSize), GUILayout.Height(ButtonSize))) 
        {
            Object.DestroyImmediate(currentLevelInfo.ObjectsOnLevel[index], true);
            currentLevelInfo.ObjectsOnLevel[index] = CreateInstance<CollectableObject>();
            currentLevelInfo.ObjectsOnLevel[index].objectType = currentObjectType;
        }
    }
    public void GetTexturesFromDirectory() // Get object textures to visualize level design.
    {
        DirectoryInfo editorImagesPath = new DirectoryInfo(Application.dataPath + "/Resources/Editor"); // set directory path.
        FileInfo[] fileInfo = editorImagesPath.GetFiles("*.png", SearchOption.TopDirectoryOnly); // get png files.
        foreach (FileInfo file in fileInfo)
        {
            string filename = Path.GetFileNameWithoutExtension(file.Name);
            objectTextures[filename] = FileUtils.LoadTexture(filename);// add pngs to dictionary.
        }
    }
}

