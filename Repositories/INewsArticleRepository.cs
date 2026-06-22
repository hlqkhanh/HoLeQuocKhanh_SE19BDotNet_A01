using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetNewsArticles();
        NewsArticle? GetNewsArticleById(string newsArticleId);
        void SaveNewsArticle(NewsArticle article, List<int> tagIds);
        void UpdateNewsArticle(NewsArticle article, List<int> tagIds);
        void DeleteNewsArticle(NewsArticle article);
        List<NewsArticle> GetNewsHistoryByCreatorId(short creatorId);
    }
}
