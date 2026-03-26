using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubEnabledProvider
{
    bool Used { get; }
}

public sealed class CreationClubEnabledProvider : ICreationClubEnabledProvider
{
    private readonly IGameCategoryContext _category;

    public CreationClubEnabledProvider(
        IGameCategoryContext category)
    {
        _category = category;
    }
        
    public bool Used
    {
        get
        {
            switch (_category.Category)
            {
                case GameCategory.Oblivion:
                case GameCategory.Starfield:
                case GameCategory.Fallout76:
                    return false;
                case GameCategory.Skyrim:
                case GameCategory.Fallout4:
                    return true;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

public record CreationClubEnabledInjection(bool Used) : ICreationClubEnabledProvider;