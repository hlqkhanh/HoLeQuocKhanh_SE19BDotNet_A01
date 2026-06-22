using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticleService()
        {
            _repository = new NewsArticleRepository();
        }

        public NewsArticleService(INewsArticleRepository repository)
        {
            _repository = repository;
        }

        public List<NewsArticle> GetNewsArticles() => _repository.GetNewsArticles();
        public NewsArticle? GetNewsArticleById(string newsArticleId) => _repository.GetNewsArticleById(newsArticleId);
        public void SaveNewsArticle(NewsArticle article, List<int> tagIds) => _repository.SaveNewsArticle(article, tagIds);
        public void UpdateNewsArticle(NewsArticle article, List<int> tagIds) => _repository.UpdateNewsArticle(article, tagIds);
        public void DeleteNewsArticle(NewsArticle article) => _repository.DeleteNewsArticle(article);
        public List<NewsArticle> GetNewsHistoryByCreatorId(short creatorId) => _repository.GetNewsHistoryByCreatorId(creatorId);
    }
}
