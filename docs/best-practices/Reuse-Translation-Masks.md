# Reuse Translation Masks
[[Translation Masks]] are a powerful toolset that give you fine-grained control over things like a Copy or Equality check.

They typically look like this:
```cs
var bookCopy = book.DeepCopy(new Book.TranslationMask(defaultOn: true)
{
    PickUpSound = false,
    PutDownSound = false,
});
```
This code snippet would copy a book, but skip `PickUpSound` and `PutDownSound` and leave them on their default values.

However, code like the ones above are typically done in loops:
```cs
foreach (var book in books)
{
    var bookCopy = book.DeepCopy(new Book.TranslationMask(defaultOn: true)
    {
        PickUpSound = false,
        PutDownSound = false,
    });
}
```
This code will work, but is slightly wasteful, as it creates a new mask per loop iteration.

A TranslationMask is just instructions for what to copy, so they can be reused.  A better way would look like this:
```cs
var bookCopyMask = new Book.TranslationMask(defaultOn: true)
{
    PickUpSound = false,
    PutDownSound = false,
};
foreach (var book in books)
{
    var bookCopy = book.DeepCopy(bookCopyMask);
}
```
Now the job is a bit more optimized, as the same mask can just be reused for each book.