<!-- START doctoc generated TOC please keep comment here to allow auto update -->
<!-- DON'T EDIT THIS SECTION, INSTEAD RE-RUN doctoc TO UPDATE -->
## Table Of Contents

- [Perk Effect Types](#perk-effect-types)
- [Perk Entry Point Effect Types](#perk-entry-point-effect-types)

<!-- END doctoc generated TOC please keep comment here to allow auto update -->

## Perk Effect Types
The abstract base class is `APerkEffect` which is inherited by:
- `PerkQuestEffect`
- `PerkAbilityEffect`
- `APerkEntryPointEffect`

## Perk Entry Point Effect Types
The abstract base class is `APerkEntryPointEffect` which is inherited by:
- `PerkModifyValue`
- `PerkAddRangeToValue`
- `PerkModifyActorValue`
- `PerkAbsoluteValue`
- `PerkAddLeveledItem`
- `PerkAddActivateChoice`
- `PerkSelectSpell`
- `PerkSelectText`
- `PerkSetText`