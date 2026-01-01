using YtaService.DTO;    
using YtaService.Models;  

namespace YtaService.BLL.Interfaces
{
    public interface IYtaBusiness
    {
        List<YTa> Search(YTaSearchDTO model, out long total);

        YTa GetById(string id);
        bool Create(YTa model);
        bool Update(YTa model);
        bool Delete(string id);

        List<YTa> GetAll();
    }
}