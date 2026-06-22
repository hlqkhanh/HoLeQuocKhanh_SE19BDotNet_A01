using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NewsArticleDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<NewsArticle> GetNewsArticles()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.NewsArticles
                    .Include(a => a.Category)
                    .Include(a => a.CreatedBy)
                    .Include(a => a.Tags)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticles: " + ex.Message);
            }
        }

        public NewsArticle? GetNewsArticleById(string newsArticleId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.NewsArticles
                    .Include(a => a.Category)
                    .Include(a => a.CreatedBy)
                    .Include(a => a.Tags)
                    .FirstOrDefault(a => a.NewsArticleId == newsArticleId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsArticleById: " + ex.Message);
            }
        }

        public void SaveNewsArticle(NewsArticle article, List<int> tagIds)
        {
            try
            {
                using var context = new FunewsManagementContext();
                
                // Fetch the tags from database
                if (tagIds != null && tagIds.Any())
                {
                    var tags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                    foreach (var tag in tags)
                    {
                        article.Tags.Add(tag);
                    }
                }

                // If Category is attached, prevent EF from trying to recreate it
                if (article.CategoryId.HasValue)
                {
                    context.Entry(article).State = EntityState.Added;
                }

                context.NewsArticles.Add(article);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveNewsArticle: " + ex.Message);
            }
        }

        public void UpdateNewsArticle(NewsArticle article, List<int> tagIds)
        {
            try
            {
                using var context = new FunewsManagementContext();
                
                // Retrieve the existing article along with its current tags
                var existingArticle = context.NewsArticles
                    .Include(a => a.Tags)
                    .FirstOrDefault(a => a.NewsArticleId == article.NewsArticleId);

                if (existingArticle != null)
                {
                    // Update property values
                    existingArticle.NewsTitle = article.NewsTitle;
                    existingArticle.Headline = article.Headline;
                    existingArticle.NewsContent = article.NewsContent;
                    existingArticle.NewsSource = article.NewsSource;
                    existingArticle.CategoryId = article.CategoryId;
                    existingArticle.NewsStatus = article.NewsStatus;
                    existingArticle.UpdatedById = article.UpdatedById;
                    existingArticle.ModifiedDate = article.ModifiedDate;
                    existingArticle.NewsImage = article.NewsImage;

                    // Clear existing tags
                    existingArticle.Tags.Clear();

                    // Load and assign new tags
                    if (tagIds != null && tagIds.Any())
                    {
                        var newTags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                        foreach (var tag in newTags)
                        {
                            existingArticle.Tags.Add(tag);
                        }
                    }

                    context.SaveChanges();
                }
                else
                {
                    throw new Exception("News article not found for update.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateNewsArticle: " + ex.Message);
            }
        }

        public void DeleteNewsArticle(NewsArticle article)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var art = context.NewsArticles
                    .Include(a => a.Tags) // also delete relationship
                    .FirstOrDefault(a => a.NewsArticleId == article.NewsArticleId);
                if (art != null)
                {
                    art.Tags.Clear(); // clear relationship references
                    context.NewsArticles.Remove(art);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteNewsArticle: " + ex.Message);
            }
        }

        public List<NewsArticle> GetNewsHistoryByCreatorId(short creatorId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.NewsArticles
                    .Include(a => a.Category)
                    .Include(a => a.CreatedBy)
                    .Include(a => a.Tags)
                    .Where(a => a.CreatedById == creatorId)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetNewsHistoryByCreatorId: " + ex.Message);
            }
        }
    }
}
