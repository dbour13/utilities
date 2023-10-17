namespace VocabFlashCards.MAUI.Blazor.Data;

public class Vocab
{
    public Int32 Id { get; set; }
    public String Kanji { get; set; }
    public String Kana { get; set; }
    public String English { get; set; }
    public Int32 Level { get; set; }
    public Int32 NumKanji { get; set; }
    public Int32 MaxKanjiLevel { get; set; }
}

public class VocabList
{
    public Vocab[] Vocab { get; set; }
}
