using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Directory = System.IO.Directory;

namespace Istu.Navigation.Domain.Services;

public interface ILuceneService
{
    List<SearchResult> Search(string searchText, int limit = 10);
    void AddDocument(Guid id, ContentType contentType, string title, string keywords, string description);
    void DeleteDocument(Guid id);
}

public class LuceneService : ILuceneService
{
    private readonly ILogger<LuceneService> logger;
    private readonly string indexPath;
    private const LuceneVersion LuceneVersion = Lucene.Net.Util.LuceneVersion.LUCENE_48;

    // Field names
    private const string TitleFieldName = "Title";
    private const string IdFieldName = "Id";
    private const string TypeFieldName = "Type";
    private const string DescriptionFieldName = "Description";
    private const string KeywordsFieldName = "Keywords";

    public LuceneService(IWebHostEnvironment env,ILogger<LuceneService> logger)
    {
        this.logger = logger;
        indexPath = Path.Combine(env.ContentRootPath, "LuceneIndex");
        if (!Directory.Exists(indexPath))
            Directory.CreateDirectory(indexPath);
    }

    public List<SearchResult> Search(string searchText, int limit = 10)
    {
        var results = new List<SearchResult>();
        try
        {
            using var dir = FSDirectory.Open(new DirectoryInfo(indexPath));
            using var indexReader = DirectoryReader.Open(dir);
            var searcher = new IndexSearcher(indexReader);
            var query = BuildQuery(searchText);
            var hits = searcher.Search(query, limit).ScoreDocs;

            foreach (var hit in hits)
            {
                var doc = searcher.Doc(hit.Doc);
                var result = GetResultFromDocument(doc);
                if (result != null)
                    results.Add(result);
            }
        }
        catch (IndexNotFoundException e)
        {
            logger.LogWarning($"Lucene index not found, exception: {e}");
        }

        return results;
    }

    public void AddDocument(Guid id, ContentType contentType, string title, string keywords, string description)
    {
        using var dir = FSDirectory.Open(new DirectoryInfo(indexPath));
        using var analyzer = new StandardAnalyzer(LuceneVersion);
        using var writer = new IndexWriter(dir, new IndexWriterConfig(LuceneVersion, analyzer));

        var doc = new Document
        {
            new StringField(IdFieldName, id.ToString(), Field.Store.YES),
            new StringField(TypeFieldName, contentType.ToString(), Field.Store.YES),
            new TextField(TitleFieldName, title, Field.Store.YES),
            new TextField(KeywordsFieldName, keywords, Field.Store.YES),
            new TextField(DescriptionFieldName, description, Field.Store.YES)
        };

        writer.AddDocument(doc);
        writer.Commit();
    }
    
    public void DeleteDocument(Guid id)
    {
        using var dir = FSDirectory.Open(new DirectoryInfo(indexPath));
        using var writer = new IndexWriter(dir, new IndexWriterConfig(LuceneVersion, new StandardAnalyzer(LuceneVersion)));
        
        var query = new TermQuery(new Term(IdFieldName, id.ToString()));
                
        writer.DeleteDocuments(query);
        writer.Commit();
    }

    private Query BuildQuery(string searchText)
    {
        using var analyzer = new StandardAnalyzer(LuceneVersion);
        var parser = new MultiFieldQueryParser(LuceneVersion, new[] { TitleFieldName, DescriptionFieldName, KeywordsFieldName }, analyzer);
        return parser.Parse(searchText);
    }

    private SearchResult? GetResultFromDocument(Document doc)
    {
        var id = doc.Get(IdFieldName);
        var title = doc.Get(TitleFieldName);
        var type = doc.Get(TypeFieldName);

        if (!Guid.TryParse(id, out var guidId) || !Enum.TryParse(type, true, out ContentType contentType))
            return null;
        

        return new SearchResult(guidId, title, contentType);
    }
}

public class SearchResult(Guid id, string title, ContentType type)
{
    public Guid Id { get; set; } = id;
    public ContentType Type { get; set; } = type;

    public string Title { get; set; } = title;
}
public enum ContentType
{
    Building = 0,
    Object = 1
}
