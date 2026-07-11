using ProjectArchive.Models;

namespace ProjectArchive.Services;

public interface ISettingsStorageService
{
    AppSettings Load();

    void Save(AppSettings settings);
}
