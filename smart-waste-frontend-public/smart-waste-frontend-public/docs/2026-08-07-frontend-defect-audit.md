# SmartWaste frontend defect audit — 7 Aug 2026

Triggered by the bin-lock regression (customer report, Campobasso). That bug was a prop
renamed on the parent but not the child, shipped silently. This audit looked for more of the
same class, plus adjacent silent-failure patterns, across `main`.

Method: 3 independent audit lenses (prop mismatches, role gating, dead UI), each candidate
then re-checked by a separate adversarial verifier instructed to default to REFUTED.
Result: **12 candidates, 9 confirmed, 3 refuted.**

## Fixed and shipped in build `smartwaste-web-production-20260807.zip`

| # | File | Defect |
|---|------|--------|
| — | `BinDetails.jsx:527` + `BinDetailsPanel.jsx:26,468` | Bin lock invisible to everyone — parent passed `canLockBin`, child read `isAdmin`. Fixed in `a4085e3`; gate now `role.id !== 5`. |
| — | `ExportModal.jsx`, `services/lookup/index.js` | Export button stuck disabled after an error; no error surfaced. Fixed in `2b9ef02`. |

## Fixed in build `smartwaste-web-production-20260807-v2.zip`

### Permission holes closed

| # | File | Fix | Commit |
|---|------|-----|--------|
| 4 | `App.jsx` `/bins/edit-bin/:id` | Added role guard denying Driver (5), matching `BinDetails.jsx:310`, the page it protects. Also waits for `userData` before deciding. | `d1ae1b5` |
| 5 | `App.jsx` `/routes/add` | Added role guard allowing roles 1–4, matching `RoutesPage.jsx:184` which only shows the "Add route" link to those roles. | `d1ae1b5` |
| 6 | `BinsList.jsx:608,786`, `BinDetails.jsx:310,527` | Negative gates now require `userData` to be loaded before evaluating, so they fail closed instead of open. Removes the flash of Edit/Delete for drivers. `Aside.jsx` already did this correctly. | `da07193` |

### Stuck / crashing UI fixed

| # | File | Fix | Commit |
|---|------|-----|--------|
| 1, 8 | `ForgotPassword.jsx:43,48` | `setLoginError` → `setError`. Verified absent from the built bundle. | `62d34d1` |
| 7 | `EditRoute.jsx:3` | Added `NoResults` to the existing `../../components` barrel import. | `62d34d1` |
| 9 | `PasswordModal.jsx:34` | `onSubmit` is now async, awaits `handleFormSubmit` and resets `setSubmitting(false)` in a `finally`. | `bcbcfb7` |
| 2 | `CustomMarker.jsx:31` | `title={item.title ?? item.name}` — serves both map call sites without touching either parent. | `eb570fd` |
| — | `services/lookup/index.js:60` | **Found during verification, not in the original audit.** `getDashboardData` referenced a block-scoped `data` outside its block on the error path, throwing `ReferenceError` and making the function return `undefined` on any non-OK response. Pre-existing, identical in `4c380f7`. | `bcbcfb7` |

## Still open — needs a product decision

**Technician (role 6) can edit bin master data from the bin details page** (`BinDetails.jsx:310`,
gate is `!== 5`), while the bins list and the sidebar both deny technicians (`!== 5 && !== 6`,
with the comment "don't show if its driver or technician"). Two places say deny, one says allow.

This was deliberately NOT changed — closing it would remove access technicians have in
production today, which is a product call, not a bug fix. The new route guard on
`/bins/edit-bin/:id` mirrors the details page (`!== 5`) so the two stay consistent; if the
decision is to deny technicians, both need changing together.

## Known, not fixed

`src/utils/getEnv.js:2,9` reference `process.env.*` as a `||` fallback, but `vite.config.js`
has no `define` block, so `process` does not exist in the browser. Harmless while the env var
is set (`||` short-circuits), but with a missing `.env` the user gets `process is not defined`
instead of the intended "VITE_SMARTWASTE_BASE_URL not defined in .env file." message.

## Refuted (checked, not real)

- `BinDetailsMap.jsx` — `CustomMarker` navigating to a missing `item.url`
- `System.jsx` — settings page spinning forever on config failure
- A flag that the lock fix "silently widened" permission to technicians — this was
  explicitly requested and approved, not silent.

## Root cause pattern

Four of nine are the identical shape: an identifier renamed or referenced on one side only,
with no compile-time check to catch it. `vite build` does no scope checking and `npm run lint`
is a separate script that is not chained into the build, so `no-undef` errors ship.

Cheapest structural fix: chain `lint` into `build`, or add `eslint --rule no-undef` to CI.
That alone would have caught #1/#8 and #7 before release.
