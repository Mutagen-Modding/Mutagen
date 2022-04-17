using System;
using Mutagen.Bethesda.Environments;
using Mutagen.Bethesda.Environments.DI;

namespace Mutagen.Bethesda.Plugins.Order.DI;

public interface ICreationClubEnabledProvider
{
    bool Used { get; }
}

public class CreationClubEnabledProvider : ICreationClubEnabledProvider
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