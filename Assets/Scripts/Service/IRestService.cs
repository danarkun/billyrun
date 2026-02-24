using System.Threading.Tasks;

public interface IRestService
{
    Task<bool> SavePlayerProfile(string username, int level);
}