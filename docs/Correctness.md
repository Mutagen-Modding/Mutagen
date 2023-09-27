# Correctness
## Passthrough Testing
Besides general unit tests for edge-case prone code section, Mutagen has a passthrough test suite that it uses to help confirm correctness.  
### Definition
A passthrough means that Mutagen will read in a source file into the classes it provides, and then immediately re-export.  If the two files match, then it was able to "pass through" Mutagen's systems without being changed unexpectedly.

### Pre-Processing
There is a bit more that goes into a passthrough than just importing, re-exporting, and comparing, though.  For one, it is much easier to compare files match byte to byte if you're comparing the uncompressed data.  As such, there are a few pre-processing steps that happen before a passthrough test is actually executed.
#### Decompression
The source file is lightly processed so that all the records are in their decompressed format.
#### Float Standardization
Some float values can be stored in multiple ways.  A common one is zero, which is stored as zero by Bethesda in two different formats.  This stage standardizes floats to be the format Mutagen will export as, while leaving their actual represented values intact.
#### Subrecord Order Standardization
Subrecords usually come in a predictable order, but they are not required to.  Bethesda in some circumstances will deviate and shuffle the subrecords to where the content overall stays the same, but the order they show up on disk is different.  This phase moves the subrecords to be in the order Mutagen would export them in.  This helps the byte to byte comparison line up better without being distracted by subrecords of differing order.
#### Strings File Key Reindexing
Mutagen does not retain strings file key indexes compared to the original.  This is not important information to store and persist so that it matches the input.  As such, the source file needs to be reindexed so that it matches what Mutagen will output.  This just means making the key index start at 1 and increment for every string that was exported.
#### Other Minor Inconsistencies
There are a few other cases of inconsistencies that need to be aligned, similar to the ones above.

### Running
Once a preprocessed file that has been trimmed, aligned, and decompressed is ready, then the actual passthrough will be done.  Mutagen will import the original (unprocessed) file, and re-export it.  The result will be compared to the pre-processed file which then should match byte to byte.  If there is even one byte differing, the passthrough fails, and either Mutagen's code must be fixed, or if it was just a non-important inconsistency as described above, the passthrough systems will be adjusted to compensate.

## Helper UI
There is an internal helper UI to to facilitate selecting which tests to run, as well as digesting the parallel output that results.

![](https://i.imgur.com/YqFdom4.gif)

![](https://i.imgur.com/clKzSP0.gif)

## More References
More documentation to come, but in the meantime, there is some testing information in this video about [Record Generation](https://youtu.be/j8r4p67eKLA).
