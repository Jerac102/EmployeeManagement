---
applyTo: "src/EmployeeManagement.UI/**"
---
- Use Windows Forms conventions: one form per file with its `.Designer.cs`.
- Never edit `*.Designer.cs` by hand except for control declarations.
- Keep logic out of event handlers; call injected services or repositories.