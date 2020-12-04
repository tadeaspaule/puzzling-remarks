public static class FileReader
{
    public static string[,] ReadCSV(string fileString)
    {
        string[] lines = fileString.Split('\n');
        string[] firstLine = lines[0].Split(',');
        string[,] csvContent = new string[lines.Length,firstLine.Length];
        for (int i = 0; i < lines.Length; i++) {
            string[] parts = lines[i].Split(',');
            for (int j = 0; j < parts.Length; j++) {
                csvContent[i,j] = parts[j];
            }
        }
        return csvContent;
    }
}