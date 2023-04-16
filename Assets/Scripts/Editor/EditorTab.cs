public class EditorTab
{
    protected GameEditor ParentEditor;

    public EditorTab(GameEditor editor) // Set the parent editor of child editors
    {
        ParentEditor = editor;
    }

    public virtual void OnTabSelected() // New tab selected on game editor.
    {

    }

    public virtual void Draw() // Draw the selected editor. Will be overriden.
    {

    }
}

