# CODE REVIEW CHECKLIST

Use this checklist on every pull request before requesting a review, and when reviewing someone else's code. Work through each section top to bottom. A PR is ready to merge only when every applicable item is checked.


## 1. Correctness

- [ ] The code does what the task or ticket describes.
- [ ] Edge cases are handled (nulls, empty collections, out-of-range values, concurrent access).
- [ ] No logic is silently swallowed — errors and unexpected states are either handled or propagated.
- [ ] No existing tests are broken.
- [ ] New behaviour is covered by new or updated tests.


## 2. Readability

- [ ] Names of classes, methods, and variables clearly describe their purpose — no abbreviations, no single-letter names outside of short loops.
- [ ] Methods do one thing. If a method needs a comment to explain what each block does, it should be split into smaller methods.
- [ ] Comments explain *why*, not *what*. Code that needs a comment to explain what it does should be rewritten to be self-explanatory.
- [ ] No dead code, commented-out blocks, or leftover debug statements.
- [ ] TODOs left in the code reference a ticket or issue number.


## 3. File Formatting (Rules 1–5)

- [ ] Indentation uses spaces, 4 per level — no tab characters anywhere.
- [ ] Every file ends with a newline.
- [ ] Line endings are CRLF throughout.
- [ ] No trailing whitespace on any line.
- [ ] All files are UTF-8 encoded.


## 4. Naming Conventions (Rules 6–10)

- [ ] Classes, structs, enums, methods, properties, events, and delegates are in `PascalCase`.
- [ ] Interfaces start with `I` followed by `PascalCase` — e.g. `IOrderRepository`.
- [ ] Static fields use the `s_` prefix — e.g. `s_instance`.
- [ ] Private and private-protected instance fields use the `_` prefix — e.g. `_userId`.
- [ ] Parameters and local variables use plain `camelCase` with no prefix.


## 5. Types and var (Rules 11–12)

- [ ] `var` is not used anywhere — all types are written out explicitly.
- [ ] C# language keywords are used instead of BCL names everywhere (`int` not `Int32`, `string` not `String`, `bool` not `Boolean`).


## 6. Null Handling and Expressions (Rules 13–16)

- [ ] Null fallback assignments use `??` or `??=`, not manual `if (x == null)` blocks.
- [ ] Member access on nullable values uses `?.`, not nested null-guard `if` statements.
- [ ] Type checks use pattern matching (`is Type variable`, `is not null`) rather than `is` + separate cast.
- [ ] Simple conditional assignments and returns use a ternary expression, not an `if/else` block.


## 7. Collections and Object Initialization (Rules 17–18)

- [ ] Collections are initialized inline using initializer syntax or collection expressions — no `Add()` calls after an empty constructor.
- [ ] Objects are initialized inline using object initializer syntax — no property assignments on separate lines after construction.


## 8. Member Declarations (Rules 19–22)

- [ ] Every non-interface member has an explicit access modifier — no implicit `private`.
- [ ] Fields that are never reassigned after construction are marked `readonly`.
- [ ] Expression bodies (`=>`) are only used on properties, accessors, and indexers — not on methods or constructors.
- [ ] Local functions and lambdas that capture no instance state are declared `static`.


## 9. Code Structure and Layout (Rules 23–24)

- [ ] All control-flow blocks (`if`, `else`, `for`, `foreach`, `while`, `do`, `using`) use curly braces — no braceless single-line bodies.
- [ ] Opening braces are on their own line (Allman style). `else`, `catch`, and `finally` each start on a new line.


## 10. Design and Architecture

- [ ] No logic is duplicated — shared behaviour is extracted into a method, class, or utility.
- [ ] New dependencies or abstractions are justified — nothing is over-engineered for the scope of the change.
- [ ] Public API surface (public methods, types, properties) is intentional — nothing is made public just for convenience or testability.
- [ ] The change does not introduce circular dependencies between namespaces or layers.


## 11. Security and Safety

- [ ] User-supplied input is validated before use.
- [ ] No secrets, credentials, or personal data are hardcoded or logged.
- [ ] Disposable resources (`IDisposable`) are wrapped in `using` statements or blocks.