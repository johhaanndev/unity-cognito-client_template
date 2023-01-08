namespace Assets.Scripts.Utilities
{
    public interface ISaveable
    {
        string ToJson();
        void LoadFromJson(string a_Json);
        string FileNameToUseForData();
    }
}