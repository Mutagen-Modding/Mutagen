<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [What Are Namespaces](#what-are-namespaces)
- [Namespaces, Intellisense, and Compiling](#namespaces-intellisense-and-compiling)
  - [Adding a Namespace](#adding-a-namespace)
    - [Add the Namespace Import Yourself](#add-the-namespace-import-yourself)
    - [Let the IDE add the Namespace](#let-the-ide-add-the-namespace)
  - [Show Types From Other Namespaces in Intellisense](#show-types-from-other-namespaces-in-intellisense)
- [Namespaces Help With Collisions](#namespaces-help-with-collisions)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

# What Are Namespaces
Namespaces are "sections" of code that you can opt to import so that you have access to.  They help organize things and prevent naming collisions.   

They typically might look like:
```cs
// Import the namespace
using Mutagen.Bethesda.Skyrim;

// Now I have access to the SkyrimMod concepts
var skyrimMod = new SkyrimMod("Skyrim.esm", SkyrimRelease.SkyrimSE);
```

# Namespaces, Intellisense, and Compiling
By default, Visual Studio will only show autocomplete for the concepts that exist within the namespaces you have imported.  

If you have an empty file and start typing `SkyrimMod`, it will not autocomplete.  If you type the whole thing out manually, it'll be underlined red saying it does not know what it is.  This is because the namespace is not imported, and so it's not showing you SkyrimMod as an option to use.

## Adding a Namespace
Often you want to add a namespace so that the compiler knows about it, and Intellisense shows it in autocomplete.

### Add the Namespace Import Yourself
You can write it out yourself by adding it to the top of your file:
```cs
using Mutagen.Bethesda.Skyrim;

var skyrimMod = new SkyrimMod("Skyrim.esm", SkyrimRelease.SkyrimSE);
```

This has the downside that you need to know what the namespace that you're interested in is

### Let the IDE add the Namespace
Alternatively, if you know the thing you would like to use, you can write it out, and have the IDE find out what namespace needs to be imported for you.

For example, you could type out `SkyrimMod`, and when it highlights red, do the Visual Studio keybind for suggest fixes (Ctrl + .) and it will suggest adding `Mutagen.Bethesda.Skyrim` to your namespaces.  Select that option, and the IDE will go ahead and do that.

This still has the downside that you need to know what type you are interested in using and type it out by hand.  In our case that was `SkyrimMod`.  This is vulnerable to typos where the IDE wont be able to locate the namespace if it's misspelled, etc.

## Show Types From Other Namespaces in Intellisense
The final option is to allow Intellisense to show in its autocomplete ALL types that exist, not just the ones from namespaces you have imported.   This is often super nice, as you can then just start typing in `SkyrimMod` and it will show it as an autocomplete option.  If you choose to autocomplete it, it will automatically add the necessary namespace, too.

This is by far the easiest usability, but of course comes with the downside of having a more "clogged" Intellisense since it is always showing all the types in existence.   Usually, though, this is not too much of a problem, as the IDE is able to narrow things down and suggest items pretty intelligently.

To turn this on, you need to go into your `Visual Studio` options:

`Visual Studio -> Top Bar -> Tools -> Options -> Text Editor -> C# -> Intellisense -> Show items from unimported namespaces`

![IntellisenseOption](https://user-images.githubusercontent.com/24981326/182715382-b0b3d125-3cd7-4169-9b23-88bde95dca55.png)

`Rider` has this functionality on by default.

# Namespaces Help With Collisions
In addition to "scoping" Intellisense as mentioned above, namespaces also help when you have a legitimate name collision.   Let's say you're using two libraries that both defined `MyClass`, and you want to use both of them in the same code snippet.  How do you do this?
```
var classA = new SomeLibrary.MyClass();
var classB = new OtherLibrary.MyClass();
```
The above snippet shows that you can include the namespace to help specify which `MyClass` you're referring to, and so both can be used side by side, despite the bad naming.