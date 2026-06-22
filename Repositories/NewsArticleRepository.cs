using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetNewsArticles() => NewsArticleDAO.Instance.GetNewsArticles();
        public NewsArticle? GetNewsArticleById(string newsArticleId) => NewsArticleDAO.Instance.GetNewsArticleById(newsArticleId);
        public void SaveNewsArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.Instance.SaveNewsArticle(article, tagIds);
        public void UpdateNewsArticle(NewsArticle article, List<int> tagIds) => NewsArticleDAO.Instance.UpdateNewsArticle(article, tagIds);
        public void DeleteNewsArticle(NewsArticle article) => NewsArticleDAO.Instance.DeleteNewsArticle(article);
        public List<NewsArticle> GetNewsHistoryByCreatorId(short creatorId) => NewsArticleDAO.Instance.GetNewsHistoryByCreatorId(creatorId);
    }
}
