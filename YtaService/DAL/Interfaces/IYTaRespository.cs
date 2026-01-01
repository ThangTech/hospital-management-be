using YtaService.DTO;
using YtaService.Models;

namespace YtaService.DAL.Interfaces
{
    public interface IYtaRepository
    {
        List<YTa> Search(YTaSearchDTO model, out long total);
        YTa GetById(string id);
        bool Create(YTa model);
        bool Update(YTa model);
        bool Delete(string id);
        // Thêm dòng này vào trong interface
        List<YTa> GetAll();
    }
}
