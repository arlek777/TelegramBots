using System.Threading.Tasks;

namespace InstagramHelper.Core.Services.Interfaces;

public interface IHashTagsCaptionsService
{
    Task<string[]> GetHashTags(string[] keywords, int totalHashTagsCount);

    Task<string> GetCaption(string keyword);
}