using System.Collections.Generic;
using BusinessObjects;

namespace Services
{
    public interface INewsArticleService
    {
        List<NewsArticle> GetNewsArticles();
        NewsArticle? GetNewsArticleById(string newsArticleId);
        void SaveNewsArticle(NewsArticle article, List<int> tagIds);
        void UpdateNewsArticle(NewsArticle article, List<int> tagIds);
        void DeleteNewsArticle(NewsArticle article);
        List<NewsArticle> GetNewsHistoryByCreatorId(short creatorId);
    }
}
