using System.Threading.Tasks;

namespace UploadStreamToQuestDB.API.Hub {
    public interface ILocalesStatusHub
    {
        Task OnStartAsync(string id);
        Task OnFinishAsync(string id);
    }
}