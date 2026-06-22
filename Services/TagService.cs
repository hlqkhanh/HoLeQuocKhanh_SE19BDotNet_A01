using System.Collections.Generic;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;

        public TagService()
        {
            _repository = new TagRepository();
        }

        public TagService(ITagRepository repository)
        {
            _repository = repository;
        }

        public List<Tag> GetTags() => _repository.GetTags();
        public Tag? GetTagById(int tagId) => _repository.GetTagById(tagId);
        public void SaveTag(Tag tag) => _repository.SaveTag(tag);
        public void UpdateTag(Tag tag) => _repository.UpdateTag(tag);
        public void DeleteTag(Tag tag) => _repository.DeleteTag(tag);
    }
}
