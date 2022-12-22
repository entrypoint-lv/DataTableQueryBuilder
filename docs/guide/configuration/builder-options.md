# Builder options

## DateFormat
A property to get/set date format used for value matching when filtering on properties of `DateTime` type.

- Type: `string`
- Default: `CultureInfo.InvariantCulture.DateTimeFormat.ShortDatePattern`

## ForField

A method to customize [individual field options](field-options).

Arguments:

| Name | Type | Comment |
| :--- | :--- | :--- |
| property | `Expression<Func<TDataTableFields, TMember>>` | Expression to the field |
| optionsAction | `Action<FieldOptions<TEntity>>` | An action to configure the field options |