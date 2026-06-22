using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataAccessObjects
{
    public class TagDAO
    {
        private static TagDAO? _instance = null;
        private static readonly object _instanceLock = new object();

        private TagDAO() { }

        public static TagDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new TagDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<Tag> GetTags()
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.Tags.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTags: " + ex.Message);
            }
        }

        public Tag? GetTagById(int tagId)
        {
            try
            {
                using var context = new FunewsManagementContext();
                return context.Tags.FirstOrDefault(t => t.TagId == tagId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error in GetTagById: " + ex.Message);
            }
        }

        public void SaveTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Tags.Add(tag);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in SaveTag: " + ex.Message);
            }
        }

        public void UpdateTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementContext();
                context.Entry(tag).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error in UpdateTag: " + ex.Message);
            }
        }

        public void DeleteTag(Tag tag)
        {
            try
            {
                using var context = new FunewsManagementContext();
                var tg = context.Tags.FirstOrDefault(t => t.TagId == tag.TagId);
                if (tg != null)
                {
                    context.Tags.Remove(tg);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error in DeleteTag: " + ex.Message);
            }
        }
    }
}
