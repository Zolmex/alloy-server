namespace Common.Resources.Xml.Descriptors;

public class QuestDesc
{
    public readonly int Level;
    public readonly int Priority;

    public QuestDesc(int level, int priority)
    {
        Level = level;
        Priority = priority;
    }
}