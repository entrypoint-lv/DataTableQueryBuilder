# Built-in Value Matching Modes

The following value matching modes (strategies) are supported:

| The type of the property in source model | Available matching modes | Default mode |
| :--- | :--- | :--- |
| Integer numbers<br /><br />(sbyte, byte, short, ushort, int, uint, long, ulong) | `Equals` <br />`Contains` | `Equals`
| String | `Contains`<br />`StartsWith`<br />`EndsWith`<br />`SQLServerContains`<br />`SQLServerFreeText` | `Contains` |
| Boolean | `Equals` | `Equals` |
| Enum | `Equals` | `Equals` |
| DateTime | The matching mode is determined by the passed filtering value.<br /><br />If single date is passed - `Equals`<br />If date range is passed - `Between` | N/A |
| Any other type | Treated as a string type by executing `Property.ToString()` | `Contains` |