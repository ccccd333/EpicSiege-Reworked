# Epic Siege (Continued) – Fix & Performance Update

This repository provides bug fixes and performance improvements for:

Epic Siege (Continued)  
https://steamcommunity.com/sharedfiles/filedetails/?id=3599382443

Tested on RimWorld 1.6 only.

---

## Bug Fix

In the original Epic Siege (Continued), destroying the second site also triggers logic that destroys a third site.

This was implemented via a Harmony patch targeting:

Site.Notify_MyMapAboutToBeRemoved

Because both:
- the current player map site, and  
- the third site  

were being destroyed simultaneously, the destroy logic executed twice (double destruction).

This caused an array out-of-range error:
Error while deiniting map: could not notify things/regions/rooms/etc:


This issue has been fixed by correcting the site destruction flow and preventing duplicate destruction calls.

---

## Performance Improvements

The original implementation relied on:

Find.WorldObjects

for locating target sites.

This has been refactored to:

- Use `WorldObjectComp`
- Use classes inheriting from `Site`
- Manage destruction targets directly via components

As a result:

- No global `Find` scanning is required
- Destruction targets are managed deterministically
- Reduced overhead and improved performance

---

## Harmony Removal

The previous Harmony patch on:

Site.Notify_MyMapAboutToBeRemoved

has been removed.

The same behavior is now implemented without Harmony, using a cleaner component-based approach.
