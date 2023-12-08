# Individual Field Options

## UseMatchMode
A method to explicitly set the [value matching mode](../value-matching) to be used when filtering. Applicable only to properties of type `String` or integer numeric types.

- Arguments:

| Name | Type | Comment |
| :--- | :--- | :--- |
| mode | Enum of type `StringMatchMode` or `IntegerMatchMode` | - |

## UseSourceProperty
A method to explicitly set the property to be used when filtering and sorting.

- Arguments: 

| Name | Type | Comment |
| :--- | :--- | :--- |
| property | `Expression<Func<T, TMember>>` | Expression to the property |

## SearchBy
A method to explicitly set the search expression to be used when filtering.

- Arguments: 

| Name | Type | Comment |
| :--- | :--- | :--- |
| expression | `Expression<Func<T, string, bool>>` | Search expression |

## OrderBy
A method to explicitly set the sort expression to be used when sorting.

- Arguments: 

| Name | Type | Comment |
| :--- | :--- | :--- |
| expression | `Expression<Func<T, object>>` | Sort expression |

## EnableGlobalSearch
A method to enable a global search on this field. Applicable only to JavaScript datatables supporting global search option.