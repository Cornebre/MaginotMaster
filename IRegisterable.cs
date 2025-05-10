using Nanoray.PluginManager;
using Nickel;

namespace Cornebre.Maginot;

internal interface IRegisterable
{
    static abstract void Register(IPluginPackage<IModManifest> package, IModHelper helper);
}