Documentation WIP ~ ! ! !

I will document the helpers soon, but feel free to browse the code yourself.

This serves to introduce several helpers to simplify api calls and streamline creation of objects.

## Setup
First, clone this repository to your workspace. Make sure you update your Game Directory in the project file. It is by default set to the linux location, but you can get your location by right-clicking your game in Steam and going to (Manage > Browse Local Files). 

```csproj
<!-- This is the Linux Path. Update if Windows User -->
<GameDir>$(HOME)/.local/share/Steam/steamapps/common/Probably Stolen/</GameDir>
```

Then, create your own project in the same workspace. Use [Modding Guide](https://github.com/dragonpan2/probably-stolen-modding/blob/main/ModdingGuide.md#dev-tools) as a reference to how to make a new project. Once you add it, make sure to add probably_helpful as a prerequisite.

```xml
<Prerequisites>
  <Mod>probably_helpful</Mod>
</Prerequisites>
```

## Item Collection

This handles all the functions of registering your items

```csharp
public class ProbablyContainersPlus : IMod
{
    public ItemCollection Items;
    public void Init(ModManifest manifest)
    {
        InitItems();
    }

    public void InitItems()
    {
        Items = new ItemCollection(MOD_ID)
            .AddItems(
                // Create Instances of ProbablyHelpfulItem here
            );
    }
    public void OnEnable()
    {
        ModHook.OnModItemDirectoryInit += RegisterItems;
    }
    public void OnDisable()
    {
        ModHook.OnModItemDirectoryInit -= RegisterItems;
    }

    public void RegisterItems(ModItemDirectory directory)
    {
        // Registers all items in ItemColleciton
        Items.RegisterItems(directory);
    }
}
```

## Item Helper
To make a new item, create a class that inheirets ProbablyHelpfulItem

```csharp
using ProbablyHelpful;

class TemplateItem : ProbablyHelpfulItem
{
    public TemplateItem() : base("mod_id", "template_item")
    {
        // Place initialization methods here!
    }
}
```
Note, that the system combines mod_id and item_id into a single key. So the real item id of the above example is ``mod_id.template_item``.

After that, you can set any initialization method to configure your item.

#### SetName, SetDescription, SetLocalizedNameAndDescription
This sets the initial name and description of an item. If you decide to use localization csv, you can instead use ``test_mod.template_item_name`` and ``test_mod.template_item_description``. Appending ``_name`` and ``_description``
```csharp
SetName(string)
SetDescription(string)
SetLocalizedNameAndDescription()
```

#### SetValue
Sets the base value of an item.
```csharp
SetValue(int)
```

#### SetSolidShape
Instead of relying on the middle transparent pixel to set the items grid slot marker, use this to set it to a solid item with dimensions ``width`` and ``height``. **TODO**: Set Nonsolid Shape (for non rectangular objects).
```csharp
SetValue(int)
```

#### SetSprite
(Optional) Sets the sprite of your item. If not used, it is defaulted to ``mod_id.template_item``
```csharp
SetSprite(string)
```

#### OnSelfActivate
Determines what happens if you activate an item. Can be done either by double-click or context menu. Double-click may be replace by other actions, such as opening a container, if applicable.
```csharp
OnSelfActivate(Action<GameItem>)
OnSelfActivate(Action<GameItem, GameInventory>)
OnSelfActivate(Action<GameItem, GameInventory, SlotMarker>)
```

#### CanSelfActivate
Checks if can activate an item. May be automated with certain conditions, such as being a readable document. Can pass in a function for dynamic condition, or a bool if alway/never activatable.
```csharp
CanSelfActivate(bool)
CanSelfActivate(Func<GameItem, GameInventory, SlotMarker, bool>)
```

#### SetItemActivateOnTarget
First GameItem is item being dragged, second is the target. Can validate by specifying if dragging item is a string item_id, or by a function. Then, specify if you can execute, then the action when executing.
```csharp
SetItemActivateOnTarget(string, Func<GameItem, GameItem, bool>, Action<GameItem, GameItem>)
SetItemActivateOnTarget(Func<GameItem, GameItem, bool>, Func<GameItem, GameItem, bool>, Action<GameItem, GameItem>)
```

## Containers
YES, you can make your own containers. Just add the interface ``IProbablyHelpfulContainer`` to your item.
```csharp
using ProbablyHelpful;

class TemplateItem : ProbablyHelpfulItem, IProbablyHelpfulContainer
{
    public TemplateItem() : base("mod_id", "template_item") {}

    public void CustomizeContainer(GameItem item) {
        IProbablyHelpfulContainer containerItem = this;
        // Place container definitions here.
    }
}
```

**Note** because these methods come from the interface, you cannot call it from the ProbablyHelpfulItem context. Instead, we create a IProbablyHelpfulContainer context to invoke.

```csharp
// NOT THIS
CreateContainer(item, 4, 9);
// THIS
IProbablyHelpfulContainer containerItem = this;
GameInventory inventory = containerItem.CreateContainer(item, 4, 9);
```

#### CreateContainer
This sets up a container for item, with inventory width and height. If you set bool=true, it will allow all items by default. Otherwise, it will deny items unless whitelisted by its id or tag below.
```csharp
CreateContainer(int,int,bool?)
```

#### ContainerAllowedItems/ContainerAllowedTags
If you did not set ``allowAllItem == true`` in CreateContainer, this will whitelist the item_id's or item tags allowed in the container.
```csharp
ContainerAllowedItems(string[])
ContainerAllowedTags(string[])
```

#### CreateMultiContainer
Multiple inventory windows with differing conditions. Supply width and height, with optional allowed Tags and Ids. Unlike the normal ``CreateContainer``, the whitelist must be defined now, or ommited to allow all items.
```csharp
CreateMultiContainer((int,int)[])
CreateMultiContainer((int,int,List<string>,List<string>)[])
CreateMultiContainerVertical((int,int)[])
CreateMultiContainerVertical((int,int,List<string>,List<string>)[])
```

#### ContainerOpenSound/ContainerCloseSound
Provide an ``AudioHelper`` sound id, or supply your own, to play sound when opening/closing container.
```csharp
ContainerOpenSound(string)
ContainerCloseSound(string)
```

#### SetAs...
This sets the item to the respective tags.
**Note**: Precontainer allows your container to be stored in other containers (including backpacks) and is only recommended if using a whitelist. Allowing all items inside a precontainer means you can infinately nest items.
```csharp
SetAsPouch()
SetAsBackpack()
SetAsPrecontainer()
StorableInBackpack()
StorableInContainers()
```

#### ModifySpriteFromInventory
If ``GameInventory`` is passed, it will base on that inventory. Otherwise, it will use default inventory. You can then pass in a function that set sprites on the container, allowing it to look different based on what items are inside it.
**ALWAYS PASS IN ``GameInventory`` WHEN CONTAINER IS MULTICONTAINER**
```csharp
ModifySpriteFromInventory(Func<GameInventory, List<ItemSpriteModifier>>)
ModifySpriteFromInventory(GameInventory,Func<GameInventory, List<ItemSpriteModifier>>)
```

## Inspections
YES, you can easily make an item viewable by magnifying glass. Just add the interface ``IProbablyHelpfulInspectionItem`` to your item.
```csharp
using ProbablyHelpful;

class TemplateItem : ProbablyHelpfulItem, IProbablyHelpfulInspectionItem
{
    public TemplateItem() : base("mod_id", "template_item") {}

    public void CustomizeInspection() {
        IProbablyHelpfulInspectionItem inspectionItem = this;
        // Setup Methods
    }
}
```

**Note** because these methods come from the interface, you cannot call it from the ProbablyHelpfulItem context. Instead, we create a IProbablyHelpfulInspectionItem context to invoke.

### CustomizeInspection

Here is a one time configuration. Unlike most helper configurations, this will only trigger a single time, when the item template is registered, rather than for each item.

#### AddSprite
First string is group name. Only a single sprite of a group is visible at a time.
Second string is your mod_id.
Third string is your sprite key.
Then, optionally pass in x,y offset. Sprite is centered on the tray at 0,0 by default.

```csharp
AddSprite(string, string, string, int?, int?)
```

#### SetSpriteGroup
This passes in a sprite group key and an index. The indexes of sprites in sprite group are automatially set by AddSprite. (ie. The first sprite of a group is index 0, then the second is index 1.) You can pass in ``-1`` to hide all sprites in the group.
```csharp
SetSpriteGroup(string, int)
```