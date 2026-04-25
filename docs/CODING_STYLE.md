# CODING STYLE RULES


## File Formatting

**Rule 1. Use spaces for indentation, never tabs.**
All files must be indented with spaces. Tab characters are not permitted anywhere in the codebase. Each indentation level is exactly 4 spaces wide.

**Rule 2. End every file with a newline.**
Every file must have a newline character at the very end. Files that terminate on the last line of content — with no trailing newline — are non-compliant.

**Rule 3. Use CRLF line endings.**
All files must use Windows-style CRLF (carriage return + line feed) line endings. LF-only (Unix-style) endings are not permitted.

**Rule 4. Strip all trailing whitespace.**
No line may end with spaces or tabs after the last non-whitespace character. Trailing whitespace must be removed before committing.

**Rule 5. Encode all files in UTF-8.**
Every file in the project must be saved with UTF-8 character encoding. Other encodings (UTF-16, Latin-1, etc.) are not allowed.


## Naming

**Rule 6. Use PascalCase for types, members, and methods.**
Classes, structs, enums, interfaces, methods, properties, events, and delegates must all be named in PascalCase. Every word starts with an uppercase letter, with no separators. Example: `OrderService`, `GetUserById`, `IsCompleted`.

**Rule 7. Prefix all interface names with the letter I.**
Every interface name must begin with a capital I, followed by a PascalCase name. Example: `IRepository`, `IEventHandler`. This is enforced at suggestion severity.

**Rule 8. Prefix static fields with `s_` followed by camelCase.**
Static fields at any accessibility level must be named with the `s_` prefix and a camelCase name after it. Example: `s_instance`, `s_defaultTimeout`. This distinguishes static state from instance state at a glance.

**Rule 9. Prefix private and private-protected fields with `_` followed by camelCase.**
Private and private-protected instance fields must start with an underscore, followed by a camelCase name. Example: `_userId`, `_connectionString`. Public or internal fields do not get this prefix.

**Rule 10. Use camelCase for parameters and local variables.**
Method parameters and local variables inside method bodies must be named in plain camelCase, with no prefix. Example: `orderCount`, `isValid`, `httpClient`.


## Types and var

**Rule 11. Never use `var` — always write the explicit type.**
The `var` keyword is banned in all contexts: for built-in types, when the type is visually obvious, and in all other situations. Always write the full type name. Example: write `int count = 0`, not `var count = 0`.

**Rule 12. Use language keywords instead of BCL type names.**
Prefer C# keywords over their .NET Framework equivalents in all contexts — locals, parameters, and member access alike. Write `int`, `string`, `bool`, `object`, not `Int32`, `String`, `Boolean`, `Object`.


## Null Handling and Expressions

**Rule 13. Use null-coalescing operators instead of manual null checks in assignments.**
When assigning a fallback value for a potentially null expression, use `??` or `??=` rather than an if statement. Example: write `name = input ?? "default"`, not `if (input == null) name = "default"`.

**Rule 14. Use null-conditional operator `?.` instead of nested null guards.**
When accessing a member of an object that may be null, use `?.` to short-circuit rather than wrapping the access in an explicit null check. Example: write `user?.Address?.City`, not `if (user != null && user.Address != null) ...`

**Rule 15. Use pattern matching instead of is-checks followed by casts.**
Replace the pattern of checking type with `is` and then casting separately. Use `is Type variable` directly. Similarly, use `is not null` instead of `!= null`, and switch expressions instead of switch statements where a value is being mapped.

**Rule 16. Prefer ternary expressions over if/else for simple assignments and returns.**
When a variable is assigned one of two values, or when a method returns one of two values based on a condition, use the conditional (ternary) expression. Reserve if/else blocks for logic with side effects or multiple statements.


## Collections and Object Initialization

**Rule 17. Use collection initializers or collection expressions instead of sequential Add calls.**
Initialize collections inline at the point of construction. Write `new List<string> { "a", "b" }` or `["a", "b"]` rather than constructing an empty collection and calling `Add()` on separate lines.

**Rule 18. Use object initializers instead of property assignments after construction.**
Set object properties inline using object initializer syntax. Write `new Order { Id = 1, Total = 99 }` rather than constructing the object first and assigning properties on separate lines.


## Member Declarations

**Rule 19. Always write access modifiers explicitly on non-interface members.**
Every class member — fields, properties, methods, constructors, events — must carry an explicit access modifier (`public`, `private`, `internal`, `protected`, etc.). Do not rely on the default private visibility.

**Rule 20. Mark fields readonly whenever possible.**
Any field that is only assigned in the constructor and never mutated afterwards must be declared `readonly`. This applies to both instance fields and static fields where appropriate.

**Rule 21. Use expression bodies for properties, accessors, and indexers — not for methods or constructors.**
Single-expression properties, getters, setters, and indexers may use the `=>` arrow syntax. Methods and constructors must always use a full block body with braces, regardless of how short the body is.

**Rule 22. Prefer static local functions and static anonymous functions.**
Local functions and lambda expressions that do not capture any instance state should be declared `static`. This makes the absence of captured state explicit and avoids unintended allocations.


## Code Structure and Layout

**Rule 23. Always use braces for control-flow blocks, even single-line ones.**
`if`, `else`, `for`, `foreach`, `while`, `do`, and `using` blocks must always be wrapped in curly braces. Single-statement bodies without braces are not permitted, regardless of how short they are.

**Rule 24. Place opening braces on their own line (Allman style).**
The opening curly brace of every block — methods, classes, if statements, loops, object initializers — must appear on a new line, not at the end of the preceding line. `else`, `catch`, and `finally` also each begin on their own line.