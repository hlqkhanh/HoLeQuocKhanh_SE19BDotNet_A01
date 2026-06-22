using System.Collections.Generic;
using BusinessObjects;

namespace Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetTags();
        Tag? GetTagById(int tagId);
        void SaveTag(Tag tag);
        void UpdateTag(Tag tag);
        void DeleteTag(Tag tag);
    }
}
