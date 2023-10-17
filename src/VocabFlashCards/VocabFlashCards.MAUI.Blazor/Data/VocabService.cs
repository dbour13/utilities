using System.IO;
using System.Text.Json;

namespace VocabFlashCards.MAUI.Blazor.Data;

public class VocabService
{
	private static Vocab[] VocabList = null;

	public async Task<Vocab[]> GetVocab()
	{
		if (VocabList == null)
		{
            using (var stream = await FileSystem.OpenAppPackageFileAsync("Vocab.json"))
            using (var reader = new StreamReader(stream))
            {
                string contents = await reader.ReadToEndAsync();
                VocabList = JsonSerializer.Deserialize<VocabList>(contents).Vocab;
            }
        }

        return VocabList;
    }
}