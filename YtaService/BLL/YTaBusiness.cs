using YtaService.BLL.Interfaces;
using YtaService.DAL;
using YtaService.DAL.Interfaces;
using YtaService.DTO;
using YtaService.Models;

namespace YtaService.BLL
{
    public class YtaBusiness : IYtaBusiness
    {
        private readonly IYtaRepository _repository;

        public YtaBusiness(IYtaRepository repository)
        {
            _repository = repository;
        }

        public List<YTa> Search(YTaSearchDTO model, out long total)
        {
            return _repository.Search(model, out total);
        }

        public bool Create(YTa model)
        {
            return _repository.Create(model);
        }

        public bool Update(YTa model)
        {
            return _repository.Update(model);
        }

        public bool Delete(string id)
        {
            return _repository.Delete(id);
        }

        public YTa GetById(string id)
        {
            return _repository.GetById(id);
        }
        public List<YTa> GetAll()
        {
            return _repository.GetAll();
        }
    }
}