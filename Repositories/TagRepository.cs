using System.Collections.Generic;
using BusinessObjects;
using DataAccessObjects;

namespace Repositories
{
    public class TagRepository : ITagRepository
    {
        public List<Tag> GetTags() => TagDAO.Instance.GetTags();
        public Tag? GetTagById(int tagId) => TagDAO.Instance.GetTagById(tagId);
        public void SaveTag(Tag tag) => TagDAO.Instance.SaveTag(tag);
        public void UpdateTag(Tag tag) => TagDAO.Instance.UpdateTag(tag);
        public void DeleteTag(Tag tag) => TagDAO.Instance.DeleteTag(tag);
    }
}
