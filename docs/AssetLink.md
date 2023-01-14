# What is an AssetLink
An AssetLink is a strongly typed object wrapping what is normally a `string` subpath pointing to an asset.

For example, a Skyrim Weapon's Model has a `File` member, which is the `MODL` subrecord, which contains a string relative to the `Data/Meshes` folder.   Rather than exposing this as a `string`, Mutagen exposes this as an `AssetLink<SkyrimModelAssetType>`.

If you're just after the path, you can still get it via an AssetLink's `RawPath` member.  But the AssetLink now comes with a bunch of additional features and convenience

# AssetLink Members
## RawPath
`RawPath` is the string that is present on disk in the record itself as it appears.  This is also the field that you can set when you want to modify an AssetLink to point to a different path.

`AssetLink`s are implicitly convertible to/from `string`, which uses `RawPath` as the string to return.  This is in place to provide some backwards compatibility

## DataRelativePath
`DataRelativePath` is a `get` only member that does some checking/trimming of `RawPath` to try to standardize it to be relative to a game's Data folder.

It will trim a prefixed "Data" folder in the path, while also ensuring the appropriate asset subfolder is present.  Some examples:

| RawPath                        | DataRelativePath          |
|--------------------------------|---------------------------|
| textures/some/subpath.nif      | textures/some/subpath.nif |
| data/textures/some/subpath.nif | textures/some/subpath.nif |
| some/subpath.nif               | textures/some/subpath.nif |

In all examples, the `DataRelativePath` remains the same, as that is what it's attempting to do: keep it standardized to be relative the Data folder, no matter the RawPath's contents.

# AssetTypes - AssetLink's Generic Type
An `AssetLink`'s generic type is the type of asset it relates to.  So, for example, a Skyrim Model path will be of type `AssetLink<SkyrimModelAssetType>`, where `SkyrimModelAssetType` is a meta class containing information about Skyrim Model assets, such as:
- 

# Asset Enumeration
Similar to FormLinks, a mod can enumerate AssetLinks of a desired type.  
