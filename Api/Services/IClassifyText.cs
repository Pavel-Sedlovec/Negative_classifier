namespace Api.Services
{
    public interface IClassifyText
    {
        int Classify(string text);
        (int label, double confidence) ClassifyWithConfidence(string text);
        List<int> ClassifyBatch(List<string> texts);
    }
}
